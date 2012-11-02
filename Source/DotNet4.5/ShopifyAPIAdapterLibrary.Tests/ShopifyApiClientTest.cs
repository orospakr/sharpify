using System;
using NUnit.Framework;

namespace ShopifyAPIAdapterLibrary.Tests
{
    [TestFixture]
    public class ShopifyApiClientTest
    {
        public ShopifyApiClientTest ()
        {
        }

        [Test]
        public void ShouldCreateProductUri() {
            var authState = new ShopifyAuthorizationState() {AccessToken = "beep boop", ShopName = "chucks-chili-dogs"};
            var sapi = new ShopifyAPIClient(authState);
            Assert.AreEqual("/products/67", sapi.ProductPath("67"));
            // TODO start here and figure out how and where we should be decorating with the format component
            // does shopify's API honour the Accept: header?
        }
    }
}
