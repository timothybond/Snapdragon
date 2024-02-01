namespace Snapdragon.Calculations
{
    public record ConstantPower<T>(int Value) : IPowerCalculation<T>
    {
        public int GetValue(Game game, T source, Card target)
        {
            return this.Value;
        }
    }
}
