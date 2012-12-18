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
    public class BlogsController : Controller
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
            // get all shopify blogs
            var blogs = await _shopify.GetResource<Blog>().AsList();
            return View(blogs);
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
                var newBlog = new Blog()
                {
                    Title = collection["Title"]
                };

                await _shopify.GetResource<Blog>().Save<Blog>(newBlog);

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
            var blog = await _shopify.GetResource<Blog>().Find(id);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Blog data)
        {
                await _shopify.GetResource<Blog>().Update<Blog>(data);

                return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            var blog = await _shopify.GetResource<Blog>().Find(id);
            
            
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Blog condemned)
        {
            await _shopify.GetResource<Blog>().Delete<Blog>(condemned);

            return RedirectToAction("Index");
        }
    }
}
