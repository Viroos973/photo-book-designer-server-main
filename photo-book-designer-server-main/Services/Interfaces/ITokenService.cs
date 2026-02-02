using System.Security.Claims;

namespace photo_book_designer_server_main.Services.Interfaces;

public interface ITokenService
{
    Guid GetUserIdFromClaims(ClaimsPrincipal user);
}
