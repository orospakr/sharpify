using Sharpify.Models;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sharpify
{

    public interface IShopifyAPIContext
    {
        /// <summary>
        /// The base path of the REST interface.
        /// </summary>
        string AdminPath();

        /// <summary>
        /// Make the specified HTTP call and receive late-bindable JObjects deserialized from the response body.
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="path">HTTP path</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="data">Data to include in request body</param>
        /// <returns>Deserialized to JObject graph, suitable for use with dynamic.</returns>
        Task<object> Call(System.Net.Http.HttpMethod method, string path, System.Collections.Specialized.NameValueCollection parameters = null, object data = null);

        /// <summary>
        /// Make the specified API call and get the returned response body as a string.
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="acceptType">Accept: HTTP header, for specifying preferred content type of results</param>
        /// <param name="path">HTTP path</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="requestBody">Data to include in request body</param>
        /// <returns>Response body</returns>
        Task<string> CallRaw(System.Net.Http.HttpMethod method, System.Net.Http.Headers.MediaTypeHeaderValue acceptType, string path, System.Collections.Specialized.NameValueCollection parameters, string requestBody);

        /// <summary>
        /// Call HTTP DELETE on the specified path
        /// </summary>
        Task<object> Delete(string path);

        /// <summary>
        /// Call HTTP GET on the specified path, get late-bindable JObjects from response
        /// </summary>
        Task<object> Get(string path);

        /// <summary>
        /// Call HTTP GET on the specified path, get late-bindable JObjects from response
        /// </summary>
        /// <param name="callParams">Query string parameters</param>
        Task<object> Get(string path, System.Collections.Specialized.NameValueCollection callParams);

        /// <summary>
        /// The Accept: content type that this API Context will use by default.
        /// </summary>
        MediaTypeHeaderValue GetRequestContentType();

        /// <summary>
        /// Generate a Sharpify developer friendly exception from the given response.
        /// </summary>
        ShopifyException HandleError(System.Net.Http.HttpResponseMessage response, string reason);

        /// <summary>
        /// Call HTTP POST on the specified path with the given serializable object
        /// </summary>
        Task<object> Post(string path, object data);

        /// <summary>
        /// Call HTTP PUT on the specified path with the given serializable object
        /// </summary>
        Task<object> Put(string path, object data);
        
        /// <summary>
        /// Deserialize an object using the Context'sapplication-specific
        /// translator.
        /// </summary>
        T TranslateObject<T>(string subfieldName, string content);

        /// <summary>
        /// Serialize an object using the Context's application-specific translator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subfieldName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        string ObjectTranslate<T>(String subfieldName, T model);

        /// <summary>
        /// Get a RestResource<typeparamref name="T"/> object for type-safe access
        /// for the given model type to the REST interface.
        /// </summary>
        RestResource<T> GetResource<T>() where T : IResourceModel, new();
    }
}
