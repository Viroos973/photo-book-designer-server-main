using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDTO register)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authService.RegisterAsync(register);
            return Ok(result);
        } 
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new Responce
            {
                Status = "405",
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Responce
            {
                Status = "500",
                Message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO login)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authService.LoginAsync(login);
            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new Responce
            {
                Status = "405",
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Responce
            {
                Status = "500",
                Message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult> GetProfile()
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var profile = await _authService.GetProfileAsync(userId);
            return Ok(profile);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new Responce
            {
                Status = "401",
                Message = "User is not authorized"
            });
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new Responce
            {
                Status = "405",
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Responce
            {
                Status = "500",
                Message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO updateProfile)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var updated = await _authService.UpdateProfileAsync(userId, updateProfile);
            return Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new Responce
            {
                Status = "401",
                Message = "User is not authorized"
            });
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new Responce
            {
                Status = "405",
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Responce
            {
                Status = "500",
                Message = ex.Message
            });
        }
    }
}

