using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/product_image.html
    /// 
    /// http://wiki.shopify.com/Product_Image_%28API%29
    /// 
    /// Aka ProductImage.
    /// </summary>
    public class Image : ShopifyResourceModel, IResourceModel, ICreatable, IDeletable
    {
        private IHasOne<Product> _Product;
        public IHasOne<Product> Product
        {
            get { return _Product; }
            set {
                SetProperty(ref _Product, value);
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


        /// <summary>
        /// URI of the Image to fetch.
        /// </summary>
        private string _Src;
        public string Src
        {
            get { return _Src; }
            set {
                SetProperty(ref _Src, value);
            }
        }


        public Image()
        {
        }
    }
}
