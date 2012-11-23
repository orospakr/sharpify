using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/scripttag.html
    /// 
    /// http://wiki.shopify.com/Script_Tag_%28API%29
    /// </summary>
    [Paginated]
    public class ScriptTag : ShopifyResourceModel
    {
        /// <summary>
        /// Name of JS-subscrible event emitted by the Browser's Document API
        /// that should invoke the provided script contents.
        /// </summary>
        private string _Event;
        public string Event
        {
            get { return _Event; }
            set {
                SetProperty(ref _Event, value);
            }
        }


        /// <summary>
        /// URI of JavaScript that should be executed.
        /// </summary>
        private string _Src;
        public string Src
        {
            get { return _Src; }
            set {
                SetProperty(ref _Src, value);
            }
        }


        public ScriptTag()
        {
        }
    }
}
