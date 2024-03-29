namespace Snapdragon
{
    public interface IMoveAbility<T>
    {
        bool CanMove(ICard target, T source, Column destination, Game game);
    }
}
