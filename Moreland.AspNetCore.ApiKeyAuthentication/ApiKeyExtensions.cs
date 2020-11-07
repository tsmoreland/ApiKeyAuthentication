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
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    /// <summary>
    /// <see cref="ApiKey"/> Extension methods
    /// </summary>
    public static class ApiKeyExtensions
    {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="apiKey"/> is null,
        /// equals <see cref="ApiKey{TConsumerId}.Empty"/> or its id is an empty
        /// GUID
        /// </summary>
        public static bool IsNullorEmpty<TConsumerId>(this ApiKey<TConsumerId>? apiKey) =>
            apiKey == null ||
            ReferenceEquals(apiKey, ApiKey.Empty<TConsumerId>()) ||
            apiKey.Id == Guid.Empty;

        public static AuthenticationTicket ToAuthenticationTicket<TConsumerId>(this ApiKey<TConsumerId> apiKey, string scheme, string authenticationType)
        {
            if (apiKey.IsNullorEmpty())
                throw new ArgumentException("Invalid Api-Key", nameof(apiKey));

            var claims = Lists
                .From(new Claim(ClaimTypes.Name, apiKey.Consumer))
                .AppendRange(apiKey.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new AuthenticationTicket(
                new ClaimsPrincipal(Lists.From(new ClaimsIdentity(claims, authenticationType))), 
                scheme);

        }
    }
}
