/*
 * 
 * Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
 * dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
 * All rights reserved.

 * This file is part of TenaCareeHMIS
 * TenaCareeHMIS is free software: 
 * 
 * you can redistribute it and/or modify it under the terms of the 
 * GNU General Public License as published by the Free Software Foundation, 
 * version 3 of the License, or any later version.
 * TenaCareeHMIS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
 * If not, see http://www.gnu.org/licenses/.    
 * 
 * 
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using General.Util;


namespace SqlManagement.Database
{
    [Serializable]
    public class DBConnHelper : IDisposable
    {
        #region Member Variables

        private static string _defaultConnectionString = null;
        private static bool _defaultIsLogging = false;

        //Profiling variables - TCHUBS 2007-03-02
        private static int everCreatedCount = 0;
        private static int currentInstanceCount = 0;

        public SqlConnection _connection = null;
        private SqlTransaction _sqlTransaction = null;

        public string _connectionString;


        private int _cmd_timeout = 4000;
        private string _source = null;
        private string _dbname = null;
        private string _user = null;
        private string _password = null;

        //2007-03-14 @JJ: changing the default value to false.
        private readonly bool _isLoggingSql = false;

        // @@JJ Added 4/26/05 to allow for localization to british date times
        private static bool _useBritishDates = true;

        // @@JJ Added 2/1/06 to allow for cases when we don't want to close the 
        // db connection after each use.  
        private bool _manualConnectionClose = false;

        #endregion Member Variables

        #region Constructors

        /// <summary>
        /// Creates a new instance of the DAL using settings from the application .config file
        /// </summary>
        public DBConnHelper()
        {
            // ABK: a bit of an unfortunate hack to remove the dependency on having a full app environment
            try
            {
                /*
                * In the below I swap the location that the db connection string is obtained to a new file "ccpts.config.xml." 
                * NOTE: The new class is not implemented as it should be. Right now I am just getting the db string from another location.
                * 2007-03-02 TCHUBS
                */
                DatabaseConnectionStringWrapper dbStringWrapper = AppSettingsUtility.GetDBConnectionStringFromConfigFile();
                //dbStringWrapper.Password = DevHelper.GetPwd();
                String connectionString = dbStringWrapper.ToString();

                //connectionString = "Data Source=(local)\\SmartCare40; Initial Catalog=CDC_FDB_DB; User ID=sa; Password=ruth!@#$1234;";
                connectionString = SqlManagement.Database.DBConnection.GetConnectionString();
                _connectionString = connectionString;

                // Profiling data. We make way to many instances of this class. 2007-03-02 TCHUBS
                //Logger.Log("[profiling] - SQL - New instance of SQLHelper created. ");
                //Logger.Log("[profiling] - SQL - " + ++everCreatedCount + " instances of SQLHelper have been created.");
                //Logger.Log("[profiling] - SQL - There are " + ++currentInstanceCount + " instances of SQLHelper currently allocated on the Heap.");

                ConnectionString = connectionString;

                if (_defaultConnectionString == null)
                {
                    _defaultConnectionString = _connectionString;
                }

                //2007-03-14 @JJ: moved this below the default connection string so that if an error is thrown by the line
                //below (which is due to running unit tests), the catch does not rethrow the error.
                //string str = Convert.ToString( new AppSettingsReader().GetValue( "UseSqlLogging", typeof (string) ) );
                //2006-10-24 JJ: not sure how this never got changed or it is supposed to not be changed.
                //string str = System.Convert.ToString( AppSettingsUtility.GetAppSetting( "UseSqlLogging" ) );

                //if ( str.ToLower() != Boolean.TrueString.ToLower() )
                //{
                //    _isLoggingSql = false;
                //}
            }
            catch (InvalidOperationException e)
            {
                if (_defaultConnectionString != null)
                {

                    ConnectionString = _defaultConnectionString;
                    _isLoggingSql = _defaultIsLogging;
                }
                else
                {
                    throw (e);
                }
            }

            try
            {
                // Merra Kokebie Added to fix the following error Mar. 31st, 2008
                // A transport-level error has occurred when sending the request to the server. (provider: TCP
                // Provider, error: 0 - An existing connection was forcibly closed by the
                // remote host.)
                TestConnection();
            }
            catch
            {
                // 2008-04-17 @CZUE:  do nothing - this will fail when the first real query gets called unless the 
                // connection string is changed.  This is just to not have the constructor throwing exceptions against
                // code that was previously relying on it not to
            }

        }

        // For SmartCare connection
        public DBConnHelper(bool loadFromPatientRecord)
        {
            // ABK: a bit of an unfortunate hack to remove the dependency on having a full app environment
            try
            {
                /*
                * In the below I swap the location that the db connection string is obtained to a new file "ccpts.config.xml." 
                * NOTE: The new class is not implemented as it should be. Right now I am just getting the db string from another location.
                * 2007-03-02 TCHUBS
                */
                DatabaseConnectionStringWrapper dbStringWrapper = AppSettingsUtility.GetDBConnectionStringFromConfigFile();
                //dbStringWrapper.Password = DevHelper.GetPwd();
                String connectionString = dbStringWrapper.ToString();

                //connectionString = "Data Source=(local)\\SmartCare40; Initial Catalog=CDC_FDB_DB; User ID=sa; Password=ruth!@#$1234;";
                if (loadFromPatientRecord == true)
                {
                    connectionString = SqlManagement.Database.DBConnection.GetPatientRecordConnectionString();
                }
                else
                {
                    connectionString = SqlManagement.Database.DBConnection.GetConnectionString();
                }
                _connectionString = connectionString;

                // Profiling data. We make way to many instances of this class. 2007-03-02 TCHUBS
             
                ConnectionString = connectionString;

                if (_defaultConnectionString == null)
                {
                    _defaultConnectionString = _connectionString;
                }

                //2007-03-14 @JJ: moved this below the default connection string so that if an error is thrown by the line
                //below (which is due to running unit tests), the catch does not rethrow the error.
                //string str = Convert.ToString( new AppSettingsReader().GetValue( "UseSqlLogging", typeof (string) ) );
                //2006-10-24 JJ: not sure how this never got changed or it is supposed to not be changed.
                //string str = System.Convert.ToString( AppSettingsUtility.GetAppSetting( "UseSqlLogging" ) );

                //if ( str.ToLower() != Boolean.TrueString.ToLower() )
                //{
                //    _isLoggingSql = false;
                //}
            }
            catch (InvalidOperationException e)
            {
                if (_defaultConnectionString != null)
                {

                    ConnectionString = _defaultConnectionString;
                    _isLoggingSql = _defaultIsLogging;
                }
                else
                {
                    throw (e);
                }
            }

            try
            {
                // Merra Kokebie Added to fix the following error Mar. 31st, 2008
                // A transport-level error has occurred when sending the request to the server. (provider: TCP
                // Provider, error: 0 - An existing connection was forcibly closed by the
                // remote host.)
                TestConnection();
            }
            catch
            {
                // 2008-04-17 @CZUE:  do nothing - this will fail when the first real query gets called unless the 
                // connection string is changed.  This is just to not have the constructor throwing exceptions against
                // code that was previously relying on it not to
            }

        }

        /// <summary>
        /// Constructor to allow default conncetion string but specify logging.  Used by unit tests.
        /// </summary>
        /// <param name="logSql"></param>
        //public DBConnHelper(bool logSql) : this() {
        //    _isLoggingSql = logSql;
        //}

        /// <summary>
        /// Constructs a new helper using an explicit logging setting and database connection string.
        /// </summary>
        /// <param name="logSql">Whether SQL operations should be logged</param>
        /// <param name="dbConnection">The string to use for establishing a connection to the database.</param>
        public DBConnHelper(bool logSql, String dbConnection)
        {
            _isLoggingSql = logSql;
            ConnectionString = dbConnection;

            if (_defaultConnectionString == null)
            {
                _defaultConnectionString = dbConnection;
                _defaultIsLogging = logSql;
            }
        }

        #endregion Constructors

        #region Finalizers
        ~DBConnHelper()
        {
            currentInstanceCount--;
        }
        #endregion

        #region Properties

        /// <summary>
        /// A sql transaction object to use on all execute commands.  If you set this, 
        /// the execute methods will attempt to use this transaction until it is set to null
        /// </summary>
        public SqlTransaction SqlTransaction
        {
            get { return _sqlTransaction; }
            set { _sqlTransaction = value; }
        }

        public static bool UseBritishDates
        {
            get { return _useBritishDates; }
            set { _useBritishDates = value; }
        }

        /// <summary>
        /// Timeout (in seconds) to use for executing
        /// SQL commands.
        /// </summary>
        public int CommandTimeout
        {
            get { return _cmd_timeout; }
            set { _cmd_timeout = value; }
        }

        /// <summary>
        /// Whether this helper will log SQL commands.
        /// </summary>
        public bool IsLoggingSql
        {
            get { return _isLoggingSql; }
        }

        // Gets or Sets the ConnectionString for the SQL CE connection
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                if (!_connectionString.Trim().EndsWith(";"))
                {
                    _connectionString += ";";
                }
            }
        }

        /// <summary>
        /// The source of the database.
        /// </summary>
        public string DatabaseSource
        {
            get
            {
                if (_source == null)
                {
                    SetConnectionProperties();
                }
                return _source;
            }
        }

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                if (_dbname == null)
                {
                    SetConnectionProperties();
                }
                return _dbname;
            }
        }

        /// <summary>
        /// The user name used for the database connection.
        /// </summary>
        public string DatabaseUser
        {
            get
            {
                if (_user == null)
                {
                    SetConnectionProperties();
                }
                return _user;
            }
        }

        /// <summary>
        /// The password used for the database connection.
        /// </summary>
        public string DatabasePassword
        {
            get
            {
                if (_password == null)
                {
                    SetConnectionProperties();
                }
                return _password;
            }
        }

        // Lazy loads the connection from the connection string property
        public SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return _connection;
            }
        }

        public bool ManualCloseConnection
        {
            get { return _manualConnectionClose; }
            set { _manualConnectionClose = value; }
        }

        #endregion Properties

        #region Private Methods

        private void TestConnection()
        {
            // Merra - This can fail the first time but succeed the second time, so make sure
            // the first call is done in a try/catch so an intermittent failure won't cause crash
            // later.
            object scalar = null;
            string sql = "select count(*) from Province";
            try
            {
                scalar = new SqlCommand(sql, Connection).ExecuteScalar();
            }
            catch (SqlException)
            {
                scalar = new SqlCommand(sql, Connection).ExecuteScalar();
            }
        }

        private void SetConnectionProperties()
        {
            if (_connectionString != null)
            {
                Dictionary<string, string> connectionParams = GetConnectionParams(_connectionString);
                foreach (string key in connectionParams.Keys)
                {
                    if ("data source".Equals(key.ToLower().Trim()))
                    {
                        _source = connectionParams[key];
                    }
                    else if ("initial catalog".Equals(key.ToLower().Trim()))
                    {
                        _dbname = connectionParams[key];
                    }
                    else if ("user id".Equals(key.ToLower().Trim()))
                    {
                        _user = connectionParams[key];
                    }
                    else if ("password".Equals(key.ToLower().Trim()))
                    {
                        _password = connectionParams[key];
                    }                   
                }
            }
        }


        // Closes the current connection, if open
        public void CloseConnection()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection = null;
            }
        }


        private void LogSql(string sql)
        {
            if (_isLoggingSql)
            {                
#if DEBUG
                Console.WriteLine("[db] " + sql);
#endif
            }
        }

        #endregion Private Methods

        #region Public Methods

        // Get a scalar value from the db by executing the specified sql
        public object GetScalar(string sql)
        {
            object scalar = null;
            try
            {
                LogSql(sql);
                scalar = new SqlCommand(sql, Connection).ExecuteScalar();
                if (!ManualCloseConnection)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                //Logging.Log(exc);
                throw (new Exception("The scalar value could not be retreived", exc));
            }
            return scalar;
        }

        /// <summary>
        ///   Get a scalar value from the database by executing the given SqlCommand.
        /// </summary>
        /// <param name="command">The command to use.</param>
        /// <returns>The scalar value requested, or null if there is a problem.</returns>
        public object GetScalar(SqlCommand command)
        {
            object scalar = null;
            try
            {
                LogSql(command.CommandText);
                command.Connection = Connection;
                scalar = command.ExecuteScalar();
                if (!ManualCloseConnection)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                //Logging.Log(exc);
                throw (new Exception("The scalar value could not be retrieved", exc));
            }
            return scalar;
        }


        /// <summary>
        /// GetScalar method that allows passing in a default and returns that if nothing comes back
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object GetScalar(SqlCommand sc, object defaultValue)
        {
            DataSet ret = GetDataSet(sc);
            if (ret.Tables[0].Rows.Count == 1)
            {
                return ret.Tables[0].Rows[0][0];
            }
            return defaultValue;

        }

        /// <summary>
        /// Gets a setting.  Allowed data types are "int" and "string".  
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="dataType"></param>
        public object GetSetting(string setting, string dataType)
        {
            if ("string".Equals(dataType))
            {
                return GetSettingString(setting);
            }
            //else if ("int".Equals(dataType))
            //{
            //    return GetSettingInt(setting);
            //}
            else if ("bool".Equals(dataType))
            {
                string toRet = GetSettingString(setting);
                if (string.IsNullOrEmpty(toRet))
                {
                    // 2008-03-24 @CZUE:  assume empty strings equivalent as false
                    return false;
                }
                bool toRetBool;
                if (bool.TryParse(toRet, out toRetBool))
                {
                    return toRetBool;
                }
                else
                {
                    throw new ApplicationException("Unparseable boolean value: " + toRet);
                }

            }

            else
            {
                throw new ApplicationException("Unknown data type: " + dataType);
            }
        }

        public string GetSettingString(string setting)
        {
            //string sql = string.Format(
            //    "SELECT [Value] FROM Setting WHERE [Name] = '{0}'",
            //    StringUtility.FixDbString(setting));

            //try
            //{
            //    return GetScalar(sql) as string;
            //}
            //catch
            //{                

            //    return "";
            //}
            return "";
        }

        //public int GetSettingInt(string setting)
        //{
        //    string sql = string.Format(
        //        "SELECT [Value] FROM Setting WHERE [Name] = '{0}'",
        //        StringUtility.FixDbString(setting));

        //    try
        //    {
        //        // INF: Modified by Anton Delsink: you were reading a string value and then casting it. A string can't cast to an int. You have to int.Parse or Converto.ToXXX as below.
        //        return Convert.ToInt32(GetScalar(sql));
        //    }
        //    catch
        //    {                

        //        return -1;
        //    }
        //}

        /// <summary>
        /// Sets a setting.  Allowed data types are "int" and "string".  It is the caller's
        /// responsibility to ensure the val is the correct internal type or this method will fail
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="val"></param>
        /// <param name="dataType"></param>
        public void SetSetting(string setting, object val, string dataType)
        {
            if ("string".Equals(dataType))
            {
                SetSettingString(setting, (string)val);
            }
            else if ("int".Equals(dataType))
            {
                SetSettingInt(setting, (int)val);
            }
            else if ("bool".Equals(dataType))
            {
                SetSettingString(setting, val.ToString());
            }
            else
            {
                throw new ApplicationException("Unknown data type: " + dataType);
            }
        }

        public void SetSettingString(string setting, string val)
        {
            //string sql = string.Format(
            //    "UPDATE Setting SET [Value] = '{0}' WHERE [Name] = '{1}'",
            //    StringUtility.FixDbString(val),
            //    StringUtility.FixDbString(setting));

            //try
            //{
            //    Execute(sql);
            //}
            //catch
            //{                
            //}
        }

        public void InsertSetting(string setting, string val, string type, string desc)
        {
            string sql = "INSERT INTO Setting VALUES (" + SqlTable.Normalize(setting) + ", " + SqlTable.Normalize(val) +
                         ", " + SqlTable.Normalize(type) + ", " + SqlTable.Normalize(desc) + ")";
            SqlCommand sc = new SqlCommand(sql);
            //sc.Parameters.AddWithValue("setting", setting);
            //sc.Parameters.AddWithValue("val", val);
            //sc.Parameters.AddWithValue("type", type);
            //sc.Parameters.AddWithValue("desc", desc);

            try
            {
                Execute(sql);
            }
            catch
            {                
            }
        }

        public void InsertOrUpdateSetting(string setting, string val, string type, string desc)
        {
            if (SettingExists(setting))
            {
                SetSettingString(setting, val);
            }
            else
            {
                InsertSetting(setting, val, type, desc);
            }
        }

        public bool SettingExists(string setting)
        {
            string sql = "SELECT count(*) FROM Setting WHERE [Name] = @Name";
            SqlCommand sc = new SqlCommand(sql);
            sc.Parameters.AddWithValue("Name", setting);
            return (int)GetScalar(sc) != 0;
        }

        public void SetSettingInt(string setting, int val)
        {
            //string sql = string.Format(
            //    "UPDATE Setting SET [Value] = {0} WHERE [Name] = '{1}'", val,
            //    StringUtility.FixDbString(setting));

            //try
            //{
            //    Execute(sql);
            //}
            //catch
            //{                
            //}
        }

        /// <summary>
        ///  Get a dataset from the db by executing the specified sql
        ///  NOTE from JJ: most sql queries should NEVER be strings,
        ///  any time we are adding a parameter to a query it should
        ///  be parameterized to avoid issues with ' and "
        ///  
        ///  Perhaps the reason this was done is to have more clarity
        ///  in the logs to the transaction file.  If so, we should
        ///  just print out the parameters.
        /// </summary>ll
        /// <param name="sql"></param>
        /// <returns></returns>
        //public DataSet GetDataSet(string sql)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        LogSql(sql);
        //        // Fill the dataset by calling a temp data adapter's fill method (automatically closes connection)
        //        new SqlDataAdapter(sql, Connection).Fill(ds);
        //        if (!ManualCloseConnection)
        //        {
        //            CloseConnection();
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        Logging.Log(exc);
        //        throw (new Exception("The dataset could not be filled [query] " + sql + " [/query]", exc));
        //    }
        //    return ds;
        //}

        public DataSet GetDataSet(string sql)
        {
            //DataSet ds = new DataSet();
            DataSet ds;
            try
            {
                //LogSql(sql);
                // Fill the dataset by calling a temp data adapter's fill method (automatically closes connection)
                //new SqlDataAdapter(sql, Connection).Fill(ds);
                //if (!ManualCloseConnection)
                //{
                //    CloseConnection();
                //}
                SqlCommand toExecute = new SqlCommand(sql);
                toExecute.CommandTimeout = 4000;

                ds = GetDataSet(toExecute);
            }
            catch (Exception exc)
            {
                //Logging.Log(exc);
                throw (new Exception("The dataset could not be filled [query] " + sql + " [/query]", exc));
            }
            return ds;
        }


        /// <summary>
        /// Get a dataset by using a SqlCommand
        /// </summary>
        /// <param name="cmd">the command to use</param>
        /// <returns></returns>
        public DataSet GetDataSet(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            try
            {
                LogSql(cmd.ToString());
                cmd.CommandTimeout = 0;
                cmd.Connection = Connection;
                // Fill the dataset by calling a temp data adapter's fill method (automatically closes connection)
                new SqlDataAdapter(cmd).Fill(ds);
                if (!ManualCloseConnection)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message + "\n"
                             + cmd.CommandText + "\n"
                             + CreatePrettyParamterValuesDisplayString(cmd);               
                throw (new Exception("The sql could not be executed: " + msg, exc));
            }
            return ds;
        }


        // Executes the specified sql and returns the number of rows affected (any result will be discarded)
        public int Execute(string sql, int timeout)
        {
            int rows = -1;
            try
            {
                //2007-01-24 @JJ: We made the conversion format of the DateTime object to a string before
                //it gets here safe by using yyyy-mm-dd.  WE HAVE NOT CHECKED THAT THIS IS SAFE IN ALL LOCALES
                //BUT IT IS SAFE FOR THESE TWO.
                //if ((CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "dd/mm/yyyy" |
                //       CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "d/m/yyyy"))
                //{
                //    sql = "set dateformat ymd;" + sql;
                //}
                LogSql(sql);
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandTimeout = timeout;

                //2007-04-02 @JJ: Added to allow for transactions to be used.
                if (_sqlTransaction != null)
                {
                    cmd.Transaction = _sqlTransaction;
                }

                rows = cmd.ExecuteNonQuery();

                if (!ManualCloseConnection && _sqlTransaction == null)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                //Logging.Log(exc);
                throw (new Exception("The sql could not be executed: " + sql, exc));
            }
            return rows;
        }

        // Executes the specified sql and returns the number of rows affected (any result will be discarded)

        public int Execute(string sql)
        {
            int rows = -1;
            try
            {
                //if ( ( CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "dd/mm/yyyy" |
                //       CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "d/m/yyyy" ) )
                //{
                //    sql = "set dateformat ymd;" + sql;
                //}
                LogSql(sql);
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandTimeout = 10000;

                //2007-04-02 @JJ: added to allow for transactions in the helper
                if (_sqlTransaction != null)
                {
                    cmd.Transaction = _sqlTransaction;
                }

                rows = cmd.ExecuteNonQuery();

                if (!ManualCloseConnection && _sqlTransaction == null)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                //Logging.Log(exc);
                throw (new Exception("The sql could not be executed: " + sql, exc));
            }
            return rows;
        }

        // Executes the specified sql after adding single blob parameter into command
        public int ExecuteBlob(string sql, string paramName, byte[] bytes)
        {
            SqlCommand blob = null;
            try
            {
                LogSql(sql);
                blob = new SqlCommand(sql, Connection);
                blob.CommandTimeout = 4000;

                blob.Parameters.Add("@" + paramName, SqlDbType.Image, bytes.Length).Value = bytes;

                //if ( ( CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "dd/mm/yyyy" |
                //       CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "d/m/yyyy" ) )
                //{
                //    blob.CommandText = "set dateformat dmy;" + blob.CommandText;
                //}

                if (_sqlTransaction != null)
                {
                    blob.Transaction = _sqlTransaction;
                }

                blob.ExecuteNonQuery();

                if (!ManualCloseConnection && _sqlTransaction == null)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                //Logging.Log(exc);
                throw (new Exception("The sql could not be executed: " + sql, exc));
            }

            return 0;


            // Dim nwindConn As SqlConnection = New SqlConnection("Data Source=localhost; Integrated Security=SSPI; Initial Catalog=Northwind;")

            // Dim addEmp As SqlCommand = New SqlCommand("INSERT INTO Employees (LastName, FirstName, Title, HireDate, ReportsTo, Photo) " & _
            // 			"Values(@LastName, @FirstName, @Title, @HireDate, @ReportsTo, @Photo)", nwindConn)

            // addEmp.Parameters.Add("@LastName", SqlDbType.NVarChar, 20).Value = lastName
            // addEmp.Parameters.Add("@FirstName", SqlDbType.NVarChar, 10).Value = firstName
            // addEmp.Parameters.Add("@Title", SqlDbType.NVarChar, 30).Value = title
            // addEmp.Parameters.Add("@HireDate", SqlDbType.DateTime).Value = hireDate
            // addEmp.Parameters.Add("@ReportsTo", SqlDbType.Int).Value = reportsTo

            // addEmp.Parameters.Add("@Photo", SqlDbType.Image, photo.Length).Value = photo
            // nwindConn.Open()
            // addEmp.ExecuteNonQuery()
            // nwindConn.Close()
        }

        public int Execute(SqlCommand cmd)
        {
            int rows = -1;
            try
            {
                //if ( ( CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "dd/mm/yyyy" |
                //       CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower() == "d/m/yyyy" ) )
                //{
                //    cmd.CommandText = "set dateformat dmy;" + cmd.CommandText;
                //}

                LogSql(cmd.CommandText);
                //2007-03-26 @CZUE:  added for debugging
                //string sqlString = SqlHelper.GetSqlStringFromCommand(cmd);
                cmd.CommandTimeout = 4000;
                cmd.Connection = Connection;

                //2007-04-02 @JJ: added to allow for transactions
                if (_sqlTransaction != null)
                {
                    cmd.Transaction = _sqlTransaction;
                }

                rows = cmd.ExecuteNonQuery();
                if (!ManualCloseConnection && _sqlTransaction == null)
                {
                    CloseConnection();
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message + "\n"
                             + cmd.CommandText + "\n"
                             + CreatePrettyParamterValuesDisplayString(cmd);
                //Logging.Log(msg);
                //Logging.Log(exc);
                throw (new Exception("The sql could not be executed: " + msg, exc));
            }
            return rows;
        }

        /// <summary>
        /// 28 August 2006 @PS: Added this function override to allow for executing
        /// param stored proc sql without setting datetime format (see Execute func above)
        /// Prepending date format text to the sql cmd was causing the sql to fail
        /// 
        /// 2007-01-24 @JJ:  Date format override was remove from Execute
        /// </summary>
        /// <param name="cmd">Sql Command object</param>
        /// <returns>Number of rows affected by the command</returns>
        //public int ExecuteWithNoDateFormatOverride( SqlCommand cmd )
        //{
        //    int rows = -1;
        //    try
        //    {
        //        LogSql( cmd.CommandText );
        //        cmd.Connection = Connection;
        //        rows = cmd.ExecuteNonQuery();
        //        if ( !ManualCloseConnection )
        //        {
        //            CloseConnection();
        //        }
        //    }
        //    catch ( Exception exc )
        //    {
        //        string msg = exc.Message + "\n"
        //                     + cmd.CommandText + "\n"
        //                     + CreatePrettyParamterValuesDisplayString(cmd);
        //        Logger.Log(msg);
        //        throw ( new Exception( "The sql could not be executed: " + msg, exc ) );
        //    }

        //    return rows;
        //}

        /// <summary>
        /// Generates a database connection string of the format...
        /// "Data Source=[DataSource];Initial Catalog=[Catalog];User ID=[User];Password=[Password]"
        /// </summary>
        /// <param name="DataSource">source of database</param>
        /// <param name="Catalog">initial catalog to access</param>
        /// <param name="User">authenticated database user</param>
        /// <param name="Password">user passwordparam>
        /// <returns>a formatted connection string</returns>
        public static String FormatConnectionString(String DataSource, String Catalog, String User, String Password)
        {
            return "Data Source=" + DataSource + ";" +
                   "Initial Catalog=" + Catalog + ";" +
                   "User ID=" + User + ";" +
                   "Password=" + Password;
        }

        /// <summary>
        /// Convenience method to extract the values from the current database connection string.
        /// </summary>
        /// <returns>a Dictionary of the key, value pairs extracted from the connection string</returns>
        public static Dictionary<string, string> GetConnectionParams(string connectionString)
        {
            char[] sectionSplit = new char[] { ';' };
            char[] keyValueSplit = new char[] { '=' };

            Dictionary<string, string> vals = new Dictionary<string, string>();
            String[] Sections = connectionString.Split(sectionSplit);
            foreach (String section in Sections)
            {
                String[] keyValue = section.Split(keyValueSplit);
                if (keyValue.Length == 2)
                {
                    vals[keyValue[0]] = keyValue[1];
                }             
            }
            return vals;
        }

        /// <summary>
        /// Replaces the "Initial Catalog" value in a connection string.
        /// </summary>
        /// <param name="connectionString">the original connection string</param>
        /// <param name="newName">new name for the initial catalog</param>
        /// <returns>revised version of the connection string</returns>
        public static String ReplaceDatabaseName(String connectionString, String newName)
        {
            string replacedConnection = "";
            char[] sectionSplit = new char[] { ';' };
            char[] keyValueSplit = new char[] { '=' };

            String[] Sections = connectionString.Split(sectionSplit);

            foreach (String Section in Sections)
            {
                String[] KeyValue = Section.Split(keyValueSplit);
                String key = KeyValue[0].Trim();
                String val = KeyValue[1].Trim();
                replacedConnection += key + "=";
                if (key.Equals("Initial Catalog"))
                {
                    replacedConnection += newName;
                }
                else
                {
                    replacedConnection += val;
                }
                replacedConnection += ";";
            }
            replacedConnection = replacedConnection.TrimEnd(new char[] { ';' }); // ABK: ugly
            return replacedConnection;
        }

        /// <summary>
        /// Accepts a dateTime object and produces an ODBC canonical datetime string useful in 
        /// sql queries - Sql Server parses the properly formatted string into a sql datetime
        /// </summary>
        /// <remarks>
        /// The ODBC format looks like this:
        /// { ts '1998-05-02 01:23:56.123' }
        /// { d '1990-10-02' }
        /// { t '13:33:41' }
        /// </remarks>
        /// <param name="dt">DateTime object to be used in creating a sql datetime string</param>
        /// <returns>A string representation of a sql datetime. Ex: '2000-10-23 16:47:58.291'</returns>
        public static string MakeOdbcCanonicalDateTimeString(DateTime dt)
        {
            string odbcStrFmt = "{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}";

            string dtStr = string.Empty;

            dtStr += string.Format(
                odbcStrFmt,
                dt.Year,
                dt.Month,
                dt.Day,
                dt.Hour,
                dt.Minute,
                dt.Second,
                dt.Millisecond);

            return "{ ts '" + dtStr + "' }";
        }

        // 08 October 2006 @PS
        // 09 October 2006 @PS: changed for more direct and scalar sysfiles table info
        /// <summary>
        /// Gets the size of the current database in megabytes
        /// </summary>
        /// <returns>decimal number representing the size of the database in megabytes</returns>
        public long GetCurrentDatabaseSizeKiloBytes()
        {
            long dbSzMbytes = -1;

            try
            {
                // the following query gets the size of the primary data file (*.mdf)
                // command returns number of pages - multiplying by 8 gets the db size in kbytes
                string sqlCmd = "select size * 8 from sysfiles " +
                                "where fileid = 1 and groupid = 1";

                dbSzMbytes = (int)GetScalar(sqlCmd);
            }
            catch
            {
                dbSzMbytes = -1;
            }

            return dbSzMbytes;
        }

        /// <summary>
        /// Allows simple way to get a string value from a DataRow object as well handle a dbNull value that may occur
        /// </summary>
        /// <param name="dr">The DataRow object from which a value is to be fetched</param>
        /// <param name="columnName">The name of the field to fetch</param>
        /// <param name="nullReplacement">The value to return instead in case the value is a db null</param>
        /// <returns></returns>
        public static string GetNullableStringFromRow(DataRow dr, string columnName, string nullReplacement)
        {
            object obj = dr[columnName];

            if (obj == null || obj == System.DBNull.Value)
            {
                return nullReplacement;
            }
            else if (obj.GetType() == typeof(string))
            {
                return (string)obj;
            }
            else
            {
                throw new Exception("Column " + columnName + " is not a string type!");
                //throw new DynamicReportingEngine.Exceptions.DataTypeMismatchException( "Column " + columnName + " is not a string type!" );
                throw new Exception();//For Building Purpose only:- Girum
            }
        }

        /// <summary>
        /// Allows simple way to get a bool value from a DataRow object as well handle a dbNull value that may occur
        /// </summary>
        /// <param name="dr">The DataRow object from which a value is to be fetched</param>
        /// <param name="columnName">The name of the field to fetch</param>
        /// <param name="nullReplacement">The bool value to return instead in case the field value is a db null</param>
        /// <returns></returns>
        public static bool GetNullableBoolFromRow(DataRow dr, string columnName, bool nullReplacement)
        {
            object obj = dr[columnName];

            if (obj == null || obj == System.DBNull.Value)
            {
                return nullReplacement;
            }
            else if (obj.GetType() == typeof(bool))
            {
                return (bool)obj;
            }
            else
            {
                throw new Exception("Column " + columnName + " is not a bool type!");
                //throw new DynamicReportingEngine.Exceptions.DataTypeMismatchException( "Column " + columnName + " is not a bool type!" );
                throw new Exception();//For Building Purpose only
            }
        }

        /// <summary>
        /// Allows simple way to get a int value from a DataRow object as well handle a dbNull value that may occur
        /// </summary>
        /// <param name="dr">The DataRow object from which a value is to be fetched</param>
        /// <param name="columnName">The name of the field to fetch (Handles Int16's as well)</param>
        /// <param name="nullReplacement">The int value to return instead in case the field value is a db null</param>
        /// <returns></returns>
        public static int GetNullableIntFromRow(DataRow dr, string columnName, int nullReplacement)
        {
            object obj = dr[columnName];

            if (obj == null || obj == System.DBNull.Value)
            {
                return nullReplacement;
            }
            else if (obj.GetType() == typeof(int) || obj.GetType() == typeof(Int16))
            {
                return (int)obj;
            }
            else
            {
                throw new Exception("Column " + columnName + " is not a int type!");
                // throw new DynamicReportingEngine.Exceptions.DataTypeMismatchException( "Column " + columnName + " is not a int type!" );
                throw new Exception();//just for making it build
            }
        }

        /// <summary>
        /// Allows simple way to get a decimal value from a DataRow object as well handle a dbNull value that may occur
        /// </summary>
        /// <param name="dr">The DataRow object from which a value is to be fetched</param>
        /// <param name="columnName">The name of the field to fetch (Handles Int16's as well)</param>
        /// <param name="nullReplacement">The decimal value to return instead in case the field value is a db null</param>
        /// <returns></returns>
        public static decimal GetNullableDecFromRow(DataRow dr, string columnName, decimal nullReplacement)
        {
            object obj = dr[columnName];

            if (obj == null || obj == System.DBNull.Value)
            {
                return nullReplacement;
            }
            else if (obj.GetType() == typeof(decimal))
            {
                return (decimal)obj;
            }
            else
            {
                throw new Exception("Column " + columnName + " is not a decimal type!");
                //throw new DynamicReportingEngine.Exceptions.DataTypeMismatchException( "Column " + columnName + " is not a decimal type!" );
                throw new Exception();//For Building purpose only: Girum
            }
        }


        public static string CreatePrettyParamterValuesDisplayString(SqlCommand cmd)
        {
            string returnString = "";
            foreach (SqlParameter parameter in cmd.Parameters)
            {
                returnString += parameter.ParameterName + ": " + parameter.Value + "\n";
            }
            return returnString;
        }

        public static string GetSqlStringFromCommand(SqlCommand cmd)
        {
            string cmdStr = cmd.CommandText;
            foreach (SqlParameter param in cmd.Parameters)
            {
                string name = param.ParameterName;
                object value = param.Value;
                string replStr = SqlTable.Normalize(value);
                int index = cmdStr.IndexOf(name);
                cmdStr = cmdStr.Substring(0, index) + replStr + cmdStr.Substring(index + name.Length);
            }
            return cmdStr;
        }

        /// <summary>
        /// Gets a new open transaction for this connection.  This is a safer method than SqlConnection.BeginTransaction
        /// as it will work when a connection temporarily goes down and comes back up
        /// </summary>
        /// <returns></returns>
        public SqlTransaction GetOpenTransaction()
        {
            // Merra Kokebie Added to fix the following error Mar. 31st, 2008
            // A transport-level error has occurred when sending the request to the server. (provider: TCP
            // Provider, error: 0 - An existing connection was forcibly closed by the
            // remote host.)  Calling it twice resolves it.  This happenes when a server is restarted.
            SqlTransaction transaction;
            try
            {
                transaction = Connection.BeginTransaction();
            }
            catch (SqlException)
            {
                transaction = Connection.BeginTransaction();
            }
            return transaction;
        }

        #endregion Public Methods


        #region IDisposable Members

        public void Dispose()
        {

            if (!ManualCloseConnection && _sqlTransaction == null)
            {
                CloseConnection();
            }
        }

        #endregion
    }
}
