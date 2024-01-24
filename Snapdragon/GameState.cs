using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    public record GameState(int Turn, Location Left, Location Middle, Location Right, PlayerState Top, PlayerState Bottom)
    {
    }
}
