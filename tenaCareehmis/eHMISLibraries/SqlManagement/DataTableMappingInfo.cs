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

namespace SqlManagement.Database
{
    /// <summary>
    /// Data class to store table mappings
    /// </summary>
    public class DataTableMappingInfo
    {
        #region Member Variables

        private DataTableMappingTypes _id;
        private string _databaseName;
        private string _description;
        
        #endregion

        #region Constructors

        public DataTableMappingInfo(DataTableMappingTypes id, string databaseName, string description)
        {
            _id = id;
            _databaseName = databaseName;
            _description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Id of the mapping
        /// </summary>
        public DataTableMappingTypes Id
        {
            // 2008-12-18 @CZUE:  I like the idea of making this an enum, but for truly dynamic mapping
            // we could just make it a string.  Left as a TODO.
            get { return _id; }
        }

        /// <summary>
        /// The name of the database to talk to
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
        }

        /// <summary>
        /// A description of the mapping
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the readable Id String for the ID
        /// </summary>
        /// <returns></returns>
        public string GetIdString()
        {
            return Enum.GetName(typeof (DataTableMappingTypes), _id);
        }

        public override bool Equals(object obj)
        {
            DataTableMappingInfo other = obj as DataTableMappingInfo;
            if (other == null) {
                return false;
            }
            return other.Id == Id && other.DatabaseName == DatabaseName && other.Description == Description;
        }

        public override int GetHashCode()
        {
            // use XOR of the three items
            return Id.GetHashCode() ^ DatabaseName.GetHashCode() ^ Description.GetHashCode();
        }
        
        public override string ToString()
        {
            return string.Format("Id: {0}, Db: {1}, Description: {2}", GetIdString(), DatabaseName, Description);
        }
        
        #endregion

        #region Private Methods

        #endregion

    }
}