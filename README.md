# Sharpify

Type-safe, asynchronous, object-oriented .NET adapter for the Shopify
API, derived from Colin McDonald's original lightweight dynamic
adapter.

## Author

Copyright (c) 2012 Andrew Clunis <[andrew@orospakr.ca](andrew@orospakr.ca)>

Copyright (c) 2012 Colin McDonald [colinmcdonald.ca](http://colinmcdonald.ca)

Licensed under the MIT license.

## Requirements

* Mono 3.0 or .NET 4.5 or greater
  * ASP.net MVC3 is required to build and run the Sample Web app (I
    haven't tried this with Mono yet)

## Installation

I do not yet offer a NuGet package.  For now, the easiest and only
method is download the source code and add the project to your
solution.  I recommend using a git submodule, with something like
this:

    git submodule add git://github.com/orospakr/sharpify dependencies/Sharpify

MonoDevelop and Visual Studio 2012 happily consume the sln and csproj
without problems.

For now, I added all of the dependencies as binaries for the purpose
of convenience into the repository because NuGet isn't usable on Mono,
so no additional step for installing the secondary deps is necessary.

If, on Mono, you get "Web Security" crypto exceptions, try importing
Mozilla's trusted CA list into Mono's CA list by running:

    mozroots --import

(alternatively, you could try manually importing the certificate of
the CA shopify uses by using Mono's `certmgr` tool.)

NB. For all that, a
[bug](https://bugzilla.xamarin.com/show_bug.cgi?id=8829) in Mono 3.0
currently breaks Sharpify, preventing use on Linux and Mac OS X.  A
fix has already been committed to upstream Mono's master, so it should
presumably start working in the next release.  Sorry about that.

## Design Principles

This adapter is type-safe, with the intention of being locally
discoverable with the help of your IDE's code completion feature, and
that your usage is verifiable at compile time.  Regardless, it is
recommended that you review the
[Shopify API documentation](http://api.shopify.com/).

After obtaining your authorization token, you can create a
`ShopifyApiContext` from which in turn you can get access to the
RestResource objects, which can fetch and push individual model
objects (an Order, Customer, and so on).

However, unlike ActiveRecord and ActiveResource, the model instances
themselves do not offer any identity guarantee.  That is, when you ask
a RestResource object to save one of these or fetch the same ID, it
always will pass you a new instance.  The only state in the resource
models themselves aside from the actual data is tracking of field
dirtiness.

In the Rails tradition of modelling associations, "has one" and "has
many"-style relationships are implemented.  However, here they are
implemented with explicit containers (identifiable by IHasOne<T> and
IHasMany<T>) instead of implicit proxies.

## Usage

### Shopify API Authorization

In order to understand how shopify authorizes your code to make API
calls for a certain shopify customer, I recommend reading this
document:
[Shopify API Authentication](http://api.shopify.com/authentication.html).

Instantiate ShopifyAPIAuthorizer with your customer's store you wish
to authorize against (generally the subdomain from
https://:store_name.myshopify.com").  You'll need your app's API key
and API shared secret, as provided by your account on Shopify Partners
interface.

```csharp
var sa = new ShopifyAPIAuthorizer ("TARGET STORE NAME",
                                   "APP API KEY",
                                   "APP SHARED SECRET");
```

Then, ask the `Authorizer` to give you an authorization URL to send
your app's user to.  Specifying the REST interface portions you want
permission for
([the full list of permissions](http://api.shopify.com/authentication.html#scopes)).

You also need to pass it a redirect URI to which the temporary
authorization code will be returned to (as a query parameter,
`?code=`).  Note that the Shopify authorization service redirects the
app user's browser to this URI.  This URI needs to be nested within
the URI you provided when creating the app in the partners interface.
The Shopify service itself never attempts to fetch this URL.

If your app already has authorization for this store with the same
permissions list, the service will immediately redirect the user's
browser to your redirect URI.

```
var authUrl = sa.GetAuthorizationURL (new string[] { "write_content",
                                                     "write_themes",
                                                     "write_products",
                                                     "write_customers",
                                                     "write_script_tags",
                                                     "write_orders" },
                                      "http://myexcellentshopifyapp.com/receive_auth");
```
                                          
Sharpify can't really provide you with any means of receiving the
code.  You're own your own.
                                          
If you're developing a webapp that consumes the Shopify API, it's
likely a simple matter of adding an endpoint to your app that will
check the user's browser session and grab the auth code.

If you're developing a desktop app, it's a bit trickier.  One approach
is to find some way of instrumenting the webview you've embedded in
your app with a signal handler for noticing navigation.  With this
method, no HTTP server running at the other end of the URI is
necessary.  Alternatively, you could run a local HTTP server with the
sole purpose of grabbing the code, which is the method the live
integration test suite for Sharpify uses.

Then, there's one final task: take that temporary authorization code
and request the service to provide you with a permanent access token
for your app to that shop.

```csharp
var authState = await sa.AuthorizeClient(receivedCode);
```
    
That's just a flat POCO with the shop name and the access token.  You
can save these at your leisure for future use of the API for that
shop.

### Create an API Context

Create an instance of `ShopifyAPIContext` for accessing a specific
shop (you can create the `ShopifyAuthorizationState` on your own with
your saved access token for that shop).

```csharp
var shopify = new ShopifyAPIContext(authState, new JsonDataTranslator());
```
    
(for now, always give it the new `JsonDataTranslator`)

RestResource objects, as provided by
`ShopifyAPIContext#GetResource<T>()` where T is the type of the
resource model you want to interact with.

For brevity in the following examples, let:

```csharp
RestResource<Product> Products = shopify.GetResource<Product>();
RestResource<Customer> Customers = shopify.GetResource<Customer>();
RestResource<Order> Orders = shopify.GetResource<Order>();
```

### Fetch a Model instance

```csharp
Product engine = await Products.Get(1083);
```

### Update a Model instance

```csharp
// update a field
engine.Title = "Trent 1000";
    
var updatedEngine = await Products.Save<Product>(engine);
```

NB. It's necessary for you to pass the resource type again into the
`Save()`, and `Create()` methods in order for Sharpify to ensure that
only saveable resources can be passed to these methods.  The compiler
won't let you get it wrong.

If the Shopify API adds anything to your model on save, it'll show up
in the returned object.  The original object will not be mutated.

### Create a Model instance

```csharp
var airplane = new Product() { Name = "Boeing 787-8" };
var savedAirplane = await Products.Save<Product>(airplane);
```

The same guarantees regarding object immutability apply here.

(The day I can buy the above product on Shopify my life will be
complete)

### Fetch all Models in a Resource

This takes care of all the pagination for you.  Asynchronously iterate
over all of the models:

```csharp
await Products.Each((product) => {
    Console.WriteLine(product.Title);
});
```

Or buffer them all up as a List:

```csharp
var products = await Products.AsList();
```

### Filter by Criteria

Discrimate the resource by adding some equality criteria by field
(by means of the Shopify API's acceptance of per-field query
parameters):

```csharp
IRestResourceView<Product> cultProducts = Products.Where(
    (p) => p.ProductType, "Cult Products");
```

You can also specify the property name with a string (as it would be
underscorized for the API) rather than as the MemberExpression.

The only permissible test permitted by the the REST API is equality,
hence why `Where()` only accepts a field and a value to compare
against.

### Linked Models ("has one")

#### Get a Has One

Get the resource model instance from the server pointed to by a
`IHasOne<T>` on another model.

```csharp
// Get the customer
var customer = Customers.Find(99);

// For illustration, let a local variable equal the IHasOne<T> itself
IHasOne<Order> lastOrder = customer.LastOrder;

// Retrieve the customer's last order
Order gotOrder = await lastOrder.Get();
```

#### Set a Has One

This API is slightly convoluted, but it allows for Sharpify to handle
setting up what kind of has one (inlined or not, an implementation
detail) a given `IHasOne<T>` property on a model should contain.

```csharp
Customers.Has<Order>(customer, (c) => c.LastOrder);

Customers.Save<Customer>(customer);
```

### Nested Model Lists ("has many", "subresources")

```csharp
// Get the order
var order = Orders.Get(66);

// For illustration, let a local variable equal the IHasMany<T> itself
IHasMany<Fulfillment> fulfillments = order.Fulfillments;

// fulfillments is basically a full rest resource (which can be
// filtered, added to (with `Save()` or `Create()`), and so on).

// get a list of fulfillments
var fulfillmentsList = fulfillments.AsList();
```

### "Fragments"

There's a few inlined complex types on a few of the resources, but
they're not full REST resources.  Just treat them as the simply
serialized POCO objects, as the types will indicate.

### Events

Shopify tracks events of note on a number of important resources.
They're IHasMany lists.  Look for them on `Order` and `Product`.

### Metafields

Shopify allows you to add arbitrarily typed name/value pairs to
various resources.  They too are treated as a full IHasMany
subresource.

## Late-bindable Dynamic API

Alternatively, the original late-bound API from Colin McDonald's
Shopify.net is still here, albeit now async.  Use this, and you will
not be hidden from the URLs of the API or the ways in which the API
will require the data to be passed.  Note that this approach does not
handle pagination or any other higher-level details.

### Get a Model with the Dynamic API

Get all Products (on a single page) from the API as a string.

```csharp
ShopifyAPIClient api = new ShopifyAPIClient(authState);

// by default JSON string is returned
object data = await api.Get("/admin/products.json");

// use your favorite JSON library to decode the string into a C# object
```

Get all Products (on the first page) from the API as a JObject from
json.net, suitable for use with `dynamic`:

```csharp
// pass the supplied JSON Data Translator
var api = new ShopifyAPIClient(authState, new JsonDataTranslator());

// The JSON Data Translator will automatically decode the JSON for you
dynamic data = await api.Get("/admin/products.json");

// the dynamic object will have all the fields just like in the API Docs
foreach(var product in data.products)
{
    Console.Write(product.title);
}
```

Create a Product, using raw JSON in a string:

```csharp
ShopifyAPIClient api = new ShopifyAPIClient(authState);

// Manually construct a JSON string or in some other way
// Ugly
string dataToUpdate =
"{" +
    "\"product\": {" +
        "\"title\": \"Burton Custom Freestlye 151\"," +
        "\"body_html\": \"<strong>Good snowboard!</strong>\"" +
    "}" +
"}";

string createProductResponse = await api.Post("/admin/products.json");
```

Create a product, using an anonymous class:

```csharp
// pass the supplied JSON Data Translator
var api = new ShopifyAPIClient(authState, new JsonDataTranslator());

// use dynamics to create the object
// a lot nicer that the previous way
dynamic newProduct = new
{
    products = new {
        title		= "Burton Custom Freestlye 151",
        body_html	= "<strong>Good snowboard!</strong>"
    }
};
dynamic createProductResponse = await api.Post("/admin/products.json", newProduct);
```

Delete a Product:

```csharp
// id of the product you wish to delete
int id = 123;
var api = new ShopifyAPIClient(authState, new JsonDataTranslator());
await api.Delete(String.Format("/admin/products/{0}.json", id));
```

## Sample Web Application

This sample application should give you an excellent idea how you will
need to perform the required oAuth authentication and API calls.

Once you go through the authorization steps, you can Add/Edit/Delete
Shopify Blog objects.

## Test Suite

Included is a test suite, include both unit and integration tests.
You'll need a NUnit harness to run them.

To run the *live* integration tests (ie., test the Adapter against the
actual upstream service:

(note that the live integration tests can't test with great coverage
because they can't guarantee what fixture data is in your test store,
and what data is there will likely differ from mine and produce
different results.  Still, this harness is convenient to have in
development, if nothing else)

1. Sign up for a [Shopify Partner account](http://www.shopify.com/partners);

2. Via the Partner UI, create a Test store;

3. Via the Partner UI, create a new Application for your local test
   suite instance.  Make up a fake domain for "Application URL", as
   nothing on their end needs to talk to it;

4. Modify the `ShopifyAPIAdapterLibrary.Tests/App.config` file to
   reflect the app and shop you just created, along with your API keys
   and so on;

5. Modify your local `/etc/hosts` to add the fake domain you made for
   the Application URL as an alias for localhost.  This is needed
   because a local web browser will be redirected to it in order to
   deliver the received authentication token;

6. From the Shopify Partners interface, select the "Login" button on
   the list item for your test store.  If you try to skip this step
   don't have an active session on the test store, you will be
   prompted for a username and password in the next step.  As that
   prompt expects a full Shopify account (and cannot be used for
   Partners UI-created stores), nothing you type will succeed.

7. Run all the tests using your NUnit harness (either in MD or Visual
   Studio .net 2012, there's a
   [VSIX NUnit plugin for it](http://nunit.org/index.php?p=vsTestAdapter&r=2.6.2)).

8. A local web browser will be opened in order to obtain OAuth
   authorization.  The first time you run the test suite, Shopify will
   prompt you to add the Test App you created to your test store.

Testing protip: if a unit test (ie., not an integration) test hangs, the problem may be
that a null Task being returned by a mock receiving an unexpected call.  Temporarily add
some sane length of delay to the blocking Wait() statements used in the test, and see
if you get an error from the mock that displays a list of unexpected calls.

## Happy Trading!
