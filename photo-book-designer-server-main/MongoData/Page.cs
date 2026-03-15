using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace photo_book_designer_server_main.MongoData
{
    public class Page
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid RoomId { get; set; }
        public int PageNumber { get; set; }
        public string HtmlContent { get; set; }
        public List<BsonDocument> Shapes { get; set; }
    }
}
