namespace OneSchema.AspNetCore.Api;

/// <summary>
/// Represents a value that can be either a single string or an array of strings.
/// </summary>
public class StringOrStringArray
{
    public string? SingleValue { get; }
    public string[]? MultipleValues { get; }
    public bool IsArray => MultipleValues is not null;

    public StringOrStringArray(string value)
    {
        SingleValue = value;
    }

    public StringOrStringArray(string[] values)
    {
        MultipleValues = values;
    }

    public static implicit operator StringOrStringArray(string value) => new(value);
    public static implicit operator StringOrStringArray(string[] values) => new(values);
}