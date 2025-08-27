using ecoaccion.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ecoaccion.Infrastructure.Persistence;

namespace ecoaccion.Infrastructure.Common
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository( AppDbContext context )
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync( int id )
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync( Expression<Func<T, bool>> predicate )
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task AddAsync( T entity )
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update( T entity )
        {
            _dbSet.Update(entity);
        }

        public async Task UpdateAsync( T entity )
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete( T entity )
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IEnumerable<T> Search( Func<T, bool> filer )
           => _dbSet.Where(filer).ToList();
    }
}
