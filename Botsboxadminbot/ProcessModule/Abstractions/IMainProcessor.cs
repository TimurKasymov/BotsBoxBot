using Botsboxadminbot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Botsboxadminbot.ProcessModule.Abstractions
{
    public interface IMainProcessor
    {
        public Task<bool> TellIfBranchExistSetBranchSetProcess(Message message);
        public Task ProceedProcess();
        public Task IfNeedReturnBack();
        public void IfItsForAdmin(Message message);
        public Task CreateProcess(Message message);
        public Task HandleMessage(Message message);
    }
}
