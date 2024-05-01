namespace Snapdragon.Fluent.Ongoing
{
    /// <summary>
    /// Ongoing ability that doubles the number of times that on-reveal abilities trigger at a given <see cref="Location">.
    ///
    /// When attached to a Card, only applies to one side of a <see cref="Location">.
    /// When attached to a <see cref="Location"/>, applies to both sides.
    ///
    /// When multiple of these apply at once, it's multiplicative
    /// (i.e., two will quadruple on-reveal abilities, and four will octuple them).
    /// </summary>
    public record OngoingDoubleOnReveal<TContext> : Ongoing<TContext> { }
}
