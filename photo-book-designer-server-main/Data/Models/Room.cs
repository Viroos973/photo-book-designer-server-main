using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.Data.Models
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        public int pagesNum { get; set; }
        [Required]
        public int widthTemplate { get; set; }
        [Required]
        public int heightTemplate { get; set; }

        public List<TgBot> TgBots { get; set; }
        public List<UserRoom> UserRooms { get; set; }
    }
}
