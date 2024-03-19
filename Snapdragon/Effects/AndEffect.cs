namespace Snapdragon.Effects
{
    public record AndEffect(IEnumerable<IEffect> Effects) : IEffect
    {
        public AndEffect(params IEffect[] effects)
            : this((IEnumerable<IEffect>)effects) { }

        public Game Apply(Game game)
        {
            return Effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
