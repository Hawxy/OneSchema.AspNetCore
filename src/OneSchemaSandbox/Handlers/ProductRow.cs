using System.Text.Json.Serialization;

namespace OneSchemaSandbox.Handlers;

/// <summary>
/// Strongly-typed POCO representing the "products" template columns.
/// </summary>
public class ProductRow
{
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public string? Price { get; set; }
}

