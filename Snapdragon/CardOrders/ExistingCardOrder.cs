namespace Snapdragon.CardOrders
{
    public record ExistingCardOrder() : ICardOrder
    {
        public int GetOrder(CardDefinition cardDefinition)
        {
            // TODO: Verify this works
            return 0;
        }

        public override string ToString()
        {
            return "Existing";
        }
    }
}
