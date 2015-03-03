using FluentAssertions;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class LoginCommandTests : iBoxDBBackedTest
    {
        [Fact]
        public void login_should_success_if_user_in_database()
        {
            _db.Insert(DBTableNames.Authors, new Author()
                    {
                        Email = "test@mz.yi",
                        HashedPassword = Hasher.GetMd5Hash("test")
                    });

            var loginCommandInvoker = new LoginCommandInvoker(_db);

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

            _db.Insert(DBTableNames.Authors, documtnt);

            var loginCommandInvoker = new LoginCommandInvoker(_db);

            loginCommandInvoker.Execute(new LoginCommand()
            {
                Email = "username@mz.yi",
                Password = "psw2"
            }).Success.Should().BeFalse();
        }
    }
}