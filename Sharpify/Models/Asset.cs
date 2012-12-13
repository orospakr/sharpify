using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/asset.html
    /// 
    /// http://wiki.shopify.com/Asset_%28API%29
    /// </summary>
    public class Asset : ShopifyResourceModel, IFullMutable
    {

        private string _Key;
        public string Key
        {
            get { return _Key; }
            set {
                SetProperty(ref _Key, value);
            }
        }


        private string _PublicURL;
        public string PublicURL
        {
            get { return _PublicURL; }
            set {
                SetProperty(ref _PublicURL, value);
            }
        }


        private string _ContentType;
        public string ContentType
        {
            get { return _ContentType; }
            set {
                SetProperty(ref _ContentType, value);
            }
        }


        private long _Size;
        public long Size
        {
            get { return _Size; }
            set {
                SetProperty(ref _Size, value);
            }
        }


        private int _ThemeId;
        public int ThemeId
        {
            get { return _ThemeId; }
            set {
                SetProperty(ref _ThemeId, value);
            }
        }


        public Asset()
        {
        }
    }
}
