namespace Snapdragon
{
    public interface IPowerCalculation<T>
    {
        // TODO: Support power calculations from Locations, probably
        int GetValue(Game game, T source, ICard target);
    }
}
