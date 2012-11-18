using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/country.html
    /// 
    /// http://wiki.shopify.com/Countries_%28API%29
    /// </summary>
    public class Country : IResourceModel
    {
        public int? Id { get; set; }

        /// <summary>
        /// ISO 3166-1 Alpha-2 Country Code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Federal-level sales tax rate.
        /// </summary>
        public double Tax { get; set; }

        /// <summary>
        /// Name, as whatever the shop owner entered.  Locale unspecified.
        /// </summary>
        public string Name { get; set; }

        public IHasMany<Province> Provinces { get; set; }

        public Country()
        {
        }
    }
}