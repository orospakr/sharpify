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
    public class HasOneJsonConverter<T> : JsonConverter where T : IResourceModel, new()
    {
        protected string PropertyName;

        /// <summary>
        /// Be sure to instantiate of these for each property descriptor.
        /// </summary>
        /// <param name="propertyName">The underscorized name used in the JSON (without _id, although
        /// it's only used for serializing in the as-id case)</param>
        public HasOneJsonConverter(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// This is only interrogated on serialization.  The default contract
        /// logic matches by property name for deserialization.
        /// 
        /// Our own custom contract logic adds extra property definitions
        /// for the _id has one by id arrangement which will invoke
        /// this converter.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            // check if objectType implements IHasOne
           return typeof(IHasOneUntyped).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Converter set for the main property (non "_id") will always see only inlines.
            
            // inline:  incoming property is full object.

            T model = serializer.Deserialize<T>(reader);

            return new HasOneInline<T>(model);
        }

        public override void WriteJson(JsonWriter writer, object vee, JsonSerializer serializer)
        {
            // first off, which case are we in?  inline or not?  we discover this by testing
            // the type of the object.

            var hasOneInline = vee as HasOneInline<T>;

            if (hasOneInline != null)
            {
                // inline


                // if inline: get the model instance out of the HasOneInline and just call serializer.Serialize on it.
                // our system will do the usual exclusion of unchanged fields.  also, our system will be doing the usual
                // will-serialize check on this HasOne property itself, so we don't need to check that either.
            }
            else
            {
                // not inline (as id instead): get the ID out of the HasOne and write it out manually
                // with writer as the _id field.

                var hasOneAsId = (IHasOneUntyped)vee;

                // ANDREW START HERE shit how do I get out of property state?

                // can't do it. fail.

                // farts I need this property to not appear!
                writer.
                writer.WriteUndefined();

                var id = hasOneAsId.Id;

                writer.WritePropertyName(PropertyName + "_id");
                writer.WriteRawValue(id.ToString());

            }
        }
    }

    /// <summary>
    /// As the only way of detecting the different _id field is to have a separate JsonProperty
    /// entry, we have a variant of our converter for this case.
    /// </summary>
    public class IncomingHasOneAsIdJsonConverter<T> : HasOneJsonConverter<T> where T : IResourceModel, new()
    {
        public IncomingHasOneAsIdJsonConverter(string propertyName)
            : base(propertyName)
        {
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException("As ID (_id) has one JsonConverter should not be used at serialization time.");
        }

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
