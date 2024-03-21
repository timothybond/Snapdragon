namespace Snapdragon.TriggeredAbilities
{
    public interface ICardEventEffectBuilder
    {
        IEffect Build(CardEvent e, Game game);
    }
}
