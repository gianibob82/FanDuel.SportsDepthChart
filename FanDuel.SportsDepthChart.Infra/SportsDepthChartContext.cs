using FanDuel.SportsDepthChart.Core;
using FanDuel.SportsDepthChart.Core.Chart;
using FanDuel.SportsDepthChart.Core.Player;
using FanDuel.SportsDepthChart.Core.PlayerChartPlacement;
using FanDuel.SportsDepthChart.Core.Sport;
using FanDuel.SportsDepthChart.Core.Team;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace FanDuel.SportsDepthChart.Infra
{
    public class SportsDepthChartContext : DbContext, ISportsDepthChartContext
    {
        public DbSet<PlayerEntity> Players { get; set; }

        public DbSet<PlayerChartPlacementEntity> PlayerChartPlacements { get; set; }

        public DbSet<ChartEntity> Charts { get; set; }

        public DbSet<TeamEntity> Teams { get; set; }

        public DbSet<SportEntity> Sports { get; set; }

        public string DbPath { get; }

        public SportsDepthChartContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "sportsdepthchart.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("sportsdepthchart");
            //options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SportEntity>().HasData(new SportEntity
            {
                Id = 1,
                Name = "NFL"
            });

            modelBuilder.Entity<TeamEntity>().HasData(new TeamEntity
            {
                Id = 1,
                SportId = 1,
                Name = "Tampa Bay Buccaneers",
            });

            modelBuilder.Entity<ChartEntity>().HasData(new ChartEntity
            {
                TeamId = 1,
                Id = 1,
                Name = "Offence"
            });

            modelBuilder.Entity<PlayerEntity>().HasData(new PlayerEntity
            {
                 Id =12,
                  Name = "Tom Brady",
                   TeamEntityId = 1,
            });

            modelBuilder.Entity<PlayerChartPlacementEntity>().HasData(new PlayerChartPlacementEntity
            {
                Id = 1,
                 ChartId = 1,
                  PlayerId = 12,
                   Position = "QB",
                    PositionDepth = 1,
            });
        }
    }
}
