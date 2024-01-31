namespace Snapdragon
{
    public interface ICardDefinitionFilter
    {
        bool Applies(CardDefinition cardDefinition);
    }
}
