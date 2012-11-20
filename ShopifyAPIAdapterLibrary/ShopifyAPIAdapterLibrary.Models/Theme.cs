using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/theme.html
    /// 
    /// http://wiki.shopify.com/Theme_%28API%29
    /// </summary>
    public class Theme : IResourceModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        // TODO: Enum
        /// <summary>
        /// One of main, mobile, or unpublished.
        /// </summary>
        public string Role { get; set; }

        IHasMany<Asset> Assets { get; set; }

        public Theme()
        {
        }
    }
}
