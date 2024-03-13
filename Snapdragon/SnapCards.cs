using System.Collections.Immutable;
using Snapdragon.Calculations;
using Snapdragon.CardConditions;
using Snapdragon.CardModifiers;
using Snapdragon.CardTriggers;
using Snapdragon.Events;
using Snapdragon.LocationFilters;
using Snapdragon.MoveAbilities;
using Snapdragon.OngoingAbilities;
using Snapdragon.PlayRestrictions;
using Snapdragon.RevealAbilities;
using Snapdragon.Sensors;
using Snapdragon.TargetFilters;
using Snapdragon.TriggeredEffects;
using Snapdragon.Triggers;

namespace Snapdragon
{
    // Note: In general I have tried to put cards here in order of cost,
    // with cards of equal cost sorted by name, for convenience.
    //
    // This might not be perfect, depending on how careful I was at the time.

    /// <summary>
    /// A collection of all of the defined cards for the actual game.
    /// </summary>
    public static class SnapCards
    {
        public static ImmutableList<CardDefinition> All = new List<CardDefinition>
        {
            new("Wasp", 0, 1),
            new(
                "Ant Man",
                1,
                1,
                null,
                new OngoingAdjustPower<Card>(new SelfIfLocationFull(), new Constant(3))
            ),
            new("Agent 13", 1, 2, new AddRandomCardToHand()),
            new("Blade", 1, 3, new DiscardCard(new RightmostCardInHand())),
            new(
                "Ebony Maw",
                1,
                7,
                null,
                new OngoingBlockLocationEffect<Card>(
                    EffectType.PlayCard,
                    new CardsHere(),
                    new SameSide()
                ),
                null,
                null,
                null,
                new CannotPlayAfterTurn(3)
            ),
            new(
                "Elektra",
                1,
                2,
                new DestroyRandomCardsInPlay<Card>(
                    new OtherSide().And(new CardsWithCost(1)).And(new CardsHere()),
                    1
                )
            ),
            new(
                "Hawkeye",
                1,
                1,
                new CreateSensor<CardPlayedEvent>(
                    new SensorBuilder<CardPlayedEvent>(
                        new SensorTriggeredAbilityBuilder<CardPlayedEvent>(
                            new CardPlayedHereNextTurn(),
                            new GiveParentPowerBuilder<CardPlayedEvent>(3)
                        )
                    )
                )
            ),
            new(
                "Human Torch",
                1,
                2,
                null,
                null,
                new TriggeredCardAbility<CardMovedEvent>(new OnMoved(), new DoubleSourcePower())
            ),
            new(
                "Iron Fist",
                1,
                2,
                new CreateSensor<CardRevealedEvent>(
                    new SensorBuilder<CardRevealedEvent>(
                        new SensorTriggeredAbilityBuilder<CardRevealedEvent>(
                            new CardRevealed(),
                            new MoveNextRevealedCardLeft(),
                            false // Hulkbuster merges don't work if we delete on first activation
                        )
                    )
                )
            ),
            new("Misty Knight", 1, 2),
            new("Nightcrawler", 1, 2, null, null, null, new CanMoveOnce()),
            new(
                "Rocket Raccoon",
                1,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(2))
            ),
            new(
                "Squirrel Girl",
                1,
                2,
                new AddCardsToLocations<Card>(
                    new CardDefinition("Squirrel", 1, 1),
                    new OtherLocations(),
                    new SameSide(),
                    1
                )
            ),
            new(
                "America Chavez",
                2,
                3,
                new AddPower(
                    new TopCardInLibrary<ICard>().And(new SameSide()),
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
                new TriggeredCardAbility<CardRevealedEvent>(
                    new OnRevealCardHereSameSide(),
                    new AddPowerToSource<CardRevealedEvent>(1)
                )
            ),
            new(
                "Armor",
                2,
                3,
                null,
                new OngoingBlockLocationEffect<Card>(EffectType.DestroyCard, new CardsHere())
            ),
            new(
                "Cloak",
                2,
                4,
                new CreateSensor<Event>(
                    new SensorBuilder<Event>(
                        new ExpiringTriggeredSensorBuilder<Event>(1),
                        new CanMoveToHereNextTurnBuilder<Card>()
                    )
                )
            ),
            new("Doctor Strange", 2, 3, new MoveCardsToSelf(new OtherHighestPowerCards())),
            new(
                "Kraven",
                2,
                2,
                null,
                null,
                new TriggeredCardAbility<CardMovedEvent>(
                    new OnCardMovedHere(),
                    new AddPowerToSource<CardMovedEvent>(2)
                )
            ),
            new("Hulkbuster", 2, 3, new MergeWithRandomCard()),
            new(
                "Multiple Man",
                2,
                3,
                null,
                null,
                new TriggeredCardAbility<CardMovedEvent>(new OnMoved(), new AddCopyToOldLocation())
            ),
            new(
                "Swarm",
                2,
                3,
                null,
                null,
                new WhenDiscarded(
                    new AddCopiesToHand<CardDiscardedEvent>(2, c => c with { Cost = 0 })
                )
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
                new TriggeredCardAbility<CardRevealedEvent>(
                    new OnRevealCardSameSide(),
                    new AddPowerToSource<CardRevealedEvent>(1)
                )
            ),
            new("Cable", 3, 4, new DrawOpponentCard()),
            new("Cyclops", 3, 4),
            new("Green Goblin", 3, -3, new SwitchSides()),
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
                new OngoingAddLocationPower<Card>(new AdjacentToCard(), new Constant(2))
            ),
            new("Nakia", 3, 3, new ModifyCardsInOwnerHand(new ModifyCardPower(1))),
            new("Sword Master", 3, 6, new DiscardCard()),
            new(
                "Vulture",
                3,
                3,
                null,
                null,
                new TriggeredCardAbility<CardMovedEvent>(
                    new OnMoved(),
                    new AddPowerToSource<CardMovedEvent>(5)
                )
            ),
            new(
                "Wolfsbane",
                3,
                1,
                new AddPowerSelf(
                    new PowerPerCard(new OtherCards().And(new SameSide()).And(new CardsHere()), 2)
                )
            ),
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
            new(
                "Jessica Jones",
                4,
                5,
                new CreateSensor<TurnEndedEvent>(
                    new SensorBuilder<TurnEndedEvent>(
                        new SensorTriggeredAbilityBuilder<TurnEndedEvent>(
                            new NoCardPlayedHereNextTurn(),
                            new GiveParentPowerBuilder<TurnEndedEvent>(4)
                        )
                    )
                )
            ),
            new("Mister Negative", 4, -1, new ModifyCardsInOwnerDeck(new SwapCostAndPower())),
            new(
                "Sentry",
                4,
                10,
                new AddCardsToLocations<Card>(
                    new CardDefinition("Void", 4, -10),
                    new SpecificLocation(Column.Right),
                    new SameSide()
                ),
                null,
                null,
                null,
                null,
                new CannotPlayInColumn(Column.Right)
            ),
            new("The Thing", 4, 6),
            new(
                "Blue Marvel",
                5,
                3,
                null,
                new OngoingAdjustPower<Card>(new OtherCards().And(new SameSide()), new Constant(1))
            ),
            new("Gamora", 5, 7, new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(5))),
            new("Hobgoblin", 5, -8, new SwitchSides()),
            new(
                "Klaw",
                5,
                4,
                null,
                new OngoingAddLocationPower<Card>(new ToTheRight(), new Constant(6))
            ),
            new(
                "White Tiger",
                5,
                1,
                new AddCardToRandomLocation(
                    new CardDefinition("Tiger Spirit", 5, 8),
                    new OtherLocations()
                )
            ),
            new("Iron Man", 5, 0, null, new DoubleLocationPower()),
            new(
                "Apocalypse",
                6,
                8,
                null,
                null,
                new WhenDiscarded(
                    new ReturnCardToHand<CardDiscardedEvent>(c => c with { Power = c.Power + 4 })
                )
            ),
            new("Spectrum", 6, 7, new AddPower(new SameSide().And(new WithOngoingAbility()), 2)),
            new("Heimdall", 6, 9, new MoveCardsLeft(new OtherCards().And(new SameSide()))),
            new("Hulk", 6, 12)
        }
            .OrderBy(c => c.Cost)
            .ThenBy(c => c.Name)
            .ToImmutableList();

        public static ImmutableDictionary<string, CardDefinition> ByName =
            All.ToImmutableDictionary(cd => cd.Name);
    }
}
