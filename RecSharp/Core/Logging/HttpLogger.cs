using RecSharp.Core.Internal.Http;
using RecSharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Core.Logging
{
    internal class HttpLogger
    {
        public void RequestLog(HttpMessage msg)
        {
            this._currentMessages.Add(msg);
            this.Log();
        }

        private void Log()
        {
            for (int i = 0; i < this._currentMessages.Count; i++)
            {
                HttpMessage msg = this._currentMessages[i];

                string str = "------------------------ HTTP Request ------------------------";
                Console.WriteLine(str);
                Console.WriteLine($"Service: {msg.Service}");
                Console.WriteLine($"Url: {msg.Url}");
                Console.WriteLine($"HTTP Method: {msg.HttpMethod}");
                if(msg.HttpMethod != HttpMethods.GET)
                {
                    Console.WriteLine($"Content-Type: {msg.ContentType}");
                    Console.WriteLine($"Body Data: {msg.BodyData}");
                }
                if(LogHeaders && msg.Headers.Count > 0)
                {
                    Console.WriteLine("Headers");
                    foreach(var s in msg.Headers)
                    {
                        Console.WriteLine($" {s} -> {msg.Headers.Get(s.ToString())}");
                    }
                }
                if (msg.QueryStrings.Count > 0)
                {
                    Console.WriteLine("Query Strings");
                    foreach (var s in msg.QueryStrings)
                    {
                        Console.WriteLine($" {s} -> {msg.QueryStrings.Get(s.ToString())}");
                    }
                }
                Console.WriteLine($"Status: {msg.Status}");
                Console.WriteLine($"Response: {msg.Response}");

                foreach(char c in str)
                {
                    Console.Write("-");
                }
                Console.WriteLine();

                _currentMessages.Remove(msg);
            }
        }

        public static bool LogHeaders { get; set; } = false;

        private List<HttpMessage> _currentMessages = new List<HttpMessage>();

        private Dictionary<LogTypes, ConsoleColor> LogTypeColors = new Dictionary<LogTypes, ConsoleColor>()
        {
            { LogTypes.Info, ConsoleColor.Green },
            { LogTypes.Warning, ConsoleColor.Yellow },
            { LogTypes.Error, ConsoleColor.Red },
            { LogTypes.Debug, ConsoleColor.Blue },
            { LogTypes.Trace, ConsoleColor.DarkBlue }
        };
    }
}

class HttpMessage
{
    public string Url { get; set; }
    public HttpMethods HttpMethod { get; set; }
    public string BodyData { get; set; }
    public string ContentType { get; set; }
    public NameValueCollection Headers { get; set; }
    public NameValueCollection QueryStrings { get; set; }
    public string Response { get; set; }
    public string Status { get; set; }

    public string Service { get; set; }
}