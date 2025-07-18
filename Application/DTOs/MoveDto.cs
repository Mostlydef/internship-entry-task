using Domain.Enums;

namespace Application.DTOs
{
    public class MoveDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid Player {  get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Symbols Symbol { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
