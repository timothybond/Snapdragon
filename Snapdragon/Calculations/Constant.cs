namespace Snapdragon.Calculations
{
    public record Constant(int Value) : IPowerCalculation<Card>
    {
        public int GetValue(GameState game, Card source, Card target)
        {
            return this.Value;
        }
    }
}
