using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibStd.Logging
{
    /// <summary>
    /// interface for a logging wrapper
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Reconfigure logger to use certain level of logging
        /// </summary>
        /// <param name="level">log level</param>
        void Reconfigure(LogLevel level);
        /// <summary>
        /// Log an object with the passed in level.
        /// </summary>
        /// <param name="message">Message object</param>
        void Log(LogLevel level, object message);
        /// <summary>
        /// Log a message string with the passed in level.
        /// </summary>
        /// <param name="message">Message string</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Log a formatted string with the passed in level.
        /// </summary>
        /// <param name="level">Logging level</param>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Log(LogLevel level, string message, params object[] args);
        /// <summary>
        /// Log an object with the passed in level.
        /// </summary>
        /// <param name="level">logging level</param>
        /// <param name="user">logged in user name</param>
        /// <param name="message">Message object</param>
        void Log(LogLevel level, string user, object message);
        /// <summary>
        /// Log a message string with the passed in level.
        /// </summary>
        ///    /// <param name="level">logging level</param>
        /// <param name="user">logged in user name</param>
        /// <param name="message">Message string</param>
        void Log(LogLevel level, string user, string message);

        /// <summary>
        /// Log a formatted string with the passed in level.
        /// </summary>
        /// <param name="level">Logging level</param>
        /// <param name="user">logged in user name</param>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Log(LogLevel level, string user, string message, params object[] args);
        /// <summary>
        /// Log a message object with the Debug level.
        /// </summary>
        /// <param name="message">Message object</param>
        void Debug(object message);

        /// <summary>
        /// Log a message string with the Debug level.
        /// </summary>
        /// <param name="message">Message string</param>
        void Debug(string message);

        /// <summary>
        /// Log a formatted string with the Debug level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Log a message object with the Info level.
        /// </summary>
        /// <param name="message">Message object</param>
        void Info(object message);

        /// <summary>
        /// Log a message string with the Info level.
        /// </summary>
        /// <param name="message">Message string</param>
        void Info(string message);

        /// <summary>
        /// Log a formatted string with the Info level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Log a message object with the Warn level.
        /// </summary>
        /// <param name="message">Message object</param>
        void Warn(object message);

        /// <summary>
        /// Log a message string with the Warn level.
        /// </summary>
        /// <param name="message">Message string</param>
        void Warn(string message);

        /// <summary>
        /// Log a formatted string with the Warn level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Warn(string message, params object[] args);
        /// <summary>
        /// Log a message object with the Error level.
        /// </summary>
        /// <param name="message">Message object</param>
        void Error(object message);

        /// <summary>
        /// Log a message string with the Error level.
        /// </summary>
        /// <param name="message">Message string</param>
        void Error(string message);

        /// <summary>
        /// Log a formatted string with the Error level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Log a message object with the Fatal level.
        /// </summary>
        /// <param name="message">Message object</param>
        void Fatal(object message);
        /// <summary>
        /// Log a message string with the Fatal level.
        /// </summary>
        /// <param name="message">Message string</param>
        void Fatal(string message);
        /// <summary>
        /// Log a formatted string with the Fatal level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        void Fatal(string message, params object[] args);
        /// <summary>
        /// convenience method to replace Console.WriteLine in .exe-s
        /// </summary>
        /// <param name="message">message</param>
       void LogErrorAndOutput(string message);
        /// <summary>
        /// convenience method to replace Console.WriteLine in .exe-s
        /// </summary>
        /// <param name="message">message</param>
        void LogInfoAndOutput(string message);



    }
}
