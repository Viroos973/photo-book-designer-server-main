using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class ConnectTgBotWithHashDTO
    {
        [Required]
        public ConnectTgBotDTO TgBotData { get; set; }
        [Required]
        public HashTgBotDTO HashTgBot {  get; set; }
    }
}
