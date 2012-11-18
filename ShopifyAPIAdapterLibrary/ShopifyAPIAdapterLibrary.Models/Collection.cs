using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/customcollection.html
    /// 
    /// Aka "CustomCollection", annoyingly.
    /// </summary>
    public class Collection : IResourceModel
    {
        public string Id { get; set; }

        public string BodyHTML { get; set; }

        public string Handle { get; set; }

        // TODO: Enum
        public string SortOrder { get; set; }

        public string TemplateSuffix { get; set; }

        public string Title { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        public Collection()
        {
        }
    }
}
