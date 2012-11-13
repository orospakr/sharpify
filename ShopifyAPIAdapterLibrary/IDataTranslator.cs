using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;

namespace ShopifyAPIAdapterLibrary
{

    /// <summary>
    /// An instance of this interface would translate the data between c# and the Shopify API
    /// </summary>
    public interface IDataTranslator
    {
        /// <summary>
        /// Encode the data in a way that is expected by the Shopify API
        /// </summary>
        /// <param name="data">data that should be encoded for the Shopify API</param>
        /// <returns></returns>
        string Encode(object data);

        /// <summary>
        /// Decode the data returned by the Shopify API
        /// </summary>
        /// <param name="encodedData">data encoded by the Shopify API</param>
        /// <returns></returns>
        object Decode(string encodedData);

        /// <summary>
        /// Shopify's API returns most things wrapped in single JSON field, named by the
        /// resource being fetched ("product", "products", and so on.)
        /// 
        /// This will return that wrapped resource in a type-safe fashion.
        /// </summary>
        T ResourceDecode<T>(String subfieldName, String content);

        /// <summary>
        /// Analagous to ResourceDecode(), serializes a model
        /// wrapped in an object with a single JSON field of the specified
        /// type name.
        /// </summary>
        string ResourceEncode<T>(string subFieldName, T model);

        /// <summary>
        /// The Content Type (Mime Type) used by this translator
        /// </summary>
        /// <returns></returns>
        MediaTypeHeaderValue GetContentType();
    }
}
