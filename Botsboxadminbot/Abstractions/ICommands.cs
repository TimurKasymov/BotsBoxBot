using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botsboxadminbot.Models
{
    public interface ICommand
    {
        string Name { get; }
        Task Execute(Message message);
        Task Execute(CallbackQuery query);
        bool Contains(string messageText);
    }
}
