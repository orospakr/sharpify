using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/fulfillment.html
    /// 
    /// http://wiki.shopify.com/Fulfillment_%28API%29
    /// </summary>
    [Paginated]
    public class Fulfillment : ShopifyResourceModel, ISaveable
    {

        // TODO Second-half of HasMany<> that must be implemented
        private string _OrderId;
        public string OrderId
        {
            get { return _OrderId; }
            set {
                SetProperty(ref _OrderId, value);
            }
        }


        // TODO: Enum
        private string _Service;
        public string Service
        {
            get { return _Service; }
            set {
                SetProperty(ref _Service, value);
            }
        }


        // TODO: Enum
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set {
                SetProperty(ref _Status, value);
            }
        }


        private string _TrackingCompany;
        public string TrackingCompany
        {
            get { return _TrackingCompany; }
            set {
                SetProperty(ref _TrackingCompany, value);
            }
        }


        private string _TrackingNumber;
        public string TrackingNumber
        {
            get { return _TrackingNumber; }
            set {
                SetProperty(ref _TrackingNumber, value);
            }
        }


        private string _TrackingUrl;
        public string TrackingUrl
        {
            get { return _TrackingUrl; }
            set {
                SetProperty(ref _TrackingUrl, value);
            }
        }


        private Receipt _Receipt;
        public Receipt Receipt
        {
            get { return _Receipt; }
            set {
                SetProperty(ref _Receipt, value);
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


        public Fulfillment()
        {
        }
    }
}
