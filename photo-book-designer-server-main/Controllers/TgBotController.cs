using Microsoft.AspNetCore.Mvc;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TgBotController : ControllerBase
{
    private readonly IRoomPhotoService _roomPhotoService;
    private readonly ITgBotService _tgBotService;

    public TgBotController(IRoomPhotoService roomPhotoService, ITgBotService tgBotService)
    {
        _roomPhotoService = roomPhotoService;
        _tgBotService = tgBotService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadTgBotPhoto([FromForm] UploadTgBotPhotoDTO uploadDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _tgBotService.ValidateTelegramInitData(uploadDto.HashTgBot);
            var userId = await _tgBotService.GetTgBotId(uploadDto.ChatId);
            var roomId = await _tgBotService.GetBotRoomId(uploadDto.ChatId);
            var upload = new UploadRoomPhotoDTO
            {
                File = uploadDto.File,
                RoomId = roomId
            };

            var result = await _roomPhotoService.UploadRoomPhotoAsync(userId, upload, true);

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

    [HttpDelete("photos/{photoId}/bot/{botId}")]
    public async Task<IActionResult> DeleteTgBotPhoto(string photoId, int botId, [FromBody] HashTgBotDTO hash)
    {
        try
        {
            await _tgBotService.ValidateTelegramInitData(hash);
            var userId = await _tgBotService.GetTgBotId(botId);
            await _roomPhotoService.DeleteRoomPhotoAsync(userId, photoId, true);

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Фото успешно удалено"
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

    [HttpPost("bot/{botId}/photos")]
    public async Task<IActionResult> GetTgBotPhotos([FromBody] HashTgBotDTO hash, int botId, int? page = 1, int? size = 5)
    {
        try
        {
            await _tgBotService.ValidateTelegramInitData(hash);
            var userId = await _tgBotService.GetTgBotId(botId);
            var roomId = await _tgBotService.GetBotRoomId(botId);
            var photos = await _roomPhotoService.GetRoomPhotosAsync(userId, roomId, page, size, true);

            return Ok(photos);
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

    [HttpPost("bot/connect")]
    public async Task<IActionResult> BotConnect([FromBody] ConnectTgBotWithHashDTO connect)
    {
        try
        {
            await _tgBotService.ValidateTelegramInitData(connect.HashTgBot);
            await _tgBotService.ConnectTgBot(connect.TgBotData);

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Bot connected successfully"
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

    [HttpPost("bot/disconnect")]
    public async Task<IActionResult> BotDisconnect([FromBody] DisconnectTgBotWithHashDTO disconnect)
    {
        try
        {
            await _tgBotService.ValidateTelegramInitData(disconnect.HashTgBot);
            await _tgBotService.DisconnectTgBot(disconnect.ChatId);

            return Ok(new Responce
            {
                Status = "Success",
                Message = "Bot disconnected successfully"
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
