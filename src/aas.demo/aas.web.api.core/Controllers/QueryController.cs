using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aas.web.api.Controllers
{
    [ApiController]
    [Route("query")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> logger;

        public QueryController(ILogger<QueryController> logger) => this.logger = logger;

        [HttpGet]
        public string Get()
        {
            logger.LogInformation("Calling get data");
            var rng = new Random();
            return rng.ToString();
        }
    }
}