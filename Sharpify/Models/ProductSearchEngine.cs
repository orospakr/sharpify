using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/productsearchengine.html
    /// 
    /// http://wiki.shopify.com/Product_Search_Engine_%28API%29
    /// List of enabled search engines.
    /// </summary>
    public class ProductSearchEngine : ShopifyResourceModel
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        public ProductSearchEngine()
        {
        }
    }
}
