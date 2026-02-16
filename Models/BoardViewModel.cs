namespace CST_350_MilestoneProject.Models
{
    public class BoardViewModel
    {
        public int Size { get; set; }
        public int Score { get; set; }
        public CellViewModel[,] Grid { get; set; }
    }
}
