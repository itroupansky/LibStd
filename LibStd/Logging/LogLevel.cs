using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibStd.Logging
{
    /// <summary>
    /// define levels for logging
    /// </summary>
    public enum LogLevel
    {      /// <summary>
        /// Fatal level - potentially requires an e-mail to support
        /// </summary>
        Fatal =0,
            /// <summary>
        ///  Error log level.
        /// </summary>
        Error=1,
        /// <summary>
        ///  Warn log level.
        /// </summary>
        Warn=2,
        /// <summary>
        ///  Info log level.
        /// </summary>
        Info=3,
        /// <summary>
        ///  Debug log level.
        /// </summary>
         Debug=4,
         /// <summary>
         ///  Full instrumentation log level - potentially includes method tracing.
         /// </summary>
        Full=5,
        /// <summary>
        ///  No logging.
        /// </summary>
        Off=6
       
        
    }
}
