using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record DrawOpponentCardBuilder : IEffectBuilder<IObjectWithSide>
    {
        public IEffect Build(IObjectWithSide context, Game game)
        {
            return new DrawOpponentCard(context.Side);
        }
    }
}
