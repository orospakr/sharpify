using System;
namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// Models that are to be used as full-fledged resources or subresources
    /// (but inlines needn't), should implement this interface.
    /// </summary>
    public interface IResourceModel
    {
        string Id { get; set; }
    }
}
