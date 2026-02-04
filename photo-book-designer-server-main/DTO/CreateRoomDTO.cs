using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class CreateRoomDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int PagesNum { get; set; }

        [Required]
        public int WidthTemplate { get; set; }

        [Required]
        public int HeightTemplate { get; set; }
    }
}

