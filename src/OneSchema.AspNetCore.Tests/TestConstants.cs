using OneSchema.AspNetCore.Authentication;

namespace OneSchema.AspNetCore.Tests;

/// <summary>
/// Shared test constants and helpers.
/// </summary>
internal static class TestConstants
{
    public const string ClientId = "test-client-id";
    public const string ClientSecret = "super-secret-key-that-is-long-enough-for-hmac";

    public static string GenerateValidJwt(string userId = "user-123")
        => JwtHelper.GenerateEmbedToken(ClientId, ClientSecret, userId);

    public static string GenerateJwtWithWrongSecret(string userId = "user-123")
        => JwtHelper.GenerateEmbedToken(ClientId, "wrong-secret-key-that-is-long-enough-for-hmac", userId);
}

