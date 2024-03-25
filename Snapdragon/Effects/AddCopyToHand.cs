﻿using Snapdragon.Events;
using Snapdragon.Fluent;

namespace Snapdragon.Effects
{
    /// <summary>
    /// Adds a copy of the given card to a <see cref="Snapdragon.Player"/>'s hand, with an optional transform.
    /// </summary>
    /// <param name="Card">The target card to copy.</param>
    /// <param name="Transform">An optional transform.</param>
    /// <param name="Player">The <see cref="Snapdragon.Player"/> to give the card to.
    /// If unset, defaults to the side of <see cref="Card"/>.</param>
    public record AddCopyToHand(ICard Card, ICardTransform? Transform = null, Player? Player = null)
        : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Player?.Side ?? Card.Side];

            // TODO: Determine if there's any scenarios where this isn't correct
            if (player.Hand.Count >= Max.HandSize)
            {
                return game;
            }

            // TODO: Get the card from wherever it currently is, so the state is up-to-date
            var card = (this.Transform?.Apply(Card) ?? Card).ToCardInstance() with
            {
                State = CardState.InHand,
                Id = Ids.GetNext<ICard>(),
                Side = player.Side
            };

            player = player with { Hand = player.Hand.Add(card) };
            return game.WithPlayer(player).WithEvent(new CardAddedToHandEvent(card, game.Turn));
        }
    }
}
