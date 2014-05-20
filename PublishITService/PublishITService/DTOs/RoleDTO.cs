using System.Runtime.Serialization;

namespace PublishITService.DTOs
{
        
    /// <summary>
    /// RoleDTO functions as the data type for roles.
    /// </summary>
    [DataContract]
    public class RoleDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}