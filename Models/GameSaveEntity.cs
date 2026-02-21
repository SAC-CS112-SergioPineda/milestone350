using System;

namespace CST_350_MilestoneProject.Models
{
    // Represents a saved Minesweeper game state stored as JSON
    public class GameSaveEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateSaved { get; set; }
        public string GameData { get; set; } = string.Empty;
    }
}
