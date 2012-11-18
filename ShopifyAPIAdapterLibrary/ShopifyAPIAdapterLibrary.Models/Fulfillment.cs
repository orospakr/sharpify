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
    public class Fulfillment : IResourceModel
    {
        // TODO updated_at/created_at

        public int? Id { get; set; }

        // TODO Second-half of HasMany<> that must be implemented
        public string OrderId { get; set; }

        // TODO: Enum
        public string Service { get; set; }

        // TODO: Enum
        public string Status { get; set; }

        public string TrackingCompany { get; set; }

        public string TrackingNumber { get; set; }

        public string TrackingUrl { get; set; }

        public Receipt Receipt { get; set; }

        public IList<LineItem> LineItems { get; set; }

        public Fulfillment()
        {
        }
    }
}
