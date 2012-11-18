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
    public class ScriptTag : IResourceModel
    {
        public int? Id { get; set; }

        /// <summary>
        /// Name of JS-subscrible event emitted by the Browser's Document API
        /// that should invoke the provided script contents.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// URI of JavaScript that should be executed.
        /// </summary>
        public string Src { get; set; }

        public ScriptTag()
        {
        }
    }
}
