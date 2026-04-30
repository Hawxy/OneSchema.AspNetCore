using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation;

/// <summary>
/// A pre-built index from extracted keys back to their source rows.
/// Exposes <see cref="Keys"/> for use in batch queries and helper methods
/// to report errors/warnings for matching keys.
/// <para>
/// Create via <see cref="RowIndexExtensions.IndexBy{TKey}(Row[], Func{Row, TKey}, IEqualityComparer{TKey}?)"/>
/// or the strongly-typed overload.
/// </para>
/// </summary>
/// <example>
/// <code>
/// var byName = request.Rows.IndexBy(row => row.GetValue("name")!, StringComparer.OrdinalIgnoreCase);
///
/// var existing = await dbContext.Entities
///     .Where(e => byName.Keys.Contains(e.Name.ToLower()))
///     .Select(e => e.Name)
///     .ToListAsync(ct);
///
/// byName.ErrorForMatches(existing, results, "name", n => $"Entity '{n}' already exists.");
/// </code>
/// </example>
public sealed class RowIndex<TRow, TKey> where TRow : RowBase where TKey : notnull
{
    private readonly ILookup<TKey, TRow> _lookup;

    internal RowIndex(ILookup<TKey, TRow> lookup)
    {
        _lookup = lookup;
        Keys = [.. _lookup.Select(g => g.Key)];
    }

    /// <summary>
    /// The distinct keys extracted from the rows.
    /// Materialized as a list so it can be passed directly into LINQ-to-Entities
    /// <c>.Where(x => index.Keys.Contains(...))</c> expressions.
    /// </summary>
    public IReadOnlyList<TKey> Keys { get; }

    /// <summary>
    /// Gets all rows that produced the given <paramref name="key"/>.
    /// Returns an empty sequence if the key is not present.
    /// </summary>
    public IEnumerable<TRow> this[TKey key] => _lookup[key];

    /// <summary>
    /// Returns <c>true</c> if any row produced the given <paramref name="key"/>.
    /// </summary>
    public bool Contains(TKey key) => _lookup.Contains(key);

    /// <summary>
    /// For each key in <paramref name="matches"/>, adds an <b>error</b> to every row
    /// that produced that key.
    /// </summary>
    /// <param name="matches">Keys returned from an external query (e.g. duplicate names from the DB).</param>
    /// <param name="results">The result builder to add errors to.</param>
    /// <param name="column">The template column key where the error should be displayed.</param>
    /// <param name="messageFactory">A function that produces the error message from the matched key.</param>
    /// <param name="suggestions">The list of options to suggest to the user.</param>
    public void ErrorForMatches(
        IEnumerable<TKey> matches,
        ValidationResultBuilder results,
        string column,
        Func<TKey, string> messageFactory,
        string[]? suggestions = null
        )
    {
        foreach (var key in matches)
        foreach (var row in _lookup[key])
        {
            var rowResult = results.ForRow(row).Error(column, messageFactory(key));
            if (suggestions != null)
                rowResult.WithSuggestions(suggestions);
        }
              
    }

    /// <summary>
    /// For each key in <paramref name="matches"/>, adds a <b>warning</b> to every row
    /// that produced that key.
    /// </summary>
    /// <param name="matches">Keys returned from an external query.</param>
    /// <param name="results">The result builder to add warnings to.</param>
    /// <param name="column">The template column key where the warning should be displayed.</param>
    /// <param name="messageFactory">A function that produces the warning message from the matched key.</param>
    /// <param name="suggestions">The list of options to suggest to the user.</param>
    public void WarningForMatches(
        IEnumerable<TKey> matches,
        ValidationResultBuilder results,
        string column,
        Func<TKey, string> messageFactory,
        string[]? suggestions = null
        )
    {
        foreach (var key in matches)
        foreach (var row in _lookup[key])
        {
            var rowResult = results.ForRow(row).Warning(column, messageFactory(key));
            if (suggestions != null)
                rowResult.WithSuggestions(suggestions);
        }
              
    }
}

/// <summary>
/// Extension methods for building a <see cref="RowIndex{TRow, TKey}"/> from row arrays.
/// </summary>
public static class RowIndexExtensions
{
    /// <summary>
    /// Indexes untyped rows by a key extracted from each row.
    /// </summary>
    /// <example>
    /// <code>
    /// var byName = request.Rows.IndexBy(
    ///     row => row.GetValue("name")!,
    ///     StringComparer.OrdinalIgnoreCase);
    /// </code>
    /// </example>
    public static RowIndex<Row, TKey> IndexBy<TKey>(
        this IEnumerable<Row> rows,
        Func<Row, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null) where TKey : notnull
    {
        var lookup = rows.ToLookup(keySelector, comparer);
        return new RowIndex<Row, TKey>(lookup);
    }

    /// <summary>
    /// Indexes strongly-typed rows by a key extracted from each row's <typeparamref name="TValues"/>.
    /// The <paramref name="keySelector"/> receives the values object directly for convenience.
    /// </summary>
    /// <example>
    /// <code>
    /// var byName = request.Rows.IndexBy(
    ///     v => v.Name!,
    ///     StringComparer.OrdinalIgnoreCase);
    /// </code>
    /// </example>
    public static RowIndex<Row<TValues>, TKey> IndexBy<TValues, TKey>(
        this IEnumerable<Row<TValues>> rows,
        Func<TValues, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TValues : class
        where TKey : notnull
    {
        var lookup = rows.ToLookup(row => keySelector(row.Values), comparer);
        return new RowIndex<Row<TValues>, TKey>(lookup);
    }
}

