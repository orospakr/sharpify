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
    public class HasOneConverter<T> : JsonConverter where T: IResourceModel {

        public HasOneConverter() {
        }

        public override bool CanConvert(Type objectType)
        {
            // return objectType == typeof(string);
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var hasAIdValue = reader.Value;
            if (hasAIdValue == null)
            {
                // https://trello.com/card/make-a-test-for-incoming-has-one-id-fields-that-are-null/50a1c9c990c4980e0600178b/31
                return null;
            }
            var hasOneId = Int32.Parse(hasAIdValue.ToString());

            var placeholder = new HasOneDeserializationPlaceholder<T>(hasOneId);
            return placeholder;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // var prop = value.GetType().GetProperty(TargetProperty);
            IHasOne<T> hasOne = (IHasOne<T>)value;
            writer.WriteValue(hasOne.Id);
        }
    }

    /// <summary>
    /// NOT THREAD SAFE
    /// 
    /// should be reinstantiated for every new deserialization task.
    /// </summary>
    public class ResourceConverter : JsonConverter
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

        public ResourceConverter()
        {
        }

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
            if(typeof(IResourceModel).IsAssignableFrom(objectType)) {
                // hooray for side effects :(
                if (RecursionAvoidance)
                {
                    RecursionAvoidance = false;
                    return false;
                } else {
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
            IResourceModel model = (IResourceModel) serializer.Deserialize(reader, objectType);
            RecursionAvoidance = false;

            model.Reset();
            return model;
        }

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

    public class HasOneInlineConverter<T> : JsonConverter where T : IResourceModel
    {

        public HasOneInlineConverter() {
        }

        public override bool CanConvert(Type objectType) {
            // TODO: really should be checking the type...
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var model = serializer.Deserialize<T>(reader);
            // TODO: this is the second place that models are directly serialized at.
            // we have to use the same arrangement here as in the ResourceConverter.
            return new HasOneInline<T>(model);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var hasOneInline = (HasOneInline<T>)value;
            serializer.Serialize(writer, hasOneInline.Model);
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

            var hasOneInlineProperties = new List<JsonProperty>();

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

                    // we will mutate the original property (automatically created by the default
                    // json.net resolver) such that it handles the task of an addressed has_one
                    // ("thinger_id").
                    prop.PropertyName =  underscorized + "_id";

                    // get type argument of IHasOne
                    var hasOneTargetType = prop.PropertyType.GetGenericArguments();

                    // create an additional property for the inline deserialization case
                    JsonProperty inlineProperty = new JsonProperty() {
                        PropertyType = prop.PropertyType,
                        Ignored = false,
                        PropertyName = underscorized,
                        UnderlyingName = prop.UnderlyingName,
                        DeclaringType = prop.DeclaringType,
                        ValueProvider = prop.ValueProvider,
                        Readable = false,  // the inline property descriptor should not be used for serialization.
                        Writable = true
                    };

                    Type hasOneInlineConverterType = typeof(HasOneInlineConverter<>).MakeGenericType(hasOneTargetType);

                    // make an instance of the converter intended for deserializing inline
                    // has one resources.
                    var inlineConverter = Activator.CreateInstance(hasOneInlineConverterType);
                    
                    // deserialize inline properies using the inline converter.
                    inlineProperty.MemberConverter = (JsonConverter)inlineConverter;

                    // we add all of the created inline property descriptors after,
                    // in order to avoid modifying the collection while RemoveAll is
                    // running
                    hasOneInlineProperties.Add(inlineProperty);

                    // Adjust the originally populated property to handle the _id (not inline version):

                    // build the type of the necessary HasOneConverter with the parameter of this has one target type
                    Type hasOneConverterType = typeof(HasOneConverter<>).MakeGenericType(hasOneTargetType);

                    // I was really hoping to avoid activator.createinstance... :(
                    var converter = Activator.CreateInstance(hasOneConverterType);

                    // for deserialization:
                    prop.MemberConverter = (JsonConverter)converter;
                    
                    // for serialization:
                    prop.Converter = (JsonConverter)converter;

                    prop.ShouldSerialize = (obj) =>
                    {
                        IResourceModel model = (IResourceModel)obj;
                        return (model.IsFieldDirty(prop.UnderlyingName));
                    };

                    // the inlines should get the same serialization behaviour (to _id) as
                    // the subresource version.  so, give it the same converter.
                    // inlineProperty.Converter = (JsonConverter)converter;
                    return false;
                }

                // TODO ignore non-modified fields
                // *on serialization* only.

                if (typeof(IResourceModel).IsAssignableFrom(type))
                {
                    // only ignore unchanged fields if they're "primitive" types, like ints, int?s, strings,
                    // and not the main ID
                    if((prop.PropertyType.IsPrimitive || prop.PropertyType.IsAssignableFrom(typeof(string))) && prop.PropertyName != "id")
                    {
                        prop.ShouldSerialize = (obj) =>
                        {
                            IResourceModel model = (IResourceModel)obj;
                            return (model.IsFieldDirty(prop.UnderlyingName));
                        };
                    }
                }

                return false;
            });

            properties.AddRange(hasOneInlineProperties);
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

    public class HasOneInline<T> : IHasOneInlineUntyped, IHasOne<T> where T : IResourceModel
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