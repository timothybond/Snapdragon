namespace Snapdragon
{
    /// <summary>
    /// An instance of <see cref="IPlayerController"/> that performs a pre-specified
    /// set of actions on its next turn, but afterwards plays randomly.
    ///
    /// Used for executing Monte Carlo search.
    /// </summary>
    public class TemporaryPlayerController : IPlayerController
    {
        private IReadOnlyList<IPlayerAction>? actions;
        private readonly RandomPlayerController randomController = new RandomPlayerController();

        public TemporaryPlayerController(IReadOnlyList<IPlayerAction> initialActions)
        {
            this.actions = initialActions;
        }

        public async Task<IReadOnlyList<IPlayerAction>> GetActions(Game game, Side side)
        {
            if (this.actions != null)
            {
                var actions = this.actions;
                this.actions = null;
                return actions;
            }

            return await this.randomController.GetActions(game, side);
        }
    }
}
