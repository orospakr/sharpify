using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/page.html
    /// 
    /// http://wiki.shopify.com/Page_%28API%29
    /// </summary>
    [Paginated]
    public class Page : ShopifyResourceModel, IFullMutable
    {
        private string _Author;
        public string Author
        {
            get { return _Author; }
            set {
                SetProperty(ref _Author, value);
            }
        }


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


        private DateTime _PublishedAt;
        public DateTime PublishedAt
        {
            get { return _PublishedAt; }
            set {
                SetProperty(ref _PublishedAt, value);
            }
        }


        private IHasOne<Shop> _Shop;
        public IHasOne<Shop> Shop
        {
            get { return _Shop; }
            set {
                SetProperty(ref _Shop, value);
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


        public Page()
        {
        }
    }
}
