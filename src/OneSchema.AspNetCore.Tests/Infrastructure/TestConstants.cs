using Microsoft.Extensions.Options;
using OneSchema.AspNetCore.Authentication;

namespace OneSchema.AspNetCore.Tests.Infrastructure;

/// <summary>
/// Shared test constants and helpers.
/// </summary>
internal static class TestConstants
{
    private const string ClientId = "test-client-id";
    private const string ClientSecret = "super-secret-key-that-is-long-enough-for-hmac";

    private static readonly OneSchemaJwtContext ValidContext = new OneSchemaJwtContext(
        new OptionsWrapper<OneSchemaJwtOptions>(new OneSchemaJwtOptions()
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        }));
    
    private static readonly OneSchemaJwtContext InvalidContext = new OneSchemaJwtContext(
        new OptionsWrapper<OneSchemaJwtOptions>(new OneSchemaJwtOptions()
        {
            ClientId = ClientId,
            ClientSecret = "wrong-secret-key-that-is-long-enough-for-hmac"
        }));
    
    public static string GenerateValidJwt(string userId = "user-123")
        => ValidContext.GenerateEmbedToken(userId);

    public static string GenerateJwtWithWrongSecret(string userId = "user-123")
        => InvalidContext.GenerateEmbedToken(userId);
}

