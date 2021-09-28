using Botsboxadminbot.Models.Abstractions;
using Botsboxadminbot.Models.Entities;
using Botsboxadminbot.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Botsboxadminbot.Services
{
    public class BotMessageService :  IDbService<BotsBoxMessageEntity>
    {

        private readonly IDbRepository<BotsBoxMessageEntity> _repository;
        public BotMessageService(IDbRepository<BotsBoxMessageEntity> dbRepository)
        {
            _repository = dbRepository;
        }
        public async Task Create(BotsBoxMessageEntity message)
        {
            try
            {
                await _repository.CreateAsync(message);
                await _repository.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<BotsBoxMessageEntity> GetManyByChatId(long id)
        {
            var messages = _repository
                .Get(m => m.ChatId == id && m.IsActive);
            var message = messages.ToList();

            return message;
        }

        /// <summary>
        /// получает объект
        /// </summary>
        /// <returns></returns>
        public async Task<BotsBoxMessageEntity> Get(int id)
        {
            try
            {
                var messages = _repository.Get(e => e.Id == id);
                var message = (await messages.ToListAsync()).FirstOrDefault();
                if (message == null)
                    throw new Exception("Объект не найден");
                return message;
            }
            catch
            {
                Console.WriteLine("Объект не найден");
                return null;
            }
        }

        public async Task<BotsBoxMessageEntity> Get(Func<BotsBoxMessageEntity, bool> predicate)
        {
            var messages = _repository.Get(predicate);
            var message =  messages.ToList();
            var message1 = message.FirstOrDefault();
            if (message == null)
                Console.WriteLine("Объект не найден");

            return message1;
        }

        public async Task<List<BotsBoxMessageEntity>> GetMany(Func<BotsBoxMessageEntity, bool> predicate)
        {
            try
            {
                var messages = await _repository.Get(predicate).ToListAsync();
                return messages;
            }
            catch
            {
                Console.WriteLine("Объект не найден");
                return null;
            }
        }

        public  BotsBoxMessageEntity GetByChatId(long id)
        {
            try
            {
                var messages = _repository.Get(m => m.ChatId == id && m.IsActive);
                var message = messages.ToList().FirstOrDefault();
                if (message == null)
                    throw new Exception("Объект не найден");
                return message;
            }
            catch
            {
                Console.WriteLine("Бранч не найден");
                return null;
            }
        }



        /// <summary>
        /// обновляет объект
        /// </summary>
        /// <returns></returns>

        public async Task Update(BotsBoxMessageEntity message)
        {
            try
            {
                _repository.Update(message);
                await _repository.SaveChangesAsync();
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
                var messages = _repository.Get(e => e.Id == id);
                var message = messages.ToList().FirstOrDefault();
                if (message == null)
                    throw new Exception("Объект не найден");
                _repository.Delete(message);
                await _repository.SaveChangesAsync();

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public async Task CreateMany(List<BotsBoxMessageEntity> messages)
        {
            try
            {
                await _repository.CreateManyAsync(messages);
                await _repository.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
