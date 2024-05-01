namespace Snapdragon
{
    /// <summary>
    /// Holds the ratios for things that can be, generally, doubled by other effects (sometimes repeatedly) at a given <see cref="Location"/> and <see cref="Side"/>.
    /// </summary>
    /// <param name="OnReveal">The number of times on-reveal abilities will trigger.</param>
    /// <param name="Ongoing">The number of times to apply ongoing abilities.</param>
    public record Multipliers(int OnReveal = 1, int Ongoing = 1) { }
}
