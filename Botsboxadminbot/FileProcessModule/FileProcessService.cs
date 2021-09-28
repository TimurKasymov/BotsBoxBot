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

namespace Botsboxadminbot.FileProcessModule
{
    public class FileProcessService : IFileProcessService
    {
        string pathToFiles;

        public FileProcessService(IConfiguration configuration)
        {
            //pathToFiles = "C:/Users/Тимур/Documents/botsbox/Botsboxadminbot/Files/";
            pathToFiles = configuration["pathToFiles"];
        }
        public async Task<string> SaveFile(BotsBoxMessageEntity part, Message message)
        {
            var FileId = message.Document.FileId;
            var botClient = Bot.GetClient();
            Telegram.Bot.Types.File fileInfo = await botClient.GetFileAsync(FileId);
            var fl = message.Document.FileName.Split('.', 2);
            var fileNamePlusId = fl[0] + message.Document.FileId.Substring(0, 5) + "." + fl[1];
            using (var fileStream = new FileStream(pathToFiles + fileNamePlusId, FileMode.Create))
            {
                await botClient.DownloadFileAsync(fileInfo.FilePath, fileStream);
            }
            return fileNamePlusId;
        }
    }
}
