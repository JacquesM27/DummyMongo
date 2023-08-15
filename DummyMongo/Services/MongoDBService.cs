using DummyMongo.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DummyMongo.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Laptop> _laptopCollection;

        public MongoDBService(IOptions<MongoDBSettings> settings)
        {
            MongoClient client = new(settings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
            _laptopCollection = database.GetCollection<Laptop>(settings.Value.CollectionName);
        }

        public async Task<List<Laptop>> GetAsync() 
        { 
            return await _laptopCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<string> CreateAsync(Laptop laptop) 
        {
            await _laptopCollection.InsertOneAsync(laptop);
            return laptop.Id!;
        }

        public async Task AddToInputAsync(string id, string input) 
        {
            FilterDefinition<Laptop> filter = Builders<Laptop>.Filter.Eq("Id", id);
            UpdateDefinition<Laptop> update = Builders<Laptop>.Update.AddToSet("inputs", input);
            await _laptopCollection.UpdateOneAsync(filter, update);
            return;
        }
        public async Task DeleteAsync(string id) 
        {
            FilterDefinition<Laptop> filter = Builders<Laptop>.Filter.Eq("Id", id);
            await _laptopCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
