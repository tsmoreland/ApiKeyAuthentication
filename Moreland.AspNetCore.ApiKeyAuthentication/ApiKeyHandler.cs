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
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moreland.AspNetCore.ApiKeyAuthentication.Data;
using Moreland.AspNetCore.ApiKeyAuthentication.Helpers;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    internal static class ApiKeyHandler
    {
        private static readonly Lazy<XmlSerializer> _xmlSerializer =
            new Lazy<XmlSerializer>(() =>
                new XmlSerializer(typeof(ProblemDetails)));
        private static readonly Lazy<JsonSerializerOptions> _jsonSerializerOptions =
            new Lazy<JsonSerializerOptions>(() =>
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true});

        public static JsonSerializerOptions SerializerOptions => _jsonSerializerOptions.Value;
        public static XmlSerializer XmlSerializer => _xmlSerializer.Value;

    }


    /// <summary>
    /// Handles X-Api-Key Authentication
    /// </summary>
    /// <typeparam name="TConsumerId"></typeparam>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class ApiKeyHandler<TConsumerId> : AuthenticationHandler<ApiKeyOptions>
    {
        private readonly IApiKeyRepository<TConsumerId> _repository;

        /// <inheritdoc cref="AuthenticationHandler{ApiKeyOptions}"/>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="repository"/> is null
        /// </exception>
        public ApiKeyHandler(
            IOptionsMonitor<ApiKeyOptions> options,
            ILoggerFactory logger,
            UrlEncoder urlEncoder,
            ISystemClock systemClock,
            IApiKeyRepository<TConsumerId> repository)
            : base(options, logger, urlEncoder, systemClock)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        private string HeaderName => Options.HeaderName;

        private string? BuildJsonResponse(int status, string title, string detail) =>
            JsonSerializer.Serialize(
                GetDetails(status, title, detail, Request.Path),
                ApiKeyHandler.SerializerOptions);

        private string? BuildXmlResponse(int status, string title, string detail)
        {
            try
            {
                var details = GetDetails(status, title, detail, Request.Path);
                using var writer = new EncodedStringWriter(Encoding.UTF8);
                ApiKeyHandler.XmlSerializer.Serialize(writer, details);
                return writer.ToString();
            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                Logger.LogError(e, "Unable to serialize problem detail");
                return null;
            }
        }

        private static ProblemDetails GetDetails(int status, string title, string detail, string instance) =>
            new ProblemDetails 
            {
                Title = title, 
                Detail = detail, 
                Status = status, 
                Instance = instance,
                Type = $"https://httpstatuses.com/{status}"
            };
        private async Task WriteResponse(string responseType, Func<int, string, string, string?> buildResponse, int status, string title, string detail)
        {
            Response.ContentType = responseType;
            await Response.WriteAsync(buildResponse(status, title,detail));
        }
 
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
            switch (HttpRequestHelpers.GetResponseTypeFromRequest(Request))
            {
                case MediaTypeNames.Application.Xml:
                    await WriteResponse("application/problems+xml", BuildXmlResponse, 403, "Unauthorized", "You are not authorized to view this resource");
                    break;
                default:
                    await WriteResponse("application/problems+json", BuildJsonResponse, 403, "Unauthorized", "You are not authorized to view this resource");
                    break;
            }
        }

        /// <inheritdoc />
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;

            switch (HttpRequestHelpers.GetResponseTypeFromRequest(Request))
            {
                case MediaTypeNames.Application.Xml:
                    await WriteResponse("application/problems+xml", BuildXmlResponse, 401, "Forbidden", "You are not authorized to view this resource");
                    break;
                default:
                    await WriteResponse("application/problems+json", BuildJsonResponse, 401, "Forbidden", "You are not authorized to view this resource");
                    break;
            }
        }

   }
}
