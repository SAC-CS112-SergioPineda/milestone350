using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Drawing;

namespace CST_350_MilestoneProject.Models
{
    public class BoardModel
    {
        // Properties for the BoardModel class
        public int Size { get; set; }
        public int Difficulty { get; set; }
        public int CorrectlyFlaggedBombs { get; set; }
        // Tracks flags placed on non-bomb cells (used to validate a win)
        public int IncorrectFlags { get; set; }
        public int CountOfBombs { get; set; }
        public CellModel[,] Grid { get; set; }
        public int Score { get; set; }
        public DateTime StartTime { get; set; }

        private readonly Random _random = new Random();

        public BoardModel(int size)
        {
            // Set the size of the board
            Size = size;
            // Set the difficulty of the game
            Difficulty = 0;
            CorrectlyFlaggedBombs = 0;
            IncorrectFlags = 0;
            CountOfBombs = 0;
            // Create a new grid of CellModel objects
            Grid = new CellModel[Size, Size];
            Score = 0;

            // Fill the board with CellModel objects at each position
            InitializeBoard();
        }

        /// <summary>
        /// Fills the board with CellModel objects at each position
        /// </summary>
        private void InitializeBoard()
        {
            // Loop through each row
            for (int row = 0; row < Size; row++)
            {
                // Loop through each column
                for (int col = 0; col < Size; col++)
                {
                    // Create a cell at (row, col) and assign it to the grid
                    Grid[row, col] = new CellModel(row, col);
                }
            }
        }

        public bool IsCellOnBoard(BoardModel board, int row, int col)
        {
            // Valid row and column must fall within 0 and board.Size - 1
            return row >= 0 && col >= 0 && row < board.Size && col < board.Size;
        }

        public void SetupBombs(BoardModel boardModel, int difficulty)
        {
            // Determine how many bombs we need based on difficulty
            int bombCount = DifficultyLevel(boardModel, difficulty);
            int placed = 0;

            // keep placing until the required number of bombs is reached
            while (placed < bombCount)
            {
                // Pick random coordinates
                int row = _random.Next(boardModel.Size);
                int col = _random.Next(boardModel.Size);
                var cell = boardModel.Grid[row, col];

                // Only place a bomb in an empty spot
                if (!cell.IsBombed)
                {
                    cell.IsBombed = true;
                    placed++;
                }
            }
        }

        public void SetupRewards(BoardModel board, int difficulty)
        {
            // Reward count is currently tied to the same logic as bombs
            int rewardCount = DifficultyLevel(board, difficulty);
            int placed = 0;

            // Randomly assign rewards to cells that are not bombs or rewards
            while (placed < rewardCount)
            {
                int row = _random.Next(board.Size);
                int col = _random.Next(board.Size);
                CellModel cell = board.Grid[row, col];

                // Prevent overlap with bombs
                if (!cell.IsReward && !cell.IsBombed)
                {
                    cell.IsReward = true;
                    placed++;
                }
            }
        }

        public int DifficultyLevel(BoardModel board, int difficulty)
        {
            board.Difficulty = difficulty;

            // Difficulty scales with board size
            return board.Difficulty switch
            {
                1 => board.Size / 2,        // Easy
                2 => (board.Size / 2) * 2,    // Medium
                3 => (board.Size / 2) * 3,    // Hard
                _ => board.Size / 2
            };
        }

