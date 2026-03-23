using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation;

/// <summary>
/// Provides convenient access to a single row's strongly-typed data and methods to report validation results.
/// </summary>
/// <typeparam name="TValues">The strongly-typed POCO representing the row's column values.</typeparam>
public class RowValidationContext<TValues> where TValues : class
{
    private readonly RowResultBuilder _results;

    internal RowValidationContext(Row<TValues> row, ValidationHookRequest<TValues> request, ValidationResultBuilder builder)
    {
        Row = row;
        Request = request;
        _results = builder.ForRow(row.RowId);
    }

    /// <summary>The row being validated, with strongly-typed values.</summary>
    public Row<TValues> Row { get; }

    /// <summary>The strongly-typed values for this row (shorthand for <c>Row.Values</c>).</summary>
    public TValues Values => Row.Values;

    /// <summary>
    /// The full strongly-typed validation hook request, providing access to
    /// <see cref="ValidationHookRequest{TValues}.Columns"/>,
    /// <see cref="ValidationHookRequest{TValues}.SheetMetadata"/>,
    /// and other request-level data, as well as all typed rows.
    /// </summary>
    public ValidationHookRequest<TValues> Request { get; }

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

