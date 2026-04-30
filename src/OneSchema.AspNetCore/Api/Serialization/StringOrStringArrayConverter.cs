/*
   Copyright 2026 Hawxy (JT)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneSchema.AspNetCore.Api.Serialization;

/// <summary>
/// JSON converter that serializes/deserializes <see cref="StringOrStringArray"/>
/// as either a JSON string or a JSON array of strings.
/// </summary>
public class StringOrStringArrayConverter : JsonConverter<StringOrStringArray>
{
    public override StringOrStringArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!;
            return new StringOrStringArray(value);
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var values = new List<string>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                values.Add(reader.GetString()!);
            }
            return new StringOrStringArray(values.ToArray());
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} for StringOrStringArray.");
    }

    public override void Write(Utf8JsonWriter writer, StringOrStringArray value, JsonSerializerOptions options)
    {
        if (value.IsArray)
        {
            writer.WriteStartArray();
            foreach (var item in value.MultipleValues!)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }
        else
        {
            writer.WriteStringValue(value.SingleValue);
        }
    }
}