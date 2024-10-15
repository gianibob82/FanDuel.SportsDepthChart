using FanDuel.SportsDepthChart.Core.Chart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FanDuel.SportsDepthChart.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChartDepthController : ControllerBase
    {
        private readonly ILogger<ChartDepthController> _logger;
        private readonly IMediator _mediator;
        public ChartDepthController(ILogger<ChartDepthController> logger, IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "AddPlayer")]
        public Task<int> AddPlayer(AddPlayerToDepthChartCommand command)
        {
            return _mediator.Send(command);
        }

        [HttpDelete(Name = "RemovePlayer")]
        public Task<List<PlayerDepthChartResponse>> RemovePlayer(RemovePlayerFromDepthChartCommand command)
        {
            var result = _mediator.Send(command);
            return result;
        }

        [HttpGet("GetPlayerBackups")]
        public Task<List<PlayerDepthChartResponse>> GetPlayerBackups([FromQuery] GetBackupsQuery query)
        {
            var result = _mediator.Send(query);
            return result;
        }

        [HttpGet("GetFullDepthChart")]
        public Task<Dictionary<string, List<PlayerDepthChartResponse>>> GetFullDepthChart()
        {
            var query = new GetFullDepthChartQuery();
            var result = _mediator.Send(query);
            return result;
        }
    }
}
