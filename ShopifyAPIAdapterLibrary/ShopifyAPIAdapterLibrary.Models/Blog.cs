using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/blog.html
    /// 
    /// http://wiki.shopify.com/Blog_%28API%29
    /// </summary>
    public class Blog : ShopifyResourceModel
    {
        // TODO: Enum, because they "yes"/"no" instead of true/false here for some reason
        private string _Commentable;
        public string Commentable
        {
            get { return _Commentable; }
            set {
                SetProperty(ref _Commentable, value);
            }
        }


        private string _Feedburner;
        public string Feedburner
        {
            get { return _Feedburner; }
            set {
                SetProperty(ref _Feedburner, value);
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


        // TODO: parse tags into proper list
        private string _Tags;
        public string Tags
        {
            get { return _Tags; }
            set {
                SetProperty(ref _Tags, value);
            }
        }


        private IHasMany<Article> _Articles;
        public IHasMany<Article> Articles
        {
            get { return _Articles; }
            set {
                SetProperty(ref _Articles, value);
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


        public Blog()
        {
        }
    }
}
