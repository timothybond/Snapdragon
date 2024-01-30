namespace Snapdragon.Calculations
{
    public record ConstantPower(int Value) : IPowerCalculation<Card>
    {
        public int GetValue(Game game, Card source, Card target)
        {
            return this.Value;
        }
    }
}
