using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class LocalDbService
    {
        private readonly string _rootFolderPath;

        public LocalDbService(string rootFolderPath)
        {
            _rootFolderPath = rootFolderPath;
            if (!Directory.Exists(_rootFolderPath))
            {
                Directory.CreateDirectory(_rootFolderPath);
            }
        }

        private string GetCollectionPath(string collectionName)
        {
            return Path.Combine(_rootFolderPath, $"{collectionName}.json");
        }

        public async Task<List<T>> GetCollectionAsync<T>(string collectionName)
        {
            var filePath = GetCollectionPath(collectionName);
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public async Task SaveCollectionAsync<T>(string collectionName, List<T> collection)
        {
            var filePath = GetCollectionPath(collectionName);
            var json = JsonSerializer.Serialize(collection);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task AddRecordAsync<T>(string collectionName, T record)
        {
            var collection = await GetCollectionAsync<T>(collectionName);
            collection.Add(record);
            await SaveCollectionAsync(collectionName, collection);
        }

        public async Task UpdateRecordAsync<T>(string collectionName, Func<T, bool> predicate, T updatedRecord)
        {
            var collection = await GetCollectionAsync<T>(collectionName);
            var index = collection.FindIndex(new Predicate<T>(predicate));
            if (index >= 0)
            {
                collection[index] = updatedRecord;
                await SaveCollectionAsync(collectionName, collection);
            }
        }

        public async Task DeleteRecordAsync<T>(string collectionName, Func<T, bool> predicate)
        {
            var collection = await GetCollectionAsync<T>(collectionName);
            var record = collection.FirstOrDefault(predicate);
            if (record != null)
            {
                collection.Remove(record);
                await SaveCollectionAsync(collectionName, collection);
            }
        }
    }
}
