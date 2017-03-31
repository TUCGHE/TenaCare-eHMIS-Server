using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace eHMISWebApi.Caching
{
    public static class SerializerHelper
    {
        public static byte[] Serialize(object data)
        {
            var preparedData = new SerializableObject(data);
            byte[] result;
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, preparedData);
                result = stream.ToArray(); //GetBuffer was giving me a Protobuf.ProtoException of "Invalid field in source data: 0" when deserializing
            }

            return result;
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var returnValue = ProtoBuf.Serializer.Deserialize<T>(stream);

                return returnValue;
            }
        }
    }
}