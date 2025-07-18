using Domain.Enums;
using System.Text.Json;

namespace Application.DTOs
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public int BoardSize { get; set; }
        public int WinCondition { get; set; }
        public string Board { get; set; } = string.Empty;
        public GameStatus Status { get; set; }
        public Guid PlayerX { get; set; }
        public Guid PlayerO { get; set; }
        public Guid PlayerTurn { get; set; }
        public List<MoveDto> Moves { get; set; } = new List<MoveDto>();

        public GameDto(int boardSize)
        {
            Board = JsonSerializer.Serialize(new string[][] { new string[boardSize], new string[boardSize], new string[boardSize] });
        }
    }
}
