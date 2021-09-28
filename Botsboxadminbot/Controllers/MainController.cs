using Botsboxadminbot.Models;
using Botsboxadminbot.ProcessModule.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Botsboxadminbot.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class MainController : ControllerBase
    {
        private readonly IMainProcessor _processor;
        public MainController(IMainProcessor processor)
        {
            _processor = processor;
        }
        
        [HttpPost("messages")]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update.Message.Document != null)
                update.Message.Text = "file";

            if (update.Message?.Text != null)
            {

                var processExist = await _processor.TellIfBranchExistSetBranchSetProcess(update.Message);
                if (processExist)
                {
                    try
                    {
                        await _processor.HandleMessage(update.Message);
                    }
                    catch (Exception e)
                    {
                        return Ok();
                    }
                    return Ok();
                }

                await _processor.CreateProcess(update.Message);

                _processor.IfItsForAdmin(update.Message);
                var command = Bot.ReturnCommandIfExist(update.Message.Text);
                command?.Execute(update.Message);
            }

            if (update.CallbackQuery != null)
            {
                var command = Bot.ReturnCommandIfExist(update.CallbackQuery.Message.Text);
                command?.Execute(update.CallbackQuery);
            }
            
            return Ok();
        }
    }
}
