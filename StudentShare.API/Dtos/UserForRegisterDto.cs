using System.ComponentModel.DataAnnotations;

namespace StudentShare.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"\b[A-Za-z0-9._%-]+@(live\.wcs\.ac\.uk)\b", ErrorMessage = "Email must be a WCS college email address.")]
        public string Email {get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 8 characters.")]
        public string Password {get; set; }


    }
}