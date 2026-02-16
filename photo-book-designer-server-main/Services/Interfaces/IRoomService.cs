using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces;

public interface IRoomService
{
    Task<RoomDTO> CreateRoomAsync(Guid userId, CreateRoomDTO createRoom);
    Task DeleteRoomAsync(Guid userId, Guid roomId);
    Task<RoomDTO> UpdateRoomNameAsync(Guid userId, Guid roomId, UpdateRoomNameDTO updateRoomName);
    Task<IEnumerable<RoomDTO>> GetRoomsAsync(Guid userId);
    Task<CertainRoomDTO> GetRoomByIdAsync(Guid userId, Guid roomId);
    Task<CertainRoomDTO> AddUserIntoRoom(AddUserDTO addUser, Guid userId);
    Task<CertainRoomDTO> RemoveUserFromRoom(RemoveUserDTO removeUser, Guid adminId);
}

