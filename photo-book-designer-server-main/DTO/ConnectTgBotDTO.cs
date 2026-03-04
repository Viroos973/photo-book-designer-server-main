using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class ConnectTgBotDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string InviteCode { get; set; }
    }
}
