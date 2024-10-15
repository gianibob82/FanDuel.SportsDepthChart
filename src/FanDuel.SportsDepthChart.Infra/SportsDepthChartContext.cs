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
        public virtual DbSet<PlayerEntity> Players { get; set; }

        public virtual DbSet<PlayerChartPlacementEntity> PlayerChartPlacements { get; set; }

        public virtual DbSet<ChartEntity> Charts { get; set; }

        public virtual DbSet<TeamEntity> Teams { get; set; }

        public virtual DbSet<SportEntity> Sports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("sportsdepthchart");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<SportEntity>().HasData(new SportEntity
        //    {
        //        Id = 1,
        //        Name = "NFL"
        //    });

        //    modelBuilder.Entity<TeamEntity>().HasData(new TeamEntity
        //    {
        //        Id = 1,
        //        SportId = 1,
        //        Name = "Tampa Bay Buccaneers",
        //    });

        //    modelBuilder.Entity<ChartEntity>().HasData(new ChartEntity
        //    {
        //        TeamId = 1,
        //        Id = 1,
        //        Name = "Offence"
        //    });

        //    modelBuilder.Entity<PlayerEntity>().HasData(new PlayerEntity
        //    {
        //         Id =12,
        //          Name = "Tom Brady",
        //           TeamEntityId = 1,
        //    });

        //    modelBuilder.Entity<PlayerChartPlacementEntity>().HasData(new PlayerChartPlacementEntity
        //    {
        //        Id = 1,
        //         ChartId = 1,
        //          PlayerId = 12,
        //           Position = "QB",
        //            PositionDepth = 1,
        //    });
        //}
    }
}
