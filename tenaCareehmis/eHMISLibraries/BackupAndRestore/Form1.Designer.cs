namespace BackUpAndRestore
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnbackupdb = new System.Windows.Forms.Button();
            this.btnrestordb = new System.Windows.Forms.Button();
            this.btnsetbackuptime = new System.Windows.Forms.Button();
            this.btnvieweditschedule = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.cmblistofdb = new System.Windows.Forms.ComboBox();
            this.btnstrbackup = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtdestenationfolder = new System.Windows.Forms.TextBox();
            this.btnbrowse = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnstrrestoring = new System.Windows.Forms.Button();
            this.txtrestorelocation = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnbrowseReLocation = new System.Windows.Forms.Button();
            this.lblnewdb = new System.Windows.Forms.Label();
            this.txtnewdb = new System.Windows.Forms.TextBox();
            this.btnrestor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmblistofdbForrestor = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnDeleteNew = new System.Windows.Forms.Button();
            this.btneditTimeNew = new System.Windows.Forms.Button();
            this.cmbAMPM = new System.Windows.Forms.ComboBox();
            this.cmbmin = new System.Windows.Forms.ComboBox();
            this.lblhsepareter = new System.Windows.Forms.Label();
            this.cmbhour = new System.Windows.Forms.ComboBox();
            this.lblHour = new System.Windows.Forms.Label();
            this.lblday = new System.Windows.Forms.Label();
            this.cmbdays = new System.Windows.Forms.ComboBox();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnsave = new System.Windows.Forms.Button();
            this.btnadd = new System.Windows.Forms.Button();
            this.lstTaskList = new System.Windows.Forms.ListBox();
            this.cmbschedule = new System.Windows.Forms.ComboBox();
            this.cmblistdbtoschedule = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.cmbAMPMforEdit = new System.Windows.Forms.ComboBox();
            this.cmbminforedit = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbhourforedit = new System.Windows.Forms.ComboBox();
            this.lblhourFedit = new System.Windows.Forms.Label();
            this.lbllistday = new System.Windows.Forms.Label();
            this.cmblday = new System.Windows.Forms.ComboBox();
            this.btnupdateschedule = new System.Windows.Forms.Button();
            this.btnDeleteTask = new System.Windows.Forms.Button();
            this.btneditTime = new System.Windows.Forms.Button();
            this.txtpasswordforedit = new System.Windows.Forms.TextBox();
            this.lblpasswordforedit = new System.Windows.Forms.Label();
            this.txtusernameforedit = new System.Windows.Forms.TextBox();
            this.lbluserforedit = new System.Windows.Forms.Label();
            this.cmbSchedulforedit = new System.Windows.Forms.ComboBox();
            this.lblbackuptimeforedit = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lstAllTaskForEdit = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnbackupdb
            // 
            this.btnbackupdb.BackColor = System.Drawing.Color.LightYellow;
            this.btnbackupdb.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnbackupdb.Location = new System.Drawing.Point(6, 19);
            this.btnbackupdb.Name = "btnbackupdb";
            this.btnbackupdb.Size = new System.Drawing.Size(112, 47);
            this.btnbackupdb.TabIndex = 0;
            this.btnbackupdb.Text = "Backup Database";
            this.btnbackupdb.UseVisualStyleBackColor = false;
            this.btnbackupdb.Click += new System.EventHandler(this.btnbackupdb_Click);
            // 
            // btnrestordb
            // 
            this.btnrestordb.BackColor = System.Drawing.Color.LightYellow;
            this.btnrestordb.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnrestordb.Location = new System.Drawing.Point(6, 72);
            this.btnrestordb.Name = "btnrestordb";
            this.btnrestordb.Size = new System.Drawing.Size(112, 43);
            this.btnrestordb.TabIndex = 1;
            this.btnrestordb.Text = "Restore Database";
            this.btnrestordb.UseVisualStyleBackColor = false;
            this.btnrestordb.Click += new System.EventHandler(this.btnrestordb_Click);
            // 
            // btnsetbackuptime
            // 
            this.btnsetbackuptime.BackColor = System.Drawing.Color.LightYellow;
            this.btnsetbackuptime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsetbackuptime.Location = new System.Drawing.Point(6, 121);
            this.btnsetbackuptime.Name = "btnsetbackuptime";
            this.btnsetbackuptime.Size = new System.Drawing.Size(112, 47);
            this.btnsetbackuptime.TabIndex = 2;
            this.btnsetbackuptime.Text = "Setup A Backup Time";
            this.btnsetbackuptime.UseVisualStyleBackColor = false;
            this.btnsetbackuptime.Click += new System.EventHandler(this.btnsetbackuptime_Click);
            // 
            // btnvieweditschedule
            // 
            this.btnvieweditschedule.BackColor = System.Drawing.Color.LightYellow;
            this.btnvieweditschedule.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnvieweditschedule.Location = new System.Drawing.Point(6, 174);
            this.btnvieweditschedule.Name = "btnvieweditschedule";
            this.btnvieweditschedule.Size = new System.Drawing.Size(112, 43);
            this.btnvieweditschedule.TabIndex = 3;
            this.btnvieweditschedule.Text = "View\\Edit Backup Time Schedule";
            this.btnvieweditschedule.UseVisualStyleBackColor = false;
            this.btnvieweditschedule.Click += new System.EventHandler(this.btnvieweditschedule_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(132, 271);
            this.panel1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnbackupdb);
            this.groupBox1.Controls.Add(this.btnvieweditschedule);
            this.groupBox1.Controls.Add(this.btnrestordb);
            this.groupBox1.Controls.Add(this.btnsetbackuptime);
            this.groupBox1.Location = new System.Drawing.Point(7, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(125, 227);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(550, 271);
            this.panel2.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(548, 271);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Click += new System.EventHandler(this.tabControl1_Click);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.SteelBlue;
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.cmblistofdb);
            this.tabPage1.Controls.Add(this.btnstrbackup);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtdestenationfolder);
            this.tabPage1.Controls.Add(this.btnbrowse);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(540, 245);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Backup Database";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.SteelBlue;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(57, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Database";
            // 
            // cmblistofdb
            // 
            this.cmblistofdb.Enabled = false;
            this.cmblistofdb.FormattingEnabled = true;
            this.cmblistofdb.Items.AddRange(new object[] {
            "CDC_FDB_DB"});
            this.cmblistofdb.Location = new System.Drawing.Point(127, 30);
            this.cmblistofdb.Name = "cmblistofdb";
            this.cmblistofdb.Size = new System.Drawing.Size(142, 21);
            this.cmblistofdb.TabIndex = 4;
            // 
            // btnstrbackup
            // 
            this.btnstrbackup.BackColor = System.Drawing.Color.LightYellow;
            this.btnstrbackup.Enabled = false;
            this.btnstrbackup.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnstrbackup.Location = new System.Drawing.Point(127, 155);
            this.btnstrbackup.Name = "btnstrbackup";
            this.btnstrbackup.Size = new System.Drawing.Size(142, 23);
            this.btnstrbackup.TabIndex = 3;
            this.btnstrbackup.Text = "Start Backup Database";
            this.btnstrbackup.UseVisualStyleBackColor = false;
            this.btnstrbackup.Click += new System.EventHandler(this.btnstrbackup_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.SteelBlue;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(18, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Destination Folder";
            // 
            // txtdestenationfolder
            // 
            this.txtdestenationfolder.Enabled = false;
            this.txtdestenationfolder.Location = new System.Drawing.Point(127, 102);
            this.txtdestenationfolder.Multiline = true;
            this.txtdestenationfolder.Name = "txtdestenationfolder";
            this.txtdestenationfolder.Size = new System.Drawing.Size(405, 47);
            this.txtdestenationfolder.TabIndex = 1;
            this.txtdestenationfolder.TextChanged += new System.EventHandler(this.txtdestenationfolder_TextChanged);
            // 
            // btnbrowse
            // 
            this.btnbrowse.BackColor = System.Drawing.Color.LightYellow;
            this.btnbrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnbrowse.Location = new System.Drawing.Point(127, 73);
            this.btnbrowse.Name = "btnbrowse";
            this.btnbrowse.Size = new System.Drawing.Size(142, 23);
            this.btnbrowse.TabIndex = 0;
            this.btnbrowse.Text = "Browse";
            this.btnbrowse.UseVisualStyleBackColor = false;
            this.btnbrowse.Click += new System.EventHandler(this.btnbrowse_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.SteelBlue;
            this.tabPage2.Controls.Add(this.btnstrrestoring);
            this.tabPage2.Controls.Add(this.txtrestorelocation);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.btnbrowseReLocation);
            this.tabPage2.Controls.Add(this.lblnewdb);
            this.tabPage2.Controls.Add(this.txtnewdb);
            this.tabPage2.Controls.Add(this.btnrestor);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.cmblistofdbForrestor);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(540, 245);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Restore Database";
            // 
            // btnstrrestoring
            // 
            this.btnstrrestoring.BackColor = System.Drawing.Color.LightYellow;
            this.btnstrrestoring.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnstrrestoring.Location = new System.Drawing.Point(389, 34);
            this.btnstrrestoring.Name = "btnstrrestoring";
            this.btnstrrestoring.Size = new System.Drawing.Size(142, 57);
            this.btnstrrestoring.TabIndex = 15;
            this.btnstrrestoring.Text = "Start Restoring New";
            this.btnstrrestoring.UseVisualStyleBackColor = false;
            this.btnstrrestoring.Visible = false;
            this.btnstrrestoring.Click += new System.EventHandler(this.btnstrrestoring_Click);
            // 
            // txtrestorelocation
            // 
            this.txtrestorelocation.Enabled = false;
            this.txtrestorelocation.Location = new System.Drawing.Point(127, 102);
            this.txtrestorelocation.Multiline = true;
            this.txtrestorelocation.Name = "txtrestorelocation";
            this.txtrestorelocation.Size = new System.Drawing.Size(405, 47);
            this.txtrestorelocation.TabIndex = 14;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.SteelBlue;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label11.Location = new System.Drawing.Point(32, 78);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Restored From";
            // 
            // btnbrowseReLocation
            // 
            this.btnbrowseReLocation.BackColor = System.Drawing.Color.LightYellow;
            this.btnbrowseReLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnbrowseReLocation.Location = new System.Drawing.Point(127, 73);
            this.btnbrowseReLocation.Name = "btnbrowseReLocation";
            this.btnbrowseReLocation.Size = new System.Drawing.Size(142, 23);
            this.btnbrowseReLocation.TabIndex = 12;
            this.btnbrowseReLocation.Text = "Browse";
            this.btnbrowseReLocation.UseVisualStyleBackColor = false;
            this.btnbrowseReLocation.Click += new System.EventHandler(this.btnbrowseReLocation_Click);
            // 
            // lblnewdb
            // 
            this.lblnewdb.AutoSize = true;
            this.lblnewdb.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblnewdb.Location = new System.Drawing.Point(301, 12);
            this.lblnewdb.Name = "lblnewdb";
            this.lblnewdb.Size = new System.Drawing.Size(85, 13);
            this.lblnewdb.TabIndex = 11;
            this.lblnewdb.Text = "DataBase Name";
            this.lblnewdb.Visible = false;
            // 
            // txtnewdb
            // 
            this.txtnewdb.Location = new System.Drawing.Point(390, 6);
            this.txtnewdb.Name = "txtnewdb";
            this.txtnewdb.Size = new System.Drawing.Size(142, 20);
            this.txtnewdb.TabIndex = 10;
            this.txtnewdb.Visible = false;
            // 
            // btnrestor
            // 
            this.btnrestor.BackColor = System.Drawing.Color.LightYellow;
            this.btnrestor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnrestor.Location = new System.Drawing.Point(127, 155);
            this.btnrestor.Name = "btnrestor";
            this.btnrestor.Size = new System.Drawing.Size(142, 25);
            this.btnrestor.TabIndex = 9;
            this.btnrestor.Text = "Start Restore";
            this.btnrestor.UseVisualStyleBackColor = false;
            this.btnrestor.Click += new System.EventHandler(this.btnrestor_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(54, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Database";
            // 
            // cmblistofdbForrestor
            // 
            this.cmblistofdbForrestor.Enabled = false;
            this.cmblistofdbForrestor.FormattingEnabled = true;
            this.cmblistofdbForrestor.Items.AddRange(new object[] {
            "CDC_FDB_DB"});
            this.cmblistofdbForrestor.Location = new System.Drawing.Point(127, 30);
            this.cmblistofdbForrestor.Name = "cmblistofdbForrestor";
            this.cmblistofdbForrestor.Size = new System.Drawing.Size(142, 21);
            this.cmblistofdbForrestor.TabIndex = 7;
            this.cmblistofdbForrestor.SelectedIndexChanged += new System.EventHandler(this.cmblistofdbForrestor_SelectedIndexChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.SteelBlue;
            this.tabPage4.Controls.Add(this.btnDeleteNew);
            this.tabPage4.Controls.Add(this.btneditTimeNew);
            this.tabPage4.Controls.Add(this.cmbAMPM);
            this.tabPage4.Controls.Add(this.cmbmin);
            this.tabPage4.Controls.Add(this.lblhsepareter);
            this.tabPage4.Controls.Add(this.cmbhour);
            this.tabPage4.Controls.Add(this.lblHour);
            this.tabPage4.Controls.Add(this.lblday);
            this.tabPage4.Controls.Add(this.cmbdays);
            this.tabPage4.Controls.Add(this.txtpassword);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.txtusername);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.btnsave);
            this.tabPage4.Controls.Add(this.btnadd);
            this.tabPage4.Controls.Add(this.lstTaskList);
            this.tabPage4.Controls.Add(this.cmbschedule);
            this.tabPage4.Controls.Add(this.cmblistdbtoschedule);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(540, 245);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Schedule Backup Jobs";
            // 
            // btnDeleteNew
            // 
            this.btnDeleteNew.BackColor = System.Drawing.Color.LightYellow;
            this.btnDeleteNew.Enabled = false;
            this.btnDeleteNew.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDeleteNew.Location = new System.Drawing.Point(302, 133);
            this.btnDeleteNew.Name = "btnDeleteNew";
            this.btnDeleteNew.Size = new System.Drawing.Size(127, 23);
            this.btnDeleteNew.TabIndex = 25;
            this.btnDeleteNew.Text = "Delete Selected Task";
            this.btnDeleteNew.UseVisualStyleBackColor = false;
            this.btnDeleteNew.Click += new System.EventHandler(this.btnDeleteNew_Click);
            // 
            // btneditTimeNew
            // 
            this.btneditTimeNew.BackColor = System.Drawing.Color.LightYellow;
            this.btneditTimeNew.Enabled = false;
            this.btneditTimeNew.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btneditTimeNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btneditTimeNew.Location = new System.Drawing.Point(169, 133);
            this.btneditTimeNew.Name = "btneditTimeNew";
            this.btneditTimeNew.Size = new System.Drawing.Size(127, 23);
            this.btneditTimeNew.TabIndex = 24;
            this.btneditTimeNew.Text = "Edit Selected Task";
            this.btneditTimeNew.UseVisualStyleBackColor = false;
            this.btneditTimeNew.Click += new System.EventHandler(this.btneditTimeNew_Click);
            // 
            // cmbAMPM
            // 
            this.cmbAMPM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAMPM.FormattingEnabled = true;
            this.cmbAMPM.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.cmbAMPM.Location = new System.Drawing.Point(417, 72);
            this.cmbAMPM.Name = "cmbAMPM";
            this.cmbAMPM.Size = new System.Drawing.Size(41, 21);
            this.cmbAMPM.TabIndex = 23;
            this.cmbAMPM.Visible = false;
            // 
            // cmbmin
            // 
            this.cmbmin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbmin.FormattingEnabled = true;
            this.cmbmin.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
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
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.cmbmin.Location = new System.Drawing.Point(380, 72);
            this.cmbmin.Name = "cmbmin";
            this.cmbmin.Size = new System.Drawing.Size(35, 21);
            this.cmbmin.TabIndex = 22;
            this.cmbmin.Visible = false;
            // 
            // lblhsepareter
            // 
            this.lblhsepareter.AutoSize = true;
            this.lblhsepareter.Location = new System.Drawing.Point(370, 76);
            this.lblhsepareter.Name = "lblhsepareter";
            this.lblhsepareter.Size = new System.Drawing.Size(10, 13);
            this.lblhsepareter.TabIndex = 21;
            this.lblhsepareter.Text = ":";
            this.lblhsepareter.Visible = false;
            // 
            // cmbhour
            // 
            this.cmbhour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbhour.FormattingEnabled = true;
            this.cmbhour.Items.AddRange(new object[] {
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
            "12"});
            this.cmbhour.Location = new System.Drawing.Point(334, 72);
            this.cmbhour.Name = "cmbhour";
            this.cmbhour.Size = new System.Drawing.Size(36, 21);
            this.cmbhour.TabIndex = 20;
            this.cmbhour.Visible = false;
            // 
            // lblHour
            // 
            this.lblHour.AutoSize = true;
            this.lblHour.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHour.Location = new System.Drawing.Point(289, 76);
            this.lblHour.Name = "lblHour";
            this.lblHour.Size = new System.Drawing.Size(30, 13);
            this.lblHour.TabIndex = 19;
            this.lblHour.Text = "Hour";
            this.lblHour.Visible = false;
            // 
            // lblday
            // 
            this.lblday.AutoSize = true;
            this.lblday.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblday.Location = new System.Drawing.Point(293, 49);
            this.lblday.Name = "lblday";
            this.lblday.Size = new System.Drawing.Size(26, 13);
            this.lblday.TabIndex = 18;
            this.lblday.Text = "Day";
            this.lblday.Visible = false;
            // 
            // cmbdays
            // 
            this.cmbdays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbdays.FormattingEnabled = true;
            this.cmbdays.Location = new System.Drawing.Point(334, 46);
            this.cmbdays.Name = "cmbdays";
            this.cmbdays.Size = new System.Drawing.Size(83, 21);
            this.cmbdays.TabIndex = 17;
            this.cmbdays.Visible = false;
            // 
            // txtpassword
            // 
            this.txtpassword.Location = new System.Drawing.Point(123, 99);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PasswordChar = '*';
            this.txtpassword.Size = new System.Drawing.Size(142, 20);
            this.txtpassword.TabIndex = 16;
            this.txtpassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtpassword_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(49, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Password";
            // 
            // txtusername
            // 
            this.txtusername.Enabled = false;
            this.txtusername.Location = new System.Drawing.Point(123, 73);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(142, 20);
            this.txtusername.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(43, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "User Name";
            // 
            // btnsave
            // 
            this.btnsave.BackColor = System.Drawing.Color.LightYellow;
            this.btnsave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsave.Location = new System.Drawing.Point(190, 219);
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(75, 23);
            this.btnsave.TabIndex = 12;
            this.btnsave.Text = "Save";
            this.btnsave.UseVisualStyleBackColor = false;
            this.btnsave.Visible = false;
            this.btnsave.Click += new System.EventHandler(this.btnsave_Click);
            // 
            // btnadd
            // 
            this.btnadd.BackColor = System.Drawing.Color.LightYellow;
            this.btnadd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnadd.Location = new System.Drawing.Point(36, 133);
            this.btnadd.Name = "btnadd";
            this.btnadd.Size = new System.Drawing.Size(127, 23);
            this.btnadd.TabIndex = 11;
            this.btnadd.Text = "Add";
            this.btnadd.UseVisualStyleBackColor = false;
            this.btnadd.Click += new System.EventHandler(this.btnadd_Click);
            // 
            // lstTaskList
            // 
            this.lstTaskList.FormattingEnabled = true;
            this.lstTaskList.Location = new System.Drawing.Point(36, 162);
            this.lstTaskList.Name = "lstTaskList";
            this.lstTaskList.Size = new System.Drawing.Size(393, 56);
            this.lstTaskList.TabIndex = 10;
            // 
            // cmbschedule
            // 
            this.cmbschedule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbschedule.FormattingEnabled = true;
            this.cmbschedule.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly"});
            this.cmbschedule.Location = new System.Drawing.Point(123, 46);
            this.cmbschedule.Name = "cmbschedule";
            this.cmbschedule.Size = new System.Drawing.Size(142, 21);
            this.cmbschedule.TabIndex = 9;
            this.cmbschedule.SelectedIndexChanged += new System.EventHandler(this.cmbschedule_SelectedIndexChanged);
            // 
            // cmblistdbtoschedule
            // 
            this.cmblistdbtoschedule.Enabled = false;
            this.cmblistdbtoschedule.FormattingEnabled = true;
            this.cmblistdbtoschedule.Location = new System.Drawing.Point(123, 16);
            this.cmblistdbtoschedule.Name = "cmblistdbtoschedule";
            this.cmblistdbtoschedule.Size = new System.Drawing.Size(142, 21);
            this.cmblistdbtoschedule.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(49, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "DataBase";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(33, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Backup Period";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.SteelBlue;
            this.tabPage5.Controls.Add(this.cmbAMPMforEdit);
            this.tabPage5.Controls.Add(this.cmbminforedit);
            this.tabPage5.Controls.Add(this.label10);
            this.tabPage5.Controls.Add(this.cmbhourforedit);
            this.tabPage5.Controls.Add(this.lblhourFedit);
            this.tabPage5.Controls.Add(this.lbllistday);
            this.tabPage5.Controls.Add(this.cmblday);
            this.tabPage5.Controls.Add(this.btnupdateschedule);
            this.tabPage5.Controls.Add(this.btnDeleteTask);
            this.tabPage5.Controls.Add(this.btneditTime);
            this.tabPage5.Controls.Add(this.txtpasswordforedit);
            this.tabPage5.Controls.Add(this.lblpasswordforedit);
            this.tabPage5.Controls.Add(this.txtusernameforedit);
            this.tabPage5.Controls.Add(this.lbluserforedit);
            this.tabPage5.Controls.Add(this.cmbSchedulforedit);
            this.tabPage5.Controls.Add(this.lblbackuptimeforedit);
            this.tabPage5.Controls.Add(this.label9);
            this.tabPage5.Controls.Add(this.lstAllTaskForEdit);
            this.tabPage5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(540, 245);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "View/Edit Scheduled Backup Jobs";
            // 
            // cmbAMPMforEdit
            // 
            this.cmbAMPMforEdit.FormattingEnabled = true;
            this.cmbAMPMforEdit.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.cmbAMPMforEdit.Location = new System.Drawing.Point(235, 131);
            this.cmbAMPMforEdit.Name = "cmbAMPMforEdit";
            this.cmbAMPMforEdit.Size = new System.Drawing.Size(38, 21);
            this.cmbAMPMforEdit.TabIndex = 32;
            this.cmbAMPMforEdit.Visible = false;
            // 
            // cmbminforedit
            // 
            this.cmbminforedit.FormattingEnabled = true;
            this.cmbminforedit.Items.AddRange(new object[] {
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
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
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.cmbminforedit.Location = new System.Drawing.Point(198, 131);
            this.cmbminforedit.Name = "cmbminforedit";
            this.cmbminforedit.Size = new System.Drawing.Size(35, 21);
            this.cmbminforedit.TabIndex = 31;
            this.cmbminforedit.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(188, 135);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(10, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = ":";
            this.label10.Visible = false;
            // 
            // cmbhourforedit
            // 
            this.cmbhourforedit.FormattingEnabled = true;
            this.cmbhourforedit.Items.AddRange(new object[] {
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
            "12"});
            this.cmbhourforedit.Location = new System.Drawing.Point(152, 131);
            this.cmbhourforedit.Name = "cmbhourforedit";
            this.cmbhourforedit.Size = new System.Drawing.Size(36, 21);
            this.cmbhourforedit.TabIndex = 29;
            this.cmbhourforedit.Visible = false;
            // 
            // lblhourFedit
            // 
            this.lblhourFedit.AutoSize = true;
            this.lblhourFedit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblhourFedit.Location = new System.Drawing.Point(113, 134);
            this.lblhourFedit.Name = "lblhourFedit";
            this.lblhourFedit.Size = new System.Drawing.Size(30, 13);
            this.lblhourFedit.TabIndex = 28;
            this.lblhourFedit.Text = "Hour";
            this.lblhourFedit.Visible = false;
            // 
            // lbllistday
            // 
            this.lbllistday.AutoSize = true;
            this.lbllistday.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbllistday.Location = new System.Drawing.Point(303, 107);
            this.lbllistday.Name = "lbllistday";
            this.lbllistday.Size = new System.Drawing.Size(26, 13);
            this.lbllistday.TabIndex = 27;
            this.lbllistday.Text = "Day";
            this.lbllistday.Visible = false;
            // 
            // cmblday
            // 
            this.cmblday.FormattingEnabled = true;
            this.cmblday.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});
            this.cmblday.Location = new System.Drawing.Point(330, 104);
            this.cmblday.Name = "cmblday";
            this.cmblday.Size = new System.Drawing.Size(83, 21);
            this.cmblday.TabIndex = 26;
            this.cmblday.Visible = false;
            // 
            // btnupdateschedule
            // 
            this.btnupdateschedule.BackColor = System.Drawing.Color.LightYellow;
            this.btnupdateschedule.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnupdateschedule.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnupdateschedule.Location = new System.Drawing.Point(163, 214);
            this.btnupdateschedule.Name = "btnupdateschedule";
            this.btnupdateschedule.Size = new System.Drawing.Size(83, 23);
            this.btnupdateschedule.TabIndex = 25;
            this.btnupdateschedule.Text = "Update";
            this.btnupdateschedule.UseVisualStyleBackColor = false;
            this.btnupdateschedule.Visible = false;
            this.btnupdateschedule.Click += new System.EventHandler(this.btnupdateschedule_Click);
            // 
            // btnDeleteTask
            // 
            this.btnDeleteTask.BackColor = System.Drawing.Color.LightYellow;
            this.btnDeleteTask.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteTask.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDeleteTask.Location = new System.Drawing.Point(402, 61);
            this.btnDeleteTask.Name = "btnDeleteTask";
            this.btnDeleteTask.Size = new System.Drawing.Size(118, 28);
            this.btnDeleteTask.TabIndex = 24;
            this.btnDeleteTask.Text = "Delete Selected Task";
            this.btnDeleteTask.UseVisualStyleBackColor = false;
            this.btnDeleteTask.Click += new System.EventHandler(this.btnDeleteTask_Click);
            // 
            // btneditTime
            // 
            this.btneditTime.BackColor = System.Drawing.Color.LightYellow;
            this.btneditTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btneditTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btneditTime.Location = new System.Drawing.Point(402, 27);
            this.btneditTime.Name = "btneditTime";
            this.btneditTime.Size = new System.Drawing.Size(118, 28);
            this.btneditTime.TabIndex = 23;
            this.btneditTime.Text = "Edit Selected Task";
            this.btneditTime.UseVisualStyleBackColor = false;
            this.btneditTime.Click += new System.EventHandler(this.btneditTime_Click);
            // 
            // txtpasswordforedit
            // 
            this.txtpasswordforedit.Location = new System.Drawing.Point(152, 188);
            this.txtpasswordforedit.Name = "txtpasswordforedit";
            this.txtpasswordforedit.PasswordChar = '*';
            this.txtpasswordforedit.Size = new System.Drawing.Size(142, 20);
            this.txtpasswordforedit.TabIndex = 22;
            this.txtpasswordforedit.Visible = false;
            // 
            // lblpasswordforedit
            // 
            this.lblpasswordforedit.AutoSize = true;
            this.lblpasswordforedit.Location = new System.Drawing.Point(90, 188);
            this.lblpasswordforedit.Name = "lblpasswordforedit";
            this.lblpasswordforedit.Size = new System.Drawing.Size(53, 13);
            this.lblpasswordforedit.TabIndex = 21;
            this.lblpasswordforedit.Text = "Password";
            this.lblpasswordforedit.Visible = false;
            // 
            // txtusernameforedit
            // 
            this.txtusernameforedit.Enabled = false;
            this.txtusernameforedit.Location = new System.Drawing.Point(152, 162);
            this.txtusernameforedit.Name = "txtusernameforedit";
            this.txtusernameforedit.Size = new System.Drawing.Size(142, 20);
            this.txtusernameforedit.TabIndex = 20;
            this.txtusernameforedit.Visible = false;
            // 
            // lbluserforedit
            // 
            this.lbluserforedit.AutoSize = true;
            this.lbluserforedit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbluserforedit.Location = new System.Drawing.Point(86, 165);
            this.lbluserforedit.Name = "lbluserforedit";
            this.lbluserforedit.Size = new System.Drawing.Size(60, 13);
            this.lbluserforedit.TabIndex = 19;
            this.lbluserforedit.Text = "User Name";
            this.lbluserforedit.Visible = false;
            // 
            // cmbSchedulforedit
            // 
            this.cmbSchedulforedit.FormattingEnabled = true;
            this.cmbSchedulforedit.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly"});
            this.cmbSchedulforedit.Location = new System.Drawing.Point(152, 104);
            this.cmbSchedulforedit.Name = "cmbSchedulforedit";
            this.cmbSchedulforedit.Size = new System.Drawing.Size(142, 21);
            this.cmbSchedulforedit.TabIndex = 18;
            this.cmbSchedulforedit.Visible = false;
            this.cmbSchedulforedit.SelectedIndexChanged += new System.EventHandler(this.cmbSchedulforedit_SelectedIndexChanged);
            // 
            // lblbackuptimeforedit
            // 
            this.lblbackuptimeforedit.AutoSize = true;
            this.lblbackuptimeforedit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblbackuptimeforedit.Location = new System.Drawing.Point(40, 107);
            this.lblbackuptimeforedit.Name = "lblbackuptimeforedit";
            this.lblbackuptimeforedit.Size = new System.Drawing.Size(103, 13);
            this.lblbackuptimeforedit.TabIndex = 17;
            this.lblbackuptimeforedit.Text = "Select Backup Time";
            this.lblbackuptimeforedit.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(23, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 26);
            this.label9.TabIndex = 1;
            this.label9.Text = "List of Scheduled \r\n        Tasks";
            // 
            // lstAllTaskForEdit
            // 
            this.lstAllTaskForEdit.FormattingEnabled = true;
            this.lstAllTaskForEdit.Location = new System.Drawing.Point(121, 27);
            this.lstAllTaskForEdit.Name = "lstAllTaskForEdit";
            this.lstAllTaskForEdit.Size = new System.Drawing.Size(275, 69);
            this.lstAllTaskForEdit.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 271);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup and Restore Database";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnbackupdb;
        private System.Windows.Forms.Button btnrestordb;
        private System.Windows.Forms.Button btnsetbackuptime;
        private System.Windows.Forms.Button btnvieweditschedule;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtdestenationfolder;
        private System.Windows.Forms.Button btnbrowse;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnstrbackup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmblistofdb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmblistofdbForrestor;
        private System.Windows.Forms.Button btnrestor;
        private System.Windows.Forms.Label lblnewdb;
        private System.Windows.Forms.TextBox txtnewdb;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ComboBox cmbschedule;
        private System.Windows.Forms.ComboBox cmblistdbtoschedule;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnadd;
        private System.Windows.Forms.ListBox lstTaskList;
        private System.Windows.Forms.Button btnsave;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox lstAllTaskForEdit;
        private System.Windows.Forms.Button btnDeleteTask;
        private System.Windows.Forms.Button btneditTime;
        private System.Windows.Forms.TextBox txtpasswordforedit;
        private System.Windows.Forms.Label lblpasswordforedit;
        private System.Windows.Forms.TextBox txtusernameforedit;
        private System.Windows.Forms.Label lbluserforedit;
        private System.Windows.Forms.ComboBox cmbSchedulforedit;
        private System.Windows.Forms.Label lblbackuptimeforedit;
        private System.Windows.Forms.Button btnupdateschedule;
        private System.Windows.Forms.Label lblday;
        private System.Windows.Forms.ComboBox cmbdays;
        private System.Windows.Forms.ComboBox cmbAMPM;
        private System.Windows.Forms.ComboBox cmbmin;
        private System.Windows.Forms.Label lblhsepareter;
        private System.Windows.Forms.ComboBox cmbhour;
        private System.Windows.Forms.Label lblHour;
        private System.Windows.Forms.ComboBox cmbAMPMforEdit;
        private System.Windows.Forms.ComboBox cmbminforedit;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbhourforedit;
        private System.Windows.Forms.Label lblhourFedit;
        private System.Windows.Forms.Label lbllistday;
        private System.Windows.Forms.ComboBox cmblday;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnbrowseReLocation;
        private System.Windows.Forms.TextBox txtrestorelocation;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnstrrestoring;
        private System.Windows.Forms.Button btneditTimeNew;
        private System.Windows.Forms.Button btnDeleteNew;
    }
}

