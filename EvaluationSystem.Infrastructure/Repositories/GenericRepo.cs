using EvaluationSystem.Infrastructure.Data;
using EvaluationSystem.Application.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EvaluationSystem.Infrastructure.Repositories
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepo(ApplicationDbContext context) { 
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
             await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public  IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            if (!trackChanges)
            {
                return  _dbSet.Where(expression).AsNoTracking();
            }
            return  _dbSet.Where(expression);
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T?> GetByIdAsync(int id,params Func<IQueryable<T>, IQueryable<T>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }


        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IQueryable<T> GetAll(bool trackChanges = false)
        {
            if (!trackChanges)
            {
                return _dbSet.AsNoTracking();

            }
            return _dbSet;
        }
    }
}
