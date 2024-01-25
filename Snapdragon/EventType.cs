﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    public enum EventType
    {
        CardDrawn = 0,
        CardPlayed = 1,
        CardRevealed = 2,
        CardMoved = 3,
        CardDiscarded = 4,
        CardDestroyedFromPlay = 5,
        CardDestroyedFromHand = 6,
        CardDestroyedFromLibrary = 7,
        TurnStarted = 8,
        TurnEnded = 9
    }
}
