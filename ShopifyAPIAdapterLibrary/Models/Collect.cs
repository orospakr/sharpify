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
    public class Collect : ShopifyResourceModel
    {
        private bool _Featured;
        public bool Featured
        {
            get { return _Featured; }
            set {
                SetProperty(ref _Featured, value);
            }
        }


        private IHasOne<Product> _Product;
        public IHasOne<Product> Product
        {
            get { return _Product; }
            set {
                SetProperty(ref _Product, value);
            }
        }


        // Wheee.  As its own resource, CustomCollection is "custom_collection",
        // but as a has_one _id field, it's just "collection".
        private IHasOne<CustomCollection> _Collection;
        public IHasOne<CustomCollection> Collection
        {
            get { return _Collection; }
            set {
                SetProperty(ref _Collection, value);
            }
        }


        private int _Position;
        public int Position
        {
            get { return _Position; }
            set {
                SetProperty(ref _Position, value);
            }
        }


        private String _SortValue;
        public String SortValue
        {
            get { return _SortValue; }
            set {
                SetProperty(ref _SortValue, value);
            }
        }


        public Collect()
        {
        }
    }
}
