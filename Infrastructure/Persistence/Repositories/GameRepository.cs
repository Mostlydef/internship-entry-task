using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class GameRepository : Repository<Game>
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context) 
            : base(context) 
        { 
            _context = context; 
        }

        public override async Task<Game?> GetByIdAsync(Guid id)
        {
            return await _context.Games.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
