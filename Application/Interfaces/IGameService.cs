using Application.DTOs;
using Application.Errors;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IGameService
    {
        GameDto CreateGame();
        (bool Success, GameError? Error, MoveDto? Move, GameDto UpdatedGame) MakeMove(GameDto game, MoveRequest request);
        Task AddAsync(GameDto game);
        Task RemoveAsync(GameDto game);
        Task<GameDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<GameDto>> GetAllAsync();
        Task Update(GameDto game);
        Task Update(Guid id, GameDto dto);
    }
}
