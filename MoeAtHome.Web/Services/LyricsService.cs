using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using MoeAtHome.Web.Models;
using MoeAtHome.Web.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MoeAtHome.Web.Services
{
    public class LyricsService
    {
        private const string DatabaseName = "TomatoMusicLyrics";
        private const string LyricsCollectionName = "Lyrics";
        private readonly MongoClient _mongo;
        private readonly IMongoDatabase _database;
        private readonly Lazy<Task<IMongoCollection<Lyric>>> _lyricsRepo;
        private Task<IMongoCollection<Lyric>> LyricsRepo => _lyricsRepo.Value;
        private static readonly TTPlayerLyricsService _ttLyrics = new TTPlayerLyricsService();
        private readonly LyricsStorageService _storage;

        public LyricsService(IOptions<LyricsServiceOptions> options, LyricsStorageService storage)
        {
            _storage = storage;
            _mongo = new MongoClient(options.Value.DbConnectionString);
            _database = _mongo.GetDatabase(DatabaseName);
            _lyricsRepo = new Lazy<Task<IMongoCollection<Lyric>>>(OnCreateLyricsRepository);
        }

        public Stream OpenRead(Lyric lyric)
        {
            return _storage.OpenRead(lyric.FileName);
        }

        public async Task<Lyric> Find(string title, string artist)
        {
            Lyric lyric;
            var repo = await LyricsRepo;
            if (string.IsNullOrEmpty(artist))
                lyric = await repo.FindOneAndUpdateAsync(o => o.Title == title, Builders<Lyric>.Update.Inc(o => o.AccessTimes, 1ul));
            else
                lyric = await repo.FindOneAndUpdateAsync(o => o.Title == title && o.Artist == artist, Builders<Lyric>.Update.Inc(o => o.AccessTimes, 1ul));
            if (lyric == null)
                lyric = await DownloadAndInsertLyric(title, artist);
            return lyric;
        }

        private async Task<Lyric> DownloadAndInsertLyric(string title, string artist)
        {
            var lrcInfo = (await _ttLyrics.GetLrcList(title, artist)).FirstOrDefault();
            if (lrcInfo != null)
            {
                var id = Guid.NewGuid();
                var lyric = new Lyric
                {
                    Id = id,
                    Title = title,
                    Artist = artist,
                    CreatedTime = DateTime.UtcNow,
                    AccessTimes = 1,
                    FileName = $"{id.ToString("N")}.lrc"
                };
                using (var stream = await _ttLyrics.DownloadLrc(lrcInfo))
                    await _storage.Save(stream, lyric.FileName);
                var repo = await LyricsRepo;
                await repo.InsertOneAsync(lyric);
                return lyric;
            }
            return null;
        }

        private async Task<IMongoCollection<Lyric>> OnCreateLyricsRepository()
        {
            var coll = _database.GetCollection<Lyric>(LyricsCollectionName);
            await coll.Indexes.CreateOneAsync(new IndexKeysDefinitionBuilder<Lyric>().Ascending(
                new StringFieldDefinition<Lyric>(nameof(Lyric.Title))).Ascending
                (new StringFieldDefinition<Lyric>(nameof(Lyric.Artist))).Ascending
                (new StringFieldDefinition<Lyric>(nameof(Lyric.Album))));
            return coll;
        }

        static LyricsService()
        {
            RegisterClassMaps();
        }

        private static void RegisterClassMaps()
        {
            BsonClassMap.RegisterClassMap<Lyric>();
        }
    }
}
