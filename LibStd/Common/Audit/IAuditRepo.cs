using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibStd.Common.Audit
{
    public interface IAuditRepo
    {
        /// <summary>
        /// insert one record to AuditLog
        /// </summary>
        /// <param name="model">data to insert</param>
        ///  /// <param name="inTransaction">true if within transaction,then no commiting data into the database</param>
        void InsertAuditLog(AuditModel model, bool InTransaction = false);
        
        /// <summary>
        /// save audit log data if underlying database system requires it.
        /// </summary>
        void SaveData();
    }
}
