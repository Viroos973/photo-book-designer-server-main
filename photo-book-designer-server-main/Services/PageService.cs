using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using photo_book_designer_server_main.Data;
using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.MongoData;
using photo_book_designer_server_main.Services.Interfaces;

namespace photo_book_designer_server_main.Services
{
    public class PageService : IPageService
    {
        private readonly PhotoBookDBContext _dbContext;
        private readonly IMongoCollection<Page> _templatesCollection;

        public PageService(PhotoBookDBContext dbContext, IMongoDatabase mongoDatabase)
        {
            _dbContext = dbContext;
            _templatesCollection = mongoDatabase.GetCollection<Page>("pages");
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
                var jsonString = shape.GetRawText();
                var bsonDoc = BsonDocument.Parse(jsonString);
                shapesAsBson.Add(bsonDoc);
            }

            var newPage = new Page
            {
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
                Shapes = shapesAsBson.Select(shape => BsonTypeMapper.MapToDotNetValue(shape)).ToList()
            };
        }

        public async Task<PageDTO> GetPageByNumber(Guid userId, Guid roomId, int pageNumber)
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

            var page = await _templatesCollection.Find(p => p.RoomId == roomId && p.PageNumber == pageNumber).FirstOrDefaultAsync();
            if (page == null)
            {
                return new PageDTO
                {
                    RoomId = roomId,
                    PageNumber = pageNumber,
                    Shapes = new List<object>()
                };
            }

            return new PageDTO
            {
                RoomId = roomId,
                PageNumber = pageNumber,
                Shapes = page.Shapes.Select(shape => BsonTypeMapper.MapToDotNetValue(shape)).ToList()
            };
        }

        public async Task<List<PageDTO>> GetPagesByRoomId(Guid userId, Guid roomId)
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

            var pages = await _templatesCollection
                .Find(p => p.RoomId == roomId)
                .SortBy(p => p.PageNumber)
                .ToListAsync();

            return pages.Select(p => new PageDTO
            {
                RoomId = p.RoomId,
                PageNumber = p.PageNumber,
                Shapes = p.Shapes.Select(shape => BsonTypeMapper.MapToDotNetValue(shape)).ToList()
            }).ToList();
        }
    }
}
