using Shouldly;
using MediatR;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Entities;
using Xunit;
using System.Threading.Tasks;
using System.Threading;
using Dapper;
using Dapper.Extensions;
using System;

namespace MZBlog.Core.Tests.Accounts
{
    public class ChangePasswordCommandTests : SqliteBackedTest
    {
        private readonly string authorId = "mzyi";

        [Fact]
        public async Task Change_password_fail_if_old_password_does_not_match()
        {
            var conn = GetMemorySqliteConnection();
            await CreateAuthorTable(conn);

            var author = new Author()
            {
                Id = authorId,
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog"),
                DisplayName = "test",
                CreatedUTC = DateTime.UtcNow
            };
            conn.Insert(author);
            IRequestHandler<ChangePasswordCommand, CommandResult> handler = new ChangePasswordCommandInvoker(conn);
            var result = await handler.Handle(new ChangePasswordCommand()
            {
                AuthorId = author.Id,
                OldPassword = "wrong psw",
                NewPassword = "pswtest",
                NewPasswordConfirm = "pswtest"
            }, new CancellationToken());
            result.Success
                  .ShouldBeFalse();
        }

        [Fact]
        public async Task Change_password()
        {
            var conn = GetMemorySqliteConnection();
            await CreateAuthorTable(conn);
            var author = new Author()
            {
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog"),
                DisplayName = "test",
                CreatedUTC = DateTime.UtcNow
            };

            conn.Insert(author);

            IRequestHandler<ChangePasswordCommand, CommandResult> handler = new ChangePasswordCommandInvoker(conn);
            var result = await handler.Handle(new ChangePasswordCommand()
            {
                AuthorId = author.Id,
                OldPassword = "mzblog",
                NewPassword = "pswtest",
                NewPasswordConfirm = "pswtest"
            }, new CancellationToken());

            result.Success
                  .ShouldBeTrue();

            conn.Get<Author>(author.Id).HashedPassword
                                       .ShouldBe(Hasher.GetMd5Hash("pswtest"));
        }
    }
}