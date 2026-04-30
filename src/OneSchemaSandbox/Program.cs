using System.Security.Claims;
using Microsoft.Extensions.Options;
using OneSchema.AspNetCore.Authentication;
using OneSchema.AspNetCore.Validation;
using OneSchemaSandbox.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Register handlers
builder.Services.AddValidationHookHandler<ContactValidationHandler, ContactRow>();
builder.Services.AddValidationHookHandler<ProductUniquenessHandler, ProductRow>();
builder.Services.AddValidationHookHandler<ContactEmailExistsHandler, ContactRow>();

// Configure optional JWT validation
builder.Services.AddOneSchemaJwtValidation(opts =>
{
    opts.ClientId = builder.Configuration["OneSchema:ClientId"] ?? "test-client-id";
    opts.ClientSecret = builder.Configuration["OneSchema:ClientSecret"] ?? "super-secret-key-that-is-long-enough-for-hmac";
});

var app = builder.Build();

// Map validation hook endpoints
app.MapValidationHook<ContactValidationHandler, ContactRow>("/webhooks/validate-contacts")
    .RequireOneSchemaJwtValidation();

app.MapValidationHook<ProductUniquenessHandler, ProductRow>("/webhooks/validate-products");

app.MapValidationHook<ContactEmailExistsHandler, ContactRow>("/webhooks/validate-contacts-exists");

app.MapGet("/validation/jwt", (OneSchemaJwtContext ctx, HttpContext httpContext) =>
{
    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    var additionalClaims = new Dictionary<string, object>()
        { { "org-id", httpContext.User.FindFirstValue("org-id")! } };
    
    var jwt = ctx.GenerateEmbedToken(userId, additionalClaims);

    return jwt;
});

app.Run();

// Required for Alba / WebApplicationFactory test access
public partial class Program;
