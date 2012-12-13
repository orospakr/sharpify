using System;
using System.Collections.Generic;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/product.html
    /// 
    /// http://wiki.shopify.com/Product_%28API%29
    /// </summary>
    [Paginated]
    public class Product : ShopifyResourceModel, ISaveable
    {
        private string _BodyHtml;
        public string BodyHtml
        {
            get { return _BodyHtml; }
            set
            {
                SetProperty(ref _BodyHtml, value);
            }
        }

        private String _Handle;
        public String Handle
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
 

        private string _Vendor;
        public string Vendor
        {
            get { return _Vendor; }
            set {
                SetProperty(ref _Vendor, value);
            }
        }


        // TODO should be munged into a collection, it arrives as comma separated
        private String _Tags;
        public String Tags
        {
            get { return _Tags; }
            set {
                SetProperty(ref _Tags, value);
            }
        }


        private IHasMany<Event> _Events;
        public IHasMany<Event> Events
        {
            get { return _Events; }
            set {
                SetProperty(ref _Events, value);
            }
        }


        private IHasMany<Image> _Images;
        public IHasMany<Image> Images
        {
            get { return _Images; }
            set {
                SetProperty(ref _Images, value);
            }
        }


        public Product()
        {

        }
    }
}
