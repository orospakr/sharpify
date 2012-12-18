using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Models
{
    /// <summary>
    /// http://api.shopify.com/metafield.html
    /// 
    /// http://wiki.shopify.com/Metafield_%28API%29
    /// 
    /// Many Shopify resources permit decoration with metafields,
    /// which are simple typed, name/value pairs.
    /// 
    /// If you want your app to store some data against specific
    /// resources inside Shopify itself, Metafields appear to be the
    /// way to do it.  They have the benefit of being accessible through
    /// Liquid templates on Shopify itself, too.
    /// </summary>
    public class Metafield : ShopifyResourceModel, IFullMutable
    {
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set {
                SetProperty(ref _Description, value);
            }
        }


        private string _Key;
        public string Key
        {
            get { return _Key; }
            set {
                SetProperty(ref _Key, value);
            }
        }


        /// <summary>
        /// To avoid collisions, you can set a namespace named by
        /// your application for your metafields.
        /// </summary>
        private string _Namespace;
        public string Namespace
        {
            get { return _Namespace; }
            set {
                SetProperty(ref _Namespace, value);
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


        /// <summary>
        /// Type of the field, ie., string, integer.
        /// </summary>
        // TODO: Enum
        public string ValueType  { get; set; }

        /// <summary>
        /// Foreign key the record this metafield has been associated
        /// with.
        /// </summary>
        private int _OwnerID;
        public int OwnerID
        {
            get { return _OwnerID; }
            set {
                SetProperty(ref _OwnerID, value);
            }
        }


        /// <summary>
        /// Type of the record this metafield is associated with.
        /// 
        /// Underscorized/lowercased.
        /// </summary>
        private string _OwnerResource;
        public string OwnerResource
        {
            get { return _OwnerResource; }
            set {
                SetProperty(ref _OwnerResource, value);
            }
        }


        public Metafield()
        {
        }
    }
}
