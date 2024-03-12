namespace Snapdragon
{
    public record LocationDefinition(
        string Name,
        IRevealAbility<Location>? OnReveal = null,
        IOngoingAbility<Location>? Ongoing = null,
        ITriggeredAbility<Location>? Triggered = null,
        IMoveAbility<Location>? MoveAbility = null
    ) { }
}
