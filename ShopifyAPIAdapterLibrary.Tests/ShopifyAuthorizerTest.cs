using System;
using NUnit.Framework;

namespace ShopifyAPIAdapterLibrary.Tests
{
	[TestFixture()]
	public class ShopifyAuthorizerTest
	{
		[SetUp]
		public void BeforeEach ()
		{

		}

		[Test()]
		public void ShouldCreateAuthorizationURL ()
		{
			var sa = new ShopifyAPIAdapterLibrary.ShopifyAPIAuthorizer("atkinson-cafe", "apikey", "sekritpassword");
			Console.WriteLine (sa.GetAuthorizationURL(new string[] { "dsafsaf" }, "http://www.google.com"));
			Assert.AreEqual("https://atkinson-cafe.myshopify.com/admin/oauth/authorize?client_id=apikey&scope=dsafsaf&redirect_uri=http%3a%2f%2fwww.google.com", sa.GetAuthorizationURL(new string[] { "dsafsaf" }, "http://www.google.com"));
		}
	}
}

