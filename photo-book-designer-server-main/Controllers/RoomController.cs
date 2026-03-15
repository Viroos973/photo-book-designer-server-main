using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ITokenService _tokenService;

    public RoomController(IRoomService roomService, ITokenService tokenService)
    {
        _roomService = roomService;
        _tokenService = tokenService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetRooms()
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var rooms = await _roomService.GetRoomsAsync(userId);
            return Ok(rooms);
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
    [HttpPost]
    public async Task<ActionResult> CreateRoom([FromBody] CreateRoomDTO createRoom)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var room = await _roomService.CreateRoomAsync(userId, createRoom);
            return Ok(room);
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
    [HttpDelete("{roomId:guid}")]
    public async Task<ActionResult> DeleteRoom(Guid roomId)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            await _roomService.DeleteRoomAsync(userId, roomId);
            return Ok(new Responce
            {
                Status = "200",
                Message = "Room deleted successfully"
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

    [Authorize]
    [HttpPut("{roomId:guid}/name")]
    public async Task<ActionResult> UpdateRoomName(Guid roomId, [FromBody] UpdateRoomNameDTO updateRoomName)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var room = await _roomService.UpdateRoomNameAsync(userId, roomId, updateRoomName);
            return Ok(room);
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
    [HttpGet("{roomId:guid}")]
    public async Task<ActionResult> GetRoom(Guid roomId)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var room = await _roomService.GetRoomByIdAsync(userId, roomId);
            return Ok(room);
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
    [HttpPost("add-user")]
    public async Task<ActionResult> AddUser([FromBody] AddUserDTO addUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var room = await _roomService.AddUserIntoRoom(addUser, userId);
            return Ok(room);
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
    [HttpDelete("remove-user")]
    public async Task<ActionResult> RemoveUser([FromBody] RemoveUserDTO removeUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _tokenService.GetUserIdFromClaims(User);
            var room = await _roomService.RemoveUserFromRoom(removeUser, userId);
            return Ok(room);
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
            var newPage = await _roomService.CreateOrUpdatePage(userId, page);
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
}

