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

        [Test]
        public void ShouldPluralize() {
            Assert.AreEqual("sandwiches", ShopifyAPIClient.Pluralize("sandwich"));
            Assert.AreEqual("kitties", ShopifyAPIClient.Pluralize("kitty"));
            Assert.AreEqual("apples", ShopifyAPIClient.Pluralize("apple"));
            // yes, I know about police. I don't care :)
        }
    }
}
