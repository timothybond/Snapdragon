using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Events
{
    public record StartTurnEvent(int Turn) : Event(EventType.StartTurn)
    {
    }
}
