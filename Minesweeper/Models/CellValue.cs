using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public enum CellValue
    {
        Unselected,
        Flagged,
        Mine,
        Empty,
        Number
    }
}