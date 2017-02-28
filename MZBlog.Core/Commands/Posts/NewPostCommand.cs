using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class NewPostCommand
    {
        public Author Author { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

        public string MarkDown { get; set; }

        public string Tags { get; set; }

        public DateTime PubDate { get; set; }

        public bool Published { get; set; }
    }

    public class NewPostCommandInvoker : ICommandInvoker<NewPostCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public NewPostCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        public CommandResult Execute(NewPostCommand command)
        {
            var markdown = new MarkdownSharp.Markdown();
            //TODO:应该验证TitleSlug是否唯一
            var post = new BlogPost
            {
                Id = ObjectId.NewObjectId(),
                AuthorEmail = command.Author.Email,
                AuthorDisplayName = command.Author.DisplayName,
                MarkDown = command.MarkDown,
                Content = markdown.Transform(command.MarkDown),
                PubDate = command.PubDate.CloneToUtc(),
                Status = command.Published ? PublishStatus.Published : PublishStatus.Draft,
                Title = command.Title,
                TitleSlug = command.TitleSlug.IsNullOrWhitespace() ? command.Title.Trim().ToSlug() : command.TitleSlug.Trim().ToSlug(),
                DateUTC = DateTime.UtcNow
            };
            if (!command.Tags.IsNullOrWhitespace())
            {
                var tags = command.Tags.Trim().Split(',').Select(s => s.Trim());
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