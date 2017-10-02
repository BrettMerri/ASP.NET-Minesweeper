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
            List<string> boardStringList = new List<string>();

            //Goes through y values top to bottom
            for (int y = 0; y < Vertical; y++)
            {
                //Goes through x values left to right
                for (int x = 0; x < Horizontal; x++)
                {
                    Cell currentCell = CellArray[y, x];

                    if (currentCell.IsFlagged)
                        boardStringList.Add("F");
                    else if (!currentCell.IsSelected)
                        boardStringList.Add("U");
                    else if (currentCell.IsMine)
                        boardStringList.Add("M");
                    else
                        boardStringList.Add(currentCell.SurroundingMinesValue.ToString());
                }
            }
            return new JsonBoard(Horizontal, Vertical, boardStringList.ToArray());
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
                    CellArray[y, x] = new Cell();
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