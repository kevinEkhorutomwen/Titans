namespace Titans.Domain.Tests;

public static class ValidationExceptionExtensions
{
    public static string GetDomainValidationErrorText(string property, string message)
    {
        return $"""
                Validation failed: 
                 -- {property}: {message} Severity: Error
                """;
    }
}