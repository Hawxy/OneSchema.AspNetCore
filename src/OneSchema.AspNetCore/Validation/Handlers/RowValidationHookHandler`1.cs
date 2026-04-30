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
/// Convenient base class for validation hooks that validate one row at a time with
/// strongly-typed row values.
/// <para>
/// Example:
/// <code>
/// public class InvoiceRow
/// {
///     [JsonPropertyName("email")]
///     public string Email { get; set; }
///     [JsonPropertyName("amount")]
///     public string Amount { get; set; }
/// }
///
/// public class InvoiceValidator : RowValidationHookHandler&lt;InvoiceRow&gt;
/// {
///     protected override ValueTask ValidateRowAsync(RowValidationContext&lt;InvoiceRow&gt; context, CancellationToken ct)
///     {
///         if (string.IsNullOrWhiteSpace(context.Values.Email))
///             context.Error("email", "Email is required");
///
///         return ValueTask.CompletedTask;
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
public abstract class RowValidationHookHandler<TValues> : IValidationHookHandler<TValues>
    where TValues : class
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<ValidationHookResponseItem>> HandleAsync(
        ValidationHookRequest<TValues> request,
        CancellationToken cancellationToken = default)
    {
        var builder = new ValidationResultBuilder();

        foreach (var row in request.Rows)
        {
            var context = new RowValidationContext<TValues>(row, request, builder);
            await ValidateRowAsync(context, cancellationToken);
        }

        return builder.Build();
    }

    /// <summary>
    /// Performs asynchronous per-row validation with strongly-typed row values.
    /// </summary>
    protected abstract ValueTask ValidateRowAsync(
        RowValidationContext<TValues> context,
        CancellationToken cancellationToken);
}
