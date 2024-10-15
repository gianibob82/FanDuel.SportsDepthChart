using FanDuel.SportsDepthChart.Core.Chart;
using FanDuel.SportsDepthChart.Core.Player;
using FanDuel.SportsDepthChart.Core.PlayerChartPlacement;
using FanDuel.SportsDepthChart.Core.Sport;
using FanDuel.SportsDepthChart.Core.Team;
using Microsoft.EntityFrameworkCore;

namespace FanDuel.SportsDepthChart.Core
{
    public interface ISportsDepthChartContext
    {
        public DbSet<PlayerEntity> Players { get; set; }

        public DbSet<PlayerChartPlacementEntity> PlayerChartPlacements { get; set; }

        public DbSet<ChartEntity> Charts { get; set; }

        public DbSet<TeamEntity> Teams { get; set; }

        public DbSet<SportEntity> Sports { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
