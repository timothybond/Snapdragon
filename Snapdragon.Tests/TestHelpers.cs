using System.Collections.Immutable;
using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    public static class TestHelpers
    {
        public static Game NewGame()
        {
            var engine = new Engine(new NullLogger());

            // Note other TestHelper functions assume these are instances of TestPlayerController
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            var game = engine.CreateGame(
                new PlayerConfiguration("Top", new Deck([]), topController),
                new PlayerConfiguration("Bottom", new Deck([]), bottomController)
            );

            return game;
        }

        public static Game WithCardsInHand(this Game game, Side side, params string[] cardNames)
        {
            var cards = GetCards(side, cardNames);

            return game.WithPlayer(game[side] with { Hand = game[side].Hand.AddRange(cards) });
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played on one side.
        ///
        /// Will automatically increment the turn to the first one with enough energy to play the given cards.
        /// </summary>
        /// <param name="side">Which side to play cards for.</param>
        /// <param name="column">The location to play all of the cards.</param>
        /// <param name="cardNames">The cards to play, in order.</param>
        /// <returns></returns>
        public static Game PlayCards(Side side, Column column, params string[] cardNames)
        {
            var cards = GetCards(side, cardNames);

            var turn = cards.Select(c => c.Cost).Sum();

            // TODO: Refactor this to avoid getting the cards twice, although it doesn't matter much.
            return PlayCards(turn, side, cardNames.Select(c => (c, column)));
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played on one side.
        ///
        /// Will automatically increment the turn to the first one with enough energy to play the given cards,
        /// or the next available turn if it's already high enough.
        /// </summary>
        /// <param name="game">The prior game state.</param>
        /// <param name="side">Which side to play cards for.</param>
        /// <param name="column">The location to play all of the cards.</param>
        /// <param name="cardNames">The cards to play, in order.</param>
        /// <returns></returns>
        public static Game PlayCards(
            this Game game,
            Side side,
            Column column,
            params string[] cardNames
        )
        {
            var cards = GetCards(side, cardNames);

            var turn = cards.Select(c => c.Cost).Sum();

            while (game.Turn < turn - 1)
            {
                game = game.PlaySingleTurn();
            }

            // TODO: Refactor this to avoid getting the cards twice, although it doesn't matter much.
            return PlayCards(game, game.Turn + 1, side, cardNames.Select(c => (c, column)));
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played on one side.
        /// </summary>
        /// <param name="turn">The turn count - all prior turns will pass with no actions.</param>
        /// <param name="side">Which side to play cards for.</param>
        /// <param name="cards">Cards for the given player to play on the given turn.</param>
        /// <returns>The game state after the given turn has elapsed and all effects have resolved.</returns>
        public static Game PlayCards(
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
        public static Game PlayCards(
            int turn,
            IEnumerable<(string CardName, Column Column)> topPlayerCards,
            IEnumerable<(string CardName, Column Column)> bottomPlayerCards
        )
        {
            var game = NewGame();

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game with
            {
                Top = GetPlayerWithCardsToPlay(
                    topPlayerCards,
                    (TestPlayerController)game[Side.Top].Controller,
                    Side.Top,
                    game
                ),
                Bottom = GetPlayerWithCardsToPlay(
                    bottomPlayerCards,
                    (TestPlayerController)game[Side.Bottom].Controller,
                    Side.Bottom,
                    game
                )
            };

            game = game.PlaySingleTurn();

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
        public static Game PlayCards(
            Game game,
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
                game = game.PlaySingleTurn();
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

            game = game.PlaySingleTurn();

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
        public static Game PlayCards(
            Game game,
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
        /// Gets cards to be played (so, <see cref="Card.State"/> is set to <see cref="CardState.InHand"/>).
        /// </summary>
        /// <param name="side">Player side that will play the cards.</param>
        /// <param name="cardNames">Names of the cards.</param>
        /// <returns></returns>
        private static IReadOnlyList<Card> GetCards(Side side, params string[] cardNames)
        {
            // Somewhat pointless cast, but I didn't want to independently implement both methods.
            return GetCards(side, cardNames.ToList());
        }

        /// <summary>
        /// Gets cards to be played (so, <see cref="Card.State"/> is set to <see cref="CardState.InHand"/>).
        /// </summary>
        /// <param name="side">Player side that will play the cards.</param>
        /// <param name="cardNames">Names of the cards.</param>
        /// <returns></returns>
        private static IReadOnlyList<Card> GetCards(Side side, IEnumerable<string> cardNames)
        {
            return cardNames
                .Select(name => new Card(SnapCards.ByName[name], side, CardState.InHand))
                .ToList();
        }

        /// <summary>
        /// Helper function. Puts the cards to be played into the hand of the returned <see cref="Player"/> and sets up
        /// the <see cref="TestPlayerController"/> to actually play them.
        ///
        /// This will leave the player's current hand, which in some cases may violate the assumption
        /// that no player has more than 7 cards in their hand.
        /// </summary>
        private static Player GetPlayerWithCardsToPlay(
            IEnumerable<(string CardName, Column Column)> cardsToPlay,
            TestPlayerController controller,
            Side side,
            Game game
        )
        {
            var playerHand = game[side].Hand.ToList();
            var playerActions = new List<IPlayerAction>();

            var cardsNeeded = cardsToPlay.Where(nameAndLocation =>
                !playerHand.Any(cardInHand =>
                    string.Equals(cardInHand.Name, nameAndLocation.CardName)
                )
            );

            foreach (var absentCard in cardsNeeded)
            {
                var card = new Card(SnapCards.ByName[absentCard.CardName], side, CardState.InHand);
                playerHand.Add(card);
            }

            foreach (var play in cardsToPlay)
            {
                var card = playerHand.First(c => string.Equals(c.Name, play.CardName));
                var playCardAction = new PlayCardAction(side, card, play.Column);
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
