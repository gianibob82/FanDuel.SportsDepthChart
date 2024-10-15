using FanDuel.SportsDepthChart.Core.Player;
using FanDuel.SportsDepthChart.Core.Team;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Sport
{
    public class SportEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<TeamEntity> Teams { get; set; }
    }
}
