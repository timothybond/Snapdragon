namespace Snapdragon.CardDefinitionFilters
{
    public record AnyCardDefinition : ICardDefinitionFilter
    {
        public bool Applies(CardDefinition cardDefinition)
        {
            return true;
        }
    }
}
