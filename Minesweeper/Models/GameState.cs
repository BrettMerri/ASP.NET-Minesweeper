using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public enum GameState
    {
        BlankGameBoard,
        GameInProgress,
        MineSelected,
        GameWon
    }
}