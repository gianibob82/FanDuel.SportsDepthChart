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
    public class AddPlayerToDepthChartCommandTests
    {
        const int teamId = 1;
        const int chartId = 1;
        [Fact]
        public async Task AddPlayerToDepthChartAtGivenPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 13, Position = "QB",  PositionDepth = 1 }
                , new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 2 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();
            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            //player created if doesnt exist
            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            var command = new AddPlayerToDepthChartCommand()
            {
                PlayerName = "Tom Brady",
                PlayerNumber = 12,
                Position = "QB",
                PositionDepth = 1
            };

            AddPlayerToDepthChartCommandHandler handler = new AddPlayerToDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Once());
            mockContext.Verify(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()), Times.Once());

            // verify there are backups 
            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = command.PlayerNumber,
                Position = command.Position,
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            // there are 2 backups
            Assert.Equal(2, backups.Count);
            // The added player would get priority. Anyone below that player in the depth chart would get moved down a position_depth
            Assert.True(backups.All(b => b.PlayerChartDepth > 1));
        }

        [Fact]
        public async Task AddPlayerToDepthChartWithoutPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 13, Position = "QB",  PositionDepth = 1 }
                , new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 2 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();
            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            //player created if doesnt exist
            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            var command = new AddPlayerToDepthChartCommand()
            {
                PlayerName = "Tom Brady",
                PlayerNumber = 12,
                Position = "QB"
            };

            AddPlayerToDepthChartCommandHandler handler = new AddPlayerToDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Once());
            mockContext.Verify(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()), Times.Once());

            // verify there are no backups 
            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = command.PlayerNumber,
                Position = command.Position,
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            // there are no backups
            Assert.NotNull(backups);
            Assert.Empty(backups);

        }
    }
}