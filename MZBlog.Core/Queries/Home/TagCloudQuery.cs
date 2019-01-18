using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using Dapper;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagCloudQuery : IRequest<TagCloudViewModel>
    {
        public int Threshold { get; set; }

        public int Take { get; set; }

        public TagCloudQuery()
        {
            Threshold = 1;
            Take = int.MaxValue;
        }
    }

    public class TagCloudViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class TagCloudViewProjection : RequestHandler<TagCloudQuery, TagCloudViewModel>
    {
        private readonly SqliteConnection _conn;

        public TagCloudViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override TagCloudViewModel Handle(TagCloudQuery request)
        {
            var result = new Dictionary<Tag, int>();
            var tags = _conn.Query<Tag>("select * from Tag order by PostCount desc");

            return new TagCloudViewModel
            {
                Tags = tags
            };
        }
    }
}