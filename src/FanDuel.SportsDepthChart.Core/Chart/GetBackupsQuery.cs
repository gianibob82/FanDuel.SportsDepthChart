using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public class GetBackupsQuery : BaseCommandQuery, IRequest<List<PlayerDepthChartResponse>>
    {
        public string Position { get; set; }

        public int PlayerNumber { get; set; }
    }

    public class GetBackupsQueryHandler : IRequestHandler<GetBackupsQuery, List<PlayerDepthChartResponse>>
    {
        private readonly ISportsDepthChartContext _context;

        public GetBackupsQueryHandler(ISportsDepthChartContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerDepthChartResponse>> Handle(GetBackupsQuery request, CancellationToken cancellationToken)
        {
            var playerInPosition = _context.PlayerChartPlacements.SingleOrDefault(p => p.Position == request.Position && p.PlayerId == request.PlayerNumber && p.ChartId == request.ChartId);

            var result = new List<PlayerDepthChartResponse>();

            // An empty list should be returned if the given player is not listed in the depth chart at that position
            if (playerInPosition == null)
                return result;

            var playerBackups = from chart in _context.PlayerChartPlacements
                        join player in _context.Players on chart.PlayerId equals player.Id
                        where chart.Position == request.Position && chart.PositionDepth > playerInPosition.PositionDepth && chart.ChartId == request.ChartId
                        select new
                        {
                            chart.Position,
                            chart.PositionDepth,
                            chart.PlayerId,
                            player.Name
                        };

            // An empty list should be returned if the given player has no Backups 
            if (!playerBackups.Any())
                return result;

            foreach (var playerBackup in playerBackups)
            {
                result.Add(new PlayerDepthChartResponse
                {
                    PlayerNumber = playerBackup.PlayerId,
                    PlayerName = playerBackup.Name,
                    PlayerChartDepth = playerBackup.PositionDepth,
                    PlayerPosition = playerBackup.Position
                });
            }

            return result;
        }
    }
}
