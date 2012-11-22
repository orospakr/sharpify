using System;
using System.Configuration;
using NUnit.Framework;
using System.Threading.Tasks;
using NHttp;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections;
using ShopifyAPIAdapterLibrary.Models;
using System.Collections.Specialized;

namespace ShopifyAPIAdapterLibrary.Tests.Integration
{
    /// <summary>
    /// Test the interaction with the actual, running service at :shop.myshopify.com.
    /// </summary>
    [TestFixture]
    [Ignore]
    public class LiveAPIServiceIntegrationTest
    {
        ShopifyAuthorizationState AuthorizationState {
            get;
            set;
        }

        HttpServer Server {
            get;
            set;
        }

        String TestStoreName;
        ShopifyAPIClient ShopifyClient;

        public LiveAPIServiceIntegrationTest ()
        {
            TestStoreName = ConfigurationManager.AppSettings ["Shopify.TestStoreName"];
        }

        public Task<string> ListenForIncomingShopTokenFromRedirect (int port)
        {
            var tcs = new TaskCompletionSource<string> ();
            Server = new HttpServer ();

            Server.EndPoint.Port = port;
            Server.EndPoint.Address = IPAddress.Any;
            Server.RequestReceived += (s, e) =>
            {
                using (var writer = new StreamWriter(e.Response.OutputStream)) {
                    writer.Write ("Nom, delicious shop access code!  Test suite will now continue.");
                }

                // when we get our first request, have the TCS become ready
                tcs.SetResult (e.Request.Params ["code"]);
                // server.Dispose();
            };

            Server.Start ();

            return tcs.Task;
        }

        [TestFixtureSetUp]
        public void BeforeFixture ()
        {
            try {
                // because it's so expensive on requests, get our authorization key once for the entire integration test suite

                // this Task will become ready once Shopify redirects our browser back to us with the test shop's consent (in the form of the access token)
                var redirectReplyPromise = ListenForIncomingShopTokenFromRedirect (5409);

                Console.WriteLine ("Attempting to authorize against store " + TestStoreName);
                var sa = new ShopifyAPIAdapterLibrary.ShopifyAPIAuthorizer (TestStoreName, ConfigurationManager.AppSettings ["Shopify.TestAppKey"], ConfigurationManager.AppSettings ["Shopify.TestAppSecret"]);
                var authUrl = sa.GetAuthorizationURL (new string[] { "write_content,write_themes,write_products,write_customers,write_script_tags,write_orders" }, ConfigurationManager.AppSettings ["Shopify.TestHttpServerUri"]);
                Console.WriteLine (authUrl);


                // pop a web browser with the authorization:
                Process.Start (authUrl);
                Console.WriteLine ("Waiting for Shopify to answer...");
                redirectReplyPromise.Wait ();
                var shopCode = redirectReplyPromise.Result;
                Assert.NotNull (shopCode);
                Console.WriteLine ("Got code: " + shopCode);

                var authTask = sa.AuthorizeClient (shopCode);
                authTask.Wait ();

                // acquire our authorization token for actual API requests
                AuthorizationState = authTask.Result;

                ShopifyClient = new ShopifyAPIClient (AuthorizationState, new JsonDataTranslator ());
            } catch (Exception e) {
                using (var fd = new StreamWriter("sharpify_test_before_fixture_error.txt")) {
                    fd.Write (e.ToString ());
                }
                throw new Exception ("Rethrowing exception emitted during BeforeFixture()", e);
            }
        }

        [TestFixtureTearDown]
        public void AfterFixture ()
        {
            Server.Dispose ();
        }

        [Test]
        public void ShouldAuthorizeAgainstANewShop ()
        {
            Assert.AreEqual (TestStoreName, AuthorizationState.ShopName);
            Assert.NotNull (AuthorizationState.AccessToken);
        }

        [Test]
        public void ShouldFetchAllProducts ()
        {
            var productsTask = ShopifyClient.Get ("/admin/products");
            productsTask.Wait();
            dynamic products = productsTask.Result;
            // validate that we're actually getting a list back, even though we can't check
            // deeper because we don't strictly know the content of the test store
            Assert.AreEqual (typeof(Newtonsoft.Json.Linq.JArray), products.products.GetType ());
            Assert.Greater(products.products.Count, 2);
            // informative for the log:

            foreach (var product in products.products) {
                Console.WriteLine ("GOT PRODUCT: " + product.title);
            }
        }

        [Test]
        public void ShouldFetchAllArticles() {
            var articlesTask = ShopifyClient.Get("/admin/articles.json");
            articlesTask.Wait();
        }

        [Test]
        public void ShouldFetchAllProductsTypesafe ()
        {
            var productsTask = ShopifyClient.GetProducts();
            productsTask.Wait();

            // HACK: making silly assumptions about the content of the test store, and because we have no fixtures we can't check contents
            Assert.Greater(productsTask.Result.Count, 4);
        }

        [Test]
        public void ShouldThrowErrorWhenFetchingNonexistentResource ()
        {
            var getTask = ShopifyClient.Get ("/admin/products/doesnotexist");
            var notFound = false;
            try {
                getTask.Wait ();
            } catch (AggregateException ae) {
                ae.Handle((e) => {
                    if(e is NotFoundException) {
                        notFound = true;
                        return true;
                    }
                    return false;
                });
            }
            Assert.IsTrue(notFound);
        }

