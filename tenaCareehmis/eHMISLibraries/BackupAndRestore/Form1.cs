using System;
using System.IO;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;

using System.Runtime.InteropServices;
using UtilitiesNew.GeneralUtilities;
using SqlManagement.Database;
using System.Security.Principal;
using eHMIS;

namespace BackUpAndRestore
{
    public partial class Form1 : BaseForm
    {
        private DBConnHelper _helper;
        static string filename;
        static int Tabindex = 0;
        static DataTable dt = new DataTable();
        private Form frm;
        private static string ErrorMessageRestor = "Please select resort location first.";
        private static SqlConnection myconn = new SqlConnection();
        private string connectionString = "Data Source=(local)\\EHMIS; Initial Catalog=Master; User ID=sa; Password=ruth!@#$1234";
        private static string selectedTask = "";
        private static string errormessage = "";

        public Form1()
        {
            InitializeComponent();
            Con();
            string cmdtext2 = "SELECT * FROM sys.sysdatabases where name !='master' and name !='msdb' and name !='model' and name !='tempdb' order by name";
            DataTable dt = new DataTable();
            DataSet _ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmdtext2, myconn);
            da.Fill(_ds);
            dt = _ds.Tables[0];

            for (int index = 0; index < dt.Rows.Count; index++)
            {
                cmblistofdb.Items.Add(dt.Rows[index][0].ToString());
                cmblistofdbForrestor.Items.Add(dt.Rows[index][0].ToString());
            }
            cmblistofdb.Text = "eHMIS";
            cmblistofdbForrestor.Text = "eHMIS";
            this.btnsetbackuptime_Click(btnsetbackuptime, EventArgs.Empty);
            if (lstTaskList.Items.Count > 0)
            {
                btneditTimeNew.Enabled = true;
                btnDeleteNew.Enabled = true;
            }
            //this.btnvieweditschedule_Click(btnvieweditschedule, EventArgs.Empty);
            myconn.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //string path = eHMIS.Program.ApplicationStartupPath + "\\HMISBackUpDB";
            string path = AppDomain.CurrentDomain.BaseDirectory + "HMISBackUpDB";

            //string connectionString = CCP.Framework.Sql.SqlManager.Instance.SqlConnection._connectionString;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            Con();
        }
        public void Con()
        {

            myconn.Close();
            myconn.ConnectionString = connectionString;
            try
            {
                myconn.Open();

            }
            catch (Exception exc)
            {
                General.Util.UI.MyMessageDialogSmall.Show(exc.Message.ToString());
            }
        }
        private void btnbrowse_Click(object sender, EventArgs e)
        {
            string DBbackuplocation = BackUpAndRestore.Program.ApplicationStartupPath + "\\HMISBackUpDB";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = DBbackuplocation;
            saveDialog.FileName = cmblistofdb.Text+"_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString()  + ".bak";
            saveDialog.Title = "Select backup file location";
            //string path = eHMIS.Program.ApplicationStartupPath;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            
            saveDialog.Filter = "eHMIS backup files (*.bak)|*.bak";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {

                filename = saveDialog.FileName;

                txtdestenationfolder.Text = filename;
                btnstrbackup.Enabled = true;
            }
          
        }

        private void btnstrbackup_Click(object sender, EventArgs e)
        {
            if (cmblistofdb.Text != "" && txtdestenationfolder.Text != "")
            {
                this.Cursor = Cursors.WaitCursor;
                displaymsg(cmblistofdb.Text + "Database is Backing Up....");
                dobackup(cmblistofdb.Text,filename);
                frm.Dispose();
                this.Cursor = Cursors.Default;
            }
            else
            { 
              
            }
        }
        public void dobackup(string dbname,string filelocation)
        {
            try
            {
                
                string cmdText = "backup database " + dbname + " to disk='"+filelocation+"'";
                Con();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandText = cmdText;
                sqlcmd.CommandTimeout = 300;// 5 min
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.Connection = myconn;
                sqlcmd.ExecuteNonQuery();
                myconn.Close();
                General.Util.UI.MyMessageDialogSmall.Show("Database Backup Successfully Completed " + filelocation);
            }
            catch (Exception exc)
            {
                General.Util.UI.MyMessageDialogSmall.Show(exc.Message.ToString());
            }

        }
        private void txtdestenationfolder_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnbackupdb_Click(object sender, EventArgs e)
        {
            try
            {
                //Tabindex = 1;
                //tabControl1.SelectedIndex = Tabindex;
                //string cmdText = "SELECT * FROM sys.sysdatabases where name !='master' and name !='msdb' and name !='model' and name !='tempdb' order by name";
                //SqlCommand toExecute = new SqlCommand();
                //_helper = new DBConnHelper();
                //toExecute.CommandType = CommandType.Text;
                //toExecute.CommandText = cmdText;
                //dt = _helper.GetDataSet(toExecute).Tables[0];
                //for (int index = 0; index < dt.Rows.Count; index++)
                //{
                //    cmblistofdb.Items.Add(dt.Rows[index][0].ToString());
                //}
                //cmblistofdb.Text = "CDC_FDB_DB";
                cmblistofdb.Text = "eHMIS";
                setColoring(sender);
            }
            catch (Exception exc)
            {
                General.Util.UI.MyMessageDialogSmall.Show(exc.Message.ToString());
            }
        }


        private void tabControl1_Click(object sender, EventArgs e)
        {
            //tabControl1.SelectedIndex = Tabindex;
            if (tabControl1.SelectedIndex == 0)
            {
                this.btnbackupdb_Click(btnbackupdb, EventArgs.Empty);
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                this.btnrestordb_Click(btnrestordb, EventArgs.Empty);
            }
            else if (tabControl1.SelectedIndex ==2)
            {
                this.btnsetbackuptime_Click(btnsetbackuptime, EventArgs.Empty);
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                this.btnvieweditschedule_Click(btnvieweditschedule, EventArgs.Empty);
            }
            else
            {
                setColoring(null);
            }
        }

