namespace Snapdragon.Fluent.Ongoing
{
    /// <summary>
    /// Ongoing ability that doubles the effects of other ongoing abilities at a <see cref="Location"/>.
    ///
    /// When attached to a Card, only applies to one side of a <see cref="Location">.
    /// When attached to a <see cref="Location"/>, applies to both sides.
    ///
    /// When multiple of these apply at once, it's multiplicative
    /// (i.e., two will quadruple ongoing abilities, abilities, and four will octuple them).
    /// </summary>
    public record OngoingDoubleOtherOngoing<TContext> : Ongoing<TContext>
    {
        public OngoingDoubleOtherOngoing()
            : base(OngoingAbilityType.DoubleOngoingEffects) { }
    }
}
