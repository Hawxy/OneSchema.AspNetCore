using System.Text.Json.Serialization;

namespace OneSchema.AspNetCore.Api;

/// <summary>
/// Base class containing the shared metadata properties of a OneSchema validation hook request.
/// </summary>
public abstract class ValidationHookRequestBase : IOneSchemaWebhookRequest
{
    /// <summary>
    /// Ordered array of mapped columns. Used for referencing custom columns.
    /// </summary>
    [JsonPropertyName("columns")]
    public required Column[] Columns { get; set; }

    /// <summary>
    /// An optional key specified on the template that can be used instead of template_id
    /// to determine what template is being validated.
    /// </summary>
    [JsonPropertyName("template_key")]
    public required string TemplateKey { get; set; }

    /// <summary>
    /// A unique identifier for this validation hook.
    /// </summary>
    [JsonPropertyName("hook_id")]
    public required int HookId { get; set; }

    /// <summary>
    /// The name of the hook, as configured on the validation hook UI.
    /// </summary>
    [JsonPropertyName("hook_name")]
    public required string HookName { get; set; }

    /// <summary>
    /// The id of the Workspace containing the Sheet containing the rows being validated.
    /// </summary>
    [JsonPropertyName("workspace_id")]
    public required int WorkspaceId { get; set; }

    /// <summary>
    /// The id of the Sheet containing the rows being validated.
    /// </summary>
    [JsonPropertyName("sheet_id")]
    public required int SheetId { get; set; }

    /// <summary>
    /// A JSON Object containing any custom metadata associated with the Sheet at time of import.
    /// Includes two additional fields added automatically by OneSchema: <c>original_file_name</c>
    /// containing the name of the original uploaded file, and <c>original_sheet_name</c> containing
    /// the name of the sheet in the imported Excel file, if part of a multi-sheet Excel file upload.
    /// </summary>
    [JsonPropertyName("sheet_metadata")]
    public required SheetMetadata SheetMetadata { get; set; }

    /// <summary>
    /// If you are embedding OneSchema within your own site, this will contain the JWT token
    /// used to initialize a customer's session.
    /// </summary>
    [JsonPropertyName("embed_user_jwt")]
    public required string EmbedUserJwt { get; set; }

    /// <summary>
    /// A unique ID for this webhook request.
    /// </summary>
    [JsonPropertyName("request_id")]
    public required string RequestId { get; set; }
}

/// <summary>
/// Represents the JSON POST request body sent by OneSchema for a validation hook.
/// </summary>
public class ValidationHookRequest : ValidationHookRequestBase
{
    /// <summary>
    /// Array of rows to validate.
    /// </summary>
    [JsonPropertyName("rows")]
    public required Row[] Rows { get; set; }
}

/// <summary>
/// Strongly-typed version of <see cref="ValidationHookRequest"/> where each row's
/// <c>values</c> object is deserialized directly into <typeparamref name="TValues"/>.
/// <para>
/// This class is designed to be deserialized directly from the OneSchema webhook JSON body.
/// Use it with <see cref="OneSchema.AspNetCore.Validation.Handlers.IValidationHookHandler{TValues}"/>
/// and the corresponding <c>MapValidationHook</c> endpoint overload.
/// </para>
/// </summary>
/// <typeparam name="TValues">
/// A class whose properties map to template column keys.
/// Use <see cref="JsonPropertyNameAttribute"/> to control mapping.
/// </typeparam>
public class ValidationHookRequest<TValues> : ValidationHookRequestBase where TValues : class
{
    /// <summary>Typed rows.</summary>
    [JsonPropertyName("rows")]
    public required Row<TValues>[] Rows { get; set; }
}

/// <summary>
/// Base class for a row in the validation hook request, containing the row identifier.
/// </summary>
public abstract class RowBase
{
    /// <summary>
    /// Id of a single row. Use this value in the response data to specify errors.
    /// </summary>
    [JsonPropertyName("row_id")]
    public required int RowId { get; set; }
}

/// <summary>
/// Represents a single row in the validation hook request.
/// </summary>
public class Row : RowBase
{
    /// <summary>
    /// A map from template column names to the value in that column for a single row.
    /// All values are represented as strings. Only columns that are part of the hook's
    /// configuration are included.
    /// </summary>
    [JsonPropertyName("values")]
    public required Dictionary<string, string> Values { get; set; }
}

/// <summary>
/// Represents a single row with a strongly-typed values object.
/// </summary>
public class Row<TValue> : RowBase where TValue : class
{
    /// <summary>
    /// The strongly-typed values for this row.
    /// </summary>
    [JsonPropertyName("values")]
    public required TValue Values { get; set; }
}

/// <summary>
/// Represents a mapped column in the validation hook request.
/// </summary>
public class Column
{
    /// <summary>
    /// The name of the column in the uploaded sheet.
    /// </summary>
    [JsonPropertyName("sheet_column_name")]
    public required string SheetColumnName { get; set; }

    /// <summary>
    /// The name of the template column this sheet column is mapped to.
    /// </summary>
    [JsonPropertyName("template_column_name")]
    public required string TemplateColumnName { get; set; }

    /// <summary>
    /// The key of the template column this sheet column is mapped to.
    /// </summary>
    [JsonPropertyName("template_column_key")]
    public required string TemplateColumnKey { get; set; }

    /// <summary>
    /// Whether this is a custom column (not part of the original template).
    /// </summary>
    [JsonPropertyName("is_custom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsCustom { get; set; }

    /// <summary>
    /// The name of the custom column, if this is a custom column.
    /// </summary>
    [JsonPropertyName("custom_column_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CustomColumnName { get; set; }
}

/// <summary>
/// A JSON Object containing any custom metadata associated with the Sheet at time of import.
/// Includes <c>original_file_name</c> and optionally <c>original_sheet_name</c>,
/// plus any additional custom metadata fields.
/// </summary>
public class SheetMetadata
{
    /// <summary>
    /// The name of the original uploaded file.
    /// </summary>
    [JsonPropertyName("original_file_name")]
    public required string OriginalFileName { get; set; }

    /// <summary>
    /// The name of the sheet in the imported Excel file, if part of a multi-sheet Excel file upload.
    /// </summary>
    [JsonPropertyName("original_sheet_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OriginalSheetName { get; set; }

    /// <summary>
    /// Additional custom metadata fields associated with the Sheet.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? CustomMetadata { get; set; }
}
