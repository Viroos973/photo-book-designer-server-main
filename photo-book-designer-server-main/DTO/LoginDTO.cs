using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
