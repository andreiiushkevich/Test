using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Test.Common.Contracts;
using Test.Common.Dtos;
using Test.Receiver.Hubs;

namespace Test.Receiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IHubContext<ContractHub> _hubContext;
        private readonly IRequestClient<ContractDto> _requestClient;

        public ContractController(IRequestClient<ContractDto> requestClient, IHubContext<ContractHub> hubContext)
        {
            _requestClient = requestClient;
            _hubContext = hubContext;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ContractDto contract, CancellationToken cancellationToken)
        {
            var request = _requestClient.Create(contract, cancellationToken);
            var response = await request.GetResponse<IOperationResult<OperationStatus>>();
            var result = response.Message.Data;
            await _hubContext.Clients.All.SendAsync("ContractProcessed", result.Message);

            if (result.Success)
                return Ok();

            return BadRequest();
        }
    }
}