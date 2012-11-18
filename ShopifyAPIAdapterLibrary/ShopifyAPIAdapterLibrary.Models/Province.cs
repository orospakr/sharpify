using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/province.html
    /// 
    /// http://wiki.shopify.com/Province_%28API%29
    /// </summary>
    public class Province: IResourceModel
    {
        public int? Id { get; set; }

        /// <summary>
        /// ISO 3166-2 Alpha 2 Subdivision Code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Subdivision-left sales tax rate.
        /// </summary>
        public double Tax { get; set; }

        /// <summary>
        /// I don't know what differentiates this from Tax.
        ///  
        /// Shopify's doc doesn't clarify.
        /// </summary>
        public double TaxPercentage { get; set; }

        public string TaxType { get; set; }

        public string TaxName { get; set; }

        public IHasOne<Country> Country { get; set; }

        public Province()
        {
        }
    }
}
