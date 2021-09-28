using Botsboxadminbot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Botsboxadminbot.SendAdminModule
{
    public interface ISendAdminamService
    {
        public Task Send(BranchEntity branch, Message message);

    }
}
