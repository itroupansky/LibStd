using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibStd.Common.Audit
{
    public interface IAuditLogging
    {

        /// <summary>
        /// Compare 2 objects and log audit for all changed fileds
        /// </summary>
        /// <param name="initialObject"></param>
        /// <param name="changedObject"></param>
        /// <param name="userId">for owner portal use 2; tenant portal use 3</param>
        /// <param name="mainId">Id of the relevant table -like Tenant, Owner</param>
        /// <param name="mainType">name of the main table -S8TEN, for example</param>
        /// <param name="ItemId">Id of the logged table -described in table name</param>
        /// <param name="tableName">table we are changing</param>
        /// <param name="function">Function changing the field</param>
        /// <param name="category">category of the records (set of letters)</param>
        /// <param name="fieldsToIgnore">list of fields to ignore during comparison</param>
        /// <param name="IVSALT">salt value if values are encrypted</param>
        /// <param name="inTransaction">true if within transaction,then no commiting data into the database</param>
        /// <returns> number if changes logged</returns>
        int LogAuditComparison(object initialObject, object changedObject, long userId, long? mainId,string mainType,long ItemId, string tableName, string function,string category=null,List<string> fieldsToIgnore=null, string IVSALT = null, bool inTransaction = false);

        /// <summary>
        /// Log Audit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mainId">Id of the relevant table -like Tenant, Owner</param>
        /// <param name="mainType">name of the main table -S8TEN, for example</param>
        /// <param name="ItemId">Id of the logged table -described in table name</param>
        /// <param name="tableName">table we are changing</param>
        /// <param name="fieldName">name of the field changed</param>
        /// <param name="function">Function changing the field</param>
        ///<param name="oldValue">old field value</param>
        ///<param name="newValue">new field value</param>
        /// <param name="category">category of the records (set of letters)</param>
        /// <param name="IVSALT">salt value if values are encrypted</param>
        ///<param name="inTransaction">true if within transaction,then no commiting data into the database</param>
        void LogAudit(long userId, long? mainId, string mainType, long ItemId, string tableName, string fieldName, string function, string oldValue, string newValue, string category = null, string IVSALT = null ,bool inTransaction = false);


        /// <summary>
        /// Log Audit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mainId">Id of the relevant table -like Tenant, Owner</param>
        /// <param name="mainType">name of the main table -S8TEN, for example</param>
        /// <param name="ItemId">Id of the logged table -described in table name</param>
        /// <param name="tableName">table we are changing</param>
        /// <param name="fieldName">name of the field changed</param>
        /// <param name="function">Function changing the field</param>
        /// <param name="data">data for the operation</param>
        /// <param name="description">description of change</param>
        /// <param name="category">category of the records (set of letters)</param>
        /// <param name="inTransaction">true if within transaction,then no commiting data into the database</param>
        /// <param name="value">encrypted value if any</param>
        /// <param name="IVSALT"> salt. If not empty, value is used</param>
        void LogAuditFunction(long userId, long? mainId, string mainType, long ItemId, string tableName, string function, string data, string description, string category = null, string value = null, string IVSALT = null, bool inTransaction = false);

    }
}
