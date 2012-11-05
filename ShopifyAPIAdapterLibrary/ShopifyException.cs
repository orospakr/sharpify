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

        public ShopifyException(String reason, HttpStatusCode statusCode): base(String.Format("{0}: {1}", statusCode, reason))
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : ShopifyException {
        public NotFoundException(String reason, HttpStatusCode statusCode) : base(reason, statusCode)
        {
        }
    }

    public class InvalidContentException: ShopifyException {
        public InvalidContentException(String reason, HttpStatusCode statusCode) : base(reason, statusCode)
        {
        }
    }
}

