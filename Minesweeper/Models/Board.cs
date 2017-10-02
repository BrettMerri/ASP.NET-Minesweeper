using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minesweeper.Models
{
    public class Board
    {
        Cell[,] cellArray;
        int horizontal;
        int vertical;
        int mines;
        int blankCellsRemaining;
        GameState state;

        public Board()
        {
            Horizontal = 9;
            Vertical = 9;
            Mines = 10;
            BlankCellsRemaining = Horizontal * Vertical - Mines;
            State = GameState.BlankGameBoard;
            CreateEmptyCellArray();
        }

        public JsonBoard GetJsonBoard()
        {
            List<JsonCellValue> JsonCellValueList = new List<JsonCellValue>();

            //Goes through y values top to bottom
            for (int y = 0; y < Vertical; y++)
            {
                //Goes through x values left to right
                for (int x = 0; x < Horizontal; x++)
                {
                    Cell currentCell = CellArray[y, x];

                    if (currentCell.IsFlagged)
                        JsonCellValueList.Add(new JsonCellValue("Flagged", currentCell.Id));
                    else if (!currentCell.IsSelected)
                        JsonCellValueList.Add(new JsonCellValue("Unselected", currentCell.Id));
                    else if (currentCell.IsMine)
                        JsonCellValueList.Add(new JsonCellValue("Mine", currentCell.Id));
                    else if (currentCell.SurroundingMinesValue == 0)
                        JsonCellValueList.Add(new JsonCellValue("Blank", currentCell.Id));
                    else
                        JsonCellValueList.Add(new JsonCellValue(currentCell.SurroundingMinesValue, currentCell.Id));
                }
            }
            return new JsonBoard(Horizontal, Vertical, JsonCellValueList.ToArray());
        }

        protected void CreateEmptyCellArray()
        {
            CellArray = new Cell[Vertical, Horizontal];

            //Goes through y values top to bottom
            for (int y = 0; y < Vertical; y++)
            {
                //Goes through x values left to right
                for (int x = 0; x < Horizontal; x++)
                {
                    int id = y * Horizontal + x;
                    CellArray[y, x] = new Cell(id);
                }
            }
        }

        public static Board Current
        {
            get
            {
                var boardSession = HttpContext.Current.Session["Board"] as Board;
                if (boardSession == null)
                {
                    boardSession = new Board();
                    HttpContext.Current.Session["Board"] = boardSession;
                }
                return boardSession;
            }
        }

        public enum GameState
        {
            BlankGameBoard,
            GameInProgress,
            MineSelected,
            GameWon
        }

        public Cell[,] CellArray { get => cellArray; set => cellArray = value; }
        public GameState State { get => state; set => state = value; }
        public int Horizontal { get => horizontal; set => horizontal = value; }
        public int Vertical { get => vertical; set => vertical = value; }
        public int Mines { get => mines; set => mines = value; }
        public int BlankCellsRemaining { get => blankCellsRemaining; set => blankCellsRemaining = value; }
    }
}