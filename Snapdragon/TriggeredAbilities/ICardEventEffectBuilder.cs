namespace Snapdragon.TriggeredAbilities
{
    public interface ICardEventEffectBuilder
    {
        IEffect Build(ICardEvent e, Game game);
    }
}
