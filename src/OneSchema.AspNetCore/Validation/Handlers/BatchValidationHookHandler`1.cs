using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation.Handlers;

/// <summary>
/// Convenient base class for validation hooks that need access to all strongly-typed rows
/// at once (e.g. cross-row uniqueness checks, batch database lookups).
/// <para>
/// Example:
/// <code>
/// public class ProductRow
/// {
///     [JsonPropertyName("code")]
///     public string Code { get; set; }
/// }
///
/// public class UniquenessValidator : BatchValidationHookHandler&lt;ProductRow&gt;
/// {
///     protected override Task ValidateAsync(
///         ValidationHookRequest&lt;ProductRow&gt; request,
///         ValidationResultBuilder results,
///         CancellationToken cancellationToken)
///     {
///         var seen = new HashSet&lt;string&gt;();
///         foreach (var row in request.Rows)
///         {
///             if (row.Values.Code is not null &amp;&amp; !seen.Add(row.Values.Code))
///                 results.ForRow(row).Error("code", "Duplicate code");
///         }
///         return Task.CompletedTask;
///     }
/// }
/// </code>
/// </para>
/// </summary>
/// <typeparam name="TValues">
/// A class whose properties map to template column keys.
/// Use <see cref="System.Text.Json.Serialization.JsonPropertyNameAttribute"/> to control mapping.
/// By default, <c>snake_case</c> naming policy is used.
/// </typeparam>
public abstract class BatchValidationHookHandler<TValues> : IValidationHookHandler
    where TValues : class
{
    /// <inheritdoc />
    public async Task<ValidationHookResponseItem[]> HandleAsync(
        ValidationHookRequest request,
        CancellationToken cancellationToken = default)
    {
        var typedRequest = ValidationHookRequest<TValues>.FromRequest(request);
        var results = new ValidationResultBuilder();
        await ValidateAsync(typedRequest, results, cancellationToken);
        return results.Build();
    }

    /// <summary>
    /// Override to perform batch validation across all strongly-typed rows.
    /// Use the <paramref name="results"/> builder to report errors and warnings.
    /// </summary>
    protected abstract Task ValidateAsync(
        ValidationHookRequest<TValues> request,
        ValidationResultBuilder results,
        CancellationToken cancellationToken);
}

