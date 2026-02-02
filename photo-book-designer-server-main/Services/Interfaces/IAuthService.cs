using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces;

public interface IAuthService
{
    Task<TokenDTO> RegisterAsync(RegisterDTO register);
    Task<TokenDTO> LoginAsync(LoginDTO login);
    Task<UserProfileDTO> GetProfileAsync(Guid userId);
    Task<UserProfileDTO> UpdateProfileAsync(Guid userId, UpdateProfileDTO updateProfile);
}

