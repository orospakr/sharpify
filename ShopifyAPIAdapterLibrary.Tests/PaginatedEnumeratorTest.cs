﻿using FakeItEasy;
using FakeItEasy.Configuration;
using NUnit.Framework;
using Sharpify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Tests
{
    // This test is just kind of terrible.  Sorry. :(
    [TestFixture]
    class PaginatedEnumeratorTest
    {
        public class Customer : ShopifyResourceModel
        {
            public Customer()
            {
            }
        }

        /// <summary>
        /// FakeItEasy mysteriously breaks with IResourceView<>.
        /// (http://stackoverflow.com/questions/7559354/faking-mocking-an-interface-gives-no-default-constructor-error-how-can-that-b)
        /// So I have to make own mocks the old fashioned way.
        /// </summary>
        public class ResourcePageMock : IRestResourceView<Customer>
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

            public IRestResourceView<Customer> Page(int p)
            {
                throw new NotImplementedException();
            }

            public IRestResourceView<Customer> Where(System.Linq.Expressions.Expression<Func<Customer, object>> propertyLambda, string isEqualTo)
            {
                throw new NotImplementedException();
            }

            public IRestResourceView<Customer> Where(string field, string isEqualTo)
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


            public Task<IList<Customer>> AsList()
            {
                throw new NotImplementedException();
            }

            public Task CallAction(Customer instance, string action) {
                throw new NotImplementedException();
            }

            public Task CallAction(Customer instance, Expression<Func<Customer, SpecialAction>> actionPropertyLambda)
            {
                throw new NotImplementedException();
            }
        }

        public class PaginatedResourceMock : IRestResourceView<Customer>
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

            public IRestResourceView<Customer> Page(int p)
            {
                if (p <= PageToReturnEmptyAt)
                {
                    return new ResourcePageMock(p, 5);
                }
                else
                {
                    return new ResourcePageMock(p, 0);
                }
            }

            public IRestResourceView<Customer> Where(System.Linq.Expressions.Expression<Func<Customer, object>> propertyLambda, string isEqualTo)
            {
                throw new NotImplementedException();
            }

            public IRestResourceView<Customer> Where(string field, string isEqualTo)
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


            public Task<IList<Customer>> AsList()
            {
                throw new NotImplementedException();
            }

            public Task CallAction(Customer instance, string action)
            {
                throw new NotImplementedException();
            }

            public Task CallAction(Customer instance, Expression<Func<Customer, SpecialAction>> actionPropertyLambda)
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
