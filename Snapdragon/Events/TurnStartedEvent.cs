using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Events
{
    public record TurnStartedEvent(int Turn) : Event(EventType.TurnStarted) { }
}
