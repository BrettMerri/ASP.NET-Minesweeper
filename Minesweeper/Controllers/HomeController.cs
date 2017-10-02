﻿using System;
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
            JsonBoard currentJsonBoard = currentBoard.GetJsonBoard();
            return Json(currentJsonBoard, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}