using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using SqlManagement.Database;
using System.Threading;
using Admin.Communication;
using UtilitiesNew.GeneralUtilities;
using SMSService;
using System.Web;
using System.Net.Mail;
using General.Util.UI;
using System.Security.Cryptography;
using eHMIS;

namespace BackUpAndRestore
{
    public partial class UpgradeDownload : BaseForm
    {
        private Upgrader upgraderWindow;

        const string INITIAL_CAPTION = "Download Update";

        private SMSService.Dialer dialer = new SMSService.Dialer();
        private bool dialerActivated = false;

        private TextAnimator displayAnimator;
        private Thread backgroundDownloader; // attempts to connect to the internet and download in a different thread

        public UpgradeDownload()
        {
            InitializeComponent();
            displayAnimator = new TextAnimator(txtStatus);
        }

        public UpgradeDownload(Upgrader upgrader)
            : this()
        {
            upgraderWindow = upgrader;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            btnDownload.Enabled = false;

            backgroundDownloader = new Thread(BackgroundDownloader);
            backgroundDownloader.Start();
        }

        private void BackgroundDownloader()
        {
            dialerActivated = false;

            try
            {
                AppendAnimate("Checking for internet connection", txtStatus);

                if (!dialer.IsConnectedToInternet)
                {
                    Append("Connection not found", txtStatus);

                    dialer.Connect();
                    dialerActivated = true;

                    AppendAnimate("Attempting to connect using dial up", txtStatus);

                    // Wait for dialer to connect
                    while (!dialer.IsConnectedToInternet)
                    {
                    }

                    Append("Connected", txtStatus);
                }
                else
                {
                    Append("Existing Internet connection detected", txtStatus);
                }

                DataTable configData = getConfiguration();

                if (configData.Rows.Count == 0)
                {
                    MyMessageDialogSmall.Show("The POP3 server is not configured. Please inform the system administrator.");
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                string pop3Server = configData.Rows[0][0].ToString();
                int pop3Port = (int)configData.Rows[0][1];
                bool ssl = (bool)configData.Rows[0][3];

                Pop3MimeClient pop = new Pop3MimeClient(pop3Server, pop3Port, ssl, "ehmisupdate@tena.net.et", "updater");
                EMailControler mail = new EMailControler();

                AppendAnimate("Attempting to connect to upgrade email", txtStatus);

                try
                {
                    pop.Connect();
                    Append("Upgrade email found", txtStatus);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Invalid login"))
                    {
                        MyMessageDialogSmall.Show("Authentication failure. Upgrade E-Mail address and password may have been changed.");
                        this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                        return;
                    }

                    throw ex;
                }

                //get mailbox stats
                int numberOfMailsInMailbox, mailboxSize;
                pop.GetMailboxStats(out numberOfMailsInMailbox, out mailboxSize);

                if (numberOfMailsInMailbox == 0)
                {
                    MyMessageDialogSmall.Show("No upgrade found. Please try later.");
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                AppendAnimate("Attempting to retrieve upgrade email", txtStatus);

                RxMailMessage mm = null;

                pop.GetEmail(numberOfMailsInMailbox, out mm);

                pop.Disconnect();

                string subject;
                if (mm == null)
                {
                    MyMessageDialogSmall.Show("Attempt to get upgrade email returned null. Please contact your administrator.");
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                Append("Upgrade email retrieved", txtStatus);

                subject = mm.Subject;
                DateTime deliveryDate = mm.DeliveryDate;

                if (mm.Attachments.Count == 0 || !subject.StartsWith("UPGRADE"))
                {
                    MyMessageDialogSmall.Show("Upgrade email doesnot have required attachment or proper subject. Please contact your administrator.");
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                StreamReader sr = new StreamReader(mm.Attachments[0].ContentStream);
                string[] lines = sr.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 2)
                {
                    MyMessageDialogSmall.Show("Upgrade email does not contain complete information. Please contact your administrator");
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                string updateName = lines[0];
                string downloadUrl = lines[1];
                string md5 = "";
                if (lines.Length > 2)
                    md5 = lines[2];

                SetEmail(mail, mm);

                if (EMailControler.isMailExist(mail.EMailID))
                {
                    string message = "The latest upgrade has already been downloaded";

                    if (EMailControler.isMailImported(mail.EMailID))
                        message += " and applied";
                    else
                        message += ". You can upgrade by clicking 'Upgrade' in upgrader window";

                    upgraderWindow.Invoke(new MethodInvoker(delegate { upgraderWindow.reLoadMail(); }));

                    MyMessageDialogSmall.Show(message);
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                AppendAnimate("Attempting to download", txtStatus);

                ResumeDownload(updateName, downloadUrl, md5, mail);
            }
            catch (Exception e)
            {
                if (e is ThreadAbortException)
                    return;

                MyMessageDialogSmall.Show("The following error occured: " + e.Message);
                this.Invoke(new MethodInvoker(delegate { this.Close(); }));
            }
        }

        private static void SetEmail(EMailControler mail, RxMailMessage mm)
        {
            if (mm.To.Count > 0)
            {
                mail.To = mm.To[0].Address;
            }

            mail.Sender = mm.From.Address;
            mail.Subject = mm.Subject;

            if (!mm.IsBodyHtml && !string.IsNullOrEmpty(mm.Body))
            {
                mail.Message = mm.Body;
            }
            else if (string.Compare(mm.ContentType.MediaType, "multipart/mixed", true) == 0)
            {
                mail.Message = mm.Body;
            }
            else
            {
                foreach (AlternateView av in mm.AlternateViews)
                {
                    // check for plain text
                    if (string.Compare(av.ContentType.MediaType, "text/plain", true) == 0)
                        mail.Message = EMailControler.StreamToString(av.ContentStream);

                    // check for HTML text
                    else if (string.Compare(av.ContentType.MediaType, "text/html", true) == 0)
                        mail.Message = EMailControler.StreamToString(av.ContentStream);
                }
            }
            mail.SentData = mm.DeliveryDate;
            if (mm.MessageId != null)
            {
                mail.EMailID = mm.MessageId.Replace("<", "").Replace(">", "");
            }
        }

        private void AppendAnimate(string text, Control c)
        {
            Append(text, c, true);
        }

        private void Append(string text, Control c)
        {
            Append(text, c, false);
        }

        private void Append(string text, Control c, bool animate)
        {
            displayAnimator.Stop();

            c.Invoke(new MethodInvoker(delegate
            {
                if (c.Text == "")
                {
                    c.Text = text;
                }
                else
                {
                    c.Text += Environment.NewLine + text;
                }
            }
            ));

            if(animate)
                displayAnimator.Start();
        }

        private void ResumeDownload(string updateName, string url, string md5, EMailControler mail)
        {
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Temporary Update Files"));
            di.Create();
            FileInfo[] files = di.GetFiles("*.part");

            if (files.Length > 1)
            {
                try
                {
                    foreach (FileInfo fi in files)
                        fi.Delete();
                }
                catch (Exception e)
                {
                    MyMessageDialogSmall.Show("Unable to delete inconsistent downloaded parts. Error was: " + e.Message);
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                Append("Inconsistent downloaded parts were found and deleted. Upgrade must be downloaded again.", txtStatus);
            }

            string tempFile = HttpUtility.UrlEncode(updateName) + ".part";

            if (files.Length == 1)
            {
                if (tempFile != files[0].Name)
                {
                    Append("Partially downloaded file was found. But is a different file from current update file. It will be deleted.", txtStatus);
                    try
                    {
                        files[0].Delete();
                    }
                    catch (Exception e)
                    {
                        MyMessageDialogSmall.Show("Unable to delete outdated downloaded part. Error was: " + e.Message);
                        this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                        return;
                    }
                }
            }

            FileMode mode = FileMode.Append;

            string fullTempFile = Path.Combine(di.FullName, tempFile);
            if (!File.Exists(fullTempFile))
                mode = FileMode.Create;

            long total;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    total = response.ContentLength;
                }
            }
            catch (WebException)
            {
                total = -1;
            }

            if (total <= 0)
            {
                General.Util.UI.MyMessageDialogSmall.Show("Upgrade not found at stated location. It may have been removed. Please contact your administrator.");
                this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                return;
            }

            int soFar = 0;
            if (File.Exists(fullTempFile))
            {
                using (FileStream fs = File.Open(fullTempFile, FileMode.Open))
                {
                    soFar = (int)fs.Length;
                }
            }

            if (soFar < total)
            {
                using (FileStream fs = File.Open(fullTempFile, mode))
                {
                    request = (HttpWebRequest)WebRequest.Create(url);

                    if (soFar > 0)
                        request.AddRange(soFar);

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream inner = response.GetResponseStream();

                        byte[] buffer = new byte[1024];

                        AppendAnimate("Downloading", txtStatus);

                        int bytesRead;
                        while ((bytesRead = inner.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, (int)bytesRead);
                            fs.Flush();
                            int perc = (int)(fs.Length * 1.0 / total * 100);
                            progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value = perc; }));
                            lblPercent.Invoke(new MethodInvoker(delegate { lblPercent.Text = perc + "%"; }));
                        }
                    }
                }
            }
            else
            {
                progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value = 100; }));
                lblPercent.Invoke(new MethodInvoker(delegate { lblPercent.Text = "100%"; }));
            }

            Append("Finished downloading", txtStatus);

            MD5 md5Maker = MD5.Create();
            string tempMD5;

            using (FileStream stream = File.OpenRead(fullTempFile))
            {
                tempMD5 = BitConverter.ToString(md5Maker.ComputeHash(stream)).Replace("-", "").ToLower();
            }

            if (md5.ToLower() == tempMD5)
            {
                using (FileStream fs = File.Open(fullTempFile, FileMode.Open))
                {
                    AppendAnimate("Saving downloaded update", txtStatus);

                    byte[] content = new byte[fs.Length];
                    fs.Read(content, 0, (int)fs.Length);

                    mail.Attachment = content;
                }

                if (!EMailControler.isMailExist(mail.EMailID) && EMailControler.isSubjectCorrect(mail.Subject))
                {
                    EMailControler.insertMail(mail);
                    try
                    {
                        File.Delete(fullTempFile);
                    }
                    catch (Exception e)
                    {
                    }

                    upgraderWindow.Invoke(new MethodInvoker(delegate { upgraderWindow.reLoadMail(); }));

                    General.Util.UI.MyMessageDialogSmall.Show("Download ready to be applied. You can now click 'Upgrade' in upgrader window.");
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }
            }
            else
            {
                General.Util.UI.MyMessageDialogSmall.Show("Downloaded file is corrupted. It will be deleted. Please download again.");

                try
                {
                    File.Delete(fullTempFile);
                }
                catch (Exception e)
                {
                    MyMessageDialogSmall.Show("Unable to delete corruted download. Error was: " + e.Message);
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    return;
                }

                this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                return;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (backgroundDownloader != null)
                backgroundDownloader.Abort();

            // clean up things the thread may have started
            //
            displayAnimator.Stop();

            if (dialerActivated) // Disconnect only if connected manually
                dialer.Disconnect();

            base.OnFormClosing(e);
        }

        private DataTable getConfiguration()
        {
            DataTable configData = new DataTable();
            DBConnHelper DBConnHelper = new DBConnHelper();
            string sqlStr = "SELECT * FROM eth_POPConfig";

            configData = DBConnHelper.GetDataSet(sqlStr).Tables[0];
            return configData;
        }
    }

    public class TextAnimator
    {
        private Control control;
        private int animationInterval = 300;
        private int dotCount;
        private string initial;
        private System.Timers.Timer timer;
        private bool isRunning = false;

        public TextAnimator(Control controlToAnimate)
        {
            this.control = controlToAnimate;

            // use these defaults
            this.AnimationInterval = 300;
            this.DotCount = 5;
        }

        public int AnimationInterval
        {
            get { return animationInterval; }
            set { animationInterval = value; }
        }

        public int DotCount
        {
            get { return dotCount; }
            set { dotCount = value; }
        }

        public void Start()
        {
            this.initial = this.control.Text;

            timer = new System.Timers.Timer(AnimationInterval);
            timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
                {
                    control.Invoke(new MethodInvoker(delegate
                        {
                            string last = control.Text;
                            int newDots = (last.Length - initial.Length + 1) % DotCount;
                            control.Text = initial + new string('.', newDots);
                        }
                        ));
                };

            timer.Start();
            isRunning = true;
        }

        public void Stop()
        {
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                control.Invoke(new MethodInvoker(delegate { control.Text = initial; }));
            }
        }
    }
}