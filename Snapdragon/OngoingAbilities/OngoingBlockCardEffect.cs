﻿namespace Snapdragon.OngoingAbilities
{
    public record OngoingBlockCardEffect<T>(EffectType EffectType, ICardFilter<T> Filter)
        : IOngoingAbility<T>,
            ICardFilter<T>
    {
        public bool Applies(ICard card, T source, Game game)
        {
            return Filter.Applies(card, source, game);
        }
    }
}
