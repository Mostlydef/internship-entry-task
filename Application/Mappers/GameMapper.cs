using Application.DTOs;
using Domain.Models;

namespace Application.Mappers
{
    public static class GameMapper
    {
        public static Game MapToDomain(this GameDto gameDto)
        {
            return new Game
            {
                Id = gameDto.Id,
                BoardSize = gameDto.BoardSize,
                Board = gameDto.Board ?? string.Empty,
                Status = gameDto.Status,
                PlayerO = gameDto.PlayerO,
                PlayerX = gameDto.PlayerX,
                PlayerTurn = gameDto.PlayerX,
                Moves = gameDto.Moves?.Select(MoveMapper.MapToDomain).ToList() ?? new()
            };
        }

        public static GameDto MapToDto(this Game domain)
        {
            return new GameDto(domain.BoardSize)
            {
                Id = domain.Id,
                BoardSize = domain.BoardSize,
                Status = domain.Status,
                PlayerX = domain.PlayerX,
                PlayerO = domain.PlayerO,
                PlayerTurn = domain.PlayerTurn,
                Moves = domain.Moves?.Select(MoveMapper.MapToDto).ToList() ?? new()
            };
        }

        public static Game MapToDomain(this Game game, GameDto dto)
        {
            game.Id = dto.Id;
            game.BoardSize = dto.BoardSize;
            game.PlayerX = dto.PlayerX;
            game.PlayerO = dto.PlayerO;
            game.Board = dto.Board;
            game.Status = dto.Status;
            game.Moves = dto.Moves?.Select(MoveMapper.MapToDomain).ToList() ?? new();
            game.PlayerTurn = dto.PlayerTurn;
            return game;
        }
    }
}
