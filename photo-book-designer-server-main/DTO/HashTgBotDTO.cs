using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class HashTgBotDTO
    {
        [Required]
        public string InitData { get; set; }
        [Required]
        public string Hash { get; set; }
    }
}
