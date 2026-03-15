using System.ComponentModel.DataAnnotations;

namespace photo_book_designer_server_main.Data.Enums
{
    public enum PhotoRole
    {
        [Display(Name = "UserPhoto")]
        UserPhoto,
        [Display(Name = "UserBackground")]
        UserBackground,
        [Display(Name = "DefaultBackground")]
        DefaultBackground
    }
}
