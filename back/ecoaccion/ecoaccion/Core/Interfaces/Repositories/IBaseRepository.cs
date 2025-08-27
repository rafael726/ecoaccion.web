using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ecoaccion.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync( int id );
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync( Expression<Func<T, bool>> predicate );
        Task AddAsync( T entity );
        void Update( T entity );
        Task UpdateAsync( T entity );
        void Delete( T entity );
        Task<int> SaveChangesAsync();
        public IEnumerable<T> Search( Func<T, bool> filer );
           
    }
}
