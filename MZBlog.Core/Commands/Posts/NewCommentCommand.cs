using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Dapper.Extensions;

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
        private readonly SqliteConnection _conn;
        private readonly ISpamShieldService _spamShield;

        public NewCommentCommandInvoker(SqliteConnection conn, ISpamShieldService spamShield)
        {
            _conn = conn;
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

            var result = _conn.Insert(comment);

            return CommandResult.SuccessResult;
        }
    }
}