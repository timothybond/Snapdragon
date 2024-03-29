﻿using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record AddCardToLocation(CardDefinition Definition, Side Side, Column Column) : IEffect
    {
        public Game Apply(Game game)
        {
            var card = new CardInstance(Definition, Side, CardState.InPlay) with
            {
                Column = Column
            };

            // TODO: Handle restrictions on slots
            if (game[Column][Side].Count < Max.CardsPerLocation)
            {
                var location = game[Column];
                location = location.WithCard(card);

                // TODO: Decide what event, if any, to raise for cards that were added but not played
                return game.WithLocation(location)
                    .WithEvent(new CardAddedToLocationEvent(card, Column, game.Turn));
            }

            return game;
        }
    }
}
