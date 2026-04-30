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
using System.Diagnostics.CodeAnalysis;
using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation;

/// <summary>
/// Fluent builder for constructing validation hook responses.
/// <para>
/// Example usage:
/// <code>
/// var results = new ValidationResultBuilder();
///
/// foreach (var row in request.Rows)
/// {
///     var email = row.GetValue("email");
///     if (string.IsNullOrWhiteSpace(email))
///         results.ForRow(row).Error("email", "Email is required");
///     else if (!email.Contains('@'))
///         results.ForRow(row)
///             .Warning("email", "Email appears invalid")
///             .WithSuggestion("user@example.com");
/// }
///
/// return results.Build();
/// </code>
/// </para>
/// </summary>
public sealed class ValidationResultBuilder
{
    private readonly List<ValidationHookResponseItem> _items = [];

    /// <summary>
    /// Begins building validation results for the row with the specified id.
    /// </summary>
    public RowResultBuilder ForRow(int rowId) => new(this, rowId);

    /// <summary>
    /// Begins building validation results for the specified row.
    /// </summary>
    public RowResultBuilder ForRow(RowBase row) => new(this, row.RowId);

    /// <summary>
    /// Builds the final array of <see cref="ValidationHookResponseItem"/> to return as the response.
    /// </summary>
    public IReadOnlyList<ValidationHookResponseItem> Build() => _items;

    /// <summary>
    /// Returns <c>true</c> if no errors or warnings have been added.
    /// </summary>
    public bool IsValid => _items.Count == 0;

    /// <summary>
    /// The current number of validation items (errors + warnings).
    /// </summary>
    public int Count => _items.Count;

    internal void Add(ValidationHookResponseItem item) => _items.Add(item);
}

/// <summary>
/// Builder scoped to a single row for adding errors and warnings with optional modifiers.
/// All methods return <c>this</c> for fluent chaining.
/// </summary>
public sealed class RowResultBuilder
{
    private readonly ValidationResultBuilder _parent;
    private readonly int _rowId;
    private ValidationHookResponseItem? _lastItem;

    internal RowResultBuilder(ValidationResultBuilder parent, int rowId)
    {
        _parent = parent;
        _rowId = rowId;
    }

    /// <summary>
    /// Adds an error validation for the specified column.
    /// </summary>
    /// <param name="column">The template column key where the error should be displayed.</param>
    /// <param name="message">The message shown to the user when they click on the error.</param>
    public RowResultBuilder Error(string column, string message)
        => Add("error", column, message);

    /// <summary>
    /// Adds a warning validation for the specified column.
    /// </summary>
    /// <param name="column">The template column key where the warning should be displayed.</param>
    /// <param name="message">The message shown to the user when they click on the warning.</param>
    public RowResultBuilder Warning(string column, string message)
        => Add("warning", column, message);

    /// <summary>
    /// Attaches a single suggested replacement value to the last added error or warning.
    /// </summary>
    public RowResultBuilder WithSuggestion(string suggestion)
    {
        EnsureLastItem();
        _lastItem.Suggestion = suggestion;
        return this;
    }

    /// <summary>
    /// Attaches multiple suggested replacement values to the last added error or warning.
    /// The user may choose from any of the suggestions.
    /// </summary>
    public RowResultBuilder WithSuggestions(string[] suggestions)
    {
        EnsureLastItem();
        _lastItem.Suggestion = suggestions;
        return this;
    }

    /// <summary>
    /// Sets the popover title for the last added error or warning.
    /// </summary>
    public RowResultBuilder WithPopoverTitle(string title)
    {
        EnsureLastItem();
        _lastItem.Options ??= new ValidationOptions();
        _lastItem.Options.PopoverTitle = title;
        return this;
    }

    /// <summary>
    /// Sets the column index for the last added error or warning.
    /// Required when the target column is a custom column.
    /// </summary>
    public RowResultBuilder WithColumnIndex(int index)
    {
        EnsureLastItem();
        _lastItem.ColumnIndex = index;
        return this;
    }

    private RowResultBuilder Add(string severity, string column, string message)
    {
        _lastItem = new ValidationHookResponseItem
        {
            RowId = _rowId,
            Column = column,
            Severity = severity,
            Message = message
        };
        _parent.Add(_lastItem);
        return this;
    }

    [MemberNotNull(nameof(_lastItem))]
    private void EnsureLastItem()
    {
        if (_lastItem is null)
            throw new InvalidOperationException(
                "Call Error() or Warning() before calling a modifier such as WithSuggestion().");
    }
}