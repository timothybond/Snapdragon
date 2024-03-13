using Snapdragon.Effects;

namespace Snapdragon.TriggeredAbilities
{
    // TODO: Either consolidate this with similarly-named types somehow,
    // or figure out a better naming scheme to differentiate it.
    public record AddCopyOfCardToHand(ISideFilter<ICardEvent> Sides) : ICardEventEffectBuilder
    {
        public IEffect Build(ICardEvent e, Game game)
        {
            var sides = All.Sides.Where(s => Sides.Applies(s, e, game));

            return sides
                .Select(s => new AddCopiesToHand(e.Card, 1, null, s))
                .Aggregate((IEffect)new NullEffect(), (acc, e) => new AndEffect(acc, e));
        }
    }
}
