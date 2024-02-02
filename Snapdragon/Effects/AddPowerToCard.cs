﻿using System.Collections.Immutable;

namespace Snapdragon.Effects
{
    public record AddPowerToCard(Card Card, int Amount) : IEffect
    {
        public Game Apply(Game game)
        {
            // Note: This assumes the Card.State value is up-to-date, even if other things aren't.
            switch (Card.State)
            {
                case CardState.InLibrary:
                    var library = game[Card.Side].Library;
                    library = new Library(
                        library.Cards.Select(c => this.ApplyToCard(c, game)).ToImmutableList()
                    );

                    var playerWithLibrary = game[Card.Side] with { Library = library };
                    return game.WithPlayer(playerWithLibrary);

                case CardState.InHand:
                    var hand = game[Card.Side].Hand;
                    hand = hand.Select(c => this.ApplyToCard(c, game)).ToImmutableList();

                    var playerWithHand = game[Card.Side] with { Hand = hand };
                    return game.WithPlayer(playerWithHand);
                case CardState.InPlay:
                case CardState.PlayedButNotRevealed:
                    if (!Card.Column.HasValue)
                    {
                        throw new InvalidOperationException(
                            $"Card state was {Card.State} but Column was null."
                        );
                    }

                    var location = game[Card.Column.Value];
                    switch (Card.Side)
                    {
                        case Side.Top:
                            var topPlayerCards = location
                                .TopPlayerCards.Select(c => this.ApplyToCard(c, game))
                                .ToImmutableList();
                            location = location with { TopPlayerCards = topPlayerCards };
                            return game.WithLocation(location);
                        case Side.Bottom:
                            var bottomPlayerCards = location
                                .BottomPlayerCards.Select(c => this.ApplyToCard(c, game))
                                .ToImmutableList();
                            location = location with { BottomPlayerCards = bottomPlayerCards };
                            return game.WithLocation(location);
                        default:
                            throw new NotImplementedException();
                    }
                case CardState.Destroyed:
                    var playerWithDestroyed = game[Card.Side];
                    var destroyed = playerWithDestroyed
                        .Destroyed.Select(c => this.ApplyToCard(c, game))
                        .ToImmutableList();

                    playerWithDestroyed = playerWithDestroyed with { Destroyed = destroyed };
                    return game.WithPlayer(playerWithDestroyed);
                case CardState.Discarded:
                    var playerWithDiscards = game[Card.Side];
                    var discards = playerWithDiscards
                        .Discards.Select(c => this.ApplyToCard(c, game))
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
        private Card ApplyToCard(Card possibleCard, Game game)
        {
            if (possibleCard.Id != Card.Id)
            {
                return possibleCard;
            }

            var blockedEffects = game.GetBlockedEffects(possibleCard);

            if (blockedEffects.Contains(EffectType.AdjustPower))
            {
                return possibleCard;
            }

            if (blockedEffects.Contains(EffectType.ReducePower) && Amount < 0)
            {
                return possibleCard;
            }

            return possibleCard with
            {
                Power = possibleCard.Power + Amount
            };
        }
    }
}