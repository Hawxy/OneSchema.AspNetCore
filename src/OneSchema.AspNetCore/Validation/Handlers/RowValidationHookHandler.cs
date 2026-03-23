using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation.Handlers;

/// <summary>
/// Convenient base class for validation hooks that validate one row at a time.
/// <para>
/// Example:
/// <code>
/// public class EmailValidator : RowValidationHookHandler
/// {
///     protected override ValueTask ValidateRowAsync(RowValidationContext context)
///     {
///         var email = context["email"];
///         if (string.IsNullOrWhiteSpace(email))
///             context.Error("email", "Email is required");
///         else if (!email.Contains('@'))
///             context.Warning("email", "Email looks invalid")
///                 .WithSuggestion("user@example.com");
///
///         return ValueTask.CompletedTask;
///     }
/// }
/// </code>
/// </para>
/// </summary>
public abstract class RowValidationHookHandler : IValidationHookHandler
{
    /// <inheritdoc />
    public async Task<ValidationHookResponseItem[]> HandleAsync(
        ValidationHookRequest request,
        CancellationToken cancellationToken = default)
    {
        var builder = new ValidationResultBuilder();

        foreach (var row in request.Rows)
        {
            var context = new RowValidationContext(row, request, builder);
            await ValidateRowAsync(context, cancellationToken);
        }

        return builder.Build();
    }

    /// <summary>
    /// Performs asynchronous per-row validation.
    /// </summary>
    protected abstract ValueTask ValidateRowAsync(
        RowValidationContext context,
        CancellationToken cancellationToken);

}