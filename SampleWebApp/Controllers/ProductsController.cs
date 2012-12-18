using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleWebApp.Shopify;
using Sharpify;
using System.Threading.Tasks;
using Sharpify.Models;

namespace SampleWebApp.Controllers
{
    [ShopifyAuthorize]
    public class ProductsController : Controller
    {
        ShopifyAPIContext _shopify;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ShopifyAuthorizationState authState = ShopifyAuthorize.GetAuthorizationState(this.HttpContext);
            if (authState != null)
            {
                _shopify = new ShopifyAPIContext(authState, new JsonDataTranslator());
            }
        }

        public async Task<ActionResult> Index()
        {
            // get all shopify products
            ViewBag.ShopName = (await _shopify.GetShop()).Name;
            var products = await _shopify.GetResource<Product>().AsList();
            return View(products);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                var newProduct = new Product()
                {
                    Title = collection["Title"]
                };

                await _shopify.GetResource<Product>().Save<Product>(newProduct);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                // handle the exception
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var product = await _shopify.GetResource<Product>().Find(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product data)
        {
            await _shopify.GetResource<Product>().Update<Product>(data);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            var product = await _shopify.GetResource<Product>().Find(id);


            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Product condemned)
        {
            await _shopify.GetResource<Product>().Delete<Product>(condemned);

            return RedirectToAction("Index");
        }
    }
}
