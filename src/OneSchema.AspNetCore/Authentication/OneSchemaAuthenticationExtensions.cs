using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// Extension methods for registering and applying OneSchema JWT validation.
/// </summary>
public static class OneSchemaAuthenticationExtensions
{
    /// <summary>
    /// Registers OneSchema JWT validation options in the DI container.
    /// Call <see cref="RequireOneSchemaJwtValidation"/> on individual endpoints to enforce validation.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services.AddOneSchemaJwtValidation(opts =>
    /// {
    ///     opts.ClientId = "my-client-id";
    ///     opts.ClientSecret = "my-secret";
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddOneSchemaJwtValidation(
        this IServiceCollection services,
        Action<OneSchemaJwtOptions> configure)
    {
        services.Configure(configure);
        return services;
    }

    /// <summary>
    /// Adds JWT validation to this endpoint. The filter extracts the <c>embed_user_jwt</c>
    /// from the request body and validates its signature, issuer, and expiration using
    /// the options registered via <see cref="AddOneSchemaJwtValidation"/>.
    /// Returns <c>401 Unauthorized</c> if the token is missing or invalid.
    /// </summary>
    /// <example>
    /// <code>
    /// app.MapValidationHook&lt;MyHandler&gt;("/webhooks/validate")
    ///    .RequireOneSchemaJwtValidation();
    /// </code>
    /// </example>
    public static RouteHandlerBuilder RequireOneSchemaJwtValidation(
        this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<OneSchemaJwtValidationFilter>();
    }
}


