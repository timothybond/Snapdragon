using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    /// <summary>
    /// Represents an action that a <see cref="Player"/> can choose to do on a specific Turn.
    /// </summary>
    public interface IPlayerAction
    {
        public Side Side { get; }

        GameState Apply(GameState initialState);
    }
}
