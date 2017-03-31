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
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using UtilitiesNew.GeneralUtilities;

namespace SqlManagement.Database
{
    public class DBConnection
    {
        public static string GetConnectionString()
        {
            string ConnStrName = "HMISConnString";
            ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[ConnStrName];
            string conStr = settings.ConnectionString;
            SqlConnectionStringBuilder builder =
                       new SqlConnectionStringBuilder(conStr);
            builder.Password = CryptorEngine.Decrypt(builder.Password, false);                
            return builder.ConnectionString;
        }
        public static string GetConnectionString(string ConnStrName)
        {            
            ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[ConnStrName];
            string conStr = settings.ConnectionString;
            SqlConnectionStringBuilder builder =
                       new SqlConnectionStringBuilder(conStr);
            builder.Password = CryptorEngine.Decrypt(builder.Password, false);
            return builder.ConnectionString;
        }
        public static string GetPatientRecordConnectionString()
        {
            string ConnStrName = "PatientRecordConnString";
            ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[ConnStrName];
            string conStr = settings.ConnectionString;
            SqlConnectionStringBuilder builder =
                       new SqlConnectionStringBuilder(conStr);
            builder.Password = CryptorEngine.Decrypt(builder.Password, false);
            return builder.ConnectionString;
        }
        public static string GetSecurityConncetionString()
        {
            string ConnStrName = "SecurityConnString";
            ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[ConnStrName];
            string conStr = settings.ConnectionString;
            SqlConnectionStringBuilder builder =
                       new SqlConnectionStringBuilder(conStr);
            builder.Password = CryptorEngine.Decrypt(builder.Password, false);
            return builder.ConnectionString;
        }
        public static string GetMasterConnectionString()
        {
            string ConnStrName = "MasterDBConnString";
            ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[ConnStrName];
            string conStr = settings.ConnectionString;
            SqlConnectionStringBuilder builder =
                       new SqlConnectionStringBuilder(conStr);
            builder.Password = CryptorEngine.Decrypt(builder.Password, false);
            return builder.ConnectionString;
        }
        public static string GetGISConnectionString()
        {
            string ConnStrName = "GISDBConnString";
            ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[ConnStrName];
            string conStr = settings.ConnectionString;
            SqlConnectionStringBuilder builder =
                       new SqlConnectionStringBuilder(conStr);
            builder.Password = CryptorEngine.Decrypt(builder.Password, false);
            return builder.ConnectionString;
        }
       
    }
}
