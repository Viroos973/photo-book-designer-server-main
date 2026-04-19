using System.Text.Json;

namespace photo_book_designer_server_main.DTO
{
    public class CreatePageDTO
    {
        public Guid RoomId { get; set; }
        public int PageNumber { get; set; }
        public string HtmlContent { get; set; }
        public List<JsonElement> Shapes { get; set; }
    }
}
