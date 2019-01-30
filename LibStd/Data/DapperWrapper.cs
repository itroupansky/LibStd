using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using LibStd.Logging;
using Dapper;
using LibStd.Common;

namespace LibStd.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DapperTable : Attribute
    {
        public DapperTable(string table)
        {
            TableName = table;
        }

        public string TableName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DapperKey : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DapperIgnore : Attribute
    {
    }

    /// <summary>
    /// class foe working with Dapper
    /// </summary>
    public class DapperWrapper : IDAO
    {
        SqlConnection _conn;
        SqlTransaction trans;
        private readonly ILog _log;
        private readonly string _connectionString;
        private static IStdConfig _configurator;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="log">Log Object</param>
        public DapperWrapper(ILog log, IStdConfig configurator, string connectionName = "TrackerConnection")
        {
            _log = log;
            _configurator = configurator;
            string connStr = _configurator.GetEnvConnectionString(connectionName);

            if (string.IsNullOrEmpty(connStr))
            {
                _log.Error("DapperWrapper-cannot find connection {0} in the config file", connectionName);
            }
            _connectionString = connStr;
            _conn = new SqlConnection(connStr);

        }


        /// <summary>
        /// synchronously query database and return list of objects of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">parameters as anonimous object</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns></returns>
        public List<T> Query<T>(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false)
        {

            return UseConnectionOrOpen<IEnumerable<T>>(() => _conn.Query<T>(sql, parameters, trans), useConnectionExclusive, closeConnection).ToList<T>();
        }

        /// <summary>
        /// synchronously query database and return object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">parameters as anonimous object</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false,int? timeout=null)
        {            
            return UseConnectionOrOpen<IEnumerable<T>>(() => _conn.Query<T>(sql, parameters, trans, commandTimeout:timeout), useConnectionExclusive, closeConnection).FirstOrDefault<T>();
        }
        /// <summary>
        /// synchronously execute non-query statement
        /// </summary>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">parameters as anonimous object</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns>number of records affected</returns>
        public int Execute(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false)
        {

            return UseConnectionOrOpen<int>(() => _conn.Execute(sql, parameters, trans), useConnectionExclusive, closeConnection);
        }

        /// <summary>
        /// execute non-query statement(S) within transaction
        /// </summary>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">anonymous object or list of objects</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns>number of records affected</returns>
        public int ExecuteWithinTransaction(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            return UseConnectionOrOpen<int>(() => ExecuteWithinTrans(sql, parameters), useConnectionExclusive, closeConnection);
        }

        private int ExecuteWithinTrans(string sql, object parameters)
        {

            trans = _conn.BeginTransaction();
            try
            {


                int ret = _conn.Execute(sql, parameters, transaction: trans);

                trans.Commit();
                trans = null;
                return ret;
            }
            catch
            {
                trans.Rollback();
                trans = null;
                throw;
            }

        }
        /// <summary>
        /// execute some action within transaction
        /// </summary>
        /// <param name="action"action to execute, returning T</param>
        public T ExecuteWithinTrans<T>(Func<T> action)
        {
            using (_conn)
            {
                if (_conn.ConnectionString == "") _conn.ConnectionString = _connectionString;
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                trans = _conn.BeginTransaction();
                try
                {


                    T ret = action();

                    trans.Commit();
                    trans = null;
                    return ret;

                }
                catch (Exception ex)
                {
                    _log.Error("DapperWarpper Transaction Rollback: {0}", ex);
                    trans.Rollback();
                    trans = null;
                    throw;
                }
            }

        }
        /// <summary>
        /// perform synchronous action opening a connenction if needeed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">action to perform</param>
        /// <param name="useConnectionExclusive">open and close connection within this function</param>
        /// <param name="closeConnection">close connection</param>
        /// <returns></returns>
        private T UseConnectionOrOpen<T>(Func<T> action, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            _log.Debug("DapperWrapper - enter UseConnectionOrOpen for {0}", typeof(T).ToString());

            if (useConnectionExclusive)
            {
                using (_conn)
                {
                    if (_conn.ConnectionString == "") _conn.ConnectionString = _connectionString;
                    if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                    T ret = action();
                    _log.Debug("DapperWrapper - exit UseConnectionOrOpen for {0}", typeof(T).ToString());
                    return ret;

                }
            }
            else
            {
                if (_conn.State == System.Data.ConnectionState.Broken)
                {
                    throw new Exception("DapperWrapper: connection is in the broken state");

                }
                if (_conn.State == System.Data.ConnectionState.Closed)
                {
                    _conn = new SqlConnection(_connectionString);
                    _conn.Open();
                }
                else if (_conn.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("DapperWrapper: connection is busy and cannot be opened");
                }
                try
                {
                    T ret = action();
                    _log.Debug("DapperWrapper - exit UseConnectionOrOpen for {0}", typeof(T).ToString());
                    return ret;
                }
                catch
                {
                    _conn.Close();
                    throw;
                }
                finally
                {
                    if (closeConnection) _conn.Close();
                }



            }
        }

        #region Automated methods for: Insert, Update, Delete
        // source : https://www.codeproject.com/Tips/844691/Generic-Insert-for-Dapper
        // These methods are provided for your convenience.
        // For simple objects they will work fine, 
        // but please be aware that they will not cover more complex scenarios!
        // Id column is assumed to be of type int IDENTITY.
        // Reflection is used to create appropriate SQL statements.
        // Even if reflection is costly in itself, the average gain 
        // compared to Entity Framework is approximately a factor 10!
        // Key property is determined by convention 
        // (Id, TypeNameId or TypeName_Id) or by custom attribute [DapperKey].
        // All properties with public setters are included. 
        // Exclusion can be manually made with custom attribute [DapperIgnore].
        // If key property is mapped to single database Identity column, 
        // then it is automatically reflected back to object.

        //
        /// <summary>
        /// Automatic generation of SELECT statement, BUT only for simple equality criterias!
        /// Example: Select<LogItem>(new {Class = "Client"})
        /// For more complex criteria it is necessary to call GetItems method with custom SQL statement.
        /// </summary>
        public IEnumerable<T> Select<T>(object criteria = null, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            var tableName = GetTableName<T>();
            var properties = ParseProperties(criteria);
            var sqlPairs = GetSqlPairs(properties.AllNames, " AND ");
            var condition = (criteria != null) ? $"WHERE {sqlPairs}" : string.Empty;
            var sql = $"SELECT * FROM [{tableName ?? typeof(T).Name}] {condition}";
            return Query<T>(sql, properties.AllPairs, useConnectionExclusive, closeConnection);
        }

        public IEnumerable<T> Select<T>(string criteria = null, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            var tableName = GetTableName<T>();
            var condition = (criteria != null) ? $"WHERE {criteria}" : string.Empty;
            var sql = $"SELECT * FROM [{tableName ?? typeof(T).Name}] {condition}";
            return Query<T>(sql, parameters, useConnectionExclusive, closeConnection);
        }



        public void Insert<T>(T obj, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            var tableName = GetTableName<T>();

            var propertyContainer = ParseProperties(obj);
            var sql = $"INSERT INTO [{tableName ?? typeof(T).Name}] ({string.Join(", ", propertyContainer.ValueNames)}) " +
                $"VALUES(@{ string.Join(", @", propertyContainer.ValueNames)}); SELECT scope_identity() ";

            var id = GetScalar<long>(sql, propertyContainer.ValuePairs, useConnectionExclusive, closeConnection);
            SetId(obj, id, propertyContainer.IdPairs);
        }

        public void Update<T>(T obj, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            var tableName = GetTableName<T>();
            var propertyContainer = ParseProperties(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            var sql = $"UPDATE [{tableName ?? typeof(T).Name}] SET {sqlValuePairs} WHERE {sqlIdPairs}";
            Execute(sql, propertyContainer.AllPairs, useConnectionExclusive, closeConnection);
        }

        public void Delete<T>(T obj, bool useConnectionExclusive = true, bool closeConnection = false)
        {
            var tableName = GetTableName<T>();
            var propertyContainer = ParseProperties(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sql = string.Format("DELETE FROM [{0}] WHERE {1}", tableName ?? typeof(T).Name, sqlIdPairs);
            Execute(sql, propertyContainer.IdPairs, useConnectionExclusive, closeConnection);
        }

        #endregion


        protected string GetTableName<T>()
        {
            var tableAttribute = typeof(T).GetCustomAttributes(
                typeof(DapperTable), true
            ).FirstOrDefault() as DapperTable;
            if (tableAttribute != null)
            {
                return tableAttribute.TableName;
            }
            return null;
        }

        #region Reflection support

        /// <summary>
        /// Retrieves a Dictionary with name and value 
        /// for all object properties matching the given criteria.
        /// </summary>
        private static PropertyContainer ParseProperties<T>(T obj)
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name;
            var validKeyNames = new[] { "Id",
            string.Format("{0}Id", typeName), string.Format("{0}_Id", typeName) };

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;

                var name = property.Name;
                var value = typeof(T).GetProperty(property.Name).GetValue(obj, null);

                if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name))
                {
                    propertyContainer.AddId(name, value);
                }
                else
                {
                    propertyContainer.AddValue(name, value);
                }
            }

            return propertyContainer;
        }

        /// <summary>
        /// Create a commaseparated list of value pairs on 
        /// the form: "key1=@value1, key2=@value2, ..."
        /// </summary>
        private static string GetSqlPairs
        (IEnumerable<string> keys, string separator = ", ")
        {
            var pairs = keys.Select(key => string.Format("{0}=@{0}", key)).ToList();
            return string.Join(separator, pairs);
        }

        private void SetId<T, T1>(T obj, T1 id, IDictionary<string, object> propertyPairs)
        {
            if (propertyPairs.Count == 1)
            {
                var propertyName = propertyPairs.Keys.First();
                var propertyInfo = obj.GetType().GetProperty(propertyName);
                if (propertyInfo.PropertyType == typeof(T1))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }

        #endregion

        private class PropertyContainer
        {
            private readonly Dictionary<string, object> _ids;
            private readonly Dictionary<string, object> _values;

            #region Properties

            internal IEnumerable<string> IdNames
            {
                get { return _ids.Keys; }
            }

            internal IEnumerable<string> ValueNames
            {
                get { return _values.Keys; }
            }

            internal IEnumerable<string> AllNames
            {
                get { return _ids.Keys.Union(_values.Keys); }
            }

            internal IDictionary<string, object> IdPairs
            {
                get { return _ids; }
            }

            internal IDictionary<string, object> ValuePairs
            {
                get { return _values; }
            }

            internal IEnumerable<KeyValuePair<string, object>> AllPairs
            {
                get { return _ids.Concat(_values); }
            }

            #endregion

            #region Constructor

            internal PropertyContainer()
            {
                _ids = new Dictionary<string, object>();
                _values = new Dictionary<string, object>();
            }

            #endregion

            #region Methods

            internal void AddId(string name, object value)
            {
                _ids.Add(name, value);
            }

            internal void AddValue(string name, object value)
            {
                _values.Add(name, value);
            }

            #endregion
        }
    }
}