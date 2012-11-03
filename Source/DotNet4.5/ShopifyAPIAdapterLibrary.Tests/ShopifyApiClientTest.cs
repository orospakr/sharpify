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
            Assert.AreEqual("/admin/products/67", sapi.ProductPath("67"));
        }
    }
}
