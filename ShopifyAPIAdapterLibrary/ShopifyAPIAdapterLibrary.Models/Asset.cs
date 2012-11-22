using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/asset.html
    /// 
    /// http://wiki.shopify.com/Asset_%28API%29
    /// </summary>
    public class Asset : ShopifyResourceModel
    {

        public string Key { get; set; }

        public string PublicURL { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public int ThemeId { get; set; }

        public Asset()
        {
        }
    }
}
