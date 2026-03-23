namespace OneSchema.AspNetCore.Api;

/// <summary>
/// Common interface for OneSchema webhook request types that carry an embedded user JWT.
/// Used by the JWT validation filter to extract the token from the request body.
/// </summary>
public interface IOneSchemaWebhookRequest
{
    /// <summary>
    /// The JWT token used to initialize the customer's embedded session.
    /// </summary>
    string EmbedUserJwt { get; }
}

