using CST_350_MilestoneProject.Models;

namespace CST_350_MilestoneProject.Data
{
    public interface IGameStatsRepository
    {
        List<GameStatsEntity> GetAll();
        bool Insert(GameStatsEntity stat);
    }
}
