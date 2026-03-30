using System.Security.Claims;

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

    /// <summary>
    /// The <see cref="ClaimsIdentity"/> produced by validating <see cref="EmbedUserJwt"/>,
    /// or <c>null</c> if JWT validation was not applied or the token was not validated.
    /// <para>
    /// This property is automatically populated by the JWT validation filter when
    /// <c>RequireOneSchemaJwtValidation()</c> is applied to the endpoint.
    /// </para>
    /// </summary>
    ClaimsIdentity? Identity { get; set; }
}

