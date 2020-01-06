﻿using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using MZBlog.Core.Entities;
using System.Collections.Generic;

namespace MZBlog.Core.Queries.Home
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
            var tags = _conn.Query<Tag>("SELECT * FROM Tag ORDER BY PostCount DESC");

            return new TagCloudViewModel
            {
                Tags = tags
            };
        }
    }
}