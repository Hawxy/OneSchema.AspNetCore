using OneSchema.AspNetCore.Authentication;
using OneSchema.AspNetCore.Validation;
using OneSchemaSandbox.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Register handlers
builder.Services.AddValidationHookHandler<ContactValidationHandler, ContactRow>();
builder.Services.AddValidationHookHandler<ProductUniquenessHandler, ProductRow>();

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

app.Run();

// Required for Alba / WebApplicationFactory test access
public partial class Program;
