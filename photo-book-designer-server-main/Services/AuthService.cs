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
        var passwordHash = await PasswordHashing(login.Password);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == passwordHash);
        if (user == null)
        {
            throw new BadHttpRequestException("Invalid credentials.");
        }

        var token = GenerateJwtToken(user);

        return new TokenDTO
        {
            Token = token
        };
    }

    public async Task<TokenDTO> RegisterAsync(RegisterDTO register) 
    {
        var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
        if (existing != null)
        {
            throw new BadHttpRequestException(message: $"Email {register.Email} is already taken");
        }

        var passwordHash = await PasswordHashing(register.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = register.Email,
            Name = register.Name,
            Password = passwordHash
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return await LoginAsync(new LoginDTO
        {
            Email = register.Email,
            Password = register.Password
        });
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
            var passwordHash = await PasswordHashing(updateProfile.Password);
            user.Password = passwordHash;
        }

        await _dbContext.SaveChangesAsync();

        return new UserProfileDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = updateProfile.Name
        };
    }

    private async Task<string> PasswordHashing(string password)
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
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
}

