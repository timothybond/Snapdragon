using Snapdragon.Effects;

namespace Snapdragon.Fluent
{
    public record OnReveal<TContext>(
        IEffectBuilder<TContext> EffectBuilder,
        ICondition<TContext>? Condition = null
    )
    {
        public IEffect Apply(TContext context, Game game)
        {
            if (Condition?.IsMet(context, game) ?? true)
            {
                return EffectBuilder.Build(context, game);
            }

            return new NullEffect();
        }
    }
}
