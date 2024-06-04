using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BsonDocument>> GetDocumentsAsync(string collectionName)
        {
            return await _httpClient.GetFromJsonAsync<List<BsonDocument>>($"api/mongo/{collectionName}");
        }

        public async Task<BsonDocument> GetDocumentByIdAsync(string collectionName, string id)
        {
            return await _httpClient.GetFromJsonAsync<BsonDocument>($"api/mongo/{collectionName}/{id}");
        }

        public async Task InsertDocumentAsync(string collectionName, BsonDocument document)
        {
            await _httpClient.PostAsJsonAsync($"api/mongo/{collectionName}", document);
        }

        public async Task UpdateDocumentAsync(string collectionName, string id, BsonDocument updatedDocument)
        {
            await _httpClient.PutAsJsonAsync($"api/mongo/{collectionName}/{id}", updatedDocument);
        }

        public async Task DeleteDocumentAsync(string collectionName, string id)
        {
            await _httpClient.DeleteAsync($"api/mongo/{collectionName}/{id}");
        }
    }
}
