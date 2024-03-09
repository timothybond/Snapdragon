namespace Snapdragon.Calculations
{
    public record SourcePower(int Value) : IPowerCalculation<CardInstance>
    {
        public int GetValue(Game game, CardInstance source, ICard target)
        {
            return source.Power;
        }
    }
}
