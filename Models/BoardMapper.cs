namespace CST_350_MilestoneProject.Models
{
    public static class BoardMapper
    {
        public static BoardViewModel ToViewModel(BoardModel board)
        {
            var vm = new BoardViewModel
            {
                Size = board.Size,
                Score = board.Score,
                Grid = new CellViewModel[board.Size, board.Size]
            };

            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    var cell = board.Grid[r, c];
                    vm.Grid[r, c] = new CellViewModel
                    {
                        Row = cell.Row,
                        Column = cell.Column,
                        IsVisited = cell.IsVisited,
                        IsBombed = cell.IsBombed,
                        IsFlagged = cell.IsFlagged,
                        NumberOfBombNeighbors = cell.NumberOfBombNeighbors
                    };
                }
            }

            return vm;
        }
    }
}
