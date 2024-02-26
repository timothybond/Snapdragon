namespace Snapdragon.GeneticAlgorithm
{
    public record Experiment(
        Guid Id,
        string Name,
        DateTimeOffset Started,
        ISnapdragonRepository? Repository = null
    ) { }
}
