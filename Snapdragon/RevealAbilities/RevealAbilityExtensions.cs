namespace Snapdragon.RevealAbilities
{
    public static class RevealAbilityExtensions
    {
        public static IRevealAbility<Card> And(
            this IRevealAbility<Card> First,
            IRevealAbility<Card> Second
        )
        {
            return new AndOnReveal(First, Second);
        }
    }
}
