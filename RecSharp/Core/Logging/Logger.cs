namespace RecSharp.Core.Logging
{
    public class Logger : ILogger
    {
        public Logger(string source)
        {
            this.Source = source;
        }

        public void Info(string msg)
        {
            _currentMessages.Add(new(msg, LogTypes.Info));
            this.Log();
        }
        
        public void Warn(string msg)
        {
            _currentMessages.Add(new(msg, LogTypes.Warning));
            this.Log();
        }
        
        public void Error(string msg)
        {
            _currentMessages.Add(new(msg, LogTypes.Error));
            this.Log();
        }
        
        public void Debug(string msg)
        {
            _currentMessages.Add(new(msg, LogTypes.Debug));
            this.Log();
        }
        
        public void Trace(string msg)
        {
            _currentMessages.Add(new(msg, LogTypes.Trace));
            this.Log();
        }

        private void Log()
        {
            for (int i = 0; i < this._currentMessages.Count; i++)
            {
                LogMessage msg = this._currentMessages[i];
                if (msg.LogType > LogTypes.Error && CurrentLogType < LogTypes.Debug)
                    return;
                string currentTime = DateTime.Now.ToString("T");
                ConsoleColor color = LogTypeColors[msg.LogType];

                Console.Write($"[");
                Console.ForegroundColor = color;
                Console.Write(currentTime);
                Console.ResetColor();
                Console.Write($"] ");

                Console.Write($"[");
                Console.ForegroundColor = color;
                Console.Write(this.Source);
                Console.ResetColor();
                Console.Write($"] ");
                
                Console.Write($"[");
                Console.ForegroundColor = color;
                Console.Write(msg.LogType.ToString());
                Console.ResetColor();
                Console.Write($"] ");

                Console.WriteLine(msg.Message);

                _currentMessages.Remove(msg);
            }
        }

        private List<LogMessage> _currentMessages = new List<LogMessage>();

        private Dictionary<LogTypes, ConsoleColor> LogTypeColors = new Dictionary<LogTypes, ConsoleColor>()
        {
            { LogTypes.Info, ConsoleColor.Green },
            { LogTypes.Warning, ConsoleColor.Yellow },
            { LogTypes.Error, ConsoleColor.Red },
            { LogTypes.Debug, ConsoleColor.Blue },
            { LogTypes.Trace, ConsoleColor.DarkBlue }
        };

        private string Source { get; set; } = "";

        public static LogTypes CurrentLogType { get; set; } = LogTypes.Info;
    }

    class LogMessage
    {
        public LogMessage(string message, LogTypes type)
        {
            this.Message = message;
            this.LogType = type;
        }

        public string Message { get; set; }
        public LogTypes LogType { get; set; }
    }
}
