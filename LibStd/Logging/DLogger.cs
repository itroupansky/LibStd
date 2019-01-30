using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
namespace LibStd.Logging
{

    /// <summary>
    /// this class is for wrapping diagnostic logging
    /// </summary>
    public class DLogger : ILog
    {


        private Logger _logger;

        #region Constructors

        /// <summary>
        /// Ctor-Creates an instance of the DLogger class with passed in retain days.
        /// </summary>
        /// <param name="logConnection">Directory for logs or Database Connection string, depending on log type</param>
        /// <param name="logName">Name of log</param>
        /// <param name="level">Log level</param>
        /// <param name="logType">null or F- file, D- database</param>
        public DLogger(LogLevel level, string logConnection, string logName, string logType=null)
        {
            Init( logName,logType, level,logConnection, 30);
        }

        /// <summary>
        /// Ctor-Creates an instance of the DLogger class with passed in retain days.
        /// </summary>
        /// <param name="logDirectory">Directory for logs</param>
        /// <param name="logName">Name of log</param>
        /// <param name="level">Log level</param>
        /// <param name="retainDays">how many days to retain log archives for</param>
        public DLogger(LogLevel level, string logDirectory, string logName, int retainDays)
        {

            Init( logName,"F", level,logDirectory, retainDays);
        }


        /// <summary>
        /// Ctor-Creates an instance of the DLogger class with passed in retain days.
        /// </summary>
        /// <param name="logDirectory">Directory for logs</param>
        /// <param name="logName">Name of log</param>
        /// <param name="level">Log level</param>
        /// <param name="retainDays">how many days to retain log archives for</param>
        /// <param name="archivePeriod">Archive every what-Day, Hour, Minute,None,Year</param>
        public DLogger(LogLevel level, string logDirectory, string logName, int retainDays, string archivePeriod)
        {
            FileArchivePeriod period;
            if (!Enum.TryParse<FileArchivePeriod>(archivePeriod, out period)) period = FileArchivePeriod.Day;

            Init( logName, "F",level,logDirectory, retainDays, period);
        }

