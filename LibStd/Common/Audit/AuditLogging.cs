using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibStd.Logging;

namespace LibStd.Common.Audit
{
    public class AuditLogging : IAuditLogging
    {
        private CompareLogic compareLogic;
        private readonly ILog _log;
        private IAuditRepo _repo;
        private List<string> encryptedFields = new List<string>() { "LONGID" };
        private List<string> fieldsAlwaysIgnore= new List<string>() { "LAST_UPDATE","LAST_USER" };
        public const string ENCRYPT_VALUE = "Encrypt the value";
        private int maxDifferences = 40;
        public int MaxDifferences { get { return maxDifferences; } }
        private IStdConfig _configurator;

        public AuditLogging(IAuditRepo repo, ILog log, IStdConfig config)
        {
            _log = log;
            _repo = repo;
            _configurator = config;
            
            string maxD = _configurator.GetAppSetting("MaxDifferences");
            if (maxD != "" && !Int32.TryParse(maxD, out maxDifferences))
            {
                maxDifferences = 40;
            }
            compareLogic = new CompareLogic(new ComparisonConfig()
            {
                MaxDifferences = maxDifferences

            });
        }

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
        public int LogAuditComparison(object initialObject, object changedObject, long userId, long? mainId, string mainType, long ItemId, string tableName, string function, string category = null, List<string> fieldsToIgnore = null,string IVSALT=null, bool inTransaction = false)
        {
            int iCount = 0;
            if (fieldsToIgnore != null)
            {
                compareLogic.Config.MembersToIgnore = fieldsToIgnore;
            }
            else
            {
                compareLogic.Config.MembersToIgnore = new List<string>();
            }
            ComparisonResult result = compareLogic.Compare(initialObject, changedObject);
          
            int count = result.Differences.Count;
            foreach (Difference diff in result.Differences)
            {
                var fieldName = diff.PropertyName; //.Substring(1); //dropping first letter
                if (!fieldsAlwaysIgnore.Contains(fieldName) && (fieldsToIgnore == null || !fieldsToIgnore.Contains(fieldName)))
                {
                    string ivsalt = null;
                    if (encryptedFields.Contains(fieldName)) ivsalt = IVSALT;
                    iCount++;
                    LogAudit(userId, mainId, mainType, ItemId, tableName, fieldName, function, diff.Object1Value, diff.Object2Value, category, ivsalt,true);
                }

            }

            if (!inTransaction)
            {
                _repo.SaveData();
            }
            return iCount;
        }

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
       public void LogAudit(long userId, long? mainId, string mainType, long ItemId, string tableName, string fieldName, string function, string oldValue, string newValue,  string category = null, string IVSALT=null,bool inTransaction = false)
        {
            AuditModel model = new AuditModel()
            {
                Category = category,
                Date = DateTime.Now,
                Description = string.Format(fieldName??""+" - Old Value: {0}, New Value: {1}", oldValue, newValue),
                MainRecordID = mainId,
                MainRecordType = mainType,
                Item_ID = ItemId,
                Item_Logged = tableName,
                FieldName = fieldName,
                Operation=function,
                Prev_Value = oldValue,
                New_Value = newValue,
                Data = "Change of Field",
                UserId = userId,
                IsEncrypted=!string.IsNullOrWhiteSpace(IVSALT),
                IVSALT=IVSALT
                

            };
            _repo.InsertAuditLog(model, inTransaction);
            if (!inTransaction)
            {
                _repo.SaveData();
            }
        }


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
        public void LogAuditFunction(long userId, long? mainId, string mainType, long ItemId, string tableName,  string function, string data, string description, string category = null,string value=null, string IVSALT = null, bool inTransaction = false)
        {
            AuditModel model = new AuditModel()
            {
                Category = category,
                Date = DateTime.Now,
                Description = description,
                MainRecordID = mainId,
                MainRecordType = mainType,
                Item_ID = ItemId,
                Item_Logged = tableName,
                Operation = function,
                UserId = userId,
                Data=data,
                IVSALT=IVSALT,
                IsEncrypted= !string.IsNullOrWhiteSpace(IVSALT),
                New_Value=value,
                Prev_Value=IVSALT!=null?ENCRYPT_VALUE:null


            };
            _repo.InsertAuditLog(model, inTransaction);
            if (!inTransaction)
            {
                _repo.SaveData();
            }
        }


    }
}
