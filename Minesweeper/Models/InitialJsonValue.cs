using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public class InitialJsonValue
    {
        int width;
        int height;
        string difficulty;
        JsonCellValue[] values;

        public InitialJsonValue(int width, int height, string difficulty, JsonCellValue[] boardValues)
        {
            Width = width;
            Height = height;
            Difficulty = difficulty;
            Values = boardValues;
        }

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public JsonCellValue[] Values { get => values; set => values = value; }
        public string Difficulty { get => difficulty; set => difficulty = value; }
    }
}