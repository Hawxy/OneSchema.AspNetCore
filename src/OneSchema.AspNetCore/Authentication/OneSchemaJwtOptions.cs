namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// Options for configuring OneSchema JWT validation on webhook endpoints.
/// </summary>
public class OneSchemaJwtOptions
{
    /// <summary>
    /// The OneSchema client id. Used to validate the <c>iss</c> (issuer) claim.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The OneSchema client secret. Used to verify the HMAC-SHA256 signature.
    /// </summary>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// The allowed clock skew when validating token expiration.
    /// Defaults to 5 minutes.
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);
}

