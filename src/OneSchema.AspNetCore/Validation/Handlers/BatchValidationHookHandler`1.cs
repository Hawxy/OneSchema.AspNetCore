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
public abstract class BatchValidationHookHandler<TValues> : IValidationHookHandler<TValues>
    where TValues : class
{
    /// <inheritdoc />
    public async Task<ValidationHookResponseItem[]> HandleAsync(
        ValidationHookRequest<TValues> request,
        CancellationToken cancellationToken = default)
    {
        var results = new ValidationResultBuilder();
        await ValidateAsync(request, results, cancellationToken);
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


