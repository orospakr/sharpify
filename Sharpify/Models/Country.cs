using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/country.html
    /// 
    /// http://wiki.shopify.com/Countries_%28API%29
    /// </summary>
    public class Country : ShopifyResourceModel, IFullMutable
    {
        /// <summary>
        /// ISO 3166-1 Alpha-2 Country Code.
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
        /// Federal-level sales tax rate.
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
        /// Name, as whatever the shop owner entered.  Locale unspecified.
        /// </summary>
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        private IHasMany<Province> _Provinces;
        public IHasMany<Province> Provinces
        {
            get { return _Provinces; }
            set {
                SetProperty(ref _Provinces, value);
            }
        }


        public Country()
        {
        }
    }
}