namespace Snapdragon
{
    public record CardDefinition(
        string Name,
        int Cost,
        int Power,
        IAbility<Card>? Ability = null
    ) { }
}
