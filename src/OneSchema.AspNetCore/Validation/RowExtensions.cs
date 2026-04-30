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