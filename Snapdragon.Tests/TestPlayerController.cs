using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Tests
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that performs whatever actions are passed into it.
    ///
    /// After each turn, <see cref="TestPlayerController.Actions"/> is reset to nothing.
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
            Side player,
            Side firstPlayerToResolve
        )
        {
            var actions = this.Actions;
            this.Actions = new List<IPlayerAction>();

            return actions;
        }
    }
}
