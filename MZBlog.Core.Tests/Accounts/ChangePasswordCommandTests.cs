using FluentAssertions;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class ChangePasswordCommandTests : MongoDBBackedTest
    {
        [Fact]
        public void change_password_fail_if_old_password_does_not_match()
        {
            var author = new Author()
            {
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog")
            };
            Collections.AuthorCollection.Save(author);
            new ChangePasswordCommandInvoker(Collections)
               .Execute(new ChangePasswordCommand()
               {
                   AuthorId = author.Id,
                   OldPassword = "wrong psw",
                   NewPassword = "pswtest",
                   NewPasswordConfirm = "pswtest"
               })
               .Success.Should().BeFalse();
        }

        [Fact]
        public void change_password()
        {
            var author = new Author()
            {
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog")
            };

            Collections.AuthorCollection.Save(author);

            new ChangePasswordCommandInvoker(Collections)
                .Execute(new ChangePasswordCommand()
                {
                    AuthorId = author.Id,
                    OldPassword = "mzblog",
                    NewPassword = "pswtest",
                    NewPasswordConfirm = "pswtest"
                })
                .Success.Should().BeTrue();

            Collections.AuthorCollection.FindOneById(author.Id.ToObjectId()).HashedPassword.Should().BeEquivalentTo(Hasher.GetMd5Hash("pswtest"));
        }
    }
}