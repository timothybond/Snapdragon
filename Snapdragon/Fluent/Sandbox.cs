using Snapdragon.Events;
using Snapdragon.Fluent.CardSelectors;

namespace Snapdragon.Fluent
{
    public static class Sandbox
    {
        public static void Test()
        {
            var medusaOnReveal = new CardRevealed()
                .If.InColumn(Column.Middle)
                .ModifyPower(new Self(), 3);

            var rocketRaccoonOnReveal = new CardRevealed().If.PastEvent.OfType<CardRevealedEvent>()
        }
    }
}
