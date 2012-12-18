using System;

namespace Sharpify
{
    public class ShopifyAuthorizationState
    {
        public ShopifyAuthorizationState()
        {
        }

        /// <summary>
        /// Shop name (as the .myshopify.com URI fragment)
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// Permanent access token for a given shop.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
