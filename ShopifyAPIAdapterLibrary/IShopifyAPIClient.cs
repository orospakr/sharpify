using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary
{
    public interface IShopifyAPIClient
    {
        RestResource<T> GetResource<T>() where T : IResourceModel, new();
        string AdminPath();
        Task<object> Call(System.Net.Http.HttpMethod method, string path, System.Collections.Specialized.NameValueCollection parameters = null, object data = null);
        Task<string> CallRaw(System.Net.Http.HttpMethod method, System.Net.Http.Headers.MediaTypeHeaderValue acceptType, string path, System.Collections.Specialized.NameValueCollection parameters, string requestBody);
        Task<object> Delete(string path);
        Task<object> Get(string path);
        Task<object> Get(string path, System.Collections.Specialized.NameValueCollection callParams);
        Task<System.Collections.Generic.ICollection<ShopifyAPIAdapterLibrary.Models.Product>> GetProducts();
        MediaTypeHeaderValue GetRequestContentType();
        ShopifyException HandleError(System.Net.Http.HttpResponseMessage response, string reason);
        Task<object> Post(string path, object data);
        Task<object> Put(string path, object data);
        
        [Obsolete]
        T TranslateObject<T>(string subfieldName, string content);

        [Obsolete]
        string ObjectTranslate<T>(String subfieldName, T model);
    }
}
