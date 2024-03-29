using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record ReturnAllDiscardsToPlay : IRevealAbility<ICard>
    {
        public Game Activate(Game game, ICard source)
        {
            var effects = game[source.Side]
                .Discards.Select(discard => new ReturnDiscardToRandomLocation(discard));

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
