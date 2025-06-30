//using DAL.Data;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class Repository : IDisposable
    {
        private readonly AppDbContext _context;
        private bool _disposed;
        
        public Repository(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            _context = new AppDbContext(optionsBuilder.Options);
        }

        private Dictionary<Type, object> _repositories;

        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity
        {
            _repositories ??= new Dictionary<Type, object>();

            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T>(_context);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
               _context.Dispose();
            }
            _disposed = true;
        }
    }
}
