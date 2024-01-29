namespace Snapdragon
{
    public record Card(
        int Id,
        CardDefinition Definition,
        string Name,
        int Cost,
        int Power,
        ICardAbility? Ability,
        CardState State,
        Side Side,
        Column? Column,
        int? PowerAdjustment
    )
    {
        public Card(CardDefinition definition, Side side, CardState state = CardState.InLibrary)
            : this(
                Ids.GetNext<Card>(),
                definition,
                definition.Name,
                definition.Cost,
                definition.Power,
                definition.Ability,
                state,
                side,
                null,
                null
            ) { }

        public int AdjustedPower => this.Power + (this.PowerAdjustment ?? 0);
    }
}
