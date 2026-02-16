namespace CST_350_MilestoneProject.Models
{
    // ViewModel representing a cell in a Minesweeper game
    public class CellViewModel
    {
        // Properties representing the state of a cell in a Minesweeper game
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsVisited { get; set; }
        public bool IsBombed { get; set; }
        public bool IsFlagged { get; set; }
        public int NumberOfBombNeighbors { get; set; }
    }
}
