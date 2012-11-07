using NSubstitute;
using NUnit.Framework;
using System;

namespace ShopifyAPIAdapterLibrary.Tests
{
    public class Robot {
        
    }

    [TestFixture]
    public class RestResourceTest
    {
        public RestResourceTest ()
        {
            
        }

        public IShopifyAPIClient Shopify { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Shopify = Substitute.For<IShopifyAPIClient>();
        }

        [Test]
        public void ShouldAmalgamateQueryParametersWithWhere()
        {
            Shopify.AdminPath().Returns<string>("/admin");

            var rr = new RestResource<Robot>(Shopify, "robot");

            Assert.AreEqual("/admin/robots", rr.Path());

            var rrNexus = rr.Where("robot_type", "nexus");

            var rrNexusParameters = rrNexus.FullParameters();
            Assert.AreEqual("nexus", rrNexusParameters["robot_type"]);

            var rrNexusByTyrell = rrNexus.Where("producer", "tyrell");
            var rrNexusByTyrellParameters = rrNexusByTyrell.FullParameters();
            Assert.AreEqual("nexus", rrNexusByTyrellParameters["robot_type"]);
            Assert.AreEqual("tyrell", rrNexusByTyrellParameters["producer"]);
        }

        [Test]
        public void ShouldAmalgamateQueryParmaetersWithWhereByMemberExpressions()
        {

        }


    }
}

