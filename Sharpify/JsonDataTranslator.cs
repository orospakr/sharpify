using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using ShopifyAPIAdapterLibrary.Models;
using System.ComponentModel;

namespace ShopifyAPIAdapterLibrary
{
    /// <summary>
    /// NOT THREAD SAFE
    /// 
    /// should be reinstantiated for every new deserialization task.
    /// </summary>
    public abstract class WrappedConverter<T> : JsonConverter
    {
        /// <summary>
        /// We are currently in the midst of asking the stock JsonConverter
        /// to deserialize (using the default approach) the resource model.
        /// 
        /// Of course, default policy is to invoke ResourceConverter again,
        /// so, we break for that initial re-entering by temporarily claiming
        /// that we cannot handle deserializing RestResources, thus allowing
        /// json.net to continue has normal.
        /// 
        /// Ideally, JsonConverter should add some affordance for having
        /// a JsonConverter call serializer.(De)serialize() and 
        /// </summary>
        private bool RecursionAvoidance;

        /// <summary>
        /// In order to work around an interesting deficiency in json.net,
        /// we need to have ResourceConverter refuse to deserialize the
        /// content of this IResourceModel, otherwise, the ResourceConverter
        /// will persistently invoke itself inadverdently forever.
        /// 
        /// It appears, from my cursory investigation, that json.net
        /// does not (yet!) cache this information.  As soon as it does,
        /// the assumption I make in this workaround code.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            if (typeof(T).IsAssignableFrom(objectType))
            {
                // hooray for side effects :(
                if (RecursionAvoidance)
                {
                    RecursionAvoidance = false;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (RecursionAvoidance)
            {
                throw new Exception("Crap.  Collision Avoidance should never be true here.");
            }
            RecursionAvoidance = true;
            T model = (T)serializer.Deserialize(reader, objectType);
            RecursionAvoidance = false;
            PostProcess(model);
            return model;
        }

        public abstract void PostProcess(T obj);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (RecursionAvoidance)
            {
                throw new Exception("Crap.  Collision Avoidance should never be true here.");
            }
            RecursionAvoidance = true;
            serializer.Serialize(writer, value);
            RecursionAvoidance = false;
        }
    }

    public class ResourceConverter : WrappedConverter<IResourceModel>
    {
        public override void PostProcess(IResourceModel model)
        {
            model.Reset();
            model.SetExisting();
        }
    }

    public class FragmentConverter : WrappedConverter<Fragment>
    {
        public override void PostProcess(Fragment frag)
        {
            frag.Reset();
        }
    }

    public class FragmentListConverter : WrappedConverter<UntypedDirtiableList>
    {
        public override void PostProcess(UntypedDirtiableList fragList)
        {
            fragList.Reset();
        }
    }


    public class ShopifyRestStyleJsonResolver : DefaultContractResolver
    {
        public string ResourceName { get; private set; }

        public ShopifyRestStyleJsonResolver(string resourceName) :base (false)
        {
            // sadly, can't leave the cacher enabled because different instances of the resolver
            // generate different results, hence why I pass false.
            ResourceName = resourceName;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return ShopifyAPIClient.Underscoreify(propertyName);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = new List<JsonProperty>(base.CreateProperties(type, memberSerialization));

            var hasOneAdIdProperties = new List<JsonProperty>();

            properties.RemoveAll((prop) => {
                // do not attempt to (de)serialize IHasManys
                if(prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IHasMany<>)) return true;

                // if we're (de)serializing the top-level Container object, perform our wrapper-object
                // name transformation
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Container<>))
                {
                    // our Container type only has one property (the specially named per-resource property),
                    // so, we just presume to match all properties in Container.
                    prop.PropertyName = ResourceName;
                    return false;
                }

