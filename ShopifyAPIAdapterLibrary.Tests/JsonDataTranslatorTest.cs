using FakeItEasy;
using Newtonsoft.Json;
using NUnit.Framework;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests
{
    public class Tax
    {
        public string Region { get; set; }

        public string Name { get; set; }

        public double Percentage { get; set; }
    }

    // a subresource that we won't do much with.
    // the DataTranslator is not responsible for inserting
    // subresource proxies.
    public class SKU : IResourceModel
    {
        public string Id { get; set; }
    }

    public class Transaction : IResourceModel {
        public string Currency { get; set; }

        public double Value { get; set; }

        public string Receipient { get; set; }

        public ICollection<Tax> Taxes { get; set; }

        public ISubResource<SKU> SKUs { get; set; }

        public string Id { get; set; }
    }

    [TestFixture]
    class JsonDataTranslatorTest
    {
        public JsonDataTranslator DataTranslator { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            DataTranslator = new JsonDataTranslator();
        }

        [Test]
        public void ShouldDeserializeSimpleObject()
        {
            var fixture = @"{""transaction"": {""Id"": ""56"", ""Currency"": ""CAD"", ""Value"": 78.45}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);
            Assert.AreEqual("56", decoded.Id);
            Assert.AreEqual("CAD", decoded.Currency);
            Assert.AreEqual(78.45, decoded.Value);
        }

        [Test]
        public void ShouldSerializeSimpleObject()
        {
            var c = new Transaction() { Id = "788",
                Currency = "CAD",
                Value = 25.60,
                Receipient = "Jaded Pixel Technologies" };

            // this bit is for testing that the SubResource proxies
            // don't mess with the serialization process
            var shopify = A.Fake<IShopifyAPIClient>();
            var transactions = new RestResource<Transaction>(shopify, "transaction");
            c.SKUs = new SubResource<SKU>(transactions, c, "sku");

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", c);

            // use late-binding to concisely test expected fields
            // in the JSON
            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            Assert.AreEqual("788", decoded.transaction.Id.ToString());
            Assert.AreEqual("CAD", decoded.transaction.Currency.ToString());
            Assert.AreEqual("Jaded Pixel Technologies", decoded.transaction.Receipient.ToString());
            
            DataTranslator.Decode(encoded);
        }

        [Test]
        public void ShouldDeserializeObjectWithInlineResource()
        {
            var fixture = @"{""transaction"": {""Id"": 48, ""Currency"": ""USD"", ""Taxes"": [" +
                @"{""Region"": ""Illinois"", ""Percentage"": 6.25}]}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);
            Assert.AreEqual("48", decoded.Id);
            Assert.AreEqual(1, decoded.Taxes.Count);
            Assert.AreEqual(6.25, decoded.Taxes.ElementAt(0).Percentage);
        }

        [Test]
        public void ShouldSerializeObjectWithInlineResource()
        {
            var c = new Transaction()
            {
                Id = "77",
                Currency = "NZD",
                Receipient = "Weta Workshop",
                Taxes = new List<Tax> {
                    new Tax { Region = "New Zealand", Percentage = 15}
                }
            };

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", c);

            dynamic decoded = JsonConvert.DeserializeObject(encoded);
            Assert.AreEqual("77", decoded.transaction.Id.ToString());
            Assert.AreEqual(1, decoded.transaction.Taxes.Count);
            Assert.AreEqual("New Zealand", decoded.transaction.Taxes[0].Region.ToString());
        }
    }
}
