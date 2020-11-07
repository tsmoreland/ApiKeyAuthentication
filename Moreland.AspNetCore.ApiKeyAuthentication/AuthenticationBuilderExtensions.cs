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
using Microsoft.AspNetCore.Authentication;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    /// <summary>
    /// Extension methods for <see cref="AuthenticationBuilder"/>
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Add Api-Key support with default scheme and empty display name 
        /// and empty options
        /// </summary>
        public static AuthenticationBuilder AddApiKeySupport<TExternalId>(this AuthenticationBuilder builder) =>
            builder.AddApiKeySupport<TExternalId>(ApiKeyDefaults.AuthenticationScheme, displayName: null, options => { });

        /// <summary>
        /// Add Api-Key support with default scheme and empty display name using
        /// provided <paramref name="options"/>
        /// </summary>
        public static AuthenticationBuilder AddApiKeySupport<TExternalId>(this AuthenticationBuilder builder,
            Action<ApiKeyOptions> options) =>
            builder.AddApiKeySupport<TExternalId>(ApiKeyDefaults.AuthenticationScheme, displayName: null, options);

        /// <summary>
        /// Add Api-Key support with empty display name using
        /// provided <paramref name="scheme"/> and <paramref name="options"/>
        /// </summary>
        public static AuthenticationBuilder AddApiKeySupport<TExternalId>(this AuthenticationBuilder builder,
            string scheme, Action<ApiKeyOptions> options) =>
            builder.AddApiKeySupport<TExternalId>(scheme, displayName: null, options);

        /// <summary>
        /// Add Api-Key support provided <paramref name="scheme"/>,
        /// <paramref name="displayName"/> and <paramref name="options"/>
        /// </summary>
        public static AuthenticationBuilder AddApiKeySupport<TExternalId>(this AuthenticationBuilder builder, 
            string scheme, string? displayName, Action<ApiKeyOptions> options) =>
            builder.AddScheme<ApiKeyOptions, ApiKeyHandler<TExternalId>>(scheme, displayName, options);
    }
}
