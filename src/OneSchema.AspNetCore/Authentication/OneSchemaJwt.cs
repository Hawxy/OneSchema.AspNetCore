/*
   Copyright 2026 Hawxy (JT)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// Helper class for generating JWTs for OneSchema embed sessions.
/// </summary>
public static class OneSchemaJwt
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


