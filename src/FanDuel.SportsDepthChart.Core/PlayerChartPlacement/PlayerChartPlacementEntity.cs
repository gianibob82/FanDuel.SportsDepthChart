using FanDuel.SportsDepthChart.Core.Chart;
using FanDuel.SportsDepthChart.Core.Player;
using FanDuel.SportsDepthChart.Core.Sport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.PlayerChartPlacement
{
    public class PlayerChartPlacementEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Position { get; set; }
        public int PositionDepth { get; set; }

        public int ChartId { get; set; }
        public ChartEntity Chart { get; set; }

        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }
    }
}
