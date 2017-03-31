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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlManagement.Database
{
	public abstract class SqlTable 
	{

		#region Member Variables

		private String _tableName;
		private String _IDColumnName;
		private String[] _columnNames;
		private String[] _columnTypes;

		protected DBConnHelper _helper;//  interface to the data access layer

		#endregion Member Variables

		#region Properties

		public String TableName {
			get {
				return _tableName;
			}
			set {
				_tableName = value;
			}
		}
		public String IDColumnName {
			get {
				return _IDColumnName;
			}
			set {
				_IDColumnName = value;
			}
		}

		public String[] ColumnNames {
			get {
				return _columnNames;
			}
			set {
				_columnNames = value;
			}
		}

		public String[] ColumnTypes 
		{
			get 
			{
				return _columnTypes;
			}
			set 
			{
				_columnTypes = value;
			}
		}

		#endregion Properties

		#region Constructors

		public SqlTable() {
			_tableName = "(unknown)";
			_IDColumnName = "ID";
			_columnNames = new String[0];
			_columnTypes = new String[0];
		}

		public SqlTable(String tableName, String IDColumnName, String[] columnNames, String[] columnTypes) {
			_tableName = tableName;
			_IDColumnName = IDColumnName;
			_columnNames = columnNames;
			_columnTypes = columnTypes;
		}

		#endregion Constructors

		#region TABLE management


		/// <summary>
		/// An inelegant test for the existence of a database table. 
        /// This is no longer quite as inelegant
		/// </summary>
		/// <returns>true if the table exists, false otherwise</returns>
        public bool TableExists()
		{
		    //2007-08-15 @JJ: switching this to match code for column exists
		    //string sql = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME = '" +
		    //             TableName + "'";
		    //try 
		    //{
		    //    int cnt = (int)_helper.GetScalar(sql);
		    //    if (cnt > 0) {
		    //        return true;
		    //    }
		    //    return false;
		    //} 
		    //catch
		    //{
		    //    return false;
		    //}

		    string sql = "SELECT Case WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
		                 "WHERE table_name = '" + TableName + "') Then 1 " +
		                 "ELSE 0 " +
		                 "END ";

		    int cnt = (int) _helper.GetScalar( sql );
		    if ( cnt > 0 )
		    {
		        return true;
		    }
		    return false;
		}

	    //2007-08-15 @JJ: this could be move so that you could also pass in the table name
        /// <summary>
        /// A method to determine if a column exists in a particular table
        /// </summary>
        /// <param name="columnName">the name of the column</param>
        /// <returns>true if it does, false otherwise</returns>
        public bool ColumnExists(string columnName)
        {
            string sql = "SELECT Case WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS " +
                         "WHERE table_name = '" + TableName + "' " +
                         "AND column_name = '" + columnName + "') THEN 1 " +
                         "ELSE 0 " +
                         "END";

            int cnt = (int)_helper.GetScalar(sql);
            if (cnt > 0)
            {
                return true;
            }
            return false;
        }

	    public string ChangeBlankIntStringtoNull(string intString)
        {
            if (intString == "")
            {
                return null;
            }
            else
            {
                return intString;
            }
	}

		/// <summary>
		/// Creates the database table as defined by the Properties TableName,
		/// ColumnNames, and ColumnTypes.
		/// </summary>
		public virtual void CreateTable()
		{
			_helper.Execute(CreateTableCreationCall());
		}

		/// <summary>
		/// Drops the associated database table.
		/// </summary>
		public void DropTable()
		{
			_helper.Execute(CreateTableDropCall());
        }


        #endregion Table management


        #region Data Management 


        /// <summary>
        /// Gets a full list of all IDs.
        /// </summary>
        /// <returns>a list of IDs, as strings</returns>
        virtual public ArrayList GetAllIDs()
        {
            ArrayList ids = new ArrayList();
            // ABK NOTE: This is generic, so don't presume to know the table or ID column name
            DataSet ds = _helper.GetDataSet(CreateSelectAllIdsCall());
            ids.AddRange(GetIdsForDataSet(ds));
            return ids;
        }


	    /// <summary>
        /// Gets a list of all unique ids
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetAllUniqueIDs()
        {
	        DataSet ds = _helper.GetDataSet(CreateSelectDistinctIdsCall());
	        return GetIdsForDataSet(ds);
        }


	    /// <summary>
        /// Gets a list of IDs that have not been deprecated
        /// </summary>
        /// <returns></returns>
        virtual public List<string> GetAllNonDeprecatedIDs()
        {
            DataSet ds = _helper.GetDataSet(CreateSelectAllNonDeprecatedIdsCall());
            return GetIdsForDataSet(ds);
        }


        protected List<string> GetIdsForDataSet(DataSet ds)
        {
            List<string> ids = new List<string>();
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                if (!(dr.IsNull(IDColumnName)))
                {
                    ids.Add((string)(dr[IDColumnName]));
                }
            }
            return ids;
        }


	    #endregion

        #region SQL generation methods

        /// <summary>
		/// Create the SQL needed to create a table based upon the 
		/// Properties TableName, ColumnNames, and ColumnTypes.
		/// </summary>
		/// <returns></returns>
		public string CreateTableCreationCall()
		{
			String retVal = "CREATE TABLE " + TableName + "( ";
			for (int i=0; i<ColumnNames.Length; i++)
			{
				retVal += "[" + ColumnNames[i] + "] " + ColumnTypes[i];
				if ((i+1)<ColumnNames.Length) retVal += ", ";
			}
			retVal += ")";

			return retVal;
		}

		/// <summary>
		/// Creates the SQL call to drop the table named TableName.
		/// </summary>
		/// <returns>the SQL to drop the table</returns>
		public String CreateTableDropCall()
		{
			String retVal = "DROP TABLE " + TableName;

			return retVal;
		}

		/// <summary>
		///   Create a SQL string to return the largest existing ID in the table.
		/// </summary>
		/// <returns></returns>
		public string CreateMaxIDCall() {

			String retVal = "SELECT Max(" + _IDColumnName + ") from " + _tableName;

			return retVal;
	
		}

		/// <summary>
		///   Create a SQL string to determine if the row whose ID is IDValue exists.
		/// </summary>
		/// <param name="IDValue"></param>
		/// <returns></returns>
		public string CreateExistsCall(String IDValue) {

			String retVal = "SELECT COUNT(*) from " + _tableName;
			retVal += " where " + _IDColumnName + " = " + Normalize(IDValue);

			return retVal;
	
		}

        public string CreateExistsCall(String column, String columnValue, String tableName)
        {

            String retVal = "SELECT COUNT(*) from " + tableName;
            retVal += " where " + column + " = " + Normalize(columnValue);

            return retVal;

        }

		/// <summary>
		///   Create a SQL string to select the row whose ID is IDValue.
		/// </summary>
		/// <param name="IDValue"></param>
		/// <returns></returns>
		public string CreateSelectCall(object IDValue) {

			String retVal = "SELECT ";
			for (int i = 0; i < _columnNames.Length; i++ ) {
				if (i != 0) retVal += ", ";
				retVal += _columnNames[i];
			}
			retVal += " FROM " + _tableName;
			//TODO: CHECK ON THIS AND THE IMPLICATIONS OF ROWSTATUS = NULL
			retVal += " WHERE " + _IDColumnName + " = " + Normalize(IDValue);

			return retVal;
		}

		/// <summary>
		///   Create a SQL string to select the rows where each specified
		///   column has the corresponding value.
		/// </summary>
		/// <param name="columns">The columns to constrain on.</param>
		/// <param name="values">The desired constraint values.</param>
		/// <returns></returns>
		public string CreateSelectCall(String [] columns, Object[] values) {

			String retVal = "SELECT ";
			for (int i = 0; i < _columnNames.Length; i++ ) {
				if (i != 0) retVal += ", ";
				retVal += _columnNames[i];
			}
			retVal += " from " + _tableName;
			retVal += " where ";
			
			for (int i = 0; i < columns.Length; i++ ) {
				if (i != 0) retVal += " and ";
				retVal += columns[i] + " = " + Normalize(values[i]);
			}

			return retVal;
		}

		/// <summary>
		///   Create a SQL string to select all the rows in the table.
		/// </summary>
		/// <returns></returns>
		public string CreateSelectAllCall() {

			String retVal = "SELECT ";
			for (int i = 0; i < _columnNames.Length; i++ ) {
				if (i != 0) retVal += ", ";
				retVal += _columnNames[i];
			}
			retVal += " from " + _tableName;

			return retVal;
		}

		/// <summary>
		/// Create a SQL string to select all the ID column data
		/// from the table.
		/// </summary>
		/// <returns></returns>
		public string CreateSelectAllIdsCall()
		{
			String retVal = "SELECT " +  _IDColumnName + " from " +
				_tableName + " ORDER BY " + _IDColumnName;

			return retVal;
		}


        /// <summary>
        /// Create a SQL string to select all the ID column data
        /// from the table ignoring deprecated rows.
        /// </summary>
        /// <returns></returns>
        public string CreateSelectAllNonDeprecatedIdsCall()
        {
            // 2008-03-25 @CZUE:  change this check from = 0 to also check nulls
            String retVal = "SELECT " + _IDColumnName + " from " +
                _tableName + " WHERE RowStatus = 0 OR RowStatus IS NULL ORDER BY " + _IDColumnName;

            return retVal;
        }

        /// <summary>
        /// Select all id's but make the list unique
        /// </summary>
        /// <returns></returns>
        private string CreateSelectDistinctIdsCall()
        {
            String retVal = "SELECT DISTINCT " + _IDColumnName + " FROM " +
                _tableName + " ORDER BY " + _IDColumnName;

            return retVal;
        }

		/// <summary>
		///   Create a SQL string to insert a new row into the table.
		///   Values must be in the same order as the column names.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public string CreateInsertCall(Object[] values) 
		{
			String retVal = "INSERT into " + _tableName + " (";
			for (int i = 0; i < _columnNames.Length; i++ ) 
			{
				if (i != 0) retVal += ", ";
				retVal += _columnNames[i];
			}

			retVal += ") VALUES (";
			
			for (int i = 0; i < values.Length; i++ ) 
			{
				if (i != 0) retVal += ", ";
				retVal += Normalize(values[i]);
			}

			retVal += ")";

			return retVal;
		}
		
		/// <summary>
		/// Creates a SQL command to insert a new row into the table.  It does this in a stupid manner,
		/// by creating a concatenated string first and then creating a sql command out of it.  
		/// this <method exists to be overridden in the event that more information is known about
		/// the parameter types and we don't want to defaul to string values.
		/// </summary>
		/// <returns></returns>
		public virtual SqlCommand CreateInsertSqlCommand(Object[] values)
		{
			String retVal = "INSERT into " + _tableName + " (";
			for (int i = 0; i < _columnNames.Length; i++ ) 
			{
				if (i != 0) retVal += ", ";
				retVal += _columnNames[i];
			}

			retVal += ") VALUES (";
			
			for (int i = 0; i < values.Length; i++ ) 
			{
				if (i != 0) retVal += ", ";
				retVal += Normalize(values[i]);
			}

			retVal += ")";

			SqlCommand cmd = new SqlCommand( retVal);
			
			return cmd;
		}

		/// <summary>
		/// Paul Sasik
		///   Create a SQL string to insert a new row into the table.
		///   Values must be in the same order as the column names.
		///   THIS version skips the required LoginId which is not appropriate to set
		///   on insert
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public string CreateSkipAutoincrementInsertCall(Object[] values) 
		{
			String retVal = "INSERT into " + _tableName + " (";
			for (int i = 1; i < _columnNames.Length; i++ )  
			{
				if (i != 1) retVal += ", ";
				retVal += _columnNames[i];
			}

			retVal += ") VALUES (";
			
			for (int i = 0; i < values.Length; i++ ) 
			{
				if (i != 0) retVal += ", ";
				retVal += Normalize(values[i]);
			}

			retVal += ")";

			return retVal;
		}
	
		public string CreateUpdateCall(String IDValue, Object[] values) {

			String retVal = "UPDATE " + _tableName + " set ";
			for (int i = 0; i < _columnNames.Length; i++ ) {
				if (i != 0) retVal += ", ";
				retVal += _columnNames[i] + " = " + Normalize(values[i]);
			}

			retVal += " where " + _IDColumnName + " = " + Normalize(IDValue);

			return retVal;
		}

		/// <summary>
		/// Paul Sasik
		/// Modified version of the above to skip trying to update the identity column
		///   Create a SQL string to update the row where the ID is IDValue.
		///   Values must be in the same order as the column names.
		/// </summary>
		/// <param name="values"></param>
		/// <param name="IDValue"></param>
		/// <returns></returns>
		public string CreateOmitIdUpdateCall(String IDValue, Object[] values) 
		{
			String retVal = "UPDATE " + _tableName + " set ";
			for (int i = 1; i < _columnNames.Length; i++ ) 
			{
				if (i != 1) retVal += ", ";
				retVal += _columnNames[i] + " = " + Normalize(values[i]);
			}

			retVal += " where " + _IDColumnName + " = " + Normalize(IDValue);

			return retVal;
		}

		/// <summary>
		/// Overloaded form that allows a column to be skipped and the original value retained during an update call.
		/// </summary>
		/// <param name="IDValue">id or primary key identifier</param>
		/// <param name="values">array of values with which to update a table row</param>
		/// <param name="skipColName">name of column to not update</param>
		/// <returns></returns>
		public string CreateOmitIdUpdateCall(String IDValue, Object[] values, string skipColName) 
		{
			String retVal = "UPDATE " + _tableName + " set ";

			for (int i = 1; i < _columnNames.Length; i++ ) 
			{
				if (i != 1) retVal += ", ";

				if ( skipColName.ToUpper().Equals(_columnNames[i].ToUpper()) )
				{
					retVal += _columnNames[i] + " = " + _columnNames[i]; // simply assign existing values to itself
				}
				else
				{
					retVal += _columnNames[i] + " = " + Normalize(values[i]);
				}
			}

			retVal += " where " + _IDColumnName + " = " + Normalize(IDValue);

			return retVal;
		}

		/// <summary>
		///   Create a SQL string to delete the specific row from the table.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="IDValue"></param>
		/// <returns></returns>
		public string CreateDeleteCall(String IDValue) {

			String retVal = "DELETE from " + _tableName;
			retVal += " where " + _IDColumnName + " = " + Normalize(IDValue);

			return retVal;
		}

		/// <summary>
		///   Create a SQL string to delete all rows from the table
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="IDValue"></param>
		/// <returns></returns>
		public string CreateDeleteAllCall() {

			String retVal = "DELETE from " + _tableName;

			return retVal;
		}
		#endregion SQL generation methods

		#region SqlCommand method version graveyard
		/************
		 * 5/8/06 JJ: We attempted to change all sql calls to commands.
		 * however, you cannot simply insert System.DbNull.Value for all parameter
		 * types, as that is not valid for certain columns.  Therefore, in order
		 * for this type of code to work, the type of each column would need to be
		 * known.  

		/// <summary>
		///   Create a SqlCommand to delete the specified row from the table.
		/// </summary>
		/// <param name="IDValue">The ID of the row to be deleted.</param>
		/// <returns>A SqlCommand object representing the deletion command.</returns>
		[Obsolete]
		public SqlCommand CreateDeleteCommand(String IDValue) 
		{

			String queryString = "DELETE from " + _tableName;
			queryString += " where " + _IDColumnName + " = @" + _IDColumnName;
			
			SqlCommand returnCommand = new SqlCommand(queryString);
			returnCommand.Parameters.Add("@" + _IDColumnName, NormalizeObject(IDValue));

			return returnCommand;
		}

		/// <summary>
		///   Create a SqlCommand to update a row in the database without changing its ID.
		/// <param name="IDValue">The ID of the row to be updated.</param>
		/// <param name="columnNames">The list of column names to update.</param>
		/// <param name="values">The list of new values to use.</param>
		/// <returns>A SqlCommand object representing the update call.</returns>
		/// </summary>
		[Obsolete]
		public SqlCommand CreateOmitIdUpdateCommand(String IDValue, String[] columnNames, Object[] values) 
		{

			// MTS - We want to update all columns except the ID column, which is always the first one.

			String[] columnNamesWithoutID = new String[columnNames.Length - 1];
			Array.Copy(columnNames, 1, columnNamesWithoutID, 0, columnNamesWithoutID.Length);

			Object[] valuesWithoutID = new Object[values.Length - 1];
			Array.Copy(values, 1, valuesWithoutID, 0, valuesWithoutID.Length);

			SqlCommand returnCommand = CreateUpdateCommand(IDValue, columnNamesWithoutID, valuesWithoutID);

			return returnCommand;
		}

		/// <summary>
		///   Create a SqlCommand to update a new row into the table.
		/// <param name="IDValue">The ID of the row to be updated.</param>
		/// <param name="columnNames">The list of column names to update.</param>
		/// <param name="values">The list of new values to use.</param>
		/// <returns>A SqlCommand object representing the update call.</returns>
		/// </summary>
		[Obsolete]
		public SqlCommand CreateUpdateCommand(String IDValue, String[] columnNames, Object[] values) 
		{

			String queryString = "UPDATE " + _tableName + " set ";
			for (int i = 0; i < columnNames.Length; i++ ) 
			{
				if (i != 0) queryString += ", ";
				queryString += columnNames[i] + " = @" + columnNames[i];
			}

			queryString += " where " + _IDColumnName + " = @IDValue";

			SqlCommand returnCommand = new SqlCommand(queryString);
			returnCommand.Parameters.Add("@IDValue", NormalizeObject(IDValue));
			for (int i = 0; i < columnNames.Length && i < values.Length; i++) 
			{
				returnCommand.Parameters.Add("@" + columnNames[i], NormalizeObject(values[i]));
			}

			return returnCommand;
		}

		
		/// <summary>
		///   Create a SqlCommand to insert a new row into the table.
		///   How is this different than an ordinary insertion command?
		///   This version skips the required LoginId which is not appropriate to set
		///   on insert		
		/// <param name="columnNames">The list of column names to insert.</param>
		/// <param name="values">The list of values to be inserted.</param>
		/// <returns>A SqlCommand object representing the insertion call.</returns>
		/// </summary>
		[Obsolete]
		public SqlCommand CreateSkipAutoincrementInsertCommand(String[] columnNames, Object[] values) 
		{
			return CreateInsertCommand(columnNames, values);
		}

		
		/// <summary>
		///   Create a SqlCommand to select the rows where each specified
		///   column has the corresponding value.
		/// </summary>
		/// <param name="columns">The columns to use as constraints.</param>
		/// <param name="values">The desired constraint values.</param>
		/// <returns>A SqlCommand object representing the selection query.</returns>
		[Obsolete]
		public SqlCommand CreateSelectCommand(String [] columns, Object[] values) 
		{

			String queryString = "SELECT ";
			for (int i = 0; i < _columnNames.Length; i++ ) 
			{
				if (i != 0) queryString += ", ";
				queryString += _columnNames[i];
			}
			queryString += " from " + _tableName;
			queryString += " where ";
			
			for (int i = 0; i < columns.Length; i++ ) 
			{
				if (i != 0) queryString += " and ";
				queryString += columns[i] + " = @" + columns[i];
			}

			SqlCommand returnCommand = new SqlCommand(queryString);
			for (int i = 0; i < columns.Length; i++) 
			{
				returnCommand.Parameters.Add("@" + columns[i], NormalizeObject(values[i]));
			}

			return returnCommand;
		}

		/// <summary>
		///   Create a SqlCommand to insert a new row into the database table.
		///   Values must be in the same order as the column names.
		///   
		///   JJ:  5/8/06 this was never finished due to issues with NULL values
		///   created by the normalize function.  
		/// </summary>
		/// <param name="columnNames">The list of column names to insert.</param>
		/// <param name="values">The list of values to be inserted.</param>
		/// <returns>A SqlCommand object representing the insertion call.</returns>
		[Obsolete]
		public SqlCommand CreateInsertCommand(String [] columnNames, Object[] values) 
		{
			String queryString = "INSERT into " + _tableName + " (";
			for (int i = 0; i < columnNames.Length; i++ ) 
			{
				if (i != 0) queryString += ", ";
				queryString += columnNames[i];
			}

			queryString += ") VALUES (";
			
			for (int i = 0; i < columnNames.Length; i++ ) 
			{
				if (i != 0) queryString += ", ";
				queryString += "@" + columnNames[i];
			}

			queryString += ")";

			SqlCommand returnCommand = new SqlCommand(queryString);
			for (int i = 0; i < columnNames.Length && i < values.Length; i++) 
			{
				returnCommand.Parameters.Add("@" + columnNames[i], NormalizeObject(values[i]));
			}

			return returnCommand;
		}

		/// <summary>
		///   Create a SqlCommand to determine if the row whose ID is IDValue exists.
		/// </summary>
		/// <param name="IDValue">The row ID to check for existence.</param>
		/// <returns>A SqlCommand object representing the query.</returns>
		[Obsolete]
		public SqlCommand CreateExistsCommand(String IDValue) 
		{

			String queryString = "SELECT COUNT(*) from " + _tableName;
			queryString += " where " + _IDColumnName + " = @" + _IDColumnName;

			SqlCommand returnCommand = new SqlCommand(queryString);
			returnCommand.Parameters.Add("@" + _IDColumnName, NormalizeObject(IDValue));

			return returnCommand;
	
		}

		
		/// <summary>
		///   Create a SqlCommand to select the row whose ID is IDValue.
		/// </summary>
		/// <param name="IDValue">The ID of the row to select.</param>
		/// <returns>A SqlCommand object representing the selection query.</returns>
		[Obsolete]
		public SqlCommand CreateSelectCommand(String IDValue) 
		{

			String queryString = "SELECT ";
			for (int i = 0; i < _columnNames.Length; i++ ) 
			{
				if (i != 0) queryString += ", ";
				queryString += _columnNames[i];
			}
			queryString += " from " + _tableName;
			queryString += " where RowStatus is null and " + _IDColumnName + " = @" + _IDColumnName;
			
			SqlCommand returnCommand = new SqlCommand(queryString);
			returnCommand.Parameters.Add("@" + _IDColumnName, NormalizeObject(IDValue));

			return returnCommand;
		}
		*/
		#endregion

		#region SQL formatting methods
       
		public Object NormalizeObject(Object inputObject) 
		{
			Object returnValue = null;

			if ( inputObject == null || inputObject == System.DBNull.Value) {
				return System.DBNull.Value;
			} else {
				switch ( inputObject.GetType().Name ) {
					case "String":
						// escape single-quotes
						String stringObject = (String) inputObject;
						returnValue = stringObject.Replace("'", "''");
						break;
					case "Int32":
						returnValue = (Int32) inputObject;
						break;
					case "Boolean":
						returnValue = (Boolean) inputObject;
						break;
					case "DateTime":
						DateTime dateObject = (DateTime) inputObject;
						// Consider any date prior to 1900 a null date
						if (dateObject < new DateTime( 1900, 1, 1 ) ) {
							returnValue = General.Util.Constants.NULLDATE;
						} else {
							returnValue = dateObject;
						}
						break;
					default:
						// this probably shouldn't happen.
						returnValue = inputObject.ToString();
						break;
				}
			}

			return returnValue;
		}

		public static string Normalize(Object input) {
			if ( input == null || input == System.DBNull.Value) {
				// This is an empty string;not what we want.
				//return System.DBNull.Value.ToString();
				return "NULL";
			}
			else {
				switch ( input.GetType().Name ) {
					case "String":
						return GetSafeString( System.Convert.ToString( input ) );
					case "Int32":
						return GetSafeInteger( System.Convert.ToInt32( input ) );
					case "Boolean":
						return GetSafeBoolean( System.Convert.ToBoolean( input ) );
					case "DateTime":
                        // Due to issues with different configurations in SQL Server itself, we can't assume a string is convertible.
                        // This format is something SQL cannot misinterpret:
                        return DBConnHelper.MakeOdbcCanonicalDateTimeString( System.Convert.ToDateTime( input ));

                    // TODO [POA 10/25/04]: Add additional conversions as needed
					default:
						// this probably shouldn't happen.
						return input.ToString();
				}
			}
		}

		public static string GetSafeString( string input ) {
			if ( input == null ) {return "''";}
			// escape any single quotes, then wrap the text in single quotes
			return "'" + input.Replace( "'", "''" ) + "'";
		}
                
		public static string GetSafeInteger( int input ) {
			return "'" + input.ToString() + "'";
		}
                
		public static string GetSafeBoolean( bool input ) {
			return (input ? "'1'" : "'0'" );
		}
                                
		public static string GetSafeDate( DateTime input ) {
			// Consider any date prior to 1900 a null date
			if ( input < new DateTime( 1900, 1, 1 ) ) {return "NULL";}
			return "'" + input.ToString( "yyyy-MM-dd HH:mm:ss.fff" ) + "'";
		}

		#endregion SQL formatting methods

		#region Universal Methods

        /// <summary>
        /// Deprecates rows in this table matching the ID.  
        /// </summary>
        /// <param name="interactionID">the id representing the data to deprecate</param>
        /// <param name="idColumnName">the column name to search for the id</param>
		public void DeprecateTable(string interactionID, string idColumnName) {
			string[] oldColumnNames = ColumnNames;
			string oldIDcol = IDColumnName;
		    // 2008-03-18 @CZUE:  add try/finally in case the sql pukes to be sure the 
            // column names get restored to their original state
            try
            {
                IDColumnName = idColumnName;
                ColumnNames = new string[] { "RowStatus" };
                object[] values = { 1 };
                // _helper.Execute(CreateUpdateCommand(interactionID, ColumnNames, values ));
                _helper.Execute(CreateUpdateCall(interactionID, values));
            }
            finally
            {
                ColumnNames = oldColumnNames;
                IDColumnName = oldIDcol;
            }
		}
        
		#endregion

	}
}