                // is property a HasOne?
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IHasOne<>))
                {
                    var underscorized = ShopifyAPIClient.Underscoreify(prop.PropertyName);

                    // get type argument of IHasOne
                    var hasOneTargetType = prop.PropertyType.GetGenericArguments();

                    // I was really hoping to avoid activator.createinstance... :(

                    Type hasOneInlineConverterType = typeof(HasOneInlineJsonConverter<>).MakeGenericType(hasOneTargetType);
                    var hasOneInlineConverter = Activator.CreateInstance(hasOneInlineConverterType);

                    Type hasOneAsIdConverterType = typeof(HasOneAsIdJsonConverter<>).MakeGenericType(hasOneTargetType);
                    var hasOneAsIdConverter = Activator.CreateInstance(hasOneAsIdConverterType);

                    // set the standard HasOneConverter
                    // TODO if we can de-genericify hasOneConverter, just set it up globally and get rid of this dynamic instantiation
                    prop.Converter = (JsonConverter)hasOneInlineConverter;
                    prop.MemberConverter = (JsonConverter)hasOneInlineConverter;

                    // create an additional property for the as-id deserialization (deserialization *only*) case
                    JsonProperty asIdProperty = new JsonProperty() {
                        PropertyType = prop.PropertyType,
                        Ignored = false,
                        PropertyName = underscorized + "_id",
                        UnderlyingName = prop.UnderlyingName,
                        DeclaringType = prop.DeclaringType,
                        ValueProvider = prop.ValueProvider,
                        Readable = true,
                        Writable = true,
                        // Use the as-id converter
                        Converter = (JsonConverter)hasOneAsIdConverter,
                        MemberConverter = (JsonConverter)hasOneAsIdConverter
                    };

                    // make props:

                    // outgoing inline
                    // outgoing as id
                    // incoming inline
                    // incoming as id

                    // the outgoings should have ShouldSerializes that do the HasOne type check IN addition to the usual dirty check
                    // the incomings can be naiive, since the presence of either field in the incoming json makes the selection
        
                    // we should only serialize such properties as have changed.
                    prop.ShouldSerialize = (obj) =>
                    {
                        // TODO: https://trello.com/card/isfielddirty-if-called-on-inline-hasone-fields-should-return-true-if-the-contained-resource-model-is-at-all-dirty/50a1c9c990c4980e0600178b/58
                        var hasOneInstance = prop.ValueProvider.GetValue(obj);
                        if ((hasOneInstance as IHasOneInlineUntyped) == null) return false;
                        IResourceModel model = (IResourceModel)obj;
                        return (model.IsFieldDirty(prop.UnderlyingName));
                    };

                    asIdProperty.ShouldSerialize = (obj) =>
                    {
                        var hasOneInstance = prop.ValueProvider.GetValue(obj);
                        if ((hasOneInstance as IHasOneAsIdUntyped) == null) return false;
                        IResourceModel model = (IResourceModel)obj;
                        return (model.IsFieldDirty(asIdProperty.UnderlyingName));
                    };

                    // we add all of the created as=id property descriptors after,
                    // in order to avoid modifying the collection while RemoveAll is
                    // running
                    hasOneAdIdProperties.Add(asIdProperty);

                    return false;
                }

                if (typeof(IGranularDirtiable).IsAssignableFrom(type))
                {
                    // the main ID field should always be included
                    if(prop.PropertyName != "id")
                    {
                        prop.ShouldSerialize = (obj) =>
                        {
                            IGranularDirtiable model = (IGranularDirtiable)obj;
                            return (model.IsFieldDirty(prop.UnderlyingName));
                        };
                    }
                }

                return false;
            });

            properties.AddRange(hasOneAdIdProperties);
            return properties;
        }
    }

    /// <summary>
    /// Rails-style resource representations are typically wrapped in a JSON object
    /// with a single property with the name of the contained resource.
    /// 
    /// In order to use the type-safe JsonConvert API (and have our IContractResolver
    /// get used) we have to arrange to have a (de)serializable object type getting
    /// a dynamic name to use for that single field at runtime.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Container<T>
    {
        public T Resource { get; set; }
    }

    /// <summary>
    /// Object that remembers the _id of the single subresource temporarily until
    /// code in RestResource replaces it with a live fetcher.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HasOneDeserializationPlaceholder<T> : IHasOnePlaceholderUntyped, IHasOne<T> where T : IResourceModel
    {
        public int Id { get; private set; }

        public HasOneDeserializationPlaceholder(int id)
        {
            Id = id;
        }

        private Exception Fail()
        {
            return new ShopifyConfigurationException("The second pass has not yet replaced this HasOne placeholder with the real thing.");
        }

        public System.Threading.Tasks.Task<T> Get()
        {
            throw Fail();
        }
    }

    public interface IHasOneInlineUntyped
    {
        IResourceModel GetUntypedInlineModel();
    }

    public class HasOneInline<T> : IDirtiable, IHasOneInlineUntyped, IHasOne<T> where T : IResourceModel
    {
        public int Id
        {
            get
            {
                return Model.Id.Value;
            }
        }

        public T Model { get; private set; }

        public HasOneInline(T model)
        {
            // TODO: this will have to change when we support creating new resources that
            // should post with a similarly new inline
            if (model.Id == null) throw new ShopifyUsageException("HasOneInline must be given a model that has a set ID.");
            Model = model;
        }

        public async System.Threading.Tasks.Task<T> Get()
        {
            return Model;
        }

        public IResourceModel GetUntypedInlineModel() {
            return Model;
        }

        public bool IsClean()
        {
            if(Model == null) return true;
            return Model.IsClean();
        }
    }

    /// <summary>
    /// This class is used to translate to and from C# objects and JSON strings 
    /// </summary>
    public class JsonDataTranslator : IDataTranslator
    {

        public JsonDataTranslator()
        {
        }

        /// <summary>
        /// This should be created for every invocation of use.  ResourceConverter
        /// in particular must do things that compromise thread safety, in order
        /// to avoid an infinite recursion loop.
        /// </summary>
        private JsonSerializerSettings CreateSerializerSettings(string topLevelResourceName)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new ShopifyRestStyleJsonResolver(topLevelResourceName) };
            settings.Converters.Add(new ResourceConverter());
            settings.Converters.Add(new FragmentConverter());
            settings.Converters.Add(new FragmentListConverter());
            return settings;
        }

        /// <summary>
        /// Given a C# object, return a JSON string that can be used by the Shopify API
        /// </summary>
        /// <param name="data">a c# object that maps to a JSON object</param>
        /// <returns>JSON string</returns>
        public string Encode(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Given a JSON String, return a corresponding C# object
        /// </summary>
        /// <param name="encodedData">JSON string return from the Shopify API</param>
        /// <returns>c# object corresponding to the JSON data return from the Shopify API</returns>
        public object Decode(string encodedData)
        {
            return JObject.Parse(encodedData);
        }

        public T ResourceDecode<T>(String subfieldName, String content)
        {
            Container<T> decoded = JsonConvert.DeserializeObject<Container<T>>(content, CreateSerializerSettings(subfieldName));
            return decoded.Resource;
        }

        public string ResourceEncode<T>(string subFieldName, T model)
        {
            Container<T> wrapped = new Container<T>() { Resource = model };

            return JsonConvert.SerializeObject(wrapped, CreateSerializerSettings(subFieldName));
        }

        /// <summary>
        /// The content type used by JSON
        /// </summary>
        /// <returns></returns>
        public MediaTypeHeaderValue GetContentType()
        {
            return ContentType;
        }

        /// <summary>
        /// The content type used by JSON
        /// </summary>
        public static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue("application/json");
    }
}