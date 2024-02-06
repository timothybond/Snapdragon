namespace Snapdragon
{
    public interface IMoveAbility<T>
    {
        bool CanMove(Card target, T source, Column destination, Game game);
    }
}
