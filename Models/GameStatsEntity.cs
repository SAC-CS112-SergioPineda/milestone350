namespace CST_350_MilestoneProject.Models
{
    public class GameStatsEntity
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public double TimeTaken { get; set; }
        public int Difficulty { get; set; }
        public DateTime DatePlayed { get; set; }
    }
}
