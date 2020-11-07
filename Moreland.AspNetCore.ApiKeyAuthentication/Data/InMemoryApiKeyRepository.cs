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
using System.Threading.Tasks;

namespace Moreland.AspNetCore.ApiKeyAuthentication.Data
{
    /// <summary>
    /// In Memory implementation of Api-Key repository
    /// </summary>
    public sealed class InMemoryApiKeyRepository<TExternalId> : IApiKeyRepository<TExternalId>
    {
        private readonly object _locker;
        private readonly Dictionary<Guid, ApiKey<TExternalId>> _apiKeyById;
        private readonly Dictionary<string, Guid> _idByKey;

        public InMemoryApiKeyRepository()
        {
            _locker = new object();
            _apiKeyById = new Dictionary<Guid, ApiKey<TExternalId>>();
            _idByKey = new Dictionary<string, Guid>();
        }

        public InMemoryApiKeyRepository(IEnumerable<ApiKey<TExternalId>> apiKeys)
        {
            _locker = new object();
            _apiKeyById = new Dictionary<Guid, ApiKey<TExternalId>>();
            _idByKey = new Dictionary<string, Guid>();

            foreach (var apiKey in apiKeys)
            {
                _apiKeyById[apiKey.Id] = apiKey;
                _idByKey[apiKey.Key] = apiKey.Id;
            }
        }

        /// <inheritdoc/>
        public Task<(Guid id, string key)> CreateAsync(string owner, TExternalId externalId, IEnumerable<string> roles)
        {
            var apiKey = ApiKey.GenerateKey();
            var apiKeyEntry = new ApiKey<TExternalId>(Guid.NewGuid(), owner, apiKey, externalId, DateTime.UtcNow, roles);

            lock (_locker)
            {
                _apiKeyById[apiKeyEntry.Id] = apiKeyEntry;
                _idByKey[apiKeyEntry.Key] = apiKeyEntry.Id;
            }

            return Task.FromResult((apiKeyEntry.Id, apiKey));
        }

        /// <inheritdoc/>
        public Task<ApiKey<TExternalId>> GetByIdAsync(Guid id)
        {
            lock (_locker)
                return Task.FromResult(_apiKeyById.TryGetValue(id, out var apiKey)
                    ? apiKey
                    : ApiKey.Empty<TExternalId>());

        }

        /// <inheritdoc/>
        public Task<ApiKey<TExternalId>> GetByKeyAsync(string key)
        {
            lock (_locker)
            {
                return Task.FromResult(_idByKey.TryGetValue(key, out var id) &&
                                       _apiKeyById.TryGetValue(id, out var apiKey)
                    ? apiKey
                    : ApiKey.Empty<TExternalId>());
            }
        }

        /// <inheritdoc/>
        public Task RemoveAllForOwnerAsync(string owner)
        {
            lock (_locker)
            {
                var idsToRemove = _apiKeyById.Values
                    .Where(apiKey => apiKey.Owner == owner)
                    .Select(apiKey => apiKey.Id)
                    .ToList();
                var keysToRemove = idsToRemove.Select(id => _apiKeyById[id].Key).ToList();

                foreach (var id in idsToRemove)
                    _apiKeyById.Remove(id);
                foreach (var key in keysToRemove)
                    _idByKey.Remove(key);
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveByIdAsync(Guid id)
        {
            lock (_locker)
            {
                if (!_apiKeyById.TryGetValue(id, out var apiKey))
                    return Task.CompletedTask;
                _apiKeyById.Remove(id);
                if (_idByKey.ContainsKey(apiKey.Key))
                    _idByKey.Remove(apiKey.Key);
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveByKeyAsync(string key)
        {
            lock (_locker)
            {
                if (!_idByKey.TryGetValue(key, out var id))
                    return Task.CompletedTask;
                _idByKey.Remove(key);
                if (_apiKeyById.ContainsKey(id))
                    _apiKeyById.Remove(id);
            }
            return Task.CompletedTask;
        }
    }
}
