namespace photo_book_designer_server_main.DTO
{
    public class UploadTgBotPhotoDTO
    {
        public IFormFile File { get; set; }
        public int ChatId { get; set; }
        public HashTgBotDTO HashTgBot { get; set; }
    }
}
