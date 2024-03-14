using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record ReturnAllDiscardsToPlay : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var effects = game[source.Side]
                .Discards.Select(discard => new ReturnDiscardToRandomLocation(discard));

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
