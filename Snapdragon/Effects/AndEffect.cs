namespace Snapdragon.Effects
{
    public record AndEffect(IEnumerable<IEffect> Effects) : IEffect
    {
        public AndEffect(params IEffect[] effects)
            : this((IEnumerable<IEffect>)effects) { }

        public Game Apply(Game game)
        {
            foreach (var effect in Effects)
            {
                game = effect.Apply(game);
            }

            return game;
        }
    }
}
