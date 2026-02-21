using Microsoft.AspNetCore.Mvc;
using CST_350_MilestoneProject.Models;
using System.Linq;

namespace CST_350_MilestoneProject.Controllers
{
    // Part 3 – Game API Service (no auth required yet per rubric)
    [ApiController]
    [Route("api")]
    public class GameApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public GameApiController(AppDbContext db)
        {
            _db = db;
        }

        // 1) localhost/api/showSavedGames – Displays all saved games.
        [HttpGet("showSavedGames")]
        public IActionResult ShowSavedGames()
        {
            var games = _db.Games
                .OrderByDescending(g => g.DateSaved)
                .Select(g => new { g.Id, g.UserId, g.DateSaved })
                .ToList();

            return Ok(games);
        }

        // 2) localhost/api/showSavedGames/5 – Displays the contents of a single game.
        [HttpGet("showSavedGames/{id:int}")]
        public IActionResult ShowOneGame(int id)
        {
            var game = _db.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return NotFound();

            return Ok(new { game.Id, game.UserId, game.DateSaved, game.GameData });
        }

        // 3) localhost/api/deleteOneGame/5 – Deletes one game from the database.
        [HttpDelete("deleteOneGame/{id:int}")]
        public IActionResult DeleteOneGame(int id)
        {
            var game = _db.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return NotFound();

            _db.Games.Remove(game);
            _db.SaveChanges();

            return Ok(new { message = "Deleted", id });
        }
    }
}
