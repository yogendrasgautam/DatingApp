using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(10,MinimumLength=5,ErrorMessage="Please specify password between 4 to 10 characters.")]
        public string Password { get; set; }
    }
}