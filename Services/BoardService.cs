using CST_350_MilestoneProject.Models;
using System.Numerics;

namespace CST_350_MilestoneProject.Services
{
    public class BoardService
    {
        private BoardModel _board;
        public bool LastClickWasReward { get; private set; }
        public void InitializeGame(int size, int difficulty)
        {
            _board = new BoardModel(size);

            // Call the logic sequences already defined in your BoardModel
            _board.SetupBombs(_board, difficulty);
            _board.SetupRewards(_board, difficulty);
            _board.CountBombsNearby(_board);
            _board.StartTime = DateTime.Now;
        }
        public bool HandleCellClick(int row, int col)
{
    LastClickWasReward = false;

    if (_board == null)
    {
        return true;
    }

    // Left-click should always "dig" (reveal). Flagging is handled separately via right-click.
    return MarkCellVisited(row, col);
}

// Handle a cell being clicked (dig/reveal)
public bool MarkCellVisited(int row, int col)
{
    var cell = _board.Grid[row, col];

    // Ignore already visited cells, and do NOT allow digging flagged cells (milestone requirement)
    if (cell.IsVisited || cell.IsFlagged)
    {
        return true;
    }

    // Bomb = game over
    if (cell.IsBombed)
    {
        cell.IsVisited = true;
        return false;
    }

    // Reward = reveal reward + neighbors, and "ping" UI
    if (cell.IsReward && !cell.IsVisited)
    {
        _board.ShowReward(_board, row, col);
        LastClickWasReward = true;
        return true;
    }

    // Normal safe reveal
    _board.RevealSafeCell(_board, row, col);
    return true;
}

// 4. Check for win status
        public bool CheckWin()
        {
            return _board.PlayerWon(_board);
        }

        public BoardModel GetBoard()
        {
            return _board;
        }
    }
}
