using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using ShopifyAPIAdapterLibrary.Models;

namespace ShopifyAPIAdapterLibrary
{
    public interface IParentableResource
    {
        IShopifyAPIClient Context { get; }

        string InstancePath(string id);

        Type GetModelType(); 
    }

    public class RestResource<T> : IParentableResource {
        public IShopifyAPIClient Context { get; protected set; }

        public string Name { get; protected set; }

        public RestResource<T> Parent { get; protected set; }

        public NameValueCollection QueryParameters { get; protected set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyAPIAdapterLibrary.RestResource`1"/> class.
        /// </summary>
        /// <param name='name'>
        /// The lowercase resource name, as it would appear in URIs
        /// </param>
        public RestResource(IShopifyAPIClient context, string name) {
            Context = context;
            Name = name;
        }

        public RestResource(RestResource<T> parent, NameValueCollection queryParameters) {
            this.Context = parent.Context;
            this.Parent = parent;
            this.Name = parent.Name;
            this.QueryParameters = queryParameters;
        }

        public NameValueCollection FullParameters() {
            var r = (Parent != null) ? new NameValueCollection(Parent.FullParameters()) : new NameValueCollection();
           // var r = Parent.QueryParameters);

            if (QueryParameters != null)
            {
                foreach (string key in QueryParameters.Keys)
                {
                    r[key] = QueryParameters[key];
                }
            }
            return r;
        }
        
        public virtual string Path() {
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

        public RestResource<T> Where(Expression<Func<T, object>> propertyLambda, string isEqualTo)
        {
            // http://merrickchaffer.blogspot.ca/2012/03/accessing-properties-in-c-using-lambda.html
            var memberExpression = propertyLambda.Body as MemberExpression;
            if (memberExpression == null) {
                throw new ShopifyConfigurationException("Expression specified to Where() did not specify a type member.");
            }
            var prop = memberExpression.Member as PropertyInfo;
            if(prop == null) {
                throw new ShopifyConfigurationException("Expression specified to Where() did not specify a property.");
            }
            var type = typeof(T);
            if (type != prop.ReflectedType && !type.IsSubclassOf(prop.ReflectedType))
            {
                throw new ShopifyConfigurationException("Expression specified to Where() specified a property on a different class than the model object type specified to RestResource.");
            }
            return Where(prop.Name, isEqualTo);
        }

        private T SetSubResourceProxies(T modelInstance)
        {
            // TODO
            // what will the proxies be?
            // they're pretty much just RestResource, except that instead of using a new query filter addition,
            // they'll use a new addition to the path
            // how about a RestResource where the parent passed to the constructor is the model POCO?
            return default(T);
        }

        public async Task<ICollection<T>> AsList() {
            // TODO Need an async/streamed version.
            // ICollection is okay for now. we buffer up all the answers before returning them.
            // actually, the thing this returns needs to offer

            // ANDREW start here: once finished dealing with silly untyped combinatoric
            // impedances(lol), make sure that build up of nested RestResources with query
            // parameter filters works (make test) DONE

            // then add creation of query parameters with cute func-to-property-name c#
            // idiom WhereAsync method DONE

            // Min/max updated_at and created_at where() things

            // then go on to make my own IAsyncResourceSet (kind of like ICollection, but
            // appropriate for this and also async) that RestResource inherits.

            // then make the subresources (Shopify Article in Blog, for instance) by having
            // the IAsyncQueryable set on the models which is actually a RestResource with
            // the appropriate filter on (or, rather, concatenated paths) DONE

            // then go on to make mutation work on those subresource IAsyncResourceSet work
            // (just take the QueryParamter filter and naiively set those same fields on the
            // incoming model POCO, perhaps? review list of query params for how appropriate
            // this approach could be)

            // Inline subresources?  We do want it to work.
            // Does Shopify have any of these?

            // then go on to add Count to IAsyncResourceSet and RestResource

            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), Path(), parameters: null, requestBody: null);
            return Context.TranslateObject<List<T>>(ShopifyAPIClient.Pluralize(Name), resourceString);
        }


        public Type GetModelType()
        {
            return typeof(T);
        }
    }

    public class SubResource<T> : RestResource<T>
    {
        public IParentableResource ParentResource;
        public IResourceModel ParentInstance;
 
        public SubResource(IParentableResource parent, IResourceModel parentInstance, string name) : base(parent.Context, name)
        {
            if (!parent.GetModelType().IsAssignableFrom(parentInstance.GetType()))
            {
                throw new ShopifyConfigurationException("Parent model instance provided to SubResource must be appropriate to the provided Parent Resource.");
            }
            if (parentInstance.Id == null || parentInstance.Id.Length == 0)
            {
                throw new ShopifyConfigurationException("Parent model instance provided for subresource has null id, which would lead to untenable subresource URIs.");
            }
            ParentResource = parent;
            ParentInstance = parentInstance;
        }

        public override string Path()
        {
            return ShopifyAPIClient.UriPathJoin(ParentResource.InstancePath(ParentInstance.Id), ShopifyAPIClient.Pluralize(Name));
        }
    }
}
