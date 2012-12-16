using System;
namespace ShopifyAPIAdapterLibrary
{
    public interface IRestResource<T> : IRestResourceView<T>
     where T : ShopifyAPIAdapterLibrary.Models.IResourceModel, new()
    {
        /// <summary>
        /// Call a special per-resource action on the server (such as "Cancel" on an Order).
        /// </summary>
        /// <param name="actionPropertyLambda">Use a "member expression" to identify the SpecialAction
        /// property of the resource model, ie.,
        /// <example><code>
        /// Orders.CallAction(myOrder, () => myOrder.Cancel);
        /// </code></example>
        /// </param>
        System.Threading.Tasks.Task CallAction(T instance, System.Linq.Expressions.Expression<Func<SpecialAction>> actionPropertyLambda);

        /// <summary>
        /// Call a special per-resource action on the server (such as "cancel" on an Order);
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="action">The action, specified in lowercase underscorized format expected by the API.</param>
        /// <returns></returns>
        System.Threading.Tasks.Task CallAction(T instance, string action);

        /// <summary>
        /// Set a "has one" relation on the specified object to a new target.
        /// </summary>
        /// <typeparam name="H">The type of the target of the has one.</typeparam>
        /// <param name="belongsTo">The resource model instance containing the IHasOne field</param>
        /// <param name="propertyLambda">Use a "member expression" to identify the IHasOne field itself</param>
        /// <param name="hasOne">The target to set the "has one" relationship to</param>
        void Has<H>(T belongsTo, System.Linq.Expressions.Expression<Func<T, IHasOne<H>>> propertyLambda, H hasOne) where H : ShopifyAPIAdapterLibrary.Models.IResourceModel, new();

        /// <summary>
        /// The underscorized identifier of the resource on the REST interface.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Save a new (that is, does not yet exist in the service) model instance into this REST resource on the service.
        /// </summary>
        /// <typeparam name="T1">Pass the type of the model back in;  this is needed so an additional type check of the creatability of the model type is possible</typeparam>
        /// <returns>The newly created model instance sent back from the service</returns>
        System.Threading.Tasks.Task<T> Create<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.ICreatable;

        /// <summary>
        /// Fetch a model by ID from the service.
        /// </summary>
        System.Threading.Tasks.Task<T> Find(int id);

        /// <summary>
        /// Save over an existing model instance back into the REST resource.
        /// </summary>
        /// <typeparam name="T1">Pass the type of the model back in;  this is needed so an additional type check of the saveablity of the model type is possible</typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<T> Update<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.IMutable;

        /// <summary>
        /// Destroy an existing model instance from the REST resource.
        /// </summary>
        /// <typeparam name="T1">Pass the type of the model back in;  this is needed so an additional type check of the deletability of the model type is possible</typeparam>
        System.Threading.Tasks.Task Delete<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.IDeletable;

        /// <summary>
        /// Either create a new model or save an existing model into the REST resource.
        /// </summary>
        /// <typeparam name="T1">Pass the type of the model back in;  this is needed so an additional type check of the creatability of the model type is possible</typeparam>
        System.Threading.Tasks.Task<T> Save<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.ICreatable, ShopifyAPIAdapterLibrary.Models.IMutable;
    }
}
