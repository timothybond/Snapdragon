using Snapdragon.Fluent;

namespace Snapdragon
{
    public record LocationDefinition(
        string Name,
        OnReveal<Location>? OnReveal = null,
        Ongoing<Location>? Ongoing = null,
        ITriggeredAbility<Location>? Triggered = null,
        IMoveAbility<Location>? MoveAbility = null
    ) { }
}
