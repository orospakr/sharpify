using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/shop.html
    /// 
    /// http://wiki.shopify.com/Shop_%28API%29
    /// </summary>
    public class Shop : IResourceModel
    {
        public int? Id { get; set; }

        public string Address1 { get; set; }

        public string City { get; set; }

        public string CustomerEmail { get; set; }

        /// <summary>
        /// Shop-owner entered human-readable name of country.
        /// </summary>
        public string Country { get; set; }

        public string Domain { get; set; }

        public string Email { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        /// <summary>
        /// Human-readable (not URI name) of the Shop.
        /// </summary>
        public string Name { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// Shop-owner entered human-readable name of province.
        /// </summary>
        public string Provice { get; set; }

        // UNKNOWN
        public string Public { get; set; }

        // UNKNOWN
        public string Source { get; set; }

        public string Zip { get; set; }

        public string Currency { get; set; }

        public string Timezone { get; set; }

        /// <summary>
        /// Full name of the shop owner.
        /// </summary>
        public string ShopOwner { get; set; }

        /// <summary>
        /// Some sort of Liquid snippet for formatting the money type
        /// in views.
        /// </summary>
        public string MoneyFormat { get; set; }

        /// <summary>
        /// Liquid snippet for formatting the money type, but additionally
        /// including the currency name.
        /// </summary>
        public string MoneyWithCurrencyFormat { get; set; }

        // UNKNOWN
        public string TaxesIncluded { get; set; }

        // UNKNOWN
        public string TaxShipping { get; set; }

        // TODO: Enum?
        public string PlanName { get; set; }

        public string MyshopifyDomain { get; set; }

        public string GoogleAppsDomain { get; set; }

        public bool? GooleAppsLoginEnabled { get; set; }

        public Shop()
        {
        }
    }
}
