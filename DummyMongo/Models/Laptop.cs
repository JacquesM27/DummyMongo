using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace DummyMongo.Models
{
    public class Laptop
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)] 
        public string? Id { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }

        [BsonElement("inputs")]
        [JsonPropertyName("inputs")]
        public List<string> Inputs { get; set; } = null!;
    }
}
