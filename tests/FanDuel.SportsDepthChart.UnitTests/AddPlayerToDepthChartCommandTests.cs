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
        public async Task AddNewPlayerToDepthChartAtGivenPosition()
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

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);
            mockContext.Setup(m => m.Players.Add(It.IsAny<PlayerEntity>()))
            .Callback<PlayerEntity>(entity =>
            {
                players.Add(entity);
            });

            var command = new AddPlayerToDepthChartCommand()
            {
                PlayerName = "Tom Brady",
                PlayerNumber = 12,
                Position = "qb",
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
        public async Task AddNewPlayerToDepthChartWithoutPosition()
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

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);
            mockContext.Setup(m => m.Players.Add(It.IsAny<PlayerEntity>()))
            .Callback<PlayerEntity>(entity =>
            {
                players.Add(entity);
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

        [Fact]
        public async Task AddExistingPlayerToDepthChartAtGivenPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 1 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();

            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);
            mockContext.Setup(m => m.Players.Add(It.IsAny<PlayerEntity>()))
            .Callback<PlayerEntity>(entity =>
            {
                players.Add(entity);
            });

            var command = new AddPlayerToDepthChartCommand()
            {
                PlayerName = "Mike Evans",
                PlayerNumber = 13,
                Position = "QB",
                PositionDepth = 1
            };

            AddPlayerToDepthChartCommandHandler handler = new AddPlayerToDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Never);
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
            Assert.Single(backups);
            // The added player would get priority. Anyone below that player in the depth chart would get moved down a position_depth
            Assert.True(backups.All(b => b.PlayerChartDepth > 1));
        }

        [Fact]
        public async Task AddExistingPlayerToDepthChartWithoutPosition()
        {
            var chartPlacement = new List<PlayerChartPlacementEntity>
            {
                new PlayerChartPlacementEntity{  ChartId = chartId, PlayerId = 18, Position = "QB",  PositionDepth = 1 }
            };

            var players = new List<PlayerEntity>
            {
                new PlayerEntity{ Id = 13, Name = "Mike Evans", TeamEntityId = teamId},
                new PlayerEntity{ Id = 18, Name = "Tyler Johnson", TeamEntityId = teamId},
            };

            var mockContext = new Mock<ISportsDepthChartContext>();

            mockContext.Setup(x => x.PlayerChartPlacements).ReturnsDbSet(chartPlacement);

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);
            mockContext.Setup(m => m.Players.Add(It.IsAny<PlayerEntity>()))
            .Callback<PlayerEntity>(entity =>
            {
                players.Add(entity);
            });

            var command = new AddPlayerToDepthChartCommand()
            {
                PlayerName = "Mike Evans",
                PlayerNumber = 13,
                Position = "QB"
            };

            AddPlayerToDepthChartCommandHandler handler = new AddPlayerToDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Never);
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

        [Fact]
        public async Task CantAddSamePlayerToSameDepthChart()
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

            mockContext.Setup(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()))
            .Callback<PlayerChartPlacementEntity>(entity =>
            {
                chartPlacement.Add(entity);
            });

            mockContext.Setup(x => x.Players).ReturnsDbSet(players);
            mockContext.Setup(m => m.Players.Add(It.IsAny<PlayerEntity>()))
            .Callback<PlayerEntity>(entity =>
            {
                players.Add(entity);
            });

            var command = new AddPlayerToDepthChartCommand()
            {
                PlayerName = "Mike Evans",
                PlayerNumber = 13,
                Position = "QB"
            };

            AddPlayerToDepthChartCommandHandler handler = new AddPlayerToDepthChartCommandHandler(mockContext.Object);
            // Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mockContext.Verify(m => m.Players.Add(It.IsAny<PlayerEntity>()), Times.Never);
            mockContext.Verify(m => m.PlayerChartPlacements.Add(It.IsAny<PlayerChartPlacementEntity>()), Times.Never);

            Assert.Equal(0, response);

        }
    }
}