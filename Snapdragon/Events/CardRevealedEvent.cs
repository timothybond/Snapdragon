using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Events
{
    public record CardRevealedEvent(int Turn, Card Card) : Event(EventType.CardRevealed) { }
}
