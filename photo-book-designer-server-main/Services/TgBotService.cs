using Microsoft.EntityFrameworkCore;
using photo_book_designer_server_main.Data;
using photo_book_designer_server_main.Data.Models;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace photo_book_designer_server_main.Services
{
    public class TgBotService : ITgBotService
    {
        private readonly string _botToken;
        private readonly PhotoBookDBContext _dbContext;

        public TgBotService(PhotoBookDBContext dbContext)
        {
            _dbContext = dbContext;
            _botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
                ?? throw new InvalidOperationException("TELEGRAM_BOT_TOKEN environment variable is not set");
        }

        public async Task<bool> ValidateTelegramInitData(HashTgBotDTO hashTgBot)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"));
            var secretKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(_botToken));

            using var hmacData = new HMACSHA256(secretKey);
            var hashBytes = hmacData.ComputeHash(Encoding.UTF8.GetBytes(hashTgBot.InitData));

            var expectedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            if (!string.Equals(expectedHash, hashTgBot.Hash, StringComparison.OrdinalIgnoreCase))
            {
                throw new BadHttpRequestException("This is not a request from a bot.");
            }

            return true;
        }

        public async Task ConnectTgBot(ConnectTgBotDTO connect)
        {
            var existingBot = await _dbContext.TgBots.FirstOrDefaultAsync(tb => tb.Id == connect.Id);
            if (existingBot != null)
            {
                throw new BadHttpRequestException("The bot is already connected.");
            }

            var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == connect.RoomId);
            if (room == null)
            {
                throw new BadHttpRequestException("Room not found.");
            }

            var tgBot = new TgBot
            {
                Id = connect.Id,
                Name = connect.Name,
                RoomId = room.Id
            };
            await _dbContext.TgBots.AddAsync(tgBot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DisconnectTgBot(Guid chatId)
        {
            var existingBot = await _dbContext.TgBots.FirstOrDefaultAsync(tb => tb.Id == chatId);
            if (existingBot == null)
            {
                throw new BadHttpRequestException("Bot not found.");
            }

            _dbContext.TgBots.Remove(existingBot);
            await _dbContext.SaveChangesAsync();
        }
    }
}
