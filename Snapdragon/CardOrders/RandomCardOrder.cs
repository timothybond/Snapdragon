namespace Snapdragon.CardOrders
{
    public record RandomCardOrder : ICardOrder
    {
        public int GetOrder(CardDefinition cardDefinition)
        {
            return Random.Next();
        }

        public override string ToString()
        {
            return "Random";
        }
    }
}
