using System.Collections.Generic;

namespace CST_350_MilestoneProject.Models
{
    // DTO used for JSON serialization of a game save
    public class SavedGameDto
    {
        public int Size { get; set; }
        public int Difficulty { get; set; }
        public int Score { get; set; }
        public int CorrectlyFlaggedBombs { get; set; }
        public int IncorrectFlags { get; set; }
        public int CountOfBombs { get; set; }
        public string StartTimeIso { get; set; } = string.Empty;

        public List<CellStateDto> Cells { get; set; } = new();
    }

    public class CellStateDto
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsVisited { get; set; }
        public bool IsBombed { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsReward { get; set; }
        public int NumberOfBombNeighbors { get; set; }
    }
}
