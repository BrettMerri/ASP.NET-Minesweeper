using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public class JsonBoard
    {
        int width;
        int height;
        JsonCellValue[] values;

        public JsonBoard(int width, int height, JsonCellValue[] boardValues)
        {
            Width = width;
            Height = height;
            Values = boardValues;
        }

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public JsonCellValue[] Values { get => values; set => values = value; }
    }
}