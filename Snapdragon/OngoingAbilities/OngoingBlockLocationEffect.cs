namespace Snapdragon.OngoingAbilities
{
    public record OngoingBlockLocationEffect<T>(EffectType EffectType, ILocationFilter<T> Filter)
        : IOngoingAbility<T>,
            ILocationFilter<T>
    {
        public bool Applies(Location location, T source, Game game)
        {
            return Filter.Applies(location, source, game);
        }
    }
}
