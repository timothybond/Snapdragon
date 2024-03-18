using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.EffectBuilders;

namespace Snapdragon.Fluent
{
    public static class BuilderExtensions
    {
        public static TResult ModifyPower<TResult, TContext>(
            this IBuilder<TResult, TContext> builder,
            ICardSelector<TContext> cardSelector,
            int amount
        )
        {
            return builder.Build(new ModifyPowerBuilder<TContext>(cardSelector, amount));
        }
    }
}
