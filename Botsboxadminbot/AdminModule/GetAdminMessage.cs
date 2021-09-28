using Botsboxadminbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botsboxadminbot.SendAdminModule
{
    public class GetAdminMessage
    {
        public void SendClient(int chatId)
        {
            var client = Bot.GetClient();

            var text1 = $@"Форма оплаты. Размер: 5000 руб.  ";
            client.SendTextMessageAsync(chatId, text1, replyMarkup: new ReplyKeyboardRemove());

        }
    }
}
