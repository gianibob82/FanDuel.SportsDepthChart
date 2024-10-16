using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanDuel.SportsDepthChart.Core.Chart
{
    public abstract class ChartBaseCommandQuery : BaseCommandQuery
    {
        private string _position;
        public string Position 
        {
            get { return _position.ToUpperInvariant(); }
            set { _position = value; }
        }

        public int PlayerNumber { get; set; }
    }
}
