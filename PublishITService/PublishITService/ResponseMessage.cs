using System.Runtime.Serialization;

namespace PublishITService
{
    /// <summary>
    /// ResponseMessage object containing a boolean for IsExecuted and explaning message.
    /// </summary>
    [DataContract]
    public class ResponseMessage
    {
        [DataMember]
        public bool IsExecuted { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}