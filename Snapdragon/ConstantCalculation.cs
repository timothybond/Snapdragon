namespace Snapdragon
{
    public record ConstantCalculation(int Value) : ICalculation
    {
        public int GetValue(Game game)
        {
            return this.Value;
        }
    }
}
