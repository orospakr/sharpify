using System;
using NUnit.Framework;

namespace Sharpify.Tests
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
            var sapi = new ShopifyAPIContext(authState);
            Assert.AreEqual("/admin/products/67", sapi.ProductPath("67"));
        }

       
        [Test]
        public void ShouldPluralize() {
            Assert.AreEqual("sandwiches", ShopifyAPIContext.Pluralize("sandwich"));
            Assert.AreEqual("kitties", ShopifyAPIContext.Pluralize("kitty"));
            Assert.AreEqual("bricks", ShopifyAPIContext.Pluralize("brick"));
            Assert.AreEqual("addresses", ShopifyAPIContext.Pluralize("address"));
            Assert.AreEqual("addresses", ShopifyAPIContext.Pluralize("ADDRESS"));
            Assert.AreEqual("apples", ShopifyAPIContext.Pluralize("apple"));
            Assert.AreEqual("theses", ShopifyAPIContext.Pluralize("thesis"));
            Assert.AreEqual("bacteria", ShopifyAPIContext.Pluralize("bacterium"));
            Assert.AreEqual("fungi", ShopifyAPIContext.Pluralize("fungus"));
            Assert.AreEqual("viruses", ShopifyAPIContext.Pluralize("virus"));
        }

        [Test]
        public void ShouldUnderscorify()
        {
            Assert.AreEqual("ben_the_benly_benis", ShopifyAPIContext.Underscoreify("BenTheBenlyBenis"));
            Assert.AreEqual("ben_the_benly_benis", ShopifyAPIContext.Underscoreify("benTheBenlyBenis"));
            Assert.AreEqual("browser_ip", ShopifyAPIContext.Underscoreify("BrowserIP"));
            Assert.AreEqual("total_price_usd", ShopifyAPIContext.Underscoreify("TotalPriceUSD"));
        }
    }
}
