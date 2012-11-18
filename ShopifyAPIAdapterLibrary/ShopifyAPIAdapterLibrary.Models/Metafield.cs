using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/metafield.html
    /// 
    /// http://wiki.shopify.com/Metafield_%28API%29
    /// 
    /// Many Shopify resources permit decoration with metafields,
    /// which are simple typed, name/value pairs.
    /// 
    /// If you want your app to store some data against specific
    /// resources inside Shopify itself, Metafields appear to be the
    /// way to do it.  They have the benefit of being accessible through
    /// Liquid templates on Shopify itself, too.
    /// </summary>
    public class Metafield : IResourceModel
    {
        public int? Id { get; set; }

        public string Description { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// To avoid collisions, you can set a namespace named by
        /// your application for your metafields.
        /// </summary>
        public string Namespace { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// Type of the field, ie., string, integer.
        /// </summary>
        // TODO: Enum
        public string ValueType  { get; set; }

        /// <summary>
        /// Foreign key the record this metafield has been associated
        /// with.
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// Type of the record this metafield is associated with.
        /// 
        /// Underscorized/lowercased.
        /// </summary>
        public string OwnerResource { get; set; }

        public Metafield()
        {
        }
    }
}
