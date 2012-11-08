using FakeItEasy;
using NUnit.Framework;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests
{
    public class Robot : IResourceModel {
        public string Id { get; set; }

        public string RobotType { get; set; }
        public string Producer { get; set; }
        public ICollection<Part> Parts { get; set; }
    }

    public class Part : IResourceModel
    {
        public string Id { get; set; }
    }

    [TestFixture]
    public class RestResourceTest
    {
        public RestResourceTest ()
        {

        }

        public IShopifyAPIClient Shopify { get; set; }

        public RestResource<Robot> Robots { get; set; }

        /// <summary>
        /// SubResources would normally be created dynamically
        /// and set as "proxies" as the collections backing subresource
        /// lists on a given model object.  We make one manually here.
        /// </summary>
        public SubResource<Part> CalculonsParts { get; set; }

        public Robot Calculon { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Shopify = A.Fake<IShopifyAPIClient>();
            A.CallTo(() => Shopify.AdminPath()).Returns("/admin");
            A.CallTo(() => Shopify.GetRequestContentType()).Returns(new MediaTypeHeaderValue("application/json"));

            Robots = new RestResource<Robot>(Shopify, "robot");
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

            var rrNexusByTyrell = rrNexus.Where("producer", "tyrell");
            var rrNexusByTyrellParameters = rrNexusByTyrell.FullParameters();
            Assert.AreEqual("nexus", rrNexusByTyrellParameters["robot_type"]);
            Assert.AreEqual("tyrell", rrNexusByTyrellParameters["producer"]);
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
                A<MediaTypeHeaderValue>.That.Matches(mt => mt.ToString() == "application/json"),
                "/admin/robots", null, null));
            callRawExpectation.Returns(TaskForResult<string>("json text!"));

            var translationExpectation = A.CallTo(() => Shopify.TranslateObject<List<Robot>>("robots", "json text!"));
            translationExpectation.Returns(new List<Robot>() { new Robot() { Id = "fdaf" } } );

            var answer = Robots.AsList();

            answer.Wait();

            callRawExpectation.MustHaveHappened();
            translationExpectation.MustHaveHappened();

            Assert.AreEqual(1, answer.Result.Count);
        }

        [Test]
        public void ShouldBuildSubResourcePaths()
        {
            Assert.AreEqual("/admin/robots/42/parts", CalculonsParts.Path());
            Assert.AreEqual("/admin/robots/42/parts/67", CalculonsParts.InstancePath("67"));
        }

        [Test]
        public void ShouldFetchARecord()
        {

            // Robots.Get("89");
        }
    }
}