        public void CountBombsNearby(BoardModel board)
        {
            // Row and Column offsets for 8 directions
            int[] x = { 1, -1, 0, 0, 1, -1, 1, -1 };
            int[] y = { 0, 0, 1, -1, 1, -1, -1, 1 };

            int size = board.Size;

            // Loop through entire board
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    int neighbors = 0;

                    // Check all 8 neighboring positions
                    for (int i = 0; i < x.Length; i++)
                    {
                        int newRow = row + x[i];
                        int newCol = col + y[i];

                        // If valid and has a bomb, increment count
                        if (IsCellOnBoard(board, newRow, newCol) && board.Grid[newRow, newCol].IsBombed) neighbors++;
                    }

                    // Store the bomb count for this cell
                    board.Grid[row, col].NumberOfBombNeighbors = neighbors;
                }
            }
        }

        public void ShowFlag(BoardModel board, int row, int col)
        {
            // Ensure the target is on the board
            if (!IsCellOnBoard(board, row, col))
            {
                return;
            }

            var cell = board.Grid[row, col];

            // Do not allow flagging revealed cells
            if (cell.IsVisited)
            {
                return;
            }

            // Toggle flag on/off
            if (cell.IsFlagged)
            {
                cell.IsFlagged = false;

                // Update counters based on what was flagged
                if (cell.IsBombed)
                {
                    board.CorrectlyFlaggedBombs = Math.Max(0, board.CorrectlyFlaggedBombs - 1);
                }
                else
                {
                    board.IncorrectFlags = Math.Max(0, board.IncorrectFlags - 1);
                }

                return;
            }

            // Place a flag
            cell.IsFlagged = true;

            if (cell.IsBombed)
            {
                board.CorrectlyFlaggedBombs++;
            }
            else
            {
                board.IncorrectFlags++;
            }
        }

        /// <summary>
        /// Checks if all bombs were flagged correctly
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public bool PlayerWon(BoardModel board)
        {
            int totalBombs = 0;
            int totalFlags = 0;

            // Scan the board for bombs and flags
            for (int row = 0; row < board.Size; row++)
            {
                for (int col = 0; col < board.Size; col++)
                {
                    var cell = board.Grid[row, col];

                    if (cell.IsBombed) totalBombs++;
                    if (cell.IsFlagged) totalFlags++;
                }
            }

            // Win requires correct flags == bombs, and no extra flags
            return board.CorrectlyFlaggedBombs == totalBombs
                   && totalFlags == totalBombs
                   && board.IncorrectFlags == 0;
        }

        public void ShowReward(BoardModel boardModel, int selectedRow, int selectedCol)
        {
            if (!IsCellOnBoard(boardModel, selectedRow, selectedCol))
                return;

            // Get the selected cell
            CellModel cell = boardModel.Grid[selectedRow, selectedCol];

            // Only trigger effect if it's a reward
            if (!cell.IsReward || cell.IsVisited)
                return;

            // Reveal reward + bonus score
            cell.IsVisited = true;
            boardModel.Score += 10;

            // Reveal all adjacent NON-bomb cells (including numbered cells).
            // Clear (0) cells will expand using RevealSafeCell's flood-fill rules.
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;

                    int r = selectedRow + dr;
                    int c = selectedCol + dc;

                    if (!IsCellOnBoard(boardModel, r, c))
                        continue;

                    CellModel neighbor = boardModel.Grid[r, c];

                    // Skip bombs, rewards, flags, and already visited cells
                    if (neighbor.IsBombed || neighbor.IsReward || neighbor.IsFlagged || neighbor.IsVisited)
                        continue;

                    // Use normal reveal logic so numbered cells reveal once,
                    // and 0-cells expand outward.
                    RevealSafeCell(boardModel, r, c);
                }
            }
        }

        public BoardModel RevealSafeCell(BoardModel boardModel, int selectedRow, int selectedCol)
        {
            // Stop if out of bounds
            if (!IsCellOnBoard(boardModel, selectedRow, selectedCol))
                return boardModel;

            var cell = boardModel.Grid[selectedRow, selectedCol];

            // Don't reveal invalid cells
            if (cell.IsVisited || cell.IsBombed || cell.IsReward || cell.IsFlagged)
                return boardModel;

            // Reveal this cell
            cell.IsVisited = true;
            boardModel.Score += 1;

            // 🔥 CRITICAL FIX:
            // If this cell touches a bomb, STOP expanding.
            if (cell.NumberOfBombNeighbors > 0)
                return boardModel;

            // Only expand if this is a 0-cell
            boardModel = RevealSafeCell(boardModel, selectedRow - 1, selectedCol);
            boardModel = RevealSafeCell(boardModel, selectedRow + 1, selectedCol);
            boardModel = RevealSafeCell(boardModel, selectedRow, selectedCol - 1);
            boardModel = RevealSafeCell(boardModel, selectedRow, selectedCol + 1);

            boardModel = RevealSafeCell(boardModel, selectedRow - 1, selectedCol - 1);
            boardModel = RevealSafeCell(boardModel, selectedRow - 1, selectedCol + 1);
            boardModel = RevealSafeCell(boardModel, selectedRow + 1, selectedCol - 1);
            boardModel = RevealSafeCell(boardModel, selectedRow + 1, selectedCol + 1);

            return boardModel;
        }

    }
}
