using FanDuel.SportsDepthChart.Core;
using FanDuel.SportsDepthChart.Core.Chart;
using FanDuel.SportsDepthChart.Core.Player;
using FanDuel.SportsDepthChart.Core.PlayerChartPlacement;
using FanDuel.SportsDepthChart.Core.Sport;
using FanDuel.SportsDepthChart.Core.Team;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.EntityFrameworkCore;

namespace FanDuel.SportsDepthChart.UnitTests
{
    public class GetFullDepthChartQueryTests
    {
        const int teamId = 1;
        const int chartId = 1;
        [Fact]
        public async Task ReturnBackupsForPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 13, Position = "QB",  PositionDepth = 1 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 12, Position = "QB",  PositionDepth = 2 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 3 },

                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 7, Position = "RB",  PositionDepth = 1 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 27, Position = "RB",  PositionDepth = 2 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 21, Position = "RB",  PositionDepth = 3 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 12, Name = "Tom Brady", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},

                new PlayerEntity{ Id = 7, Name = "Leonard Fournette", TeamEntityId = teamId},
                new PlayerEntity{ Id = 27, Name = "Ronald James", TeamEntityId = teamId},
                new PlayerEntity{ Id = 21, Name = "KeShawn Vaugh", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();
            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            GetFullDepthChartQuery query = new GetFullDepthChartQuery();

            GetFullDepthChartQueryHandler handler = new GetFullDepthChartQueryHandler(mockContext.Object);
            var chart = await handler.Handle(query, new System.Threading.CancellationToken());

            Assert.Equal(2, chart.Count);

            Assert.Equal(3, chart["QB"].Count);
            Assert.Equal(3, chart["RB"].Count);
        }
    }
}