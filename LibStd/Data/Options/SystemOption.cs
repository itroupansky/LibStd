using System;
using System.Collections.Generic;
using System.Linq;
using LibStd.Common.Core;
using LibStd.Logging;

namespace LibStd.Data.Options
{
    public class SystemOption
    {
        private static List<SystemOption> Options = new List<SystemOption>();
        public int? PHA_ID { get; set; }
        public int? RAA_ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? ValueInt { get; set; }
        public decimal? ValueDec { get; set; }
        public DateTime? ValueDate { get; set; }
        public string Type { get; set; }
        public bool TrackerUseOnly { get; set; }

        /// <summary>
        /// initialize system options,this has to be called at startup
        /// </summary>
        /// <param name="log"></param>
        /// <param name="connName">connection name in config file</param>
        /// <param name="ApplicationId">application id from MsgApps table [TAP = 1][TOP = 2]</param>  
        public static void Initialize(ILog log, string connName, int ApplicationId)
        {
            IDAO dao = new DapperWrapper(log, Configurator.configurator, connName);
            Initialize(log, dao, ApplicationId);
        }

        /// <summary>
        /// initialize system options, this has to be called at startup
        /// </summary>
        /// <param name="log"></param>
        /// <param name="dao"></param>
        /// <param name="ApplicationId">application id from MsgApps table</param>
        public static void Initialize(ILog log, IDAO dao, int ApplicationId)
        {
            lock (Options)
            {
                Options.Clear();
                try
                {
                    //get options
                    Options = dao.Query<SystemOption>(Resources.sql.GetOptions, new { ApplicationId });
                    //convert options
                    foreach (var opt in Options)
                    {
                        if (opt.Type == "int")
                        {
                            if (int.TryParse(opt.Value, out int opti)) opt.ValueInt = opti;
                        }
                        else if (opt.Type == "decimal")
                        {
                            if (decimal.TryParse(opt.Value, out decimal optd)) opt.ValueDec = optd;
                        }
                        else if (opt.Type == "date")
                        {
                            if (DateTime.TryParse(opt.Value, out DateTime optd)) opt.ValueDate = optd;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Cannot initialize system options: " + ex);
                    throw;
                }
            }
        }

        private static SystemOption GetOptionInternal(string name, int? PHA_ID = null, int? RAA_ID = null)
        {
            if (!PHA_ID.HasValue)
            {//TrackerOnly
                var opt = Options.Where(rec => rec.Name == name).FirstOrDefault();
                if (opt != null && !opt.TrackerUseOnly) throw new Exception($"Requested option {name} is not Tracker Only Option.");
                return opt;
            }
            else
            {
                //get Agency -level value
                SystemOption opt = null;
                if (RAA_ID.HasValue)
                {
                    opt = Options.Where(rec => rec.Name == name && rec.RAA_ID == RAA_ID).FirstOrDefault();                    
                }
                if (opt == null) //Agency-level option not found
                {
                    //get PHA-level value
                    opt = Options.Where(rec => rec.Name == name && rec.PHA_ID == PHA_ID && !rec.RAA_ID.HasValue).FirstOrDefault();
                }

                if (opt != null && opt.TrackerUseOnly) throw new Exception($"Requested for PHA/Agency option {name} is Tracker Only Option.");
                return opt;
            }
        }
        /// <summary>
        /// Get string value option
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="PHA_ID"></param>
        /// <param name="RAA_ID"></param>
        /// <returns></returns>
        public static string GetOption(string name, int? PHA_ID = null, int? RAA_ID = null)
        {
            var opt = GetOptionInternal(name, PHA_ID, RAA_ID);
            if (opt != null) return opt.Value;
            return null;
        }

        /// <summary>
        /// Get int value option
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="PHA_ID"></param>
        /// <param name="RAA_ID"></param>
        /// <returns></returns>
        public static int? GetIntOption(string name, int? PHA_ID = null, int? RAA_ID = null)
        {
            var opt = GetOptionInternal(name, PHA_ID, RAA_ID);
            if (opt != null)
            {
                if (opt.Type != "int") throw new Exception($"Option {name} is not of the type 'int'.");
                return opt.ValueInt;
            }
            return null;
        }

        /// <summary>
        /// Get decimal value option
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="PHA_ID"></param>
        /// <param name="RAA_ID"></param>
        /// <returns></returns>
        public static decimal? GetDecOption(string name, int? PHA_ID = null, int? RAA_ID = null)
        {
            var opt = GetOptionInternal(name, PHA_ID, RAA_ID);
            if (opt != null)
            {
                if (opt.Type != "decimal") throw new Exception($"Option {name} is not of the type 'decimal'.");
                return opt.ValueDec;
            }
            return null;
        }

        /// <summary>
        /// Get date value option
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="PHA_ID"></param>
        /// <param name="RAA_ID"></param>
        /// <returns></returns>
        public static DateTime? GetDateOption(string name, int? PHA_ID = null, int? RAA_ID = null)
        {
            var opt = GetOptionInternal(name, PHA_ID, RAA_ID);
            if (opt != null)
            {
                if (opt.Type != "date") throw new Exception($"Option {name} is not of the type 'date'.");
                return opt.ValueDate;
            }
            return null;
        }

    }
}
