using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    public class Fragment : DirtiableObject
    {
    }

    public class Property : Fragment
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class Receipt : Fragment
    {
        public bool? Testcase { get; set; }

        public string Authorization { get; set; }

        public Receipt()
        {
        }
    }

    public class DiscountCode : Fragment
    {
        public string Code { get; set; }
        public double? Amount { get; set; }

        public DiscountCode()
        {
        }
    }

    public class Rule : Fragment
    {
        public string Column { get; set; }

        // TODO: Enum
        public string Relation { get; set; }

        public string Condition { get; set; }
    }

    public class ShippingLine : Fragment
    {
        public string Code { get; set; }

        public double? Price { get; set; }

        public string Source { get; set; }

        public string Title { get; set; }

        public ShippingLine()
        {
        }
    }

    // Da, Comrade Mulroney.
    public class TaxLine : Fragment
    {
        public string Price { get; set; }

        public double? Rate { get; set; }

        public string Title { get; set; }

        public TaxLine()
        {
        }
    }

    public class PaymentDetails : Fragment
    {
        public string AvsResultCode { get; set; }

        public string CreditCardBin { get; set; }

        public string CvvResultCode { get; set; }

        public string CreditCardNumber { get; set; }

        public string CreditCardCompany { get; set; }

        public PaymentDetails()
        {
        }
    }

    public class ClientDetails : Fragment
    {
        /// <summary>
        /// Presumably in ISO 639-2?
        /// </summary>
        public string AcceptLanguage { get; set; }

        public string BrowserIP { get; set; }

        public string SessionHash { get; set; }

        public string UserAgent { get; set; }

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
        public string City { get; set; }

        public string Company { get; set; }

        /// <summary>
        /// Country's written name, suitable for the delivery locale.
        /// </summary>
        public string Country { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// Province/State subdivision written name, suitable for the delivery locale.
        /// </summary>
        public string Provice { get; set; }

        public string Zip { get; set; }

        /// <summary>
        /// Addressee's full name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ISO 3166-1.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// ISO 3166-2.
        /// </summary>
        public string ProviceCode { get; set; }
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
        public string FulfillmentService { get; set; }

        public int? Grams { get; set; }

        public string Id { get; set; }

        public double Price { get; set; }

        // I hope this works okay without being nested in a IResourceModel...
        public IHasOne<Product> Product { get; set; }

        public int Quantity { get; set; }

        public bool RequiresShipping { get; set; }

        public string SKU { get; set; }

        /// <summary>
        /// Seems to be the title of the referenced Product.
        /// </summary>
        public string Title { get; set; }

        public IHasOne<Variant> Variant { get; set; }

        /// <summary>
        /// A copy of the title of the reference Variant, for convenience.
        /// </summary>
        public string VariantTitle { get; set; }

        public string Vendor { get; set; }

        /// <summary>
        /// Seems to be the Title and the Variant title concatenated.
        /// </summary>
        public string Name { get; set; }

        // TODO: not really sure what this is
        public string VariantInventoryManagement { get; set; }

        public IList<Property> Properties { get; set; }
    }
}
