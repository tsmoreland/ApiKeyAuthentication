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
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Moreland.AspNetCore.ApiKeyAuthentication.Helpers
{
    /// <summary>
    /// Helper methods to aid with accessing <see cref="HttpRequest"/>
    /// </summary>
    internal static class HttpRequestHelpers
    {
        private static MediaTypeHeaderValue JsonTypeHeadervalue { get; } =
            new MediaTypeHeaderValue(MediaTypeNames.Application.Json);

            private static MediaTypeHeaderValue XmlTypeHeaderValue { get; } =
                new MediaTypeHeaderValue(MediaTypeNames.Application.Xml);

            /// <summary>
        /// Determines appropriate response type based on Accept type
        /// </summary>
        /// <param name="request"/>
        /// <returns>
        /// matching response type defaulting with application/json if none found
        /// </returns>
        public static string GetResponseTypeFromRequest(HttpRequest request)
        {
            if (request == null!)
                throw new ArgumentNullException(nameof(request));

            var acceptHeaders = request.GetTypedHeaders().Accept;

            bool acceptJson = false;
            bool acceptXml = false;
            foreach (var acceptHeader in acceptHeaders)
            {
                acceptJson |= acceptHeader.IsSubsetOf(JsonTypeHeadervalue);
                acceptXml |= acceptHeader.IsSubsetOf(XmlTypeHeaderValue);
            }

            return acceptXml && !acceptJson
                ? MediaTypeNames.Application.Xml
                : MediaTypeNames.Application.Json;
        }
    }
}
