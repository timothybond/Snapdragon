namespace Snapdragon.RevealAbilities
{
    public static class RevealAbilityExtensions
    {
        public static IRevealAbility<T> And<T>(
            this IRevealAbility<T> First,
            IRevealAbility<T> Second
        )
        {
            return new AndOnReveal<T>(First, Second);
        }
    }
}
