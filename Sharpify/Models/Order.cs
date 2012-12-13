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
    public class Order : ShopifyResourceModel, ISaveable
    {
        // TODO updated_at/created_at
        private bool? _BuyerAcceptsMarketing;
        public bool? BuyerAcceptsMarketing
        {
            get { return _BuyerAcceptsMarketing; }
            set {
                SetProperty(ref _BuyerAcceptsMarketing, value);
            }
        }


        private string _CancelReason;
        public string CancelReason
        {
            get { return _CancelReason; }
            set {
                SetProperty(ref _CancelReason, value);
            }
        }


        private DateTime? _CancelledAt;
        public DateTime? CancelledAt
        {
            get { return _CancelledAt; }
            set {
                SetProperty(ref _CancelledAt, value);
            }
        }


        private string _Currency;
        public string Currency
        {
            get { return _Currency; }
            set {
                SetProperty(ref _Currency, value);
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


        // TODO: Enum
        private string _FinancialStatus;
        public string FinancialStatus
        {
            get { return _FinancialStatus; }
            set {
                SetProperty(ref _FinancialStatus, value);
            }
        }


        // TODO: Enum
        private string _FullfillmentStatus;
        public string FullfillmentStatus
        {
            get { return _FullfillmentStatus; }
            set {
                SetProperty(ref _FullfillmentStatus, value);
            }
        }


        // TODO: Enum
        private string _Gateway;
        public string Gateway
        {
            get { return _Gateway; }
            set {
                SetProperty(ref _Gateway, value);
            }
        }


        private string _LandingSite;
        public string LandingSite
        {
            get { return _LandingSite; }
            set {
                SetProperty(ref _LandingSite, value);
            }
        }


        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        private string _Note;
        public string Note
        {
            get { return _Note; }
            set {
                SetProperty(ref _Note, value);
            }
        }


        private int? _Number;
        public int? Number
        {
            get { return _Number; }
            set {
                SetProperty(ref _Number, value);
            }
        }


        private string _ReferringSite;
        public string ReferringSite
        {
            get { return _ReferringSite; }
            set {
                SetProperty(ref _ReferringSite, value);
            }
        }


        private double? _SubtotalPrice;
        public double? SubtotalPrice
        {
            get { return _SubtotalPrice; }
            set {
                SetProperty(ref _SubtotalPrice, value);
            }
        }


        private bool? _TaxesIncluded;
        public bool? TaxesIncluded
        {
            get { return _TaxesIncluded; }
            set {
                SetProperty(ref _TaxesIncluded, value);
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


        private double _TotalDiscounts;
        public double TotalDiscounts
        {
            get { return _TotalDiscounts; }
            set {
                SetProperty(ref _TotalDiscounts, value);
            }
        }


        private double _TotalLineItemsPrice;
        public double TotalLineItemsPrice
        {
            get { return _TotalLineItemsPrice; }
            set {
                SetProperty(ref _TotalLineItemsPrice, value);
            }
        }


        private double _TotalPrice;
        public double TotalPrice
        {
            get { return _TotalPrice; }
            set {
                SetProperty(ref _TotalPrice, value);
            }
        }


        private double _TotalPriceUSD;
        public double TotalPriceUSD
        {
            get { return _TotalPriceUSD; }
            set {
                SetProperty(ref _TotalPriceUSD, value);
            }
        }


        private double _TotalTax;
        public double TotalTax
        {
            get { return _TotalTax; }
            set {
                SetProperty(ref _TotalTax, value);
            }
        }


        private double _TotalWeight;
        public double TotalWeight
        {
            get { return _TotalWeight; }
            set {
                SetProperty(ref _TotalWeight, value);
            }
        }


        private string _BrowserIP;
        public string BrowserIP
        {
            get { return _BrowserIP; }
            set {
                SetProperty(ref _BrowserIP, value);
            }
        }


        private string _LandingSiteRef;
        public string LandingSiteRef
        {
            get { return _LandingSiteRef; }
            set {
                SetProperty(ref _LandingSiteRef, value);
            }
        }


        private int? _OrderNumber;
        public int? OrderNumber
        {
            get { return _OrderNumber; }
            set {
                SetProperty(ref _OrderNumber, value);
            }
        }


        private FragmentList<DiscountCode> _DiscountCodes;
        public FragmentList<DiscountCode> DiscountCodes
        {
            get { return _DiscountCodes; }
            set {
                SetProperty(ref _DiscountCodes, value);
            }
        }


        private FragmentList<Property> _NoteAttributes;
        public FragmentList<Property> NoteAttributes
        {
            get { return _NoteAttributes; }
            set {
                SetProperty(ref _NoteAttributes, value);
            }
        }


        // TODO: Enum
        private string _ProcessingMethod;
        public string ProcessingMethod
        {
            get { return _ProcessingMethod; }
            set {
                SetProperty(ref _ProcessingMethod, value);
            }
        }


        private FragmentList<LineItem> _LineItems;
        public FragmentList<LineItem> LineItems
        {
            get { return _LineItems; }
            set {
                SetProperty(ref _LineItems, value);
            }
        }


        private FragmentList<ShippingLine> _ShippingLines;
        public FragmentList<ShippingLine> ShippingLines
        {
            get { return _ShippingLines; }
            set {
                SetProperty(ref _ShippingLines, value);
            }
        }


        private FragmentList<TaxLine> _TaxLines;
        public FragmentList<TaxLine> TaxLines
        {
            get { return _TaxLines; }
            set {
                SetProperty(ref _TaxLines, value);
            }
        }


        private PaymentDetails _PaymentDetails;
        public PaymentDetails PaymentDetails
        {
            get { return _PaymentDetails; }
            set {
                SetProperty(ref _PaymentDetails, value);
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


        private Address _ShippingAddress;
        public Address ShippingAddress
        {
            get { return _ShippingAddress; }
            set {
                SetProperty(ref _ShippingAddress, value);
            }
        }


        private IHasMany<Fulfillment> _Fulfillments;
        public IHasMany<Fulfillment> Fulfillments
        {
            get { return _Fulfillments; }
            set {
                SetProperty(ref _Fulfillments, value);
            }
        }


        private ClientDetails _ClientDetails;
        public ClientDetails ClientDetails
        {
            get { return _ClientDetails; }
            set {
                SetProperty(ref _ClientDetails, value);
            }
        }


        // crap, this is actually inline, but should be a IHasOne<> because /admin/customers exists
        private Customer _Customer;
        public Customer Customer
        {
            get { return _Customer; }
            set {
                SetProperty(ref _Customer, value);
            }
        }


        private IHasMany<Event> _Events;
        public IHasMany<Event> Events
        {
            get { return _Events; }
            set {
                SetProperty(ref _Events, value);
            }
        }


        private IHasMany<Metafield> _Metafields;
        public IHasMany<Metafield> Metafields
        {
            get { return _Metafields; }
            set {
                SetProperty(ref _Metafields, value);
            }
        }


        /// <summary>
        /// Cancel the order.
        /// </summary>
        private SpecialAction _Cancel;
        public SpecialAction Cancel
        {
            get { return _Cancel; }
            set {
                SetProperty(ref _Cancel, value);
            }
        }


        /// <summary>
        /// Reopen the order.
        /// </summary>
        private SpecialAction _Open;
        public SpecialAction Open
        {
            get { return _Open; }
            set {
                SetProperty(ref _Open, value);
            }
        }


        /// <summary>
        /// Close the order.
        /// </summary>
        private SpecialAction _Close;
        public SpecialAction Close
        {
            get { return _Close; }
            set {
                SetProperty(ref _Close, value);
            }
        }


        public Order()
        {
        }
    }
}
