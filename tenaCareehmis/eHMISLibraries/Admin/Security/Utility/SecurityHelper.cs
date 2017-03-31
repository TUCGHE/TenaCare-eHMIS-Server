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
using System.Data;
using System.Data.SqlClient;
using Admin.Security.Model;

namespace Admin.Security.Utility
{
    public class SecurityHelper
    {
        public static bool AuthenticateUser(User user)
        {
            string filter = " UserId = '" + user.UserId + "' and Password = '" + CryptorEngine.Encrypt(user.Password,false) + "' and IsActive='True'";
            //string filter = " UserId = '" + user.UserId + "' and Password = '" +user.Password+"' and IsActive='True'";
            string sqlStatement = "SELECT UserId,FullName, Password FROM [User] WHERE " + filter;
            DataTable usrDt = SecurityDAO.GetDataSet(sqlStatement).Tables[0];
            if (usrDt.Rows.Count > 0)
            {
                if (usrDt.Rows[0]["FullName"].ToString() != "" || usrDt.Rows[0]["FullName"].ToString() != null)
                    user.FullName = usrDt.Rows[0]["FullName"].ToString();
            }
            return (usrDt.Rows.Count> 0);

        }
        public static DataTable AuthorizeUser(string uid)
        {
            string sqlSt = "select Description, CanWrite, CanRead from dbo.userAuthorize('" + uid + "')";
            DataTable dt = SecurityDAO.GetDataSet(sqlSt).Tables[0];
            return dt;

        }
        public static bool ApplyWritePermissionPolicy(DataTable dtPermissions, string securiable)
        {
            for(int i=0;i<dtPermissions.Rows.Count;i++)
                if (dtPermissions.Rows[i]["Description"].ToString().Equals(securiable))
                {
                    return Convert.ToBoolean(dtPermissions.Rows[i]["CanWrite"].ToString());
                }
            return false;
        }
        public static bool ApplyReadPermissionPolicy(DataTable dtPermissions, string securiable)
        {
            for (int i = 0; i < dtPermissions.Rows.Count; i++)
                if (dtPermissions.Rows[i]["Description"].ToString().Equals(securiable))
                {
                    return Convert.ToBoolean(dtPermissions.Rows[i]["CanRead"].ToString());
                }
            return false;
        }


    }
}
