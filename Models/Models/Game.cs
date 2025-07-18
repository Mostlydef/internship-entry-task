using Domain.Enums;

namespace Domain.Models;

public class Game
{
    public Guid Id { get; set; }
    public int BoardSize { get; set; }
    public int WinCondition { get; set; }
    public string Board { get; set; } = string.Empty;
    public GameStatus Status { get; set; }
    public List<Move> Moves { get; set; } = new List<Move>();
    public Guid PlayerX { get; set; }
    public Guid PlayerO { get; set; }
    public Guid PlayerTurn { get; set; }
}
