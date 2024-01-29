namespace Snapdragon
{
    public interface IPowerCalculation
    {
        // TODO: Support power calculations from Locations, probably
        int GetValue(GameState game, Card source, Card target);
    }
}
