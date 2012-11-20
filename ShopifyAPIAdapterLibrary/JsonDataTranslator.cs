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

    public class HasOneInlineConverter<T> : JsonConverter where T : IResourceModel
    {
        public HasOneInlineConverter() {
        }

        public override bool CanConvert(Type objectType) {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var model = serializer.Deserialize<T>(reader);
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
                // do not attempt to serialize IHasManys
                if(prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IHasMany<>)) return true;

                // if we're serializing the top-level Container object, perform our wrapper-object
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
                        Readable = true,
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

                    // the inlines should get the same serialization behaviour (to _id) as
                    // the subresource version.  so, give it the same converter.
                    inlineProperty.Converter = (JsonConverter)converter;
                    return false;
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
    public class HasOneDeserializationPlaceholder<T> : IHasOne<T> where T : IResourceModel
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

        public void Set(T model)
        {
            throw Fail();
        }
    }

    public class HasOneInline<T> : IHasOne<T> where T : IResourceModel
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

        public void Set(T model)
        {
            Model = model;
        }
    }

    /// <summary>
    /// This class is used to translate to and from C# object and JSON strings 
    /// </summary>
    public class JsonDataTranslator : IDataTranslator
    {
        public JsonDataTranslator()
        {
        }

        private JsonSerializerSettings CreateSerializerSettings(string topLevelResourceName)
        {
            return new JsonSerializerSettings() { ContractResolver = new ShopifyRestStyleJsonResolver(topLevelResourceName) }; 
        }

        private JsonSerializer CreateSerializer(string topLevelResourceName)
        {
            return JsonSerializer.Create(CreateSerializerSettings(topLevelResourceName));
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

            //var typeDesc = new TypeDescriptionProvider();
            //container.Resource.GetType().TypeDescri

            //JObject decoded = (JObject)JsonConvert.DeserializeObject(content, Settings);

            //if (decoded[subfieldName] == null)
            //{
            //    throw new ShopifyException("Response does not contain field: " + subfieldName);
            //}

            Container<T> decoded = JsonConvert.DeserializeObject<Container<T>>(content, CreateSerializerSettings(subfieldName));
            return decoded.Resource;
        }

        public string ResourceEncode<T>(string subFieldName, T model)
        {
            Container<T> wrapped = new Container<T>() { Resource = model };
            //var json = new JObject();
            //var wrappedModel = JObject.FromObject(model, Serializer);
            //json.Add(subFieldName, wrappedModel);
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