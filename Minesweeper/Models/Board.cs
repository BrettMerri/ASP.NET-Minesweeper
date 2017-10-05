using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        int safeCellsRemaining;
        GameState state;
        DateTime startTime;
        Stopwatch timer;

        public Board()
        {
            Horizontal = 15; 
            Vertical = 15;
            Mines = 10;
            SafeCellsRemaining = Horizontal * Vertical - Mines;
            State = GameState.BlankGameBoard;
            CreateEmptyCellArray();
        }

        public InitialJsonValue GetJsonBoard()
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
                        JsonCellValueList.Add(new JsonCellValue("Flag", currentCell.Id));
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
            return new InitialJsonValue(Horizontal, Vertical, JsonCellValueList.ToArray());
        }

        public JsonValue SelectCell(int id)
        {
            int y = id / Horizontal;
            int x = id % Horizontal;

            Cell selectedCell = CellArray[y, x];

            if (selectedCell.IsSelected || selectedCell.IsFlagged)
                return null; //Do not allow selection of a cell that is already selected or flagged

            //Beyond this point at least one cell will be selected
            selectedCell.IsSelected = true;

            //If this is the user's first selection, generate mines and then set gamestate to GameInProgress
            if (State == GameState.BlankGameBoard)
            {
                GenerateMines(y, x); // State will be changed to GameInProgress on CheckWinCondition() call
                Timer = new Stopwatch();
                Timer.Start();
            }

            if (selectedCell.IsMine)
            {
                State = GameState.MineSelected;
                return RevealAllBoardValues(id);
            }

            //If the selected cell has zero mines surrounding
            if (selectedCell.SurroundingMinesValue == 0)
            {
                List<JsonCellValue> JsonCellValueList = new List<JsonCellValue>
                {
                    new JsonCellValue("Blank", id)
                };
                RevealAroundConnectingZeros(y, x, JsonCellValueList); //Adds values to the JsonCellValueList
                SafeCellsRemaining -= JsonCellValueList.Count;
                State = CheckWinCondition();
                if (State == GameState.GameWon)
                    return RevealAllBoardValues(id);
                return new JsonValue(JsonCellValueList.ToArray(), State, Timer);
            }

            SafeCellsRemaining--;
            State = CheckWinCondition();
            if (State == GameState.GameWon)
                return RevealAllBoardValues(id);

            return new JsonValue(new JsonCellValue[] {
                new JsonCellValue(selectedCell.SurroundingMinesValue, id)
            }, State, Timer);
        }

        private GameState CheckWinCondition()
        {
            return SafeCellsRemaining == 0 ? GameState.GameWon : GameState.GameInProgress;
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

        public bool FlagCell(int id)
        {
            int y = id / Horizontal;
            int x = id % Horizontal;

            Cell flaggedCell = CellArray[y, x];

            if (flaggedCell.IsSelected)
            {
                return false;
            }

            //Toggles the IsFlagged boolean value between true and false
            flaggedCell.IsFlagged = !flaggedCell.IsFlagged;
            return true;
        }

        private JsonValue RevealAllBoardValues(int selectedCellId)
        {
            Timer.Stop();
            JsonCellValue[] JsonCellValueArray = new JsonCellValue[Vertical * Horizontal];
            for (int y = 0; y < Vertical; y++)
            {
                //Goes through x values left to right
                for (int x = 0; x < Horizontal; x++)
                {
                    int id = y * Horizontal + x;
                    if (CellArray[y, x].IsMine)
                    {
                        if (State == GameState.GameWon)
                            JsonCellValueArray[id] = new JsonCellValue("Flag", id);
                        else
                        {
                            if (id == selectedCellId)
                                JsonCellValueArray[id] = new JsonCellValue("MineDeath", id);
                            else if (CellArray[y, x].IsFlagged)
                                JsonCellValueArray[id] = new JsonCellValue("MineFlagged", id);
                            else
                                JsonCellValueArray[id] = new JsonCellValue("Mine", id);
                        }

                    }
                    else if (CellArray[y, x].IsFlagged)
                        JsonCellValueArray[id] = new JsonCellValue("MineMisFlagged", id);
                    else if (CellArray[y, x].SurroundingMinesValue == 0)
                        JsonCellValueArray[id] = new JsonCellValue("Blank", id);
                    else
                        JsonCellValueArray[id] = new JsonCellValue(CellArray[y, x].SurroundingMinesValue, id);
                }
            }
            return new JsonValue(JsonCellValueArray, State, Timer);
        }

        private void RevealAroundConnectingZeros(int yInput, int xInput, List<JsonCellValue> list)
        {
            //This is set to true to indicate that this cell's surroundings have been checked already.
            CellArray[yInput, xInput].SurroundingMinesChecked = true;

            //Starts one y value above and works its way down
            for (int y = yInput - 1; y <= yInput + 1; y++)
            {
                //Checks y value's bounds
                if (y < 0 || y >= Vertical)
                {
                    continue;
                }

                //Starts one x value left and works its way right
                for (int x = xInput - 1; x <= xInput + 1; x++)
                {
                    //Checks x value's bounds
                    if (x < 0 || x >= Horizontal)
                    {
                        continue;
                    }

                    int id = y * Horizontal + x;

                    if (!CellArray[y, x].IsSelected)
                    {
                        CellArray[y, x].IsSelected = true;
                        CellArray[y, x].IsFlagged = false;
                        if (CellArray[y, x].SurroundingMinesValue == 0)
                            list.Add(new JsonCellValue("Blank", id));
                        else
                            list.Add(new JsonCellValue(CellArray[y, x].SurroundingMinesValue, id));
                    }

                    //If a revealed cell is a zero, and it has not been checked already...
                    if (CellArray[y, x].SurroundingMinesValue == 0 &&
                        !CellArray[y, x].SurroundingMinesChecked)
                    {
                        //Use recursion to reveal around the newly revealed zeros
                        RevealAroundConnectingZeros(y, x, list);
                    }
                }
            }
        }

        private void GenerateMines(int y, int x)
        {
            BlacklistSurroundingCells(y, x);

            Random rnd = new Random();

            for (int i = 0; i < Mines; i++)
            {
                int yRndValue = rnd.Next(Vertical);
                int xRndValue = rnd.Next(Horizontal);

                Cell randomCell = CellArray[yRndValue, xRndValue];

                if (randomCell.IsMineBlacklisted ||
                    randomCell.IsMine ||
                    randomCell.IsSelected)
                {
                    i--; //This restarts the for loop with the same i value
                }
                else
                {
                    randomCell.IsMine = true;
                }
            }

            GenerateSurroundingMinesValues();
        }

        private void BlacklistSurroundingCells(int yInput, int xInput)
        {
            //Starts one y value above and works its way down
            for (int y = yInput - 1; y <= yInput + 1; y++)
            {
                //Checks y value's bounds
                if (y < 0 || y >= Vertical)
                {
                    continue;
                }

                //Starts one x value left and works its way right
                for (int x = xInput - 1; x <= xInput + 1; x++)
                {
                    //Checks x value's bounds
                    if (x < 0 || x >= Horizontal)
                    {
                        continue;
                    }

                    CellArray[y, x].IsMineBlacklisted = true;
                }
            }
        }

        private void GenerateSurroundingMinesValues()
        {
            //Goes through y values top to bottom
            for (int y = 0; y < Vertical; y++)
            {
                //Goes through x values left to right
                for (int x = 0; x < Horizontal; x++)
                {
                    CellArray[y, x].SurroundingMinesValue = CountSurroundingMines(y, x);
                }
            }
        }

        private int CountSurroundingMines(int yInput, int xInput)
        {
            int surroundingMinesValue = 0;
            //Starts one y value above and works its way down
            for (int y = yInput - 1; y <= yInput + 1; y++)
            {
                //Checks y value's bounds
                if (y < 0 || y >= Vertical)
                {
                    continue;
                }

                //Starts one x value left and works its way right
                for (int x = xInput - 1; x <= xInput + 1; x++)
                {
                    //Checks x value's bounds and checks if a mine exists
                    if (x < 0 || x >= Horizontal ||
                        !CellArray[y, x].IsMine)
                    {
                        continue;
                    }

                    surroundingMinesValue++;
                }
            }
            return surroundingMinesValue;
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

        public Cell[,] CellArray { get => cellArray; set => cellArray = value; }
        public GameState State { get => state; set => state = value; }
        public int Horizontal { get => horizontal; set => horizontal = value; }
        public int Vertical { get => vertical; set => vertical = value; }
        public int Mines { get => mines; set => mines = value; }
        public int SafeCellsRemaining { get => safeCellsRemaining; set => safeCellsRemaining = value; }
        public DateTime StartTime { get => startTime; set => startTime = value; }
        public Stopwatch Timer { get => timer; set => timer = value; }
    }
}