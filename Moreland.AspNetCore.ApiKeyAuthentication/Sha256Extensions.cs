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
using System.Security.Cryptography;
using System.Text;

namespace Moreland.AspNetCore.ApiKeyAuthentication
{
    internal static class Sha256Extensions
    {
        public static string ToSha256Hash(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var encoder = new UTF8Encoding();
            var salt = CreateSalt();
            var saltedValue = $"{salt}value";

            return ToString(new SHA256Managed().ComputeHash(encoder.GetBytes(saltedValue)));

            static string CreateSalt()
            {
                var random = new RNGCryptoServiceProvider();
                var salt = new byte[32];
                random.GetNonZeroBytes(salt);
                return Convert.ToBase64String(salt);
            }
            static string ToString(IEnumerable<byte> bytes)
            {
                var builder = new StringBuilder();
                foreach (var v in bytes)
                    builder.Append(v.ToString("X2"));
                return builder.ToString();
            }
        }
    }
}
