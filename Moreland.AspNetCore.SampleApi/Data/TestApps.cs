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
using System.Security.Claims;
using Moreland.AspNetCore.SampleApi.Models;

namespace Moreland.AspNetCore.SampleApi.Data
{
    public static class TestApps
    {
        public static IEnumerable<App> Apps
        {
            get
            {
                yield return App.CreateTestData(new Guid("6EEB9975-A2D9-4C83-8D83-8BB925680D7A"), "Alpha",
                    new[] { ToPair("group", "api-key"), ToPair("group", "app"), ToPair(ClaimTypes.Role, "manager")});
                yield return App.CreateTestData(new Guid("15541E86-E915-4D1D-B110-D73C6C5B1880"), "Beta",
                    new[] { ToPair("group", "app"), ToPair(ClaimTypes.Role, "manager")});

                yield return App.CreateTestData(new Guid("FF15FCE1-1512-44C0-894B-5A5980F35185"), "Charlie",
                    new[] { ToPair("group", "api-key"), ToPair(ClaimTypes.Role, "viewer")});
                yield return App.CreateTestData(new Guid("B8EF9F7A-35FA-4D25-8FFE-1157CC9A4F70"), "Delta",
                    new[] { ToPair("group", "app"), ToPair(ClaimTypes.Role, "user")});
            }
        }

        private static KeyValuePair<string, string> ToPair(string key, string value) =>
            new KeyValuePair<string, string>(key, value);
    }
}
