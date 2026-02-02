using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class UpdateProfileDTO
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        [MinLength(6)]
        public string? Password { get; set; }
    }
}
