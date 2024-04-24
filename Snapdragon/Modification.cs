namespace Snapdragon
{
    /// <summary>
    /// A semi-permanent alteration to the Cost and/or Power of a <see cref="CardBase"/>.
    /// </summary>
    public record Modification(int? CostChange, int? PowerChange, object Source) { }
}
