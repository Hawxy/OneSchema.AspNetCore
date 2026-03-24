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
    Task<ValidationHookResponseItem[]> HandleAsync(
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
    Task<ValidationHookResponseItem[]> HandleAsync(
        ValidationHookRequest<TValues> request,
        CancellationToken cancellationToken = default);
}
