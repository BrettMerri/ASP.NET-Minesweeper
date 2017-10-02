using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public class Cell
    {
        int surroundingMinesValue;
        bool isMine;
        bool isSelected;
        bool isFlagged;
        bool isMineBlacklisted; //used to check if a mine can be generated at this cell
        bool surroundingMinesChecked; //used to check if surrounding cells have already been checked when revealing all connecting zeros

        public Cell()
        {
            SurroundingMinesValue = 0;
            isMine = false;
            isSelected = false;
            isFlagged = false;
            isMineBlacklisted = false;
            surroundingMinesChecked = false;
        }

        public bool IsMine { get => isMine; set => isMine = value; }
        public bool IsSelected { get => isSelected; set => isSelected = value; }
        public bool IsFlagged { get => isFlagged; set => isFlagged = value; }
        public bool IsMineBlacklisted { get => isMineBlacklisted; set => isMineBlacklisted = value; }
        public bool SurroundingMinesChecked { get => surroundingMinesChecked; set => surroundingMinesChecked = value; }
        public int SurroundingMinesValue { get => surroundingMinesValue; set => surroundingMinesValue = value; }
    }
}