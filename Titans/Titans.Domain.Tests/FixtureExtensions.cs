﻿namespace Titans.Domain.Tests;
using AutoFixture;
using Titans.Domain.User;

internal static class FixtureExtensions
{
    internal static IFixture RegisterDomainCreatorFunctions(this IFixture fixture)
    {
        fixture.Register(() => RefreshToken.Create(fixture.Create<string>(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7)));
        return fixture;
    }
}