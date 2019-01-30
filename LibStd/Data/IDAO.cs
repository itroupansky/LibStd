using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibStd.Data
{
    /// <summary>
    /// Interface for dealing with data
    /// </summary>
    public interface IDAO
    {

        /// <summary>
        /// synchronously query database and return list of objects of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">parameters as anonimous object</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns>List of T</returns>
        List<T> Query<T>(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false);

        /// <summary>
        /// synchronously query database and return object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">parameters as anonimous object</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns></returns>
        T GetScalar<T>(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false, int? timeout = null);        

        /// <summary>
        /// synchronously execute non-query statement
        /// </summary>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">parameters as anonimous object</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns>number of affected records</returns>
        int Execute(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false);

        /// <summary>
        /// execute non-query statement(S) within transaction
        /// </summary>
        /// <param name="sql">sql statement</param>
        /// <param name="parameters">anonymous object or list of objects</param>
        /// <param name="useConnectionExclusive"> if true:open and close connection within this method, if false: - open the connectionif it is not opened. Default is true</param>
        /// <param name="closeConnection">used if useConnectionExclusive=false. Close the connection within the method.Default is false</param>
        /// <returns>number of records affected</returns>
         int ExecuteWithinTransaction(string sql, object parameters = null, bool useConnectionExclusive = true, bool closeConnection = false);

        /// execute some action within transaction
        /// </summary>
        /// <param name="action"action to execute, returning T</param>
        T ExecuteWithinTrans<T>(Func<T> action);

        

        #region Generic CRUD for Simple Classes

        void Insert<T>(T entity, bool useConnectionExclusive = true, bool closeConnection = false);
        void Update<T>(T entity, bool useConnectionExclusive = true, bool closeConnection = false);
        void Delete<T>(T entity, bool useConnectionExclusive = true, bool closeConnection = false);        

        IEnumerable<T> Select<T>(object criteria = null, bool useConnectionExclusive = true, bool closeConnection = false);
        IEnumerable<T> Select<T>(string criteria =null,object parameters=null, bool useConnectionExclusive = true, bool closeConnection = false);

        #endregion Generic CRUD for Simple Classes
    }
}
