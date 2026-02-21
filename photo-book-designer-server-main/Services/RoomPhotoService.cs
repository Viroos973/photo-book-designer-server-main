using Microsoft.EntityFrameworkCore;
using photo_book_designer_server_main.Data;
using photo_book_designer_server_main.Data.Models;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace photo_book_designer_server_main.Services;

public class RoomPhotoService : IRoomPhotoService
{
    private readonly PhotoBookDBContext _dbContext;
    private readonly IPhotoStorageClient _photoStorageClient;

    public RoomPhotoService(PhotoBookDBContext dbContext, IPhotoStorageClient photoStorageClient)
    {
        _dbContext = dbContext;
        _photoStorageClient = photoStorageClient;
    }

    public async Task<RoomPhotoDTO> UploadRoomPhotoAsync(Guid userId, UploadRoomPhotoDTO uploadDto)
    {
        var room = await _dbContext.Rooms
            .Include(r => r.UserRooms)
            .FirstOrDefaultAsync(r => r.Id == uploadDto.RoomId);

        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = room.UserRooms.Any(ur => ur.UserId == userId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("User is not the member of the room.");
        }

        try
        {
            var photoStorageResult = await _photoStorageClient.UploadPhotoAsync(uploadDto.File);

            var roomPhoto = new RoomPhoto
            {
                ImageId = photoStorageResult.ImageId,
                ImageUrl = photoStorageResult.ImageUrl,
                RoomId = uploadDto.RoomId
            };

            await _dbContext.RoomPhotos.AddAsync(roomPhoto);
            await _dbContext.SaveChangesAsync();

            return new RoomPhotoDTO
            {
                ImageId = roomPhoto.ImageId,
                ImageUrl = roomPhoto.ImageUrl,
                RoomId = roomPhoto.RoomId
            };
        }
        catch
        {
            throw new BadHttpRequestException("Error when uploading a photo.");
        }
    }

    public async Task DeleteRoomPhotoAsync(Guid userId, string photoId)
    {
        var roomPhoto = await _dbContext.RoomPhotos
            .Include(p => p.Room)
            .ThenInclude(r => r.UserRooms)
            .FirstOrDefaultAsync(p => p.ImageId == photoId);

        if (roomPhoto == null)
        {
            throw new BadHttpRequestException("Photo not found.");
        }

        var isMember = roomPhoto.Room.UserRooms.Any(ur => ur.UserId == userId);
        var isAuthor = roomPhoto.Room.AuthorId == userId;

        if (!isMember && !isAuthor)
        {
            throw new UnauthorizedAccessException("You aren't admin.");
        }

        try
        {
            await _photoStorageClient.DeletePhotoAsync(roomPhoto.ImageId);

            _dbContext.RoomPhotos.Remove(roomPhoto);
            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            throw new BadHttpRequestException("Error when delete a photo");
        }
    }

    public async Task<GetRoomPhotoDTO> GetRoomPhotosAsync(Guid userId, Guid roomId, int? page, int? size)
    {
        if (size <= 0)
        {
            throw new BadHttpRequestException("Size value must be greater than 0");
        }

        var room = await _dbContext.Rooms
            .Include(r => r.UserRooms)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
        {
            throw new BadHttpRequestException("Room not found.");
        }

        var isMember = room.UserRooms.Any(ur => ur.UserId == userId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("User is not the member of the room.");
        }

        var photos = _dbContext.RoomPhotos
            .Where(p => p.RoomId == roomId);

        var totalCount = await photos.CountAsync();
        var maxPage = (int)Math.Ceiling(totalCount / (double)size);

        if (page < 1 || totalCount <= (page - 1) * size)
        {
            throw new BadHttpRequestException($"Page value must be greater than 0 and less than {maxPage + 1}");
        }

        var items = await photos
            .Skip((int)((page - 1) * size))
            .Take((int)size)
            .Select(p => new RoomPhotoDTO
            {
                ImageId = p.ImageId,
                ImageUrl = p.ImageUrl,
                RoomId = p.RoomId
            })
            .ToListAsync();

        var pagination = new Pagination
        {
            TotalCount = totalCount,
            Page = (int)page,
            TotalPages = maxPage,
            PageSize = (int)size
        };

        return new GetRoomPhotoDTO
        {
            RoomPhotos = items,
            Pagination = pagination
        };
    }

    public async Task<RoomPhotoDTO> GetRoomPhotoByIdAsync(Guid userId, string photoId)
    {
        var photo = await _dbContext.RoomPhotos
            .Include(p => p.Room)
            .ThenInclude(r => r.UserRooms)
            .FirstOrDefaultAsync(p => p.ImageId == photoId);

        if (photo == null)
        {
            throw new BadHttpRequestException("Photo not found.");
        }

        var isMember = photo.Room.UserRooms.Any(ur => ur.UserId == userId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You aren't admin.");
        }

        return new RoomPhotoDTO
        {
            ImageId = photo.ImageId,
            ImageUrl = photo.ImageUrl,
            RoomId = photo.RoomId
        };
    }
}
