using ChatAppApi.Repositories;

namespace ChatAppApi.Utils
{
    public class Transactional
    {
        private readonly AppDbContext _context;

        public Transactional(AppDbContext context)
        {
            _context = context;
        }

        public async Task RunAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}