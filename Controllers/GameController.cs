using CST_350_MilestoneProject.Filters;
using CST_350_MilestoneProject.Services;
using CST_350_MilestoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace CST_350_MilestoneProject.Controllers
{
    [SessionCheckFilter]
    public class GameController : Controller
    {
        private readonly BoardService _boardService;
        private readonly GameStatsService _gameStatsService;
        private readonly AppDbContext _db;

        public GameController(BoardService boardService, GameStatsService gameStatsService, AppDbContext db)
        {
            _boardService = boardService;
            _gameStatsService = gameStatsService;
            _db = db;
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            return View();
        }

        
// AJAX timestamp endpoint (rubric requirement)
[HttpGet]
public IActionResult GetTimestamp()
{
    var board = _boardService.GetBoard();
    var serverTime = DateTime.Now;

    int elapsedSeconds = 0;
    if (board != null && board.StartTime != default)
    {
        elapsedSeconds = (int)(serverTime - board.StartTime).TotalSeconds;
    }

    return Json(new
    {
        serverTime = serverTime.ToString("yyyy-MM-dd HH:mm:ss"),
        elapsedSeconds = elapsedSeconds
    });
}

[HttpPost]
        public IActionResult MineSweeperBoard(int size, int difficulty)
        {
            _boardService.InitializeGame(size, difficulty);
            var board = _boardService.GetBoard();
            var vm = BoardMapper.ToViewModel(board);
            return View("MineSweeperBoard", vm);
        }

        // Non-AJAX (kept for grading / fallback)
        [HttpPost]
        public IActionResult HandleLeftClick(int row, int col)
        {
            bool stillAlive = _boardService.HandleCellClick(row, col);

            var board = _boardService.GetBoard();
            var vm = BoardMapper.ToViewModel(board);

            if (!stillAlive)
            {
                SaveGameStats();
                return RedirectToAction("Loss");
            }

            if (_boardService.CheckWin())
            {
                SaveGameStats();
                return RedirectToAction("Win");
            }

            return View("MineSweeperBoard", vm);
        }

        // AJAX left-click: returns board partial OR modal partial (no redirect)
        [HttpPost]
        public IActionResult HandleLeftClickAjax(int row, int col)
        {
            bool stillAlive = _boardService.HandleCellClick(row, col);

            var board = _boardService.GetBoard();
            var vm = BoardMapper.ToViewModel(board);

            if (!stillAlive)
            {
                SaveGameStats();
                return PartialView("_LossModal", vm);
            }

            if (_boardService.CheckWin())
            {
                SaveGameStats();
                return PartialView("_WinModal", vm);
            }

            if (_boardService.LastClickWasReward)
            {
                return PartialView("_RewardModal", vm);
            }

            return PartialView("_GamePanel", vm);
        }

        // AJAX right-click: flag toggle + check win -> modal
        [HttpPost]
        public IActionResult HandleRightClickAjax(int row, int col)
        {
            var board = _boardService.GetBoard();

            if (board != null)
            {
                board.ShowFlag(board, row, col);
            }

            if (_boardService.CheckWin())
            {
                SaveGameStats();
                var winVm = BoardMapper.ToViewModel(board);
                return PartialView("_WinModal", winVm);
            }

            var vm = BoardMapper.ToViewModel(board);
            return PartialView("_GamePanel", vm);
        }

        // Part 1 – Save Game (serialize board + user into JSON saved in DB)
[HttpPost]
public IActionResult SaveGame()
{
    var board = _boardService.GetBoard();
    if (board == null)
    {
        return RedirectToAction("StartGame");
    }

    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

    var dto = SavedGameMapper.ToDto(board);
    string json = JsonSerializer.Serialize(dto);

    var save = new GameSaveEntity
    {
        UserId = userId,
        DateSaved = DateTime.Now,
        GameData = json
    };

    _db.Games.Add(save);
    _db.SaveChanges();

    return RedirectToAction("ShowSavedGames");
}

// Part 2 – List Saved Games
[HttpGet]
public IActionResult ShowSavedGames()
{
    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

    // If userId isn't set (older login flow), still show any saves for userId=0
    var games = _db.Games
        .Where(g => g.UserId == userId)
        .OrderByDescending(g => g.DateSaved)
        .ToList();

    return View(games);
}

// Part 2 – Load Saved Game
[HttpPost]
public IActionResult LoadGame(int id)
{
    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

    var save = _db.Games.FirstOrDefault(g => g.Id == id && g.UserId == userId);
    if (save == null)
    {
        return RedirectToAction("ShowSavedGames");
    }

    var dto = JsonSerializer.Deserialize<SavedGameDto>(save.GameData);
    if (dto == null)
    {
        return RedirectToAction("ShowSavedGames");
    }

    var restoredBoard = SavedGameMapper.FromDto(dto);
    _boardService.LoadBoard(restoredBoard);

    var vm = BoardMapper.ToViewModel(restoredBoard);
    return View("MineSweeperBoard", vm);
}

// Part 2 – Delete Saved Game
[HttpPost]
public IActionResult DeleteGame(int id)
{
    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

    var save = _db.Games.FirstOrDefault(g => g.Id == id && g.UserId == userId);
    if (save != null)
    {
        _db.Games.Remove(save);
        _db.SaveChanges();
    }

    return RedirectToAction("ShowSavedGames");
}


        public IActionResult Win()
        {
            return View();
        }

        public IActionResult Loss()
        {
            return View();
        }

        private void SaveGameStats()
        {
            var username = HttpContext.Session.GetString("Username") ?? "Guest";
            var board = _boardService.GetBoard();
            TimeSpan elapsed = DateTime.Now - board.StartTime;
            int secondsPlayed = (int)elapsed.TotalSeconds;

            _gameStatsService.SaveGame(username, board.Score, secondsPlayed, board.Difficulty);

            TempData["LastScore"] = board.Score;
            TempData["LastSecondsPlayed"] = secondsPlayed;
        }
    }
}
