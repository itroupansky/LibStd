using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LibStd.Logging;

namespace LibStd.Common
{
    /// <summary>
    /// this class holds common configuration functions
    /// </summary>
    public interface IStdConfig
    {



        /// <summary>
        /// get app setting from .config file and return empty if no setting is present
        /// </summary>
        /// <param name="settingName"></param>
       string GetAppSetting(string settingName, string sectionName = "AppSettings");

        /// <summary>
        /// get app setting from .config file as integer and return null if no setting is present
        /// </summary>
        /// <param name="settingName"></param>
        int? GetIntAppSetting(string settingName, int? dflt = null, string sectionName = "AppSettings");

        /// <summary>
        /// get app setting from .config file as boolean and return false if no setting is present
        /// </summary>
        /// <param name="settingName"></param>
       bool GetBoolAppSetting(string settingName, string sectionName = "AppSettings");

        /// <summary>
        /// returns default logging direcotory
        /// </summary>
        /// <returns></returns>
        string GetLoggingDirectory();

        /// <summary>
        /// returns default logging level
        /// </summary>
        /// <returns></returns>
        LogLevel GetLogLevel();

        /// <summary>
        /// get connection string by name from .config file and return empty if none is present
        /// </summary>
        /// <param name="settingName"></param>
        string GetConnectionString(string Name);

        /// <summary>
        /// get connection string by name from .config file and return empty if none is present
        /// if environment setting is present- replace Env. setting name with Env Setting value 
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="envName">name of Environmental variable containing name of the sql server</param>
        string GetEnvConnectionString(string Name, string envName = "SQLSERVERLOCATION");


    }
}
