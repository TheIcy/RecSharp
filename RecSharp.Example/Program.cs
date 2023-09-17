using RecSharp.Core.Logging;
using RecSharp.Example.Servers;
using RecSharp.Startup;

namespace RecSharp.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RecSharp | Example";

            Bootstrap.Load(new BootstrapOptions
            {
                MinimumLogType = Core.Logging.LogTypes.Trace
            });

            logger.Info("RecSharp | Example Project.");

            new ApiServer().StartService(1200);

            Thread.Sleep(-1);
        }

        static Logger logger = new Logger("RecSharp");
    }
}