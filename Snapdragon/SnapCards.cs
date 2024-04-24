using System.Collections.Immutable;
using Snapdragon.Events;
using Snapdragon.Fluent;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;
using Snapdragon.Fluent.Transforms;
using Snapdragon.MoveAbilities;
using Snapdragon.PlayRestrictions;

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
        public static SnapCardsSelector PossibleCards => new SnapCardsSelector();

        /// <summary>
        /// Self-reference for fluent abilities. Provided here for convenience.
        /// </summary>
        public static Fluent.Selectors.Self Self => new Fluent.Selectors.Self();

        /// <summary>
        /// Convenience reference for the builder for <see cref="OnReveal{Card}"/> abilities.
        /// </summary>
        public static Fluent.CardRevealed OnReveal => new Fluent.CardRevealed();

        /// <summary>
        /// Convenience reference for the builder for <see cref="Ongoing{Card}"/> abilities.
        /// </summary>
        public static Fluent.CardOngoing Ongoing => new Fluent.CardOngoing();

        public static HereFilter Here => new HereFilter();

        public static readonly ImmutableList<CardDefinition> All = new List<CardDefinition>
        {
            new("Wasp", 0, 1),
            new("Ant Man", 1, 1, null, Ongoing.If.LocationFull().AdjustPower(Self, 3)),
            new(
                "Agent 13",
                1,
                2,
                OnReveal.AddToHand(new RandomSingleItem<CardDefinition, ICard>(), My.Self) // TODO: Simplify
            ),
            new("Blade", 1, 3, OnReveal.Discard(My.Hand.Last())),
            new(
                "Ebony Maw",
                1,
                7,
                null,
                new CardOngoing()
                    .Block(EffectType.PlayCard)
                    .ForLocationAndSide(At.Here, new Fluent.Selectors.SameSide()),
                null,
                null,
                null,
                new CannotPlayAfterTurn(3)
            ),
            new(
                "Elektra",
                1,
                2,
                // TODO: Make sure not to select indestructible items
                OnReveal.Destroy(Opposing.Cards.Here().WithCost(1).GetRandom())
            ),
            new(
                "Hawkeye",
                1,
                1,
                If.NextTurnEvent<CardPlayedEvent>()
                    .Where(EventCard.AtSensor.And(EventCard.SameSide))
                    .ModifyPower(new SourceCard(), 3)
            ),
            new(
                "Human Torch",
                1,
                2,
                null,
                null,
                When.InPlayAnd<CardMovedEvent>().Where(EventCard.Self).Then(Self.DoublePower())
            ),
            new("Iron Fist", 1, 2, When.NextCardRevealed(EventCard.Get.MoveLeft())),
            new("Misty Knight", 1, 2),
            new("Nightcrawler", 1, 2, null, null, null, new CanMoveOnce()),
            new(
                "Rocket Raccoon",
                1,
                2,
                OnReveal
                    .If.PastEvent()
                    .OfType<CardPlayedEvent>()
                    .Where(EventCard.OtherSide.And(EventCard.Here))
                    .Then(Self.ModifyPower(2))
            ),
            new(
                "Squirrel Girl",
                1,
                2,
                OnReveal.Then(
                    new OtherLocations<ICard>().AddCard(
                        new CardDefinition("Squirrel", 1, 1),
                        My.Self
                    )
                )
            ),
            new("America Chavez", 2, 3, OnReveal.Then(My.Library.First().ModifyPower(2))),
            new("Hazmat", 2, 2, OnReveal.Then(Fluent.Selectors.All.OtherCards.ModifyPower(-1))),
            new(
                "Mantis",
                2,
                2,
                OnReveal.Then(
                    Opposing
                        .CardsIncludingUnrevealed.Here()
                        .PlayedThisTurn()
                        .GetRandom()
                        .CopyToHand(My.Self)
                )
            ),
            new("Medusa", 2, 2, OnReveal.If.InColumn(Column.Middle).Then(Self.ModifyPower(3))),
            new("Okoye", 2, 2, OnReveal.ModifyPower(My.Library, 1)),
            new("Scorpion", 2, 2, OnReveal.ModifyPower(Opposing.Hand, -1)),
            new("Shocker", 2, 3),
            new(
                "Star-Lord",
                2,
                2,
                OnReveal
                    .If.PastEvent()
                    .OfType<CardPlayedEvent>()
                    .Where(EventCard.OtherSide.And(EventCard.Here))
                    .Then(Self.ModifyPower(3))
            ),
            new(
                "Angela",
                2,
                2,
                null,
                null,
                When.InPlayAnd<CardRevealedEvent>()
                    .Where(EventCard.SameSide.And(EventCard.OtherCards).And(EventCard.Here))
                    .Then(Self.ModifyPower(1))
            ),
            new("Armor", 2, 3, null, Ongoing.Block(EffectType.DestroyCard).ForLocation(Here)),
            new("Cloak", 2, 4, NextTurn.CanMoveHere()),
            new("Colleen Wing", 2, 4, OnReveal.Discard(My.Hand.WithMinCost().GetRandom())),
            new("Doctor Strange", 2, 3, OnReveal.Then(My.OtherCards.WithMaxPower().MoveToHere())),
            new(
                "Kraven",
                2,
                2,
                null,
                null,
                When.InPlayAnd<CardMovedEvent>().Where(Moved.ToHere).Then(Self.ModifyPower(2))
            ),
            // TODO: Implement
            new(
                "Hulkbuster",
                2,
                3,
                OnReveal.Then(Self.MergeInto(My.OtherCards.Here().GetRandom()))
            ),
            new(
                "Multiple Man",
                2,
                3,
                null,
                null,
                When.InPlayAnd<CardMovedEvent>()
                    .Where(EventCard.Self)
                    .Then(Self.CopyToLocation(At.PriorLocation))
            ),
            new(
                "Swarm",
                2,
                3,
                null,
                null,
                When.Discarded.Then(Self.CopyToHand(new WithZeroCost()).Times(2))
            ),
            new(
                "Agent Coulson",
                3,
                4,
                OnReveal.Then(
                    My.Self.AddToHand(PossibleCards.WithCost(4).GetRandom())
                        .And(My.Self.AddToHand(PossibleCards.WithCost(5).GetRandom()))
                )
            ),
            new(
                "Bishop",
                3,
                1,
                null,
                null,
                When.InPlayAnd<CardRevealedEvent>()
                    .Where(EventCard.SameSide.And(EventCard.OtherCards))
                    .Then(Self.ModifyPower(1))
            ),
            new("Cable", 3, 4, OnReveal.Then(new DrawOpponentCardBuilder())),
            new("Cyclops", 3, 4),
            new(
                "Debrii",
                3,
                3,
                OnReveal.Then( // TODO: Shorten this somehow
                    new AddCardToLocationBuilder<ICard>(
                        new CardDefinition("Rock", 1, 0),
                        new OtherLocations<ICard>(),
                        new BothPlayers()
                    )
                )
            ),
            new("Green Goblin", 3, -3, OnReveal.Then(Self.SwitchSides())),
            new("Ironheart", 3, 0, OnReveal.ModifyPower(My.OtherCards.GetRandom(3), 2)),
            new("Lady Sif", 3, 5, OnReveal.Discard(My.Hand.WithMaxCost().GetRandom())),
            new(
                "Luke Cage",
                3,
                4,
                null,
                new OngoingBlockCardEffect<ICard>(My.Cards, [EffectType.ReducePower])
            ),
            new(
                "Mister Fantastic",
                3,
                2,
                null,
                Ongoing.AdjustLocationPower(new AdjacentLocations(), 2)
            ),
            new("Nakia", 3, 3, OnReveal.ModifyPower(My.Hand, 1)),
            new("Sword Master", 3, 6, OnReveal.Discard(My.Hand.GetRandom())),
            new(
                "Vulture",
                3,
                3,
                null,
                null,
                When.InPlayAnd<CardMovedEvent>().Where(EventCard.Self).Then(Self.ModifyPower(5))
            ),
            new(
                "Wolfsbane",
                3,
                1,
                OnReveal.Then(Self.ModifyPower(2).Times(My.OtherCards.Here().Count()))
            ),
            new("Ghost Rider", 4, 3, OnReveal.Then(My.Discards.GetRandom().ReturnDiscardTo(Here))),
            new("Ka-Zar", 4, 4, null, Ongoing.AdjustPower(My.OtherCards.WithCost(1), 1)),
            new(
                "Jessica Jones",
                4,
                5,
                If.NoNextTurnEvent<CardPlayedEvent>()
                    .Where(EventCard.Here.And(EventCard.SameSide))
                    .ModifyPower(new SourceCard(), 4)
            ),
            new("Mister Negative", 4, -1, OnReveal.Then(My.Library.SwapCostAndPower())),
            new(
                "Sentry",
                4,
                10,
                OnReveal.Then(
                    new SpecificLocation(Column.Right).AddCard(new CardDefinition("Void", 4, -10))
                ),
                null,
                null,
                null,
                null,
                new CannotPlayInColumn(Column.Right)
            ),
            new("The Thing", 4, 6),
            new("Blue Marvel", 5, 3, null, Ongoing.AdjustPower(My.OtherCards, 1)),
            new(
                "Gamora",
                5,
                8,
                OnReveal
                    .If.PastEvent()
                    .OfType<CardPlayedEvent>()
                    .Where(EventCard.OtherSide.And(EventCard.Here))
                    .Then(Self.ModifyPower(4))
            ),
            new("Hobgoblin", 5, -8, OnReveal.Then(Self.SwitchSides())),
            new("Klaw", 5, 4, null, Ongoing.AdjustLocationPower(new LocationToTheRight(), 6)),
            new(
                "White Tiger",
                5,
                1,
                OnReveal.Then(
                    new OtherLocations<ICard>()
                        .WithOpenSlots(My.Self)
                        .GetRandom()
                        .AddCard(new CardDefinition("Tiger Spirit", 5, 8))
                )
            ),
            new("Iron Man", 5, 0, null, new OngoingAbilities.DoubleLocationPower()),
            new(
                "Apocalypse",
                6,
                8,
                null,
                null,
                When.Discarded.Then(Self.ModifyPower(4).And(Self.ReturnToHand()))
            ),
            new(
                "Spectrum",
                6,
                7,
                OnReveal.Then(My.OtherCards.WithOngoingAbilities().ModifyPower(2))
            ),
            new("Heimdall", 6, 9, OnReveal.Then(My.OtherCards.MoveLeft())),
            new(
                "Hela",
                6,
                6,
                OnReveal.Then(
                    My.Discards.First()
                        .ReturnDiscardTo(new AllLocations().WithOpenSlots(My.Self).GetRandom())
                        .Times(My.Discards.Count())
                )
            ),
            new("Hulk", 6, 12)
        }
            .OrderBy(c => c.Cost)
            .ThenBy(c => c.Name)
            .ToImmutableList();

        public static readonly ImmutableDictionary<string, CardDefinition> ByName =
            All.ToImmutableDictionary(cd => cd.Name);

        public static void Sandbox()
        {
            var withCost = new WithCost(4);

            FilterExtensions.GetRandom<CardDefinition, object>(withCost);
            FilterExtensions.GetRandom((IFilter<CardDefinition, object>)withCost);
        }
    }
}
