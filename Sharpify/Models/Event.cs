using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
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
        private IList<string> _Arguments;
        public IList<string> Arguments
        {
            get { return _Arguments; }
            set {
                SetProperty(ref _Arguments, value);
            }
        }


        /// <summary>
        /// belongs_to :polymorphic target ID.
        /// </summary>
        private int _SubjectId;
        public int SubjectId
        {
            get { return _SubjectId; }
            set {
                SetProperty(ref _SubjectId, value);
            }
        }


        /// <summary>
        /// belongs_to :polymorphic target type.
        /// 
        /// This is (presumably) all of the Shopify API resource types.
        /// 
        /// In CamelCase format.
        /// </summary>
        private string _SubjectType;
        public string SubjectType
        {
            get { return _SubjectType; }
            set {
                SetProperty(ref _SubjectType, value);
            }
        }


        /// <summary>
        /// The action of the event.  Probably subject-specific.
        /// </summary>
        private string _Verb;
        public string Verb
        {
            get { return _Verb; }
            set {
                SetProperty(ref _Verb, value);
            }
        }


        public Event()
        {
        }
    }
}
