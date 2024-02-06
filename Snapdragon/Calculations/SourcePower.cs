namespace Snapdragon.Calculations
{
    public record SourcePower(int Value) : IPowerCalculation<Card>
    {
        public int GetValue(Game game, Card source, Card target)
        {
            return source.Power;
        }
    }
}
