//
// Copyright © 2020 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moreland.AspNetCore.ApiKeyAuthentication.Data;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    internal static class ApiKeyHandler
    {
        public static Lazy<JsonSerializerOptions> SerializerOptions { get; } =
            new Lazy<JsonSerializerOptions>(() =>
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true});
    }


    public sealed class ApiKeyHandler<TExternalId> : AuthenticationHandler<ApiKeyOptions>
    {
        private readonly IApiKeyRepository<TExternalId> _repository;


        private static JsonSerializerOptions SerializerOptions => ApiKeyHandler.SerializerOptions.Value;

        /// <inheritdoc cref="AuthenticationHandler{ApiKeyOptions}"/>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="repository"/> is null
        /// </exception>
        public ApiKeyHandler(
            IOptionsMonitor<ApiKeyOptions> options,
            ILoggerFactory logger,
            UrlEncoder urlEncoder,
            ISystemClock systemClock,
            IApiKeyRepository<TExternalId> repository)
            : base(options, logger, urlEncoder, systemClock)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        private string HeaderName => Options.HeaderName;

        private static string? BuildResponse(int status, string title, string detail) =>
            JsonSerializer.Serialize(
                new {title, detail, status, type = $"https://httpstatuses.com/{status}"},
                SerializerOptions);


        /// <inheritdoc cref="HandleAuthenticateAsync"/>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var matchingHeaders) || !matchingHeaders.Any())
            {
                Logger.LogDebug($"No matching Api-Key headers found");
                return AuthenticateResult.NoResult();
            }

            var apiKey = matchingHeaders.FirstOr(string.Empty);
            if (string.IsNullOrEmpty(apiKey))
            {
                Logger.LogWarning("Api-Key is empty");
                return AuthenticateResult.NoResult();
            }

            var apiKeyEntity = await _repository.GetByKeyAsync(apiKey);
            if (!apiKeyEntity.IsNullorEmpty())
                return AuthenticateResult.Success(apiKeyEntity.ToAuthenticationTicket(Options.Scheme, Options.AuthenticationType));

            Logger.LogWarning($"Api-Key {apiKey} not found");
            return AuthenticateResult.Fail(new KeyNotFoundException("Unauthorized access."));
        }

        /// <inheritdoc />
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/problems+json";
            await Response.WriteAsync(BuildResponse(401, "Unauthorized", "You are not authorized to view this resource"));
        }

        /// <inheritdoc />
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = "application/problems+json";
            await Response.WriteAsync(BuildResponse(403, "Forbidden", "You are not authorized to view this resource"));
        }
    }
}
