using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/smartcollection.html
    /// 
    /// http://wiki.shopify.com/Smart_Collection_%28API%29
    /// 
    /// A persistent filter of Products.
    /// </summary>
    [Paginated]
    public class SmartCollection : ShopifyResourceModel, IFullMutable
    {
        private string _BodyHTML;
        public string BodyHTML
        {
            get { return _BodyHTML; }
            set {
                SetProperty(ref _BodyHTML, value);
            }
        }


        /// <summary>
        /// Short name appropriate for URI fragment.
        /// </summary>
        private string _Handle;
        public string Handle
        {
            get { return _Handle; }
            set {
                SetProperty(ref _Handle, value);
            }
        }


        private DateTime? _PublishedAt;
        public DateTime? PublishedAt
        {
            get { return _PublishedAt; }
            set {
                SetProperty(ref _PublishedAt, value);
            }
        }


        // TODO: Enum
        private string _SortOrder;
        public string SortOrder
        {
            get { return _SortOrder; }
            set {
                SetProperty(ref _SortOrder, value);
            }
        }


        private string _TemplateSuffix;
        public string TemplateSuffix
        {
            get { return _TemplateSuffix; }
            set {
                SetProperty(ref _TemplateSuffix, value);
            }
        }


        private string _Title;
        public string Title
        {
            get { return _Title; }
            set {
                SetProperty(ref _Title, value);
            }
        }


        private FragmentList<Rule> _Rules;
        public FragmentList<Rule> Rules
        {
            get { return _Rules; }
            set {
                SetProperty(ref _Rules, value);
            }
        }


        // TODO is this actually the same record as ProductImage?
        // Just leaving it directly inline.
        private Image _Image;
        public Image Image
        {
            get { return _Image; }
            set {
                SetProperty(ref _Image, value);
            }
        }

    }
}
