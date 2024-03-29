using Snapdragon.Fluent;

namespace Snapdragon.OngoingAbilities
{
    /// <summary>
    /// Doubles the power of one side at a location (Iron Man's ability).  Logic for actually applying this is in the
    /// scoring
    /// </summary>
    public record DoubleLocationPower : Ongoing<ICard>, IOngoingAbility<ICard> { }
}
