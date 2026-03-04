using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class DisconnectTgBotWithHashDTO
    {
        [Required]
        public int ChatId { get; set; }
        [Required]
        public HashTgBotDTO HashTgBot { get; set; }
    }
}
