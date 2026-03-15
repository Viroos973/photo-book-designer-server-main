using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using photo_book_designer_server_main.Data;
using photo_book_designer_server_main.Data.Models;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Options;
using photo_book_designer_server_main.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace photo_book_designer_server_main.Services;

public class AuthService : IAuthService
{
    private readonly PhotoBookDBContext _dbContext;
    private readonly JwtOptions _jwtOptions;

    public AuthService(PhotoBookDBContext dbContext, IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<TokenDTO> LoginAsync(LoginDTO login)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
        if (user == null || !VerifyPassword(login.Password, user.Password))
        {
            throw new BadHttpRequestException("Invalid credentials.");
        }

        return await GenerateTokensAsync(user);
    }

    public async Task<TokenDTO> RegisterAsync(RegisterDTO register)
    {
        var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
        if (existing != null)
        {
            throw new BadHttpRequestException($"Email {register.Email} is already taken");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = register.Email,
            Name = register.Name,
            Password = HashPassword(register.Password)
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<TokenDTO> RefreshTokenAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || token.ExpiresAt < DateTime.UtcNow || token.RevokedAt != null)
        {
            throw new BadHttpRequestException("Invalid or expired refresh token");
        }

        token.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return await GenerateTokensAsync(token.User);
    }

    public async Task LogoutAsync(Guid userId, string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<UserProfileDTO> GetProfileAsync(Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new BadHttpRequestException("User not found.");
        }

        return new UserProfileDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };
    }

    public async Task<UserProfileDTO> UpdateProfileAsync(Guid userId, UpdateProfileDTO updateProfile)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new BadHttpRequestException("User not found.");
        }

        user.Name = updateProfile.Name;
        if (!string.IsNullOrWhiteSpace(updateProfile.Password))
        {
            user.Password = HashPassword(updateProfile.Password);
        }

        await _dbContext.SaveChangesAsync();

        return new UserProfileDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = updateProfile.Name
        };
    }

    private async Task<TokenDTO> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return new TokenDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = _jwtOptions.ExpiresMinutes * 60
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}