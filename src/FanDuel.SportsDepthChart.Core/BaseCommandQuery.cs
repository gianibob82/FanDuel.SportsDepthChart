using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core
{
    public abstract class BaseCommandQuery
    {
        public int SportId => 1;
        public int TeamId => 1;
        public int ChartId => 1;
    }
}
