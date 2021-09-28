using Botsboxadminbot.Models.Abstractions;
using Botsboxadminbot.Models.Entities;
using Botsboxadminbot.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Services
{
    public class BranchService : IDbService<BranchEntity>
    {
        private readonly IDbRepository<BranchEntity> _repository;
        public BranchService(IDbRepository<BranchEntity> dbRepository)
        {
            _repository = dbRepository;
        }
        public async Task Create(BranchEntity branch)
        {
            try
            {
                await _repository.CreateAsync(branch);
                await _repository.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<BranchEntity> Get(Func<BranchEntity, bool> predicate)
        {
            try
            {
                var branches = _repository.Get(predicate);
                var branch =  branches.ToList().FirstOrDefault();
                if (branch == null)
                    throw new Exception("Объект не найден");
                return branch;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<List<BranchEntity>> GetMany(Func<BranchEntity, bool> predicate)
        {
            try
            {
                var branches = await _repository.Get(predicate).ToListAsync();
                return branches;
            }
            catch
            {
                Console.WriteLine("Объект не найден");
                return null;
            }
        }
        public List<BranchEntity> GetManyByChatId(long id)
        {
            try
            {
                var branches = _repository.Get(m => m.ChatId == id && m.IsActive);
                var branch = branches.ToList();
                if (!branch.Any())
                    throw new Exception("Объект не найден");
                return branch;
            }
            catch
            {
                Console.WriteLine("Объект не найден");
                return null;
            }
        }

        /// <summary>
        /// получает объект
        /// </summary>
        /// <returns></returns>

        public async Task<BranchEntity> Get(int id)
        {
            try
            {
                var branches = _repository.Get(e => e.Id == id && e.IsActive);
                var branch = await branches.FirstOrDefaultAsync();
                if (branch == null)
                    throw new Exception("Объект не найден");
                return branch;
            }
            catch
            {
                Console.WriteLine("Объект не найден");
                return null;
            }
        }

        public BranchEntity GetByChatId(long id)
        {
            try
            {
                var branches = _repository.Get(m => m.ChatId == id && m.IsActive);
                var branch = branches.ToList().FirstOrDefault();
                if (branch == null)
                    throw new Exception("Объект не найден");
                return branch;
            }
            catch
            {
                Console.WriteLine("Объект не найден");
                return null;
            }
        }


        /// <summary>
        /// обновляет объект
        /// </summary>
        /// <returns></returns>

        public async Task Update(BranchEntity branch)
        {
            try
            {
                _repository.Update(branch);
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var e = ex.Entries;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        /// <summary>
        /// удалает объект
        /// </summary>
        /// <returns></returns>

        public async Task Delete(long id)
        {
            try
            {
                var branches = _repository.Get(e => e.Id == id );
                _repository.Delete(branches.FirstOrDefault());
                await _repository.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public async Task CreateMany(List<BranchEntity> branch)
        {
            try
            {
                await _repository.CreateManyAsync(branch);
                await _repository.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
