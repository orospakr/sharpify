using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using Sharpify.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Sharpify
{
    /// <summary>
    /// Untyped access to a RestResource.
    /// 
    /// Largely intended for having a means for ShopifyApiClient
    /// to have a list of resources by hosted IResourceModel type.
    /// </summary>
    public interface IUntypedResource
    {
        /// <summary>
        /// Return the type of models (always inherits from
        /// IResourceModel) used by this resource?
        /// </summary>
        Type GetModelType();

        /// <summary>
        /// Request the number of models this resource will match on
        /// the service.
        /// </summary>
        Task<int> Count();

        string Path();

        string InstancePath(int p);
    }

    public interface IHasMany<T> : IUntypedResource, IRestResource<T> where T : IResourceModel, new()
    {
    }

    public interface IHasOneUntyped
    {
        int Id { get; }
    }

    public interface IHasOneAsIdUntyped : IHasOneUntyped
    {
    }

    /// <summary>
    /// We use this for detecting placeholders inside RestResource.
    /// </summary>
    public interface IHasOnePlaceholderUntyped : IHasOneAsIdUntyped
    {
    }

    public interface IHasOne<T> : IHasOneUntyped where T : IResourceModel
    {
        // Retrieve the resource associated with the parent object.
        Task<T> Get();
    }

    public interface IParentableResource
    {
        IShopifyAPIContext Context { get; }

        string InstanceOrVerbPath(string id);

        Type GetModelType();
    }

    public interface IRestResourceView<T> : IUntypedResource
        where T : IResourceModel, new() 
    {
        /// <summary>
        /// Fetch all the records matched by this resource, and
        /// asynchronously call the provided callback with each.
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        Task Each(Action<T> cb);

        /// <summary>
        /// Fetch and buffer the entire set of records matched by this
        /// resource, and return them all together.
        /// </summary>
        Task<IList<T>> AsListUnpaginated();

        /// <summary>
        /// Fetch and buffer up all resource models matched by this
        /// resource view on the service and return them in a list.
        /// </summary>
        Task<IList<T>> AsList();

        /// <summary>
        /// Returns a new view of this resource filtered to a specific
        /// page (for paginated resources).
        /// </summary>
        IRestResourceView<T> Page(int p);

        /// <summary>
        /// Return a new view of this RestResource with the specified
        /// additional field constraint added.
        /// </summary>
        /// <param name="propertyLambda"></param>
        /// <param name="isEqualTo"></param>
        /// <returns></returns>
        IRestResourceView<T> Where(Expression<Func<T, object>> propertyLambda, string isEqualTo);

        IRestResourceView<T> Where(string field, string isEqualTo);

        /// <summary>
        /// Returns the full set of query string parameters that this
        /// resource view will pass to the REST API.
        /// </summary>
        NameValueCollection FullParameters();
    }

    public class RestResource<T> : IRestResourceView<T>, IParentableResource, IRestResource<T> where T : IResourceModel, new()
    {
        public IShopifyAPIContext Context { get; protected set; }

        public string Name { get; protected set; }

        /// <summary>
        /// Not to be confused with the parenting of SubResources,
        /// this relation is for chaining multiple RestResource
        /// instances pointing to the same effective Resource path on
        /// the REST service with separate filter/query parameters.
        /// 
        /// Clear as mud, right?
        /// </summary>
        public RestResource<T> Parent { get; protected set; }

        public NameValueCollection QueryParameters { get; protected set; }
        
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="ShopifyAPIAdapterLibrary.RestResource`1"/> class.
        /// </summary>
        /// <param name='name'>
        /// The lowercase resource name, as it would appear in URIs.
        /// Default is underscorized from the model type.
        /// </param>
        // TODO: put name back as an optional
        public RestResource(IShopifyAPIContext context, string name = null) {
            Context = context;

            if (name == null)
            {
                Name = ShopifyAPIContext.Underscoreify(typeof(T).Name);
            }
            else
            {
                Name = name;
            }
        }

        public RestResource(RestResource<T> parent, NameValueCollection queryParameters) {
            this.Context = parent.Context;
            this.Parent = parent;
            this.Name = parent.Name;
            this.QueryParameters = queryParameters;
        }

        public NameValueCollection FullParameters() {
            var r = (Parent != null) ? new NameValueCollection(Parent.FullParameters()) : new NameValueCollection();

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
            return ShopifyAPIContext.UriPathJoin(Context.AdminPath(), ShopifyAPIContext.Pluralize(Name));
        }
        
        public string InstanceOrVerbPath(string id) {
            return ShopifyAPIContext.UriPathJoin(Path(), id);
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
        
        public async Task<T> Find(int id) {
            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), InstancePath(id), parameters: FullParameters(), requestBody: null);
            return TranslateObject(Name, resourceString);
        }

        public async Task<T> Create<T1>(T model) where T1 : T, ICreatable {
            if (!model.IsNew())
            {
                throw new ShopifyUsageException("Cannot create an already existing resource model instance.");
            }
            var jsonString = Context.ObjectTranslate<T>(Name, model);
            var resourceString = await Context.CallRaw(HttpMethod.Post, Context.GetRequestContentType(),
                Path(), null, jsonString);
            return TranslateObject(Name, resourceString);
        }

        public Task<T> Save<T1>(T model) where T1 : T, ICreatable, IMutable
        {
            if (model.IsNew())
            {
                return Create<T1>(model);
            }
            else
            {
                return Update<T1>(model);
            }
        }

        public Task Delete<T1>(T model) where T1 : T, IDeletable
        {
            if (model.Id == null)
            {
                throw new ShopifyUsageException("Resource model instance must have an ID in order to delete it.");
            }
            return Context.CallRaw(HttpMethod.Delete, Context.GetRequestContentType(),
                InstanceOrVerbPath(model.Id.ToString()), null, null);
        }

        public async Task<T> Update<T1>(T model) where T1 : T, IMutable
        {
            if (model.Id == null)
            {
                throw new ShopifyUsageException("Model must have an ID in order to PUT an update.");
            }
            var resourceString = Context.ObjectTranslate<T>(Name, model);
            var updatedResourceString = await Context.CallRaw(HttpMethod.Put, Context.GetRequestContentType(),
                InstanceOrVerbPath(model.Id.ToString()), null, resourceString);
            return TranslateObject(Name, updatedResourceString);
        }

        public IRestResourceView<T> Where(string field, string isEqualTo) {
            return new RestResource<T>(this, new NameValueCollection { { field, isEqualTo } });
        }

        public IRestResourceView<T> Page(int page)
        {
            return this.Where("page", page.ToString());
        }

        /// <summary>
        /// Post-process incoming model instances fresh from the JsonDataTranslator,
        /// in order to put live HasOne fetchers (aka SingleInstanceSubResource<>).
        /// </summary>
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

            RecurseThroughAllHasOneProperties(Context, model);

            model.Reset();

            return model;
        }

        private static void RecurseThroughAllHasOneProperties(IShopifyAPIContext shopify, IResourceModel model)
        {
            // change all placeholders (which tell us the ID that was on the _id) into single
            // instance subresources (lives ones that are Get()able)
            var hasOneProperties = from p in model.GetType().GetProperties() where p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == (typeof(IHasOne<>)) select p;
            foreach (var placeholderPropCandidate in hasOneProperties)
            {
                // naturally, any null or inline ones (HasOneInline<>) are skipped
                var hasOnePlaceholder = placeholderPropCandidate.GetValue(model) as IHasOnePlaceholderUntyped;
                if (hasOnePlaceholder == null) continue;

                // get resource model type from the has_a property
                var subResourceModelType = placeholderPropCandidate.PropertyType.GetGenericArguments()[0];
                var baseSubresourceType = typeof(SingleInstanceSubResource<>);
                var resourceType = baseSubresourceType.MakeGenericType(new Type[] { subResourceModelType });

                // I was hoping to avoid using Activator in this project, but no such luck
                var subResourceInstance = Activator.CreateInstance(resourceType, shopify, hasOnePlaceholder.Id);

                placeholderPropCandidate.SetValue(model, subResourceInstance);
            }

            // recurse into and do the same to any inlined hasones on the model.
            foreach (var inlineHasOnePropCandidate in hasOneProperties)
            {
                var inlineHasOne = inlineHasOnePropCandidate.GetValue(model) as IHasOneInlineUntyped;
                if (inlineHasOne == null) continue;

                var inlineModel = inlineHasOne.GetUntypedInlineModel();
                if (inlineModel == null) continue;

                RecurseThroughAllHasOneProperties(shopify, inlineModel);
            }
        }

        public IRestResourceView<T> Where(Expression<Func<T, object>> propertyLambda, string isEqualTo)
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
            var resourceString = await Context.CallRaw(HttpMethod.Get, Context.GetRequestContentType(), Path(), parameters: FullParameters(), requestBody: null);
            var list = Context.TranslateObject<List<T>>(ShopifyAPIContext.Pluralize(Name), resourceString);
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

        public bool IsPaginated()
        {
            return (GetModelType().GetCustomAttribute<Paginated>() != null);
        }

        public Task Each(Action<T> cb)
        {
            var enumerator = new PaginatedEnumerator<T>(this);
            return enumerator.Each(cb);
        }

        public Task<IList<T>> AsList()
        {
            if (IsPaginated())
            {
                var enumerator = new PaginatedEnumerator<T>(this);
                return enumerator.AsList();
            }
            else
            {
                return AsListUnpaginated();
            }
        }

        public async Task CallAction(T instance, string action)
        {
            await Context.CallRaw(HttpMethod.Post,
                Context.GetRequestContentType(),
                ShopifyAPIContext.UriPathJoin(InstancePath(instance.Id.Value), action), FullParameters(), null);
        }

        public Task CallAction(T instance, Expression<Func<SpecialAction>> actionPropertyLambda)
        {
            var memberExpression = actionPropertyLambda.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ShopifyConfigurationException("Please specify a flat expression specifying a SpecialAction property on the IResourceModel.");
            }
            var prop = memberExpression.Member as PropertyInfo;
            if (prop == null)
            {
                throw new ShopifyConfigurationException("Expression specified to CallAction?() did not specify a property.  This probably can't happen (because of type parameter to Func<>)?");
            }
            var type = typeof(T);
            if (type != prop.ReflectedType && !type.IsSubclassOf(prop.ReflectedType))
            {
                throw new ShopifyConfigurationException("Expression specified to CallAction() specified a property on a different class than the IResourceModel type of this RestResource.");
            }
            if (instance.Id == null)
            {
                throw new ShopifyUsageException("Actions can only be called on models with an ID.");
            }
            var action = ShopifyAPIContext.Underscoreify(prop.Name);
            return CallAction(instance, action);
        }

        public void Has<H>(T belongsTo, Expression<Func<T, IHasOne<H>>> propertyLambda, H hasOne) where H : IResourceModel, new()
        {
            var memberExpression = propertyLambda.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ShopifyUsageException("Expression specified to Has() did not specify a member of a type.");
            }
            var prop = memberExpression.Member as PropertyInfo;
            if (prop == null)
            {
                throw new ShopifyUsageException("Expression specified to Has() did not specify a property.");
            }
            var checkType = typeof(T);
            if (!checkType.IsAssignableFrom(prop.ReflectedType))
            {
                throw new ShopifyUsageException("Expression specified to Has() specified a property on a different resource model than this one.");
            }

            if (prop.GetCustomAttribute<Inlinable>() != null)
            {
                // make an inline
                var hasOneProp = new HasOneInline<H>(hasOne);
                prop.SetValue(belongsTo, hasOneProp);
            }
            else
            {
                // make a singleinstance subresource
                if (!hasOne.Id.HasValue)
                {
                    throw new ShopifyUsageException("Must use a model instance with an ID to save a not-inlined Has One.");
                }
                var hasOneProp = new SingleInstanceSubResource<H>(Context, hasOne.Id.Value);
                prop.SetValue(belongsTo, hasOneProp);
            }
        }
    }

    /// <summary>
    /// Represents a nested REST subresource, a "has many" relationship.
    /// </summary>
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
            return ShopifyAPIContext.UriPathJoin(ParentResource.InstanceOrVerbPath(ParentInstance.Id.ToString()), ShopifyAPIContext.Pluralize(Name));
        }
    }

    public class SingleInstanceSubResource<T> : IHasOne<T>, IHasOneAsIdUntyped where T : IResourceModel, new()
    {
        public IShopifyAPIContext Context { get; set; }
        public int Id { get; set; }

        public SingleInstanceSubResource(IShopifyAPIContext context, int id)
        {
            Context = context;
            Id = id;
        }

        public SingleInstanceSubResource(IShopifyAPIContext context, T model)
        {
            Context = context;
            Set(model);
        }

        public async Task<T> Get()
        {
            return await Context.GetResource<T>().Find(Id);
        }

        private void Set(T model)
        {
            if (model.Id == null)
            {
                throw new ShopifyUsageException(String.Format("A model (type {0}) with an existing ID must be set as an instance passed to Has One.", typeof(T).Name));
            }
            Id = model.Id.Value;
        }
    }
}
