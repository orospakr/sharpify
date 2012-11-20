using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/product_image.html
    /// 
    /// http://wiki.shopify.com/Product_Image_%28API%29
    /// 
    /// Aka ProductImage.
    /// </summary>
    public class Image : IResourceModel
    {
        public int? Id { get; set; }

        public IHasOne<Product> Product { get; set; }

        public int Position { get; set; }

        /// <summary>
        /// URI of the Image to fetch.
        /// </summary>
        public string Src { get; set; }

        public Image()
        {
        }
    }
}
