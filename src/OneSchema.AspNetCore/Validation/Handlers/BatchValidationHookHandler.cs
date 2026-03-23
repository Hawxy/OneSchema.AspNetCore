using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation.Handlers;

/// <summary>
/// Convenient base class for validation hooks that need access to all rows at once
/// (e.g. cross-row uniqueness checks, batch database lookups).
/// <para>
/// Example:
/// <code>
/// public class UniquenessValidator : BatchValidationHookHandler
/// {
///     protected override Task ValidateAsync(
///         ValidationHookRequest request,
///         ValidationResultBuilder results,
///         CancellationToken cancellationToken)
///     {
///         var seen = new HashSet&lt;string&gt;();
///         foreach (var row in request.Rows)
///         {
///             var code = row.GetValue("code");
///             if (code is not null &amp;&amp; !seen.Add(code))
///                 results.ForRow(row).Error("code", "Duplicate code");
///         }
///         return Task.CompletedTask;
///     }
/// }
/// </code>
/// </para>
/// </summary>
public abstract class BatchValidationHookHandler : IValidationHookHandler
{
    /// <inheritdoc />
    public async Task<ValidationHookResponseItem[]> HandleAsync(
        ValidationHookRequest request,
        CancellationToken cancellationToken = default)
    {
        var results = new ValidationResultBuilder();
        await ValidateAsync(request, results, cancellationToken);
        return results.Build();
    }

    /// <summary>
    /// Override to perform batch validation across all rows.
    /// Use the <paramref name="results"/> builder to report errors and warnings.
    /// </summary>
    protected abstract Task ValidateAsync(
        ValidationHookRequest request,
        ValidationResultBuilder results,
        CancellationToken cancellationToken);
}