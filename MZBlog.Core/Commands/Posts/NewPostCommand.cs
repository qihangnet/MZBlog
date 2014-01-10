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
        private readonly MongoCollections _collections;

        public NewPostCommandInvoker(MongoCollections collections)
        {
            _collections = collections;
        }

        public CommandResult Execute(NewPostCommand command)
        {
            var markdown = new MarkdownSharp.Markdown();
            //TODO:应该验证TitleSlug是否唯一
            var post = new BlogPost
                           {
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
                post.Tags = command.Tags.Trim().Split(',')
                .Select(t => new Tag { Name = t.Trim(), Slug = t.Trim().ToSlug() })
                .ToArray();
            }
            else
                post.Tags = new Tag[] { };

            var result = _collections.BlogPostCollection.Insert(post);

            return CommandResult.SuccessResult;
        }
    }
}