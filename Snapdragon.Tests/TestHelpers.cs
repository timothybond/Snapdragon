using System.Collections.Immutable;
using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    public static class TestHelpers
    {
        /// <summary>
        /// Helper function for testing what happens when certain cards are played on one side.
        /// </summary>
        /// <param name="turn">The turn count - all prior turns will pass with no actions.</param>
        /// <param name="side">Which side to play cards for.</param>
        /// <param name="cards">Cards for the given player to play on the given turn.</param>
        /// <returns>The game state after the given turn has elapsed and all effects have resolved.</returns>
        public static GameState PlayCards(
            int turn,
            Side side,
            IEnumerable<(string CardName, Column Column)> cards
        )
        {
            switch (side)
            {
                case Side.Top:
                    return PlayCards(turn, cards, []);
                case Side.Bottom:
                    return PlayCards(turn, [], cards);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played.
        /// </summary>
        /// <param name="turn">The turn count - all prior turns will pass with no actions.</param>
        /// <param name="topPlayerCards">Cards for the top player to play on the given turn.</param>
        /// <param name="bottomPlayerCards">Cards for the bottom player to play on the given turn.</param>
        /// <returns>The game state after the given turn has elapsed and all effects have resolved.</returns>
        public static GameState PlayCards(
            int turn,
            IEnumerable<(string CardName, Column Column)> topPlayerCards,
            IEnumerable<(string CardName, Column Column)> bottomPlayerCards
        )
        {
            var engine = new Engine(new NullLogger());
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            var game = engine.CreateGame(
                new PlayerConfiguration("Top", new Deck([]), topController),
                new PlayerConfiguration("Bottom", new Deck([]), bottomController)
            );

            for (var i = 1; i < turn; i++)
            {
                game = engine.PlaySingleTurn(game);
            }

            game = game with
            {
                Top = GetPlayerWithCardsToPlay(topPlayerCards, topController, Side.Top, game),
                Bottom = GetPlayerWithCardsToPlay(
                    bottomPlayerCards,
                    bottomController,
                    Side.Bottom,
                    game
                )
            };

            game = engine.PlaySingleTurn(game);

            return game;
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played.
        /// </summary>
        /// <param name="game">Existing game state.</param>
        /// <param name="turn">The turn count - all prior turns will pass with no actions.</param>
        /// <param name="topPlayerCards">Cards for the top player to play on the given turn.</param>
        /// <param name="bottomPlayerCards">Cards for the bottom player to play on the given turn.</param>
        /// <returns>The game state after the given turn has elapsed and all effects have resolved.</returns>
        public static GameState PlayCards(
            GameState game,
            int turn,
            IEnumerable<(string CardName, Column Column)> topPlayerCards,
            IEnumerable<(string CardName, Column Column)> bottomPlayerCards
        )
        {
            if (turn <= game.Turn)
            {
                throw new ArgumentException(
                    $"Game already played to turn {game.Turn}, but tried to play cards on turn {turn}."
                );
            }

            var engine = new Engine(new NullLogger());
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            game = game with
            {
                Top = game.Top with
                {
                    Configuration = game.Top.Configuration with { Controller = topController }
                },
                Bottom = game.Bottom with
                {
                    Configuration = game.Bottom.Configuration with { Controller = bottomController }
                }
            };

            for (var i = 1; i < turn - game.Turn; i++)
            {
                game = engine.PlaySingleTurn(game);
            }

            game = game with
            {
                Top = GetPlayerWithCardsToPlay(topPlayerCards, topController, Side.Top, game),
                Bottom = GetPlayerWithCardsToPlay(
                    bottomPlayerCards,
                    bottomController,
                    Side.Bottom,
                    game
                )
            };

            game = engine.PlaySingleTurn(game);

            return game;
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played.
        /// </summary>
        /// <param name="game">Existing game state.</param>
        /// <param name="turn">The turn count - all prior turns will pass with no actions.</param>
        /// <param name="topPlayerCards">Cards for the top player to play on the given turn.</param>
        /// <param name="bottomPlayerCards">Cards for the bottom player to play on the given turn.</param>
        /// <returns>The game state after the given turn has elapsed and all effects have resolved.</returns>
        public static GameState PlayCards(
            GameState game,
            int turn,
            Side side,
            IEnumerable<(string CardName, Column Column)> cardsToPlay
        )
        {
            switch (side)
            {
                case Side.Top:
                    return PlayCards(game, turn, cardsToPlay, []);
                case Side.Bottom:
                    return PlayCards(game, turn, [], cardsToPlay);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Helper function. Puts the cards to be played into the hand of the returned <see cref="Player"/>
        /// and sets up the <see cref="TestPlayerController"/> to actually play them.
        /// </summary>
        private static Player GetPlayerWithCardsToPlay(
            IEnumerable<(string CardName, Column Column)> cardsToPlay,
            TestPlayerController controller,
            Side side,
            GameState game
        )
        {
            var playerHand = new List<Card>();
            var playerActions = new List<IPlayerAction>();

            foreach (var topPlay in cardsToPlay)
            {
                var card = new Card(SnapCards.ByName[topPlay.CardName], side, CardState.InHand);
                playerHand.Add(card);
                var playCardAction = new PlayCardAction(side, card, topPlay.Column);
                playerActions.Add(playCardAction);
            }

            controller.Actions = playerActions;

            return game[side] with
            {
                Hand = playerHand.ToImmutableList()
            };
        }
    }
}
