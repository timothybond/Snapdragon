namespace Snapdragon
{
    /// <summary>
    /// Gives the count of how many times a particular card appears in each generation of a population.
    /// </summary>
    /// <param name="Name">Card name.</param>
    /// <param name="Counts">Per-generation counts of the card.</param>
    public record CardCount(string Name, IReadOnlyList<int> Counts) { }
}
