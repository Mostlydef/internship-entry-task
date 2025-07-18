using Domain.Enums;

namespace Domain.Models
{
    public class Move
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid Player { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Symbols Symbol { get; set; }
        public DateTime Timestamp { get; set; }
        public Game Game { get; set; } = null!;
    }
}
