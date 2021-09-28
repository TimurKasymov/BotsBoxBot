using Botsboxadminbot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Botsboxadminbot.FileProcessModule
{
    public interface IFileProcessService
    {
        public Task<string> SaveFile(BotsBoxMessageEntity part, Message message);
    }
}
