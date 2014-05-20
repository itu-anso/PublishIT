using System;
using System.Runtime.Serialization;

namespace PublishITService.DTOs
{
    public class MediaDTO
    {
        [DataMember]
        public int MediaId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int FormatId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? Date { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public double? AvgRating { get; set; }
        [DataMember]
        public int? NumberOfDownloads { get; set; }

        //For video
        [DataMember]
        public double? Length { get; set; }
        [DataMember]
        public int? NumberOfRents { get; set; }
        [DataMember]
        public int? NumberOfTrailerViews { get; set; }

        //For document
        [DataMember]
        public string Status { get; set; }
    }
}