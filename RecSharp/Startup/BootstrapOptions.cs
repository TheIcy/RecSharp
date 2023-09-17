using RecSharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Startup
{
    public class BootstrapOptions
    {
        public LogTypes MinimumLogType { get; set; }
        public bool LogHTTPHeaders { get; set; }
    }
}
