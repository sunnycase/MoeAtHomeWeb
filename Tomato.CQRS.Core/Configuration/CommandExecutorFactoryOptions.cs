using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core.Configuration
{
    public class CommandExecutorFactoryOptions
    {
        public Assembly ExecutorsDefineAssembly { get; set; }

        public string ExecutorsDefineNamespace { get; set; }
    }
}