        /// <summary>
        /// Creates an instance of the NLog Log class. Can take a database connection
        /// </summary>
        /// <param name="logConnection">Directory for logs or Database Connection string, depending on log type</param>
        /// <param name="logName">Name of log</param>
        ///  /// <param name="logType">null or F- file, D- database</param>
        /// <param name="level">Log level</param>
        /// <param name="retainDays">how many days to retain log archives for</param>
        /// <param name="archivePeriod">Archive every what</param>
        /// 
        private void Init( string logName,string logType, LogLevel level,string logConnection, int retainDays=0, FileArchivePeriod archivePeriod = FileArchivePeriod.Day)
        {
            if (logType == "D")
            {
                LogManager.LoadConfiguration("nlog.config");
            }
            LoggingConfiguration config = LogManager.Configuration ?? new LoggingConfiguration();
           
            Target target = null;
            if (logType != "D")
            {//file type configuration
                if (!logConnection.EndsWith("\\"))
                {
                    logConnection = logConnection + "\\";
                }
                const string dateFormat = "${date:format=yyyy-MM-dd}.";
                var logDir = Path.GetDirectoryName(logConnection);
               
                var filePath = Path.Combine(logDir, dateFormat + string.Format("{0}.txt", logName));

                FileTarget fileTarget = new FileTarget()
                {
                    FileName = filePath,
                    Layout = "[${date:format=hh\\:mm\\:ss.fff tt}] ${level}: ${message}",
                    ArchiveEvery = archivePeriod,
                    ArchiveFileName = "{#}.log.txt",
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    ArchiveDateFormat = "yyyy-MM-dd", /*fileTarget.ArchiveFileName = filePath.Replace("Current.", dateFormat);*/
                    MaxArchiveFiles = retainDays
                };


                target = fileTarget;
            } else
            {
                var dbTarget = new DatabaseTarget();
                dbTarget.Name = "test";
                dbTarget.ConnectionString = logConnection;
                //aspnet stuff at the moment doesn't work for Standard 2.0-6-19-2018- may work in the future
                //keep commented code below
               /* dbTarget.CommandText = "insert into TLog(Date,Level,Logger,Message,MachineName,UserName," + "" +
                    "SiteName,ThreadId, ServerName, Port, Url, ServerAddress, RemoteAddress,Properties) "
                    + " values(GetDate(),@Level,@Logger,@Message,@MachineName,@UserName," +
                    "@SiteName,@ThreadId, @ServerName, @Port, @Url, @ServerAddress, @RemoteAddress, @Properties);";
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@UserName", new NLog.Layouts.SimpleLayout("${identity}")));//${identity}
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@ServerName", new NLog.Layouts.SimpleLayout("${aspnet-request-host}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Port", new NLog.Layouts.SimpleLayout("${aspnet-request:serverVariable=SERVER_PORT}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Url", new NLog.Layouts.SimpleLayout("${aspnet-request-url}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@ServerAddress", new NLog.Layouts.SimpleLayout("${aspnet-request:serverVariable=LOCAL_ADDR}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@RemoteAddress", new NLog.Layouts.SimpleLayout("${aspnet-request:serverVariable=REMOTE_ADDR}:${aspnet-request:serverVariable=REMOTE_PORT}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Properties", new NLog.Layouts.SimpleLayout("${all-event-properties}")));*/

                dbTarget.CommandText = "insert into TLog(Date,Level,Logger,Message,MachineName,Environment,ThreadId) "
                    + " values(GetDate(),@Level,@Logger,@Message,@MachineName,@Environment,@ThreadId);";



               
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Level", new NLog.Layouts.SimpleLayout("${level}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Logger", new NLog.Layouts.SimpleLayout("${logger}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Message", new NLog.Layouts.SimpleLayout("${message}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@MachineName", new NLog.Layouts.SimpleLayout("${machinename}")));
             
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@Environment", new NLog.Layouts.SimpleLayout("${appsetting:name=AppSettings.Environment:default=NA}")));
                dbTarget.Parameters.Add(new DatabaseParameterInfo("@ThreadId", new NLog.Layouts.SimpleLayout("${threadid}")));


                target = dbTarget;
              
               

            }

            config.AddTarget("db", target);

           var rule = new LoggingRule(logName, GetLogLevel(level), target);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config; // Assign back to force NLog to update the configuration
           
            _logger = LogManager.GetLogger(logName);


        }
        #endregion Constructors

        /// <summary>
        /// Reconfigure logger to use certain level of logging
        /// </summary>
        /// <param name="level">log level</param>
        public void Reconfigure(LogLevel level)
        {
            var levels = Enum.GetValues(typeof(LogLevel));
            int iLevel = (int)level;
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                foreach (LogLevel lvl in levels)
                {
                    if (lvl == LogLevel.Off) continue;
                    int iLvl = (int)lvl;
                    if (level == LogLevel.Off || iLvl > iLevel)
                        rule.DisableLoggingForLevel(GetLogLevel(lvl));//lower levels disabled
                    else
                        rule.EnableLoggingForLevel(GetLogLevel(lvl));//upper levels enabled
                }
            }

            //Call to update existing Loggers created with GetLogger() or 
            //GetCurrentClassLogger()
            LogManager.ReconfigExistingLoggers();

        }
        /// <summary>
        /// Log an object with the passed in level.
        /// </summary>
        /// <param name="message">Message object</param>
        public void Log(LogLevel level, object message)
        {
            _logger.Log(GetLogLevel(level), message);
        }
        /// <summary>
        /// Log a message string with the passed in level.
        /// </summary>
        /// <param name="message">Message string</param>
        public void Log(LogLevel level, string message)
        {
            _logger.Log(GetLogLevel(level), message);
        }

