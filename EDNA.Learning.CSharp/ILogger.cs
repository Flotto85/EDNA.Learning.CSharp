using System;
using System.Collections.Generic;
using System.Text;

namespace EDNA.Learning.CSharp
{
    /// <summary>
    /// Interface for implementations that write log messages
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes a log message
        /// </summary>
        /// <param name="message">The text to log</param>
        void Log(string message);
    }
}
