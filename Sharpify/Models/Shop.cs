using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/shop.html
    /// 
    /// http://wiki.shopify.com/Shop_%28API%29
    /// 
    /// This resource is special in that it is a singleton
    /// (at least as far as a given auth context/ShopifyApiClient
    /// is concerned), and as such is not hosted in a RestResource.
    /// </summary>
    public class Shop : ShopifyResourceModel
    {
        public string Address1 { get; set; }

        private string _City;
        public string City
        {
            get { return _City; }
            set {
                SetProperty(ref _City, value);
            }
        }


        private string _CustomerEmail;
        public string CustomerEmail
        {
            get { return _CustomerEmail; }
            set {
                SetProperty(ref _CustomerEmail, value);
            }
        }


        /// <summary>
        /// Shop-owner entered human-readable name of country.
        /// </summary>
        private string _Country;
        public string Country
        {
            get { return _Country; }
            set {
                SetProperty(ref _Country, value);
            }
        }


        private string _Domain;
        public string Domain
        {
            get { return _Domain; }
            set {
                SetProperty(ref _Domain, value);
            }
        }


        private string _Email;
        public string Email
        {
            get { return _Email; }
            set {
                SetProperty(ref _Email, value);
            }
        }


        private double? _Latitude;
        public double? Latitude
        {
            get { return _Latitude; }
            set {
                SetProperty(ref _Latitude, value);
            }
        }


        private double? _Longitude;
        public double? Longitude
        {
            get { return _Longitude; }
            set {
                SetProperty(ref _Longitude, value);
            }
        }


        /// <summary>
        /// Human-readable (not URI name) of the Shop.
        /// </summary>
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        private string _Phone;
        public string Phone
        {
            get { return _Phone; }
            set {
                SetProperty(ref _Phone, value);
            }
        }


        /// <summary>
        /// Shop-owner entered human-readable name of province.
        /// </summary>
        private string _Provice;
        public string Provice
        {
            get { return _Provice; }
            set {
                SetProperty(ref _Provice, value);
            }
        }


        // UNKNOWN
        private string _Public;
        public string Public
        {
            get { return _Public; }
            set {
                SetProperty(ref _Public, value);
            }
        }


        // UNKNOWN
        private string _Source;
        public string Source
        {
            get { return _Source; }
            set {
                SetProperty(ref _Source, value);
            }
        }


        private string _Zip;
        public string Zip
        {
            get { return _Zip; }
            set {
                SetProperty(ref _Zip, value);
            }
        }


        private string _Currency;
        public string Currency
        {
            get { return _Currency; }
            set {
                SetProperty(ref _Currency, value);
            }
        }


        private string _Timezone;
        public string Timezone
        {
            get { return _Timezone; }
            set {
                SetProperty(ref _Timezone, value);
            }
        }


        /// <summary>
        /// Full name of the shop owner.
        /// </summary>
        private string _ShopOwner;
        public string ShopOwner
        {
            get { return _ShopOwner; }
            set {
                SetProperty(ref _ShopOwner, value);
            }
        }


        /// <summary>
        /// Some sort of Liquid snippet for formatting the money type
        /// in views.
        /// </summary>
        private string _MoneyFormat;
        public string MoneyFormat
        {
            get { return _MoneyFormat; }
            set {
                SetProperty(ref _MoneyFormat, value);
            }
        }


        /// <summary>
        /// Liquid snippet for formatting the money type, but additionally
        /// including the currency name.
        /// </summary>
        private string _MoneyWithCurrencyFormat;
        public string MoneyWithCurrencyFormat
        {
            get { return _MoneyWithCurrencyFormat; }
            set {
                SetProperty(ref _MoneyWithCurrencyFormat, value);
            }
        }


        // UNKNOWN
        private string _TaxesIncluded;
        public string TaxesIncluded
        {
            get { return _TaxesIncluded; }
            set {
                SetProperty(ref _TaxesIncluded, value);
            }
        }


        // UNKNOWN
        private string _TaxShipping;
        public string TaxShipping
        {
            get { return _TaxShipping; }
            set {
                SetProperty(ref _TaxShipping, value);
            }
        }


        // TODO: Enum?
        private string _PlanName;
        public string PlanName
        {
            get { return _PlanName; }
            set {
                SetProperty(ref _PlanName, value);
            }
        }


        private string _MyshopifyDomain;
        public string MyshopifyDomain
        {
            get { return _MyshopifyDomain; }
            set {
                SetProperty(ref _MyshopifyDomain, value);
            }
        }


        private string _GoogleAppsDomain;
        public string GoogleAppsDomain
        {
            get { return _GoogleAppsDomain; }
            set {
                SetProperty(ref _GoogleAppsDomain, value);
            }
        }


        private bool? _GooleAppsLoginEnabled;
        public bool? GooleAppsLoginEnabled
        {
            get { return _GooleAppsLoginEnabled; }
            set {
                SetProperty(ref _GooleAppsLoginEnabled, value);
            }
        }


        public Shop()
        {
        }
    }
}
