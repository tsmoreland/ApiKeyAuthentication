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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Moreland.AspNetCore.SampleApi.Authentication
{
    public sealed class ApiKeyToUserClaimsTransformation : IClaimsTransformation
    {
        private readonly ILogger<ApiKeyToUserClaimsTransformation> _logger;

        /// <summary>
        /// Instantiates a new instance of the ApiKeyToUserClaimsTransformation class.
        /// </summary>
        /// <param name="logger"/>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="logger"/> is null
        /// </exception>
        public ApiKeyToUserClaimsTransformation(ILogger<ApiKeyToUserClaimsTransformation> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        /// <remarks>
        /// this would be more meaningful for an external Idp combined with a local user store, perhaps
        /// once that was setup to allow sigin but didn't offer a sign in page.
        /// It doesn't make much sense for Api-Key becuase at the moment it doesn't have a name
        /// identifier claim and because Api-Key typically represents an application not a user
        /// </remarks>
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identities.FirstOrDefault(i => i.IsAuthenticated);
            if (identity == null)
            {
                _logger.LogInformation("principal is not authenticated");
                return Task.FromResult(principal);
            }

            var idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                _logger.LogError("name identifier claim not found");
                return Task.FromResult(principal);
            }

            // this is where we would look up wihtin our own user store or something equivalent to fetch
            // user claims 

            return Task.FromResult(principal);
        }
    }
}
