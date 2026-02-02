using photo_book_designer_server_main.Services.Interfaces;
using System.Security.Claims;

namespace photo_book_designer_server_main.Services;

public class TokenService : ITokenService
{
    public Guid GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user identifier");
        }
        return userId;
    }
}
