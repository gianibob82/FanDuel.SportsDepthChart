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
    public class GetBackupsQueryTests
    {
        const int teamId = 1;
        const int chartId = 1;
        [Fact]
        public async Task ReturnBackupsForPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 13, Position = "QB",  PositionDepth = 1 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 12, Position = "QB",  PositionDepth = 2 }
                , new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 3 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 12, Name = "Tom Brady", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();
            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = 12,
                Position = "QB",
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            Assert.Single(backups);
            Assert.True(backups.All(b => b.PlayerChartDepth > 2));
        }

        [Fact]
        public async Task ReturnNoBackupsIfPlayerNotInPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 13, Position = "QB",  PositionDepth = 1 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 12, Position = "QB",  PositionDepth = 2 }
                , new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 3 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 12, Name = "Tom Brady", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();
            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = 12,
                Position = "LT",
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            Assert.Empty(backups);
        }

        [Fact]
        public async Task ReturnNoBackupsWhenNoneAreInDepth()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 13, Position = "QB",  PositionDepth = 1 },
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 12, Position = "QB",  PositionDepth = 2 }
                , new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 3 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 12, Name = "Tom Brady", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();
            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = 18,
                Position = "QB",
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            Assert.Empty(backups);
        }
    }
}