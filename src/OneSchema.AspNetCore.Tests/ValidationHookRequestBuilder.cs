using System.Text.Json;

namespace OneSchema.AspNetCore.Tests;

/// <summary>
/// Builds OneSchema validation hook request JSON payloads for testing.
/// </summary>
internal sealed class ValidationHookRequestBuilder
{
    private readonly List<object> _rows = [];
    private string _embedUserJwt = "";
    private string _templateKey = "test-template";

    public ValidationHookRequestBuilder WithJwt(string jwt)
    {
        _embedUserJwt = jwt;
        return this;
    }

    public ValidationHookRequestBuilder WithTemplateKey(string key)
    {
        _templateKey = key;
        return this;
    }

    public ValidationHookRequestBuilder AddRow(int rowId, Dictionary<string, string> values)
    {
        _rows.Add(new { row_id = rowId, values });
        return this;
    }

    public string BuildJson() => JsonSerializer.Serialize(new
    {
        rows = _rows,
        columns = new[]
        {
            new
            {
                sheet_column_name = "Col A",
                template_column_name = "Column A",
                template_column_key = "col_a"
            }
        },
        template_key = _templateKey,
        hook_id = 1,
        hook_name = "test-hook",
        workspace_id = 1,
        sheet_id = 1,
        sheet_metadata = new { original_file_name = "test.csv" },
        embed_user_jwt = _embedUserJwt,
        request_id = Guid.NewGuid().ToString()
    });
}

