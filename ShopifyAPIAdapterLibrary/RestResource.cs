using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ShopifyAPIAdapterLibrary
{
    public class RestResource<T> {
        public ShopifyAPIClient Context;
        public string Name;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyAPIAdapterLibrary.RestResource`1"/> class.
        /// </summary>
        /// <param name='name'>
        /// The lowercase resource name, as it would appear in URIs
        /// </param>
        public RestResource(ShopifyAPIClient context, string name) {
            Context = context;
            Name = name;
        }
        
        public string Path() {
            return ShopifyAPIClient.UriPathJoin(Context.AdminPath(), ShopifyAPIClient.Pluralize(Name));
        }
        
        public string InstancePath(string id) {
            return ShopifyAPIClient.UriPathJoin(Path(), id);
        }
        
        public async Task<T> Get(string id) {
            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), InstancePath(id), null);
            return Context.TranslateObject<T>(Name, resourceString);
        }
    }
}
