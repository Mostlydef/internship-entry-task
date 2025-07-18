using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public enum ErrorCodes
    {
        GameIsOver,
        BoardNotCreated,
        InvalidMove,
        ErrorDeserializing,
        CageOccupied,
        InvalidPlayers,
        MoveOutsideBoard,
        OtherMove,
        IncorrectGameSettings,
        InvalidSymbol
    }
}
