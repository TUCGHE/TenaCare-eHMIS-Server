
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
using SqlManagement.Database;
using Admin.Security.Model;
using System.Diagnostics;
namespace Admin.Security.Utility
{
    public class SecurityDAO
    {

        public static DataSet GetDataSet(string sql)
        {
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

            return ds;
        }
        public static List<User> GetAllUsersList()
        {
            string sql = "SELECT  FullName, UserId, Password, IsActive FROM [User] WHERE IsActive='True'";
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
                    allUsers.Add(new User(dr["UserId"].ToString(), dr["FullName"].ToString(), dr["Password"].ToString(),
                        Convert.ToBoolean(dr["IsActive"].ToString())));
            return allUsers;
        }
       
        public static List<User> GetAllUsersList(bool withgrant)
        {
            string sql = "SELECT  FullName, UserId, Password, IsActive, TitleOfCourtesy,JobTitle FROM [User] WHERE IsActive='True'";
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

        public static bool SaveUser(User usr, string fullName, bool isNew, string oldid)
        {
            if (usr == null)
                return false;
            bool success = false;
            string insertSql = "INSERT into [User] Values ('" +
                usr.FullName + "','" + usr.UserId + "','" + CryptorEngine.Encrypt(usr.Password, false) +
                "','" + usr.IsActive + "'," + usr.Title + "," + usr.JobTilte + ",'false')";
            string updateSql = "UPDATE [User] SET [FullName] = '" + usr.FullName +
                "'    , [UserId] = '" + usr.UserId + "'," + " [Password] ='" + CryptorEngine.Encrypt(usr.Password, false) +
                "',[IsActive]='"
                + usr.IsActive + "', [TitleOfCourtesy]=" + usr.Title + ", [JobTitle]=" + usr.JobTilte + "  WHERE UserId='" + oldid + "'";
            SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            SqlCommand insertUpdateCommand = new SqlCommand();
            if (isNew)
                insertUpdateCommand.CommandText = insertSql;
            else
                insertUpdateCommand.CommandText = updateSql;
            if (!(Connection.State == ConnectionState.Open))
                Connection.Open();
            try
            {
                insertUpdateCommand.Connection = Connection;
                insertUpdateCommand.ExecuteNonQuery();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                if (ex.Message.Contains("PRIMARY KEY"))
                    throw new Exception("There is a user already saved with the same name in the database. Please provide another user name.");
                else
                    throw ex;
            }
            finally
            {
                Connection.Close();
            }
            return success;
        }


        public static List<Role> GetAllRolesList()
        {
            string sql = "SELECT RoleId, RoleDescription, IsActive FROM [Role]";
            List<Role> allRoles = new List<Role>();
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
                    allRoles.Add(new Role(Convert.ToInt32(dr["RoleId"].ToString()), dr["RoleDescription"].ToString(),
                        Convert.ToBoolean(dr["IsActive"].ToString())));
            return allRoles;
        }
        public static List<string> GetAllSecurableList()
        {
            string sql = "SELECT Description  FROM dbo.[Securable] WHERE IsActive ='true'";
            List<string> allSecurables = new List<string>();
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
            }
            finally
            {
                Connection.Close();
            }
            if (ds.Tables[0].Rows.Count > 0)
                foreach (DataRow dr in ds.Tables[0].Rows)
                    allSecurables.Add(dr["Description"].ToString());
            return allSecurables;
        }

        public static bool SaveUserRole(bool isNew, UserRole userRole)
        {

            if (userRole == null)
                return false;
            int returned = 0;
            string insertSql = "INSERT into [UserRole] (UserId, RoleId, IsActive) Values ('" + userRole.UserId + "'," + userRole.RoleId + ",'" + userRole.IsActive + "')";
            if (isNew)
            {
                SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());

                if (!(Connection.State == ConnectionState.Open))
                {
                    Connection.Open();
                }
                try
                {
                    SqlCommand insertCommand = new SqlCommand(insertSql, Connection);
                    returned = insertCommand.ExecuteNonQuery();

                }
                catch (Exception ex) { throw ex; }

                finally
                {
                    Connection.Close();
                }
            }
            else
            {
            }

            return (returned != 0);
        }

