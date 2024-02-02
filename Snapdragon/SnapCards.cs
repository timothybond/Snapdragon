using Snapdragon.Calculations;
using Snapdragon.CardConditions;
using Snapdragon.CardTriggers;
using Snapdragon.LocationFilters;
using Snapdragon.OngoingAbilities;
using Snapdragon.RevealAbilities;
using Snapdragon.Sensors;
using Snapdragon.TargetFilters;
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
                new OngoingAdjustPower<Card>(new SelfIfLocationFull(), new ConstantPower<Card>(3))
            ),
            new("Agent 13", 1, 2, new AddRandomCardToHand()),
            new("Blade", 1, 3, new DiscardCard(new RightmostCardInHand<Card>())),
            new(
                "Elektra",
                1,
                2,
                new DestroyRandomCardsInPlay<Card>(
                    new OtherSide().And(new CardsWithCost(1)).And(new SameLocation()),
                    1
                )
            ),
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
                new CreateSensor(
                    new(new(new CardPlayedHereNextTurn(), new GiveParentPowerBuilder(3)))
                )
            ),
            new(
                "Squirrel Girl",
                1,
                2,
                new AddCardsToLocations<Card>(
                    new CardDefinition("Squirrel", 1, 1),
                    new OtherLocations(),
                    new SideFilters.SameSide(),
                    1
                )
            ),
            new(
                "America Chavez",
                2,
                3,
                new AddPower(
                    new TopCardInLibrary<Card>().And(new SameSide()),
                    2,
                    CardState.InLibrary
                )
            ),
            new(
                "Mantis",
                2,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new DrawOpponentCard())
            ),
            new("Medusa", 2, 2, new OnRevealIf(new InLocation(Column.Middle), new AddPowerSelf(3))),
            new("Okoye", 2, 2, new AddPower(new SameSide(), 1, CardState.InLibrary)),
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
                "Swarm",
                2,
                3,
                null,
                null,
                new WhenDiscarded(new AddCopiesToHand(2, c => c with { Cost = 0 }))
            ),
            new(
                "Armor",
                2,
                3,
                null,
                new OngoingBlockLocationEffect<Card>(EffectType.DestroyCard, new SameLocation())
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
                new AddPowerRandomly<Card>(new SameSide().And(new OtherCards()), 2, 3)
            ),
            new("Lady Sif", 3, 5, new DiscardCard(new TopCostInHand<Card>())),
            new(
                "Mister Fantastic",
                3,
                2,
                null,
                new OngoingAddLocationPower<Card>(new AdjacentToCard(), new ConstantPower<Card>(2))
            ),
            new("Sword Master", 3, 6, new DiscardCard()),
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
                    new ConstantPower<Card>(1)
                )
            ),
            new("The Thing", 4, 6),
            new CardDefinition(
                "Jessica Jones",
                4,
                5,
                new CreateSensor(
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
                    new ConstantPower<Card>(1)
                )
            ),
            new("Gamora", 5, 7, new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(5))),
            new(
                "Klaw",
                5,
                4,
                null,
                new OngoingAddLocationPower<Card>(new ToTheRight(), new ConstantPower<Card>(6))
            ),
            new("White Tiger", 5, 1, new AddCardToRandomLocation(new CardDefinition("Tiger Spirit", 5, 8), new OtherLocations())),
            new("Iron Man", 5, 0, null, new DoubleLocationPower()),
            new(
                "Apocalypse",
                6,
                8,
                null,
                null,
                new WhenDiscarded(new ReturnCardToHand(c => c with { Power = c.Power + 4 }))
            ),
            new("Spectrum", 6, 7, new AddPower(new SameSide().And(new WithOngoingAbility()), 2)),
            new("Hulk", 6, 12)
        };

        public static IReadOnlyDictionary<string, CardDefinition> ByName = All.ToDictionary(cd =>
            cd.Name
        );
    }
}
