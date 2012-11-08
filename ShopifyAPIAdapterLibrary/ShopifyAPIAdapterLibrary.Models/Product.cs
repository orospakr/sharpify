using System;
using System.Collections.Generic;

namespace ShopifyAPIAdapterLibrary.Models
{
    public class Product : IResourceModel
    {

//        "body_html": "<p>It's the small iPod with one very big idea: Video. Now the world's most popular music player, available in 4GB and 8GB models, lets you enjoy TV shows, movies, video podcasts, and more. The larger, brighter display means amazing picture quality. In six eye-catching colors, iPod nano is stunning all around. And with models starting at just $149, little speaks volumes.</p>",
//        "created_at": "2012-10-30T16:09:40-04:00",
//        "handle": "ipod-nano",
//        "id": 632910392,
//        "product_type": "Cult Products",
//        "published_at": "2007-12-31T19:00:00-05:00",
//        "template_suffix": null,
//        "title": "IPod Nano - 8GB",
//        "updated_at": "2012-10-30T16:09:40-04:00",
//        "vendor": "Apple",
//        "tags": "Emotive, Flash Memory, MP3, Music",

        public string BodyHtml { get; set; }

        public DateTime CreatedAt { get; set; }

        public String Handle { get; set; }

        public String Id { get; set; }

        public DateTime PublishedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string TemplateSuffix { get; set; }

        public string Title { get; set; } 

        public string Vendor { get; set; }

        // TODO should be munged into a collection, it arrives as comma separated
        public String Tags { get; set; }

        public Product()
        {

        }
    }
}
