using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.Data.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public List<UserRoom> UserRooms { get; set; }
    }
}
