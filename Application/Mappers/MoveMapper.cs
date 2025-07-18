using Application.DTOs;
using Domain.Models;

namespace Application.Mappers
{
    public static class MoveMapper
    {
        public static Move MapToDomain(this MoveDto dto)
        {
            return new Move
            {
                Id = dto.Id,
                GameId = dto.GameId,
                Player = dto.Player,
                Row = dto.Row,
                Column = dto.Column,
                Symbol = dto.Symbol,
                Timestamp = dto.Timestamp
            };
        }

        public static MoveDto MapToDto(this Move move)
        {
            return new MoveDto
            {
                Id = move.Id,
                GameId = move.GameId,
                Player = move.Player,
                Row = move.Row,
                Column = move.Column,
                Symbol = move.Symbol,
                Timestamp = move.Timestamp
            };
        }
    }
}
