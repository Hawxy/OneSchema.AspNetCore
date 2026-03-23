using System.Text.Json.Serialization;
using OneSchema.AspNetCore.Api.Serialization;

namespace OneSchema.AspNetCore.Api;

/// <summary>
/// Represents a single validation entry in the validation hook response.
/// The response itself is an array of these objects (<c>ValidationHookResponseItem[]</c>).
/// </summary>
public class ValidationHookResponseItem
{
    /// <summary>
    /// The row_id with the warning or error.
    /// </summary>
    [JsonPropertyName("row_id")]
    public required int RowId { get; set; }

    /// <summary>
    /// The key of the template column where the validation should be displayed.
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; set; }

    /// <summary>
    /// (Optional) The index of the column for this validation, based on the columns array
    /// provided in the JSON POST request body. Required if the column is a custom column.
    /// </summary>
    [JsonPropertyName("column_index")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ColumnIndex { get; set; }

    /// <summary>
    /// Either "warning" (displayed in yellow) or "error" (displayed in red).
    /// </summary>
    [JsonPropertyName("severity")]
    public required string Severity { get; set; }

    /// <summary>
    /// Message to be displayed to the user when they click on the validation.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    /// <summary>
    /// (Optional) A suggestion or list of suggestions for a new value that will be displayed
    /// to the user. The user may choose whether or not to accept the suggested value.
    /// Accepts a single string or an array of strings.
    /// </summary>
    [JsonPropertyName("suggestion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(StringOrStringArrayConverter))]
    public StringOrStringArray? Suggestion { get; set; }

    /// <summary>
    /// (Optional) Additional information related to the error.
    /// </summary>
    [JsonPropertyName("options")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ValidationOptions? Options { get; set; }
}

/// <summary>
/// Additional information related to a validation error or warning.
/// </summary>
public class ValidationOptions
{
    /// <summary>
    /// The title of the popover which appears when the user clicks an error or warning in OneSchema.
    /// </summary>
    [JsonPropertyName("popover_title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PopoverTitle { get; set; }
}