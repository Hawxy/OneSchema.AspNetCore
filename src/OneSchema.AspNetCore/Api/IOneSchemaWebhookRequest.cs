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
using System.Security.Claims;

namespace OneSchema.AspNetCore.Api;

/// <summary>
/// Common interface for OneSchema webhook request types that carry an embedded user JWT.
/// Used by the JWT validation filter to extract the token from the request body.
/// </summary>
public interface IOneSchemaWebhookRequest
{
    /// <summary>
    /// The JWT token used to initialize the customer's embedded session.
    /// </summary>
    string EmbedUserJwt { get; }

    /// <summary>
    /// The <see cref="ClaimsIdentity"/> produced by validating <see cref="EmbedUserJwt"/>,
    /// or <c>null</c> if JWT validation was not applied or the token was not validated.
    /// <para>
    /// This property is automatically populated by the JWT validation filter when
    /// <c>RequireOneSchemaJwtValidation()</c> is applied to the endpoint.
    /// </para>
    /// </summary>
    ClaimsIdentity? Identity { get; set; }
}

