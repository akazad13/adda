using System;
using System.ComponentModel.DataAnnotations;

namespace EasyConnect.API.Dtos
{
    public class UserForRegisterDto
    {
        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }

        [Required]
        public string Username { get; set; }
        public string Email { get; set; }

        [Required]
        [StringLength(
            8,
            MinimumLength = 4,
            ErrorMessage = "You must Specify password between 4 and 8 characters"
        )]
        public string Password { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}
