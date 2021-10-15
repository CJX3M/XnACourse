using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// An enumeration of the possible game states
    /// </summary>
    public enum GameState
    {
        CheckingHandOver,
        DealerHitting,
        DisplayingHandResults,
        Exiting,
        PlayerHitting,
        WaitingForDealer,
        WaitingForPlayer
    }
}
