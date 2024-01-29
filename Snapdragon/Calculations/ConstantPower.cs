namespace Snapdragon.Calculations
{
    public record ConstantPower(int Value) : IPowerCalculation<Card>
    {
        public int GetValue(GameState game, Card source, Card target)
        {
            return this.Value;
        }
    }
}
