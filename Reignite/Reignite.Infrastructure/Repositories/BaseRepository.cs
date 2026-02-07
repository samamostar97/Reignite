using Reignite.Application.Common;
using Reignite.Application.IRepositories;
using Reignite.Core.Entities;
using Reignite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Repositories
{
    public class BaseRepository<T, TKey> : IRepository<T, TKey>
        where T : BaseEntity
    {
        protected readonly ReigniteDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ReigniteDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual IQueryable<T> AsQueryable()
        {
            return _dbSet.Where(e => !e.IsDeleted);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }


        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity is null || entity.IsDeleted)
                throw new KeyNotFoundException($"Entity tipa {typeof(T).Name} sa id '{id}' ne postoji.");

            return entity;
        }

        public virtual async Task<PagedResult<T>> GetPagedAsync(IQueryable<T> query, PaginationRequest request)
        {
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((request.PageNumber - 1) * request.PageSize)
               .Take(request.PageSize)
               .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber
            };
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}

