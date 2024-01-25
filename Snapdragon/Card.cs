namespace Snapdragon
{
    public record Card(int Id, CardDefinition Definition, string Name, int Cost, int Power, CardState State, Side Side)
    {
        public Card(CardDefinition definition, Side side, CardState state = CardState.InLibrary)
            : this(Ids.GetNext<Card>(), definition, definition.Name, definition.Cost, definition.Power, state, side) { }
    }
}