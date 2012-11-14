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

namespace ShopifyAPIAdapterLibrary
{
    public class ShopifyRestStyleJsonResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            return new List<JsonProperty>(from p in properties where !(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ISubResource<>)) select p);
        }
    }

    /// <summary>
    /// This class is used to translate to and from C# object and JSON strings 
    /// </summary>
    public class JsonDataTranslator : IDataTranslator
    {
        private JsonSerializerSettings Settings { get; set; }

        public JsonSerializer Serializer { get; set; }

        public JsonDataTranslator()
        {
            Settings = new JsonSerializerSettings() { ContractResolver = new ShopifyRestStyleJsonResolver() };
            Serializer = JsonSerializer.Create(Settings);
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
            JObject decoded = (JObject)JsonConvert.DeserializeObject(content, Settings);

            if (decoded[subfieldName] == null)
            {
                throw new ShopifyException("Response does not contain field: " + subfieldName);
            }
            return decoded[subfieldName].ToObject<T>();
        }

        public string ResourceEncode<T>(string subFieldName, T model)
        {
            var json = new JObject();
            var wrappedModel = JObject.FromObject(model, Serializer);
            json.Add(subFieldName, wrappedModel);
            return JsonConvert.SerializeObject(json, Settings);
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