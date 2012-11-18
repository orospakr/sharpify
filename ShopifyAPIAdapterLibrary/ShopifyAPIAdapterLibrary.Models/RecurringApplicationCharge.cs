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
    public class RecurringApplicationCharge : IResourceModel
    {
        public int? Id { get; set; }

        public DateTime? ActivatedOn { get; set; }

        public DateTime? BillingOn { get; set; }

        public DateTime? CancelledOn { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        // TODO: Enum
        public string Status { get; set; }

        // UNKNOWN
        public string Test { get; set; }

        public int TrialDays { get; set; }

        public DateTime? TrialEndsOn { get; set; }

        public RecurringApplicationCharge()
        {
        }
    }
}
