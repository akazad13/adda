using System;

namespace DatingApp.API.DTOs
{
    public class PhotoForReturnDTO
    {
        public int Id { get; set; }
        public string url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
        public bool IsApproved { get; set; }
    }
}