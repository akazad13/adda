using System;

namespace DatingApp.API.DTOs
{
    public class PhotosForDetailedDTO
    {
        public int Id { get; set; }
        public string url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
    }
}