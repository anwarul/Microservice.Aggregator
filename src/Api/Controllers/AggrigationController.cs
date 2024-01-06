using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedlimeSolutions.Microservice.Framework.ApiPipline;
using RS.MF.Aggregator.Application.Commands;
using RS.MF.Aggregator.Application.ServicesContracts;
using RS.MF.Aggregator.Core.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace RS.MF.ServiceName.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AggrigationController : ControllerBase
    {
        private readonly ILogger<AggrigationController> _logger;

        private readonly IHttpAggregateCallCommandHandler httpAggregateCallCommandHandler;

        public AggrigationController(
            ILogger<AggrigationController> logger,
            IHttpAggregateCallCommandHandler httpAggregateCallCommandHandler)
        {
            _logger = logger;
            this.httpAggregateCallCommandHandler = httpAggregateCallCommandHandler;
        }

        /// <summary>
        /// Execute aggregated call from multiple system touch-points.
        /// </summary>
        /// <remarks> <param name="command"></param> List of recipients and additional information:
        ///
        /// command. Uri: Uniform Resource Identifier of the aggregated call.
        ///
        /// command. Verb: The request method for executing call. For example - http Post, http Get etc.
        ///
        /// command. Payload: The payload content for executing call.
        ///
        /// command. SuccessIf: The criteria by which payload is accepted. If one entry value is deemed incorrect, the remaining entries (in sequence) will likely fail.
        ///
        /// </remarks>
        ///
        /// <returns></returns>
        [HttpPost("Execute")]
        [AnyonomusEndPoint]
        public Task<HttpAggregateCallResponse> Execute([FromBody] HttpAggregateCallCommand command, CancellationToken cancellationToken)
        {
            return httpAggregateCallCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}