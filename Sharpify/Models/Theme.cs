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
    public class Theme : ShopifyResourceModel
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        // TODO: Enum
        /// <summary>
        /// One of main, mobile, or unpublished.
        /// </summary>
        private string _Role;
        public string Role
        {
            get { return _Role; }
            set {
                SetProperty(ref _Role, value);
            }
        }


        IHasMany<Asset> Assets { get; set; }

        public Theme()
        {
        }
    }
}
