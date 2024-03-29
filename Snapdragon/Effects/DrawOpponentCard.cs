﻿namespace Snapdragon.Effects
{
    /// <summary>
    /// Effect where a player draws a card from their opponent's library.
    /// </summary>
    public record DrawOpponentCard(Side Side) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Side];
            var opponent = game[Side.Other()];

            if (player.Hand.Count < Max.HandSize && opponent.Library.Count > 0)
            {
                var card = opponent.Library[0];
                opponent = opponent with { Library = opponent.Library.RemoveAt(0) };

                player = player with { Hand = player.Hand.Add(card with { Side = Side }) };

                // TODO: Raise an event of some kind for this (although maybe not a normal "draw" one)
                return game.WithPlayer(player).WithPlayer(opponent);
            }

            return game;
        }
    }
}
