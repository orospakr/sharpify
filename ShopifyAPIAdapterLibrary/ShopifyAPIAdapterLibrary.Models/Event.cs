using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{

    /// <summary>
    /// http://api.shopify.com/event.html
    /// 
    /// http://wiki.shopify.com/Event_%28API%29
    /// 
    /// This resource uses has_many/belongs_to as/polymorphic, which we don't
    /// really support yet.
    /// 
    /// Use the IHasMany:Event relations on Orders, and Events.
    /// </summary>
    [Paginated]
    public class Event: ShopifyResourceModel
    {
        /// <summary>
        /// Different event types have varying
        /// 
        /// 
        /// </summary>
        public IList<string> Arguments { get; set; }

        /// <summary>
        /// belongs_to :polymorphic target ID.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// belongs_to :polymorphic target type.
        /// 
        /// This is (presumably) all of the Shopify API resource types.
        /// 
        /// In CamelCase format.
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// The action of the event.  Probably subject-specific.
        /// </summary>
        public string Verb { get; set; }

        public Event()
        {
        }
    }
}
