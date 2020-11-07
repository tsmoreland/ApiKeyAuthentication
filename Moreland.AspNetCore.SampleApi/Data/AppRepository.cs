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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Moreland.AspNetCore.SampleApi.Models;

namespace Moreland.AspNetCore.SampleApi.Data
{
    public sealed class AppRepository
    {
        private readonly ConcurrentDictionary<Guid, App> _appsById;

        public AppRepository()
        {
            _appsById = new ConcurrentDictionary<Guid, App>();
        }

        public App? Create(string name, IEnumerable<KeyValuePair<string, string>> claims)
        {
            var app = App.Create(name, claims);
            return _appsById.TryAdd(app.Id, app)
                ? app
                : null;
        }
        public App? FindById(Guid id) =>
            _appsById.TryGetValue(id, out var app)
                ? app
                : null;

        public void Update(App app) =>
            _appsById.AddOrUpdate(
                app.Id, 
                key => app, 
                (key, oldValue) => app);

        public bool Remove(App app) =>
            app != null! && Remove(app.Id);

        public bool Remove(Guid id) =>
            _appsById.TryRemove(id, out _);

    }
}
