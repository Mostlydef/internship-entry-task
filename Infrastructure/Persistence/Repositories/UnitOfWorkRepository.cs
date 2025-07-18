using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Persistence.Repositories.Interfaces;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly AppDbContext _context;

        private IRepository<Game>? _gameRepository;
        private IRepository<Move>? _moveRepository;

        public UnitOfWorkRepository(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<Game> Games => _gameRepository ??= new GameRepository(_context);
        public IRepository<Move> Moves => _moveRepository ??= new Repository<Move>(_context);

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
