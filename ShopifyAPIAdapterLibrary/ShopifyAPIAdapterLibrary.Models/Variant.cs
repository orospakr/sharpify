using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/product_variant.html
    /// 
    /// http://wiki.shopify.com/Product_Variant_%28API%29
    /// </summary>
    public class Variant : ShopifyResourceModel
    {
        public double? CompareAtPrice { get; set; }

        // TODO: Enum (or, again, do we?  what is story with fulfillment services list?)
        public string FulfillmentService { get; set; }

        // TODO: Enum
        public string InventoryPolicy { get; set; }

        // I don't know what this is. value was "shopify"?
        public string InventoryManagement { get; set; }

        public int Grams { get; set; }

        public double Price { get; set; }

        // I hope this works okay without being nested in a IResourceModel...
        public IHasOne<Product> Product { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public int Position { get; set; }

        public bool RequiresShipping { get; set; }

        public string SKU { get; set; }

        public bool Taxable { get; set; }

        public int InventoryQuantity { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        public Variant()
        {
        }
    }
}
