# OneSchema.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/OneSchema.AspNetCore.svg?style=flat-square)](https://www.nuget.org/packages/OneSchema.AspNetCore)

ASP.NET Core integration for [OneSchema](https://www.oneschema.co/) validation
webhooks. Provides minimal-API endpoint mapping, strongly-typed row models, optional
JWT request validation, and helpers for batch lookups against your data store.

Requires .NET 10+

## Install

```sh
dotnet add package OneSchema.AspNetCore
```

## Quick Start

The minimal setup is outlined below:

```csharp
using OneSchema.AspNetCore.Authentication;
using OneSchema.AspNetCore.Validation;

var builder = WebApplication.CreateBuilder(args);

// 1. Register your handlers
builder.Services.AddValidationHookHandler<ContactValidationHandler, ContactRow>();
builder.Services.AddValidationHookHandler<ProductUniquenessHandler, ProductRow>();

// 2. (Optional) Configure JWT validation
builder.Services.AddOneSchemaJwtValidation(opts =>
{
    opts.ClientId = builder.Configuration["OneSchema:ClientId"]!;
    opts.ClientSecret = builder.Configuration["OneSchema:ClientSecret"]!;
});

var app = builder.Build();

// 3. Map endpoints (one per handler)
app.MapValidationHook<ContactValidationHandler, ContactRow>("/webhooks/validate-contacts")
    .RequireOneSchemaJwtValidation();

app.MapValidationHook<ProductUniquenessHandler, ProductRow>("/webhooks/validate-products");

// 4. Generate a JWT to send to OneSchema (optional, required if using JWT validation).
app.MapGet("/validation/jwt", (IOptions<OneSchemaJwtOptions> options, HttpContext httpContext) =>
{
    var optionsValue = options.Value;
    
    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    var additionalClaims = new Dictionary<string, object>()
        { { "org-id", httpContext.User.FindFirstValue("org-id")! } };
    
    var jwt = OneSchemaJwt.GenerateEmbedToken(optionsValue.ClientId, optionsValue.ClientSecret, userId, additionalClaims);

    return jwt;
});

app.Run();
```

Each handler is bound to its own URL. Configure that URL in the OneSchema dashboard
as a validation hook for the matching template.


## Handler Types

Handlers come in both dynamic and strongly typed flavors. The non-generic types expose a dictionary of values,
while the generic handlers types map directly to the template columns. Use whichever
is most convenient for your use case.

### Per-row Validation — `RowValidationHookHandler/RowValidationHookHandler<TRow>`

Use when each row is validated independently. The base class iterates rows for you;
report errors/warnings on the supplied context.

```csharp
public class ContactValidationHandler : RowValidationHookHandler<ContactRow>
{
    protected override ValueTask ValidateRowAsync(
        RowValidationContext<ContactRow> context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.Values.Email))
            context.Error("email", "Email is required.");
        else if (!context.Values.Email.Contains('@'))
            context.Warning("email", "Email appears invalid.")
                .WithSuggestion("user@example.com");

        return ValueTask.CompletedTask;
    }
}
```

### Batch Validation — `BatchValidationHookHandler/BatchValidationHookHandler<TRow>`

Use when validation requires looking across rows or batched for querying against an external system (such as a DB). You'll recieve every row in the webhook batch and a
`ValidationResultBuilder` to attach errors/warnings against specific rows.

```csharp
public class ProductUniquenessHandler : BatchValidationHookHandler<ProductRow>
{
    protected override Task ValidateAsync(
        ValidationHookRequest<ProductRow> request,
        ValidationResultBuilder results,
        CancellationToken cancellationToken)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in request.Rows)
        {
            if (string.IsNullOrWhiteSpace(row.Values.Sku))
            {
                results.ForRow(row).Error("sku", "SKU is required.");
                continue;
            }
            if (!seen.Add(row.Values.Sku))
                results.ForRow(row).Error("sku", "Duplicate SKU.");
        }
        return Task.CompletedTask;
    }
}
```

### Defining a Row Model

To define a strongly typed model, map your OneSchema template columns to a POCO using `JsonPropertyName`:

```csharp
public class ContactRow
{
    [JsonPropertyName("first_name")] public string? FirstName { get; set; }
    [JsonPropertyName("last_name")]  public string? LastName  { get; set; }
    [JsonPropertyName("email")]      public string? Email     { get; set; }
    [JsonPropertyName("phone")]      public string? Phone     { get; set; }
}
````

### Database Lookups - `RowIndex` helper

For bulk checks involving database queries, use `IndexBy` to build a key -> rows
index, issue a single batch query, then pass the matching values back to `ErrorForMatches` (or `WarningForMatches`).

```csharp
public class ContactEmailExistsHandler : BatchValidationHookHandler<ContactRow>
{
    protected override async Task ValidateAsync(
        ValidationHookRequest<ContactRow> request,
        ValidationResultBuilder results,
        CancellationToken cancellationToken)
    {
        var byEmail = request.Rows
            .Where(r => !string.IsNullOrWhiteSpace(r.Values.Email))
            .IndexBy(v => v.Email!, StringComparer.OrdinalIgnoreCase);

        var taken = await db.Users
            .Where(u => byEmail.Keys.Contains(u.Email))
            .Select(u => u.Email)
            .ToListAsync(cancellationToken);

        byEmail.ErrorForMatches(
            taken,
            results,
            column: "email",
            messageFactory: email => $"'{email}' is already registered.");
    }
}
```

## JWT Validation

OneSchema signs webhook requests with an `embed_user_jwt` field in the request body.
If present, the JWT claims are available on the webhook request via `context.Request.Identity`.

To enforce JWT validation:

1. Register your configuration with `AddOneSchemaJwtValidation`.
2. Chain `.RequireOneSchemaJwtValidation()` on each endpoint that should enforce it.

```csharp
builder.Services.AddOneSchemaJwtValidation(opts =>
{
    opts.ClientId = builder.Configuration["OneSchema:ClientId"]!;
    opts.ClientSecret = builder.Configuration["OneSchema:ClientSecret"]!;
});

app.MapValidationHook<ContactValidationHandler, ContactRow>("/webhooks/validate-contacts")
    .RequireOneSchemaJwtValidation();
```

3. Generate a JWT to use in your frontend via `OneSchemaJwt.GenerateEmbedToken`.

```csharp
app.MapGet("/validation/jwt", (OneSchemaJwtContext ctx, HttpContext httpContext) =>
{
    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    var additionalClaims = new Dictionary<string, object>()
        { { "org-id", httpContext.User.FindFirstValue("org-id")! } };
    
    return ctx.GenerateEmbedToken(userId, additionalClaims);
});

```

Endpoints without `RequireOneSchemaJwtValidation()` accept unsigned requests.
Invalid or missing tokens produce `401 Unauthorized`.

## Webhook Submission

The `ExportWebhookRequest` type is included for convenience, but no specific abstraction is provided for working with this type. This might change in the future.
