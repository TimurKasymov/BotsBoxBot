using Botsboxadminbot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Models.Repositories
{
    public sealed class BranchRepository : DbRepository<BranchEntity>
    {
        public BranchRepository(BotContext botContext) : base(botContext) { }
    }
}
