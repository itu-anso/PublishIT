using System.Runtime.Serialization;

namespace PublishITService.DTOs
{
    [DataContract]
    public class RoleDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}