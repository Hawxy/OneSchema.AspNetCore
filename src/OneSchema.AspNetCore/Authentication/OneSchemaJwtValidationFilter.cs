using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Authentication;

/// <summary>
/// An <see cref="IEndpointFilter"/> that validates the <c>embed_user_jwt</c> field
/// on any request body implementing <see cref="IOneSchemaWebhookRequest"/>.
/// Returns <c>401 Unauthorized</c> if the token is missing or invalid.
/// </summary>
internal sealed class OneSchemaJwtValidationFilter : IEndpointFilter
{
    /// <summary>
    /// The key used to store the validated <see cref="System.Security.Claims.ClaimsPrincipal"/>
    /// in <see cref="HttpContext.Items"/>.
    /// </summary>
    internal const string ClaimsPrincipalKey = "OneSchema:ClaimsPrincipal";

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var options = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<OneSchemaJwtOptions>>().Value;

        // Find the IOneSchemaWebhookRequest argument from model binding
        var webhookRequest = FindWebhookRequest(context);
        if (webhookRequest is null)
        {
            return Results.Problem(
                detail: "No OneSchema webhook request found in endpoint arguments.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var token = webhookRequest.EmbedUserJwt;
        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(options.ClientSecret));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.ClientId,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = options.ClockSkew,
            IssuerSigningKey = key,
            ValidateIssuerSigningKey = true
        };

        var handler = new JsonWebTokenHandler();
        var result = await handler.ValidateTokenAsync(token, validationParameters);

        if (!result.IsValid)
        {
            return Results.Unauthorized();
        }

        webhookRequest.Identity = result.ClaimsIdentity;

        return await next(context);
    }

    private static IOneSchemaWebhookRequest? FindWebhookRequest(
        EndpointFilterInvocationContext context)
    {
        for (var i = 0; i < context.Arguments.Count; i++)
        {
            if (context.Arguments[i] is IOneSchemaWebhookRequest request)
                return request;
        }

        return null;
    }
}


