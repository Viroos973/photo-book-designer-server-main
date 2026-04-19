namespace photo_book_designer_server_main.DTO
{
    public class CertainRoomDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public string InviteCode { get; set; }
        public int PagesNum { get; set; }
        public int WidthTemplate { get; set; }
        public int HeightTemplate { get; set; }
        public List<UserProfileDTO> Users { get; set; }
    }
}
