using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/page.html
    /// 
    /// http://wiki.shopify.com/Page_%28API%29
    /// </summary>
    [Paginated]
    public class Page : ShopifyResourceModel
    {
        public string Author { get; set; }

        public string BodyHTML { get; set; }

        public string Handle { get; set; }

        public DateTime PublishedAt { get; set; }

        public IHasOne<Shop> Shop { get; set; }

        public string TemplateSuffix { get; set; }

        public string Title { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        public Page()
        {
        }
    }
}
