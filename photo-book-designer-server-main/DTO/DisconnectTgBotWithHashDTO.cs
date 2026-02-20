using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class DisconnectTgBotWithHashDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public HashTgBotDTO HashTgBot { get; set; }
    }
}
