﻿using System.Collections.Immutable;

namespace Snapdragon.Effects
{
    public record AddPowerToCard(ICard Card, int Amount) : IEffect
    {
        public Game Apply(Game game)
        {
            // Note: This assumes the Card.State value is up-to-date, even if other things aren't.
            switch (Card.State)
            {
                case CardState.InLibrary:
                    var library = game[Card.Side].Library;
                    library = new Library(
                        library
                            .Cards.Select(c => this.ApplyToCard(c, game).ToCardInstance())
                            .ToImmutableList()
                    );

                    var playerWithLibrary = game[Card.Side] with { Library = library };
                    return game.WithPlayer(playerWithLibrary);

                case CardState.InHand:
                    var hand = game[Card.Side].Hand;
                    hand = hand.Select(c => this.ApplyToCard(c, game).ToCardInstance())
                        .ToImmutableList();

                    var playerWithHand = game[Card.Side] with { Hand = hand };
                    return game.WithPlayer(playerWithHand);
                case CardState.InPlay:
                case CardState.PlayedButNotRevealed:
                    if (!Card.Column.HasValue)
                    {
                        // Note: normally effects fail silently, but this would indicate a corrupt Game/Location instance
                        throw new InvalidOperationException(
                            $"Card state was {Card.State} but Column was null."
                        );
                    }

                    var location = game[Card.Column.Value];
                    var actualCard = location.AllCards.SingleOrDefault(c => c.Id == Card.Id);

                    if (actualCard == null)
                    {
                        return game;
                    }

                    // TODO: See if we can clear up the weird round-trip here
                    return game.WithModifiedCard(
                        actualCard,
                        c => this.ApplyToCard(c, game).InPlayAt(c.Column)
                    );
                case CardState.Destroyed:
                    var playerWithDestroyed = game[Card.Side];
                    var destroyed = playerWithDestroyed
                        .Destroyed.Select(c => this.ApplyToCard(c, game).ToCardInstance())
                        .ToImmutableList();

                    playerWithDestroyed = playerWithDestroyed with { Destroyed = destroyed };
                    return game.WithPlayer(playerWithDestroyed);
                case CardState.Discarded:
                    var playerWithDiscards = game[Card.Side];
                    var discards = playerWithDiscards
                        .Discards.Select(c => this.ApplyToCard(c, game).ToCardInstance())
                        .ToImmutableList();

                    playerWithDiscards = playerWithDiscards with { Discards = discards };
                    return game.WithPlayer(playerWithDiscards);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Helper function that applies the effect to a given card, if it's a match and nothing is preventing it.
        /// </summary>
        /// <param name="possibleCard">A card to which the effect might (but does not necessarily) apply.</param>
        /// <param name="game">The overall game state.</param>
        /// <returns>The modified card (or, for a non-match or blocked effect, the original card).</returns>
        private ICard ApplyToCard(ICard possibleCard, Game game)
        {
            if (possibleCard.Id != Card.Id)
            {
                return possibleCard;
            }

            var blockedEffects = possibleCard is Card possibleCardInPlay
                ? game.GetBlockedEffects(possibleCardInPlay)
                : new HashSet<EffectType>();

            if (possibleCard.Column != null)
            {
                blockedEffects = blockedEffects
                    .Concat(game.GetBlockedEffects(possibleCard.Column.Value, possibleCard.Side))
                    .ToHashSet();
            }

            if (blockedEffects.Contains(EffectType.AdjustPower))
            {
                return possibleCard;
            }

            if (blockedEffects.Contains(EffectType.ReducePower) && Amount < 0)
            {
                return possibleCard;
            }

            // Note: this could be a bit of a weird roundtrip in many cases,
            // but is necessary due to the current state of the types that
            // differentiate in-play cards from those in hands/libraries.
            return possibleCard.ToCardInstance() with
            {
                Power = possibleCard.Power + Amount
            };
        }
    }
}
