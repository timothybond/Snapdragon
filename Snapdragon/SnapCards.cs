using Snapdragon.Calculations;
using Snapdragon.CardAbilities;
using Snapdragon.CardConditions;
using Snapdragon.TargetFilters;
using Snapdragon.TemporaryEffects;

namespace Snapdragon
{
    /// <summary>
    /// A collection of all of the defined cards for the actual game.
    /// </summary>
    public static class SnapCards
    {
        public static IReadOnlyList<CardDefinition> All = new List<CardDefinition>
        {
            new("Misty Knight", 1, 2),
            new(
                "Rocket Raccoon",
                1,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(2))
            ),
            new("Medusa", 2, 2, new OnRevealIf(new InLocation(Column.Middle), new AddPowerSelf(3))),
            new("Shocker", 2, 3),
            new(
                "Star-Lord",
                2,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(3))
            ),
            new("Gamora", 5, 7, new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(5))),
            new(
                "Ka-Zar",
                4,
                4,
                null,
                new OngoingAdjustPower<Card>(
                    new CardsWithCost(1).And(new SameSide()),
                    new Constant(1)
                )
            ),
            new CardDefinition(
                "Blue Marvel",
                5,
                3,
                null,
                new OngoingAdjustPower<Card>(new OtherCards().And(new SameSide()), new Constant(1))
            ),
            new CardDefinition(
                "Ant Man",
                1,
                1,
                new CreateTemporaryEffect(
                    new(new(new CardPlayedHereNextTurn(), new GiveParentPowerBuilder(3)))
                )
            ),
            new CardDefinition(
                "Jessica Jones",
                4,
                5,
                new CreateTemporaryEffect(
                    new(new(new NoCardPlayedHereNextTurn(), new GiveParentPowerBuilder(4)))
                )
            )
        };

        public static IReadOnlyDictionary<string, CardDefinition> ByName = All.ToDictionary(cd =>
            cd.Name
        );
    }
}
