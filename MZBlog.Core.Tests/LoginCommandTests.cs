﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Extensions;
using MediatR;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Entities;
using FluentAssertions;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class LoginCommandTests : SqliteBackedTest
    {
        [Fact]
        public async Task Login_should_success_if_user_in_database()
        {
            var conn = GetMemorySqliteConnection();
            (await CreateAuthorTable(conn)).Should().Be(1);

            conn.Insert(new Author()
            {
                Email = "test@mz.yi",
                DisplayName = "yimingzhi",
                HashedPassword = Hasher.GetMd5Hash("test"),
                CreatedUTC = DateTime.UtcNow
            });

            IRequestHandler<LoginCommand, LoginCommandResult> loginCommandInvoker = new LoginCommandInvoker(conn);

            var result = await loginCommandInvoker.Handle(new LoginCommand
            {
                Email = "test@mz.yi",
                Password = "test"
            }, new CancellationToken());

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Login_should_fail_if_invalid_password_provided()
        {
            var conn = GetMemorySqliteConnection();
            await CreateAuthorTable(conn);

            var document = new Author()
            {
                Email = "username@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("psw1"),
                DisplayName = "username",
                CreatedUTC = DateTime.UtcNow
            };

            conn.Insert(document);

            IRequestHandler<LoginCommand, LoginCommandResult> loginCommandInvoker = new LoginCommandInvoker(conn);

            var result = await loginCommandInvoker.Handle(new LoginCommand()
            {
                Email = "username@mz.yi",
                Password = "psw2"
            }, new CancellationToken());

            result.Success.Should().BeFalse();
        }
    }
}