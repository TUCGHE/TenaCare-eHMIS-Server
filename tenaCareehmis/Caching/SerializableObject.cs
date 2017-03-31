using ProtoBuf;
namespace eHMISWebApi.Caching
{
    [ProtoContract]
    public class SerializableObject
    {
        public SerializableObject(object data)
        {
            Data = data;
        }
        [ProtoMember(1, DynamicType = true)]
        public object Data { get; set; }
    }
}