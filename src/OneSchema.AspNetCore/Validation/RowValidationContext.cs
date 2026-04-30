using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation;

/// <summary>
/// Provides convenient access to a single row's data and methods to report validation results.
/// </summary>
public class RowValidationContext
{
    private readonly RowResultBuilder _results;

    internal RowValidationContext(Row row, ValidationHookRequest request, ValidationResultBuilder builder)
    {
        Row = row;
        Request = request;
        _results = builder.ForRow(row);
    }

    /// <summary>The row being validated.</summary>
    public Row Row { get; }

    /// <summary>
    /// The full validation hook request, providing access to <see cref="ValidationHookRequest.Columns"/>,
    /// <see cref="ValidationHookRequest.SheetMetadata"/>, and other request-level data.
    /// </summary>
    public ValidationHookRequest Request { get; }

    /// <summary>
    /// Gets the value of the specified column key for this row, or <c>null</c> if absent.
    /// </summary>
    public string? this[string columnKey] => Row.Values.GetValueOrDefault(columnKey);

    /// <summary>
    /// Returns <c>true</c> if the specified column key has a non-null, non-empty value.
    /// </summary>
    public bool HasValue(string columnKey) => !string.IsNullOrEmpty(this[columnKey]);

    /// <summary>
    /// Adds an error for the specified column on this row.
    /// Returns the <see cref="RowResultBuilder"/> for chaining modifiers like
    /// <see cref="RowResultBuilder.WithSuggestion"/>.
    /// </summary>
    public RowResultBuilder Error(string column, string message)
        => _results.Error(column, message);

    /// <summary>
    /// Adds a warning for the specified column on this row.
    /// Returns the <see cref="RowResultBuilder"/> for chaining modifiers like
    /// <see cref="RowResultBuilder.WithSuggestion"/>.
    /// </summary>
    public RowResultBuilder Warning(string column, string message)
        => _results.Warning(column, message);
}