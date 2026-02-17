namespace photo_book_designer_server_main.DTO
{
    public class UploadRoomPhotoDTO
    {
        public IFormFile File { get; set; }
        public Guid RoomId { get; set; }
    }
}
