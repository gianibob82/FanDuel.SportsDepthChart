using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public class GetFullDepthChartQuery : BaseCommandQuery, IRequest<Dictionary<string, List<PlayerDepthChartResponse>>>
    {
    }

    public class GetFullDepthChartQueryHandler : IRequestHandler<GetFullDepthChartQuery, Dictionary<string, List<PlayerDepthChartResponse>>>
    {
        private readonly ISportsDepthChartContext _context;

        public GetFullDepthChartQueryHandler(ISportsDepthChartContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, List<PlayerDepthChartResponse>>> Handle(GetFullDepthChartQuery request, CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, List<PlayerDepthChartResponse>>();

            var allPlayersInChart = _context.PlayerChartPlacements.AsNoTracking().Where(c => c.ChartId == request.ChartId).ToList();

            foreach (var position in allPlayersInChart.DistinctBy(p => p.Position).Select(p => p.Position))
            {
                var positionPlayers = new List<PlayerDepthChartResponse>();

                var playerBackups = from chart in allPlayersInChart
                                    join player in _context.Players on chart.PlayerId equals player.Id
                                    where chart.Position == position
                                    orderby chart.PositionDepth
                                    select new
                                    {
                                        chart.Position,
                                        chart.PositionDepth,
                                        chart.PlayerId,
                                        player.Name
                                    };

                foreach (var player in playerBackups)
                {
                    positionPlayers.Add(new PlayerDepthChartResponse
                    {
                        PlayerChartDepth = player.PositionDepth,
                        PlayerName = player.Name,
                        PlayerNumber = player.PlayerId
                    });
                }

                result.Add(position, positionPlayers);
            }

            return result;
        }
    }
}
