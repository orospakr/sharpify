using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/webhook.html
    /// 
    /// http://wiki.shopify.com/Webhook_%28API%29
    /// 
    /// http://wiki.shopify.com/WebHook
    /// 
    /// Have Shopify POST back to an HTTP server you run
    /// with a copy of a changed resource.
    /// 
    /// Naturally, you'll have to implement the receiver in your
    /// own HTTP service.
    /// 
    /// However, you can have Sharpify parse the JSON into Sharpify
    /// objects using JsonDataTranslator: T where T is the
    /// IResourceModel you're expecting back.
    /// </summary>
    public class Webhook : IResourceModel
    {
        public string Id { get; set; }

        /// <summary>
        /// URI to POST to when the specified event occurs.
        /// </summary>
        public string Address { get; set; }

        // TODO: Enum
        /// <summary>
        /// Format to 
        /// 
        /// One of "json" or "xml".  
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Shopify event to capture, form of "orders/partially_fulfilled".
        /// </summary>
        public string Topic { get; set; }

        public Webhook()
        {
        }
    }
}
