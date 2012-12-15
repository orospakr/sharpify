using System;
using System.ComponentModel;
namespace ShopifyAPIAdapterLibrary.Models
{
    public interface IDirtiable
    {
        bool IsClean();
    }

    public interface IGranularDirtiable : IDirtiable
    {
        bool IsFieldDirty(string field);
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Inlinable : Attribute
    {
    }

    /// <summary>
    /// Tags resources that can be deleted (DELETE).
    /// </summary>
    public interface IDeletable
    {
    }

    /// <summary>
    /// Tags resources that can be created (POST).
    /// </summary>
    public interface ICreatable
    {
    }

    /// <summary>
    /// Tags resources that can be modified (PUT).
    /// </summary>
    public interface IMutable
    {
    }

    /// <summary>
    /// Used to tag resources that are fully mutable on the REST service.
    /// 
    /// (Create, Update, Delete).
    /// </summary>
    public interface IFullMutable : ICreatable, IDeletable, IMutable
    {
    }

    /// <summary>
    /// Models that are to be used as full-fledged resources or subresources
    /// (but inlined "fragments" -- not has ones, mind -- needn't), should implement
    /// this interface.
    /// </summary>
    public interface IResourceModel : INotifyPropertyChanged, IGranularDirtiable
    {
        /// <summary>
        /// The ID of the record.  Must be present on resources.
        /// 
        /// Is an integer for two main reasons: the ID and custom
        /// verbs occupy the same segment in path URIs, and are
        /// disambiguated by means of intishness, and that
        /// ActiveRecord on the remote end is using integers
        /// as the database PK.
        /// </summary>
        int? Id { get; set; }

        /// <summary>
        /// Marks this resource model as clean (ie., forgets that
        /// any fields have changed).
        /// </summary>
        void Reset();

        /// <summary>
        /// Has this resource model come from the remote service?
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool IsFieldDirty(string field);

        bool IsNew();

        /// <summary>
        /// Marks this resource model as existing on the server.
        /// 
        /// The deserializer typically hits this.
        /// </summary>
        void SetExisting();
    }
}
