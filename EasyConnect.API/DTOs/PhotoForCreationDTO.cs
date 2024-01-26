using System;
using Microsoft.AspNetCore.Http;

namespace EasyConnect.API.DTOs
{
    public class PhotoForCreationDTO
    {
        public PhotoForCreationDTO()
        {
            this.DateAdded = DateTime.Now;
        }

        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
    }
}
