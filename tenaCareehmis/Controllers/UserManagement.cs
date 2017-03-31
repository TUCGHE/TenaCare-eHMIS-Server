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

using Admin.Security.Model;
using SqlManagement.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace eHMISWebApi.Controllers
{
    internal class UserManagement
    {
        public static List<User> GetAllUsersList(bool withgrant)
        {
            return GetAllUsersList(withgrant, false); // by default, exclude deactivated users
        }

        public static List<User> GetAllUsersList(bool withgrant, bool includeInActive)
        {
            string sql = "SELECT  FullName, UserId, Password, IsActive, TitleOfCourtesy,JobTitle FROM [User] "+ (!includeInActive ? "WHERE IsActive ='True'": "");
            List<User> allUsers = new List<User>();
            DataSet ds = new DataSet();
            SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            if (!(Connection.State == ConnectionState.Open))
            {
                Connection.Open();
            }
            try
            {
                new SqlDataAdapter(sql, Connection).Fill(ds);
            }
            catch (Exception exc)
            {
                throw (new Exception("The dataset could not be filled [query] " + sql + " [/query]", exc));
            }
            finally
            {
                Connection.Close();
            }
            if (ds.Tables[0].Rows.Count > 0)
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    short title = 0;
                    short jobtitle = 0;
                    try
                    {
                        title = (dr["TitleOfCourtesy"] != null ? Convert.ToInt16(dr["TitleOfCourtesy"].ToString()) : (short)0);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        jobtitle = (dr["JobTitle"] != null ? Convert.ToInt16(dr["JobTitle"].ToString()) : (short)0);
                    }
                    catch (Exception)
                    {
                    }
                    if (withgrant && dr["UserId"].ToString().Contains("sadmin"))
                    {
                        allUsers.Add(new User(dr["UserId"].ToString(), dr["FullName"].ToString(), dr["Password"].ToString(),
                            Convert.ToBoolean(dr["IsActive"].ToString()), title, jobtitle));
                    }
                    else if (!dr["UserId"].ToString().Contains("sadmin"))
                    {
                        allUsers.Add(new User(dr["UserId"].ToString(), dr["FullName"].ToString(), dr["Password"].ToString(),
                            Convert.ToBoolean(dr["IsActive"].ToString()), title, jobtitle));
                    }
                }
            return allUsers;
        }
    }
}