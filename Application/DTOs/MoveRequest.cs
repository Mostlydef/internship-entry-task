using Domain.Enums;

namespace Application.DTOs
{
    public class MoveRequest
    {
        public Guid Player { get; set; }
        public Symbols Symbol { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
