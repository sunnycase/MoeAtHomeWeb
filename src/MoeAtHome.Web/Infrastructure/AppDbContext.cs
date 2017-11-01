using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MoeAtHome.Web.Infrastructure
{
    public class AppDbContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public IMongoCollection<Friend> Friends => GetCollection<Friend>("friends");

        public static bool _dummy;

        public static bool _initBsonClassMap = false;
        private static object _syncLock;

        public AppDbContext(IOptions<AppSecretConfiguration> options)
        {
            LazyInitializer.EnsureInitialized(ref _dummy, ref _initBsonClassMap, ref _syncLock, CreateModel);

            _client = new MongoClient(options.Value.DbConnectionString);
            _database = _client.GetDatabase("moeathome");
        }

        public IMongoCollection<T> GetCollection<T>(string name) =>
            _database.GetCollection<T>(name);

        private static bool CreateModel()
        {
            return true;
        }
    }
}
