using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces;

public interface IRoomPhotoService
{
    Task<RoomPhotoDTO> UploadRoomPhotoAsync(Guid userId, UploadRoomPhotoDTO uploadDto, bool isTgBot);
    Task<RoomPhotoDTO> UploadBackgroundAsync(Guid userId, UploadRoomPhotoDTO uploadDto);
    Task DeleteRoomPhotoAsync(Guid userId, string photoId, bool isTgBot);
    Task<GetRoomPhotoDTO> GetRoomPhotosAsync(Guid userId, Guid roomId, int? page, int? size, bool isTgBot);
    Task<RoomPhotoDTO> GetRoomPhotoByIdAsync(Guid userId, string photoId);
}
