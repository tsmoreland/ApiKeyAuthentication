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
using System.Collections.ObjectModel;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    /// <summary>
    /// Api Key Data Structure
    /// </summary>
    public sealed class ApiKey
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="ApiKey"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <param name="key"></param>
        /// <param name="created"></param>
        /// <param name="scopes"></param>
        public ApiKey(Guid id, string owner, string key, DateTime created, IEnumerable<string> scopes)
        {
            Id = id;
            Owner = owner ?? string.Empty;
            Key = key ?? string.Empty;
            Created = created;

            var claimsList = new List<string>(scopes ?? Array.Empty<string>());
            Roles = new ReadOnlyCollection<string>(claimsList);
        }

        /// <summary>
        /// Private Constructor to allow use of Entity Framework
        /// </summary>
        private ApiKey()
        {
            // ... required if EF is used
        }

        /// <summary>
        /// Unique Identifier of the ApiKey
        /// </summary>
        public Guid Id { get; private set; } = Guid.Empty;

        /// <summary>
        /// Owner (possibly a user) of the Api Key
        /// </summary>
        public string Owner { get; private set; } = string.Empty;

        /// <summary>
        /// The Api Key
        /// </summary>
        public string Key { get; private set; } = string.Empty;

        /// <summary>
        /// Creation Date/Time of the ApiKey
        /// </summary>
        /// <remarks>this should represent a UTC Time/date</remarks>
        public DateTime Created { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// User Roles
        /// </summary>
        public IReadOnlyCollection<string> Roles { get; private set; } =
            new ReadOnlyCollection<string>(new List<string>());

        /// <summary>
        /// Empty Representation
        /// </summary>
        public static ApiKey Empty { get; } = new ApiKey();

        /// <inheritdoc cref="object.Equals(object?)"/>
        public override bool Equals(object? obj) =>
            obj is ApiKey key && Id.Equals(key.Id);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() =>
            // ReSharper disable once NonReadonlyMemberInGetHashCode -- Id can only be change
            2108858624 + Id.GetHashCode();
    }
}
