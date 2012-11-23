using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{

    /// <summary>
    /// http://api.shopify.com/order.html
    /// 
    /// http://wiki.shopify.com/Order_%28API%29
    /// </summary>
    [Paginated]
    public class Order : ShopifyResourceModel
    {
        // TODO updated_at/created_at
        public bool? BuyerAcceptsMarketing { get; set; }

        public string CancelReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string Currency { get; set; }

        public string Email { get; set; }

        // TODO: Enum
        public string FinancialStatus { get; set; }

        // TODO: Enum
        public string FullfillmentStatus { get; set; }

        // TODO: Enum
        public string Gateway { get; set; }

        public string LandingSite { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public int? Number { get; set; }

        public string ReferringSite { get; set; }

        public double? SubtotalPrice { get; set; }

        public bool? TaxesIncluded { get; set; }

        public string Token { get; set; }

        public double TotalDiscounts { get; set; }

        public double TotalLineItemsPrice { get; set; }

        public double TotalPrice { get; set; }

        public double TotalPriceUSD { get; set; }

        public double TotalTax { get; set; }

        public double TotalWeight { get; set; }

        public string BrowserIP { get; set; }

        public string LandingSiteRef { get; set; }

        public int? OrderNumber { get; set; }

        public IList<DiscountCode> DiscountCodes { get; set; }

        public IList<Property> NoteAttributes { get; set; }

        // TODO: Enum
        public string ProcessingMethod { get; set; }

        public IList<LineItem> LineItems { get; set; }

        public IList<ShippingLine> ShippingLines { get; set; }

        public IList<TaxLine> TaxLines { get; set; }

        public PaymentDetails PaymentDetails { get; set; }

        public Address BillingAddress { get; set; }

        public Address ShippingAddress { get; set; }

        public IHasMany<Fulfillment> Fulfillments { get; set; }

        public ClientDetails ClientDetails { get; set; }

        // crap, this is actually inline, but should be a IHasOne<> because /admin/customers exists
        public Customer Customer { get; set; }

        public IHasMany<Event> Events { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        /// <summary>
        /// Cancel the order.
        /// </summary>
        public SpecialAction Cancel { get; set; }

        /// <summary>
        /// Reopen the order.
        /// </summary>
        public SpecialAction Open { get; set; }

        /// <summary>
        /// Close the order.
        /// </summary>
        public SpecialAction Close { get; set; }

        public Order()
        {
        }
    }
}
