using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public class GetBackupsQuery : BaseCommandQuery, IRequest<List<RemovePlayerFromDepthChartCommandResponse>>
    {
        public string Position { get; set; }

        public int PlayerNumber { get; set; }
    }

    public class GetBackupsQueryHandler : IRequestHandler<GetBackupsQuery, List<RemovePlayerFromDepthChartCommandResponse>>
    {
        private readonly ISportsDepthChartContext _context;

        public GetBackupsQueryHandler(ISportsDepthChartContext context)
        {
            _context = context;
        }

        public async Task<List<RemovePlayerFromDepthChartCommandResponse>> Handle(GetBackupsQuery request, CancellationToken cancellationToken)
        {
            var playerInPosition = await _context.PlayerChartPlacements.SingleOrDefaultAsync(p => p.Position == request.Position && p.Player.Id == request.PlayerNumber && p.ChartId == request.ChartId);

            var result = new List<RemovePlayerFromDepthChartCommandResponse>();

            // An empty list should be returned if the given player is not listed in the depth chart at that position
            if (playerInPosition == null)
                return result;

            var playerBackups = _context.PlayerChartPlacements.Where(p => p.Position == request.Position && p.PositionDepth > playerInPosition.PositionDepth && p.ChartId == request.ChartId);

            // An empty list should be returned if the given player has no Backups 
            if (!playerBackups.Any())
                return result;

            foreach (var playerBackup in playerBackups)
            {
                result.Add(new RemovePlayerFromDepthChartCommandResponse
                {
                    PlayerNumber = playerBackup.PlayerId,
                    //PlayerName = playerBackup.Player.Name,
                    PlayerChartDepth = playerBackup.PositionDepth,
                    PlayerPosition = playerBackup.Position
                });
            }

            return result;
        }
    }
}
