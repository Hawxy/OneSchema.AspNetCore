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


