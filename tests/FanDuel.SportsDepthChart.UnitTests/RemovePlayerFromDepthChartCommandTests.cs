using FanDuel.SportsDepthChart.Core.Chart;
using FanDuel.SportsDepthChart.Core.Player;
using FanDuel.SportsDepthChart.Core.PlayerChartPlacement;
using FanDuel.SportsDepthChart.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;

namespace FanDuel.SportsDepthChart.UnitTests
{
    public class RemovePlayerFromDepthChartCommandTests
    {
        const int teamId = 1;
        const int chartId = 1;

        [Fact]
        public async Task RemovePlayerAtGivenPosition()
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

            mockContext.Setup(m => m.PlayerChartPlacements.Remove(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Remove(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            var command = new RemovePlayerFromDepthChartCommand()
            {
                PlayerNumber = 12,
                Position = "QB"
            };

            RemovePlayerFromDepthChartCommandHandler handler = new RemovePlayerFromDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            Assert.Single(response);
            Assert.True(response.Single().PlayerNumber == command.PlayerNumber);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Never);
            mockContext.Verify(m => m.PlayerChartPlacements.Remove(It.IsAny<PlayerChartPlacementEntity>()), Times.Once());

            // verify there are backups 
            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = 13,
                Position = "QB",
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            // there are 1 backups
            Assert.Single(backups);
            // verify after removing player the next backup is moved left
            Assert.True(backups.Single().PlayerChartDepth == 2);
        }

        [Fact]
        public async Task ReturnEmptyListIfNotInPosition()
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

            mockContext.Setup(m => m.PlayerChartPlacements.Remove(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Remove(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);

            var command = new RemovePlayerFromDepthChartCommand()
            {
                PlayerNumber = 12,
                Position = "RT"
            };

            RemovePlayerFromDepthChartCommandHandler handler = new RemovePlayerFromDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            Assert.Empty(response);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Never);
            mockContext.Verify(m => m.PlayerChartPlacements.Remove(It.IsAny<PlayerChartPlacementEntity>()), Times.Never);

            // verify there are backups 
            GetBackupsQuery query = new GetBackupsQuery
            {
                PlayerNumber = 12,
                Position = "QB",
            };
            GetBackupsQueryHandler getBackupsQueryHandler = new GetBackupsQueryHandler(mockContext.Object);
            var backups = await getBackupsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            // there is still 1 backups
            Assert.Single(backups);
            // next backup hasn't been shifted because no remove happened
            Assert.True(backups.Single().PlayerChartDepth == 3);
        }
    }
}
