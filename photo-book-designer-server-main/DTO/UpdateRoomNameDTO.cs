using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class UpdateRoomNameDTO
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}

