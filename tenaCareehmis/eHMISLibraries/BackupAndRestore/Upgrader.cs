
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Net.Sockets;
using System.Collections;
using System.Data;

using SqlManagement.Database;
using Admin.Communication.Email;
using ICSharpCode.SharpZipLib.Zip;
using UtilitiesNew.GeneralUtilities;
using eHMIS;


namespace BackUpAndRestore
{
    public partial class Upgrader : BaseForm
    {
        private Form frm;
        private string selectedEmailFilename = "";
        private string emailID = "";
        private static string name = "Root";
        public static string dir = "";
       
        public Upgrader()
        {
            InitializeComponent();
        }

        private void Upgrader_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "";
            reLoadMail();
            //if (lblInfo.Text == "")
            //{
            //    General.Util.UI.MyMessageDialogSmall.Show("No Update Please Try Later");
            //}
        }

        private void btncheckUpdate2_Click(object sender, EventArgs e)
        {
            UpgradeDownload ud = new UpgradeDownload(this);
            ud.ShowDialog();
        }

        private void btncheckUpdate_Click(object sender, EventArgs e)
        {
            if (General.Util.UI.MyMessageDialogSmall.Show("Checking from upgrade may take time are you sure you want to upgrade?", "Upgrading", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lblInfo.Text = "";
                sendDownLoadRequest();
                //download the emails
                //string appPath = Directory.GetCurrentDirectory();
                string appPath = Application.StartupPath;
                string appPath2 = appPath.Substring(0, appPath.LastIndexOf('\\'));
                FindAndKillProcess("SMSService");


                if (System.IO.File.Exists(appPath2 + @"\Runtime\SMSService\SMSService.exe"))
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(appPath2 + @"\Runtime\SMSService\SMSService.exe");
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;

                    System.Diagnostics.Process.Start(psi);
                }
                else if (System.IO.File.Exists(appPath2 + @"\RuntimeDirectory\SMSService\SMSService.exe"))
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(appPath2 + @"\RuntimeDirectory\SMSService\SMSService.exe");
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;

                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    string newAppStartPath = appPath2;

                    for (int i = 0; i < 2; i++)
                    {
                        newAppStartPath = newAppStartPath.Substring(0, newAppStartPath.LastIndexOf('\\'));
                    }

                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(newAppStartPath + @"\RuntimeDirectory\SMSService\SMSService.exe");
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;

                    System.Diagnostics.Process.Start(psi);
                }

                //if (System.IO.File.Exists(appPath2 + @"\Runtime\SMSService\SMSService.exe"))
                //{
                //    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(appPath2 + @"\Runtime\SMSService\SMSService.exe");
                //    psi.UseShellExecute = false;
                //    psi.CreateNoWindow = true;
                //    psi.RedirectStandardOutput = true;

                //    System.Diagnostics.Process.Start(psi);
                //}
                int count = 0;
                Cursor.Current = Cursors.WaitCursor;
                Stopwatch measureElapsedtime = new Stopwatch();
                measureElapsedtime.Start();
                              
                bool mailLoaded = false;
                SplashScreen.Show("", "", null, 1800);

                while (FindProcess("SMSService"))
                {
                    mailLoaded = reLoadMail();
                    if (mailLoaded == true)
                    {
                        mailLoaded = true;
                        break;
                    }
                    //if (lblInfo.Text == "" && count > 499999)
                    //{
                    //    //General.Util.UI.MyMessageDialogSmall.Show("No Update Please Try Later");
                    //    break;
                    //}
                    //count = count + 1;

                    //if (stopWatch.ElapsedMilliseconds == 1000)
                    //{
                    long elTime = (measureElapsedtime.ElapsedMilliseconds / 1000);

                    int percentage = int.Parse(elTime.ToString());

                    string msgProgress = " Progress : " + (measureElapsedtime.ElapsedMilliseconds/1000) + " Seconds from Remaining 1800 Seconds";
                    SplashScreen.SetProgress(percentage);                    
                    SplashScreen.SetProgressText(msgProgress);

                    // }

                    if (SplashScreen.splashEnded == true)
                    {
                        break;
                    }

                    if (measureElapsedtime.ElapsedMilliseconds == 1800000)
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("No Update Please Try Later");
                        measureElapsedtime.Stop();

                        //SplashScreen.SetProgress(count);
                        //SplashScreen.SetProgressText(msgProgress);

                        break;
                    }
                }

