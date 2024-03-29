﻿using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    /// <summary>
    /// A trigger that fires when the source card is moved.
    /// </summary>
    public record OnMoved : ITrigger<ICard, CardMovedEvent>
    {
        public bool IsMet(CardMovedEvent e, Game game, ICard source)
        {
            return e.Card.Id == source.Id;
        }
    }
}
