using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public class GetFullDepthChartQuery : BaseCommandQuery, IRequest<Dictionary<string, List<RemovePlayerFromDepthChartCommandResponse>>>
    {
    }

    public class GetFullDepthChartQueryHandler : IRequestHandler<GetFullDepthChartQuery, Dictionary<string, List<RemovePlayerFromDepthChartCommandResponse>>>
    {
        private readonly ISportsDepthChartContext _context;

        public GetFullDepthChartQueryHandler(ISportsDepthChartContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, List<RemovePlayerFromDepthChartCommandResponse>>> Handle(GetFullDepthChartQuery request, CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, List<RemovePlayerFromDepthChartCommandResponse>>();

            var allPlayersInChart = _context.PlayerChartPlacements.Where(c => c.ChartId == request.ChartId).ToList();

            foreach (var position in allPlayersInChart.DistinctBy(p => p.Position).Select(p => p.Position))
            {
                var positionPlayers = new List<RemovePlayerFromDepthChartCommandResponse>();

                foreach (var player in allPlayersInChart.Where(p => p.Position == position).OrderBy(p => p.PositionDepth)) {

                    positionPlayers.Add(new RemovePlayerFromDepthChartCommandResponse { 
                     PlayerChartDepth = player.PositionDepth,
                         //PlayerName = player.Player.Name,
                          PlayerNumber = player.PlayerId
                    });
                }

                result.Add(position, positionPlayers);
            }

            return result;
        }
    }
}
