using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/redirect.html
    /// 
    /// http://wiki.shopify.com/Redirect_%28API%29
    /// </summary>
    [Paginated]
    public class Redirect : ShopifyResourceModel
    {
        /// <summary>
        /// Original path to intercept.
        /// </summary>
        private string _Path;
        public string Path
        {
            get { return _Path; }
            set {
                SetProperty(ref _Path, value);
            }
        }


        /// <summary>
        /// New path to redirect to.
        /// </summary>
        private string _Target;
        public string Target
        {
            get { return _Target; }
            set {
                SetProperty(ref _Target, value);
            }
        }


        public Redirect()
        {
        }
    }
}
