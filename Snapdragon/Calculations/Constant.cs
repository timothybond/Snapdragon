namespace Snapdragon.Calculations
{
    public record Constant(int Value) : ICalculation
    {
        public int GetValue(Game game)
        {
            return Value;
        }
    }

    public record Constant<T>(int Value) : ICalculation, IPowerCalculation<T>
    {
        public int GetValue(Game game)
        {
            return Value;
        }

        public int GetValue(Game game, T source, Card target)
        {
            return Value;
        }
    }
}