        private void btnrestordb_Click(object sender, EventArgs e)
        {
            try
            {
                setColoring(sender);
                //Tabindex = 2;
                //tabControl1.SelectedIndex = Tabindex;
                listAllDBToCMBDBForRestor();
                cmblistofdbForrestor.Text = "eHMIS";
                //DataTable listofDB = new DataTable();
                //listofDB=getallDB();
                //for (int index = 0; index < listofDB.Rows.Count; index++)
                //{
                //    cmblistofdbForrestor.Items.Add(listofDB.Rows[index][0].ToString());
                //}
                //cmblistofdbForrestor.Items.Add("New DataBase");
                //cmblistofdbForrestor.SelectedItem = "CDC_FDB_DB";
            }
            catch (Exception exc)
            {
                General.Util.UI.MyMessageDialogSmall.Show(exc.Message.ToString());
            }
        }
        public DataTable getallDB()
        {
            //string cmdText = "SELECT * FROM sys.sysdatabases where name !='master' and name !='msdb' and name !='model' and name !='tempdb' order by name";
            //SqlCommand toExecute = new SqlCommand();
            //_helper = new DBConnHelper();
            //toExecute.CommandType = CommandType.Text;
            //toExecute.CommandText = cmdText;
            //DataTable dt = new DataTable();
            //dt = _helper.GetDataSet(toExecute).Tables[0];
            //return dt;



            string cmdtext2 = "SELECT * FROM sys.sysdatabases where name !='master' and name !='msdb' and name !='model' and name !='tempdb' order by name";
            DataTable dt = new DataTable();
            DataSet _ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmdtext2, myconn);
            da.Fill(_ds);
            dt = _ds.Tables[0];
            return dt;
        }
        private void cmblistofdbForrestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmblistofdbForrestor.Text == "New DataBase")
            {
                txtnewdb.Visible = true;
                lblnewdb.Visible = true;
            }
            else
            {
                txtnewdb.Visible = false;
                lblnewdb.Visible = false;
            }
        }

        private void btnrestor_Click(object sender, EventArgs e)
        {
            if (txtrestorelocation.Text != "")
            {
                if (cmblistofdbForrestor.Text == "New DataBase")
                {
                    try
                    {
                        OpenFileDialog opdilg = new OpenFileDialog();
                        opdilg.FileName = "*.bak";
                        opdilg.Title = "Select eHMIS backup file";
                        opdilg.Filter = "eHMIS Backup files (*.bak)|*.bak";
                        if (opdilg.ShowDialog() == DialogResult.OK)
                        {
                            //string restorlocation = eHMIS.Program.ApplicationStartupPath + "\\HMISRESTOREDDB";
                            string restorlocation = AppDomain.CurrentDomain.BaseDirectory + "HMISRESTOREDDB";

                            if (!System.IO.Directory.Exists(restorlocation))
                            {
                                System.IO.Directory.CreateDirectory(restorlocation);
                            }
                            string locationDB = restorlocation + "\\" + txtnewdb.Text;

                            dorestorwithMove(txtnewdb.Text, opdilg.FileName, locationDB);
                            General.Util.UI.MyMessageDialogSmall.Show("Successfully Completed");
                            txtnewdb.Visible = false;
                            lblnewdb.Visible = false;

                            //fill agina list of DB
                            listAllDBToCMBDBForRestor();
                        }
                        else
                        {
                            General.Util.UI.MyMessageDialogSmall.Show("Restoring database has been cancelled");
                        }
                    }
                    catch (Exception exc)
                    {
                        General.Util.UI.MyMessageDialogSmall.Show(exc.Message.ToString());
                    }
                }
                else
                {
                    if (General.Util.UI.MyMessageDialogSmall.Show("The eHMIS application needs to close to restore the database.All data on the current applicatoin will be changed with the new database data. click yes to continue", "Restoring...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        string DBbackuplocation = BackUpAndRestore.Program.ApplicationStartupPath + "\\HMISBackUpDB";

                        if (!System.IO.Directory.Exists(DBbackuplocation))
                        {
                            System.IO.Directory.CreateDirectory(DBbackuplocation);
                        }
                
                       

                        //OpenFileDialog opdilg = new OpenFileDialog();
                        //opdilg.FileName = "*.bak";
                        //opdilg.Title = "Select backup file";
                        //opdilg.Filter = "eHMIS Backup files (*.bak)|*.bak";
                        //if (opdilg.ShowDialog() == DialogResult.OK)
                        //{
                        //    filename = opdilg.FileName;
                            //show status on new box 
                            displaymsg("Please wait while old database is being backed up....");

                            dobackup(cmblistofdbForrestor.Text, @DBbackuplocation + "\\" + cmblistofdbForrestor.Text +"_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() +  ".bak");
                            frm.Dispose();
                            

                            //restore location
                            


                            displaymsg("Selected Database is Restoring....");
                            Upgrader ug = new Upgrader();
                            ug.FindAndKillProcess("MainApp");

                            dorestorwithReplace(cmblistofdbForrestor.Text, txtrestorelocation.Text);
                            frm.Dispose();
                            // start smartcare 
                            displaymsg("Starting eHMIS....");
                            try
                            {
                                Upgrader ug2 = new Upgrader();
                                string path = BackUpAndRestore.Program.ApplicationStartupPath;//Application.StartupPath.ToString();
                                //if (path.Contains("NewEhmisPhemModules"))
                                //{
                                //    path = path.Substring(0, path.LastIndexOf('\\')) + @"\RuntimeDirectory\eHMIS";
                                //}
                                //else 
                                //{
                                //    path = path.Substring(0, path.LastIndexOf('\\')) + @"\RuntimeDirectory\eHMIS";
                                //}

                                ug2.StartSmartCare(path);
                                
                                frm.Dispose();
                                ug.FindAndKillProcess("BackUpAndRestore");
                            }
                            catch (Exception ex)
                            {
                                //General.Util.UI.MyMessageDialogSmall.Show("SmarCare Can't Start");
                                General.Util.UI.MyMessageDialogSmall.Show(ex.Message.ToString());
                                frm.Dispose();
                            }
                          
                            

                        //}
                    }
                    else
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Restoring database has been canceled");
                    }
                }
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("Select DataBase First");
            }
        }
        private string RestoreLocation()
        {
           //FolderBrowserDialog FolderDialog = new FolderBrowserDialog();
           string path="";
           OpenFileDialog opdilg = new OpenFileDialog();
            opdilg.FileName = "*.bak";
            opdilg.Title = "Select backup file";
            opdilg.Filter = "eHMIS Backup files (*.bak)|*.bak";
            if (opdilg.ShowDialog() == DialogResult.OK)
            {

                path = opdilg.FileName;
            }
            
            return path;
        }
        private void displaymsg(string job)
        {
            frm = new Form();
            frm.Size = new System.Drawing.Size(386, 0);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.MinimizeBox = false;
            frm.MaximizeBox = false;
            frm.ControlBox = false;
            frm.TopMost = true;
            frm.Text = job;
            frm.Show();
        }
        private void listAllDBToCMBDBForRestor()
        {
            DataTable listofDB = new DataTable();
            listofDB = getallDB();
            cmblistofdbForrestor.Items.Clear();
            for (int index = 0; index < listofDB.Rows.Count; index++)
            {
                cmblistofdbForrestor.Items.Add(listofDB.Rows[index][0].ToString());
            }
            cmblistofdbForrestor.Items.Add("New DataBase");

        }

        public void dorestorwithReplace(string dbname, string filename)
        {
            try
            {
                // Close all active connections first
                //
                string single = "alter database " + dbname + " set single_user with rollback immediate";

                string cmdText = single + ";" + "restore database " + dbname + " from disk ='" + filename + "' with replace";// 'Database' to '"+mdflocation+"' , move 'Database_log' to '"+ldfloaction+"'";

                Con();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandTimeout = 300; // 5 min wait
                sqlcmd.CommandText = cmdText;
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.Connection = myconn;
                sqlcmd.ExecuteNonQuery();
                myconn.Close();

                string Dbloc = DBLocation();
                General.Util.UI.MyMessageDialogSmall.Show("Restoring Database is Successfully Completed and Restored Location is " + Dbloc);
            }
            catch (Exception ex)
            {
                General.Util.UI.MyMessageDialogSmall.Show(ex.ToString());
            }
            finally
            {
                string cmdText = "alter database " + dbname + " set multi_user";
                Con();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandTimeout = 50000;
                sqlcmd.CommandText = cmdText;
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.Connection = myconn;
                sqlcmd.ExecuteNonQuery();
            }
   
        }
        public string DBLocation()
        {
            string cmdtext2 = "SELECT name, physical_name AS current_file_location FROM sys.master_files where [name] = 'ehmis'";
            DataTable _dt = new DataTable();
            DataSet _ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmdtext2, myconn);
            da.Fill(_ds);
            _dt = _ds.Tables[0];
            string Dblocation = _dt.Rows[0][1].ToString().Replace(".mdf", "");
            return Dblocation;
        }
        public void dorestorwithMove(string dbname, string bakfilename, string locationDB)
        {
            try
            {
                string cmdText = @"restore database " + dbname + @" from disk = '" + bakfilename + @"' with move 'eHMIS' to '" + locationDB + @".mdf', move 'eHMIS_log' to '" + locationDB + "_log.ldf'";
                Con();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandTimeout = 50000;
                sqlcmd.CommandText = cmdText;
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.Connection = myconn;
                sqlcmd.ExecuteNonQuery();



                //SqlCommand toExecute = new SqlCommand();
                //_helper = new DBConnHelper();
                //toExecute.CommandType = CommandType.Text;
                //toExecute.CommandText = cmdText;
                //_helper.Execute(toExecute);
            }
            catch (Exception exp)
            {
                General.Util.UI.MyMessageDialogSmall.Show(exp.ToString());  
            }
        }
        //private string getdatabaseprelocation(string name)
        //{
        //    string cmdText = "SELECT filename FROM sys.sysdatabases where name=@name";
        //    SqlCommand toExecute = new SqlCommand();
        //    _helper = new DBConnHelper();
        //    toExecute.CommandType = CommandType.Text;
        //    toExecute.CommandText = cmdText;
        //    SqlParameter paraFilename = new SqlParameter("@name", name);
        //    toExecute.Parameters.Add(paraFilename);
        //    DataTable dt = new DataTable();
        //    dt = _helper.GetDataSet(toExecute).Tables[0];
        //    return dt.Rows[0][0].ToString();
        //}

        private void btnsetbackuptime_Click(object sender, EventArgs e)
        {
            setColoring(sender);
            //Tabindex = 3;
            //tabControl1.SelectedIndex = Tabindex;
            string Domainame = Environment.UserDomainName.ToString();
            if (Domainame != "")
            {
                txtusername.Text = Domainame + "\\" + Environment.UserName.ToUpper();
            }
            else
            {
                txtusername.Text = Environment.UserName.ToUpper();
            }
            //DataTable listofdb = new DataTable();
            //listofdb=getallDB();
            //for (int index = 0; index < listofdb.Rows.Count; index++)
            //{
            //    cmblistdbtoschedule.Items.Add(listofdb.Rows[index][0].ToString());
            //}
            cmblistdbtoschedule.Text = "eHMIS";
            lstTaskList.Items.Clear();
            ScheduledTasks st = new ScheduledTasks();
            string[] tasklist = st.GetTaskNames();
            for (int i = 0; i < tasklist.Length; i++)
            {
                if (tasklist[i].ToString() == "EHMISDataBaseBackUpDaily.job" )
                {
                    lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Daily");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpWeekly.job")
                {
                    lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Weekly");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpMonthly.job")
                {
                    lstTaskList.Items.Add(cmblistdbtoschedule.Text + " Database Scheduled to Backup Monthly");
                }
            }
            if (lstTaskList.Items.Count == 3)
            {
                btnadd.Enabled = false;
            }
            else
                btnadd.Enabled = true;
           
        }
        private void Warrning()
        {
           
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);//role);

                    if (isAdmin)
                    {
                        string warning = "The current user is an administrator.  Please log off and log in as 'eHMIS user' to create scheduled backup.";
                       // + Environment.NewLine + Environment.NewLine + "Scheduled back up you create now will work for you (the current user) but it will not work for the eHMIS user.";

                        //MessageBox.Show(warning, "User Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        General.Util.UI.MyMessageDialogSmall.Show(warning, "User Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

        }
        private void btnadd_Click(object sender, EventArgs e)
        {
            if (checkSchedule())
            {
                // Task t = null;
                ScheduledTasks st = new ScheduledTasks();
                try
                {
                    short hour = -999;
                    short min = -999;
                    try
                    {
                        hour = Convert.ToInt16(cmbhour.Text);
                        min = Convert.ToInt16(cmbmin.Text);
                        hour= getAMPM(cmbhour.Text, cmbAMPM.Text);
                    }
                    catch
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Please select hour and minute from the list");
                    }

                    if (cmbschedule.SelectedIndex == 0)//means Daily
                    {
                        CreatScheduleforDaily(hour, min, txtusername.Text, txtpassword.Text);
                        Warrning();
                    }
                    else if (cmbschedule.SelectedIndex == 1)  //Weekly
                    {
                        DaysOfTheWeek day = new DaysOfTheWeek();
                        day = getday();
                        CreatScheduleforWeekly(txtusername.Text.ToLower(), txtpassword.Text.ToLower(), day, hour, min);
                        Warrning();
                    }
                    else if (cmbschedule.SelectedIndex == 2)//Monthly
                    {
                        CreatScheduleforMonthly(txtusername.Text.ToLower(), txtpassword.Text.ToLower(), hour, min,cmbdays.Text);
                        Warrning();
                    }
                    if (btneditTimeNew.Text == "Update" && errormessage=="")
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Successfully Updated");
                        btneditTimeNew.Enabled = true;
                        btnDeleteNew.Enabled = true;
                        cmbschedule.Text = "";
                        cmbdays.Text = "";
                        cmbhour.Text = "";
                        cmbmin.Text = "";
                        cmbAMPM.Text = "";
                        changeVisibity(4);
                        cmbschedule.SelectedIndex = -1;
                        cmbschedule.Refresh();
                        Warrning();
                    }
                    else if (errormessage == "")
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Schedule Created");
                        btneditTimeNew.Enabled = true;
                        btnDeleteNew.Enabled = true;
                        cmbschedule.Text = "";
                        cmbdays.Text = "";
                        cmbhour.Text = "";
                        cmbmin.Text = "";
                        cmbAMPM.Text = "";
                        changeVisibity(4);
                        cmbschedule.SelectedIndex = -1;
                        cmbschedule.Refresh();
                    }
                    else
                    {
                        General.Util.UI.MyMessageDialogSmall.Show(errormessage);
                        errormessage = "";
                    }
                   
                }
                catch 
                {
                    
                    General.Util.UI.MyMessageDialogSmall.Show(errormessage);
                }
            }
        }
        private short getAMPM(string hour,string AMPM)
        {
            short hour1=Convert.ToInt16(hour);
            if (AMPM == "AM")
            {
                if (hour1 == 12)
                {
                    return 0;
                }
            }
            else if (AMPM == "PM")
            {
                if (hour1 != 12)
                {
                    hour1 = Convert.ToInt16(hour1 + 12);
                    return hour1;
                }
            }
            return hour1;
        }
        public bool checkSchedule()
        {
            if (cmbschedule.Text != "")
            {
                for (int index = 0; index < lstTaskList.Items.Count; index++)
                {
                    if (lstTaskList.Items[index].ToString().Contains(cmbschedule.Text))
                    {
                        General.Util.UI.MyMessageDialogSmall.Show(cmbschedule.Text+ " Backup exists. Pleasse select from the Schedule list to edit this schedule");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("Please Select Schedule");
                cmbschedule.Focus();
                return false;
            }
            
        }
        public bool checkUser()
        {
            if (txtusername.Text != "")
            {
                string Domainame = Environment.UserDomainName.ToString();
                string username = Environment.UserName.ToString();
               // string 
                return false;
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("Pleas Enter User Name");
                return false;
            }
        }
        private void btnsave_Click(object sender, EventArgs e)
        {
         
        }

        private void btnvieweditschedule_Click(object sender, EventArgs e)
        {
            setColoring(sender);
            //Tabindex = 4;
            //tabControl1.SelectedIndex = Tabindex;
           
            changeVisibityForedit(1);
            //DataTable listofdb = new DataTable();
            //listofdb=getallDB();  used to get all database found on ower SQL server
            //for (int index = 0; index < listofdb.Rows.Count; index++)
            //{
            //    cmblistdbtoschedule.Items.Add(listofdb.Rows[index][0].ToString());
            //}
           // cmblistdbtoschedule.Text = "CDC_FDB_DB";
            listoftaskforedit();
            
        }
        public void listoftaskforedit()
        {
            lstAllTaskForEdit.Items.Clear();
            lstTaskList.Items.Clear();
            ScheduledTasks st = new ScheduledTasks();
            string[] tasklist = st.GetTaskNames();
            for (int i = 0; i < tasklist.Length; i++)
            {
                if (tasklist[i].ToString() == "EHMISDataBaseBackUpDaily.job")
                {
                    lstAllTaskForEdit.Items.Add("eHMIS DataBase Scheduled to Backup Daily");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpWeekly.job")
                {
                    lstAllTaskForEdit.Items.Add("eHMIS DataBase Scheduled to Backup Weekly");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpMonthly.job")
                {
                    lstAllTaskForEdit.Items.Add("eHMIS DataBase Scheduled to Backup Monthly");
                }


                if (tasklist[i].ToString() == "EHMISDataBaseBackUpDaily.job")
                {
                    lstTaskList.Items.Add("eHMIS DataBase Scheduled to Backup Daily");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpWeekly.job")
                {
                    lstTaskList.Items.Add("eHMIS DataBase Scheduled to Backup Weekly");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpMonthly.job")
                {
                    lstTaskList.Items.Add("eHMIS DataBase Scheduled to Backup Monthly");
                }
            }
        }
        private void btneditTime_Click(object sender, EventArgs e)
        {
            if (lstAllTaskForEdit.Text != "")
            {
                changeVisibityForedit(0);
                if (lstAllTaskForEdit.SelectedItem.ToString().Contains("Daily"))
                {
                    cmbSchedulforedit.SelectedIndex = 0;
                    lbllistday.Visible = false;
                    cmblday.Visible = false;
                }
                else if (lstAllTaskForEdit.SelectedItem.ToString().Contains("Weekly"))
                {
                    cmbSchedulforedit.SelectedIndex = 1;
                    lbllistday.Visible = true;
                    cmblday.Visible = true;

                }
                else if (lstAllTaskForEdit.SelectedItem.ToString().Contains("Monthly"))
                {
                    cmbSchedulforedit.SelectedIndex = 2;
                    
                    lbllistday.Visible = true;
                    cmblday.Visible = true;
                }
                string Domainame = Environment.UserDomainName.ToString();
                if (Domainame != "")
                {
                    txtusernameforedit.Text = Domainame + "\\" + Environment.UserName.ToUpper();
                }
                else
                {
                    txtusernameforedit.Text = Environment.UserName.ToUpper();
                }

            }
            else
            {
                if (lstAllTaskForEdit.Items.Count == 0)
                {
                    General.Util.UI.MyMessageDialogSmall.Show("No task to edit");
                }
                else
                    General.Util.UI.MyMessageDialogSmall.Show("Before you click, select from the list");
            }
        }
        public void changeVisibityForedit(int sender)
        {
            bool setvisibilityEdit = false;
            bool setvisibilityUpdate = false;
            if (sender == 0)
            {
                setvisibilityEdit = false;
                setvisibilityUpdate = true;
            }
            else if (sender == 1)
            {
                setvisibilityEdit = true;
                setvisibilityUpdate = false;
            }
            btneditTime.Visible = setvisibilityEdit;
            btnDeleteTask.Visible = setvisibilityEdit;
            lstAllTaskForEdit.Enabled = setvisibilityEdit;
            cmbSchedulforedit.Enabled = setvisibilityEdit;

            btnupdateschedule.Visible = setvisibilityUpdate;
            lblbackuptimeforedit.Visible = setvisibilityUpdate;
            cmbSchedulforedit.Visible = setvisibilityUpdate;
            lblhourFedit.Visible = setvisibilityUpdate;
            cmbhourforedit.Visible = setvisibilityUpdate;
            label10.Visible = setvisibilityUpdate;
            cmbminforedit.Visible = setvisibilityUpdate;
            cmbAMPMforEdit.Visible = setvisibilityUpdate;
            lbluserforedit.Visible = setvisibilityUpdate;
            txtusernameforedit.Visible = setvisibilityUpdate;
            lblpasswordforedit.Visible = setvisibilityUpdate;
            txtpasswordforedit.Visible = setvisibilityUpdate;
        }
        private void btnupdateschedule_Click(object sender, EventArgs e)
        {
            if (txtpasswordforedit.Text != "")
            {
                changeVisibityForedit(1);
                if (General.Util.UI.MyMessageDialogSmall.Show("Are You Sure You Want To Update This Task Schedule ", "Update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bool status = UpdateDaily();
                    lbllistday.Visible = false;
                    cmblday.Visible = false;
                    if (status)
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Successfully Updated");
                    }
                    else
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Error On Updating");
                    }
                }
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("Please enter your password");
            }
        }
        private bool UpdateDaily()
        {
            ScheduledTasks st = new ScheduledTasks();
            Task t = null;
            string freq = "";
            if (cmbSchedulforedit.Text.Contains("Daily"))
            {
                t = st.OpenTask("EHMISDataBaseBackUpDaily");
                freq = "Daily";
                if (t != null)
                {

                    short hour = -999;
                    short min = -999;
                    try
                    {
                        hour = Convert.ToInt16(cmbhour.Text);
                        min = Convert.ToInt16(cmbmin.Text);
                        hour = getAMPM(cmbhour.Text, cmbAMPM.Text);
                        deleteTask(lstAllTaskForEdit.SelectedItem.ToString());
                        CreatScheduleforDaily(hour, min, txtusernameforedit.Text, txtpasswordforedit.Text);
                    }
                    catch
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Please select hour and minute from the list");
                        bool de = st.DeleteTask("EHMISDataBaseBackUpDaily");


                    }
                    t.Close();
                    st.Dispose();
                    return true;
                }
                else
                {
                    General.Util.UI.MyMessageDialogSmall.Show("Task Doesn't Exist");
                    st.Dispose();
                    return false;
                }
            }
            else if (cmbSchedulforedit.Text.Contains("Weekly"))
            {
                t = st.OpenTask("EHMISDataBaseBackUpWeekly");
                freq = "Weekly";
                if (t != null)
                {
                    DaysOfTheWeek day = new DaysOfTheWeek();
                    day = getlday();

                    short hour = -999;
                    short min = -999;
                    try
                    {
                        hour = Convert.ToInt16(cmbhourforedit.Text);
                        min = Convert.ToInt16(cmbminforedit.Text);
                        deleteTask(lstAllTaskForEdit.SelectedItem.ToString());
                        CreatScheduleforWeekly(txtusernameforedit.Text.ToLower(), txtpasswordforedit.Text.ToLower(), day, hour, min);
                    }
                    catch
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Please select hour and minute from the list");
                        bool de = st.DeleteTask("EHMISDataBaseBackUpDaily");


                    }
                    t.Close();
                    st.Dispose();
                    return true;
                }
                else
                {
                    General.Util.UI.MyMessageDialogSmall.Show("Task Doesn't Exist");
                    st.Dispose();
                    return false;
                }
            }
            else if (cmbSchedulforedit.Text.Contains("Monthly"))
            {
                t = st.OpenTask("EHMISDataBaseBackUpMonthly");
                freq = "Weekly";
                if (t != null)
                {
                    short hour = -999;
                    short min = -999;
                    try
                    {
                        hour = Convert.ToInt16(cmbhourforedit.Text);
                        min = Convert.ToInt16(cmbminforedit.Text);
                        deleteTask(lstAllTaskForEdit.SelectedItem.ToString());
                        CreatScheduleforMonthly(txtusernameforedit.Text.ToLower(), txtpasswordforedit.Text.ToLower(), hour, min,"28");
                    }
                    catch
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Please select hour and minute from the list");
                        bool de = st.DeleteTask("EHMISDataBaseBackUpMonthly");
                    }
                    t.Close();
                    st.Dispose();
                    return true;
                }
                else
                {
                    General.Util.UI.MyMessageDialogSmall.Show("Task Doesn't Exist");
                    st.Dispose();
                    return false;
                }
            }
            return false;

        }
        
        private void cmbschedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbschedule.Text != "")
            {
                changeVisibity(cmbschedule.SelectedIndex);
                if (cmbschedule.Text != "Daily")
                {
                    cmbdays.SelectedIndex = 0;
                }
            }
        }
        private DaysOfTheWeek getlday()
        {
            DaysOfTheWeek result = new DaysOfTheWeek();
            if (cmblday.Text == "Monday")
                result = DaysOfTheWeek.Monday;
            if (cmblday.Text == "Tuesday")
                result = DaysOfTheWeek.Tuesday;
            if (cmblday.Text == "Wednesday")
                result = DaysOfTheWeek.Wednesday;
            if (cmblday.Text == "Thursday")
                result = DaysOfTheWeek.Thursday;
            if (cmblday.Text == "Friday")
                result = DaysOfTheWeek.Friday;
            if (cmblday.Text == "Saturday")
                result = DaysOfTheWeek.Saturday;
            if (cmblday.Text == "Sunday")
                result = DaysOfTheWeek.Sunday;

            return result;
        }
        public void changeVisibity(int sender)
        {
            if (sender == 0 )
            {
                // Daily  or Monthly
                lblHour.Visible=true;
                cmbhour.Visible=true;
                lblhsepareter.Visible=true;
                cmbmin.Visible=true;
                cmbAMPM.Visible=true;
                cmbdays.Visible = false ;
                lblday.Visible = false;
                
            }
            else if(sender == 2)
            {
                lblHour.Visible = true;
                cmbhour.Visible = true;
                lblhsepareter.Visible = true;
                cmbmin.Visible = true;
                cmbAMPM.Visible = true;
                cmbdays.Visible = true;
                popDays("Monthly");
                lblday.Visible = true;
            }
            else if (sender == 1)
            {
                // Weekly 
                lblday.Visible = true;
                cmbdays.Visible = true;
                lblHour.Visible = true;
                cmbhour.Visible = true;
                lblhsepareter.Visible = true;
                cmbmin.Visible = true;
                popDays("Weekly");
                cmbAMPM.Visible = true;
            }
            else
            {
                lblday.Visible = false;
                cmbdays.Visible = false;
                cmbdays.Text = "";
                lblHour.Visible = false;
                cmbhour.Visible = false;
                cmbhour.Text = "";
                lblhsepareter.Visible = false;
                cmbmin.Visible = false;
                cmbmin.Text = "";
                cmbAMPM.Visible = false;
                cmbAMPM.Text = "";
                txtpassword.Text = "";
            }
        }
        private void popDays(string Type)
        {
            cmbdays.Items.Clear();
            if (Type == "Weekly")
            {
                this.cmbdays.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});


                this.cmblday.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});
            }
            else if (Type=="Monthly")
            {
                this.cmbdays.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14", 
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
                });
                this.cmblday.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14", 
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
                });
            }
           
        }
        public void CreatScheduleforDaily(short hour, short min, string username, string psw)
        {
            ScheduledTasks st = new ScheduledTasks();
            Task t = null;
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

            string path = Application.StartupPath;
            if (path.Contains("NewEhmisPhemModules"))
            {
                t.ApplicationName = path + @"\backupDeveloper.bat";
            }
            else
            {
                t.ApplicationName = path + @"\backupInstaller.bat";
            }
            try
            {
                t.SetAccountInformation(@username, psw);
                t.MaxRunTime = new TimeSpan(0, 6, 0);
                t.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                t.Comment = "Daily EHMIS DataBase Backup";
                t.Triggers.Add(new DailyTrigger(hour, min));
                t.Save();
                t.Close();
                lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled To Backup Daily");
                lstAllTaskForEdit.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled To Backup Daily");
            }
            catch
            {
                bool de = st.DeleteTask("EHMISDataBaseBackUpDaily");
               // General.Util.UI.MyMessageDialogSmall.Show("Password Is Not Correct");
                errormessage = "Password is not Correct";
                txtpassword.Text = "";
                txtpassword.Focus();
                goto last;
            }

        last:
            {
                st.Dispose();
            }
        }
        private void btnDeleteTask_Click(object sender, EventArgs e)
        {
            if (lstAllTaskForEdit.Text != "")
            {
                if (General.Util.UI.MyMessageDialogSmall.Show("Are you sure you want to delete this task schedule? ", "Deleteing...", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    deleteTask(lstAllTaskForEdit.SelectedItem.ToString());
                    General.Util.UI.MyMessageDialogSmall.Show("Successfully deleted");
                }
            }
            else
            {
                if (lstAllTaskForEdit.Items.Count == 0)
                {
                    General.Util.UI.MyMessageDialogSmall.Show("No task to delete");
                }
                else
                    General.Util.UI.MyMessageDialogSmall.Show("First select from the list");
            }
            
        }
        private void deleteTask(string tasktodelete)
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
            if (delte)
            {

                listoftaskforedit();
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("Task Is Not Deleted");
            }
        }
        public void CreatScheduleforWeekly(string username, string psw, DaysOfTheWeek day, short hour, short min)
        {
            ScheduledTasks st = new ScheduledTasks();
            Task t = null;
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

            string path = Application.StartupPath;

            //string path = Application.StartupPath;
            if (path.Contains("NewEhmisPhemModules"))
            {
                t.ApplicationName = path + @"\backupDeveloper.bat";
            }
            else
            {
                t.ApplicationName = path + @"\backupInstaller.bat";
            }
            try
            {
                t.SetAccountInformation(@username, psw);
                t.MaxRunTime = new TimeSpan(0, 6, 0);
                t.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                t.Comment = "Weekly EHMIS DataBase Backup";
                t.Triggers.Add(new WeeklyTrigger(hour, min, day));
                t.Save();
                t.Close();
                lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Weekly");
                lstAllTaskForEdit.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Weekly");
            }
            catch
            {
                bool de = st.DeleteTask("EHMISDataBaseBackUpWeekly");
                //General.Util.UI.MyMessageDialogSmall.Show("Password Is Not Correct");
                errormessage = "Password is not correct";
                txtpassword.Text = "";
                txtpassword.Focus();
                goto last;
            }

        last:
            {
                st.Dispose();
            }
       
        }
        public void CreatScheduleforMonthly(string username, string psw, short hour, short min,string days)
        {
            ScheduledTasks st = new ScheduledTasks();
            Task t = null;
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

            string path = Application.StartupPath;
            if (path.Contains("Ver1-EthiopiaMerge-Jan15-2010"))
            {
                t.ApplicationName = path + @"\backupDeveloper.bat";
            }
            else
            {
                t.ApplicationName = path + @"\backupInstaller.bat";
            }
            try
            {
                t.SetAccountInformation(@username, psw);
                t.MaxRunTime = new TimeSpan(0, 6, 0);
                t.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                t.Comment = "Monthly EHMIS DataBase Backup";
                //checkMonth(days);
                int dys = Convert.ToInt16(days);
                int[] DaysOfMonth =  checkMonth(days);//{ dys, dys, dys, dys, dys, dys, dys, dys, dys, dys, dys, dys }; 

                t.Triggers.Add(new MonthlyTrigger(hour, min, DaysOfMonth));
                t.Save();
                t.Close();
                lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Monthly");
                lstAllTaskForEdit.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Monthly");
            }
            catch
            {
                bool de = st.DeleteTask("EHMISDataBaseBackUpMonthly");
                //General.Util.UI.MyMessageDialogSmall.Show("Password Is Not Correct");
                errormessage = "Password is not correct";
                txtpassword.Text = "";
                txtpassword.Focus();
                goto last;
            }

        last:
            {
                st.Dispose();
            }
       
        }
        public int[] checkMonth(string dys)
        {
            int selectday = Convert.ToInt16(dys);
            int[] days ={ selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday, selectday };
            
            if (selectday > 28 && selectday<30)
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
        private DaysOfTheWeek getday()
        {
            DaysOfTheWeek result = new DaysOfTheWeek();
            if(cmbdays.Text=="Monday")
              result = DaysOfTheWeek.Monday;
            if(cmbdays.Text=="Tuesday")
              result = DaysOfTheWeek.Tuesday;
            if(cmbdays.Text=="Wednesday")
              result = DaysOfTheWeek.Wednesday;
            if(cmbdays.Text=="Thursday")
              result = DaysOfTheWeek.Thursday;
            if(cmbdays.Text=="Friday")
              result = DaysOfTheWeek.Friday;
            if(cmbdays.Text=="Saturday")
              result = DaysOfTheWeek.Saturday;
            if(cmbdays.Text=="Sunday")
              result = DaysOfTheWeek.Sunday;

          return result;
        }

        private void cmbSchedulforedit_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cmbSchedulforedit.SelectedIndex == 0 || cmbSchedulforedit.SelectedIndex == 2)
            {
                // Daily  
                lblHour.Visible=true;
                cmbhour.Visible=true;
                lblhsepareter.Visible=true;
                cmbmin.Visible=true;
                cmbAMPM.Visible=true;

                lblday.Visible = false;
                cmbdays.Visible = false;
            }
            else if (cmbSchedulforedit.SelectedIndex == 1)
            { 
               // Weekly or Monthly
                lblday.Visible=true;
                cmbdays.Visible=true;
                lblHour.Visible=true;
                cmbhour.Visible=true;
                lblhsepareter.Visible=true;
                cmbmin.Visible=true;
                cmbAMPM.Visible = true;
            }
        }
        private void setColoring(Object selectedBtn)
        {
            this.btnbackupdb.BackColor = System.Drawing.Color.LightYellow;
            this.btnrestordb.BackColor = System.Drawing.Color.LightYellow;
            this.btnsetbackuptime.BackColor = System.Drawing.Color.LightYellow;
            this.btnvieweditschedule.BackColor = System.Drawing.Color.LightYellow;

            if (selectedBtn != null)
            {
                Button btn = (Button)selectedBtn;
                btn.BackColor = System.Drawing.Color.LightSteelBlue;
            }
        }

        private void btnstrrestoring_Click(object sender, EventArgs e)
        {

            if ((txtrestorelocation.Text != "") && CloseeHMIS())
            {
                OpenFileDialog opdilg = new OpenFileDialog();
                opdilg.FileName = "*.bak";
                opdilg.Title = "Select eHMIS backup file";
                opdilg.Filter = "eHMIS Backup files (*.bak)|*.bak";
                string baklocation="";
                string DBbackuplocation = BackUpAndRestore.Program.ApplicationStartupPath + "\\HMISBackUpDB";
                if (opdilg.ShowDialog() == DialogResult.OK)
                { 
                   baklocation=opdilg.FileName.ToString();
                   string locationDB ="eHMIS";
                   displaymsg("Please Wait Old Database is Backing Up....");

                   dobackup(cmblistofdbForrestor.Text, @DBbackuplocation + "\\" + cmblistofdbForrestor.Text + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".bak");
                   frm.Dispose();

                   myconn.Close();
                   Con();
                   displaymsg("DataBase is Restoring....");
                   string cmdtext2 = "drop database eHMIS";
                   DataTable _dt = new DataTable();
                   DataSet _ds = new DataSet();
                   SqlDataAdapter da = new SqlDataAdapter(cmdtext2, myconn);
                   da.Fill(_ds);


                   
                   dorestorwithMove("eHMIS", baklocation, txtrestorelocation.Text+"\\eHMIS");
                   frm.Dispose();
                   General.Util.UI.MyMessageDialogSmall.Show("Successfully Completed");
                }

                
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show(ErrorMessageRestor);
            }
        }
        private bool CloseeHMIS()
        {
            if (General.Util.UI.MyMessageDialogSmall.Show("The eHMIS application needs to close to restore the database. All data on the current applicatoin will be changed with the new database data. click yes to continue", "Closing eHMIS...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Upgrader ug = new Upgrader();
                ug.FindAndKillProcess("MainApp");
                return true;
            }
            ErrorMessageRestor = "Restoring has been cancelled.";
            return false;
        }
        private void btnbrowseReLocation_Click(object sender, EventArgs e)
        {
            txtrestorelocation.Text = RestoreLocation();
        }

        private void btneditTimeNew_Click(object sender, EventArgs e)
        {
            if (lstTaskList.Text != ""&& lstTaskList.SelectedIndex!=null)
            {
                btneditClick(btneditTimeNew.Text);
            }

          
            else
                MessageBox.Show("First select from the list");
            
        }
        private void btneditClick(string btntxt)
        {
            if (btntxt == "Edit Selected Task")
            {
                selectedTask = lstTaskList.SelectedItem.ToString();
                if (lstTaskList.Text != "")
                {
                    btnDeleteNew.Enabled = false;
                    btnadd.Enabled = false;
                    if (lstTaskList.SelectedItem.ToString().Contains("Daily"))
                    {
                        changeVisibity(0);
                        cmbschedule.SelectedIndex = 0;
                        lblday.Visible = false;
                        cmbdays.Visible = false;
                    }
                    else if (lstTaskList.SelectedItem.ToString().Contains("Weekly"))
                    {
                        changeVisibity(1);
                        cmbschedule.SelectedIndex = 1;
                        lbllistday.Visible = true;
                        cmblday.Visible = true;

                    }
                    else if (lstTaskList.SelectedItem.ToString().Contains("Monthly"))
                    {
                        changeVisibity(1);
                        cmbschedule.SelectedIndex = 2;

                        lbllistday.Visible = true;
                        cmblday.Visible = true;
                    }
                    cmbschedule.Enabled = false;
                    btneditTimeNew.Text = "Update";
                    //tabControl1.SelectTab(4); // Enabled = false;
                }
                else
                {
                    if (lstTaskList.Items.Count == 0)
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("No task to edit");
                    }
                    else
                        General.Util.UI.MyMessageDialogSmall.Show("Before you click, select from the list");
                }
            }
            else if (btntxt == "Update")
            {
                if (txtpassword.Text != "")
                {
                   
                    if (General.Util.UI.MyMessageDialogSmall.Show("Are You Sure You Want To Update This Task Schedule ", "Update", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        deleteTask(selectedTask);
                        // deleteTask(lstTaskList.SelectedItem.ToString());
                        this.btnadd_Click(btnsetbackuptime, EventArgs.Empty);
                        btneditTimeNew.Text = "Edit Selected Task";
                        btnDeleteNew.Enabled = true;
                        btnadd.Enabled = true;
                        cmbschedule.Enabled = true;
                        cmbschedule.Text = "";
                    }
                }
                else
                {
                    General.Util.UI.MyMessageDialogSmall.Show("Please enter your password");
                }
            }
        }
        private void btnDeleteNew_Click(object sender, EventArgs e)
        {
            if (lstTaskList.Text != "")
            {
                if (General.Util.UI.MyMessageDialogSmall.Show("Are you sure you want to delete this task schedule? ", "Deleteing...", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    deleteTask(lstTaskList.SelectedItem.ToString());
                    General.Util.UI.MyMessageDialogSmall.Show("Successfully deleted");
                }
            }
            else
            {
                if (lstTaskList.Items.Count == 0)
                {
                    General.Util.UI.MyMessageDialogSmall.Show("No task to delete");
                }
                else
                    General.Util.UI.MyMessageDialogSmall.Show("First select from the list");
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btneditTimeNew.Text == "Update")
            {
                
                tabControl1.SelectedIndex = 2;
                btnadd.Enabled = false;
            }
           
            popschedullist();
        }

        private void txtpassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (btneditTimeNew.Text == "Update")
                {
                    btneditTimeNew.PerformClick();
                    e.Handled = true;
                }
                else
                {
                    btnadd.PerformClick();
                    e.Handled = true;
                }
            }
        }
        public void popschedullist()
        {
            lstTaskList.Items.Clear();
            ScheduledTasks st = new ScheduledTasks();
            string[] tasklist = st.GetTaskNames();
            for (int i = 0; i < tasklist.Length; i++)
            {
                if (tasklist[i].ToString() == "EHMISDataBaseBackUpDaily.job")
                {
                    lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Daily");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpWeekly.job")
                {
                    lstTaskList.Items.Add(cmblistdbtoschedule.Text + " DataBase Scheduled to Backup Weekly");
                }
                else if (tasklist[i].ToString() == "EHMISDataBaseBackUpMonthly.job")
                {
                    lstTaskList.Items.Add(cmblistdbtoschedule.Text + " Database Scheduled to Backup Monthly");
                }
            }
            //if (lstTaskList.Items.Count == 3)
            //{
            //    btnadd.Enabled = false;
            //}
            //else
            //    btnadd.Enabled = true;
        }
    }

}
