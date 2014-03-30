using System.Runtime.Serialization;

namespace PublishITService
{
    [DataContract]
    public class ResponseMessage
    {
        [DataMember]
        public bool IsExecuted { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}