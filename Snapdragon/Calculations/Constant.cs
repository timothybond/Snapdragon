namespace Snapdragon.Calculations
{
    public record Constant(int Value) : IPowerCalculation
    {
        public int GetValue(GameState game, Card source, Card target)
        {
            return this.Value;
        }
    }
}
