using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces
{
    public interface IPageService
    {
        Task<PageDTO> CreateOrUpdatePage(Guid userId, CreatePageDTO page);
        Task<PageDTO> GetPageByNumber(Guid userId, Guid roomId, int pageNumber);
        Task<List<PageDTO>> GetPagesByRoomId(Guid userId, Guid roomId);
    }
}
