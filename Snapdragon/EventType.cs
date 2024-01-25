using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    public enum EventType
    {
        DrawCard = 0,
        PlayCard = 1,
        RevealCard = 2,
        MoveCard = 3,
        DiscardCard = 4,
        DestroyCardInPlay = 5,
        DestroyCardInHand = 6,
        DestroyCardInDeck = 7,
        StartTurn = 8,
        EndTurn = 9
    }
}
