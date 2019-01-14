using iBoxDB.LocalServer;
using Markdig;
using MediatR;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class EditPostCommand:IRequest<CommandResult>
    {
        public string PostId { get; set; }

        public string AuthorId { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

        public string MarkDown { get; set; }

        public string Tags { get; set; }

        public DateTime PubDate { get; set; }

        public bool Published { get; set; }
    }

    public class EditPostCommandInvoker : RequestHandler<EditPostCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public EditPostCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        protected override CommandResult Handle(EditPostCommand cmd)
        {
            var post = _db.SelectKey<BlogPost>(DBTableNames.BlogPosts, cmd.PostId);

            if (post == null)
                throw new ApplicationException("Post with id: {0} was not found".FormatWith(cmd.PostId));
            if (post.Tags != null)
            {
                foreach (var tag in post.Tags)
                {
                    var slug = tag.ToSlug();
                    var tagEntry = _db.SelectKey<Tag>(DBTableNames.Tags, slug);
                    if (tagEntry != null)
                    {
                        tagEntry.PostCount--;
                        _db.Update(DBTableNames.Tags, tagEntry);
                    }
                }
            }

            //TODO:应该验证TitleSlug是否是除了本文外唯一的
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            post.MarkDown = cmd.MarkDown;
            post.Content = Markdown.ToHtml(cmd.MarkDown, pipeline);
            post.PubDate = cmd.PubDate.CloneToUtc();
            post.Status = cmd.Published ? PublishStatus.Published : PublishStatus.Draft;
            post.Title = cmd.Title;
            post.TitleSlug = cmd.TitleSlug.Trim().ToSlug();
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
            _db.Update(DBTableNames.BlogPosts, post);

            return CommandResult.SuccessResult;
        }
    }
}