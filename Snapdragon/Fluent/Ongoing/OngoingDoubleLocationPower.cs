namespace Snapdragon.Fluent.Ongoing
{
    /// <summary>
    /// Doubles the power of one side at a location (Iron Man's ability).  Logic for actually applying this is in the
    /// scoring
    /// </summary>
    public record OngoingDoubleLocationPower : Ongoing<ICard>, IOngoingAbility<ICard>
    {
        public OngoingDoubleLocationPower()
            : base(OngoingAbilityType.DoubleLocationPower) { }
    }
}
