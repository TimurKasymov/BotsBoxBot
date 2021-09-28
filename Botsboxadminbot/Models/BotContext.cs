using Botsboxadminbot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.Models
{
    public class BotContext : DbContext
    {
        public DbSet<BotsBoxMessageEntity> Messages { get; set; }
        public DbSet<BranchEntity> Branches { get; set; }
        public BotContext(DbContextOptions<BotContext> options) : base(options) { }
    }
}
