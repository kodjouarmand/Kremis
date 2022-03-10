using Kremis.Domain.Contexts;
using Kremis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public class BaseRepository<TEntity, TEntityKey> 
        : IBaseRepository<TEntity, TEntityKey> where TEntity 
        : BaseEntity<TEntityKey>
    {
        private readonly ApplicationDbContext _db;
        protected DbSet<TEntity> dbSet;

        public BaseRepository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<TEntity>();
        }

        public TEntityKey Add(TEntity entity)
        {
            dbSet.Add(entity);
            return entity.Id;
        }

        public TEntity GetById(TEntityKey id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query;
        }

        public void Delete(TEntityKey id)
        {
            TEntity entity = dbSet.Find(id);
            dbSet.Remove(entity);
        }

        public void Cancel(TEntityKey id, string cancelationReason = null)
        {
            TEntity entity = dbSet.Find(id);
            entity.IsCanceled = true;
            entity.CancelationReason = cancelationReason;
            dbSet.Update(entity);
        }
    }
}
