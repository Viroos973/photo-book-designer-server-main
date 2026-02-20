using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces
{
    public interface ITgBotService
    {
        Task<bool> ValidateTelegramInitData(HashTgBotDTO hashTgBot);
        Task ConnectTgBot(ConnectTgBotDTO connect);
        Task DisconnectTgBot(Guid chatId);
    }
}
