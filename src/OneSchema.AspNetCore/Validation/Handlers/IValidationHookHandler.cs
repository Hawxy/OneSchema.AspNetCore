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

namespace OneSchema.AspNetCore.Validation.Handlers;

/// <summary>
/// Defines a handler for processing OneSchema validation hook requests.
/// Implement this interface directly for full control over the validation flow.
/// </summary>
public interface IValidationHookHandler
{
    /// <summary>
    /// Processes the validation hook request and returns validation results.
    /// </summary>
    Task<IReadOnlyList<ValidationHookResponseItem>> HandleAsync(
        ValidationHookRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler for processing OneSchema validation hook requests with strongly-typed row values.
/// Implement this interface directly for full control over the validation flow.
/// </summary>
/// <typeparam name="TValues">
/// A class whose properties map to template column keys.
/// The JSON body is deserialized directly into <see cref="ValidationHookRequest{TValues}"/>.
/// </typeparam>
public interface IValidationHookHandler<TValues> where TValues : class
{
    /// <summary>
    /// Processes the strongly-typed validation hook request and returns validation results.
    /// </summary>
    Task<IReadOnlyList<ValidationHookResponseItem>> HandleAsync(
        ValidationHookRequest<TValues> request,
        CancellationToken cancellationToken = default);
}
