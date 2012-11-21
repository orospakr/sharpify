using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary
{
    public class PaginatedEnumerator<T> where T : IResourceModel, new()
    {
        private IResourceView<T> Resource { get; set; }

        public PaginatedEnumerator(IResourceView<T> resource)
        {
            Resource = resource;
        }

        public async Task<IList<T>> AsList() {
            var list = new List<T>();
            await Each((model) => {
                list.Add(model);
            });
            return list;
        }

        public async Task Each(Action<T> cb) {
            var p = 1;
            IList<T> batch;
            do
            {
                var currentPageView = Resource.Page(p);
                batch = await currentPageView.AsListUnpaginated();
                foreach (T model in batch)
                {
                    cb(model);
                }
                p++;
            } while (batch.Count() > 0);
        }
    }
}
