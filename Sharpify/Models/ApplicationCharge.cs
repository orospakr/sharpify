using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/applicationcharge.html
    /// 
    /// 
    /// </summary>
    public class ApplicationCharge : ShopifyResourceModel
    {
        // TODO: this particular class is probably sensitive to unnecessary fields in updates.

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        private bool _Price;
        public bool Price
        {
            get { return _Price; }
            set {
                SetProperty(ref _Price, value);
            }
        }


        private string _Status;
        public string Status
        {
            get { return _Status; }
            set {
                SetProperty(ref _Status, value);
            }
        }


        private string _ConfirmationURL;
        public string ConfirmationURL
        {
            get { return _ConfirmationURL; }
            set {
                SetProperty(ref _ConfirmationURL, value);
            }
        }


        private string _ReturnURL;
        public string ReturnURL
        {
            get { return _ReturnURL; }
            set {
                SetProperty(ref _ReturnURL, value);
            }
        }


        public ApplicationCharge()
        {
        }
    }
}
