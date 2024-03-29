namespace Snapdragon.Calculations
{
    public record SourcePower(int Value) : IPowerCalculation<ICardInstance>
    {
        public int GetValue(Game game, ICardInstance source, ICardInstance target)
        {
            return source.Power;
        }
    }
}
