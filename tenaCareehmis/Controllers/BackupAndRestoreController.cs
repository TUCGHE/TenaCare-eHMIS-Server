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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using SqlManagement.Database;
using System.Web.Http.Cors;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading.Tasks;
using BackUpAndRestore;

namespace eHMISWebApi.Controllers
{

    [EnableCors("*", "*", "*")]
    public class BackupAndRestoreController : ApiController
    {
        DBConnHelper _helper = new DBConnHelper();
        string folderName = @"c:\temp";
        string Filename = null;
        StringBuilder sb = new StringBuilder();
        string fileName = @"\eHMIS_" + DateTime.Now.Day + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".bak";
        private static string errormessage;
        [HttpPost]
        [EnableCors("*", "*", "*")]
        public async Task<HttpResponseMessage> PostFile()
        {
            if (Directory.Exists(folderName))
            {

            }
            else
            {
                Directory.CreateDirectory(folderName);

            }
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var provider = new CustomMultipartFormDataStreamProvider(folderName);
            try
            {
                // Request.Content.LoadIntoBufferAsync().Wait();
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                var model = result.FormData["model"];
                //  var year = result.FormData["year"];
                dynamic data = JObject.Parse(model);
                string databaseName = data.databaseName;

                // This illustrates how to get the file names for uploaded files.

                foreach (var file in provider.FileData)
                {
                    Filename = file.Headers.ContentDisposition.FileName;
                    Filename = Filename.Replace("\"", string.Empty).Trim();
                }
                if (Filename.Substring(Filename.LastIndexOf('.') + 1).ToLower() != "bak")
                {
                    sb.Append(string.Format("Wrong file uploaded: " + Filename + ", Please choose correct eHMIS report file."));
                    if (Filename != null)
                        System.IO.File.Delete(folderName + "\\" + Filename);
                }
                else
                {
                    string shortfilename = Filename;
                    string fullfilename = folderName + "\\" + Filename;
                }
                string DBbackuplocation = folderName + "\\HMISBackUpDB";
                if (Directory.Exists(DBbackuplocation))
                { }
                else
                {
                    Directory.CreateDirectory(DBbackuplocation);
                }
                dobackup(databaseName, @DBbackuplocation + fileName);

                dorestorwithMove(databaseName, Filename, folderName);

                return new HttpResponseMessage()
                {
                    Content = new StringContent(sb.ToString())
                };
            }
            catch (Exception ex)
            {
                if (Filename != null)
                    System.IO.File.Delete(folderName + "\\" + Filename);

                return new HttpResponseMessage()
                {

                    Content = new StringContent(HttpStatusCode.InternalServerError.ToString() + ". " + ex.Message)
                };
                // return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [EnableCors("*", "*", "*")]
        public string  Get()
        {
            string userName;
            string Domainame = Environment.UserDomainName.ToString();
            if (Domainame != "")
            {
                userName = Domainame + "\\" + Environment.UserName.ToUpper();
            }
            else
            {
                userName = Environment.UserName.ToUpper();
            }
            return userName;
        }
        [EnableCors("*", "*", "*")]
        public IEnumerable<object> Get(int id)
        {
            string[] listsDaily = new string[2];
            string[] listsWeekly = new string[2];
            string[] listsMonthly = new string[2];
            ScheduledTasks st = new ScheduledTasks();
            string[] tasklist = st.GetTaskNames();
            List<string[]> lstofTasks = new List<string[]>();
            for (int i = 0; i < tasklist.Length; i++)
            {
                if (tasklist[i].ToString() == "EHMISDataBaseBackUpDaily.job")
                {
                    listsDaily[0] = "0";
                    listsDaily[1] = "eHMIS DataBase Scheduled to Backup Daily";
                    lstofTasks.Add(listsDaily);
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpWeekly.job")
                {
                    listsWeekly[0] = "1";
                    listsWeekly[1] = "eHMIS DataBase Scheduled to Backup Weekly";
                    lstofTasks.Add(listsWeekly);
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpMonthly.job")
                {
                    listsMonthly[0] = "2";
                    listsMonthly[1] = "eHMIS DataBase Scheduled to Backup Monthly";
                    lstofTasks.Add(listsMonthly);
                }
            }
            return lstofTasks;
        }
        [HttpPost]
        [EnableCors("*", "*", "*")]
        public IEnumerable<object> Post(int id, [FromBody]string databaseName)
        {
            //eHMIS_03 January 2017_14_35.bak
            IEnumerable<object> message = null;
            if (id == 1)
            {
                if (Directory.Exists(folderName))
                {

                }
                else
                {
                    Directory.CreateDirectory(folderName);

                }
                message = dobackup(databaseName, folderName + fileName);

            }
            else if (id == 2)//to delete stored task
            {
                List<object> messages = new List<object>();
                string taskDelete = databaseName;
                bool deleted = deleteTask(taskDelete);
                if (deleted == true)
                {
                    messages.Add(deleted);
                }
                else
                    messages.Add(deleted);

                message = messages;
            }
            return message;

        }
        [HttpPost]
        [EnableCors("*", "*", "*")]
        public string Post(int id , int id2, [FromBody]ScheduleTime value)
        {
            string returnValue = null;
            if (id2 == 2)
            {
                if (value.Period == 0)
                {
                    CreatScheduleforDaily(Convert.ToInt16(value.Time.Hour), Convert.ToInt16(value.Time.Minute), value.UserName, value.Password);
                }
                else if (value.Period == 1)
                {
                    CreatScheduleforWeekly(value.UserName, value.Password, getDay(value.Day), Convert.ToInt16(value.Time.Hour), Convert.ToInt16(value.Time.Minute));
                }
                else if (value.Period == 2)
                {
                    CreatScheduleforMonthly(value.UserName, value.Password, Convert.ToInt16(value.Time.Hour), Convert.ToInt16(value.Time.Minute), value.Day.ToString());
                }
                if (errormessage == "")
                {
                    returnValue = "Schedule Created";
                }
                else
                {
                    returnValue = errormessage;
                }
            }
            else if (id2 == 3)
            {
                string constratTask = null;

                if (value.Period == 0)
                {
                    constratTask = "Daily";
                }
                else if (value.Period == 1)
                {
                    constratTask = "Weekly";
                }
                else
                {
                    constratTask = "Monthly";
                }

                bool deleted = deleteTask(constratTask);
                if (deleted == true)
                {
                    if (value.Period == 0)
                    {
                        CreatScheduleforDaily(Convert.ToInt16(value.Time.Hour), Convert.ToInt16(value.Time.Minute), value.UserName, value.Password);
                    }
                    else if (value.Period == 1)
                    {
                        CreatScheduleforWeekly(value.UserName, value.Password, getDay(value.Day), Convert.ToInt16(value.Time.Hour), Convert.ToInt16(value.Time.Minute));
                    }
                    else if (value.Period == 2)
                    {
                        CreatScheduleforMonthly(value.UserName, value.Password, Convert.ToInt16(value.Time.Hour), Convert.ToInt16(value.Time.Minute), value.Day.ToString());
                    }
                    if (errormessage == "")
                    {
                        returnValue = "Schedule Created";
                    }
                    else
                    {
                        returnValue = errormessage;
                    }
                }
                else
                    returnValue = "Unable to update task";

            }

            return returnValue;
        }

        public List<object> dobackup(string dbname, string filelocation)
        {
            List<object> message = new List<object>();
            try
            {
                string cmdText = "backup database " + dbname + " to disk='" + filelocation + "'";
                SqlCommand sqlcmd = new SqlCommand(cmdText);
                _helper.Execute(sqlcmd);
                message.Add(true);
                message.Add("Database Backup Successfully Completed " + filelocation);

            }
            catch (Exception exc)
            {
                message.Add(false);
                message.Add(exc.Message.ToString());

            }
            return message;
        }
        public void dorestorwithMove(string dbname, string bakfilename, string locationDB)
        {
            ServiceController serviceController = new ServiceController();
            serviceController.ServiceName = "SQL Server (EHMIS)";
            serviceController.Stop();

            string cmdText = "drop database eHMIS";
            SqlCommand cmd = new SqlCommand(cmdText);
            _helper.Execute(cmd);

            try
            {
                cmdText = @"restore database " + dbname + @" from disk = '" + bakfilename + @"' with move 'eHMIS' to '" + locationDB + @".mdf', move 'eHMIS_log' to '" + locationDB + "_log.ldf'";

                SqlCommand sqlcmd = new SqlCommand(cmdText);
                _helper.Execute(sqlcmd);
            }
            catch (Exception exp)
            {
                //General.Util.UI.MyMessageDialogSmall.Show(exp.ToString());
            }
            serviceController.Start();
        }
       public DaysOfTheWeek getDay (int day)
        {
            DaysOfTheWeek dayofweek = new DaysOfTheWeek();
           switch(day)
           {
                case 0:
                    dayofweek =  DaysOfTheWeek.Sunday;
                    break;
                case 1:
                    dayofweek =  DaysOfTheWeek.Monday;
                    break;
                case 2:
                    dayofweek = DaysOfTheWeek.Tuesday;
                    break;
                case 3:
                    dayofweek = DaysOfTheWeek.Wednesday;
                    break;
                case 4:
                    dayofweek = DaysOfTheWeek.Thursday;
                    break;
                case 5:
                    dayofweek = DaysOfTheWeek.Friday;
                    break;
                case 6:
                    dayofweek = DaysOfTheWeek.Saturday;
                    break;
                default:
                    break;   
           }
            return dayofweek;
        }
        public int[] checkMonth(string dys)
        {
            int selectday = Convert.ToInt16(dys);
            int[] days = { selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday };

            if (selectday > 28 && selectday < 30)
            {
                int[] days2 = { selectday, 28, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday };
                return days2;
            }
            else if (selectday > 30)
            {
                int[] days2 = { selectday, 28, selectday, 30, selectday, 30, selectday, selectday, 30, selectday, 30, selectday };
                return days2;
            }

            return days;

        }
        /// <summary>
        /// Schedule
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="min"></param>
        /// <param name="username"></param>
        /// <param name="psw"></param>
        public void CreatScheduleforDaily(short hour, short min, string username, string psw)
        { 
            ScheduledTasks st = new ScheduledTasks();
            BackUpAndRestore.Task t = null;
            try
            {
                t = st.CreateTask("EHMISDataBaseBackUpDaily");
            }
            catch
            {
                errormessage = "Task Exists";

                //General.Util.UI.MyMessageDialogSmall.Show("Task Exit");
                goto last;
            }

            string path = folderName;
            //if (path.Contains("NewEhmisPhemModules"))
            //{
                t.ApplicationName = path + @"\backupDeveloper.bat";
            //}
            //else
            //{
            //    t.ApplicationName = path + @"\backupInstaller.bat";
            //}
            try
            {
                t.SetAccountInformation(@username, psw);
                t.MaxRunTime = new TimeSpan(0, 6, 0);
                t.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                t.Comment = "Daily EHMIS DataBase Backup";
                t.Triggers.Add(new DailyTrigger(hour, min));
                t.Save();
                t.Close();
               // lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled To Backup Daily");
               // lstAllTaskForEdit.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled To Backup Daily");
            }
            catch
            {
                bool de = st.DeleteTask("EHMISDataBaseBackUpDaily");
                // General.Util.UI.MyMessageDialogSmall.Show("Password Is Not Correct");
                errormessage = "Password is not Correct";
                //txtpassword.Text = "";
              //  txtpassword.Focus();
                goto last;
            }

            last:
            {
                st.Dispose();
            }
        }
        public void CreatScheduleforWeekly(string username, string psw, DaysOfTheWeek day, short hour, short min)
        {
            ScheduledTasks st = new ScheduledTasks();
            BackUpAndRestore.Task t = null;
            string[] stl = st.GetTaskNames();
            try
            {
                t = st.CreateTask("EHMISDataBaseBackUpWeekly");
            }
            catch
            {
                //General.Util.UI.MyMessageDialogSmall.Show("Task Exit");
                errormessage = "Task Exists";
                goto last;
            }

            string path = folderName;

            //string path = Application.StartupPath;
            //if (path.Contains("NewEhmisPhemModules"))
            //{
                t.ApplicationName = path + @"\backupDeveloper.bat";
            //}
            //else
            //{
            //    t.ApplicationName = path + @"\backupInstaller.bat";
            //}
            try
            {
                t.SetAccountInformation(@username, psw);
                t.MaxRunTime = new TimeSpan(0, 6, 0);
                t.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                t.Comment = "Weekly EHMIS DataBase Backup";
                t.Triggers.Add(new WeeklyTrigger(hour, min, day));
                t.Save();
                t.Close();
                //lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Weekly");
               // lstAllTaskForEdit.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Weekly");
            }
            catch
            {
                bool de = st.DeleteTask("EHMISDataBaseBackUpWeekly");
                //General.Util.UI.MyMessageDialogSmall.Show("Password Is Not Correct");
                errormessage = "Password is not correct";
                goto last;
            }

            last:
            {
                st.Dispose();
            }

        }
        public void CreatScheduleforMonthly(string username, string psw, short hour, short min, string days)
        {
            ScheduledTasks st = new ScheduledTasks();
            BackUpAndRestore.Task t = null;
            string[] stl = st.GetTaskNames();
            try
            {
                t = st.CreateTask("EHMISDataBaseBackUpMonthly");
            }
            catch
            {
                // General.Util.UI.MyMessageDialogSmall.Show("Task Exit");
                errormessage = "Task Exists";
                goto last;
            }

            string path = folderName;
            //if (path.Contains("Ver1-EthiopiaMerge-Jan15-2010"))
            //{
                t.ApplicationName = path + @"\backupDeveloper.bat";
            //}
            //else
            //{
            //    t.ApplicationName = path + @"\backupInstaller.bat";
            //}
            try
            {
                t.SetAccountInformation(@username, psw);
                t.MaxRunTime = new TimeSpan(0, 6, 0);
                t.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                t.Comment = "Monthly EHMIS DataBase Backup";
                //checkMonth(days);
                int dys = Convert.ToInt16(days);
                int[] DaysOfMonth = checkMonth(days);//{ dys, dys, dys, dys, dys, dys, dys, dys, dys, dys, dys, dys }; 

                t.Triggers.Add(new MonthlyTrigger(hour, min, DaysOfMonth));
                t.Save();
                t.Close();
                //lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Monthly");
                //lstAllTaskForEdit.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Monthly");
            }
            catch
            {
                bool de = st.DeleteTask("EHMISDataBaseBackUpMonthly");
                //General.Util.UI.MyMessageDialogSmall.Show("Password Is Not Correct");
                errormessage = "Password is not correct";
                goto last;
            }

            last:
            {
                st.Dispose();
            }

        }
        private bool deleteTask(string tasktodelete)
        {
            ScheduledTasks st = new ScheduledTasks();
            bool delte = false;
            if (tasktodelete.Contains("Daily"))
            {
                delte = st.DeleteTask("EHMISDataBaseBackUpDaily");
            }
            else if (tasktodelete.Contains("Weekly"))
            {
                delte = st.DeleteTask("EHMISDataBaseBackUpWeekly");
            }
            else if (tasktodelete.Contains("Monthly"))
            {
                delte = st.DeleteTask("EHMISDataBaseBackUpMonthly");
            }
            return delte;
        }
    }
    public class ScheduleTime
    {
        public string DatabaseName { get; set; }
        public int Period { get; set; }
        public int Day { get; set; }
        public DateTime Time { get; set; }    
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
