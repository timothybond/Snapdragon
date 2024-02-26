namespace Snapdragon.Tests
{
    /// <summary>
    /// An implementation of <see cref="IPlayerController"/> that performs whatever actions are passed into it.  After
    /// each turn, <see cref="TestPlayerController.Actions"/> is reset to nothing.
    /// </summary>
    public class TestPlayerController : IPlayerController
    {
        public TestPlayerController()
        {
            this.Actions = new List<IPlayerAction>();
        }

        public IReadOnlyList<IPlayerAction> Actions { get; set; }

        public Task<IReadOnlyList<IPlayerAction>> GetActions(Game game, Side player)
        {
            var actions = this.Actions;
            this.Actions = new List<IPlayerAction>();

            return Task.FromResult(actions);
        }
    }
}
