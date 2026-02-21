namespace CST_350_MilestoneProject.Models
{
    // Model that represents a single cell on the Minesweeper board
    public class CellModel
    {
        // Properties for the CellModel class
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsVisited { get; set; }
        public bool IsBombed { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsReward { get; set; }
        public int NumberOfBombNeighbors { get; set; }

        /// <summary>
        /// Parameterized constructor for the CellModel class
        /// </summary>
        public CellModel(int row, int col)
        {
            // Set cell position
            Row = row;
            Column = col;
            // Cell has not been visited yet
            IsVisited = false;
            // No bomb placed in this cell
            IsBombed = false;
            // No neighboring bombs counted yet
            IsFlagged = false;
            NumberOfBombNeighbors = 0;
        }
    }
}
