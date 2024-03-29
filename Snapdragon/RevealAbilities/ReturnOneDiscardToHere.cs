using Snapdragon.Effects;

namespace Snapdragon.RevealAbilities
{
    public record ReturnOneDiscardToHere : IRevealAbility<ICard>
    {
        public Game Activate(Game game, ICard source)
        {
            if (game[source.Side].Discards.Count == 0)
            {
                return game;
            }

            var effect = new ReturnDiscardToLocation(
                Random.Of(game[source.Side].Discards),
                source.Column
            );
            return effect.Apply(game);
        }
    }
}
