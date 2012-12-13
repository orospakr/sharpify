using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/customcollection.html
    /// 
    /// Aka "CustomCollection", annoyingly.
    /// </summary>
    [Paginated]
    public class CustomCollection : ShopifyResourceModel, ISaveable
    {
        private string _BodyHTML;
        public string BodyHTML
        {
            get { return _BodyHTML; }
            set {
                SetProperty(ref _BodyHTML, value);
            }
        }


        private string _Handle;
        public string Handle
        {
            get { return _Handle; }
            set {
                SetProperty(ref _Handle, value);
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


        private IHasMany<Metafield> _Metafields;
        public IHasMany<Metafield> Metafields
        {
            get { return _Metafields; }
            set {
                SetProperty(ref _Metafields, value);
            }
        }


        public CustomCollection()
        {
        }
    }
}
