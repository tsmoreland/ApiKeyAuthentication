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
using Moreland.AspNetCore.ApiKeyAuthentication;

namespace Moreland.AspNetCore.SampleApi.Data
{
    public static class TestApiKeys
    {
        public static IEnumerable<ApiKey<Guid>> Keys
        {
            get
            {
                yield return ApiKey.CreateTestData(new Guid("D0944969-D442-4183-8F7B-05DFA6915E4B"), "charlie-app", "B47900BB1C704100B60063D1B84B287F",
                    new Guid("FF15FCE1-1512-44C0-894B-5A5980F35185"), new DateTime(2019, 06, 01), new[] {"read", "write"});

                yield return ApiKey.CreateTestData(new Guid("DF17DF48-EB48-46A7-8566-1E243284BCDB"), "beta-app", "BE2E4B48DE6D4277BD17EA52884E380B",
                    new Guid("FF15FCE1-1512-44C0-894B-5A5980F35185"),  new DateTime(2019, 08, 01), new[] {"manage", "read"});

                yield return ApiKey.CreateTestData(new Guid("CC6EBE75-F2C9-4DE2-82AA-66AFB3B8D492"), "alpha-app", "0C1CE9B860B242FBA5FDF277E65CCCE4",
                    new Guid("6EEB9975-A2D9-4C83-8D83-8BB925680D7A"), new DateTime(2020, 01, 01), new[] {"manage", "create", "read", "update", "delete"});
            }
        }
    }
}
