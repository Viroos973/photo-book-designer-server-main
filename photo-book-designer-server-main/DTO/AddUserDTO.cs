using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.DTO
{
    public class AddUserDTO
    {
        [Required]
        public string InviteCode { get; set; }
    }
}
