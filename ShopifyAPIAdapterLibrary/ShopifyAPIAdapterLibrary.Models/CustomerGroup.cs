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
    class CustomerGroup : IResourceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Query { get; set; }

        public IHasMany<Customer> Customers { get; set; }

        public CustomerGroup()
        {
        }
    }
}
