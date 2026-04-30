using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// OneSchema JWT context for generating and validating embed tokens.
/// </summary>
public class OneSchemaJwtContext
{
    private readonly JsonWebTokenHandler _tokenHandler = new();
    private readonly TokenValidationParameters _validationParameters;
    
    private readonly string _clientId;
    private readonly SigningCredentials _credentials;

    public OneSchemaJwtContext(IOptions<OneSchemaJwtOptions> options)
    {
        _clientId = options.Value.ClientId;
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.ClientSecret));
        
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Value.ClientId,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = options.Value.ClockSkew,
            IssuerSigningKey = key,
            ValidateIssuerSigningKey = true
        };
        
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
    
    /// <summary>
    /// Generates a signed JWT used to embed a user in a OneSchema API request.
    /// </summary>
    /// <param name="userId">The user identifier to embed in the <c>user_id</c> claim.</param>
    /// <param name="additionalClaims">Additional claims </param>
    /// <returns>A compact-serialized JWT string.</returns>
    public string GenerateEmbedToken(string userId, IDictionary<string, object>? additionalClaims = null)
    {
        var claims = additionalClaims ?? new Dictionary<string, object>();
        
        claims.Add("user_id", userId);
        
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _clientId,
            Claims = claims,
            SigningCredentials = _credentials,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(6)
        };
        
        return _tokenHandler.CreateToken(descriptor);
    }

    /// <summary>
    /// Validates a signed JWT and returns the result.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        return await _tokenHandler.ValidateTokenAsync(token, _validationParameters);
    }
    
}