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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Moreland.AspNetCore.SampleApi.Models
{
    public sealed class App : IEquatable<App>
    {
        public App(string name, IEnumerable<KeyValuePair<string, string>> claims)
            : this(Guid.NewGuid(), name, claims)
        {
        }

        private App(Guid id, string name, IEnumerable<KeyValuePair<string, string>> claims)
        {
            Id = id;
            Name = name;
            Claims = claims?.ToList() ?? new List<KeyValuePair<string, string>>();
        }

        private App()
        {
            // needed for entity framework
        }

        public static App Create(string name, IEnumerable<KeyValuePair<string, string>> claims) =>
            new App(name, claims);

        internal static App CreateTestData(Guid id, string name, IEnumerable<KeyValuePair<string, string>> claims) =>
            new App(id, name, claims);



        public Guid Id { get; private set; } = Guid.Empty;
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Claims
        /// </summary>
        public List<KeyValuePair<string, string>> Claims { get; private set; } =
            new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Empty Representation
        /// </summary>
        public static App Empty { get; } = new App();

        /// <summary>
        /// Returns <c>true</c> is ApiKey is empty
        /// </summary>
        public bool IsEmpty =>
            Id == default;

        /// <inheritdoc/>
        public bool Equals([AllowNull] App other) =>
            other != null && Id.Equals(other.Id);

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            Equals(obj as App);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            HashCode.Combine(Id);
    }
}
