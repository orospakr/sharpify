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
            Assert.AreEqual("bricks", ShopifyAPIClient.Pluralize("brick"));
            Assert.AreEqual("addresses", ShopifyAPIClient.Pluralize("address"));
            Assert.AreEqual("addresses", ShopifyAPIClient.Pluralize("ADDRESS"));
            Assert.AreEqual("apples", ShopifyAPIClient.Pluralize("apple"));
            Assert.AreEqual("theses", ShopifyAPIClient.Pluralize("thesis"));
            Assert.AreEqual("bacteria", ShopifyAPIClient.Pluralize("bacterium"));
            Assert.AreEqual("fungi", ShopifyAPIClient.Pluralize("fungus"));
            Assert.AreEqual("viruses", ShopifyAPIClient.Pluralize("virus"));
        }

        [Test]
        public void ShouldUnderscorify()
        {
            Assert.AreEqual("ben_the_benly_benis", ShopifyAPIClient.Underscoreify("BenTheBenlyBenis"));
            Assert.AreEqual("ben_the_benly_benis", ShopifyAPIClient.Underscoreify("benTheBenlyBenis"));
            Assert.AreEqual("browser_ip", ShopifyAPIClient.Underscoreify("BrowserIP"));
            Assert.AreEqual("total_price_usd", ShopifyAPIClient.Underscoreify("TotalPriceUSD"));
        }
    }
}
