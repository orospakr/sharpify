using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/customergroup.html
    /// 
    /// http://wiki.shopify.com/Customer_Group_%28API%29
    /// 
    /// A defined Customer Group, which is basically a savable filter
    /// preset for selecting customers.
    /// </summary>
    [Paginated]
    public class CustomerGroup : ShopifyResourceModel, ISaveable
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        private string _Query;
        public string Query
        {
            get { return _Query; }
            set {
                SetProperty(ref _Query, value);
            }
        }


        private IHasMany<Customer> _Customers;
        public IHasMany<Customer> Customers
        {
            get { return _Customers; }
            set {
                SetProperty(ref _Customers, value);
            }
        }


        public CustomerGroup()
        {
        }
    }
}
