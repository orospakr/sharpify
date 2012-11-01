using System;
using System.Net;

namespace ShopifyAPIAdapterLibrary
{
    public class ShopifyException : Exception
    {
        HttpStatusCode StatusCode
        {
            get;
            set;
        }

        public ShopifyException(String reason, HttpStatusCode statusCode): base(reason)
        {
            StatusCode = statusCode;
        }
    }
}

