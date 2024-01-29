namespace Snapdragon
{
    public interface IPowerCalculation<T>
    {
        // TODO: Support power calculations from Locations, probably
        int GetValue(GameState game, T source, Card target);
    }
}
