using System.Text.Json.Serialization;

namespace OneSchemaSandbox.Handlers;

/// <summary>
/// Strongly-typed POCO representing the "contacts" template columns.
/// </summary>
public class ContactRow
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}

