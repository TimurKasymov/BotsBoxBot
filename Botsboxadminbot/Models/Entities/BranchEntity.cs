using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Models.Entities
{
    public class BranchEntity : BaseEntity
    {
        public long ChatId { get; set; }
        public string Name { get; set; }
        public List<BotsBoxMessageEntity> Parts { get; set; } 
    }
}
