using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/applicationcharge.html
    /// 
    /// 
    /// </summary>
    public class ApplicationCharge : IResourceModel
    {
        // TODO: this particular class is probably sensitive to unnecessary fields in updates.
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Price { get; set; }

        public string Status { get; set; }

        public string ConfirmationURL { get; set; }

        public string ReturnURL { get; set; }

        public ApplicationCharge()
        {
        }
    }
}
