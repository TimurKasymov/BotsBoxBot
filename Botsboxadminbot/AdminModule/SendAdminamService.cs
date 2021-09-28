using Botsboxadminbot.FileProcessModule;
using Botsboxadminbot.Models;
using Botsboxadminbot.Models.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Botsboxadminbot.SendAdminModule
{
    public class SendAdminamService : ISendAdminamService
    {
        string pathToFiles;
        int NikitaId;
        int TimId;
        public string p = "\n";
        IFileProcessService _fileProcessService;
        public Dictionary<string, string> paidLoad = new Dictionary<string, string>();
        public SendAdminamService(IFileProcessService fileProcessService, IConfiguration configuration)
        {
            _fileProcessService = fileProcessService;
            //pathToFiles = "C:/Users/Тимур/Documents/botsbox/Botsboxadminbot/Files/";
            pathToFiles = configuration["pathToFiles"];
            NikitaId = Convert.ToInt32(configuration["NikitaId"]);
            TimId = Convert.ToInt32(configuration["TimId"]);
        }
        public async Task Send(BranchEntity branch, Message message)
        {
            BotsBoxMessageEntity fileExist = null;
            var text = $@"Новая заявка. {p} Тип заявки:  {branch.Name}{p}";

            foreach (var part in branch.Parts)
            {
                if (part.File != null)
                {
                    fileExist = part;
                }
                text += $@"{part.Description}:  " + $@"  {part.CLientAnswer} {p}";
            }
            if (branch.Name == "Создать бота")
                text += $@"{p}{p}Чтобы одобрить завку, отпрвьте id чата клиента: {message.Chat.Id}";

            var client = Bot.GetClient();

            //Новый settings file
            if (fileExist == null)
            {
                await client.SendTextMessageAsync(TimId, text);
                await client.SendTextMessageAsync(NikitaId, text);
                return;
            }
            using (FileStream stream = System.IO.File.Open(pathToFiles + fileExist.File, FileMode.Open))
            {
                InputOnlineFile iof = new InputOnlineFile(stream);
                iof.FileName = fileExist.File;
                await client.SendDocumentAsync(TimId, iof, text);   
            }
            using (FileStream stream = System.IO.File.Open(pathToFiles + fileExist.File, FileMode.Open))
            {
                InputOnlineFile iof = new InputOnlineFile(stream);
                iof.FileName = fileExist.File;
                await client.SendDocumentAsync(NikitaId, iof, text);
            }
        }
    }
}
