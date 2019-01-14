using iBoxDB.LocalServer;
using Markdig;
using MediatR;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;

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
        private readonly DB.AutoBox _db;

        public NewPostCommandInvoker(DB.AutoBox db)
        {
            _db = db;
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
                PubDate = cmd.PubDate.CloneToUtc(),
                Status = cmd.Published ? PublishStatus.Published : PublishStatus.Draft,
                Title = cmd.Title,
                TitleSlug = cmd.TitleSlug.IsNullOrWhitespace() ? cmd.Title.Trim().ToSlug() : cmd.TitleSlug.Trim().ToSlug(),
                DateUTC = DateTime.UtcNow
            };
            if (!cmd.Tags.IsNullOrWhitespace())
            {
                var tags = cmd.Tags.Trim().Split(',').Select(s => s.Trim());
                post.Tags = tags.Select(s => s.ToSlug()).ToArray();
                foreach (var tag in tags)
                {
                    var slug = tag.ToSlug();
                    var tagEntry = _db.SelectKey<Tag>(DBTableNames.Tags, slug);
                    if (tagEntry == null)
                    {
                        tagEntry = new Tag
                        {
                            Slug = slug,
                            Name = tag,
                            PostCount = 1
                        };
                        _db.Insert(DBTableNames.Tags, tagEntry);
                    }
                    else
                    {
                        tagEntry.PostCount++;
                        _db.Update(DBTableNames.Tags, tagEntry);
                    }
                }
            }
            else
                post.Tags = new string[] { };

            var result = _db.Insert(DBTableNames.BlogPosts, post);

            return CommandResult.SuccessResult;
        }
    }
}