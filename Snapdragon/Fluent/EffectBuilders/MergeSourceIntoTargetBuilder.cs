using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record MergeSourceIntoTargetBuilder<TContext>(
        ISingleItemSelector<ICard, TContext> Source,
        ISingleItemSelector<ICard, TContext> Target
    ) : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var source = Source.GetOrDefault(context, game);
            var target = Target.GetOrDefault(context, game);

            if (source == null || target == null)
            {
                return new NullEffect();
            }

            return new MergeSourceIntoTarget(source, target);
        }
    }

    public static class MergeSourceIntoTargetExtensions
    {
        public static MergeSourceIntoTargetBuilder<TContext> MergeInto<TContext>(
            this ISingleItemSelector<ICard, TContext> source,
            ISingleItemSelector<ICard, TContext> target
        )
        {
            return new MergeSourceIntoTargetBuilder<TContext>(source, target);
        }
    }
}
