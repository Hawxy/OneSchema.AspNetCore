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