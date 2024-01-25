using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    /// <summary>
    /// An implementation of <see cref="IGameLogger"/> that just doesn't log anything.
    /// </summary>
    public class NullLogger : IGameLogger
    {
        public void LogEvent(Event e)
        {
        }

        public void LogGameState(GameState game)
        {
        }
    }
}
