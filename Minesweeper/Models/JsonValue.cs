using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public class JsonValue
    {
        string state;
        JsonCellValue[] values;

        public JsonValue()
        {
            state = "";
            Values = Array.Empty<JsonCellValue>();
        }
        public JsonValue(JsonCellValue[] boardValues, GameState state)
        {
            Values = boardValues;
            State = state.ToString();
        }

        public JsonCellValue[] Values { get => values; set => values = value; }
        public string State { get => state; set => state = value; }
    }
}