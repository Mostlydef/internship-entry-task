using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence.Repositories.Interfaces;
using Application.Mappers;

namespace Application.Service
{
    public class MoveService : IMoveService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public MoveService(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(MoveDto moveDto)
        {
            await _unitOfWork.Moves.AddAsync(moveDto.MapToDomain());
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
