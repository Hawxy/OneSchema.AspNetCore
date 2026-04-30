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
    public async Task<IReadOnlyList<ValidationHookResponseItem>> HandleAsync(
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