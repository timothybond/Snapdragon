using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record AddCardToRandomLocation(
        CardDefinition Definition,
        ILocationFilter<Card> LocationFilter
    ) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            // TODO: Handle restrictions on where cards can be added, number of slots
            var locations = new[] { game.Left, game.Middle, game.Right }
                .Where(l => LocationFilter.Applies(l, source, game))
                .Where(l => l[source.Side].Count < 4)
                .ToList();

            if (locations.Count == 0)
            {
                return game;
            }

            var location = Random.Of(locations);
            var effect = new AddCardToLocation(Definition, source.Side, location.Column);
            return effect.Apply(game);
        }
    }
}
