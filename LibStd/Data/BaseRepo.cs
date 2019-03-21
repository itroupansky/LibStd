using System;
using System.Collections.Generic;
using System.Text;
using LibStd.Logging;

namespace LibStd.Data
{
    /// <summary>
    /// base repository class
    /// </summary>
    public class BaseRepo
    {
        protected ILog _log;
        protected IDAO _dao;


        public BaseRepo(ILog log, IDAO dao)
        {
            _log = log;
            _dao = dao;

        }
    }
}
