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

    public class Bank : IResourceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
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

        public string FinancialStatus { get; set; }

        public ICollection<Tax> Taxes { get; set; }

        public IHasMany<SKU> SKUs { get; set; }

        public IHasOne<Bank> Bank { get; set; }

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
            var fixture = @"{""transaction"": {""Id"": ""56"", ""currency"": ""CAD"", ""value"": 78.45, ""financial_status"": ""authorized""}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);
            Assert.AreEqual("56", decoded.Id);
            Assert.AreEqual("CAD", decoded.Currency);
            Assert.AreEqual(78.45, decoded.Value);
            Assert.AreEqual("authorized", decoded.FinancialStatus);
        }

        [Test]
        public void ShouldSerializeSimpleObject()
        {
            var c = new Transaction() { Id = "788",
                Currency = "CAD",
                Value = 25.60,
                Receipient = "Jaded Pixel Technologies",
                FinancialStatus = "refunded"
            };

            // this bit is for testing that the SubResource proxies
            // don't mess with the serialization process
            var shopify = A.Fake<IShopifyAPIClient>();
            var transactions = new RestResource<Transaction>(shopify, "transaction");
            c.SKUs = new SubResource<SKU>(transactions, c, "sku");

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", c);

            // use late-binding to concisely test expected fields
            // in the JSON
            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            Assert.AreEqual("788", decoded.transaction.id.ToString());
            Assert.AreEqual("CAD", decoded.transaction.currency.ToString());
            Assert.AreEqual("Jaded Pixel Technologies", decoded.transaction.receipient.ToString());
            Assert.AreEqual("refunded", decoded.transaction.financial_status.ToString());
            
            DataTranslator.Decode(encoded);
        }

        [Test]
        public void ShouldDeserializeObjectWithInlineObject()
        {
            var fixture = @"{""transaction"": {""id"": 48, ""currency"": ""USD"", ""taxes"": [" +
                @"{""region"": ""Illinois"", ""percentage"": 6.25}]}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);
            Assert.AreEqual("48", decoded.Id);
            Assert.AreEqual(1, decoded.Taxes.Count);
            Assert.AreEqual(6.25, decoded.Taxes.ElementAt(0).Percentage);
        }

        [Test]
        public void ShouldSerializeObjectWithInlineObject()
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
            Assert.AreEqual("77", decoded.transaction.id.ToString());
            Assert.AreEqual(1, decoded.transaction.taxes.Count);
            Assert.AreEqual("New Zealand", decoded.transaction.taxes[0].region.ToString());
        }

        [Test]
        public void ShouldDeserializeObjectWithAnInlineHasOne()
        {
            var fixture = @"{""transaction"": {""id"": 48, ""currency"": ""USD"", ""bank"": {""id"": 18, ""name"": ""Mulligan Bank"" }, ""taxes"": [" +
               @"{""region"": ""Illinois"", ""percentage"": 6.25}]}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);

            // validate that the usual fields are still ok
            Assert.AreEqual("48", decoded.Id);
            Assert.AreEqual(1, decoded.Taxes.Count);

            // Hm, one problem with implementation:

            // say I have a different IHasA for inlines.  great and all, but:
            // the interface for saving must necessarily differ:
            //   -- the inline version can happily reserialize into inline, but, the user will get differing behaviour:
            //      if they save the belonged to object, it'll have different behaviour depending on which version they have
            //      one will save the _id, and one will save the the entire contents (benefit is that it's the same as it came,
            //      limitation is that user gets different behaviour with same interface.

            // what does activeresource do?


            // leaning towards non-inlined saving.  can't

            var bankAnswer = decoded.Bank.Get();
            bankAnswer.Wait();

            Assert.AreEqual("Mulligan Bank", bankAnswer.Result.Name);
            Assert.AreEqual("18", bankAnswer.Result.Id);
        }

        [Test]
        public void ShouldInsertHasAPlaceHoldersWhenDeserializing()
        {
            var fixture = @"{""transaction"": {""id"": 48, ""currency"": ""USD"", ""bank_id"": 18, ""taxes"": [" +
               @"{""region"": ""Illinois"", ""percentage"": 6.25}]}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);

            // validate that the usual fields are still ok
            Assert.AreEqual("48", decoded.Id);
            Assert.AreEqual(1, decoded.Taxes.Count);

            Assert.IsInstanceOf<HasOneDeserializationPlaceholder<Bank>>(decoded.Bank);
            Assert.AreEqual("18", decoded.Bank.Id);
        }

        [Test]
        public void ShouldSerializeObjectWithAHasOne()
        {
            var t = new Transaction()
            {
                Id = "99",
                Currency = "EUR",
                Receipient = "Somewhere",
                Bank = new HasOneDeserializationPlaceholder<Bank>("88")
            };

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", t);

            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            Assert.AreEqual("99", decoded.transaction.id.ToString());
            Assert.AreEqual("88", decoded.transaction.bank_id.ToString());
        }

        [Test]
        public void ShouldSerializeWhileIgnoringHasMany()
        {
            // https://trello.com/card/inlined-has-many/50a1c9c990c4980e0600178b/23
            // while the Shopify REST API does support inlining has_many,
            // we don't support it yet.

            var hasMany = A.Fake<IHasMany<SKU>>();

            var t = new Transaction()
            {
                Id = "99",
                Currency = "EUR",
                SKUs = hasMany
            };

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", t);

            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            Assert.AreEqual("EUR", decoded.transaction.currency.ToString());
            Assert.AreEqual("99", decoded.transaction.id.ToString());
            Assert.IsNull(decoded.transaction.skus);
        }

        [Test]
        public void ShouldDeserializeWhileIgnoringHasMany()
        {
            // https://trello.com/card/inlined-has-many/50a1c9c990c4980e0600178b/23
            // while the Shopify REST API does support inlining has_many,
            // we don't support it yet.
            var fixture = @"{""transaction"": {""id"": 9345, ""currency"": ""EUR"", ""skus"": [ ] }}";

            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);

            Assert.AreEqual("9345", decoded.Id);

            // check that the has many instance itself is null (during normal operation,
            // the host resource doing the decoding will replace it with a SubResource.
            Assert.IsNull(decoded.SKUs);
        }
    }
}
