namespace Snapdragon
{
    public record ConstantCalculation(int Value) : ICalculation
    {
        public int GetValue(GameState game)
        {
            return this.Value;
        }
    }
}
