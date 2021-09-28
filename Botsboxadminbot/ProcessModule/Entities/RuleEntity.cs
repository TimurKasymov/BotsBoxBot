using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot.ProcessModule
{
    public class RuleEntity
    {
        public string StageName { get; set; }
        public string Value { get; set; }
        public Dictionary<string, string> Rules { get; set; }

    }
}
