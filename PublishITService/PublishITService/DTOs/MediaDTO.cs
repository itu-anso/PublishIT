using System;
using System.Runtime.Serialization;

namespace PublishITService.DTOs
{
    /// <summary>
    /// MediaDTO is the data type that covers for both a video- and document-type.. 
    /// </summary>
    public class MediaDTO
    {
        [DataMember]
        public int media_id { get; set; }
        [DataMember]
        public int user_id { get; set; }
        [DataMember]
        public int format_id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public DateTime? date { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public double? average_rating { get; set; }
        [DataMember]
        public int? number_of_downloads { get; set; }

        //For video
        [DataMember]
        public double? length { get; set; }
        [DataMember]
        public int? number_of_rents { get; set; }
        [DataMember]
        public int? number_of_trailer_views { get; set; }

        //For document
        [DataMember]
        public string status { get; set; }
    }
}