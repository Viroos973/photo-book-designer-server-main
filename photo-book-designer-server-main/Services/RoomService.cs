using Microsoft.EntityFrameworkCore;
using photo_book_designer_server_main.Data;
using photo_book_designer_server_main.Data.Models;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Services;

public class RoomService : IRoomService
{
    private readonly PhotoBookDBContext _dbContext;

    public RoomService(PhotoBookDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomDTO> CreateRoomAsync(Guid userId, CreateRoomDTO createRoom)
    {
        if (createRoom.PagesNum <= 0 || createRoom.WidthTemplate <= 0 || createRoom.HeightTemplate <= 0)
        {
            throw new BadHttpRequestException("Room dimensions and pages number must be greater than zero.");
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = createRoom.Name,
            AuthorId = userId,
            pagesNum = createRoom.PagesNum,
            widthTemplate = createRoom.WidthTemplate,
            heightTemplate = createRoom.HeightTemplate
        };
        await _dbContext.Rooms.AddAsync(room);

        var userRoom = new UserRoom
        {
            UserId = userId,
            RoomId = room.Id
        };
        await _dbContext.UserRoom.AddAsync(userRoom);

        await _dbContext.SaveChangesAsync();

        return new RoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            AuthorId = room.AuthorId,
            PagesNum = room.pagesNum,
            WidthTemplate = room.widthTemplate,
            HeightTemplate = room.heightTemplate
        };
    }

    public async Task DeleteRoomAsync(Guid userId, Guid roomId)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        if (room.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("User is not the author of the room.");
        }

        var userRooms = _dbContext.UserRoom.Where(ur => ur.RoomId == roomId);
        _dbContext.UserRoom.RemoveRange(userRooms);

        _dbContext.Rooms.Remove(room);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<RoomDTO> UpdateRoomNameAsync(Guid userId, Guid roomId, UpdateRoomNameDTO updateRoomName)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = await _dbContext.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("User is not the member of the room.");
        }

        room.Name = updateRoomName.Name;
        await _dbContext.SaveChangesAsync();

        return new RoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            AuthorId = room.AuthorId,
            PagesNum = room.pagesNum,
            WidthTemplate = room.widthTemplate,
            HeightTemplate = room.heightTemplate
        };
    }

    public async Task<IEnumerable<RoomDTO>> GetRoomsAsync(Guid userId)
    {
        var rooms = await _dbContext.Rooms
            .Include(r => r.UserRooms)
            .Where(r => r.UserRooms.Any(ur => ur.UserId == userId))
            .ToListAsync();

        return rooms.Select(room => new RoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            AuthorId = room.AuthorId,
            PagesNum = room.pagesNum,
            WidthTemplate = room.widthTemplate,
            HeightTemplate = room.heightTemplate
        });
    }

    public async Task<RoomDTO> GetRoomByIdAsync(Guid userId, Guid roomId)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = await _dbContext.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("User is not the member of the room.");
        }

        return new RoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            AuthorId = room.AuthorId,
            PagesNum = room.pagesNum,
            WidthTemplate = room.widthTemplate,
            HeightTemplate = room.heightTemplate
        };
    }
}

