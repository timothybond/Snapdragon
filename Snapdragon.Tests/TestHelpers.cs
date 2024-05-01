using System.Collections.Immutable;
using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    public static class TestHelpers
    {
        /// <summary>
        /// Generates an initial set of cards for use with a basic genetic algorithm.  In practice this will be 12 each of
        /// every combination of the numbers 1-3 for cost and cost-power bonus, where actual power will be that bonus plus
        /// the cost. Some cards are therefore strictly inferior and should be expected to vanish from the population over
        /// time, while others will represent conditional tradeoffs between less and more expensive cards.  All cards will
        /// be given a three-character name of the form [cost][power]A through [cost][power]L, since there will be twelve
        /// copies of each.
        /// </summary>
        /// <returns></returns>
        public static ImmutableList<CardDefinition> GetInitialCardDefinitions()
        {
            const string labels = "ABCDEFGHIJKL";

            var cards = new List<CardDefinition>();

            for (var cost = 1; cost <= 3; cost++)
            {
                for (var power = 1; power <= 3; power++)
                {
                    for (var i = 0; i < 12; i++)
                    {
                        var actualPower = cost + power;

                        cards.Add(
                            new CardDefinition($"{cost}{actualPower}{labels[i]}", cost, actualPower)
                        );
                    }
                }
            }

            return cards.ToImmutableList();
        }

        /// <summary>
        /// Helper function for a game with one specific <see cref="Location"/> (the others being "Ruins").
        /// </summary>
        public static Game NewGame(string locationName, Column column)
        {
            switch (column)
            {
                case Column.Left:
                    return NewGame(leftLocation: locationName);
                case Column.Middle:
                    return NewGame(middleLocation: locationName);
                case Column.Right:
                    return NewGame(rightLocation: locationName);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Game NewGame(
            string leftLocation = "Ruins",
            string middleLocation = "Ruins",
            string rightLocation = "Ruins"
        )
        {
            var engine = new Engine(new NullLogger());

            // Note other TestHelper functions assume these are instances of TestPlayerController
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            var game = engine.CreateGame(
                new PlayerConfiguration("Top", new Deck([]), topController),
                new PlayerConfiguration("Bottom", new Deck([]), bottomController),
                leftLocationName: leftLocation,
                middleLocationName: middleLocation,
                rightLocationName: rightLocation
            );

            return game;
        }

        public static Game WithCardsInHand(this Game game, Side side, params string[] cardNames)
        {
            var cards = GetCards(cardNames);

            return game.WithCardsInHand(side, cards.ToArray());
        }

        public static Game WithCardsInHand(this Game game, Side side, params CardDefinition[] cards)
        {
            var kernel = game.Kernel;

            foreach (var card in cards)
            {
                kernel = kernel.AddNewCardToHand(card, side, out long newCardId);
            }

            return game with
            {
                Kernel = kernel
            };
        }

        public static Game WithCardsInDeck(this Game game, Side side, params string[] cardNames)
        {
            var cards = GetCards(cardNames);

            return game.WithCardsInDeck(side, cards.ToArray());
        }

        public static Game WithCardsInDeck(
            this Game game,
            Side side,
            params CardDefinition[] cardDefinitions
        )
        {
            var kernel = game.Kernel;

            foreach (var cardDef in cardDefinitions)
            {
                kernel = kernel.AddNewCardToLibrary(cardDef, side, out long newCardId);
            }

            return game with
            {
                Kernel = kernel
            };
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are moved.
        ///
        /// Note this will take any matching cards by name/side/location, even duplicates,
        /// and will ignore it if there are some names that don't match.
        /// </summary>
        /// <param name="game">Prior game state.</param>
        /// <param name="side">Which side to move cards for.</param>
        /// <param name="from">Start location for moved cards.</param>
        /// <param name="to">End location for moved cards.</param>
        /// <param name="cardNames">Names of cards to move.</param>
        /// <returns></returns>
        public static Game MoveCards(
            this Game game,
            Side side,
            Column from,
            Column to,
            params string[] cardNames
        )
        {
            var cards = game[from][side].Where(c => cardNames.Contains(c.Name));

            var moveActions = cards.Select(c => new MoveCardAction(side, c, from, to)).ToList();

            var controller = (TestPlayerController)game[side].Controller;
            controller.Actions = moveActions;

            return game.PlaySingleTurn();
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
            var cards = GetCards(cardNames);

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
            var cards = GetCards(cardNames);

            var turn = cards
                .Select(c =>
                {
                    var cardInHand = game[side]
                        .Hand.FirstOrDefault(card => string.Equals(card.Name, c.Name));

                    if (cardInHand != null)
                    {
                        return cardInHand.Cost;
                    }

                    return c.Cost;
                })
                .Sum();

            if (turn > 6)
            {
                throw new InvalidOperationException(
                    "Cannot play more than 6 energy worth of cards."
                );
            }

            while (game.Turn < turn - 1)
            {
                game = game.PlaySingleTurn();
            }

            // TODO: Refactor this to avoid getting the cards twice, although it doesn't matter much.
            return PlayCards(game, game.Turn + 1, side, cardNames.Select(c => (c, column)));
        }

        /// <summary>
        /// Helper function for testing what happens when certain cards are played on one side.
        ///
        /// Will automatically increment the turn to the first one with enough energy to play the given cards,
        /// or the next available turn if it's already high enough.
        /// </summary>
        /// <param name="side">Which side to play cards for.</param>
        /// <param name="cards">Cards for the given player to play on the given turn.</param>
        /// <returns>The game state after the given turn has elapsed and all effects have resolved.</returns>
        public static Game PlayCards(Side side, params (string CardName, Column Column)[] cards)
        {
            var cardsToPlay = cards.ToList();
            var turn = GetCards(cards.Select(c => c.CardName)).Select(c => c.Cost).Sum();

            return PlayCards(turn, side, cards);
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
            this Game game,
            IEnumerable<(string CardName, Column Column)> topPlayerCards,
            IEnumerable<(string CardName, Column Column)> bottomPlayerCards
        )
        {
            var minimumTurn = Math.Max(
                GetTotalCost(topPlayerCards.ToArray()),
                GetTotalCost(bottomPlayerCards.ToArray())
            );

            for (var i = game.Turn; i < minimumTurn - 1; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.StartNextTurn();

            game = GetPlayerWithCardsToPlay(
                topPlayerCards,
                (TestPlayerController)game[Side.Top].Controller,
                Side.Top,
                game
            );
            game = GetPlayerWithCardsToPlay(
                bottomPlayerCards,
                (TestPlayerController)game[Side.Bottom].Controller,
                Side.Bottom,
                game
            );

            game = game.PlayAlreadyStartedTurn();

            return game;
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

            game = game.StartNextTurn();

            game = GetPlayerWithCardsToPlay(
                topPlayerCards,
                (TestPlayerController)game[Side.Top].Controller,
                Side.Top,
                game
            );
            game = GetPlayerWithCardsToPlay(
                bottomPlayerCards,
                (TestPlayerController)game[Side.Bottom].Controller,
                Side.Bottom,
                game
            );

            game = game.PlayAlreadyStartedTurn();

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
                TopPlayer = game.TopPlayer with
                {
                    Configuration = game.Top.Configuration with { Controller = topController }
                },
                BottomPlayer = game.BottomPlayer with
                {
                    Configuration = game.Bottom.Configuration with { Controller = bottomController }
                }
            };

            for (var i = 1; i < turn - game.Turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.StartNextTurn();

            game = GetPlayerWithCardsToPlay(topPlayerCards, topController, Side.Top, game);
            game = GetPlayerWithCardsToPlay(bottomPlayerCards, bottomController, Side.Bottom, game);

            game = game.PlayAlreadyStartedTurn();

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

        private static int GetTotalCost(params (string CardName, Column Column)[] cardsToPlay)
        {
            return GetCards(cardsToPlay.Select(c => c.CardName).ToList()).Sum(c => c.Cost);
        }

        private static int GetTotalCost(params string[] cardNames)
        {
            return GetCards(cardNames.ToList()).Sum(c => c.Cost);
        }

        /// <summary>
        /// Gets cards to be played.
        /// </summary>
        /// <param name="cardNames">Names of the cards.</param>
        private static IReadOnlyList<CardDefinition> GetCards(params string[] cardNames)
        {
            // Somewhat pointless cast, but I didn't want to independently implement both methods.
            return GetCards((IEnumerable<string>)cardNames);
        }

        /// <summary>
        /// Gets cards to be played.
        /// </summary>
        /// <param name="cardNames">Names of the cards.</param>
        private static IReadOnlyList<CardDefinition> GetCards(IEnumerable<string> cardNames)
        {
            // Somewhat pointless cast, but I didn't want to independently implement both methods.
            return cardNames.Select(name => SnapCards.ByName[name]).ToList();
        }

        /// <summary>
        /// Helper function. Puts the cards to be played into the hand of the returned <see cref="Player"/> and sets up
        /// the <see cref="TestPlayerController"/> to actually play them.
        ///
        /// This will leave the player's current hand, which in some cases may violate the assumption
        /// that no player has more than <see cref="Max.HandSize"/> (7) cards in their hand.
        /// </summary>
        private static Game GetPlayerWithCardsToPlay(
            IEnumerable<(string CardName, Column Column)> cardsToPlay,
            TestPlayerController controller,
            Side side,
            Game game
        )
        {
            var playerHand = game[side].Hand;
            var playerActions = new List<IPlayerAction>();

            var cardsNeeded = cardsToPlay.Where(nameAndLocation =>
                !playerHand.Any(cardInHand =>
                    string.Equals(cardInHand.Name, nameAndLocation.CardName)
                )
            );

            foreach (var absentCard in cardsNeeded)
            {
                game = game with
                {
                    Kernel = game.Kernel.AddNewCardToHand(
                        SnapCards.ByName[absentCard.CardName],
                        side,
                        out long _
                    )
                };
            }

            playerHand = game[side].Hand;

            foreach (var play in cardsToPlay)
            {
                var card = playerHand.First(c => string.Equals(c.Name, play.CardName));
                var playCardAction = new PlayCardAction(side, card, play.Column);
                playerActions.Add(playCardAction);
            }

            controller.Actions = playerActions;

            return game;
        }
    }
}