        public static DataTable GetRolesPerUser(string uid)
        {
            string sql = "SELECT  RoleDescription,UserRoleId, UserId, dbo.[Role].RoleId, dbo.UserRole.IsActive  FROM dbo.UserRole  JOIN dbo.[Role] " +
                    " ON UserRole.RoleId = dbo.[Role].RoleId  WHERE UserID='" + uid + "'";
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
            return ds.Tables[0];
        }
      
        public static string GetScalar(string sql)
        {
            string scalar = null;
            SqlConnection Connection = null;
            try
            {
                Connection = new SqlConnection(DBConnection.GetConnectionString());
                if (!(Connection.State == ConnectionState.Open))
                {
                    Connection.Open();
                }

                scalar = new SqlCommand(sql, Connection).ExecuteScalar().ToString();
            }
            catch (Exception exc)
            {

            }
            finally
            {
                Connection.Close();
            }
            return scalar;
        }
        public static bool SaveUserPermission(Permission usrPermission, bool isNew)
        {
            if (usrPermission == null)
                return false;
            bool success = false;
            string insertSql = " INSERT INTO [Permission]  ([RoleId],[SecurableId] ,[CanWrite] ,[CanRead],[IsActive])   VALUES " +
                " ( " + usrPermission.RoleId + "," + usrPermission.SecurableId + ",'" + usrPermission.CanWrite + "','" + usrPermission.CanRead + "','" +
                usrPermission.IsActive + "')";
            string updateSql = "UPDATE [dbo].[Permission]   SET [RoleId] = " + usrPermission.RoleId + ", [SecurableId] = " + usrPermission.SecurableId +
                ",    [CanWrite] ='" + usrPermission.CanWrite + "', [CanRead]= '" + usrPermission.CanRead + "' ,[IsActive] = '" + usrPermission.IsActive + "'" +
            " WHERE  PermissionId=" + usrPermission.PermissionId;

            //Replaced new connection creation to use DBConnHelper connection
            //Modified Jan 2012-01-12
            DBConnHelper dbConnHelper = new DBConnHelper();

            //SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            SqlConnection Connection = dbConnHelper.Connection;

            //if (!(Connection.State == ConnectionState.Open))
            //{
            //    Connection.Open();
            //}
            SqlCommand insertUdateCommand = new SqlCommand();
            if (isNew)
            {
                insertUdateCommand.CommandText = insertSql;
            }

            else
            {
                insertUdateCommand.CommandText = updateSql;
            }
            insertUdateCommand.Connection = Connection;
            try
            {
                insertUdateCommand.ExecuteNonQuery();
                success = true;
            }
            catch (Exception ex) { success = false; }

            finally
            {
                Connection.Close();
            }

            return success;
        }

