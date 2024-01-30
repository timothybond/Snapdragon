﻿using Snapdragon.Triggers;

namespace Snapdragon.TemporaryEffects
{
    public record CardPlayedHereNextTurn : ITriggerBuilder<TemporaryEffect<Card>>
    {
        public ITrigger Build(Game game, TemporaryEffect<Card> source)
        {
            return new OnPlayCard(source.Column, source.Side, game.Turn + 1);
        }
    }
}
