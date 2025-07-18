using Domain.Interfaces.Repositories;
using Domain.Models;

namespace Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IUnitOfWorkRepository : IDisposable
    {
        IRepository<Game> Games { get; }
        IRepository<Move> Moves { get; }
        public void SaveChanges();
        public Task SaveChangesAsync();
    }
}
