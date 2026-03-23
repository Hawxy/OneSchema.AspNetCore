using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// Helper class for generating JWTs for OneSchema embed sessions.
/// </summary>
public static class JwtHelper
{
    /// <summary>
    /// Generates a signed JWT containing an <c>iss</c> claim (client id) and a <c>user_id</c> claim.
    /// </summary>
    /// <param name="clientId">The OneSchema client id, used as the <c>iss</c> (issuer) claim.</param>
    /// <param name="userId">The user identifier to embed in the <c>user_id</c> claim.</param>
    /// <param name="clientSecret">The secret used to sign the token (HMAC-SHA256).</param>
    /// <param name="additionalClaims"></param>
    /// <returns>A compact-serialized JWT string.</returns>
    public static string GenerateEmbedToken(string clientId, string clientSecret, string userId, IDictionary<string, object>? additionalClaims = null)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(clientSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = additionalClaims ?? new Dictionary<string, object>();
        
        claims.Add("user_id", userId);
        
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = clientId,
            Claims = claims,
            SigningCredentials = credentials,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(6)
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}


