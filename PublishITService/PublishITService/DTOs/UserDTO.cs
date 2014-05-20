using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PublishITService.DTOs
{
    /// <summary>
    /// UserDTO functions as the data type for a user of the system. 
    /// </summary>
    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public int user_id { get; set; }
        [DataMember]
        public List<RoleDTO> roles { get; set; }
        [DataMember]
        public int organization_id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string username { get; set; }
        [EmailAddress]
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public DateTime? birthday { get; set; }
        [DataMember]
        public string salt { get; set; }
        [DataMember]
        public string status { get; set; }
    }
}