﻿using System.Collections.Immutable;
using Snapdragon.Fluent.Ongoing;

namespace Snapdragon.Fluent.Builders
{
    public record BlockCardEffectFactory<TContext>(
        ISelector<ICardInstance, TContext> Selector,
        params EffectType[] EffectTypes
    ) : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingBlockCardEffect<TContext>(
                Selector,
                EffectTypes.ToImmutableList(),
                condition
            );
        }
    }
}
