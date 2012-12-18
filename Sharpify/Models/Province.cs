using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/province.html
    /// 
    /// http://wiki.shopify.com/Province_%28API%29
    /// </summary>
    public class Province: ShopifyResourceModel, IMutable
    {
        // actually not deletable, but ISaveable will do.

        /// <summary>
        /// ISO 3166-2 Alpha 2 Subdivision Code.
        /// </summary>
        private string _Code;
        public string Code
        {
            get { return _Code; }
            set {
                SetProperty(ref _Code, value);
            }
        }


        /// <summary>
        /// Subdivision-left sales tax rate.
        /// </summary>
        private double _Tax;
        public double Tax
        {
            get { return _Tax; }
            set {
                SetProperty(ref _Tax, value);
            }
        }


        /// <summary>
        /// I don't know what differentiates this from Tax.
        ///  
        /// Shopify's doc doesn't clarify.
        /// </summary>
        private double _TaxPercentage;
        public double TaxPercentage
        {
            get { return _TaxPercentage; }
            set {
                SetProperty(ref _TaxPercentage, value);
            }
        }


        private string _TaxType;
        public string TaxType
        {
            get { return _TaxType; }
            set {
                SetProperty(ref _TaxType, value);
            }
        }


        private string _TaxName;
        public string TaxName
        {
            get { return _TaxName; }
            set {
                SetProperty(ref _TaxName, value);
            }
        }


        private IHasOne<Country> _Country;
        public IHasOne<Country> Country
        {
            get { return _Country; }
            set {
                SetProperty(ref _Country, value);
            }
        }


        public Province()
        {
        }
    }
}
