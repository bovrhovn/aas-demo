using System;
using System.Collections.Generic;
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
        public async Task<string> Get()
        {
            logger.LogInformation("Authenticating....");
            var authData = await queryService.AuthenticateAsync();
            logger.LogInformation($"Authenticated with bearer token - {authData.PasswordOrToken}");
            logger.LogInformation("Executing query...");
            var query = @"
                    EVALUATE
                      TOPN(
                        1001,
                        SUMMARIZECOLUMNS('Product'[Name], ""SumListPrice"", CALCULATE(SUM('Product'[ListPrice]))),
                        [SumListPrice],
                        0,
                        'Product'[Name],
                        1
                      )

                    ORDER BY
                      [SumListPrice] DESC, 'Product'[Name]
                    ";
            var result = await queryService.QueryAsync(authData, new Dictionary<string, string>
            {
                {"query",query}
            });

            return result == null ? "Empty database" : Encoding.UTF8.GetString(result);
        }
    }
}