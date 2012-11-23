using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/recurringapplicationcharge.html
    /// </summary>
    public class RecurringApplicationCharge : ShopifyResourceModel
    {
        private DateTime? _ActivatedOn;
        public DateTime? ActivatedOn
        {
            get { return _ActivatedOn; }
            set {
                SetProperty(ref _ActivatedOn, value);
            }
        }


        private DateTime? _BillingOn;
        public DateTime? BillingOn
        {
            get { return _BillingOn; }
            set {
                SetProperty(ref _BillingOn, value);
            }
        }


        private DateTime? _CancelledOn;
        public DateTime? CancelledOn
        {
            get { return _CancelledOn; }
            set {
                SetProperty(ref _CancelledOn, value);
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


        private double _Price;
        public double Price
        {
            get { return _Price; }
            set {
                SetProperty(ref _Price, value);
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


        // UNKNOWN
        private string _Test;
        public string Test
        {
            get { return _Test; }
            set {
                SetProperty(ref _Test, value);
            }
        }


        private int _TrialDays;
        public int TrialDays
        {
            get { return _TrialDays; }
            set {
                SetProperty(ref _TrialDays, value);
            }
        }


        private DateTime? _TrialEndsOn;
        public DateTime? TrialEndsOn
        {
            get { return _TrialEndsOn; }
            set {
                SetProperty(ref _TrialEndsOn, value);
            }
        }


        public RecurringApplicationCharge()
        {
        }
    }
}
