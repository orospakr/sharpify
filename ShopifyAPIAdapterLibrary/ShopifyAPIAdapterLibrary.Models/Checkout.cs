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
    [Paginated]
    public class Checkout : ShopifyResourceModel
    {
        /// <summary>
        /// Presumably a copy of the BuyerAcceptsMarketing bit from
        /// the Customer.
        /// </summary>
        public bool BuyerAcceptsMarketing  { get; set; }

        private string _CartToken;
        public string CartToken
        {
            get { return _CartToken; }
            set {
                SetProperty(ref _CartToken, value);
            }
        }


        private string _Email;
        public string Email
        {
            get { return _Email; }
            set {
                SetProperty(ref _Email, value);
            }
        }


        private string _Token;
        public string Token
        {
            get { return _Token; }
            set {
                SetProperty(ref _Token, value);
            }
        }


        private Address _BillingAddress;
        public Address BillingAddress
        {
            get { return _BillingAddress; }
            set {
                SetProperty(ref _BillingAddress, value);
            }
        }


        public Address ShippingAddress  { get; set; }

        public IHasOne<Customer> Customer  { get; set; }

        public Checkout()
        {
        }
    }
}
