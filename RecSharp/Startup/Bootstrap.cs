using RecSharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Startup
{
    public class Bootstrap
    {
        public static void Load(BootstrapOptions bootstrapOptions)
        {
            Constants.Logger.Info($"Loading RecSharp v{CurrentVersion} into \"{Assembly.GetEntryAssembly().GetName().Name}\"");
            Logger.CurrentLogType = bootstrapOptions.MinimumLogType;
            HttpLogger.LogHeaders = bootstrapOptions.LogHTTPHeaders;
        }

        public static readonly string CurrentVersion = "0.0.1";
    }
}
