namespace Snapdragon.OngoingAbilities
{
    public record OngoingBlockLocationEffect<T>(
        EffectType EffectType,
        ILocationFilter<T> Filter,
        ISideFilter<T>? SideFilter = null,
        IGameFilter? GameFilter = null
    ) : IOngoingAbility<T>
    {
        public bool Applies(Location location, Side side, T source, Game game)
        {
            return Filter.Applies(location, source, game)
                && (SideFilter?.Applies(side, source, game) ?? true)
                && (GameFilter?.Applies(game) ?? true);
        }
    }
}
