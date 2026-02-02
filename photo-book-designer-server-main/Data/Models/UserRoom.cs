using System.ComponentModel.DataAnnotations.Schema;

namespace photo_book_designer_server_main.Data.Models
{
    public class UserRoom
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
    }
}
