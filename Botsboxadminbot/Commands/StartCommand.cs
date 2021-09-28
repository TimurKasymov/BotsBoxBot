using Botsboxadminbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botsboxadminbot.Commands
{
    public class StartCommand : ICommand
    {

        public string Name => @"/start";

        public bool Contains(string message)
        {
            return message.Contains(this.Name);
        }
        public async Task Execute(Message message)
        {
            var botClient = Bot.GetClient();
            var text = $@"Вы можете написать письмо в тех поддержку или отправить заявку на создание бота";
            var chatId = message.Chat.Id;

            KeyboardButton[] keyboardButtons1 =
            {
                new KeyboardButton() { Text = "Помощь" },
            };
            KeyboardButton[] keyboardButtons2 =
            {       
                new KeyboardButton() { Text = "Создать бота" },
            };
            KeyboardButton[][] keyboardButtons3 = { keyboardButtons1, keyboardButtons2 };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons3, true);

            await botClient.SendTextMessageAsync(chatId, text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: markup);
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
