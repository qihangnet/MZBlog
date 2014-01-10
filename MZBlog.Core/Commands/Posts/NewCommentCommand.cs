using MongoDB.Bson;
using MZBlog.Core.Documents;
using System;

namespace MZBlog.Core.Commands.Posts
{
    public class NewCommentCommand
    {
        public NewCommentCommand()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public string Id { get; set; }

        public SpamShield SpamShield { get; set; }

        public string PostId { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string SiteUrl { get; set; }

        public string Content { get; set; }

        public string IPAddress { get; set; }
    }

    public class NewCommentCommandInvoker : ICommandInvoker<NewCommentCommand, CommandResult>
    {
        private readonly MongoCollections _collections;
        private readonly ISpamShieldService _spamShield;

        public NewCommentCommandInvoker(MongoCollections collections, ISpamShieldService spamShield)
        {
            _collections = collections;
            _spamShield = spamShield;
        }

        public CommandResult Execute(NewCommentCommand command)
        {
            if (_spamShield.IsSpam(command.SpamShield))
            {
                return new CommandResult("You are a spam!");
            }

            var comment = new BlogComment
            {
                Id = command.Id,
                Email = command.Email,
                NickName = command.NickName,
                Content = command.Content,
                IPAddress = command.IPAddress,
                PostId = command.PostId,
                SiteUrl = command.SiteUrl,
                CreatedTime = DateTime.UtcNow
            };

            var result = _collections.BlogCommentCollection.Insert(comment);

            return CommandResult.SuccessResult;
        }
    }
}