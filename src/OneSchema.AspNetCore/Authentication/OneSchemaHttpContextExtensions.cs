using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// Extension methods for accessing the OneSchema-validated identity from <see cref="HttpContext"/>.
/// </summary>
public static class OneSchemaHttpContextExtensions
{
    /// <summary>
    /// Returns the <see cref="ClaimsIdentity"/> from the validated <c>embed_user_jwt</c>,
    /// or <c>null</c> if JWT validation was not applied or the token was not validated.
    /// </summary>
    public static ClaimsIdentity? GetOneSchemaIdentity(this HttpContext httpContext)
    {
        return httpContext.Items.TryGetValue(
            OneSchemaJwtValidationFilter.ClaimsPrincipalKey, out var value)
            ? value as ClaimsIdentity
            : null;
    }
}

