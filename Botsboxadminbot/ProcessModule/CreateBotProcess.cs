using Botsboxadminbot.Models;
using Botsboxadminbot.Models.Entities;
using Botsboxadminbot.ProcessModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botsboxadminbot.ProcessModule
{
    public class CreateBotProcess : IProcess
    {
        public string Name { get; set; } = "Создать бота";
        public Message LocalMessage { get; set; } = null;
        public string p = "\n";
        public List<ActionEntity> Actions { get; set; }
        public IMainProcessor processor;
        public BranchEntity LocalBranch { get; set; }
        public string Number { get; set; }
        public Action NeedFinalStage { get; set; }
        public List<RuleEntity> Rules { get; set; }
        public bool NeedSendMessageToAdmins { get; set; } = true;

        public TelegramBotClient Client { get; set; } = Bot.GetClient();

        public CreateBotProcess()
        {
            Actions = new List<ActionEntity>
            {
                new ActionEntity { MethodName = "ChoosTypeOrg",
                Action = () => ChoosTypeOrg(),
                Description = "Выбор типа организации"},

                new ActionEntity { MethodName = "DescribeGoal",
                Action = () => DescribeGoal(),
                Description = "Описание бота"},

                new ActionEntity { MethodName = "UsingPeriod",
                Action = () => UsingPeriod(),
                Description = "Выбор периода"},

                new ActionEntity { MethodName = "UsingPeriodAsLegalEntity",
                Action = () => UsingPeriodAsLegalEntity(),
                Description = "Выбор периода"},

                new ActionEntity { MethodName = "InsertFile",
                Action = () => InsertFile(),
                Description = "Загрузка файла с тех заданимем"},

                new ActionEntity { MethodName = "DescribeGoalAsLegalEntity",
                Action = () => DescribeGoalAsLegalEntity(),
                Description = "Описание бота"},

                new ActionEntity { MethodName = "TermsOfUse",
                Action = () => Policy(),
                Description = "Пользовательское соглашение"},

                new ActionEntity { MethodName = "RejectedTermsOfUse",
                Action = () => RejectTermsOfUse(),
                Description = "Отмена"},

            };

            Rules = new List<RuleEntity>
            {
                {new RuleEntity{StageName = "ChoosTypeOrg",
                Value = "Юридическое лицо",
                Rules = new Dictionary<string, string>
                {
                    {"DescribeGoal",  "DescribeGoalAsLegalEntity"},
                    {"UsingPeriod",  "UsingPeriodAsLegalEntity"}
                }
                } },
                {new RuleEntity{StageName = "TermsOfUse",
                Value = "Да",
                Rules = new Dictionary<string, string>
                {
                    {"RejectedTermsOfUse",  "Delete"},
                }
                } }
            };
            NeedFinalStage = () => FinalStage();
        }

        public Tuple<List<BotsBoxMessageEntity>, BranchEntity> CreateProcess(Message message)
        {
            var text = $@"{LocalMessage.From.FirstName}, {p} Давайте составим заявку на создание бота. ";
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text, replyMarkup: new ReplyKeyboardRemove());

            var newProcess = new BranchEntity
            {
                ChatId = message.Chat.Id,
                Name = Name,
            };
            var newStages = new List<BotsBoxMessageEntity>
            {
                new BotsBoxMessageEntity
                {
                    ChatId =  message.Chat.Id,
                    MethodName = "ChoosTypeOrg",
                },
                new BotsBoxMessageEntity
                {
                    ChatId =  message.Chat.Id,
                    MethodName = "DescribeGoal",
                },
                new BotsBoxMessageEntity
                {
                    ChatId =  message.Chat.Id,
                    MethodName = "InsertFile",
                },
                new BotsBoxMessageEntity
                {
                    ChatId =  message.Chat.Id,
                    MethodName = "UsingPeriod",
                },
                new BotsBoxMessageEntity
                {
                    ChatId =  message.Chat.Id,
                    MethodName = "TermsOfUse",
                    IsActive = false
                },
                new BotsBoxMessageEntity
                {
                    ChatId =  message.Chat.Id,
                    MethodName = "RejectedTermsOfUse",
                    IsActive = false
                },
            };
            
            LocalBranch = newProcess;
            return Tuple.Create(newStages, newProcess);
        }

        public void RejectTermsOfUse()
        {
            LocalBranch.IsActive = false;
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, "Заявка отменена", replyMarkup: new ReplyKeyboardRemove());
        }

        public void FinalStage()
        {
            var text1 = $@"Мы приняли Вашу заявку. Ожидаем ответа администраторов";
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text1, replyMarkup: new ReplyKeyboardRemove());
        }


        public void Policy()
        {
            var text1 = $@"Вы принимаете пользовательское соглашение? ";
            KeyboardButton[] keyboardButtons =
{
                new KeyboardButton(){Text = "Да" },
                new KeyboardButton(){Text = "Нет" }
            };
            KeyboardButton[][] keyboardButtons3 = { keyboardButtons };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons3, true);
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text1, replyMarkup: markup);
        }

        public void ChoosTypeOrg()
        {
            var text1 = $@"Вопрос {Number}{p} Вы являетесь юридическим лицом или физическим лицом? ";

            KeyboardButton[] keyboardButtons =
{
                new KeyboardButton(){Text = "Отмена" },
                new KeyboardButton(){Text = "Вернуться" }
            };
            KeyboardButton[] keyboardButtons1 =
{
                new KeyboardButton() { Text = "Юридическое лицо" },
            };
            KeyboardButton[] keyboardButtons2 =
{
                new KeyboardButton() { Text = "Физическое лицо" },
            };
            KeyboardButton[][] keyboardButtons3 = { keyboardButtons, keyboardButtons1, keyboardButtons2 };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons3, true);
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text1, replyMarkup: markup);
        }
        public void DescribeGoal()
        {

            KeyboardButton[] keyboardButtons =
            {
                new KeyboardButton(){Text = "Отмена" },
                new KeyboardButton(){Text = "Вернуться" }
            };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons, true);
            var text = $@"Вопрос {Number}{p}Кратко опишите Функционал будущего бота. ";
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text, replyMarkup: markup);
        }
        public void InsertFile()
        {
            KeyboardButton[] keyboardButtons =
            {
                new KeyboardButton(){Text = "Отмена" },
                new KeyboardButton(){Text = "Вернуться" }
            };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons, true);
            var text = $@"Вопрос {Number}{p}Вставьте файл со всеми техическими подробностями. ";
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text, replyMarkup: markup);
        }
        public void UsingPeriod()
        {
            var text = $@"Вопрос {Number}{p}Выберите период пользования ботом.";
            KeyboardButton[] keyboardButtons =
{
                new KeyboardButton(){Text = "Отмена" },
                new KeyboardButton(){Text = "Вернуться" }
            };
            KeyboardButton[] keyboardButtons1 =
{
                new KeyboardButton() { Text = "месяц" },
            };
            KeyboardButton[] keyboardButtons2 =
{
                new KeyboardButton() { Text = "пол года" },
            };
            KeyboardButton[] keyboardButtons3 =
{
                new KeyboardButton() { Text = "год" },
            };
            KeyboardButton[] keyboardButtons4 =
{
                new KeyboardButton() { Text = "больше года" },
            };
            KeyboardButton[][] keyboardButtons5 = { keyboardButtons, keyboardButtons1, keyboardButtons2, keyboardButtons3,
            keyboardButtons4 };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons5, true);
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text, replyMarkup: markup);
        }

        public void DescribeGoalAsLegalEntity()
        {
            KeyboardButton[] keyboardButtons =
            {
                new KeyboardButton(){Text = "Отмена" },
                new KeyboardButton(){Text = "Вернуться" }
            };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons, true);
            var text = $@"Вопрос {Number}{p}Кратко опишите Функционал будущего бота Вашей организации. ";
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text, replyMarkup: markup);
        }

        public void UsingPeriodAsLegalEntity()
        { 

            var text = $@"Вопрос {Number}{p}Выберите период пользования ботом. Для юридичесикх лиц длина периода начинается от года";
            KeyboardButton[] keyboardButtons =
{
                new KeyboardButton(){Text = "отмена" },
                new KeyboardButton(){Text = "Вернуться" }
            };
            KeyboardButton[] keyboardButtons1 =
{
                new KeyboardButton() { Text = "год" },
            };
            KeyboardButton[] keyboardButtons2 =
{
                new KeyboardButton() { Text = "два года" },
            };
            KeyboardButton[] keyboardButtons3 =
{
                new KeyboardButton() { Text = "пять лет" },
            };
            KeyboardButton[] keyboardButtons4 =
{
                new KeyboardButton() { Text = "больше пяти лет" },
            };
            KeyboardButton[][] keyboardButtons5 = { keyboardButtons, keyboardButtons1, keyboardButtons2, keyboardButtons3,
            keyboardButtons4 };
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(keyboardButtons5, true);
            Client.SendTextMessageAsync(LocalMessage.Chat.Id, text, replyMarkup: markup);

        }
    }
}
