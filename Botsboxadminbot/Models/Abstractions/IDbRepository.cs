using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Models.Abstractions
{
    public interface IDbRepository<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task CreateManyAsync(List<TEntity> entity);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Get(Func<TEntity, bool> predicate);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task SaveChangesAsync();
    }
}
