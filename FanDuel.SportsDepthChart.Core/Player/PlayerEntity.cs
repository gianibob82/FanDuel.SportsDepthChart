using FanDuel.SportsDepthChart.Core.Chart;
using FanDuel.SportsDepthChart.Core.PlayerChartPlacement;
using FanDuel.SportsDepthChart.Core.Sport;
using FanDuel.SportsDepthChart.Core.Team;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Player
{
    public class PlayerEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }

        public int TeamEntityId { get; set; }
        public TeamEntity TeamEntity { get; set; }

    }
}
