﻿using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record ReturnToHandBuilder<TContext>(ISelector<ICardInstance, TContext> CardSelector)
        : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game).ToList();

            if (cards.Count == 0)
            {
                return new NullEffect();
            }

            return new AndEffect(cards.Select(c => new ReturnCardToHand(c)));
        }
    }

    public static class ReturnToHandExtensions
    {
        public static ReturnToHandBuilder<TContext> ReturnToHand<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector
        )
            where TContext : class
        {
            return new ReturnToHandBuilder<TContext>(cardSelector);
        }
    }
}
