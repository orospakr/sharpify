using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/article.html
    /// 
    /// http://wiki.shopify.com/Article_%28API%29
    /// </summary>
    [Paginated]
    public class Article : ShopifyResourceModel
    {

        private string _Author;
        public string Author
        {
            get { return _Author; }
            set {
                SetProperty(ref _Author, value);
            }
        }


        // TODO belongs_to half of HasMany
        private int _BlogId;
        public int BlogId
        {
            get { return _BlogId; }
            set {
                SetProperty(ref _BlogId, value);
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


        private DateTime? _PublishedAt;
        public DateTime? PublishedAt
        {
            get { return _PublishedAt; }
            set {
                SetProperty(ref _PublishedAt, value);
            }
        }


        private string _SummaryHTML;
        public string SummaryHTML
        {
            get { return _SummaryHTML; }
            set {
                SetProperty(ref _SummaryHTML, value);
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


        // TODO belongs_to
        private int? _UserId;
        public int? UserId
        {
            get { return _UserId; }
            set {
                SetProperty(ref _UserId, value);
            }
        }


        private string _Tags;
        public string Tags
        {
            get { return _Tags; }
            set {
                SetProperty(ref _Tags, value);
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


        private IHasMany<Comment> _Comments;
        public IHasMany<Comment> Comments
        {
            get { return _Comments; }
            set {
                SetProperty(ref _Comments, value);
            }
        }


        public Article()
        {
        }
    }
}
