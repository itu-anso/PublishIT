using System;

namespace PublishITService.DTOs
{
    /// <summary>
    /// MediaDTO is the data type that covers for both a video- and document-type.. 
    /// </summary>
    public class MediaDTO
    {
        public int MediaId { get; set; }
        public int UserId { get; set; }
        public int FormatId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string Location { get; set; }
        public double? AvgRating { get; set; }
        public int? NumberOfDownloads { get; set; }

        //For video
        public double? Length { get; set; }
        public int? NumberOfRents { get; set; }
        public int? NumberOfTrailerViews { get; set; }

        //For document
        public string Status { get; set; }
    }
}