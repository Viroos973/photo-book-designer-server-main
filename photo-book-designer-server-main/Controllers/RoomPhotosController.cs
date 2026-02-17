using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomPhotosController : ControllerBase
{
    private readonly IRoomPhotoService _roomPhotoService;
    private readonly ITokenService _tokenService;

    public RoomPhotosController(IRoomPhotoService roomPhotoService, ITokenService tokenService)
    {
        _roomPhotoService = roomPhotoService;
        _tokenService = tokenService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadRoomPhoto([FromForm] UploadRoomPhotoDTO uploadDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var result = await _roomPhotoService.UploadRoomPhotoAsync(userId, uploadDto);

            return Ok(result);
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

    [HttpDelete("photos/{photoId}")]
    public async Task<IActionResult> DeleteRoomPhoto(string photoId)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            await _roomPhotoService.DeleteRoomPhotoAsync(userId, photoId);

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Фото успешно удалено"
            });
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

    [HttpGet("rooms/{roomId}/photos")]
    public async Task<IActionResult> GetRoomPhotos(Guid roomId)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var photos = await _roomPhotoService.GetRoomPhotosAsync(userId, roomId);

            return Ok(photos);
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

    [HttpGet("photos/{photoId}")]
    public async Task<IActionResult> GetRoomPhotoById(string photoId)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var photo = await _roomPhotoService.GetRoomPhotoByIdAsync(userId, photoId);

            return Ok(photo);
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
