using System;
using System.Collections.Generic;
using System.Text;

namespace EDNA.Learning.CSharp
{
    internal class SafeLogger : ILogger
    {
        private ILogger innerLogger;

        public SafeLogger(ILogger logger)
        {
            innerLogger = logger;
        }

        public void Log(string message)
        {
            try
            {
                innerLogger.Log(message);
            }
            catch(Exception e)
            {
                Console.WriteLine("Logging failes");
                Console.WriteLine(e.Message);
            }
        }
    }
}
