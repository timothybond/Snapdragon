namespace Snapdragon
{
    public record Card(
        int Id,
        CardDefinition Definition,
        string Name,
        int Cost,
        int Power,
        CardState State,
        Side Side,
        Column? Column,
        int? PowerAdjustment,
        IRevealAbility<Card>? OnReveal = null,
        IOngoingAbility<Card>? Ongoing = null,
        ITriggeredAbility<Card>? Triggered = null
    )
    {
        public Card(CardDefinition definition, Side side, CardState state = CardState.InLibrary)
            : this(
                Ids.GetNext<Card>(),
                definition,
                definition.Name,
                definition.Cost,
                definition.Power,
                state,
                side,
                null,
                null,
                definition.OnReveal,
                definition.Ongoing,
                definition.Triggered
            ) { }

        public int AdjustedPower => this.Power + (this.PowerAdjustment ?? 0);
    }
}
