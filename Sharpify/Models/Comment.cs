using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/comment.html
    /// 
    /// http://wiki.shopify.com/Comment_%28API%29
    /// </summary>
    [Paginated]
    public class Comment : ShopifyResourceModel, IFullMutable
    {
        private string _Body;
        public string Body
        {
            get { return _Body; }
            set {
                SetProperty(ref _Body, value);
            }
        }


        private IHasOne<Article> _Article;
        public IHasOne<Article> Article
        {
            get { return _Article; }
            set {
                SetProperty(ref _Article, value);
            }
        }


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


        private string _Email;
        public string Email
        {
            get { return _Email; }
            set {
                SetProperty(ref _Email, value);
            }
        }


        /// <summary>
        ///  IP address of the poster.
        /// </summary>
        private string _IP;
        public string IP
        {
            get { return _IP; }
            set {
                SetProperty(ref _IP, value);
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
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set {
                SetProperty(ref _Status, value);
            }
        }


        /// <summary>
        /// Browser User-Agent of the poster.
        /// </summary>
        private string _UserAgent;
        public string UserAgent
        {
            get { return _UserAgent; }
            set {
                SetProperty(ref _UserAgent, value);
            }
        }


        /// <summary>
        /// Mark this comment as spam.
        /// </summary>
        private SpecialAction _Spam;
        public SpecialAction Spam
        {
            get { return _Spam; }
            set {
                SetProperty(ref _Spam, value);
            }
        }


        /// <summary>
        /// Mark this comment as not spam.
        /// </summary>
        private SpecialAction _NotSpam;
        public SpecialAction NotSpam
        {
            get { return _NotSpam; }
            set {
                SetProperty(ref _NotSpam, value);
            }
        }


        public Comment()
        {
        }
    }
}
