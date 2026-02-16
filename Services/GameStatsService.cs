using CST_350_MilestoneProject.Data;
using CST_350_MilestoneProject.Models;

namespace CST_350_MilestoneProject.Services
{
    public class GameStatsService
    {
        private readonly IGameStatsRepository _gameStatsRepository;

        public GameStatsService(IGameStatsRepository gameStatsRepository)
        {
            _gameStatsRepository = gameStatsRepository;
        }

        public bool SaveGame(string playerName, int score, double timeTaken, int difficulty)
        {
            return _gameStatsRepository.Insert(new Models.GameStatsEntity
            {
                PlayerName = playerName,
                Score = score,
                TimeTaken = timeTaken,
                Difficulty = difficulty,
                DatePlayed = DateTime.Now
            });
        }

        public List<GameStatsEntity> GetAllStats()
        {
            return _gameStatsRepository.GetAll();
        }
    }
}
