using FanDuel.SportsDepthChart.Core.Player;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public class RemovePlayerFromDepthChartCommand : BaseCommandQuery, IRequest<List<RemovePlayerFromDepthChartCommandResponse>>
    {
        public string Position { get; set; }

        public int PlayerNumber { get; set; }
    }

    public class RemovePlayerFromDepthChartCommandHandler : IRequestHandler<RemovePlayerFromDepthChartCommand, List<RemovePlayerFromDepthChartCommandResponse>>
    {
        private readonly ISportsDepthChartContext _context;

        public RemovePlayerFromDepthChartCommandHandler(ISportsDepthChartContext context)
        {
            _context = context;
        }
        public async Task<List<RemovePlayerFromDepthChartCommandResponse>> Handle(RemovePlayerFromDepthChartCommand request, CancellationToken cancellationToken)
        {
            var playerToRemove = _context.PlayerChartPlacements.SingleOrDefault(p => p.Position == request.Position && p.Player.Id == request.PlayerNumber && p.ChartId == request.ChartId);

            var result = new List<RemovePlayerFromDepthChartCommandResponse>();

            // An empty list should be returned if that player is not listed in the depth chart at that position
            if (playerToRemove == null)
                return result;

            // Removes a player from the depth chart for a given position and returns that player
            _context.PlayerChartPlacements.Remove(playerToRemove);

            var chartdepthPositions = _context.PlayerChartPlacements.Where(p => p.Position == request.Position && p.PositionDepth > playerToRemove.PositionDepth && p.ChartId == request.ChartId).OrderBy(p => p.PositionDepth);
            foreach (var positions in chartdepthPositions)
            {
                positions.PositionDepth -= 1;
            }

            await _context.SaveChangesAsync(cancellationToken);

            result.Add(new RemovePlayerFromDepthChartCommandResponse
            {
                PlayerNumber = request.PlayerNumber,
                //PlayerName = playerToRemove.Player.Name,
                PlayerChartDepth = playerToRemove.PositionDepth,
                PlayerPosition = playerToRemove.Position
            });

            return result;
        }
    }

    public class RemovePlayerFromDepthChartCommandResponse
    {
        public int PlayerNumber { get; set; }
        //public string PlayerName { get; set; }
        public string PlayerPosition { get; set; }
        public int PlayerChartDepth { get; set; }
    }
}
