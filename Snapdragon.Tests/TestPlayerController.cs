using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Tests
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that performs whatever actions are passed into it.
    /// </summary>
    public class TestPlayerController : IPlayerController
    {
        public TestPlayerController()
        {
            this.Actions = new List<IPlayerAction>();
        }

        public IReadOnlyList<IPlayerAction> Actions { get; set; }

        public IReadOnlyList<IPlayerAction> GetActions(
            GameState gameState,
            Side firstPlayerToResolve
        )
        {
            return this.Actions;
        }
    }
}
