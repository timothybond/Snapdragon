using Snapdragon.Calculations;
using Snapdragon.CardAbilities;
using Snapdragon.CardConditions;
using Snapdragon.TargetFilters;

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
            new("Shocker", 2, 3),
            new(
                "Star-Lord",
                2,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(3))
            ),
            new("Gamora", 5, 7, new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(5))),
            new("Ka-Zar", 4, 4, new OngoingAdjustPower(new CardsWithCost(1), new Constant(1))),
            new CardDefinition(
                "Blue Marvel",
                5,
                3,
                new OngoingAdjustPower(new OtherCards(), new Constant(1))
            )
        };

        public static IReadOnlyDictionary<string, CardDefinition> ByName = All.ToDictionary(cd =>
            cd.Name
        );
    }
}
