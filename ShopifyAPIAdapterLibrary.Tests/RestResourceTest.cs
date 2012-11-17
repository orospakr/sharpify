using FakeItEasy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ShopifyAPIAdapterLibrary;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests
{
    // our test top-level resource
    public class Robot : IResourceModel {
        public string Id { get; set; }

        public string RobotType { get; set; }
        public string Manufacturer { get; set; }

        public IHasMany<Part> Parts { get; set; }

        public IList<Inspection> Inspections { get; set; }

        public IHasOne<Brain> Brain { get; set; }
    }

    public class Brain : IResourceModel
    {
        public string Id { get; set; }
        public int SynapseCount { get; set; }
    }

    // our test subresource
    public class Part : IResourceModel
    {
        public string Id { get; set; }
        public string Sku { get; set; }
    }

    // our inlined resource
    public class Inspection
    {
        public DateTime InspectedAt { get; set; }
        public String Inspector { get; set; }
    }

    [TestFixture]
    public class RestResourceTest
    {
        public RestResourceTest ()
        {

        }

        public IShopifyAPIClient Shopify { get; set; }

        public RestResource<Robot> Robots { get; set; }

        public RestResource<Brain> Brains { get; set; }

        /// <summary>
        /// SubResources would normally be created dynamically
        /// and set as "proxies" as the collections backing subresource
        /// lists on a given model object.  We make one manually here.
        /// </summary>
        public IHasMany<Part> CalculonsParts { get; set; }

        public Robot Calculon { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Shopify = A.Fake<IShopifyAPIClient>();
            A.CallTo(() => Shopify.AdminPath()).Returns("/admin");
            A.CallTo(() => Shopify.GetRequestContentType()).Returns(new MediaTypeHeaderValue("application/json"));

            Robots = new RestResource<Robot>(Shopify, "robot");
            Brains = new RestResource<Brain>(Shopify, "brain");
            Calculon = new Robot() { Id = "42" };
            CalculonsParts = new SubResource<Part>(Robots, Calculon, "part");
        }

        [Test]
        public void ShouldGenerateCorrectBasePath()
        {
            Assert.AreEqual("/admin/robots", Robots.Path());
        }

        [Test]
        public void ShouldAmalgamateQueryParametersWithWhere()
        {
            var rrNexus = Robots.Where("robot_type", "nexus");

            var rrNexusParameters = rrNexus.FullParameters();
            Assert.AreEqual("nexus", rrNexusParameters["robot_type"]);

            var rrNexusByTyrell = rrNexus.Where("manufacturer", "tyrell");
            var rrNexusByTyrellParameters = rrNexusByTyrell.FullParameters();
            Assert.AreEqual("nexus", rrNexusByTyrellParameters["robot_type"]);
            Assert.AreEqual("tyrell", rrNexusByTyrellParameters["manufacturer"]);
        }

        [Test]
        public void ShouldAmalgamateQueryParmaetersWithWhereByMemberExpressions()
        {
            var rrNexus = Robots.Where(r => r.RobotType, "nexus");
            var rrNexusParameters = rrNexus.FullParameters();
            Assert.AreEqual("nexus", rrNexusParameters["RobotType"]);
        }

        private Task<T> TaskForResult<T>(T input)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(input);
            return tcs.Task;
        }

        [Test]
        public void ShouldFetchAListOfAllMatchedModels()
        {
            var callRawExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots", EmptyQueryParametersExpectation(), null));
            callRawExpectation.Returns(TaskForResult<string>("json text!"));

            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<List<Robot>>("robots", "json text!"));
            translationExpectation.Returns(new List<Robot>() { new Robot() { Id = "fdaf" } } );

            var answer = Robots.AsList();

            answer.Wait();

            callRawExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();

            Assert.AreEqual(1, answer.Result.Count);
            Assert.NotNull(answer.Result[0].Parts);
            Assert.AreEqual("/admin/robots/fdaf/parts", answer.Result[0].Parts.Path());
        }

        [Test]
        public void ShouldBuildSubResourcePaths()
        {
            Assert.AreEqual("/admin/robots/42/parts", CalculonsParts.Path());
            Assert.AreEqual("/admin/robots/42/parts/67", CalculonsParts.InstancePath("67"));
        }

        private MediaTypeHeaderValue JsonFormatExpectation() {
            return A<MediaTypeHeaderValue>.That.Matches(mt => mt.ToString() == "application/json");
        }

        private String JsonContentsExpectation(Func<object, bool> predicate) {
            return A<String>.That.Matches(json => predicate(JsonConvert.DeserializeObject(json)));
        }

        private NameValueCollection EmptyQueryParametersExpectation()
        {
            return A<NameValueCollection>.That.Matches(nvc => nvc.Keys.Count == 0);
        }

        [Test]
        public void ShouldFetchARecord()
        {
            var callRawExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/89", EmptyQueryParametersExpectation(), null));
            callRawExpectation.Returns(TaskForResult<string>("robot #89's json"));

            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<Robot>("robot", "robot #89's json"));
            var translatedRobot = new Robot { Id = "89" };

            //
            // TODO: .Get will start setting

            translationExpectation.Returns(translatedRobot);
            var answer = Robots.Get("89");
            answer.Wait();

            Assert.AreSame(answer.Result, translatedRobot);

            // check for the Parts subresource object
            Assert.AreEqual("/admin/robots/89/parts", answer.Result.Parts.Path());

            callRawExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();
        }

        [Test]
        public void ShouldCreateASubResourceRecord()
        {
            var partToPost = new Part() { Sku = "0xdeadbeef" };

            var translationExpectation = A.CallTo(() => Shopify.ObjectTranslate<Part>("part", partToPost));
            translationExpectation.Returns("PART 988 JSON");

            var postRawExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Post,
                JsonFormatExpectation(),
                "/admin/robots/42/parts", null, "PART 988 JSON"));
            postRawExpectation.Returns(TaskForResult<string>(""));

            var answer = CalculonsParts.Create(partToPost);

            answer.Wait();


            translationExpectation.MustHaveHappened();
            postRawExpectation.MustHaveHappened();
        }

        [Test]
        public void ShouldUpdateASubResourceRecord()
        {
            var partToPost = new Part() { Id = "9777" };
            var translationExpectation = A.CallTo(() => Shopify.ObjectTranslate<Part>("part", partToPost));
            translationExpectation.Returns("PART 988 JSON");

            var putRawExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Put,
                JsonFormatExpectation(),
                "/admin/robots/42/parts/9777", null, "PART 988 JSON"));
            putRawExpectation.Returns(TaskForResult<string>(""));

            var answer = CalculonsParts.Update(partToPost);

            answer.Wait();

            translationExpectation.MustHaveHappened();
            putRawExpectation.MustHaveHappened();
        }

        [Test]
        public void ShouldFetchASubResourceRecord()
        {
            var callRawExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/42/parts/69", EmptyQueryParametersExpectation(), null));
            callRawExpectation.Returns(TaskForResult<string>("robot #42's part #69 json"));

            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<Part>("part", "robot #42's part #69 json"));
            var translatedPart = new Part { Id = "69" };
            translationExpectation.Returns(translatedPart);

            var answer = CalculonsParts.Get("69");
            answer.Wait();

            Assert.AreSame(translatedPart, answer.Result);
        }

        [Test]
        public void ShouldCreateSingleInstanceSubResourceForHasA()
        {
            var getRobotExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/420", EmptyQueryParametersExpectation(), null));
            getRobotExpectation.Returns(TaskForResult<string>("Robot #420's json"));

            // Robot #42 has Brain #56
            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<Robot>("robot", "Robot #420's json"));
            var translatedRobot = new Robot { Id = "420",
                Brain = new HasOneDeserializationPlaceholder<Brain>("56")
            };
            translationExpectation.Returns(translatedRobot);

            var answer = Robots.Get("420");
            answer.Wait();

            getRobotExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();

            Assert.IsInstanceOf<SingleInstanceSubResource<Brain>>(answer.Result.Brain);
        }

        [Test]
        public void ShouldSetHasOneIdOnOwnedModel()
        {
            // dev user stories for has_one:
            // -- fetching from a host model object you already have
            // -- setting a new instance on -- one that has been already saved
            //                              -- one that has not been saved (ie., has no ID);
            // -- setting a new

            var r = new Robot() { Id = "67" };
            r.Brain = new SingleInstanceSubResource<Brain>(Shopify, new Brain() { Id = "89" });
        }
    }
}
