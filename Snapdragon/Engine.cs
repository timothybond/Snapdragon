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
            Side? firstRevealed = null,
            ISnapdragonRepository? repository = null,
            Guid? experimentId = null,
            int? generation = null
        )
        {
            var firstRevealedOrRandom = firstRevealed ?? Random.Side();

            // TODO: Specify different Locations, with effects
            // TODO: Handle card abilities that put them in a specific draw order
            var game = new Game(
                Guid.NewGuid(),
                0,
                new Location("Left", Column.Left),
                new Location("Middle", Column.Middle),
                new Location("Right", Column.Right),
                topPlayer.ToPlayer(Side.Top, shuffle),
                bottomPlayer.ToPlayer(Side.Bottom, shuffle),
                firstRevealedOrRandom,
                [],
                [],
                this.logger
            );

            if (repository != null)
            {
                game = game with { Logger = new RepositoryGameLogger(repository, game.Id, experimentId, generation) };
            }

            game = game with
            {
                Top = game.Top.DrawCard().DrawCard().DrawCard(),
                Bottom = game.Bottom.DrawCard().DrawCard().DrawCard()
            };

            return game;
        }
    }
}
