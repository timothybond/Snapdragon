using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Tests
{
    /// <summary>
    /// A test <see cref="IPlayerController"/> that just doesn't do anything on its turn.
    /// </summary>
    public class NullPlayerController : IPlayerController
    {
        public IReadOnlyList<IPlayerAction> GetActions(
            GameState gameState,
            Side firstPlayerToResolve
        )
        {
            return new List<IPlayerAction>();
        }
    }
}
