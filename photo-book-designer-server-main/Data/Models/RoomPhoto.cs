using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace photo_book_designer_server_main.Data.Models
{
    public class RoomPhoto
    {
        [Key]
        [Required]
        public string ImageId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public Guid RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
    }
}
