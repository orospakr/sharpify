using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/transactions.html
    /// 
    /// http://wiki.shopify.com/Transaction_%28API%29
    /// </summary>
    public class Transaction : ShopifyResourceModel
    {
        public Double Amount { get; set; }

        // TODO: Enum?
        // UNKNOWN
        public string Authorization { get; set; }

        // TODO: Enum?
        public string Gateway { get; set; }

        // TODO: Enum?
        // UNKNOWN
        public string Kind { get; set; }

        public IHasOne<Order> Order { get; set; }

        // TODO: Enum?
        public string Status { get; set; }

        public bool? Test { get; set; }

        public Receipt Receipt { get; set; }

        public Transaction()
        {
        }
    }
}
