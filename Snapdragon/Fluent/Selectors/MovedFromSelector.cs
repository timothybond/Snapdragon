﻿using Snapdragon.Events;

namespace Snapdragon.Fluent.Selectors
{
    public record MovedFromSelector : ILocationSelector<CardMovedEvent, object>
    {
        public IEnumerable<Location> Get(CardMovedEvent e, object context, Game game)
        {
            yield return game[e.From];
        }
    }
}
