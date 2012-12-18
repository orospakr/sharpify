using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/customer.html
    /// 
    /// http://wiki.shopify.com/Customer_%28API%29
    /// </summary>
    [Paginated]
    public class Customer : ShopifyResourceModel, IFullMutable
    {
        private bool _AcceptsMarketing;
        public bool AcceptsMarketing
        {
            get { return _AcceptsMarketing; }
            set {
                SetProperty(ref _AcceptsMarketing, value);
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


        private string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set {
                SetProperty(ref _FirstName, value);
            }
        }


        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set {
                SetProperty(ref _LastName, value);
            }
        }


        private IHasOne<Order> _LastOrder;
        public IHasOne<Order> LastOrder
        {
            get { return _LastOrder; }
            set {
                SetProperty(ref _LastOrder, value);
            }
        }


        // what is this?
        private string _Note;
        public string Note
        {
            get { return _Note; }
            set {
                SetProperty(ref _Note, value);
            }
        }


        private int _OrdersCount;
        public int OrdersCount
        {
            get { return _OrdersCount; }
            set {
                SetProperty(ref _OrdersCount, value);
            }
        }


        private string _State;
        public string State
        {
            get { return _State; }
            set {
                SetProperty(ref _State, value);
            }
        }


        private double _TotalSpent;
        public double TotalSpent
        {
            get { return _TotalSpent; }
            set {
                SetProperty(ref _TotalSpent, value);
            }
        }


        /// <summary>
        /// Tags, presumably comma-separated?
        /// </summary>
        private string _Tags;
        public string Tags
        {
            get { return _Tags; }
            set {
                SetProperty(ref _Tags, value);
            }
        }


        /// <summary>
        /// Name of the last order, for convenience.
        /// </summary>
        private string _LastOrderName;
        public string LastOrderName
        {
            get { return _LastOrderName; }
            set {
                SetProperty(ref _LastOrderName, value);
            }
        }

        // even though ID fields appear in this one, I have to
        // treat it as a fragment because there isn't
        // any has-many REST api (subresource) for it.
        private FragmentList<Address> _Addresses;
        public FragmentList<Address> Addresses
        {
            get { return _Addresses; }
            set {
                SetProperty(ref _Addresses, value);
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


        public Customer()
        {
        }
    }
}
