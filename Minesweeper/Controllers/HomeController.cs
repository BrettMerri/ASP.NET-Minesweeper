using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Minesweeper.Models;

namespace Minesweeper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var currentBoard = Board.Current;
            return View(currentBoard);
        }

        [HttpGet]
        public JsonResult GetBoard()
        {
            Board currentBoard = Board.Current;
            InitialJsonValue currentJsonBoard = currentBoard.GetJsonBoard();
            return Json(currentJsonBoard, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeDifficulty(string difficulty)
        {
            if (!Enum.TryParse(difficulty, true, out GameDifficulty difficultyResult))
            {
                return GetBoard();
            }
            HttpContext.Session.Remove("Board");
            HttpContext.Session.Add("Difficulty", difficultyResult.ToString());
            return GetBoard();
        }

        [HttpGet]
        public JsonResult SelectCell(int id)
        {
            Board currentBoard = Board.Current;
            JsonValue currentJsonValue = currentBoard.SelectCell(id);
            if (currentBoard.State == GameState.MineSelected ||
                currentBoard.State == GameState.GameWon)
                HttpContext.Session.Remove("Board");
            return Json(currentJsonValue, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FlagCell(int id)
        {
            Board currentBoard = Board.Current;
            bool success = currentBoard.FlagCell(id);
            return Json(success, JsonRequestBehavior.AllowGet);
        }
    }
}