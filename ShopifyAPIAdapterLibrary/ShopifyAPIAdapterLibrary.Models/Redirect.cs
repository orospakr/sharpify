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
    public class Redirect : IResourceModel
    {
        public int? Id { get; set; }

        /// <summary>
        /// Original path to intercept.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// New path to redirect to.
        /// </summary>
        public string Target { get; set; }

        public Redirect()
        {
        }
    }
}
