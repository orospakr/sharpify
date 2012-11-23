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
        private double? _CompareAtPrice;
        public double? CompareAtPrice
        {
            get { return _CompareAtPrice; }
            set {
                SetProperty(ref _CompareAtPrice, value);
            }
        }


        // TODO: Enum (or, again, do we?  what is story with fulfillment services list?)
        private string _FulfillmentService;
        public string FulfillmentService
        {
            get { return _FulfillmentService; }
            set {
                SetProperty(ref _FulfillmentService, value);
            }
        }


        // TODO: Enum
        private string _InventoryPolicy;
        public string InventoryPolicy
        {
            get { return _InventoryPolicy; }
            set {
                SetProperty(ref _InventoryPolicy, value);
            }
        }


        // I don't know what this is. value was "shopify"?
        private string _InventoryManagement;
        public string InventoryManagement
        {
            get { return _InventoryManagement; }
            set {
                SetProperty(ref _InventoryManagement, value);
            }
        }


        private int _Grams;
        public int Grams
        {
            get { return _Grams; }
            set {
                SetProperty(ref _Grams, value);
            }
        }


        private double _Price;
        public double Price
        {
            get { return _Price; }
            set {
                SetProperty(ref _Price, value);
            }
        }


        // I hope this works okay without being nested in a IResourceModel...
        private IHasOne<Product> _Product;
        public IHasOne<Product> Product
        {
            get { return _Product; }
            set {
                SetProperty(ref _Product, value);
            }
        }


        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        private int _Position;
        public int Position
        {
            get { return _Position; }
            set {
                SetProperty(ref _Position, value);
            }
        }


        private bool _RequiresShipping;
        public bool RequiresShipping
        {
            get { return _RequiresShipping; }
            set {
                SetProperty(ref _RequiresShipping, value);
            }
        }


        private string _SKU;
        public string SKU
        {
            get { return _SKU; }
            set {
                SetProperty(ref _SKU, value);
            }
        }


        private bool _Taxable;
        public bool Taxable
        {
            get { return _Taxable; }
            set {
                SetProperty(ref _Taxable, value);
            }
        }


        private int _InventoryQuantity;
        public int InventoryQuantity
        {
            get { return _InventoryQuantity; }
            set {
                SetProperty(ref _InventoryQuantity, value);
            }
        }


        private IHasMany<Metafield> _Metafields;
        public IHasMany<Metafield> Metafields
        {
            get { return _Metafields; }
            set {
                SetProperty(ref _Metafields, value);
            }
        }


        public Variant()
        {
        }
    }
}