        public static bool deleteAllRolesForUser(string userId, string roleid)
        {
            string sql = "";
            if (roleid.Equals("") || roleid.Equals(null))
                sql = "DELETE FROM [UserRole]  WHERE UserId = '" + userId + "'";
            else
                sql = "DELETE FROM [UserRole]  WHERE UserId = '" + userId + "' and RoleId=" + roleid + "";

            //Replaced new connection creation to use DBConnHelper connection
            //Modified Jan 2012-01-12
            DBConnHelper dbConnHelper = new DBConnHelper();

            //SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            SqlConnection Connection = dbConnHelper.Connection;

            //if (!(Connection.State == ConnectionState.Open))
            //{
            //    Connection.Open();
            //}

            try
            {
                SqlCommand deleteCommand = new SqlCommand(sql, Connection);
                deleteCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { return false; }

            finally
            {
                Connection.Close();
            }
        }
        public static bool setUserInactive(User usr, bool status)
        {
            string sql = "UPDATE [User]  SET [IsActive] = '" + status + "' WHERE UserId ='" + usr.UserId + "'";


            //Replaced new connection creation to use DBConnHelper connection
            //Modified Jan 2012-01-12
            DBConnHelper dbConnHelper = new DBConnHelper();

            //SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            SqlConnection Connection = dbConnHelper.Connection;

            //if (!(Connection.State == ConnectionState.Open))
            //{
            //    Connection.Open();
            //}

            try
            {
                SqlCommand deleteCommand = new SqlCommand(sql, Connection);
                deleteCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { return false; }

            finally
            {
                Connection.Close();
            }
        }
        public static bool DataExists(string tableName, string columnName, string value)
        {
            string sql = "SELECT * FROM " + tableName + " WHERE " + columnName + "='" + value + "'";
            SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            if (!(Connection.State == ConnectionState.Open))
            {
                Connection.Open();
            }
            try
            {

                SqlCommand checkCommand = new SqlCommand(sql, Connection);
                SqlDataReader reader = checkCommand.ExecuteReader();
                return reader.HasRows;
            }
            catch (Exception ex) { return false; }
            finally
            {
                Connection.Close();
            }
        }

        public static bool AddNewRole(string roleDescription)
        {
            string sql = "INSERT INTO dbo.[Role] (RoleDescription, IsActive) VALUES ('" + roleDescription + "' , 'true')";
            SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            if (!(Connection.State == ConnectionState.Open))
            {
                Connection.Open();
            }
            try
            {
                SqlCommand insertCommand = new SqlCommand(sql, Connection);
                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { return false; }
            finally
            {
                Connection.Close();
            }
        }
        public static bool AddNewSecurable(string securableDescription)
        {
            string sql = "INSERT INTO dbo.[Securable] (Description, IsActive) VALUES ('" + securableDescription + "' , 'true')";
            SqlConnection Connection = new SqlConnection(DBConnection.GetConnectionString());
            if (!(Connection.State == ConnectionState.Open))
            {
                Connection.Open();
            }
            try
            {
                SqlCommand insertCommand = new SqlCommand(sql, Connection);
                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { return false; }
            finally
            {
                Connection.Close();
            }
        }

        public static int GetUserByIntID(string UserID)
        {
            int intid = Convert.ToInt32(GetScalar
                ("SELECT Id FROM dbo.[User] WHERE UserId='" + UserID + "'"));
            return intid;
        }
        public static string GetUserByStringID(int intid)
        {
            string userid = GetScalar
                ("SELECT Id FROM dbo.[User] WHERE Id='" + intid.ToString() + "'");
            return userid;
        }       
        public static bool ExecuteSQLCommand(string sql)
        {
            SqlConnection conn = new SqlConnection(DBConnection.GetConnectionString());
            if (conn.State != ConnectionState.Open)
                conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable GetListOfInactiveUsers()
        {
            string sql = "select UserId, FullName as [User Full Name],IsActive  from dbo.[User] left outer join dbo.UserTitle " +
                        " on dbo.[User].TitleOfCourtesy = dbo.UserTitle.id where IsActive='False' and UserId != 'sadmin'";
            SqlConnection conn = new SqlConnection(DBConnection.GetConnectionString());
            if (conn.State != ConnectionState.Open)
                conn.Open();
            DataSet ds = new DataSet();
            try
            {
                new SqlDataAdapter(sql, conn).Fill(ds);
                DataTable dt = ds.Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }

        }
        public static void DeleteUserInfo(string uid)
        {
            string sql = "DELETE dbo.[USER] WHERE UserId='" + uid + "'";
            ExecuteSQLCommand(sql);

        }
        public static void ClearAllRememberMe()
        {
            string str = "UPDATE dbo.[User]  SET  IsLastLoggedIn='False' ";
            ExecuteSQLCommand(str);
        }
        public static void HandleRememberMe(string userid)
        {
            string str1 = "UPDATE dbo.[User]  SET  IsLastLoggedIn='False' WHERE UserId!='" + userid + "'";
            string str2 = "UPDATE dbo.[User]  SET  IsLastLoggedIn='True'  WHERE UserId='" + userid + "'";
            ExecuteSQLCommand(str1);
            ExecuteSQLCommand(str2);
        }
        public static string GetLastUserForRememberMe()
        {
            string str = "SELECT UserId FROM dbo.[User]  WHERE IsLastLoggedIn='True'";
            string rememberMeUser = GetScalar(str);
            return (rememberMeUser != null ? rememberMeUser : "");
        }

        public static void ExitAlleHMISProcesses(string processName)
        {
            Process[] activeProcs = Process.GetProcessesByName(processName);
            foreach (Process p in activeProcs)
                p.Kill();
        }
    }

}
