using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using ShopifyAPIAdapterLibrary.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ShopifyAPIAdapterLibrary
{
    /// <summary>
    /// Untyped access to a RestResource.
    /// 
    /// Largely intended for having a means for ShopifyApiClient
    /// to have a list of resources by hosted IResourceModel type.
    /// </summary>
    public interface IUntypedResource
    {
        Type GetModelType();

        Task<int> Count();

        string Path();

        string InstancePath(int p);
    }

    public interface IHasMany<T> : IUntypedResource
    {
        Task<T> Get(int id);

        Task Create(T model);

        Task Update(T model);
    }

    public interface IHasOneUntyped
    {
        int Id { get; }
    }

    public interface IHasOne<T> : IHasOneUntyped where T : IResourceModel
    {
        // Retrieve the resource associated with the parent object.
        Task<T> Get();

        /// <summary>
        /// Sets the instance of the "has one" relationship to the provided model.
        /// 
        /// Ie., the "$model_name_id" json field will change.
        /// 
        /// This method does *not* directly mutate any state on the server.  Update or
        /// Save the host model with its Resource for that.
        /// </summary>
        void Set(T model);
    }

    public interface IParentableResource
    {
        IShopifyAPIClient Context { get; }

        string InstanceOrVerbPath(string id);

        Type GetModelType();
    }

    public interface IResourceView<T> : IUntypedResource
        where T : IResourceModel, new() 
    {
        /// <summary>
        /// Fetch all the records matched by this resource, and asynchronously call
        /// the provided callback with each.
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        Task Each(Action<T> cb);

        /// <summary>
        /// Fetch and buffer the entire set of records matched by this resource,
        /// and return them all together.
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> AsListUnpaginated();

        /// <summary>
        /// Returns a new view of this resource filtered to a specific page (for
        /// paginated resources).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        IResourceView<T> Page(int p);

        IResourceView<T> Where(Expression<Func<T, object>> propertyLambda, string isEqualTo);

        IResourceView<T> Where(string field, string isEqualTo);

        NameValueCollection FullParameters();
    }

    public class RestResource<T> : IResourceView<T>, IParentableResource where T : IResourceModel, new()
    {
        public IShopifyAPIClient Context { get; protected set; }

        public string Name { get; protected set; }

        /// <summary>
        /// Not to be confused with the parenting of SubResources,
        /// this relation is for chaining multiple RestResource instances
        /// pointing to the same effective Resource path on the REST
        /// service with separate filter/query parameters.
        /// 
        /// Clear as mud, right?
        /// </summary>
        public RestResource<T> Parent { get; protected set; }

        public NameValueCollection QueryParameters { get; protected set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyAPIAdapterLibrary.RestResource`1"/> class.
        /// </summary>
        /// <param name='name'>
        /// The lowercase resource name, as it would appear in URIs.
        /// Default is underscorized from the model type.
        /// </param>
        // TODO: put name back as an optional
        public RestResource(IShopifyAPIClient context, string name = null) {
            Context = context;

            if (name == null)
            {
                Name = ShopifyAPIClient.Underscoreify(typeof(T).Name);
            }
            else
            {
                Name = name;
            }
        }

        // protected RestResource(IShopifyAPIClient context, string name, bool registerAsToplevel

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
        
        public string InstanceOrVerbPath(string id) {
            return ShopifyAPIClient.UriPathJoin(Path(), id);
        }

        public string InstancePath(int id)
        {
            return InstanceOrVerbPath(id.ToString());
        }

        private T TranslateObject(string subfieldName, string resourceString) {
            var translated = Context.TranslateObject<T>(Name, resourceString);

            return PlaceResourceProxesOnModel(translated);
        }

        public async Task<int> Count()
        {
            var countString = await Context.CallRaw(HttpMethod.Get,
                Context.GetRequestContentType(),
                InstanceOrVerbPath("count"), FullParameters(), null);
            return Context.TranslateObject<int>("count", countString);
        }
        
        public async Task<T> Get(int id) {
            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), InstancePath(id), parameters: FullParameters(), requestBody: null);
            return TranslateObject(Name, resourceString);
        }

        public async Task Create(T model) {
            var jsonString = Context.ObjectTranslate<T>(Name, model);
            await Context.CallRaw(HttpMethod.Post, Context.GetRequestContentType(),
                Path(), null, jsonString);
        }

        public async Task Update(T model)
        {
            if (model.Id == null)
            {
                throw new ShopifyUsageException("Model must have an ID in order to put an update.");
            }
            var resourceString = Context.ObjectTranslate<T>(Name, model);
            await Context.CallRaw(HttpMethod.Put, Context.GetRequestContentType(),
                InstanceOrVerbPath(model.Id.ToString()), null, resourceString);
        }

        public IResourceView<T> Where(string field, string isEqualTo) {
            return new RestResource<T>(this, new NameValueCollection { { field, isEqualTo } });
        }

        public IResourceView<T> Page(int page)
        {
            return this.Where("page", page.ToString());
        }

        private T PlaceResourceProxesOnModel(T model)
        {
            // add HasMany proxies to the model instance
            var r = from p in typeof(T).GetProperties() where p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == (typeof(IHasMany<>)) select p;
            foreach (var prop in r)
            {
                // get resource model type of the subresource field
                var subResourceModelType = prop.PropertyType.GetGenericArguments()[0];
                var baseSubresourceType = typeof(SubResource<>);
                var resourceType = baseSubresourceType.MakeGenericType(new Type[] { subResourceModelType });

                // I was hoping to avoid using Activator in this project, but no such luck
                var subResourceInstance = Activator.CreateInstance(resourceType, new Object[] { this, model, subResourceModelType.Name });

                prop.SetValue(model, subResourceInstance);
            }

            // replace the HasOne placeholders (which tell us the ID that was on the _id) field
            // with the live ones

            var hasaPlaceholders = from p in typeof(T).GetProperties() where p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == (typeof(IHasOne<>)) select p;
            foreach (var placeholderProp in hasaPlaceholders)
            {
                // get the ID from the placeholder
                var placeholder = (IHasOneUntyped)placeholderProp.GetValue(model);
                if (placeholder == null)
                {
                    continue;
                }

                // get resource model type from the has_a property
                var subResourceModelType = placeholderProp.PropertyType.GetGenericArguments()[0];
                var baseSubresourceType = typeof(SingleInstanceSubResource<>);
                var resourceType = baseSubresourceType.MakeGenericType(new Type[] { subResourceModelType });

                // I was hoping to avoid using Activator in this project, but no such luck
                var subResourceInstance = Activator.CreateInstance(resourceType, this.Context, placeholder.Id);

                placeholderProp.SetValue(model, subResourceInstance);
            }

            return model;
        }

        public IResourceView<T> Where(Expression<Func<T, object>> propertyLambda, string isEqualTo)
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

        public async Task<IList<T>> AsListUnpaginated() {
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

            // TODO: inline singles (has_a): actually, this should Just Work already
            // TODO: inline multiples (has_many)

            // TODO: paging.  this should be abstracted behind, ienumerable style

            // what approach I should use? if an inline is found, should
            // I make an entirely different IAsyncCollection and set it
            // on the field OR...
            // ... should I have RestResource have a notion of a cached/
            // preloaded list?
            // OR... when deserializing, as it is, inline list models should be
            // automatically added to a simple ICollection by json.net
            // perhaps we should only put our proxies in *if* they come up null
            // ... but wait, if I do that then:
            //     -- when serializing the top-level object, the serializer will reach
            //        down and hit a proxy, and try to fetch everything from the proxy,
            //        reserialize it, and then pushing it to the server (actually, this can happen to any arrangement where I have proxies)
            //        -- could use IContractResolver to filter out such proxies at serialization time
            //     -- also, say if any proxies are skipped, it means that if a user naiively
            //        adds an item to a subresource collection, in some cases adding it will directly
            //        push it to the server, and in others it just modifies a local List<>.
            // perhaps ahead of time explicit stating of what's inline and what's not is better?
            // so, use of IInlineResource<T> (implements ICollection) for inlines and ISubResource<T> for proxied.

            // then go on to add Count to IAsyncResourceSet and RestResource

            // TODO warn developer from putting an inline resource in that itself contains a full SubResource

            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), Path(), parameters: FullParameters(), requestBody: null);
            var list = Context.TranslateObject<List<T>>(ShopifyAPIClient.Pluralize(Name), resourceString);
            foreach (var model in list)
            {
                PlaceResourceProxesOnModel(model);
            }
            return list;
        }

        public Type GetModelType()
        {
            return typeof(T);
        }

        public Task Each(Action<T> cb)
        {
            var enumerator = new PaginatedEnumerator<T>(this);
            return enumerator.Each(cb);
        }
    }

    public class SubResource<T> : RestResource<T>, IHasMany<T> where T : IResourceModel, new()
    {
        public IParentableResource ParentResource;
        public IResourceModel ParentInstance;
 
        public SubResource(IParentableResource parent, IResourceModel parentInstance, String name = null) : base(parent.Context, name : name)
        {
            if (!parent.GetModelType().IsAssignableFrom(parentInstance.GetType()))
            {
                throw new ShopifyConfigurationException("Parent model instance provided to SubResource must be appropriate to the provided Parent Resource.");
            }
            if (parentInstance.Id == null)
            {
                throw new ShopifyConfigurationException("Parent model instance provided for subresource has null id, which would lead to untenable subresource URIs.");
            }
            ParentResource = parent;
            ParentInstance = parentInstance;
        }

        public override string Path()
        {
            return ShopifyAPIClient.UriPathJoin(ParentResource.InstanceOrVerbPath(ParentInstance.Id.ToString()), ShopifyAPIClient.Pluralize(Name));
        }
    }

    public class SingleInstanceSubResource<T> : IHasOne<T> where T : IResourceModel, new()
    {
        public IShopifyAPIClient Context { get; set; }
        public int Id { get; set; }

        public SingleInstanceSubResource(IShopifyAPIClient context, int id)
        {
            Context = context;
            Id = id;
        }

        public SingleInstanceSubResource(IShopifyAPIClient context, T model) {
            Context = context;
            Set(model);
        }

        public async Task<T> Get()
        {
            return await Context.GetResource<T>().Get(Id);
        }

        public void Set(T model)
        {
            if (model.Id == null)
            {
                throw new ShopifyUsageException(String.Format("A model (type {0}) with an existing ID must be set as an instance passed to Has One.", typeof(T).Name));
            }
            Id = model.Id.Value;
        }
    }
}
