using Application.DTOs;

namespace Application.Interfaces
{
    public interface IMoveService
    {
        Task AddAsync(MoveDto moveDto);
    }
}
