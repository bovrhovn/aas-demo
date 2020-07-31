using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using aas.web.api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aas.web.api.Controllers
{
    [ApiController]
    [Route("query")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> logger;
        private readonly IQueryService queryService;

        public QueryController(ILogger<QueryController> logger, IQueryService queryService)
        {
            this.logger = logger;
            this.queryService = queryService;
        }

        [HttpGet]
        [Route("data/{query}")]
        public async Task<string> Get(string query)
        {
            logger.LogInformation("Authenticating....");
            
            var authData = await queryService.AuthenticateAsync();
            
            logger.LogInformation($"Authenticated with bearer token - {authData.PasswordOrToken}");
            logger.LogInformation($"Calling service with query data at {DateTime.Now}");
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var result = await queryService.QueryAsync(authData, new Dictionary<string, string>
            {
                {"query",query}
            });
            
            stopWatch.Stop();
            logger.LogInformation($"Done calling service...took {stopWatch.ElapsedMilliseconds} ms");

            return result == null ? "Empty database" : Encoding.UTF8.GetString(result);
        }
    }
}