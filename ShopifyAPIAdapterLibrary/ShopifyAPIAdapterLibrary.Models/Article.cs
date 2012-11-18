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
    public class Article : IResourceModel
    {
        public int? Id { get; set; }

        public string Author { get; set; }

        // TODO belongs_to half of HasMany
        public int BlogId { get; set; }

        public string BodyHTML { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string SummaryHTML { get; set; }

        public string Title { get; set; }

        // TODO belongs_to
        public int? UserId { get; set; }

        public string Tags { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        public IHasMany<Comment> Comments { get; set; }

        public Article()
        {
        }
    }
}
