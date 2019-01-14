using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MZBlog.Core.Commands.Posts
{
    public class NewCommentCommand : IRequest<CommandResult>
    {
        public NewCommentCommand()
        {
            Id = ObjectId.NewId();
        }

        public string Id { get; set; }

        public SpamShield SpamShield { get; set; }

        public string PostId { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string SiteUrl { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }

        public string IPAddress { get; set; }
    }

    public class NewCommentCommandInvoker : RequestHandler<NewCommentCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;
        private readonly ISpamShieldService _spamShield;

        public NewCommentCommandInvoker(DB.AutoBox db, ISpamShieldService spamShield)
        {
            _db = db;
            _spamShield = spamShield;
        }

        protected override CommandResult Handle(NewCommentCommand cmd)
        {
            if (Regex.IsMatch(cmd.Email, @"smith\w\d*@gmail.com") || _spamShield.IsSpam(cmd.SpamShield))
            {
                return new CommandResult("You are a spam!");
            }

            var comment = new BlogComment
            {
                Id = cmd.Id,
                Email = cmd.Email,
                NickName = cmd.NickName,
                Content = cmd.Content,
                IPAddress = cmd.IPAddress,
                PostId = cmd.PostId,
                SiteUrl = cmd.SiteUrl,
                CreatedTime = DateTime.UtcNow
            };

            var result = _db.Insert(DBTableNames.BlogComments, comment);

            return CommandResult.SuccessResult;
        }
    }
}