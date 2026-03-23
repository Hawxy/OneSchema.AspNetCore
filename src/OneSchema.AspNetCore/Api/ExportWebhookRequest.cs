using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneSchema.AspNetCore.Api;

/// <summary>
/// Represents the JSON POST request body sent by OneSchema for an export webhook.
/// </summary>
public class ExportWebhookRequest
{
    /// <summary>
    /// A unique ID for this webhook request.
    /// </summary>
    [JsonPropertyName("request_id")]
    public required string RequestId { get; set; }

    /// <summary>
    /// Unique id generated for the export. Used to combine data from multiple requests
    /// when exporting larger lists.
    /// </summary>
    [JsonPropertyName("event_id")]
    public required string EventId { get; set; }

    /// <summary>
    /// The Webhook Key for the webhook used for the export.
    /// </summary>
    [JsonPropertyName("webhook_key")]
    public required string WebhookKey { get; set; }

    /// <summary>
    /// The Template Key for the Template applied to the list at the time of export.
    /// </summary>
    [JsonPropertyName("template_key")]
    public required string TemplateKey { get; set; }

    /// <summary>
    /// The id of the Workspace the exported Sheet belongs to.
    /// </summary>
    [JsonPropertyName("workspace_id")]
    public required int WorkspaceId { get; set; }

    /// <summary>
    /// A JSON Object containing any custom metadata associated with the Workspace.
    /// </summary>
    [JsonPropertyName("workspace_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public WorkspaceMetadata? WorkspaceMetadata { get; set; }

    /// <summary>
    /// The id of the Sheet being exported.
    /// </summary>
    [JsonPropertyName("sheet_id")]
    public required int SheetId { get; set; }

    /// <summary>
    /// The id of the Embed session.
    /// </summary>
    [JsonPropertyName("embed_id")]
    public required int EmbedId { get; set; }

    /// <summary>
    /// A JSON Object containing any custom metadata associated with the Sheet at time of import.
    /// Includes two additional fields added automatically by OneSchema: <c>original_file_name</c>
    /// containing the name of the original uploaded file, and <c>original_sheet_name</c> containing
    /// the name of the sheet in the imported Excel file, if part of a multi-sheet Excel file upload.
    /// </summary>
    [JsonPropertyName("sheet_metadata")]
    public required SheetMetadata SheetMetadata { get; set; }

    /// <summary>
    /// Information about the columns that are included in the export.
    /// </summary>
    [JsonPropertyName("columns")]
    public required Column[] Columns { get; set; }

    /// <summary>
    /// When exporting data that was imported from an embedded OneSchema instance,
    /// the user JWT that was used to initiate the embedded import flow.
    /// </summary>
    [JsonPropertyName("embed_user_jwt")]
    public required string EmbedUserJwt { get; set; }

    /// <summary>
    /// Current request for the current export, numbered 1 through <see cref="SequenceCount"/>.
    /// </summary>
    [JsonPropertyName("sequence_number")]
    public required int SequenceNumber { get; set; }

    /// <summary>
    /// Number of requests that will be made for this webhook export.
    /// </summary>
    [JsonPropertyName("sequence_count")]
    public required int SequenceCount { get; set; }

    /// <summary>
    /// Total number of exported records.
    /// </summary>
    [JsonPropertyName("total_record_count")]
    public required int TotalRecordCount { get; set; }

    /// <summary>
    /// The row data for the export. Contains records without errors in <see cref="ExportData.Records"/>
    /// and records with errors in <see cref="ExportData.ErrorRecords"/>.
    /// </summary>
    [JsonPropertyName("data")]
    public required ExportData Data { get; set; }
}

/// <summary>
/// Contains the actual row data for an export webhook request.
/// </summary>
public class ExportData
{
    /// <summary>
    /// Number of records contained in the request, default of 1000, configurable via the UI.
    /// </summary>
    [JsonPropertyName("count")]
    public required int Count { get; set; }

    /// <summary>
    /// Array of records that have no errors. Each record is a JSON object whose keys are the
    /// Column Keys of the Template's columns.
    /// </summary>
    [JsonPropertyName("records")]
    public required JsonElement Records { get; set; }

    /// <summary>
    /// Array of records that have one or more errors.
    /// </summary>
    [JsonPropertyName("error_records")]
    public required ExportErrorRecord[] ErrorRecords { get; set; }
}

/// <summary>
/// Represents a record that contains one or more errors.
/// </summary>
public class ExportErrorRecord
{
    /// <summary>
    /// The original data for a record that contains errors. Keys are Column Keys.
    /// </summary>
    [JsonPropertyName("data")]
    public required Dictionary<string, object?> Data { get; set; }

    /// <summary>
    /// Errors for a record that contains errors. The keys of the object are the columns with errors,
    /// and the values are lists of error codes.
    /// </summary>
    [JsonPropertyName("errors")]
    public required Dictionary<string, string[]> Errors { get; set; }
}

/// <summary>
/// A JSON Object containing any custom metadata associated with the Workspace.
/// </summary>
public class WorkspaceMetadata
{
    /// <summary>
    /// Additional custom metadata fields associated with the Workspace.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? CustomMetadata { get; set; }
}

