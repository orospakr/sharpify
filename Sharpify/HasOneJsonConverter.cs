using Newtonsoft.Json;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary
{
    /// <summary>
    /// Binds IHasOne<> to JSON.
    /// 
    /// This handles serialization for both inlined and as-id has ones,
    /// and handles deserialization for only the inline version
    /// 
    /// See IncomingHasOneAsIdConverter for handling deserialization
    /// of as-id ("_id") fields.
    /// </summary>
    public class HasOneInlineJsonConverter<T> : JsonConverter where T : IResourceModel, new()
    {
        public HasOneInlineJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            // not actually used, as these are manually put on
            // JsonProperties by JsonDataTranslator.
            return true;
        }

        /// <summary>
        /// Check to see if this version of the HasOne JsonConverter should
        /// be used to serialize the given field.
        /// </summary>
        public static bool CheckIfHasOneShouldSerializeWithThis(Type objectType)
        {
            return typeof(IHasOneInlineUntyped).IsAssignableFrom(objectType);
            // return (objectType as IHasOneInlineUntyped) != null;
        }

        // inline read
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Converter set for the main property (non "_id") will always see only inlines.
            
            // inline:  incoming property is full object.

            T model = serializer.Deserialize<T>(reader);

            model.SetExisting();

            return new HasOneInline<T>(model);
        }

        // inline write
        public override void WriteJson(JsonWriter writer, object hasOne, JsonSerializer serializer)
        {
            // if inline: get the model instance out of the HasOneInline and just call serializer.Serialize on it.
            // our system will do the usual exclusion of unchanged fields.  also, our system will be doing the usual
            // will-serialize check on this HasOne property itself, so we don't need to check that either.

            var hasOneInline = hasOne as HasOneInline<T>;

            if (hasOneInline != null)
            {
                // inline
                serializer.Serialize(writer, hasOneInline.GetUntypedInlineModel());
            }
            else
            {
                // no HasOne on the property, so just write null.
                writer.WriteNull();
            }
        }
    }

    /// <summary>
    /// As the only way of detecting the different _id field is to have a separate JsonProperty
    /// entry, we have a variant of our converter for this case.
    /// </summary>
    public class HasOneAsIdJsonConverter<T> : JsonConverter where T : IResourceModel, new()
    {
        public HasOneAsIdJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType) {
            // not actually used, as these are manually put on
            // JsonProperties by JsonDataTranslator.
            return true;
        }

        // outgoing as id
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            // not inline (as id instead): get the ID out of the HasOne and write it.

            var hasOneAsId = (IHasOneAsIdUntyped)value;

            writer.WriteRawValue(hasOneAsId.Id.ToString());
        }

        // incoming as id
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // if as id: incoming property is an ID int.  return a HasOnePlaceholder() as the old HasOneConverter did
            var id = reader.Value;

            if (id == null)
            {
                return null;
            }
            return new HasOneDeserializationPlaceholder<T>(Int32.Parse(id.ToString()));
        }
    }
}
