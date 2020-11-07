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
using System.Diagnostics.CodeAnalysis;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    public static class ApiKey
    {
        /// <summary>
        /// Creates a new ApiKey with given id, intended for test data
        /// </summary>
        public static ApiKey<TConsumerId> CreateTestData<TConsumerId>(Guid id, string consumer, string key, TConsumerId externalId, DateTime created,
            IEnumerable<string> roles) =>
            new ApiKey<TConsumerId>(id, consumer, key, externalId, created, roles);

        /// <summary>
        /// Generates a new api-key
        /// </summary>
        public static string GenerateKey() =>
            Guid.NewGuid().ToString("N").ToSha256Hash();


        /// <summary>
        /// Empty Representation
        /// </summary>
        public static ApiKey<TConsumerId> Empty<TConsumerId>() => ApiKey<TConsumerId>.Empty;

    }

    /// <summary>
    /// Api Key Data Structure
    /// </summary>
    public sealed class ApiKey<TConsumerId> : IEquatable<ApiKey<TConsumerId>>
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="ApiKey{TConsumerId}"/> class.
        /// </summary>
        /// <param name="consumer"/>
        /// <param name="key"/>
        /// <param name="externalId"/>
        /// <param name="created"/>
        /// <param name="roles"/>
        public ApiKey(string consumer, string key, TConsumerId externalId, DateTime created, IEnumerable<string> roles)
            : this(Guid.NewGuid(), consumer, key, externalId, created, roles)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="ApiKey{TConsumerId}"/> class.
        /// </summary>
        /// <param name="id"/>
        /// <param name="consumer"/>
        /// <param name="key"/>
        /// <param name="consumerId"/>
        /// <param name="created"/>
        /// <param name="roles"/>
        /// <remarks>
        /// This method should be made internal and used for test purposes or test-data purposes
        /// </remarks>
        internal ApiKey(Guid id, string consumer, string key, TConsumerId consumerId, DateTime created, IEnumerable<string> roles)
        {
            Id = id;
            Consumer = consumer ?? string.Empty;
            Key = key ?? string.Empty;
            ConsumerId = consumerId;
            Created = created;

            var claimsList = new List<string>(roles ?? Array.Empty<string>());
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
        /// Application Name of the caller
        /// </summary>
        public string Consumer { get; private set; } = string.Empty;

        /// <summary>
        /// The Api Key
        /// </summary>
        public string Key { get; private set; } = string.Empty;

        /// <summary>
        /// Extenal Id a link to the external identity 
        /// </summary>
        public TConsumerId ConsumerId { get; set; } = default!;

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
        /// Returns <c>true</c> is ApiKey is empty
        /// </summary>
        public bool IsEmpty => 
            Id == Guid.Empty && string.IsNullOrEmpty(Key);

        /// <summary>
        /// Empty Representation
        /// </summary>
        public static ApiKey<TConsumerId> Empty => new ApiKey<TConsumerId>();

        /// <inheritdoc/>
        public bool Equals([AllowNull] ApiKey<TConsumerId> other) =>
            !(other is null) && Id.Equals(other.Id);

        /// <inheritdoc cref="object.Equals(object?)"/>
        public override bool Equals(object? obj) =>
            Equals(obj as ApiKey<TConsumerId>);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() =>
            // ReSharper disable once NonReadonlyMemberInGetHashCode -- Id can only be change
            2108858624 + Id.GetHashCode();

    }
}
