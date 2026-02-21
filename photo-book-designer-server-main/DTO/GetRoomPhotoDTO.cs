namespace photo_book_designer_server_main.DTO
{
    public class GetRoomPhotoDTO
    {
        public List<RoomPhotoDTO> RoomPhotos { get; set; }
        public Pagination Pagination { get; set; }
    }
}
