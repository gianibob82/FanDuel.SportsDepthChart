using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public class AddPlayerToDepthChartCommand : BaseCommandQuery, IRequest<int>
    {
        public string Position { get; set; }

        public int PlayerNumber { get; set; }

        public string PlayerName { get; set; }

        public int? PositionDepth { get; set; }
    }

    public class AddPlayerToDepthChartCommandHandler : IRequestHandler<AddPlayerToDepthChartCommand, int>
    {
        private readonly ISportsDepthChartContext _context;

        public AddPlayerToDepthChartCommandHandler(ISportsDepthChartContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(AddPlayerToDepthChartCommand request, CancellationToken cancellationToken)
        {
            var player = _context.Players.SingleOrDefault(p => p.TeamEntityId == request.TeamId && p.Id == request.PlayerNumber);

            if (player == null)
            {
                // player doesn't exists yet
                _context.Players.Add(new Player.PlayerEntity { 
                     Id = request.PlayerNumber,
                      Name = request.PlayerName,
                       TeamEntityId = request.TeamId
                });
            }


            if (request.PositionDepth.HasValue)
            {
                // Adds a player to the depth chart at a given position
                // The added player would get priority. Anyone below that player in the depth chart would get moved down a  position_depth
                var chartdepthPositions = _context.PlayerChartPlacements.Where(p => p.Position == request.Position && p.PositionDepth >= request.PositionDepth.Value && p.ChartId == request.ChartId).OrderBy(p => p.PositionDepth);
                foreach (var positions in chartdepthPositions)
                {
                    positions.PositionDepth += 1;
                }

                _context.PlayerChartPlacements.Add(new PlayerChartPlacement.PlayerChartPlacementEntity
                {
                    Position = request.Position,
                    ChartId = request.ChartId,
                    PlayerId = request.PlayerNumber,
                    PositionDepth = request.PositionDepth.Value
                });
            }
            else {
                // Adding a player without a position_depth would add them to the end of the depth chart at that position
                var chartdepth = _context.PlayerChartPlacements.Where(p => p.Position == request.Position && p.ChartId == request.ChartId).OrderByDescending(p => p.PositionDepth).FirstOrDefault();

                _context.PlayerChartPlacements.Add(new PlayerChartPlacement.PlayerChartPlacementEntity
                {
                    Position = request.Position,
                    ChartId = request.ChartId,
                    PlayerId = request.PlayerNumber,
                    PositionDepth = chartdepth == null ? 1 : chartdepth.PositionDepth + 1
                });
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