                if (mailLoaded == false)
                {
                    General.Util.UI.MyMessageDialogSmall.Show("No Update Please Try Later");
                }               

                if (SplashScreen.splashEnded == false)
                {
                    SplashScreen.SetButtonText("OK");
                    SplashScreen.End();
                }

                //if (lblInfo.Text == "")
                //{
                //    General.Util.UI.MyMessageDialogSmall.Show("No Update Please Try Later");
                //}
                Cursor.Current = Cursors.Arrow;
            }
        }

        //private bool getPOPInformation()
        //{
        //    DBConnHelper DBConnHelper = new DBConnHelper();
        //    string sqlStrPOPConfig = "Select * From eth_POPConfig";
        //    string sqlStrEMailAccount = "Select * From eth_EMailAddress";

        //    DataSet dsPop = DBConnHelper.GetDataSet(sqlStrPOPConfig);
        //    DataSet dsEMailAccount = DBConnHelper.GetDataSet(sqlStrEMailAccount);

        //    if(dsPop.Tables[0].Rows.Count < = 0 || 
        //       dsEMailAccount.Tables[0].Rows< = 0 ||
        //       dsPop.Tables[0].Rows[0]["POP3Server"].ToString()  == "" || 
        //       dsPop.Tables[0].Rows[0]["Port"].ToString()  == "" ||
        //       dsPop.Tables[0].Rows[0]["MaxDownloadEmails"].ToString()  == "" ||
        //       dsPop.Tables[0].Rows[0]["MaxDownloadEmails"].ToString()  == "" ||

        //}

        private void sendDownLoadRequest()
        {
            DBConnHelper DBConnHelper = new DBConnHelper();
            //OriginalUser currentUser = (OriginalUser)CCPApp.EntityManager.CurrentUser;

            string sqlStr = "INSERT INTO [eth_EmailDownloadRequest]" +
                            " ([RequestedBy]" +
                            ",[RequestDateTime]" +
                            ",[Requested])" +
                            "VALUES" +
                            "(''" +
                            ",'" + DateTime.Now.Date.ToUniversalTime() + "'" +
                            ",1)";

            if (DBConnHelper.Execute(sqlStr) == 1)
            {
                General.Util.UI.MyMessageDialogSmall.Show("Your request has been sent to the server successfully. The server will download your E-Mails and send you a message shortly.", "System update ...");
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("The request is not successful please try again later.", "System update ...");
            }
        }

        private void Upgrade_Load(object sender, EventArgs e)
        {
            reLoadMail();
        }
        public bool reLoadMail()
        {
            bool mailLoaded = false;

            EMailControler mail = new EMailControler();
            DataTable tbl = mail.getMailsForUpgarade();
            //[MailID],[DeliveryDate],[FromAddress],[ToAddress],[Subject],[Body]
            if (tbl.Rows.Count > 0)
            {
                if (tbl.Rows[0]["Subject"].ToString() == "UPGRADE;")
                {
                    emailID = tbl.Rows[0]["MailID"].ToString();
                    lblInfo.Text = tbl.Rows[0]["Subject"].ToString() + " " + tbl.Rows[0]["DeliveryDate"].ToString();
                    btnUpgrade.Visible = true;
                    btncheckUpdate.Visible = false;
                    mailLoaded = true;
                    return mailLoaded;
                }
            }

            return mailLoaded;
        }
        public void stratUpgrade(string filelocation)
        {
            EMailControler mail = new EMailControler();
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(@filelocation);
           // System.Diagnostics.Process.Start(p);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.StartInfo = p;

            proc.Start();

           // General.Util.UI.MyMessageDialogSmall.Show(emailID);
            //mail.setImportedDataFlag(emailID);

            mail.setImportedDataFlag();

            FindAndKillProcess("BackUpAndRestore");
            //p.WaitForExit();
          //  proc.WaitForExit();
            //System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(upgradedir, "Update");

            //System.Diagnostics.Process.Start(psi);

        }
        public bool FindAndKillProcess(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.ToString()==name)
                {
                    clsProcess.Kill();
                    //process found, return true
                    return true;

                }
            }
            //process not found, return false
            return false;
        }
        public bool downloadedFile()
        {

            this.selectedEmailFilename = @"UPGRADE\" + emailID + ".zip";
            EMailControler mail = new EMailControler();
            string subject = "";
            mail.creatFile(emailID, out subject);
            return true;
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
           
            if (General.Util.UI.MyMessageDialogSmall.Show("This will close eHMIS and start upgrading. Are uou sure you want to upgrade now?", "Are you sure you want to upgrade now?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                string RuntimeDirectoryBackup="";
                string RuntimeBackup="";
                FindAndKillProcess("MainApp");
                downloadedFile();// this will download update zip file into HMISUPGRADE folder
                FastZip FilUnzi = new FastZip();
                string currentDir = Directory.GetCurrentDirectory();
                string UpgradFileLocationForDeveloper = "";
                string UpgradFileLocationForInstalledSmartCare = "";
                string downoldfileLocation = currentDir+"\\HMISData";
                if (currentDir.Contains("NewEhmisPhemModules"))
                {
                    //Means it is for developer 
                    FindAndKillProcess("MainApp");
                    string cdri = BackUpAndRestore.Program.ApplicationStartupPath;
                    UpgradFileLocationForDeveloper = BackUpAndRestore.Program.ApplicationStartupPath + "\\HMISUPGRADE";
                    string runttimepath = cdri.Substring(0, cdri.LastIndexOf('\\'));
                    Form1 frm1 = new Form1();
                   // displaymsg("Taking Backup before Upgrading...");
                    backupruntimeandDB(runttimepath,"Developer");
                   // frm.Dispose();
                    if (!System.IO.Directory.Exists(UpgradFileLocationForDeveloper))
                    {
                        System.IO.Directory.CreateDirectory(UpgradFileLocationForDeveloper);
                    }                    
                    //downoldfileLocation = eHMIS.Program.ApplicationStartupPath + "\\HMISData\\" + emailID + ".zip";//.Substring(0, currentDir.LastIndexOf('\\'));//this will move out from current dirrectort to \\Ver1-EthiopiaMerge-Jan15-2010
                    downoldfileLocation = AppDomain.CurrentDomain.BaseDirectory +"HMISData\\" + emailID + ".zip";//.Substring(0, currentDir.LastIndexOf('\\'));//this will move out from current dirrectort to \\Ver1-EthiopiaMerge-Jan15-2010

                }
                else
                { 
                    //Means smartcare installed
                    FindAndKillProcess("MainApp");
                    UpgradFileLocationForInstalledSmartCare = currentDir;
                    UpgradFileLocationForInstalledSmartCare = BackUpAndRestore.Program.ApplicationStartupPath + "\\HMISUPGRADE";
                    string cdri = BackUpAndRestore.Program.ApplicationStartupPath;
                    string runttimepath = cdri.Substring(0, cdri.LastIndexOf('\\'));

                   // displaymsg("Taking Backup before Upgrading...");

                    backupruntimeandDB(runttimepath, "InstalledSmartcare");
                   
                    //frm.Dispose();
                    if (!System.IO.Directory.Exists(UpgradFileLocationForInstalledSmartCare))
                    {
                        System.IO.Directory.CreateDirectory(UpgradFileLocationForInstalledSmartCare);
                    }
                    downoldfileLocation = BackUpAndRestore.Program.ApplicationStartupPath + "\\HMISData\\" + emailID + ".zip";
                  //  General.Util.UI.MyMessageDialogSmall.Show("Installer");
                }
              
                try
                {
                    FindAndKillProcess("MainApp");
                    displaymsg("Extracting Upgrade File...");
                    FilUnzi.ExtractZip(downoldfileLocation, UpgradFileLocationForInstalledSmartCare, "");
                    frm.Dispose();
                    string path = UpgradFileLocationForInstalledSmartCare + @"\hmisUpgradeStarter.bat";
                    stratUpgrade(path);
                  //  General.Util.UI.MyMessageDialogSmall.Show("Unzip locatin is " + UpgradFileLocationForInstalledSmartCare);
                    
                }
                catch
                {
                    displaymsg("Extracting Upgrade File...");
                    FilUnzi.ExtractZip(downoldfileLocation, UpgradFileLocationForDeveloper, "");
                    frm.Dispose();
                    string path = UpgradFileLocationForDeveloper + @"\hmisUpgradeStarter.bat";

                    stratUpgrade(path);
                    //General.Util.UI.MyMessageDialogSmall.Show("Unzip locatin is " + UpgradFileLocationForDeveloper);
                  
                }
                
            }
            General.Util.UI.MyMessageDialogSmall.Show(" You Can Start Working Again");
            this.Dispose();

        }
        public bool FindProcess(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.ToString() == name)
                {
                   return true;
                }
            }
            return false;
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
        public void StartSmartCare(string SmartcarePath)
        {
            string runttimepath = "";
            if (SmartcarePath.Contains("NewEhmisPhemModules"))
            {
                // means developer 
                //General.Util.UI.MyMessageDialogSmall.Show(SmartcarePath);
                runttimepath = SmartcarePath.Substring(0, SmartcarePath.LastIndexOf('\\'));
                runttimepath = runttimepath + "\\RuntimeDirectory";
            }
            else
            {
                // installed smartcare
               // General.Util.UI.MyMessageDialogSmall.Show(SmartcarePath);
                runttimepath = SmartcarePath.Substring(0, SmartcarePath.LastIndexOf('\\'));
                runttimepath = runttimepath + "\\Runtime";

            }
            runttimepath = runttimepath + @"\MainApp.exe";
           
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(@runttimepath);
            // System.Diagnostics.Process.Start(p);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = p;
            proc.Start();
        }
        private void backupruntimeandDB(string path,string WorkingArea)
        {
            string InPath = path;
            Form1 frm1 = new Form1();
            string runtimepath = "";
            if (WorkingArea == "Developer")
            {
                runtimepath = "RuntimeDirectory";
            }
            else if (WorkingArea == "InstalledSmartcare")
            {
                runtimepath = "Runtime";
            }
            string backupdir = path + "\\" + runtimepath + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
            if (!System.IO.Directory.Exists(backupdir))
            {
                System.IO.Directory.CreateDirectory(backupdir);
            }
            string sourcefolder=InPath + "\\" + runtimepath;
            string desfolder = backupdir;
            //string DBbackuplocation = eHMIS.Program.ApplicationStartupPath + "\\HMISBackUpDB";
            string DBbackuplocation = AppDomain.CurrentDomain.BaseDirectory +"HMISBackUpDB";
            
            if (!System.IO.Directory.Exists(DBbackuplocation))
            {
                System.IO.Directory.CreateDirectory(DBbackuplocation);
            }
            try
            {
                DBbackuplocation = DBbackuplocation + "\\eHMIS" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".bak";
               displaymsg("Taking database backup before upgrading...");
               frm1.dobackup("eHMIS", DBbackuplocation);
               frm.Dispose();
            }
            catch(ArgumentException error)
            {
                General.Util.UI.MyMessageDialogSmall.Show("Can't backup dataBase because " + error.ToString());
                frm.Dispose();
            }
            displaymsg("Taking runtime directory backup before upgrading...");
            CopyDirectory(sourcefolder, desfolder, true);
            frm.Dispose();

        }
       private static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)  
       {  
           bool ret = false;  
           try  
           {  
               SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";  
               DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";  
  
               if (Directory.Exists(SourcePath))  
               {  
                   if (Directory.Exists(DestinationPath) == false)  
                       Directory.CreateDirectory(DestinationPath);  
  
                   foreach (string fls in Directory.GetFiles(SourcePath))  
                   {  
                       FileInfo flinfo = new FileInfo(fls);  
                       flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);  
                   }  
                   foreach (string drs in Directory.GetDirectories(SourcePath))  
                   {  
                       DirectoryInfo drinfo = new DirectoryInfo(drs);  
                       if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)  
                           ret = false;  
                   }  
               }  
               ret = true;  
           }  
           catch (Exception ex)  
           {
               General.Util.UI.MyMessageDialogSmall.Show(ex.ToString());
               ret = false;  
           }  
           return ret;  
       }

        private void btndiskupdate_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog UpdateLocation = new FolderBrowserDialog();
            Form1 frm1 = new Form1();
            UpdateLocation.ShowDialog();
            string updatefolder = UpdateLocation.SelectedPath.ToString();
            if (updatefolder != "")
            {
                if (System.IO.File.Exists(updatefolder + @"\hmisUpgradeStarter.bat"))
                {
                    // string runtimepath = eHMIS.Program.ApplicationStartupPath.ToString();
                    string runtimepath = AppDomain.CurrentDomain.BaseDirectory;

                    runtimepath = runtimepath.Substring(0, runtimepath.LastIndexOf('\\')) + "\\runtime";
                    string backupdir = runtimepath + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
                    if (!System.IO.Directory.Exists(backupdir))
                    {
                        System.IO.Directory.CreateDirectory(backupdir);
                    }
                    string sourcefolder = runtimepath;
                    string desfolder = backupdir;


                    displaymsg("Taking runtime directory backup before upgrading...");
                    CopyDirectory(sourcefolder, desfolder, true);
                    frm.Dispose();
                    //string DBbackuplocation =eHMIS.Program.ApplicationStartupPath + "\\HMISBackUpDB";
                    string DBbackuplocation = AppDomain.CurrentDomain.BaseDirectory + "HMISBackUpDB";

                    if (!System.IO.Directory.Exists(DBbackuplocation))
                    {
                        ;
                        System.IO.Directory.CreateDirectory(DBbackuplocation);
                    }
                    try
                    {
                        DBbackuplocation = DBbackuplocation + "\\eHMIS" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".bak";
                        displaymsg("Taking dataBase backup before upgrading...");
                        frm1.dobackup("eHMIS", DBbackuplocation);
                        frm.Dispose();

                        string path = updatefolder + @"\hmisUpgradeStarter.bat";

                        FindAndKillProcess("MainApp");


                        System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(@path);

                        System.Diagnostics.Process proc = new System.Diagnostics.Process();

                        proc.StartInfo = p;

                        proc.Start();


                        General.Util.UI.MyMessageDialogSmall.Show("Upgraded successfully. You can start to use eHMIS");
                        FindAndKillProcess("BackUpAndRestore");

                    }
                    catch (ArgumentException error)
                    {
                        General.Util.UI.MyMessageDialogSmall.Show("Can't backup dataBase because " + error.ToString());
                        frm.Dispose();
                    }


                }
                else
                {
                    General.Util.UI.MyMessageDialogSmall.Show("Update file not found. Please select the appropriate folder");
                }
            }
        }

        private void Upgrader_ResizeBegin(object sender, EventArgs e)
        {
            //this.Size = new System.Drawing.Size(427, 229);
        }

        private void Upgrader_ResizeEnd(object sender, EventArgs e)
        {
           // this.Size = new System.Drawing.Size(427, 229);
        } 
    }
}
