using Botsboxadminbot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botsboxadminbot.Commands
{
    public class SendStaticPhoto : ICommand
    {
        public string Name => @"/photo";

        public bool Contains(string messageText)
        {
            return messageText.Contains(this.Name);
        }

        public async Task Execute(Message message)
        {
            var botClient = Bot.GetClient();
            using (FileStream stream = System.IO.File.Open("/filestop/FoYvH_mLmV8.jpg", FileMode.Open))
            {
                InputOnlineFile iof = new InputOnlineFile(stream);
                iof.FileName = "static photo";
                await botClient.SendPhotoAsync(message.Chat.Id, iof);
            }
        }

        public async Task Execute(CallbackQuery query)
        {
            var botClient = Bot.GetClient();
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton("name"));

            var chatId = query.Message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
        }
    }
}
