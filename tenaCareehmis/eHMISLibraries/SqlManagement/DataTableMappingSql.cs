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
using System.Data;
//using CCP.Entity.Database;

namespace SqlManagement.Database
{
    /// <summary>
    /// Class for interaction with the DataTableMapping table, which stores mappings of tables
    /// or groups of tables to databases
    /// </summary>
    public class DataTableMappingSql : SqlTable
    {
        #region Member Variables

        #endregion

        #region Constructors

        public DataTableMappingSql(DBConnHelper helper)
        {
            _helper = helper;
            TableName = "DataTableMappings";
            IDColumnName = "Id";
            ColumnNames =
                new string[]
                    {
                        "Id",
                        "DatabaseName",
                        "Description"
                    };

            ColumnTypes =
                new string[]
                    {
                        "[varchar](32)  NOT NULL",
                        "[varchar](32)  NOT NULL",
                        "[varchar](200)  NOT NULL"
                    };
        }


        #endregion

        #region Properties

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves a data mapping.  This will create the row if it doesn't already exist 
        /// </summary>
        /// <param name="mappingInfo"></param>
        public void Save(DataTableMappingInfo mappingInfo) {
            if (Exists(mappingInfo.GetIdString()))
            {
                _helper.Execute(
                    CreateUpdateCall(mappingInfo.GetIdString(),
                                     new object[] { mappingInfo.GetIdString(), mappingInfo.DatabaseName, mappingInfo.Description }));
            }
            else
            {
                _helper.Execute(CreateInsertSqlCommand(
                                    new object[]
                                        {mappingInfo.GetIdString(), mappingInfo.DatabaseName, mappingInfo.Description}));
            }
        }

        /// <summary>
        /// Get all known DB mappings
        /// </summary>
        /// <returns></returns>
        public List<DataTableMappingInfo> GetAll()
        {
            string sql = CreateSelectAllCall();
            DataTable dt = _helper.GetDataSet(sql).Tables[0];
            List<DataTableMappingInfo> toReturn = new List<DataTableMappingInfo>();
            foreach (DataRow row in dt.Rows)
            {
                toReturn.Add(BuildMappingFromDataRow(row));
            }
            return toReturn;
        }

        private DataTableMappingInfo BuildMappingFromDataRow(DataRow row)
        {
            DataTableMappingTypes type = (DataTableMappingTypes) Enum.Parse(typeof (DataTableMappingTypes), (string) row["Id"]);
            string dbName = (string) row["DatabaseName"];
            string desc = (string)row["Description"];
            return new DataTableMappingInfo(type, dbName, desc);
        }

        /// <summary>
        /// Deletes a data mapping
        /// </summary>
        /// <param name="dataMapping"></param>
        public void Delete(string dataMapping)
        {
            if (Exists(dataMapping))
            {
                _helper.Execute(CreateDeleteCall(dataMapping));
            }
            else
            {
                // nothing to do - this will be hidden from the end user
            }
        }

        /// <summary>
        /// Return whether a mapping with the passed in id has already been added to the DB
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool Exists(string type)
        {
            string sql = CreateExistsCall(type);
            int count = (int) _helper.GetScalar(sql);
            return count > 0;
        }
        

        #endregion

        #region Private Methods

        #endregion
    }
}