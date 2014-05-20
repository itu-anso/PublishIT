using System.Runtime.Serialization;

namespace PublishITService.DTOs
{
    public class GenreDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string GenreName { get; set; }
    }
}