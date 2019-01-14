using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class LoginCommandTests : iBoxDBBackedTest
    {
        [Fact]
        public async Task login_should_success_if_user_in_database()
        {
            var db = OpenTestDb();
            db.Insert(DBTableNames.Authors, new Author()
            {
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("test")
            });

            IRequestHandler<LoginCommand, LoginCommandResult> loginCommandInvoker = new LoginCommandInvoker(db);

            var result = await loginCommandInvoker.Handle(new LoginCommand
            {
                Email = "test@mz.yi",
                Password = "test"
            }, new CancellationToken());

            result.Success.ShouldBeTrue();
        }

        [Fact]
        public async Task login_should_fail_if_invalid_password_provided()
        {
            var db = OpenTestDb();
            var document = new Author()
            {
                Email = "username@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("psw1")
            };

            db.Insert(DBTableNames.Authors, document);

            IRequestHandler<LoginCommand, LoginCommandResult> loginCommandInvoker = new LoginCommandInvoker(db);

            var result = await loginCommandInvoker.Handle(new LoginCommand()
            {
                Email = "username@mz.yi",
                Password = "psw2"
            }, new CancellationToken());

            result.Success.ShouldBeFalse();
        }
    }
}