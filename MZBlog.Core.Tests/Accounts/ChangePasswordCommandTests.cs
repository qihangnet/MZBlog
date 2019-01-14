using FluentAssertions;
using MediatR;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using Xunit;
using System.Threading.Tasks;
using System.Threading;

namespace MZBlog.Core.Tests.Accounts
{
    public class ChangePasswordCommandTests : iBoxDBBackedTest
    {
        private string authorId = "mzyi";

        [Fact]
        public async Task change_password_fail_if_old_password_does_not_match()
        {
            var db = OpenTestDb();
            var author = new Author()
            {
                Id = authorId,
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog")
            };
            db.Insert(DBTableNames.Authors, author);
            IRequestHandler<ChangePasswordCommand, CommandResult> handler = new ChangePasswordCommandInvoker(db);
            var result = await handler.Handle(new ChangePasswordCommand()
            {
                AuthorId = author.Id,
                OldPassword = "wrong psw",
                NewPassword = "pswtest",
                NewPasswordConfirm = "pswtest"
            }, new CancellationToken());
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task change_password()
        {
            var db = OpenTestDb();

            var author = new Author()
            {
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog")
            };

            db.Insert(DBTableNames.Authors, author);

            IRequestHandler<ChangePasswordCommand, CommandResult> handler = new ChangePasswordCommandInvoker(db);
            var result = await handler.Handle(new ChangePasswordCommand()
            {
                AuthorId = author.Id,
                OldPassword = "mzblog",
                NewPassword = "pswtest",
                NewPasswordConfirm = "pswtest"
            }, new CancellationToken());

            result.Success.Should().BeTrue();

            db.SelectKey<Author>(DBTableNames.Authors, author.Id).HashedPassword
                .Should().BeEquivalentTo(Hasher.GetMd5Hash("pswtest"));
        }

        // ~ChangePasswordCommandTests()
        // {
        //     _db.Delete(DBTableNames.Authors, authorId);
        // }
    }
}