using System;
using System.ComponentModel;
namespace ShopifyAPIAdapterLibrary.Models
{
    public interface IDirtiable
    {
        bool IsClean();
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class Inlinable : Attribute
    {
    }

    /// <summary>
    /// Models that are to be used as full-fledged resources or subresources
    /// (but inlined "flats" -- not has ones, mind -- needn't), should implement
    /// this interface.
    /// </summary>
    public interface IResourceModel : INotifyPropertyChanged, IDirtiable
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

        void Reset();

        bool IsFieldDirty(string field);
    }
}
