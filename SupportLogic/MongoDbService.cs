using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class MongoDbService
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = "mongodb://superAccess:a1tpTEPw9yj2AahZ@ac-m3dwvk5-shard-00-00.uzcy1a7.mongodb.net:27017,ac-m3dwvk5-shard-00-01.uzcy1a7.mongodb.net:27017,ac-m3dwvk5-shard-00-02.uzcy1a7.mongodb.net:27017/?ssl=true&replicaSet=atlas-nwxpnh-shard-0&authSource=admin&retryWrites=true&w=majority&appName=PythoPlusEduClaster";
            var databaseName = "PythoPlus";

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<List<BsonDocument>> GetDocumentsAsync(string collectionName)
        {
            var collection = GetCollection(collectionName);
            return await collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<BsonDocument> GetDocumentByIdAsync(string collectionName, string id)
        {
            var collection = GetCollection(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertDocumentAsync(string collectionName, BsonDocument document)
        {
            var collection = GetCollection(collectionName);
            await collection.InsertOneAsync(document);
        }

        public async Task UpdateDocumentAsync(string collectionName, string id, BsonDocument updatedDocument)
        {
            var collection = GetCollection(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
            await collection.ReplaceOneAsync(filter, updatedDocument);
        }

        public async Task DeleteDocumentAsync(string collectionName, string id)
        {
            var collection = GetCollection(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
            await collection.DeleteOneAsync(filter);
        }
    }
}
