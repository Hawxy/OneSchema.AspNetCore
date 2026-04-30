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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OneSchema.AspNetCore.Api;
using OneSchema.AspNetCore.Validation.Handlers;

namespace OneSchema.AspNetCore.Validation;

/// <summary>
/// Extension methods for registering and mapping validation hook handlers with minimal APIs.
/// </summary>
public static class ValidationHookEndpointExtensions
{
    /// <summary>
    /// Registers a <typeparamref name="THandler"/> in the DI container as a scoped service.
    /// </summary>
    public static IServiceCollection AddValidationHookHandler<THandler>(this IServiceCollection services)
        where THandler : class, IValidationHookHandler
    {
        services.AddScoped<THandler>();
        return services;
    }

    /// <summary>
    /// Registers a strongly-typed <typeparamref name="THandler"/> in the DI container as a scoped service.
    /// </summary>
    public static IServiceCollection AddValidationHookHandler<THandler, TValues>(this IServiceCollection services)
        where THandler : class, IValidationHookHandler<TValues>
        where TValues : class
    {
        services.AddScoped<THandler>();
        return services;
    }

    /// <summary>
    /// Maps a POST endpoint at <paramref name="pattern"/> that deserializes a
    /// <see cref="ValidationHookRequest"/>, dispatches it to <typeparamref name="THandler"/>,
    /// and returns the <see cref="ValidationHookResponseItem"/>[] result as JSON.
    /// </summary>
    public static RouteHandlerBuilder MapValidationHook<THandler>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where THandler : class, IValidationHookHandler
    {
        return endpoints.MapPost(pattern, async (
            ValidationHookRequest request,
            THandler handler,
            CancellationToken ct) =>
        {
            var results = await handler.HandleAsync(request, ct);
            return TypedResults.Json(results);
        });
    }

    /// <summary>
    /// Maps a POST endpoint at <paramref name="pattern"/> that deserializes a
    /// <see cref="ValidationHookRequest{TValues}"/> directly from the JSON body,
    /// dispatches it to <typeparamref name="THandler"/>,
    /// and returns the <see cref="ValidationHookResponseItem"/>[] result as JSON.
    /// </summary>
    public static RouteHandlerBuilder MapValidationHook<THandler, TValues>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where THandler : class, IValidationHookHandler<TValues>
        where TValues : class
    {
        return endpoints.MapPost(pattern, async (
            ValidationHookRequest<TValues> request,
            THandler handler,
            CancellationToken ct) =>
        {
            var results = await handler.HandleAsync(request, ct);
            return TypedResults.Json(results);
        });
    }
}