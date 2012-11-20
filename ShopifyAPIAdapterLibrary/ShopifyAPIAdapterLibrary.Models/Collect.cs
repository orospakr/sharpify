using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/collect.html
    /// 
    /// This is actually a has_many :through join table between
    /// Product and CustomCollection.
    /// 
    /// I don't support has_many :through as such yet, so you just get naiive
    /// access to join model itself (use Where/query parameters on Product/Collection
    /// to follow the relation).
    /// https://trello.com/card/has-many-through-collects-product-custom-collection/50a1c9c990c4980e0600178b/39
    /// </summary>
    public class Collect : IResourceModel
    {
        public int? Id { get; set; }

        public bool Featured { get; set; }

        public IHasOne<Product> Product { get; set; }

        // Wheee.  As its own resource, CustomCollection is "custom_collection",
        // but as a has_one _id field, it's just "collection".
        public IHasOne<CustomCollection> Collection { get; set; }

        public int Position { get; set; }

        public String SortValue { get; set; }

        public Collect()
        {
        }
    }
}
