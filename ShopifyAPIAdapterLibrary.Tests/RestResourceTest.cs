using FakeItEasy;
using FakeItEasy.Configuration;
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
    public class Robot : ShopifyResourceModel {

        private string _RobotType;
        public string RobotType
        {
            get { return _RobotType; }
            set {
                SetProperty(ref _RobotType, value);
            }
        }

        private string _Manufacturer;
        public string Manufacturer
        {
            get { return _Manufacturer; }
            set {
                SetProperty(ref _Manufacturer, value);
            }
        }


        private IHasMany<Part> _Parts;
        public IHasMany<Part> Parts
        {
            get { return _Parts; }
            set {
                SetProperty(ref _Parts, value);
            }
        }


        private IList<Inspection> _Inspections;
        public IList<Inspection> Inspections
        {
            get { return _Inspections; }
            set {
                SetProperty(ref _Inspections, value);
            }
        }


        private IHasOne<Brain> _Brain;
        public IHasOne<Brain> Brain
        {
            get { return _Brain; }
            set {
                SetProperty(ref _Brain, value);
            }
        }

        private IHasOne<Laser> _Laser;
        [Inlinable]
        public IHasOne<Laser> Laser
        {
            get { return _Laser; }
            set
            {
                SetProperty(ref _Laser, value);
            }
        }


        private SpecialAction _Explode;
        public SpecialAction Explode
        {
            get { return _Explode; }
            set {
                SetProperty(ref _Explode, value);
            }
        }

    }

    public class DeepNestedHasAInline : ShopifyResourceModel
    {
    }

    public class Brain : ShopifyResourceModel
    {
        private int _SynapseCount;
        public int SynapseCount
        {
            get { return _SynapseCount; }
            set {
                SetProperty(ref _SynapseCount, value);
            }
        }

        private IHasOne<DeepNestedHasAInline> _DeepNested;
        public IHasOne<DeepNestedHasAInline> DeepNested
        {
            get { return _DeepNested; }
            set
            {
                SetProperty(ref _DeepNested, value);
            }
        }

    }

    public class Laser : ShopifyResourceModel
    {
    }

    // our test subresource
    public class Part : ShopifyResourceModel
    {

        private string _Sku;
        public string Sku
        {
            get { return _Sku; }
            set {
                SetProperty(ref _Sku, value);
            }
        }

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

            Robots = new RestResource<Robot>(Shopify);
            Brains = new RestResource<Brain>(Shopify);
            Calculon = new Robot() { Id = 42 };
            CalculonsParts = new SubResource<Part>(Robots, Calculon);
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

        public static Task<T> TaskForResult<T>(T input)
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
            translationExpectation.Returns(new List<Robot>() { new Robot() { Id = 8889 } } );

            var answer = Robots.AsListUnpaginated();

            answer.Wait();

            callRawExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();

            Assert.AreEqual(1, answer.Result.Count);
            Assert.NotNull(answer.Result[0].Parts);
            Assert.AreEqual("/admin/robots/8889/parts", answer.Result[0].Parts.Path());
        }

        [Test]
        public void ShouldBuildSubResourcePaths()
        {
            Assert.AreEqual("/admin/robots/42/parts", CalculonsParts.Path());
            Assert.AreEqual("/admin/robots/42/parts/67", CalculonsParts.InstancePath(67));
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

        private NameValueCollection PageNumberQueryParameterExpectation(int pageNumber)
        {
            return A<NameValueCollection>.That.Matches(nvc => nvc.Get("page") == pageNumber.ToString());
        }

        [Test]
        public void ShouldFetchARecord()
        {
            var callRawExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/89", EmptyQueryParametersExpectation(), null));
            callRawExpectation.Returns(TaskForResult<string>("robot #89's json"));

            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<Robot>("robot", "robot #89's json"));
            var translatedRobot = new Robot { Id = 89 };

            //
            // TODO: .Get will start setting

            translationExpectation.Returns(translatedRobot);
            var answer = Robots.Get(89);
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
            postRawExpectation.Returns(TaskForResult<string>("PART 988 REAL JSON"));

            var resultTranslationExpectation = A.CallTo(() => Shopify.TranslateObject<Part>("part", "PART 988 REAL JSON"));
            // it got the id of 90 on the server
            var resultantPart = new Part() { Id = 90 };
            resultTranslationExpectation.Returns(resultantPart);

            var answer = CalculonsParts.Create(partToPost);

            answer.Wait();

            translationExpectation.MustHaveHappened();
            postRawExpectation.MustHaveHappened();
            resultTranslationExpectation.MustHaveHappened();

            Assert.AreSame(resultantPart, answer.Result);
        }

        [Test]
        public void ShouldUpdateASubResourceRecord()
        {
            var partToPost = new Part() { Id = 9777 };
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
            var translatedPart = new Part { Id = 69 };
            translationExpectation.Returns(translatedPart);

            var answer = CalculonsParts.Get(69);
            answer.Wait();

            Assert.AreSame(translatedPart, answer.Result);
        }

        [Test]
        public void ShouldReplaceInstanceSubResourceForHasOnePlaceholder()
        {
            var getRobotExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/420", EmptyQueryParametersExpectation(), null));
            getRobotExpectation.Returns(TaskForResult<string>("Robot #420's json"));

            // Robot #42 has Brain #56
            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<Robot>("robot", "Robot #420's json"));
            var translatedRobot = new Robot { Id = 420,
                Brain = new HasOneDeserializationPlaceholder<Brain>(56)
            };
            translationExpectation.Returns(translatedRobot);

            var answer = Robots.Get(420);
            answer.Wait();

            getRobotExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();

            Assert.IsInstanceOf<SingleInstanceSubResource<Brain>>(answer.Result.Brain);
            Assert.AreEqual(56, answer.Result.Brain.Id);
        }

        [Test]
        public void ShouldReplaceInstanceSubResourceForHasOnePlaceholdersWithinAHasOneInline()
        {
            // if we receive a has one inline, we need to be sure that the post-processing also
            // happens for the resourcemodels deserialized inside a HasOneInline<>

            var getRobotExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/420", EmptyQueryParametersExpectation(), null));
            getRobotExpectation.Returns(TaskForResult<string>("Robot #420's json"));

            // Robot #42 has Brain #56
            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<Robot>("robot", "Robot #420's json"));
            var translatedBrain = new Brain { Id = 747, DeepNested = new HasOneDeserializationPlaceholder<DeepNestedHasAInline>(8010)};
            var translatedRobot = new Robot
            {
                Id = 420,
                Brain = new HasOneInline<Brain>(translatedBrain)
            };
            translationExpectation.Returns(translatedRobot);

            var answer = Robots.Get(420);
            answer.Wait(1000);

            getRobotExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();

            Assert.IsInstanceOf<HasOneInline<Brain>>(answer.Result.Brain);
            var hasOneBrain = (HasOneInline<Brain>)(answer.Result.Brain);
            var brain = hasOneBrain.Get();
            brain.Wait(1000);
            Assert.IsInstanceOf<SingleInstanceSubResource<DeepNestedHasAInline>>(brain.Result.DeepNested);
            Assert.AreEqual(8010, brain.Result.DeepNested.Id);
        }

        // inlined has ones are directly handled by JsonDataTranslator, and thus are not mediated by
        // by RestResource.

        [Test]
        public void ShouldSetHasOneIdOnOwnedModel()
        {
            // dev user stories for has_one:
            // -- fetching from a host model object you already have
            // -- setting a new instance on -- one that has been already saved
            //                              -- one that has not been saved (ie., has no ID);
            // -- setting a new

            var r = new Robot() { Id = 67 };
            r.Brain = new SingleInstanceSubResource<Brain>(Shopify, new Brain() { Id = 89 });
        }

        [Test]
        public void ShouldFetchCount()
        {
            var getRobotCountExpectation = A.CallTo(() => Shopify.CallRaw(HttpMethod.Get,
                JsonFormatExpectation(),
                "/admin/robots/count", EmptyQueryParametersExpectation(), null));
            getRobotCountExpectation.Returns(TaskForResult<string>("robots count json"));

            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<int>("count", "robots count json"));
            translationExpectation.Returns(34969);
            var answer = Robots.Count();
            answer.Wait();

            Assert.AreEqual(34969, answer.Result);
        }

        [Test]
        public void ShouldCallActionsByProperty()
        {
            Robots.CallAction(Calculon, (robot) => robot.Explode);
             A.CallTo(() => Shopify.CallRaw(HttpMethod.Post,
                JsonFormatExpectation(),
                "/admin/robots/42/explode", EmptyQueryParametersExpectation(),
                null)).MustHaveHappened();
        }

        [Test]
        public void ShouldSetHasOneAsIdByDefault()
        {
            var brain = new Brain() { Id = 38 };
            Robots.Has<Brain>(Calculon, (calculon) => calculon.Brain, brain);
            Assert.IsInstanceOf<SingleInstanceSubResource<Brain>>(Calculon.Brain);
        }

        [Test]
        public void ShouldSetHasOneInlineByDefault()
        {
            var laser = new Laser() { Id = 32 };
            Robots.Has<Laser>(Calculon, (calculon) => calculon.Laser, laser);
            Assert.IsInstanceOf<HasOneInline<Laser>>(Calculon.Laser);
        }
    }
}
