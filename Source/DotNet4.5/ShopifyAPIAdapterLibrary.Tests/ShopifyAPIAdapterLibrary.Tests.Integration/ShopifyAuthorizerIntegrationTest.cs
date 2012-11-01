using System;
using System.Configuration;
using NUnit.Framework;
using System.Threading.Tasks;
using NHttp;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace ShopifyAPIAdapterLibrary.Tests
{
	/// <summary>
	/// Test the interaction with the actual, running service at api.shopify.com.
	/// </summary>
	[TestFixture]
	public class ShopifyAuthorizerIntegrationTest
	{
		ShopifyAuthorizationState AuthorizationState {
			get;
			set;
		}

		String TestStoreName;

		ShopifyAPIClient ShopifyClient;

		public ShopifyAuthorizerIntegrationTest ()
		{
			TestStoreName = ConfigurationManager.AppSettings["Shopify.TestStoreName"];
		}

		public Task<string> ListenForIncomingShopTokenFromRedirect (int port)
		{
			var tcs = new TaskCompletionSource<string>();
			var server = new HttpServer();

			server.EndPoint.Port = port;
			server.EndPoint.Address = IPAddress.Any;
			server.RequestReceived += (s, e) =>
			{
				using (var writer = new StreamWriter(e.Response.OutputStream))
				{
					writer.Write("Nom, delicious shop access code!");
				}

				// when we get our first request, have the TCS become ready
				tcs.SetResult(e.Request.Params["code"]);
				// server.Dispose();
			};

			server.Start();

			return tcs.Task;
		}

		[TestFixtureSetUp]
		public void BeforeFixture ()
		{
			// because it's so expensive on requests, get our authorization key once for the entire integration test suite
			// TODO: check to be sure the given shop doesn't already have us added?
			// TODO: move sekrits/configs into App.config

			// this Task will become ready once Shopify gets back to us with the test shop's consent
			var redirectReplyPromise = ListenForIncomingShopTokenFromRedirect (5409);

			Console.WriteLine ("Attempting to authorize against store " + TestStoreName);
			var sa = new ShopifyAPIAdapterLibrary.ShopifyAPIAuthorizer (TestStoreName, ConfigurationManager.AppSettings["Shopify.TestAppKey"], ConfigurationManager.AppSettings["Shopify.TestAppSecret"]);
			var authUrl = sa.GetAuthorizationURL (new string[] { "write_content,write_themes,write_products" }, ConfigurationManager.AppSettings["Shopify.TestHttpServerUri"]);
			Console.WriteLine (authUrl);


			// pop a web browser with the authorization:
			Process.Start (authUrl);
			Console.WriteLine ("Waiting for Shopify to answer...");
			redirectReplyPromise.Wait ();
			var shopCode = redirectReplyPromise.Result;
			Assert.NotNull (shopCode);
			Console.WriteLine ("Got code: " + shopCode);

			// acquire our authorization token for actual API requests
			AuthorizationState = sa.AuthorizeClient (shopCode);

			ShopifyClient = new ShopifyAPIClient (AuthorizationState, new JsonDataTranslator());
		}

		[Test]
		public void ShouldAuthorizeAgainstANewShop() {
			Assert.AreEqual (TestStoreName, AuthorizationState.ShopName);
			Assert.NotNull (AuthorizationState.AccessToken);
		}

		[Test]
		public void ShouldFetchAllProducts ()
		{
			dynamic products = ShopifyClient.Get ("/admin/products.json");
			// validate that we're actually getting a list back, even though we can't check
			// deeper because we don't strictly know the content of the test store
			Assert.AreEqual(typeof(Newtonsoft.Json.Linq.JArray), products.products.GetType());
			// informative for the log:
			foreach (var product in products.products) {
				Console.WriteLine ("GOT PRODUCT: " + product.title);
			}
		}

		[Test]
		public void ShouldCreateAndReplaceAProduct () {
			dynamic productResponse = ShopifyClient.Post("/admin/products.json", new {
				product = new {
					title = "Rearden Metal",
					body_html = "Resistant to corrosion and evasion of reality"
				}
			});
		}

		[Test]
		public void ShouldFetchAllArticles() {

		}

		[Test]
		public void ShouldFetchAllBlogs() {

		}
	}
}
