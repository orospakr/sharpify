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
    /// Naturally, you'll have to implement the webhook callback
    /// receiver in your own HTTP service.
    /// 
    /// However, you can have Sharpify parse the JSON into Sharpify
    /// objects using JsonDataTranslator: T where T is the
    /// IResourceModel you're expecting back.
    /// </summary>
    [Paginated]
    public class Webhook : ShopifyResourceModel
    {
        /// <summary>
        /// URI to POST to when the specified event occurs.
        /// </summary>
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set {
                SetProperty(ref _Address, value);
            }
        }


        // TODO: Enum
        /// <summary>
        /// Format to 
        /// 
        /// One of "json" or "xml".  
        /// </summary>
        private string _Format;
        public string Format
        {
            get { return _Format; }
            set {
                SetProperty(ref _Format, value);
            }
        }


        /// <summary>
        /// Shopify event to capture, form of "orders/partially_fulfilled".
        /// </summary>
        private string _Topic;
        public string Topic
        {
            get { return _Topic; }
            set {
                SetProperty(ref _Topic, value);
            }
        }


        public Webhook()
        {
        }
    }
}
