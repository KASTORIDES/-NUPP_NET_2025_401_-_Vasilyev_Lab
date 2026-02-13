using StudentSystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentSystem.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity); Task DeleteAllAsync();
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly StudentSystemContext _context;
        private readonly DbSet<T> _set;

        public Repository(StudentSystemContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _set = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id) => await _set.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync()
        {
            _set.RemoveRange(_set);
            await _context.SaveChangesAsync();
        }

    }
}
