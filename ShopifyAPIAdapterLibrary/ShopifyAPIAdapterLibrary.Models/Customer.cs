using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/customer.html
    /// 
    /// http://wiki.shopify.com/Customer_%28API%29
    /// </summary>
    public class Customer : IResourceModel
    {
        public string Id { get; set; }

        public bool AcceptsMarketing { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IHasOne<Order> LastOrder { get; set; }

        // what is this?
        public string Note { get; set; }

        public int OrdersCount { get; set; }

        public string State { get; set; }

        public double TotalSpent { get; set; }

        /// <summary>
        /// Tags, presumably comma-separated?
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Name of the last order, for convenience.
        /// </summary>
        public string LastOrderName { get; set; }

        public IList<Address> Addresses { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        public Customer()
        {
        }
    }
}
