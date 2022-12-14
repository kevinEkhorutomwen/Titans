namespace Titans.Application.Tests;
using AutoFixture;
using Titans.Domain.User;

public class UserFixtures
{
    readonly static IFixture _fixture = new Fixture();
    public static User Create(string? username = null, byte[]? passwordHash = null, byte[]? passwordSalt = null)
    {
        username ??= _fixture.Create<string>();
        passwordHash ??= _fixture.Create<byte[]>();
        passwordSalt ??= _fixture.Create<byte[]>();

        var user = User.Create(username, passwordHash, passwordSalt);
        return user;
    }
}