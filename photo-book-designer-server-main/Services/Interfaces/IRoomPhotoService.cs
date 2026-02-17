using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces;

public interface IRoomPhotoService
{
    Task<RoomPhotoDTO> UploadRoomPhotoAsync(Guid userId, UploadRoomPhotoDTO uploadDto);
    Task DeleteRoomPhotoAsync(Guid userId, string photoId);
    Task<IEnumerable<RoomPhotoDTO>> GetRoomPhotosAsync(Guid userId, Guid roomId);
    Task<RoomPhotoDTO> GetRoomPhotoByIdAsync(Guid userId, string photoId);
}
