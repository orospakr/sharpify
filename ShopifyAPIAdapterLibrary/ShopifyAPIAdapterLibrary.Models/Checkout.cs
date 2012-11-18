using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/checkout.html
    /// 
    /// http://wiki.shopify.com/Customer_%28API%29
    /// </summary>
    public class Checkout : IResourceModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Presumably a copy of the BuyerAcceptsMarketing bit from
        /// the Customer.
        /// </summary>
        public bool BuyerAcceptsMarketing  { get; set; }

        public string CartToken { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public Address BillingAddress { get; set; }

        public Address ShippingAddress  { get; set; }

        public IHasOne<Customer> Customer  { get; set; }

        public Checkout()
        {
        }
    }
}
