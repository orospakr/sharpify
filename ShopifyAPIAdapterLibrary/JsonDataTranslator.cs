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
    public class HasAConverter<T> : JsonConverter where T: IResourceModel {
        public string TargetProperty { get;  set; }

        public HasAConverter(String targetProperty) {
            this.TargetProperty = targetProperty;
        }

        public override bool CanConvert(Type objectType)
        {
            // return objectType == typeof(string);
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var hasAId = reader.Value.ToString();

            // var prop = existingValue.GetType().GetProperty(TargetProperty);
            var placeholder = new HasADeserializationPlaceholder<T>(hasAId);
            return placeholder;
            // prop.SetValue(objectType, placeholder);
            // return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // var prop = value.GetType().GetProperty(TargetProperty);
            IHasA<T> hasa = (IHasA<T>)value;
            writer.WriteValue(hasa.Id);
        }
    }

    public class ShopifyRestStyleJsonResolver : DefaultContractResolver
    {
        public string ResourceName { get; private set; }

        public ShopifyRestStyleJsonResolver(string resourceName) :base (false)
        {
            // sadly, it is too expensive to leave the cacher enabled, hence why I pass false.
            ResourceName = resourceName;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            // C.O Krlos@SA http://stackoverflow.com/a/7275039
            return System.Text.RegularExpressions.Regex.Replace(
                propertyName, @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4").ToLower();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = new List<JsonProperty>(base.CreateProperties(type, memberSerialization));

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

                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IHasA<>))
                {
                    // TODO: proper underscoreize
                    prop.PropertyName = prop.PropertyName.ToLowerInvariant() + "_id";

                    // get type argument of IHasA
                    var hasATargetType = prop.PropertyType.GetGenericArguments();

                    // prop.Converter = new JsonConverter(
                    Type hasaConverterType = typeof(HasAConverter<>).MakeGenericType(hasATargetType);

                    // I was really hoping to avoid activator.createinstance... :(
                    var converter = Activator.CreateInstance(hasaConverterType, prop.UnderlyingName);

                    // for deserialization:
                    prop.MemberConverter = (JsonConverter)converter;
                    
                    // for serialization:
                    prop.Converter = (JsonConverter)converter;
                    return false;
                }

                return false;
            });
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HasADeserializationPlaceholder<T> : IHasA<T> where T : IResourceModel
    {
        public string Id { get; private set; }

        public HasADeserializationPlaceholder(string id)
        {
            Id = id;
        }

        private Exception Fail()
        {
            return new ShopifyConfigurationException("The second pass has not yet replaced this HasA placeholder with the real thing.");
        }

        public System.Threading.Tasks.Task<T> Get()
        {
            throw Fail();
        }

        public System.Threading.Tasks.Task<T> Set(T model)
        {
            throw Fail();
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