using System;
using System.Collections.Generic;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/product.html
    /// 
    /// http://wiki.shopify.com/Product_%28API%29
    /// </summary>
    public class Product : IResourceModel
    {
        public string BodyHtml { get; set; }

        public DateTime CreatedAt { get; set; }

        public String Handle { get; set; }

        public String Id { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string TemplateSuffix { get; set; }

        public string Title { get; set; } 

        public string Vendor { get; set; }

        // TODO should be munged into a collection, it arrives as comma separated
        public String Tags { get; set; }

        public IHasMany<Event> Events { get; set; }

        public IHasMany<Image> Images { get; set; }

        public Product()
        {

        }
    }
}