        [Test]
        public void ShouldCreateAndFetchBackAProduct ()
        {
            var postTask = ShopifyClient.Post ("/admin/products", new {
                product = new {
                    title = "Rearden Metal",
                    body_html = "Resistant to corrosion and evasion of reality",
                    product_type = "metal"
                }
            });
            postTask.Wait();

            dynamic postResult = postTask.Result;

            String newId = postResult.product.id;

            Assert.NotNull (newId);

            // and fetch it back
            var getTask = ShopifyClient.Get (String.Format("/admin/products/{0}", newId));

            getTask.Wait ();
            Assert.NotNull(getTask.Result);
            dynamic getResult = getTask.Result;
            Assert.AreEqual("Rearden Metal", (string)getResult.product.title);

            // and with the typesafe api:
            var getTypeTask = ShopifyClient.GetResource<Product>().Get(Int32.Parse(newId));
            getTypeTask.Wait();
            Assert.AreEqual("Rearden Metal", getTypeTask.Result.Title);
        }

        [Test]
        public void ShouldThrowErrorWhenPostingInvalidResource ()
        {
            var postTask = ShopifyClient.Post ("/admin/products", new {
                product = new {
                    title = "Invalid"
                }
            });
            var gotError = false;
            try {
                postTask.Wait ();
            } catch (AggregateException ae) {
                ae.Handle ((e) => {
                    if(e is InvalidContentException) {
                        gotError = true;
                        return true;
                    }
                    return false;
                });
            }
            Assert.IsTrue(gotError);
        }

        [Test]
        public void ShopifyShouldReturnInlinedSubResourceAsSeparate()
        {
            // try to get either /orders/:id/line_items
            // or /orders/:id/fulfillments

            // or /orders/:id/fulfillments/:id/line_items

            // this is to confirm if inlines are guaranteed fetchable as subresources intead.
            // NO THEY ARE NOT

            //var getTask = ShopifyClient.Get("/admin/orders/147593684/line_items.json", null);
            var query = new NameValueCollection();
            query.Add("page", "2");
            var getTask = ShopifyClient.Get("/admin/products", query);
            getTask.Wait();

            Console.WriteLine("done");
        }

        [Test]
        public void ShouldHandleBeingAskedForPageByWhere() {
            var query = new NameValueCollection();
            query.Add("page", "2");

            var getTask = ShopifyClient.GetResource<Product>().Where("page", "2").AsList();
            getTask.Wait();

            Assert.NotNull(getTask.Result);
        }

        [Test]
        public void ShouldFetchAllOrders() {
            // TODO oh shit paging
            var answer = ShopifyClient.GetResource<Order>().AsList();
            answer.Wait();
            Console.WriteLine("dsafdasf");
        }

        [Test]
        public void ShouldFetchAllTopLevelResources()
        {
            // this test is obviously pretty naiive, and considering that
            // we can make few guarantees about the state of the store, may
            // succeed or fail quite different depending on conditions.

            // all it will really do it flush out some obvious crashes lurking
            // in fetching and translating the toplevel resources.

            // still, a handy thing to have to validate the behaviour of Sharpify
            // against whatever your current store contents are, and thus
            // worth keeping around in this harness even if it horribly breaks
            // testing methodology.

            var productsCount = ShopifyClient.GetResource<Product>().Count();
            productsCount.Wait();

            ShopifyClient.GetResource<Asset>().AsList().Wait();
            ShopifyClient.GetResource<ApplicationCharge>().AsList().Wait();
            ShopifyClient.GetResource<Article>().AsList().Wait();
            ShopifyClient.GetResource<Blog>().AsList().Wait();
            ShopifyClient.GetResource<Checkout>().AsList().Wait();
            ShopifyClient.GetResource<Collect>().AsList().Wait();
            ShopifyClient.GetResource<CustomCollection>().AsList().Wait();
            ShopifyClient.GetResource<Comment>().AsList().Wait();
            ShopifyClient.GetResource<Country>().AsList().Wait();
            ShopifyClient.GetResource<Customer>().AsList().Wait();
            ShopifyClient.GetResource<CustomerGroup>().AsList().Wait();
            ShopifyClient.GetResource<Event>().AsList().Wait();
            ShopifyClient.GetResource<Order>().AsList().Wait();
            ShopifyClient.GetResource<Page>().AsList().Wait();
            ShopifyClient.GetResource<Product>().AsList().Wait();
            ShopifyClient.GetResource<ProductSearchEngine>().AsList().Wait();
            ShopifyClient.GetResource<RecurringApplicationCharge>().AsList().Wait();
            ShopifyClient.GetResource<Redirect>().AsList().Wait();
            ShopifyClient.GetResource<ScriptTag>().AsList().Wait();
            ShopifyClient.GetResource<SmartCollection>().AsList().Wait();
            ShopifyClient.GetResource<Theme>().AsList().Wait();
            ShopifyClient.GetResource<Webhook>().AsList().Wait();
        }
    }
}
