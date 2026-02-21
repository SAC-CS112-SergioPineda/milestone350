using CST_350_MilestoneProject.Models;
using MySql.Data.MySqlClient;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;

namespace CST_350_MilestoneProject.Data
{
    public class GameStatsRepository : IGameStatsRepository
    {
        private readonly string _connectionString;

        public GameStatsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<GameStatsEntity> GetAll()
        {
            var allGameStats = new List<GameStatsEntity>();
            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand("SELECT Id, PlayerName, Score, TimeTaken, Difficulty, DatePlayed FROM GameStats", connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                allGameStats.Add(new GameStatsEntity
                {
                    Id = reader.GetInt32(0),
                    PlayerName = reader.GetString(1),
                    Score = reader.GetInt32(2),
                    TimeTaken = reader.GetInt32(3),
                    Difficulty = reader.GetInt32(4),
                    DatePlayed = reader.GetDateTime(5)
                });
            }
            return allGameStats;
        }

        public bool Insert(GameStatsEntity stat)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = @"INSERT INTO GameStats (PlayerName, Score, TimeTaken, Difficulty, DatePlayed) VALUES (@name,@score,@time,@difficulty,@date)";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", stat.PlayerName);
            command.Parameters.AddWithValue("@score", stat.Score);
            command.Parameters.AddWithValue("@time", stat.TimeTaken);
            command.Parameters.AddWithValue("@difficulty", stat.Difficulty);
            command.Parameters.AddWithValue("@date", stat.DatePlayed);
            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
