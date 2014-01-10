using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;

namespace MZBlog.Core
{
    public class SpamShield
    {
        public string Tick { get; set; }

        public string Hash { get; set; }
    }

    public interface ISpamShieldService
    {
        string CreateTick(string key);

        string GenerateHash(string tick);

        bool IsSpam(SpamShield command);
    }

    public class SpamShieldService : ISpamShieldService
    {
        private readonly MongoCollection<SpamHash> _spamCol;

        public SpamShieldService(MongoCollections cols)
        {
            _spamCol = cols.SpamHashCollection;
        }

        public string CreateTick(string key)
        {
            var tick = ObjectId.GenerateNewId().ToString();
            var spamHash = new SpamHash
            {
                Id = tick,
                PostKey = key,
                CreatedTime = DateTime.UtcNow
            };
            _spamCol.Insert(spamHash);
            return tick;
        }

        public string GenerateHash(string tick)
        {
            var nonhash = string.Empty;
            if (tick.IsNullOrWhitespace())
                return nonhash;

            var spamHash = _spamCol.AsQueryable().FirstOrDefault(w => w.Id == tick);

            if (spamHash == null || spamHash.Pass || !spamHash.Hash.IsNullOrWhitespace())
                return nonhash;

            spamHash.Hash = new Random().NextDouble().ToString();
            _spamCol.Save(spamHash);

            return spamHash.Hash;
        }

        public bool IsSpam(SpamShield command)
        {
            if (command.Tick.IsNullOrWhitespace() || command.Hash.IsNullOrWhitespace())
                return true;

            var spamHash = _spamCol.AsQueryable().FirstOrDefault(w => w.Id == command.Tick);

            if (spamHash == null || spamHash.Pass || spamHash.Hash != command.Hash)
                return true;

            spamHash.Pass = true;
            _spamCol.Save(spamHash);
            return false;
        }
    }
}