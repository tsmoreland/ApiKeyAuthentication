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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moreland.AspNetCore.ApiKeyAuthentication;
using Moreland.AspNetCore.ApiKeyAuthentication.Data;
using Moreland.AspNetCore.SampleApi.Models;

namespace Moreland.AspNetCore.SampleApi.Controllers
{
    [ApiController]
    [Route("AppManager")]
    [Authorize(Policy = "RequiresManage")]
    public class AppManagerController : ControllerBase
    {
        private readonly IApiKeyRepository<Guid> _apiKeyRepository;
        private readonly ILogger<AppManagerController> _logger;

        /// <summary>
        /// Instantiates a new instance of the <see cref="AppManagerController"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="apiKeyRepository"/> or <paramref name="logger"/> is null
        /// </exception>
        public AppManagerController(IApiKeyRepository<Guid> apiKeyRepository, ILogger<AppManagerController> logger)
        {
            _apiKeyRepository = apiKeyRepository ?? throw new ArgumentNullException(nameof(apiKeyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetAppKey(Guid? id)
        {
            if (!id.HasValue || id == Guid.Empty)
                return BadRequest();

            var app = await _apiKeyRepository.GetByIdAsync(id.Value);

            return app.IsNullorEmpty()
                ? (ActionResult<string>) NotFound()
                : Ok(app.Key);
        }

        [HttpPost]
        [Authorize(Policy = "CrudManager")]
        public async Task<ActionResult> CreateApp(CreateApiKeyModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var (owner, roles) = model;

            if (string.IsNullOrWhiteSpace(owner))
                return BadRequest();

            var arrayOfRoles = roles?.ToArray() ?? Array.Empty<string>();
            
            // if there were more than a quick sample we'd verify the roles were expected values,
            // possibly even using an enum to represent valid values

            if (arrayOfRoles?.Any() != true)
                return BadRequest();

            var (id, key) = await _apiKeyRepository.CreateAsync(owner, model.AppId, arrayOfRoles);

            return string.IsNullOrEmpty(key) || id == Guid.Empty
                ? (ActionResult)new StatusCodeResult(500) // error should've been logged in create
                : Created(new Uri($"/App/{id}", UriKind.Relative), key);
        }
    }
}
