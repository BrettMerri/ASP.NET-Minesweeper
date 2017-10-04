using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace Minesweeper.Models
{
    public class JsonValue
    {
        string state;
        JsonCellValue[] values;
        string timer;

        public JsonValue()
        {
            state = "";
            Values = Array.Empty<JsonCellValue>();
        }
        public JsonValue(JsonCellValue[] boardValues, GameState state, Stopwatch timer)
        {
            Values = boardValues;
            State = state.ToString();
            Timer = timer.Elapsed.ToString("mm':'ss':'fff");
        }

        public JsonCellValue[] Values { get => values; set => values = value; }
        public string State { get => state; set => state = value; }
        public string Timer { get => timer; set => timer = value; }
    }
}