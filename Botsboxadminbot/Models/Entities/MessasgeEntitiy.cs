using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Models.Entities
{
    public class BotsBoxMessageEntity : BaseEntity
    {
        public long ChatId { get; set; }
        public string CLientAnswer { get; set; }
        public BranchEntity Branch { get; set; }
        public string MethodName { get; set; }
        public string File { get; set; }// new
        public string Description { get; set; }
        public bool NeedsToChange { get; set; }
        public bool IHaveBeenInvoked { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
