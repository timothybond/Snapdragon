using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record MergeSourceIntoTarget(ICard Source, ICard Target) : IEffect
    {
        public Game Apply(Game game)
        {
            var source = game.AllCards.SingleOrDefault(c => c.Id == Source.Id);
            var target = game.AllCards.SingleOrDefault(t => t.Id == Target.Id);

            if (source == null || target == null)
            {
                return game;
            }

            if (source.Column != target.Column)
            {
                // TODO: Determine if there are subsequent abilities that require this
                // (Blob, for example, if it uses the same effect, which doesn't even require in-play cards)
                throw new InvalidOperationException("Cannot merge cards in different locations.");
            }

            var location = game[source.Column].WithRemovedCard(source);
            return game.WithLocation(location)
                .WithModifiedCard(target, c => c with { Power = c.Power + source.Power })
                .WithEvent(new CardMergedEvent(game.Turn, source, target))
                .WithEvent(new CardRevealedEvent(game.Turn, target));
        }
    }
}
