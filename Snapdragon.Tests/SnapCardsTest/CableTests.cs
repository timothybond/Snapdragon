using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class CableTests
    {
        [Test]
        public void DrawsOpponentCard()
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
                            SnapCards.ByName["Cable"],
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

            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();
            game = game.StartNextTurn();

            var cableCard = game[Side.Top].Hand.Single(c => string.Equals(c.Name, "Cable"));

            topController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, cableCard, Column.Middle)
            };

            game = game.PlayAlreadyStartedTurn();

            // We drew six cards (three at start of game, three for turns), then played one, but should have drawn one from the opponent.
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(6));
            Assert.That(game[Side.Top].Hand.Last().Definition, Is.EqualTo(Cards.ThreeThree));
        }

        [Test]
        public void OpponentDeckEmpty_NothingBreaks()
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
                            SnapCards.ByName["Cable"],
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

            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();
            game = game.StartNextTurn();

            var cableCard = game[Side.Top].Hand.Single(c => string.Equals(c.Name, "Cable"));

            topController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, cableCard, Column.Middle)
            };

            game = game.PlayAlreadyStartedTurn();

            // We drew six cards (three at start of game, three for turns), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(5));
        }
    }
}
