using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;
using Talabat.Repository.Specifications;

namespace Talabat.Repository.Repositories
{
    public class GenericRepositories<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepositories(StoreDbContext context) //Ask CLR to create obj from storedbcontext Implicitly
        {
            _context = context;
        }

     

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IReadOnlyList<T>)await _context.Products.Include(P => P.Brand).Include(P => P.Category).ToListAsync();

            }
          return await _context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec).ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _context.Products.Where(P => P.Id == id).Include(P => P.Brand).Include(P => P.Category).FirstOrDefaultAsync() as T;

            }
            return await _context.Set<T>().FindAsync(id);

        }

        public  async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await  ApplySpecifications(spec).CountAsync();
        }

        public Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec);
        }

        public void Update(T item) => _context.Set<T>().Update(item);
        public async Task AddAsync(T item) => await _context.Set<T>().AddAsync(item);
        public void Delete(T item) => _context.Set<T>().Remove(item);

     
    }
}
