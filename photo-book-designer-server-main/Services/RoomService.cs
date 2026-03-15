using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using photo_book_designer_server_main.Data;
using photo_book_designer_server_main.Data.Models;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.MongoData;
using photo_book_designer_server_main.Services.Interfaces;
using System.Text.Json;

namespace photo_book_designer_server_main.Services;

public class RoomService : IRoomService
{
    private readonly PhotoBookDBContext _dbContext;
    private readonly IMongoCollection<Page> _templatesCollection;

    public RoomService(PhotoBookDBContext dbContext, IMongoDatabase mongoDatabase)
    {
        _dbContext = dbContext;
        _templatesCollection = mongoDatabase.GetCollection<Page>("pages");
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
            InviteCode = GenerateInviteCode(),
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
            InviteCode = room.InviteCode,
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
            InviteCode = room.InviteCode,
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
            InviteCode = room.InviteCode,
            PagesNum = room.pagesNum,
            WidthTemplate = room.widthTemplate,
            HeightTemplate = room.heightTemplate
        });
    }

    public async Task<CertainRoomDTO> GetRoomByIdAsync(Guid userId, Guid roomId)
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

        var users = await _dbContext.UserRoom
        .Where(ur => ur.RoomId == room.Id)
        .Join(_dbContext.Users,
            ur => ur.UserId,
            u => u.Id,
            (ur, u) => new UserProfileDTO
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name
            })
        .ToListAsync();

        var pages = await _templatesCollection
            .Find(p => p.RoomId == roomId)
            .SortBy(p => p.PageNumber)
            .Project(p => new PageDTO
            {
                RoomId = p.RoomId,
                PageNumber = p.PageNumber,
                Shapes = p.Shapes
            })
            .ToListAsync();

        return new CertainRoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            AuthorId = room.AuthorId,
            InviteCode = room.InviteCode,
            PagesNum = room.pagesNum,
            WidthTemplate = room.widthTemplate,
            HeightTemplate = room.heightTemplate,
            Users = users,
            Pages = pages
        };
    }

    public async Task<List<UserProfileDTO>> AddUserIntoRoom(AddUserDTO addUser, Guid userId)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.InviteCode == addUser.InviteCode);
        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = await _dbContext.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == room.Id);
        if (isMember)
        {
            throw new UnauthorizedAccessException("User is already the member of the room.");
        }

        var userRoom = new UserRoom
        {
            UserId = userId,
            RoomId = room.Id
        };
        await _dbContext.UserRoom.AddAsync(userRoom);
        await _dbContext.SaveChangesAsync();

        var users = await _dbContext.UserRoom
        .Where(ur => ur.RoomId == room.Id)
        .Join(_dbContext.Users,
            ur => ur.UserId,
            u => u.Id,
            (ur, u) => new UserProfileDTO
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name
            })
        .ToListAsync();

        return users;
    }

    public async Task<List<UserProfileDTO>> RemoveUserFromRoom(RemoveUserDTO removeUser, Guid adminId)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == removeUser.roomId);
        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = await _dbContext.UserRoom.FirstOrDefaultAsync(ur => ur.UserId == removeUser.userId && ur.RoomId == removeUser.roomId);
        if (isMember == null)
        {
            throw new UnauthorizedAccessException("User is not the member of the room.");
        }

        if (room.AuthorId != adminId && removeUser.userId != adminId) 
        {
            throw new BadHttpRequestException("You aren't admin.");
        }

        _dbContext.UserRoom.Remove(isMember);
        await _dbContext.SaveChangesAsync();

        var users = await _dbContext.UserRoom
        .Where(ur => ur.RoomId == room.Id)
        .Join(_dbContext.Users,
            ur => ur.UserId,
            u => u.Id,
            (ur, u) => new UserProfileDTO
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name
            })
        .ToListAsync();

        return users;
    }

    public async Task<PageDTO> CreateOrUpdatePage(Guid userId, CreatePageDTO page)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == page.RoomId);
        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = await _dbContext.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == page.RoomId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("User is not the member of the room.");
        }

        var shapesAsBson = new List<BsonDocument>();

        foreach (var shape in page.Shapes)
        {
            var json = JsonSerializer.Serialize(shape);
            var bsonDoc = BsonDocument.Parse(json);
            shapesAsBson.Add(bsonDoc);
        }

        var newPage = new Page
        {
            Id = Guid.NewGuid().ToString(),
            RoomId = page.RoomId,
            PageNumber = page.PageNumber,
            HtmlContent = page.HtmlContent,
            Shapes = shapesAsBson
        };

        var oldPage = await _templatesCollection.Find(p => p.RoomId == page.RoomId && p.PageNumber == page.PageNumber).FirstOrDefaultAsync();
        if (oldPage == null)
        {
            await _templatesCollection.InsertOneAsync(newPage);
        } 
        else
        {
            newPage.Id = oldPage.Id;
            await _templatesCollection.ReplaceOneAsync(p => p.Id == oldPage.Id, newPage);
        }
        
        return new PageDTO
        {
            RoomId = page.RoomId,
            PageNumber = page.PageNumber,
            Shapes = shapesAsBson
        };
    }

    private string GenerateInviteCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        const int length = 8;

        while (true)
        {
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (!_dbContext.Rooms.Any(r => r.InviteCode == code))
            {
                return code;
            }
        }
    }
}

