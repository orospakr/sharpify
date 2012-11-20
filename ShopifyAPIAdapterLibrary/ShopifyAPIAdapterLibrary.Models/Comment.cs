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
    public class Comment: IResourceModel
    {
        public int? Id { get; set; }

        public string Body { get; set; }

        public IHasOne<Article> Article { get; set; }

        public string Author { get; set; }

        public string BodyHTML { get; set; }

        public string Email { get; set; }

        /// <summary>
        ///  IP address of the poster.
        /// </summary>
        public string IP { get; set; }

        public DateTime? PublishedAt { get; set; }

        // TODO: Enum
        public string Status { get; set; }

        /// <summary>
        /// Browser User-Agent of the poster.
        /// </summary>
        public string UserAgent { get; set; }

        public Comment()
        {
        }
    }
}
