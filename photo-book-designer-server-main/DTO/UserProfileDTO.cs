namespace photo_book_designer_server_main.DTO
{
    public class UserProfileDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
