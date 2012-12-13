﻿using System;
namespace ShopifyAPIAdapterLibrary
{
    interface IRestResource<T>
     where T : ShopifyAPIAdapterLibrary.Models.IResourceModel, new()
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IList<T>> AsList();
        System.Threading.Tasks.Task CallAction(T instance, System.Linq.Expressions.Expression<Func<T, SpecialAction>> actionPropertyLambda);
        System.Threading.Tasks.Task CallAction(T instance, string action);
        System.Threading.Tasks.Task<int> Count();
        System.Threading.Tasks.Task<T> Create<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.ISaveable;
        System.Threading.Tasks.Task Each(Action<T> cb);
        System.Threading.Tasks.Task<T> Get(int id);
        Type GetModelType();
        void Has<H>(T belongsTo, System.Linq.Expressions.Expression<Func<T, IHasOne<H>>> propertyLambda, H hasOne) where H : ShopifyAPIAdapterLibrary.Models.IResourceModel, new();
        string Name { get; }
        System.Threading.Tasks.Task<T> Save<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.ISaveable;
        System.Threading.Tasks.Task<T> Update<T1>(T model) where T1 : T, ShopifyAPIAdapterLibrary.Models.ISaveable;
        IRestResourceView<T> Where(System.Linq.Expressions.Expression<Func<T, object>> propertyLambda, string isEqualTo);
        IRestResourceView<T> Where(string field, string isEqualTo);
    }
}