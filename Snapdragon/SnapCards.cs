using Snapdragon.Calculations;
using Snapdragon.CardConditions;
using Snapdragon.CardTriggers;
using Snapdragon.LocationFilters;
using Snapdragon.OngoingAbilities;
using Snapdragon.RevealAbilities;
using Snapdragon.TargetFilters;
using Snapdragon.TemporaryEffects;
using Snapdragon.TriggeredEffects;

namespace Snapdragon
{
    /// <summary>
    /// A collection of all of the defined cards for the actual game.
    /// </summary>
    public static class SnapCards
    {
        public static IReadOnlyList<CardDefinition> All = new List<CardDefinition>
        {
            new("Wasp", 0, 1),
            new("Squirrel", 1, 1),
            new(
                "Ant Man",
                1,
                1,
                null,
                new OngoingAdjustPower<Card>(new SelfIfLocationFull(), new ConstantPower(3))
            ),
            new("Agent 13", 1, 2, new AddRandomCardToHand()),
            new("Misty Knight", 1, 2),
            new(
                "Rocket Raccoon",
                1,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(2))
            ),
            new CardDefinition(
                "Hawkeye",
                1,
                1,
                new CreateTemporaryEffect(
                    new(new(new CardPlayedHereNextTurn(), new GiveParentPowerBuilder(3)))
                )
            ),
            new(
                "Mantis",
                2,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new DrawOpponentCard())
            ),
            new("Medusa", 2, 2, new OnRevealIf(new InLocation(Column.Middle), new AddPowerSelf(3))),
            new("Shocker", 2, 3),
            new(
                "Star-Lord",
                2,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(3))
            ),
            new(
                "Angela",
                2,
                2,
                null,
                null,
                new TriggeredAbility<Card>(new OnPlayCardHereSameSide(), new AddPowerToSource(1))
            ),
            new(
                "Agent Coulson",
                3,
                4,
                new AddRandomCardToHand(new CardDefinitionFilters.CardsWithCost(4)).And(
                    new AddRandomCardToHand(new CardDefinitionFilters.CardsWithCost(5))
                )
            ),
            new(
                "Bishop",
                3,
                1,
                null,
                null,
                new TriggeredAbility<Card>(new OnPlayCardSameSide(), new AddPowerToSource(1))
            ),
            new("Cable", 3, 4, new DrawOpponentCard()),
            new("Cyclops", 3, 4),
            new(
                "Ironheart",
                3,
                0,
                new AddPowerRandomly(new SameSide().And(new OtherCards()), 2, 3)
            ),
            new(
                "Mister Fantastic",
                3,
                2,
                null,
                new OngoingAddLocationPower<Card>(new AdjacentToCard(), new ConstantPower(2))
            ),
            new(
                "Wolfsbane",
                3,
                1,
                new AddPowerSelf(
                    new PowerPerCard(
                        new OtherCards().And(new SameSide()).And(new SameLocation()),
                        2
                    )
                )
            ),
            new(
                "Ka-Zar",
                4,
                4,
                null,
                new OngoingAdjustPower<Card>(
                    new TargetFilters.CardsWithCost(1).And(new SameSide()),
                    new ConstantPower(1)
                )
            ),
            new("The Thing", 4, 6),
            new CardDefinition(
                "Jessica Jones",
                4,
                5,
                new CreateTemporaryEffect(
                    new(new(new NoCardPlayedHereNextTurn(), new GiveParentPowerBuilder(4)))
                )
            ),
            new CardDefinition(
                "Blue Marvel",
                5,
                3,
                null,
                new OngoingAdjustPower<Card>(
                    new OtherCards().And(new SameSide()),
                    new ConstantPower(1)
                )
            ),
            new("Gamora", 5, 7, new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(5))),
            new(
                "Klaw",
                5,
                4,
                null,
                new OngoingAddLocationPower<Card>(new ToTheRight(), new ConstantPower(6))
            ),
            new("Iron Man", 5, 0, null, new DoubleLocationPower()),
            new CardDefinition(
                "Spectrum",
                6,
                7,
                new AddPower(new SameSide().And(new WithOngoingAbility()), 2)
            ),
            new("Hulk", 6, 12)
        };

        public static IReadOnlyDictionary<string, CardDefinition> ByName = All.ToDictionary(cd =>
            cd.Name
        );
    }
}
