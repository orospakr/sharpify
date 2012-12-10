using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    //public interface IFragmentList<T> : IDirtiableList<T> where T : Fragment
    //{
    //}

    public class Fragment : DirtiableObject
    {
    }

    public class FragmentList<T> : DirtiableList<T> where T : Fragment
    {
    }

    public class Property : Fragment
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        private string _Value;
        public string Value
        {
            get { return _Value; }
            set {
                SetProperty(ref _Value, value);
            }
        }

    }

    public class Receipt : Fragment
    {
        private bool? _Testcase;
        public bool? Testcase
        {
            get { return _Testcase; }
            set {
                SetProperty(ref _Testcase, value);
            }
        }


        private string _Authorization;
        public string Authorization
        {
            get { return _Authorization; }
            set {
                SetProperty(ref _Authorization, value);
            }
        }


        public Receipt()
        {
        }
    }

    public class DiscountCode : Fragment
    {
        private string _Code;
        public string Code
        {
            get { return _Code; }
            set {
                SetProperty(ref _Code, value);
            }
        }

        private double? _Amount;
        public double? Amount
        {
            get { return _Amount; }
            set {
                SetProperty(ref _Amount, value);
            }
        }


        public DiscountCode()
        {
        }
    }

    public class Rule : Fragment
    {
        private string _Column;
        public string Column
        {
            get { return _Column; }
            set {
                SetProperty(ref _Column, value);
            }
        }


        // TODO: Enum
        private string _Relation;
        public string Relation
        {
            get { return _Relation; }
            set {
                SetProperty(ref _Relation, value);
            }
        }


        private string _Condition;
        public string Condition
        {
            get { return _Condition; }
            set {
                SetProperty(ref _Condition, value);
            }
        }

    }

    public class ShippingLine : Fragment
    {
        private string _Code;
        public string Code
        {
            get { return _Code; }
            set {
                SetProperty(ref _Code, value);
            }
        }


        private double? _Price;
        public double? Price
        {
            get { return _Price; }
            set {
                SetProperty(ref _Price, value);
            }
        }


        private string _Source;
        public string Source
        {
            get { return _Source; }
            set {
                SetProperty(ref _Source, value);
            }
        }


        private string _Title;
        public string Title
        {
            get { return _Title; }
            set {
                SetProperty(ref _Title, value);
            }
        }


        public ShippingLine()
        {
        }
    }

    // Da, Comrade Mulroney.
    public class TaxLine : Fragment
    {
        private string _Price;
        public string Price
        {
            get { return _Price; }
            set {
                SetProperty(ref _Price, value);
            }
        }


        private double? _Rate;
        public double? Rate
        {
            get { return _Rate; }
            set {
                SetProperty(ref _Rate, value);
            }
        }


        private string _Title;
        public string Title
        {
            get { return _Title; }
            set {
                SetProperty(ref _Title, value);
            }
        }


        public TaxLine()
        {
        }
    }

    public class PaymentDetails : Fragment
    {
        private string _AvsResultCode;
        public string AvsResultCode
        {
            get { return _AvsResultCode; }
            set {
                SetProperty(ref _AvsResultCode, value);
            }
        }


        private string _CreditCardBin;
        public string CreditCardBin
        {
            get { return _CreditCardBin; }
            set {
                SetProperty(ref _CreditCardBin, value);
            }
        }


        private string _CvvResultCode;
        public string CvvResultCode
        {
            get { return _CvvResultCode; }
            set {
                SetProperty(ref _CvvResultCode, value);
            }
        }


        private string _CreditCardNumber;
        public string CreditCardNumber
        {
            get { return _CreditCardNumber; }
            set {
                SetProperty(ref _CreditCardNumber, value);
            }
        }


        private string _CreditCardCompany;
        public string CreditCardCompany
        {
            get { return _CreditCardCompany; }
            set {
                SetProperty(ref _CreditCardCompany, value);
            }
        }


        public PaymentDetails()
        {
        }
    }

    public class ClientDetails : Fragment
    {
        /// <summary>
        /// Presumably in ISO 639-2?
        /// </summary>
        private string _AcceptLanguage;
        public string AcceptLanguage
        {
            get { return _AcceptLanguage; }
            set {
                SetProperty(ref _AcceptLanguage, value);
            }
        }


        private string _BrowserIP;
        public string BrowserIP
        {
            get { return _BrowserIP; }
            set {
                SetProperty(ref _BrowserIP, value);
            }
        }


        private string _SessionHash;
        public string SessionHash
        {
            get { return _SessionHash; }
            set {
                SetProperty(ref _SessionHash, value);
            }
        }


        private string _UserAgent;
        public string UserAgent
        {
            get { return _UserAgent; }
            set {
                SetProperty(ref _UserAgent, value);
            }
        }


        public ClientDetails()
        {
        }
    }


    /// <summary>
    /// Address (used as either billing or shipping addresses), suitable
    /// for placement on a shipping label for international routing to the
    /// addressee.
    /// </summary>
    public class Address : Fragment
    {
        /// <summary>
        /// Line 1 of the street address.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Line 2 of the street address.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// City's written name, suitable for the delivery locale.
        /// </summary>
        private string _City;
        public string City
        {
            get { return _City; }
            set {
                SetProperty(ref _City, value);
            }
        }


        private string _Company;
        public string Company
        {
            get { return _Company; }
            set {
                SetProperty(ref _Company, value);
            }
        }


        /// <summary>
        /// Country's written name, suitable for the delivery locale.
        /// </summary>
        private string _Country;
        public string Country
        {
            get { return _Country; }
            set {
                SetProperty(ref _Country, value);
            }
        }


        private string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set {
                SetProperty(ref _FirstName, value);
            }
        }


        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set {
                SetProperty(ref _LastName, value);
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


        private double? _Latitude;
        public double? Latitude
        {
            get { return _Latitude; }
            set {
                SetProperty(ref _Latitude, value);
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
        /// Province/State subdivision written name, suitable for the delivery locale.
        /// </summary>
        private string _Provice;
        public string Provice
        {
            get { return _Provice; }
            set {
                SetProperty(ref _Provice, value);
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


        /// <summary>
        /// Addressee's full name.
        /// </summary>
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        /// <summary>
        /// ISO 3166-1.
        /// </summary>
        private string _CountryCode;
        public string CountryCode
        {
            get { return _CountryCode; }
            set {
                SetProperty(ref _CountryCode, value);
            }
        }


        /// <summary>
        /// ISO 3166-2.
        /// </summary>
        private string _ProviceCode;
        public string ProviceCode
        {
            get { return _ProviceCode; }
            set {
                SetProperty(ref _ProviceCode, value);
            }
        }

    }

    /// <summary>
    /// Per-order line items.  Seems to include a lot of content from Variant.
    /// 
    /// Could have been a full (sub)resource, but it is not exposed by Shopify except
    /// as inlines (there is no matching REST interface, so it can't be a full has_many)
    /// </summary>
    public class LineItem : Fragment
    {
        // TODO updated_at/created_at


        // TODO: Enum (or do we?  how changeable is the fulfillment services list?)
        private string _FulfillmentService;
        public string FulfillmentService
        {
            get { return _FulfillmentService; }
            set {
                SetProperty(ref _FulfillmentService, value);
            }
        }


        private int? _Grams;
        public int? Grams
        {
            get { return _Grams; }
            set {
                SetProperty(ref _Grams, value);
            }
        }


        private string _Id;
        public string Id
        {
            get { return _Id; }
            set {
                SetProperty(ref _Id, value);
            }
        }


        private double _Price;
        public double Price
        {
            get { return _Price; }
            set {
                SetProperty(ref _Price, value);
            }
        }


        // I hope this works okay without being nested in a IResourceModel...
        private IHasOne<Product> _Product;
        public IHasOne<Product> Product
        {
            get { return _Product; }
            set {
                SetProperty(ref _Product, value);
            }
        }


        private int _Quantity;
        public int Quantity
        {
            get { return _Quantity; }
            set {
                SetProperty(ref _Quantity, value);
            }
        }


        private bool _RequiresShipping;
        public bool RequiresShipping
        {
            get { return _RequiresShipping; }
            set {
                SetProperty(ref _RequiresShipping, value);
            }
        }


        private string _SKU;
        public string SKU
        {
            get { return _SKU; }
            set {
                SetProperty(ref _SKU, value);
            }
        }


        /// <summary>
        /// Seems to be the title of the referenced Product.
        /// </summary>
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set {
                SetProperty(ref _Title, value);
            }
        }


        private IHasOne<Variant> _Variant;
        public IHasOne<Variant> Variant
        {
            get { return _Variant; }
            set {
                SetProperty(ref _Variant, value);
            }
        }


        /// <summary>
        /// A copy of the title of the reference Variant, for convenience.
        /// </summary>
        private string _VariantTitle;
        public string VariantTitle
        {
            get { return _VariantTitle; }
            set {
                SetProperty(ref _VariantTitle, value);
            }
        }


        private string _Vendor;
        public string Vendor
        {
            get { return _Vendor; }
            set {
                SetProperty(ref _Vendor, value);
            }
        }


        /// <summary>
        /// Seems to be the Title and the Variant title concatenated.
        /// </summary>
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }


        // TODO: not really sure what this is
        private string _VariantInventoryManagement;
        public string VariantInventoryManagement
        {
            get { return _VariantInventoryManagement; }
            set {
                SetProperty(ref _VariantInventoryManagement, value);
            }
        }

        private FragmentList<Property> _Properties;
        public FragmentList<Property> Properties
        {
            get { return _Properties; }
            set {
                SetProperty(ref _Properties, value);
            }
        }

    }
}
