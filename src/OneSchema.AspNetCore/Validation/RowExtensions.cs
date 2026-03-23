using OneSchema.AspNetCore.Api;

namespace OneSchema.AspNetCore.Validation;

/// <summary>
/// Extension methods for easier <see cref="Row"/> value access.
/// </summary>
public static class RowExtensions
{
    extension(Row row)
    {
        /// <summary>
        /// Gets the value of the specified column key, or <c>null</c> if the key is not present.
        /// </summary>
        public string? GetValue(string columnKey)
            => row.Values.GetValueOrDefault(columnKey);

        /// <summary>
        /// Returns <c>true</c> if the specified column key has a non-null, non-empty value.
        /// </summary>
        public bool HasValue(string columnKey)
            => !string.IsNullOrEmpty(row.Values.GetValueOrDefault(columnKey));
        
        public bool TryGetValue(string columnKey, out string? value)
            => row.Values.TryGetValue(columnKey, out value);
    }
}