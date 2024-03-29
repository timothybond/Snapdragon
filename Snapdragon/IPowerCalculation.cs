namespace Snapdragon
{
    public interface IPowerCalculation<in T>
    {
        // TODO: Support power calculations from Locations, probably
        int GetValue(Game game, T source, ICardInstance target);
    }
}
