using FakeItEasy;
using FakeItEasy.Configuration;
using NUnit.Framework;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests
{
    // This test is just kind of terrible.  Sorry. :(
    [TestFixture]
    class PaginatedEnumeratorTest
    {
        public class Customer : IResourceModel
        {
            public int? Id { get; set; }

            public Customer()
            {
            }
        }

        /// <summary>
        /// FakeItEasy mysteriously breaks with IResourceView<>.
        /// (http://stackoverflow.com/questions/7559354/faking-mocking-an-interface-gives-no-default-constructor-error-how-can-that-b)
        /// So I have to make own mocks the old fashioned way.
        /// </summary>
        public class ResourcePageMock : IResourceView<Customer>
        {
            private int PageNumber;
            private int ItemCount;

            public ResourcePageMock(int page, int itemCount)
            {
                PageNumber = page;
                ItemCount = itemCount;
            }

            public Task Each(Action<Customer> cb)
            {
                throw new NotImplementedException();
            }

            public async Task<IList<Customer>> AsListUnpaginated()
            {
                var pageContents = new List<Customer>();
                for (int c = 0; c < ItemCount; c++)
                {
                    var customer = new Customer() { Id = (ItemCount * PageNumber) + c };
                    pageContents.Add(customer);
                }
                return pageContents;
            }

            public IResourceView<Customer> Page(int p)
            {
                throw new NotImplementedException();
            }

            public IResourceView<Customer> Where(System.Linq.Expressions.Expression<Func<Customer, object>> propertyLambda, string isEqualTo)
            {
                throw new NotImplementedException();
            }

            public IResourceView<Customer> Where(string field, string isEqualTo)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Specialized.NameValueCollection FullParameters()
            {
                throw new NotImplementedException();
            }

            public Type GetModelType()
            {
                throw new NotImplementedException();
            }

            public Task<int> Count()
            {
                throw new NotImplementedException();
            }

            public string Path()
            {
                throw new NotImplementedException();
            }

            public string InstancePath(int p)
            {
                throw new NotImplementedException();
            }
        }

        public class PaginatedResourceMock : IResourceView<Customer>
        {
            private int PageToReturnEmptyAt;

            public PaginatedResourceMock(int pageToReturnEmptyAt)
            {
                PageToReturnEmptyAt = pageToReturnEmptyAt;
            }

            public Task Each(Action<Customer> cb)
            {
                throw new NotImplementedException();
            }

            public Task<IList<Customer>> AsListUnpaginated()
            {
                throw new NotImplementedException();
            }

            public IResourceView<Customer> Page(int p)
            {
                if (p < PageToReturnEmptyAt)
                {
                    return new ResourcePageMock(p, 5);
                }
                else
                {
                    return new ResourcePageMock(p, 0);
                }
            }

            public IResourceView<Customer> Where(System.Linq.Expressions.Expression<Func<Customer, object>> propertyLambda, string isEqualTo)
            {
                throw new NotImplementedException();
            }

            public IResourceView<Customer> Where(string field, string isEqualTo)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Specialized.NameValueCollection FullParameters()
            {
                throw new NotImplementedException();
            }

            public Type GetModelType()
            {
                throw new NotImplementedException();
            }

            public Task<int> Count()
            {
                throw new NotImplementedException();
            }

            public string Path()
            {
                throw new NotImplementedException();
            }

            public string InstancePath(int p)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void ShouldEnumerateThroughPagesNumerically()
        {
            // var resource = A.Fake<IResourceView<Customer>>();
            var resource = new PaginatedResourceMock(5);

            var enumerator = new PaginatedEnumerator<Customer>(resource);

            var answer = enumerator.AsList();

            answer.Wait();

            Assert.AreEqual(25, answer.Result.Count());
        }
    }
}
