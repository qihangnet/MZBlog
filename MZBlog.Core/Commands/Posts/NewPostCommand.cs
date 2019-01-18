using Microsoft.Data.Sqlite;
using Markdig;
using MediatR;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.Commands.Posts
{
    public class NewPostCommand : IRequest<CommandResult>
    {
        public Author Author { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

        public string MarkDown { get; set; }

        public string Tags { get; set; }

        public DateTime PubDate { get; set; }

        public bool Published { get; set; }
    }

    public class NewPostCommandInvoker : RequestHandler<NewPostCommand, CommandResult>
    {
        private readonly SqliteConnection _conn;

        public NewPostCommandInvoker(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override CommandResult Handle(NewPostCommand cmd)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            //TODO:应该验证TitleSlug是否唯一
            var post = new BlogPost
            {
                Id = ObjectId.NewId(),
                AuthorEmail = cmd.Author.Email,
                AuthorDisplayName = cmd.Author.DisplayName,
                MarkDown = cmd.MarkDown,
                Content = Markdown.ToHtml(cmd.MarkDown, pipeline),
                PublishUTC = cmd.PubDate.CloneToUtc(),
                Status = cmd.Published ? PublishStatus.Published : PublishStatus.Draft,
                Title = cmd.Title,
                TitleSlug = cmd.TitleSlug.IsNullOrWhitespace() ? cmd.Title.Trim().ToSlug() : cmd.TitleSlug.Trim().ToSlug(),
                CreatedUTC = DateTime.UtcNow
            };
            if (!cmd.Tags.IsNullOrWhitespace())
            {
                var tags = cmd.Tags.Trim().Split(',').Select(s => s.Trim());
                post.Tags = tags.Select(s => s.ToSlug()).ToArray();
                foreach (var tag in tags)
                {
                    var slug = tag.ToSlug();
                    var tagEntry = _conn.Get<Tag>(slug);
                    if (tagEntry == null)
                    {
                        tagEntry = new Tag
                        {
                            Slug = slug,
                            Name = tag,
                            PostCount = 1
                        };
                        _conn.Insert(tagEntry);
                    }
                    else
                    {
                        tagEntry.PostCount++;
                        _conn.Update(tagEntry);
                    }
                }
            }
            else
                post.Tags = new string[] { };

            var result = _conn.Insert(post);
            foreach (var t in post.Tags)
            {
                _conn.Insert(new BlogPostTags
                {
                    BlogPostId = post.Id,
                    TagSlug = t
                });
            }
            return CommandResult.SuccessResult;
        }
    }
}