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
    public class Blog : IResourceModel
    {
        public string Id { get; set; }

        // TODO: do yes/no serialize properly?
        public bool Commentable { get; set; }

        public string Feedburner { get; set; }

        public string Handle { get; set; }

        public string TemplateSuffix { get; set; }

        public string Title { get; set; }

        // TODO: parse tags into proper list
        public string Tags { get; set; }

        public IHasMany<Article> Articles { get; set; }

        public IHasMany<Metafield> Metafields { get; set; }

        public Blog()
        {
        }
    }
}
