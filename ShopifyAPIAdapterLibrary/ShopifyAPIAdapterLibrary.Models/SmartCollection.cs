using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/smartcollection.html
    /// 
    /// http://wiki.shopify.com/Smart_Collection_%28API%29
    /// 
    /// A persistent filter of Products.
    /// </summary>
    [Paginated]
    public class SmartCollection : ShopifyResourceModel
    {
        public string BodyHTML { get; set; }

        /// <summary>
        /// Short name appropriate for URI fragment.
        /// </summary>
        public string Handle { get; set; }

        public DateTime? PublishedAt { get; set; }

        // TODO: Enum
        public string SortOrder { get; set; }

        public string TemplateSuffix { get; set; }

        public string Title { get; set; }

        public IList<Rule> Rules { get; set; }

        // TODO is this actually the same record as ProductImage?
        // Just leaving it directly inline.
        public Image Image { get; set; }
    }
}
