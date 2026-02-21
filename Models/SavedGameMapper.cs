using System;
using System.Globalization;

namespace CST_350_MilestoneProject.Models
{
    public static class SavedGameMapper
    {
        public static SavedGameDto ToDto(BoardModel board)
        {
            var dto = new SavedGameDto
            {
                Size = board.Size,
                Difficulty = board.Difficulty,
                Score = board.Score,
                CorrectlyFlaggedBombs = board.CorrectlyFlaggedBombs,
                IncorrectFlags = board.IncorrectFlags,
                CountOfBombs = board.CountOfBombs,
                StartTimeIso = board.StartTime.ToString("o", CultureInfo.InvariantCulture)
            };

            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    var cell = board.Grid[r, c];
                    dto.Cells.Add(new CellStateDto
                    {
                        Row = r,
                        Col = c,
                        IsVisited = cell.IsVisited,
                        IsBombed = cell.IsBombed,
                        IsFlagged = cell.IsFlagged,
                        IsReward = cell.IsReward,
                        NumberOfBombNeighbors = cell.NumberOfBombNeighbors
                    });
                }
            }

            return dto;
        }

        public static BoardModel FromDto(SavedGameDto dto)
        {
            var board = new BoardModel(dto.Size)
            {
                Difficulty = dto.Difficulty,
                Score = dto.Score,
                CorrectlyFlaggedBombs = dto.CorrectlyFlaggedBombs,
                IncorrectFlags = dto.IncorrectFlags,
                CountOfBombs = dto.CountOfBombs
            };

            if (DateTime.TryParse(dto.StartTimeIso, null, DateTimeStyles.RoundtripKind, out var start))
            {
                board.StartTime = start;
            }
            else
            {
                board.StartTime = DateTime.Now;
            }

            // Restore cells
            foreach (var cs in dto.Cells)
            {
                if (cs.Row < 0 || cs.Row >= board.Size || cs.Col < 0 || cs.Col >= board.Size) continue;

                var cell = board.Grid[cs.Row, cs.Col];
                cell.IsVisited = cs.IsVisited;
                cell.IsBombed = cs.IsBombed;
                cell.IsFlagged = cs.IsFlagged;
                cell.IsReward = cs.IsReward;
                cell.NumberOfBombNeighbors = cs.NumberOfBombNeighbors;
            }

            return board;
        }
    }
}
