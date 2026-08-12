

using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); 
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public void Delete(T entity)
        {
             _dbSet.Remove(entity);
             _context.SaveChanges();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            return entity != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return  await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
           return await _dbSet.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }   

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }
    }
}
