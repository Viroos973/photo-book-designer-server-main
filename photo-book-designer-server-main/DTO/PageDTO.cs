using MongoDB.Bson;

namespace photo_book_designer_server_main.DTO
{
    public class PageDTO
    {
        public Guid RoomId { get; set; }
        public int PageNumber { get; set; }
        public List<object> Shapes { get; set; }
    }
}
