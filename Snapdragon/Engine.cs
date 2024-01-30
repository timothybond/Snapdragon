namespace Snapdragon
{
    public class Engine
    {
        private readonly IGameLogger logger;

        public Engine(IGameLogger logger)
        {
            this.logger = logger;
        }

        public Game CreateGame(
            PlayerConfiguration topPlayer,
            PlayerConfiguration bottomPlayer,
            bool shuffle = true,
            Side? firstRevealed = null
        )
        {
            var firstRevealedOrRandom = firstRevealed ?? Random.Side();

            // TODO: Specify different Locations, with effects
            // TODO: Handle card abilities that put them in a specific draw order
            return new Game(
                0,
                new Location("Left", Column.Left),
                new Location("Middle", Column.Middle),
                new Location("Right", Column.Right),
                topPlayer.ToPlayer(Side.Top, shuffle).DrawCard().DrawCard().DrawCard(),
                bottomPlayer.ToPlayer(Side.Bottom, shuffle).DrawCard().DrawCard().DrawCard(),
                firstRevealedOrRandom,
                [],
                [],
                this.logger
            );
        }
    }
}
