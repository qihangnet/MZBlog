using FluentAssertions;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class LoginCommandTests : MongoDBBackedTest
    {
        [Fact]
        public void login_should_success_if_user_in_database()
        {
            Collections.AuthorCollection
                    .Insert(new Author()
                    {
                        Email = "test@mz.yi",
                        HashedPassword = Hasher.GetMd5Hash("test")
                    });

            var loginCommandInvoker = new LoginCommandInvoker(Collections);

            loginCommandInvoker.Execute(new LoginCommand
            {
                Email = "test@mz.yi",
                Password = "test"
            }).Success.Should().BeTrue();
        }

        [Fact]
        public void login_should_fail_if_invalid_password_provided()
        {
            var documtnt = new Author()
            {
                Email = "username@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("psw1")
            };

            Collections.AuthorCollection.Insert(documtnt);

            var loginCommandInvoker = new LoginCommandInvoker(Collections);

            loginCommandInvoker.Execute(new LoginCommand()
            {
                Email = "username@mz.yi",
                Password = "psw2"
            }).Success.Should().BeFalse();
        }
    }
}