using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PageController : ControllerBase
{
    private readonly IPageService _pageService;
    private readonly ITokenService _tokenService;

    public PageController(IPageService pageService, ITokenService tokenService)
    {
        _pageService = pageService;
        _tokenService = tokenService;
    }

    [Authorize]
    [HttpPost("create-update-page")]
    public async Task<ActionResult> CreateOrUpdatePage([FromBody] CreatePageDTO page)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var newPage = await _pageService.CreateOrUpdatePage(userId, page);
            return Ok(newPage);
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
    [HttpGet("get-by-number/{roomId:guid}/{pageNumber:int}")]
    public async Task<ActionResult> GetPageByNumber(Guid roomId, int pageNumber)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var page = await _pageService.GetPageByNumber(userId, roomId, pageNumber);
            return Ok(page);
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
    [HttpGet("room/{roomId:guid}")]
    public async Task<ActionResult> GetPagesByRoomId(Guid roomId)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var page = await _pageService.GetPagesByRoomId(userId, roomId);
            return Ok(page);
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
