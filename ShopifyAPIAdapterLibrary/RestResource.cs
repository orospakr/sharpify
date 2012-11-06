using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ShopifyAPIAdapterLibrary
{
    public class RestResource<T> {
        public ShopifyAPIClient Context { get; protected set; }

        public string Name { get; protected set; }

        public RestResource<T> Parent { get; protected set; }

        public NameValueCollection QueryParameters { get; protected set; }
        
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

        public RestResource(RestResource<T> parent, NameValueCollection queryParameters) {
            this.Context = parent.Context;
            this.Name = parent.Name;
            this.QueryParameters = queryParameters;
        }

        public NameValueCollection FullParameters() {
            var r = (Parent != null) ? new NameValueCollection(Parent.QueryParameters) : new NameValueCollection();
           // var r = Parent.QueryParameters);

            foreach(string key in QueryParameters.Keys) {
                r[key] = QueryParameters[key];
            }
            return r;
        }
        
        public string Path() {
            return ShopifyAPIClient.UriPathJoin(Context.AdminPath(), ShopifyAPIClient.Pluralize(Name));
        }
        
        public string InstancePath(string id) {
            return ShopifyAPIClient.UriPathJoin(Path(), id);
        }
        
        public async Task<T> Get(string id) {
            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), InstancePath(id), parameters: FullParameters(), requestBody: null);
            return Context.TranslateObject<T>(Name, resourceString);
        }

        public RestResource<T> Where(string field, string isEqualTo) {
            return new RestResource<T>(this, new NameValueCollection { { field, isEqualTo } });
        }

        public async Task<ICollection<T>> AsList() {
            // TODO Need an async/streamed version.
            // ICollection is okay for now. we buffer up all the answers before returning them.
            // actually, the thing this returns needs to offer

            // ANDREW start here: once finished dealing with silly untyped combinatoric impedances (lol), make sure that build up of nested RestResources with query parameter filters works (make test)
            // then add creation of query parameters with cute func-to-property-name c# idiom WhereAsync method
            // then go on to make my own IAsyncResourceSet (kind of like ICOllection, but appropriate for this and async) that RestResource inherits
            // then make the subresources (Shopify Article in Blog, for instance) by having the IAsyncQueryable set on the models which is actually a RestResource with the appropriate filter on (or, rather, concatenated paths)
            // then go on to make mutation work on those subresource IAsyncResourceSet work (just take the QueryParamter filter and naiively set those same fields on the incoming model POCO, perhaps? review list of query params for how appropriate this approach could be)
            // then go on to add Count to IAsyncResourceSet and RestResource

            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), Path(), parameters: null, requestBody: null);
            return Context.TranslateObject<List<T>>(Name, resourceString);
        }
    }
}
