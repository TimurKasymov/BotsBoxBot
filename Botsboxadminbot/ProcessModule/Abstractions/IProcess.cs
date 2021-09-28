using Botsboxadminbot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botsboxadminbot.ProcessModule.Abstractions
{
    public interface IProcess
    {
        public TelegramBotClient Client { get; set; }
        public BranchEntity LocalBranch { get; set; }
        public string Number { get; set; }
        public Action NeedFinalStage { get; set; }
        public Message LocalMessage { get; set; }
        public bool NeedSendMessageToAdmins { get; set; }
        public List<ActionEntity> Actions { get; set; }
        public List<RuleEntity> Rules { get; set; }
        public string Name { get; }
        public Tuple<List<BotsBoxMessageEntity>, BranchEntity> CreateProcess(Message message);
    }
}
