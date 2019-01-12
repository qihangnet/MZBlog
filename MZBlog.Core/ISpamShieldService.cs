using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace MZBlog.Core
{
    public class SpamShield
    {
        [Required]
        public string Tick { get; set; }

        [Required]
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
        private readonly DB.AutoBox _db;

        public SpamShieldService(DB.AutoBox db)
        {
            _db = db;
        }

        public string CreateTick(string key)
        {
            var tick = ObjectId.NewId().ToString();
            var spamHash = new SpamHash
            {
                Id = tick,
                PostKey = key,
                CreatedTime = DateTime.UtcNow
            };
            _db.Insert(DBTableNames.SpamHashes, spamHash);
            return tick;
        }

        public string GenerateHash(string tick)
        {
            var nonhash = string.Empty;
            if (tick.IsNullOrWhitespace())
                return nonhash;

            var spamHash = _db.SelectKey<SpamHash>(DBTableNames.SpamHashes, tick);

            if (spamHash == null || spamHash.Pass || !spamHash.Hash.IsNullOrWhitespace())
                return nonhash;

            spamHash.Hash = new Random().NextDouble().ToString();
            _db.Update(DBTableNames.SpamHashes, spamHash);

            return spamHash.Hash;
        }

        public bool IsSpam(SpamShield command)
        {
            if (command.Tick.IsNullOrWhitespace() || command.Hash.IsNullOrWhitespace())
                return true;

            var spamHash = _db.SelectKey<SpamHash>(DBTableNames.SpamHashes, command.Tick);

            if (spamHash == null || spamHash.Pass || spamHash.Hash != command.Hash)
                return true;

            spamHash.Pass = true;
            _db.Update(DBTableNames.SpamHashes, spamHash);
            return false;
        }
    }
}