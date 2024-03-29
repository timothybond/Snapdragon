namespace Snapdragon.Calculations
{
    public record Constant(int Value) : ICalculation, IPowerCalculation<object>
    {
        public int GetValue(Game game)
        {
            return Value;
        }

        public int GetValue(Game game, object source, ICardInstance target)
        {
            return Value;
        }
    }
}
