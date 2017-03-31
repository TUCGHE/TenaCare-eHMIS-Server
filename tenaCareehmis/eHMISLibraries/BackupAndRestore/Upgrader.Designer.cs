namespace BackUpAndRestore
{
    partial class Upgrader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Upgrader));
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btncheckUpdate = new System.Windows.Forms.Button();
            this.btndiskupdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.BackColor = System.Drawing.Color.LightYellow;
            this.btnUpgrade.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUpgrade.Location = new System.Drawing.Point(12, 75);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(138, 33);
            this.btnUpgrade.TabIndex = 11;
            this.btnUpgrade.Text = "Upgrade ...";
            this.btnUpgrade.UseVisualStyleBackColor = false;
            this.btnUpgrade.Visible = false;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoEllipsis = true;
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(196, 63);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(35, 13);
            this.lblInfo.TabIndex = 10;
            this.lblInfo.Text = "lblInfo";
            // 
            // btncheckUpdate
            // 
            this.btncheckUpdate.BackColor = System.Drawing.Color.LightYellow;
            this.btncheckUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btncheckUpdate.Location = new System.Drawing.Point(12, 26);
            this.btncheckUpdate.Name = "btncheckUpdate";
            this.btncheckUpdate.Size = new System.Drawing.Size(138, 33);
            this.btncheckUpdate.TabIndex = 9;
            this.btncheckUpdate.Text = "Update From Internet";
            this.btncheckUpdate.UseVisualStyleBackColor = false;
            this.btncheckUpdate.Click += new System.EventHandler(this.btncheckUpdate2_Click);
            // 
            // btndiskupdate
            // 
            this.btndiskupdate.BackColor = System.Drawing.Color.LightYellow;
            this.btndiskupdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btndiskupdate.Location = new System.Drawing.Point(12, 124);
            this.btndiskupdate.Name = "btndiskupdate";
            this.btndiskupdate.Size = new System.Drawing.Size(138, 33);
            this.btndiskupdate.TabIndex = 12;
            this.btndiskupdate.Text = "Update From Disk";
            this.btndiskupdate.UseVisualStyleBackColor = false;
            this.btndiskupdate.Click += new System.EventHandler(this.btndiskupdate_Click);
            // 
            // Upgrader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(411, 191);
            this.Controls.Add(this.btndiskupdate);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btncheckUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Upgrader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Software Update";
            this.Load += new System.EventHandler(this.Upgrader_Load);
            this.ResizeBegin += new System.EventHandler(this.Upgrader_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Upgrader_ResizeEnd);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btncheckUpdate;
        private System.Windows.Forms.Button btndiskupdate;
    }
}