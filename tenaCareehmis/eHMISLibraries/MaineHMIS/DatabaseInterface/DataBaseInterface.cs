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


namespace eHMIS.HMIS.DatabaseInterface
{
   

        class DataBaseInterface
        {

            private SqlTransaction objTrans;

            private string connecString;

            public static SqlConnection myconn = new SqlConnection();


            private List<string> selectStatementslst;

            private List<string> insertStatementslst;

            private List<string> updateStatementslst;

            private List<string> deleteStatementslst;



            public DataBaseInterface()
            {

                selectStatementslst = new List<string>();
                insertStatementslst = new List<string>();
                updateStatementslst = new List<string>();
                deleteStatementslst = new List<string>();

                
               DBConnHelper DBConnHelper = new DBConnHelper();
           
               connecString = DBConnHelper.ConnectionString;
            }



            public void addSelectStatments(string stat)
            {
                selectStatementslst.Add(stat);
            }

            public void addInsertStatments(string stat)
            {
                insertStatementslst.Add(stat);
            }

            public void addUpdateStatments(string stat)
            {
                updateStatementslst.Add(stat);
            }

            public void addDeleteStatments(string stat)
            {
                deleteStatementslst.Add(stat);
            }


            public void clearSelectStatments()
            {
                selectStatementslst.Clear();
            }

            public void clearInsertStatments()
            {
                insertStatementslst.Clear();
            }

            public void clearUpdateStatments()
            {
                updateStatementslst.Clear();
            }

            public void clearDeleteStatments()
            {
                deleteStatementslst.Clear();
            }


            public void clearAllStatments()
            {
                selectStatementslst.Clear();
                insertStatementslst.Clear();
                updateStatementslst.Clear();
                deleteStatementslst.Clear();

            }

            public void doSelectStatments()
            {


            }

            public void doInsertStatments()
            {
                try
                {

                    myconn.Close();
                    myconn.ConnectionString = connecString;
                    myconn.Open();
                    objTrans = myconn.BeginTransaction();

                    SqlCommand sqlcmd = myconn.CreateCommand();
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.Transaction = objTrans;
                    foreach (string sqlStatement in insertStatementslst)
                    {

                        sqlcmd.CommandText = sqlStatement;
                        sqlcmd.ExecuteNonQuery();
                    }

                    objTrans.Commit();
                    myconn.Close();

                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    myconn.Close();
                    throw (ex);
                }

            }

            public void doUpdateStatments()
            {

                try
                {

                    myconn.Close();
                    myconn.ConnectionString = connecString;
                    myconn.Open();
                    objTrans = myconn.BeginTransaction();

                    SqlCommand sqlcmd = myconn.CreateCommand();
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.Transaction = objTrans;
                    foreach (string sqlStatement in updateStatementslst)
                    {

                        sqlcmd.CommandText = sqlStatement;
                        sqlcmd.ExecuteNonQuery();
                    }

                    objTrans.Commit();


                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    myconn.Close();
                    throw (ex);
                }


            }

            public void doDeletStatments()
            {

            }

        }



}
