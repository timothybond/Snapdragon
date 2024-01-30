namespace Snapdragon.OngoingAbilities
{
    public record OngoingAddLocationPower<T>(
        ILocationFilter<T> LocationFilter,
        IPowerCalculation<T> Amount
    ) : IOngoingAbility<T> { }
}
