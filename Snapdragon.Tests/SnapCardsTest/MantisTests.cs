using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class MantisTests
    {
        [Test]
        public async Task OpponentPlaysCardSameLocation_DrawsOpponentCard()
        {
            // Note: Both sides will draw 3 cards during setup.
            var engine = new Engine(new NullLogger());
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            // Note that players draw three cards to start
            var game = engine.CreateGame(
                new PlayerConfiguration(
                    "Top",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            SnapCards.ByName["Mantis"],
                            Cards.OneThree
                        ]
                    ),
                    topController
                ),
                new PlayerConfiguration(
                    "Bottom",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.ThreeThree
                        ]
                    ),
                    bottomController
                ),
                false
            );

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();
            game = await game.StartNextTurn();

            var mantisCard = game[Side.Top].Hand.Single(c => string.Equals(c.Name, "Mantis"));

            topController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, mantisCard, Column.Middle)
            };
            bottomController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Bottom, game[Side.Bottom].Hand.First(), Column.Middle)
            };

            game = await game.PlayAlreadyStartedTurn();

            // We drew six cards (three at start of game, three for turns),
            // then played one, but should have drawn one from the opponent
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(6));
            Assert.That(game[Side.Top].Hand.Last().Definition, Is.EqualTo(Cards.ThreeThree));
        }

        [Test]
        public async Task OpponentPlaysCardDifferentLocation_DoesNotDraw()
        {
            // Note: Both sides will draw 3 cards during setup.
            var engine = new Engine(new NullLogger());
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            // Note that players draw three cards to start
            var game = engine.CreateGame(
                new PlayerConfiguration(
                    "Top",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            SnapCards.ByName["Mantis"],
                            Cards.OneThree
                        ]
                    ),
                    topController
                ),
                new PlayerConfiguration(
                    "Bottom",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.ThreeThree
                        ]
                    ),
                    bottomController
                ),
                false
            );

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();
            game = await game.StartNextTurn();

            var mantisCard = game[Side.Top].Hand.Single(c => string.Equals(c.Name, "Mantis"));

            topController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, mantisCard, Column.Middle)
            };
            bottomController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Bottom, game[Side.Bottom].Hand.First(), Column.Right)
            };

            game = await game.PlayAlreadyStartedTurn();

            // We drew six cards (three at start of game, three for turns), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task PlaysTwoCardsToSameLocation_DoesNotDraw()
        {
            // Note: Both sides will draw 3 cards during setup.
            var engine = new Engine(new NullLogger());
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            // Note that players draw three cards to start
            var game = engine.CreateGame(
                new PlayerConfiguration(
                    "Top",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            SnapCards.ByName["Mantis"],
                            Cards.OneThree
                        ]
                    ),
                    topController
                ),
                new PlayerConfiguration(
                    "Bottom",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.ThreeThree
                        ]
                    ),
                    bottomController
                ),
                false
            );

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();
            game = await game.StartNextTurn();

            var mantisCard = game[Side.Top].Hand.Single(c => string.Equals(c.Name, "Mantis"));

            topController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, mantisCard, Column.Middle),
                new PlayCardAction(Side.Top, game[Side.Top].Hand.First(), Column.Middle)
            };

            game = await game.PlayAlreadyStartedTurn();

            // We drew six cards (three at start of game, three for turns), then played two
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task OpponentDeckEmpty_NothingBreaks()
        {
            // Note: Both sides will draw 3 cards during setup.
            var engine = new Engine(new NullLogger());
            var topController = new TestPlayerController();
            var bottomController = new TestPlayerController();

            // Note that players draw three cards to start
            var game = engine.CreateGame(
                new PlayerConfiguration(
                    "Top",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            SnapCards.ByName["Mantis"],
                            Cards.OneThree
                        ]
                    ),
                    topController
                ),
                new PlayerConfiguration(
                    "Bottom",
                    new Deck(
                        [
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne,
                            Cards.OneOne
                        ]
                    ),
                    bottomController
                ),
                false
            );

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();
            game = await game.StartNextTurn();

            var mantisCard = game[Side.Top].Hand.Single(c => string.Equals(c.Name, "Mantis"));

            topController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, mantisCard, Column.Middle)
            };
            bottomController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Bottom, game[Side.Bottom].Hand.First(), Column.Middle)
            };

            game = await game.PlayAlreadyStartedTurn();

            // We drew six cards (three at start of game, three for turns), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(5));
        }
    }
}
