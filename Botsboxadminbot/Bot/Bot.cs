using Botsboxadminbot.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Botsboxadminbot.Models
{
    public class Bot
    {
        private static TelegramBotClient _botClient;
        private static List<ICommand> _commandsList;

        public static IReadOnlyList<ICommand> Commands => _commandsList.AsReadOnly();
        public static TelegramBotClient GetClient()
        {
            return _botClient;
        }
        
        public static ICommand ReturnCommandIfExist(string text)
        {
            var result = _commandsList.FirstOrDefault(command => command.Contains(text));
            return result;
        }

        public static async Task<TelegramBotClient> SetBotClientAsync()
        {
            _commandsList = new List<ICommand>();
            _commandsList.Add(new StartCommand());
            _commandsList.Add(new SendStaticPhoto());
            _botClient = new TelegramBotClient(AppSettings.Token);
            //await _botClient.SetWebhookAsync("https://1eb5-188-170-78-244.ngrok.io/api/bot/messages");
            _botClient.SetWebhookAsync("https://botsbox.ru/api/bot/messages").Wait();
            return _botClient;
        }
    }
}
