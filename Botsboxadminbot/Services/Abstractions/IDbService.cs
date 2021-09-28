using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Services.Abstractions
{
    public interface IDbService<TEntity>
    {
        public Task Create(TEntity entity);
        public Task<TEntity> Get(int id);
        public TEntity GetByChatId(long id);
        public List<TEntity> GetManyByChatId(long id);
        public Task Update(TEntity message);
        public Task<List<TEntity>> GetMany(Func<TEntity, bool> predicate);
        public Task Delete(long id);
        public Task CreateMany(List<TEntity> messages);
        public Task<TEntity> Get(Func<TEntity, bool> predicate);
    }
}
