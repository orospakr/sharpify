using System;
using System.Net;
using System.Net.Http;

namespace ShopifyAPIAdapterLibrary
{
    public class ShopifyException : Exception
    {
        public ShopifyException(String reason): base(reason)
        {
        }
    }

    public class ShopifyConfigurationException : ShopifyException {
        public ShopifyConfigurationException(String reason): base(reason)
        {
        }
    }

    public class ShopifyUsageException : ShopifyException
    {
        public ShopifyUsageException(String reason): base(reason)
        {
        }
    }

    public class ShopifyHttpException : ShopifyException {
        HttpResponseMessage Response
        {
            get;
            set;
        }

        public ShopifyHttpException(String reason, HttpResponseMessage response): base(String.Format("{0} --> {1}: {2}", response.RequestMessage.RequestUri.AbsolutePath, response.StatusCode, reason))
        {
            Response = response;
        }
    }

    public class NotFoundException : ShopifyHttpException {
        public NotFoundException(String reason, HttpResponseMessage response) : base(reason, response)
        {
        }
    }

    public class InvalidContentException: ShopifyHttpException {
        public InvalidContentException(String reason, HttpResponseMessage response) : base(reason, response)
        {
        }
    }
}

