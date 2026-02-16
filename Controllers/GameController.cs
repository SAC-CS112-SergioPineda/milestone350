using CST_350_MilestoneProject.Filters;
using CST_350_MilestoneProject.Services;
using CST_350_MilestoneProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_MilestoneProject.Controllers
{
    [SessionCheckFilter]
    public class GameController : Controller
    {
        private readonly BoardService _boardService;
        private readonly GameStatsService _gameStatsService;

        public GameController(BoardService boardService, GameStatsService gameStatsService)
        {
            _boardService = boardService;
            _gameStatsService = gameStatsService;
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            return View();
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
