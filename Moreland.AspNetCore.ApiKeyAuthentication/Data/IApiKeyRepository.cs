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
using System.Threading.Tasks;

namespace Moreland.AspNetCore.ApiKeyAuthentication.Data
{
    /// <summary>
    /// Api-Key Repository interface
    /// </summary>
    public interface IApiKeyRepository<TConsumerId>
    {
        /// <summary>
        /// Get Api-Key entry by id
        /// </summary>
        public Task<ApiKey<TConsumerId>> GetByIdAsync(Guid id);
        /// <summary>
        /// Get Api-Key entry by key
        /// </summary>
        /// <remarks>
        /// Database should index by this value as this is the most likely search criteria
        /// </remarks>
        public Task<ApiKey<TConsumerId>> GetByKeyAsync(string key);
        /// <summary>
        /// Remove by Id
        /// </summary>
        public Task RemoveByIdAsync(Guid id);
        /// <summary>
        /// Remove by Key
        /// </summary>
        public Task RemoveByKeyAsync(string key);

        /// <summary>
        /// Remove all Api-Key entries for <paramref name="owner"/>
        /// </summary>
        /// <param name="owner"></param>
        public Task RemoveAllForOwnerAsync(string owner);

        /// <summary>
        /// Create a new Api-Key for <paramref name="owner"/> with <paramref name="roles"/>
        /// </summary>
        /// <returns>The generated Api-Key</returns>
        public Task<(Guid id, string key)> CreateAsync(string owner, TConsumerId externalId, IEnumerable<string> roles);
    }
}
