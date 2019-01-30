using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LibStd.Logging;

namespace LibStd.Common.Core
{
    /// <summary>
    /// this class holds common configuration functions
    /// </summary>
    public class Configurator:IStdConfig
    {
      private static  IConfigurationRoot _configuration; //needs to be assigned at Startup
        public static IStdConfig configurator;


        public Configurator(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            configurator = this;
        }
        
        /// <summary>
        /// get app setting from .config file and return empty if no setting is present
        /// </summary>
        /// <param name="settingName"></param>
        public string GetAppSetting(string settingName, string sectionName= "AppSettings")
        {
            //see if jsn configuration value is overriden with environmental variable
            var env = _configuration[sectionName + "_" + settingName];
            if (!string.IsNullOrEmpty(env)) return env;
            return _configuration[sectionName+":"+settingName] ?? "";
        }

        /// <summary>
        /// get app setting from .config file as integer and return null if no setting is present
        /// </summary>
        /// <param name="settingName"></param>
        public int? GetIntAppSetting(string settingName,int? dflt=null, string sectionName = "AppSettings")
        {
            int? iout = dflt;
            string conf = GetAppSetting(settingName,sectionName);
            int num = 0;
            if (conf !="" && int.TryParse(conf,out num))
            {
                iout = num;
            }
            return iout;
        }
        
        /// <summary>
        /// get app setting from .config file as boolean and return false if no setting is present
        /// </summary>
        /// <param name="settingName"></param>
        public bool GetBoolAppSetting(string settingName,  string sectionName = "AppSettings")
        {
            bool bout = false;
            string conf = GetAppSetting(settingName, sectionName);
            bool bb;
            if (conf != "" && bool.TryParse(conf, out bb))
            {
                bout =bb;
            }
            return bout;

        }

        /// <summary>
        /// returns default logging direcotory
        /// </summary>
        /// <returns></returns>
        public string GetLoggingDirectory()
        {  //here we don't override with env variable, since in containers we don't use log path
            return _configuration["AppSettings:" + "LogPath"] ?? "C:\\Tracker\\Logs";
        }

        /// <summary>
        /// returns default logging level
        /// </summary>
        /// <returns></returns>
        public LogLevel GetLogLevel()
        {

            string sLevel = GetAppSetting("LogLevel");
            sLevel=sLevel==""? "Error":sLevel;
            LogLevel level;
            if (!Enum.TryParse<LogLevel>(sLevel, out level)) level = LogLevel.Error;
            return level;
        }

        /// <summary>
        /// get connection string by name from .config file and return empty if none is present
        /// </summary>
        /// <param name="settingName"></param>
        public string GetConnectionString(string Name)
        {
            return _configuration["ConnectionStrings:" + Name] ?? "";
        }

        /// <summary>
        /// get connection string by name from .config file and return empty if none is present
        /// if environment setting is present- replace Env. setting name with Env Setting value 
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="envName">name of Environmental variable containing name of the sql server</param>
        public string GetEnvConnectionString(string Name, string envName = "SQLSERVERLOCATION")
        {
            var cs = _configuration["ConnectionStrings:" + Name] ?? "";
            var env = _configuration[envName];
            if (!string.IsNullOrEmpty(env)) cs = cs.Replace(envName, env);
            return cs;
        }

    }
}
