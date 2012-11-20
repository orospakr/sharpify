using System;
namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// Models that are to be used as full-fledged resources or subresources
    /// (but inlines needn't), should implement this interface.
    /// </summary>
    public interface IResourceModel
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
    }
}
