using FakeItEasy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests
{
    public class Tax : Fragment
    {
        private string _Region;
        public string Region
        {
            get { return _Region; }
            set
            {
                SetProperty(ref _Region, value);
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                SetProperty(ref _Name, value);
            }
        }
        private double _Percentage;
        public double Percentage
        {
            get { return _Percentage; }
            set
            {
                SetProperty(ref _Percentage, value);
            }
        }
    }


    public class Bank : ShopifyResourceModel
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                SetProperty(ref _Name, value);
            }
        }

    }

    // a subresource that we won't do much with.
    // the DataTranslator is not responsible for inserting
    // subresource proxies.
    public class SKU : ShopifyResourceModel
    {
    }

    public class Transaction : ShopifyResourceModel
    {
        private string _Currency;
        public string Currency
        {
            get { return _Currency; }
            set {
                SetProperty(ref _Currency, value);
            }
        }


        private double _Value;
        public double Value
        {
            get { return _Value; }
            set {
                SetProperty(ref _Value, value);
            }
        }


        private string _Receipient;
        public string Receipient
        {
            get { return _Receipient; }
            set {
                SetProperty(ref _Receipient, value);
            }
        }


        private string _FinancialStatus;
        public string FinancialStatus
        {
            get { return _FinancialStatus; }
            set {
                SetProperty(ref _FinancialStatus, value);
            }
        }

        private string _Note;
        public string Note
        {
            get { return _Note; }
            set {
                SetProperty(ref _Note, value);
            }
        }


        private FragmentList<Tax> _Taxes;
        public FragmentList<Tax> Taxes
        {
            get { return _Taxes; }
            set {
                SetProperty(ref _Taxes, value);
            }
        }


        private IHasMany<SKU> _SKUs;
        public IHasMany<SKU> SKUs
        {
            get { return _SKUs; }
            set {
                SetProperty(ref _SKUs, value);
            }
        }


        private IHasOne<Bank> _Bank;
        public IHasOne<Bank> Bank
        {
            get { return _Bank; }
            set {
                SetProperty(ref _Bank, value);
            }
        }

        private IHasOne<Bank> _OriginatingInstitution;
        public IHasOne<Bank> OriginatingInstitution
        {
            get { return _OriginatingInstitution; }
            set
            {
                SetProperty(ref _OriginatingInstitution, value);
            }
        }

        private Tax _SingleTax;
        public Tax SingleTax {
            get { return _SingleTax; }
            set {
                SetProperty(ref _SingleTax, value);
            }
        }
    }

    // TODO: there is a certain argument for mocking IResourceModel instead of using real ones.
    // however, for now it's convenient to conflate testing both JsonDataTranslator and
    // integration with ShopifyResourceModel, and having Json.net operate correctly with
    // mocks would likely prove unhelpfully difficult.

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
            Assert.IsTrue(decoded.IsClean());
            Assert.AreEqual(56, decoded.Id);
            Assert.AreEqual("CAD", decoded.Currency);
            Assert.AreEqual(78.45, decoded.Value);
            Assert.AreEqual("authorized", decoded.FinancialStatus);
        }

        private bool ContainsField(IEnumerable<JProperty> props, string fieldName) {
            return (from p in props where p.Name == fieldName select p).Count() > 0;
        }

        [Test]
        public void ShouldSerializeSimpleObject()
        {
            var c = new Transaction() { Id = 788,
                Currency = "CAD",
                Value = 25.60,
                Receipient = "Jaded Pixel Technologies",
                FinancialStatus = "refunded"
            };

            // this bit is for testing that the SubResource proxies
            // don't mess with the serialization process
            var shopify = A.Fake<IShopifyAPIClient>();
            var transactions = new RestResource<Transaction>(shopify);
            c.SKUs = new SubResource<SKU>(transactions, c);

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", c);

            // use late-binding to concisely test expected fields
            // in the JSON
            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            Assert.AreEqual("788", decoded.transaction.id.ToString());
            Assert.AreEqual("CAD", decoded.transaction.currency.ToString());
            Assert.AreEqual("Jaded Pixel Technologies", decoded.transaction.receipient.ToString());
            Assert.AreEqual("refunded", decoded.transaction.financial_status.ToString());
            IEnumerable<JProperty> props = decoded.transaction.Properties();
            
            Assert.IsFalse(ContainsField(props, "note"), "There should be no note field in the produced json.");
            DataTranslator.Decode(encoded);
        }

        [Test]
        public void ShouldSerializeOnlyChangedFields()
        {
            var c = new Transaction()
            {
                Id = 88,
                Currency = "BTC",
                Value = 0.10,
                Receipient = "Sanders Aircraft",
                FinancialStatus = "new",
                Taxes = new FragmentList<Tax>{new Tax() { Name = "GST" }},
                SingleTax = new Tax() {Name = "Inline single flat object" },
                Bank = new HasOneDeserializationPlaceholder<Bank>(999),
                OriginatingInstitution = new HasOneDeserializationPlaceholder<Bank>(1000)
            };

            c.Reset();

            // set FinancialStatus, so it should appear in the JSON
            c.FinancialStatus = "completed";

            // set the originating institution, so it should appear in the JSON
            c.OriginatingInstitution = new HasOneDeserializationPlaceholder<Bank>(1001);

            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", c);

            // use late-binding to concisely test expected fields
            // in the JSON
            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            IEnumerable<JProperty> props = decoded.transaction.Properties();

            // Id should always go through
            Assert.AreEqual("88", decoded.transaction.id.ToString());

            Assert.IsFalse(ContainsField(props, "currency"));
            Assert.IsFalse(ContainsField(props, "value"));
            Assert.IsFalse(ContainsField(props, "receipient"));
            Assert.IsFalse(ContainsField(props, "bank"));
            Assert.IsFalse(ContainsField(props, "bank_id"));

            Assert.IsFalse(ContainsField(props, "originating_institution"));

            Assert.AreEqual("completed", decoded.transaction.financial_status.ToString(), "changed field should turn up in JSON");

            Assert.AreEqual("1001", decoded.transaction.originating_institution_id.ToString(), "changed has_one should turn up in JSON");

            // inlined flat objects (this does not refer to has ones!) for now have no update tracking.
            // they must always be included inline, for now, since we can't
            // know if their innards have changed
            Assert.AreEqual("Inline single flat object", decoded.transaction.single_tax.name.ToString());

            // lists of inline flats also must also always go through
            Assert.AreEqual("GST", decoded.transaction.taxes[0].name.ToString());
        }

        [Test]
        public void ShouldSerializeResourceWithAnInlineHasOne()
        {
            var c = new Transaction()
            {
                Id = 88,
                Currency = "BTC",
                Value = 0.10,
                Receipient = "Sanders Aircraft"
            };

            c.Reset();

            var bank = new Bank() { Id = 77, Name = "Commonwealth Shared Risk" };
            c.Bank = new HasOneInline<Bank>(bank);


            var encoded = DataTranslator.ResourceEncode<Transaction>("transaction", c);

            // use late-binding to concisely test expected fields
            // in the JSON
            dynamic decoded = JsonConvert.DeserializeObject(encoded);

            IEnumerable<JProperty> props = decoded.transaction.Properties();

            Assert.AreEqual("88", decoded.transaction.id.ToString());

            // as it should be serialized inline, an as-a-id field should not appear.

            Assert.IsFalse(ContainsField(props, "bank_id"));
            Assert.AreEqual("77", decoded.transaction.bank.id.ToString());
            Assert.AreEqual("Commonwealth Shared Risk", decoded.transaction.bank.name.ToString());
        }

        [Test]
        public void ShouldDeserializeObjectWithInlineArbitraryObject()
        {
            // not to be confused with an inline HasOne
            var fixture = @"{""transaction"": {""id"": 48, ""currency"": ""USD"", ""taxes"": [" +
                @"{""region"": ""Illinois"", ""percentage"": 6.25}]}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);
            Assert.IsTrue(decoded.IsClean());
            Assert.IsFalse(decoded.IsNew());
            Assert.AreEqual(48, decoded.Id);
            Assert.AreEqual(1, decoded.Taxes.Count);
            Assert.AreEqual(6.25, decoded.Taxes.ElementAt(0).Percentage);
        }

        [Test]
        public void ShouldSerializeObjectWithInlineFragmentObject()
        {
            var c = new Transaction()
            {
                Id = 77,
                Currency = "NZD",
                Receipient = "Weta Workshop",
                Taxes = new FragmentList<Tax> {
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
            Assert.AreEqual(48, decoded.Id);
            Assert.AreEqual(1, decoded.Taxes.Count);
            Assert.IsTrue(decoded.IsClean());
            Assert.IsFalse(decoded.IsNew());

            var bankAnswer = decoded.Bank.Get();
            bankAnswer.Wait();

            Assert.IsFalse(bankAnswer.Result.IsNew());
            Assert.IsTrue(bankAnswer.Result.IsClean());
            Assert.AreEqual("Mulligan Bank", bankAnswer.Result.Name);
            Assert.AreEqual(18, bankAnswer.Result.Id);
        }

        [Test]
        public void ShouldInsertHasAPlaceHoldersWhenDeserializing()
        {
            var fixture = @"{""transaction"": {""id"": 48, ""currency"": ""USD"", ""bank_id"": 18, ""taxes"": [" +
               @"{""region"": ""Illinois"", ""percentage"": 6.25}]}}";
            var decoded = DataTranslator.ResourceDecode<Transaction>("transaction", fixture);

            // validate that the usual fields are still ok
            Assert.AreEqual(48, decoded.Id);
            Assert.IsTrue(decoded.IsClean());
            Assert.AreEqual(1, decoded.Taxes.Count);

            Assert.IsInstanceOf<HasOneDeserializationPlaceholder<Bank>>(decoded.Bank);
            Assert.AreEqual(18, decoded.Bank.Id);
        }

        [Test]
        public void ShouldSerializeObjectWithAHasOne()
        {
            var t = new Transaction()
            {
                Id = 99,
                Currency = "EUR",
                Receipient = "Somewhere",
                Bank = new HasOneDeserializationPlaceholder<Bank>(88)
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
                Id = 99,
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

            Assert.AreEqual(9345, decoded.Id);

            // check that the has many instance itself is null (during normal operation,
            // the host resource doing the decoding will replace it with a SubResource.
            Assert.IsNull(decoded.SKUs);
        }

        [Test]
        public void ShouldDeserializeShopifyDateFormat()
        {
            var fixture = @"""2009-01-31T20:00:00-04:00""";
            // the above time is midnight UTC written in atlantic time in ISO 8601
            var decoded = JsonConvert.DeserializeObject<DateTime>(fixture);
            Assert.AreEqual(2009, decoded.ToUniversalTime().Year);
            Assert.AreEqual(2, decoded.ToUniversalTime().Month);
            Assert.AreEqual(0, decoded.ToUniversalTime().Hour);
        }

        [Test]
        public void ShouldSerializeShopifyDateFormat()
        {
            var dt = new DateTime(2009, 7, 29, 14, 13, 45, 0, new System.Globalization.GregorianCalendar(), DateTimeKind.Utc);
            var encoded = JsonConvert.SerializeObject(dt);
            Assert.AreEqual(@"""2009-07-29T14:13:45Z""", encoded);
        }

        [Test]
        public void ShouldHaveInlineHasOneReturnModelDirtiness()
        {
            var transaction = A.Fake<Transaction>();
            A.CallTo(() => transaction.IsClean()).Returns(false);
            transaction.Id = 56;
            var x = new HasOneInline<Transaction>(transaction);
            Assert.IsFalse(x.IsClean());
        }
    }
}
