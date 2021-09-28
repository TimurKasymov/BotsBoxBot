using Botsboxadminbot.FileProcessModule;
using Botsboxadminbot.Models;
using Botsboxadminbot.Models.Entities;
using Botsboxadminbot.ProcessModule.Abstractions;
using Botsboxadminbot.SendAdminModule;
using Botsboxadminbot.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotsBox.Shared.Enums;
using Microsoft.OpenApi.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace Botsboxadminbot.ProcessModule
{
    public class MainProcessor : IMainProcessor
    {
        private readonly IDbService<BranchEntity> _branchService;
        private readonly IDbService<BotsBoxMessageEntity> _messageService;
        private readonly List<IProcess> _processes;
        private Message _localMessage;
        private readonly IFileProcessService _fileProcessService;
        private BranchEntity _localbranch;
        private readonly TelegramBotClient _client;
        private IProcess _currentProcess;
        private GetAdminMessage _getAdminMessageService;
        private Action _action = null;
        private BotsBoxMessageEntity Part;
        private IConfiguration _configuration;
        private string _actionDescription = null;
        private readonly ISendAdminamService _sendAdminamService;

        public MainProcessor(IDbService<BranchEntity> branchService,
            IDbService<BotsBoxMessageEntity> messageService,
            IFileProcessService fileProcessService,
            ISendAdminamService sendAdminamService,
            IConfiguration configuration)
        {
            _processes = new List<IProcess>();
            _branchService = branchService;
            _messageService = messageService;
            _configuration = configuration;
            _fileProcessService = fileProcessService;
            _client = Bot.GetClient();
            _sendAdminamService = sendAdminamService;
            _getAdminMessageService = new GetAdminMessage();

            _processes.Add(new HelpProcess());
            _processes.Add(new CreateBotProcess());
        }

        public async Task HandleMessage(Message message)
        {
            switch(message.Text)
            {
                case "Отмена":
                    await DeleteProcess(message, true);
                    break;
                case "Вернуться":
                    await IfNeedReturnBack();
                    break;
                default:
                    await ProceedProcess();
                    break;
            }
        }
        
        /// <summary>
        /// Return true if any branch exists, and if there is, set LocalMessage and 
        /// LocalBranch inside the certain process, otherwise return false
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> TellIfBranchExistSetBranchSetProcess(Message message)
        {
            _localMessage = message;

            // TODO: Зачем мы это делаем?
            // Без этого из базы данных приходит branch без messages
            _messageService.GetManyByChatId(message.Chat.Id);
            
            // TODO: Может вместо IsComplited использовать IsActive? Done
            var branch = await _branchService
                .Get(b => b.ChatId == message.Chat.Id);

            if (branch == null)
                return false;

            if (!branch.IsActive)
            {
                await DeleteProcess(message, false);
                return false;
            }

            _localbranch = branch;
            foreach (var process in _processes)
            {
                if (process.Name == branch.Name)
                {
                    _currentProcess = process;
                    _currentProcess.LocalMessage = message;
                    _currentProcess.LocalBranch = branch;
                }
            }
            
            if (_localbranch.IsActive)
                return true;
            return false;
        }

        public void IfItsForAdmin(Message message)
        {
            var N = Convert.ToInt32(_configuration["NikitaId"]);
            var T = Convert.ToInt32(_configuration["TimId"]);
            if (message.Chat.Id == N
                || message.Chat.Id == T)
            {
                try
                {
                    var ichatId = Convert.ToInt32(message.Text);
                    _getAdminMessageService.SendClient(ichatId);
                }
                catch
                {
                    return;
                }
            }
        }


        /// <summary>
        /// If client message has not been saved because of 
        /// saving respons of past step in the ProceedProcess method -- save 
        /// client's response AND start new step 
        /// </summary>
        /// <returns></returns>
        public async Task SetsClientAnswerSetIsComplited(bool notWaited)
        {
            //gets client answer and set IsComplited 
            if (notWaited)
            {
                var part = _currentProcess.LocalBranch.Parts
                    .Where(p => p.IsCompleted && p.CLientAnswer == null)
                    .ToList()
                    .LastOrDefault();
                if (part != null)
                {
                    await SteerProcess(part);
                    await SaveFile(part);
                    Part = part;
                    part.CLientAnswer = _currentProcess.LocalMessage.Text;
                    part.IsCompleted = true;
                }
            }

            // return Action of first didnt complete part of the branch
            var partIsNotCompleted = _currentProcess.LocalBranch.Parts
                .Where(p => !p.IsCompleted )
                .ToList()
                .FirstOrDefault();

            if (partIsNotCompleted != null)
            {
                await GetsActionAndItsDescription(partIsNotCompleted);
                partIsNotCompleted.IHaveBeenInvoked = true;
            }
        }
        /// <summary>
        /// Save File using corresponding module
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public async Task SaveFile(BotsBoxMessageEntity part)
        {
            if (_currentProcess.LocalMessage.Text == "file")
            {
                var fileName = await _fileProcessService.SaveFile(part, _localMessage);
                part.File = fileName;
            }
        }

        /// <summary>
        /// Set Action that latly will be invoked 
        /// Set Action's Description that lately will be used by SealPrinter method
        /// so we can identify what method was called on each step 
        /// </summary>
        /// <param name="botsBoxMessageEntity"></param>
        public async Task GetsActionAndItsDescription(BotsBoxMessageEntity botsBoxMessageEntity)
        {
            
            ActionEntity Action = _currentProcess.Actions
                .Where(a => a.MethodName == botsBoxMessageEntity.MethodName)
                .ToList()
                .FirstOrDefault();
            _action = Action.Action; // will be invoked at the end of the ProceedProcess method
            _actionDescription = Action.Description; // will be used by the SealPrinter method
        }

        /// <summary>
        /// Set identifier by which it will be clear what method client was returned to
        /// AND set IsCompleted = false to the last triggered part, so lately we can trigger it
        /// because client didnt answer this question yet
        /// </summary>
        /// <returns></returns>
        public async Task SetNeedChangeInvokeIt(BotsBoxMessageEntity botsBoxMessageEntity)
        {
            botsBoxMessageEntity.NeedsToChange = true;

            await GetsActionAndItsDescription(botsBoxMessageEntity);

            var lastMethod = _currentProcess.LocalBranch.Parts
                .Where(p => p.IsCompleted )
                .ToList()
                .LastOrDefault();
            lastMethod.IsCompleted = false;
            Part = lastMethod;

            botsBoxMessageEntity.IHaveBeenInvoked = true;
        }

        /// <summary>
        /// Central method 
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ProceedProcess()
        {
            var checkIfNotWaited = true;

            var checkIfWaite = _currentProcess.LocalBranch.Parts?
                .Where(p => p.NeedsToChange)?
                .ToList()?
                .FirstOrDefault();

            if (checkIfWaite != null)
            {
                await SaveFile(checkIfWaite);
                checkIfWaite.NeedsToChange = false;
                checkIfWaite.CLientAnswer = _currentProcess.LocalMessage.Text;
                checkIfWaite.IsCompleted = true;
                checkIfNotWaited = false;
            }

            var needReturnBackPart = _currentProcess.LocalBranch.Parts
                .Where(p => p.Description == _localMessage.Text)
                .ToList()
                .FirstOrDefault();

            if (needReturnBackPart == null)
                await SetsClientAnswerSetIsComplited(checkIfNotWaited);
            else
                await SetNeedChangeInvokeIt(needReturnBackPart);

            if (_action != null)
            {
                await InvokeAction();
            }
            else
            {
                await ApplyFeaturesAskedByProcess();

                await KickUserToStartCommand();

                //Cleaning BranchEntity and MessageEntity
                await DeleteProcess(_localMessage, false);
            }
            await Update();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task InvokeAction()
        {
            var num = _currentProcess.LocalBranch
               .Parts
               .Where(p => p.IsCompleted)
               .ToList().Count + 1;

            var waitNum = _currentProcess.LocalBranch
                .Parts
                .Where(p => p.NeedsToChange)
                .ToList();

            if (waitNum.Count != 0)
                num = waitNum.Count;

            _currentProcess.Number = $@"{num} из {_currentProcess.LocalBranch.Parts
                .Where(p => p.IsActive)
                .ToList()
                .Count}";
            SealPrinter(_actionDescription);
            _action.Invoke();
            var invoked = _currentProcess.LocalBranch.Parts
                .Where(p => p.IHaveBeenInvoked)
                .ToList()
                .FirstOrDefault();
            invoked.IsCompleted = true;
            invoked.IHaveBeenInvoked = false;
        }

        /// <summary>
        /// Return user at start command
        /// </summary>
        /// <returns></returns>
        public async Task KickUserToStartCommand()
        {
            var c = Bot.ReturnCommandIfExist("/start");
            await c?.Execute(_currentProcess.LocalMessage);
        }

        /// <summary>
        /// Apply (use) features that was asked by certain process, for example NeedSendMessageToAdmins
        /// or NeedFinalStage
        /// </summary>
        /// <returns></returns>
        public async Task ApplyFeaturesAskedByProcess()
        {
            if (_currentProcess.NeedSendMessageToAdmins)
            {
                await _sendAdminamService.Send(_currentProcess.LocalBranch, _currentProcess.LocalMessage);
            }
            _currentProcess.NeedFinalStage?.Invoke();
        }

        /// <summary>
        /// Updates current branch and its parts
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            var branchBack = _currentProcess.LocalBranch;
            foreach (var p in branchBack.Parts)
            {
                await _messageService.Update(p);
            }
            await _branchService.Update(branchBack);
        }

        /// <summary>
        /// Create a new process using scheme set by certain process **Dictionary Actions**
        /// Trigger TellIfBranchExistSetBranchSetProcess and ProceedProcess
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task CreateProcess(Message message)
        {
            var branch = await _branchService
                .Get(b => (b.IsActive) && b.ChatId == message.Chat.Id);
            
            if (branch != null)
                return;
            
            foreach (var p in _processes)
            {
                if (p.Name != message.Text) 
                    continue;
                    
                p.LocalMessage = message;
                var tuple = p.CreateProcess(message);

                await _branchService.Create(tuple.Item2);
                    
                var savedBranch = await _branchService
                    .Get(b => b.IsActive && b.ChatId == message.Chat.Id);
                    
                tuple.Item1.ForEach(m => m.Branch = savedBranch);
                    
                await _messageService.CreateMany(tuple.Item1);
                    
                await _branchService.Update(savedBranch);

                await TellIfBranchExistSetBranchSetProcess(_localMessage);

                await ProceedProcess();
                
            }
        }

        /// <summary>
        /// Steering the process using scheme set by certain process **List<Dictionary> Rules**
        /// </summary>
        /// <param name="previousPart"></param>
        public async Task SteerProcess(BotsBoxMessageEntity previousPart)
        {
            if (previousPart == null)
                return;
            foreach (var r in _currentProcess.Rules)
            {
                if (previousPart.MethodName == r.StageName && _currentProcess.LocalMessage.Text == r.Value)
                {
                    foreach (var concreteRule in r.Rules)
                    {
                        var part = _currentProcess.LocalBranch.Parts.Where(p => p.MethodName == concreteRule.Key)
                            .ToList().FirstOrDefault();
                        if (part != null)
                        {
                            if (concreteRule.Value == "Delete")
                            {
                                await DeletePartOfProcess(part.MethodName);
                            }
                            else
                                part.MethodName = concreteRule.Value;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Puts signs on Branch to find out later which question the client answered
        /// </summary>
        /// <param name="print"></param>
        public void SealPrinter(string description)
        {             
            var method = _currentProcess.LocalBranch?.Parts?.Where(p => p.IHaveBeenInvoked).ToList().LastOrDefault();
            method.Description = description;
        }

        /// <summary>
        /// Returning the user to the answered questions
        /// </summary>
        public async Task IfNeedReturnBack()
        {
            var allParts = _localbranch.Parts
                .Where(p => p.CLientAnswer!=null)
                .ToList();
            
            var buttons = new List<KeyboardButton>();
            foreach (var p in allParts)
            {
                buttons.Add(new KeyboardButton() { Text = p.Description });
            }

            var markup = new ReplyKeyboardMarkup(buttons, true);
            var text = "Выберите сообщение, к которому Вы хотите вернуться";
            
            await _client.SendTextMessageAsync(_currentProcess.LocalMessage.Chat.Id, text, replyMarkup: markup);
        }

        public async Task DeletePartOfProcess(string processPartMethodName)
        {
            var part = await _messageService.Get(b => b.MethodName == processPartMethodName);
            await _messageService.Delete(part.Id);
        }
        /// <summary>
        /// delete process 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ClientAsked"></param>
        /// <returns></returns>
        public async Task DeleteProcess(Message message, bool ClientAsked)
        {
            var branch = await _branchService.Get(b =>  b.ChatId == message.Chat.Id);
            var allMessages = _messageService.GetManyByChatId(message.Chat.Id);

            if (allMessages != null)
            {
                foreach (var m in allMessages)
                {
                    await _messageService.Delete(m.Id);
                }
            }


            await _branchService.Delete(branch.Id);

            if (ClientAsked)
                await _client.SendTextMessageAsync(message.Chat.Id, "Заявка отменена", replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