        /// <summary>
        /// Log a formatted string with the passed in level.
        /// </summary>
        /// <param name="level">Logging level</param>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Log(LogLevel level, string message, params object[] args)
        {
            _logger.Log(GetLogLevel(level), message, args);
        }
        /// <summary>
        /// Log an object with the passed in level.
        /// </summary>
        /// <param name="level">logging level</param>
        /// <param name="user">logged in user name</param>
        /// <param name="message">Message object</param>
        public void Log(LogLevel level, string user, object message)
        {
            _logger.Log(GetLogLevel(level), "user: " + user +" : "+ message);
        }
        /// <summary>
        /// Log a message string with the passed in level.
        /// </summary>
        ///    /// <param name="level">logging level</param>
        /// <param name="user">logged in user name</param>
        /// <param name="message">Message string</param>
        public void Log(LogLevel level, string user, string message)
        {
           
                _logger.Log(GetLogLevel(level), "user: " + user + " : " + message);
            
        }

        /// <summary>
        /// Log a formatted string with the passed in level.
        /// </summary>
        /// <param name="level">Logging level</param>
        /// <param name="user">logged in user name</param>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Log(LogLevel level, string user, string message, params object[] args)
        {
            _logger.Log(GetLogLevel(level), "user: " + user + " : " + message, args);
        }
        /// <summary>
        /// Log a message object with the Debug level.
        /// </summary>
        /// <param name="message">Message object</param>
        public void Debug(object message)
        {
            _logger.Debug(message);
        }

        /// <summary>
        /// Log a message string with the Debug level.
        /// </summary>
        /// <param name="message">Message string</param>
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        /// <summary>
        /// Log a formatted string with the Debug level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        /// <summary>
        /// Log a message object with the Info level.
        /// </summary>
        /// <param name="message">Message object</param>
        public void Info(object message)
        {
            _logger.Info(message);
        }

        /// <summary>
        /// Log a message string with the Info level.
        /// </summary>
        /// <param name="message">Message string</param>
        public void Info(string message)
        {
            _logger.Info(message);
        }

        /// <summary>
        /// Log a formatted string with the Info level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        /// <summary>
        /// Log a message object with the Warn level.
        /// </summary>
        /// <param name="message">Message object</param>
        public void Warn(object message)
        {
            _logger.Warn(message);
        }

        /// <summary>
        /// Log a message string with the Warn level.
        /// </summary>
        /// <param name="message">Message string</param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        /// <summary>
        /// Log a formatted string with the Warn level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        /// <summary>
        /// Log a message object with the Error level.
        /// </summary>
        /// <param name="message">Message object</param>
        public void Error(object message)
        {
            _logger.Error(message);
        }

        /// <summary>
        /// Log a message string with the Error level.
        /// </summary>
        /// <param name="message">Message string</param>
        public void Error(string message)
        {
            _logger.Error(message);
        }

        /// <summary>
        /// Log a formatted string with the Error level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        /// <summary>
        /// Log a message object with the Fatal level.
        /// </summary>
        /// <param name="message">Message object</param>
        public void Fatal(object message)
        {
            _logger.Fatal(message);
        }

        /// <summary>
        /// Log a message string with the Fatal level.
        /// </summary>
        /// <param name="message">Message string</param>
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        /// <summary>
        /// Log a formatted string with the Fatal level.
        /// </summary>
        /// <param name="message">Message format</param>
        /// <param name="args">Format args</param>
        public void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }
        /// <summary>
        /// convenience method to replace Console.WriteLine in .exe-s
        /// </summary>
        /// <param name="message">message</param>
        public void LogInfoAndOutput(string message)
        {
            Log(LogLevel.Info, message);
            Console.WriteLine(message);
        }
        /// <summary>
        /// convenience method to replace Console.WriteLine in .exe-s
        /// </summary>
        /// <param name="message">message</param>
        public void LogErrorAndOutput(string message)
        {
            Log(LogLevel.Error, message);
            Console.WriteLine(message);
        }
        /// <summary>
        /// Method to convert from Proteck.LogLevel to NLog.LogLevl
        /// </summary>
        /// <param name="level">Proteck log level</param>
        /// <returns>NLog log level</returns>
        private static NLog.LogLevel GetLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Full:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case LogLevel.Off:
                default:
                    return NLog.LogLevel.Off;
            }
        }
    }
}



