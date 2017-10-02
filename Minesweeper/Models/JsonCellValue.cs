using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public class JsonCellValue
    {
        string value;
        int id;

        public JsonCellValue()
        {
            Value = "";
            Id = -1;
        }
        public JsonCellValue(string value, int id)
        {
            Value = value;
            Id = id;
        }
        public JsonCellValue(int number, int id)
        {
            Value = number.ToString();
            Id = id;
        }

        public string Value { get => value; set => this.value = value; }
        public int Id { get => id; set => id = value; }
    }
}