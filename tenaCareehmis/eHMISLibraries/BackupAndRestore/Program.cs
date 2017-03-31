using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BackUpAndRestore
{
    static class Program
    {
        public static string ApplicationStartupPath = "";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {
            ApplicationStartupPath = Application.StartupPath;

            if (arg.Length > 0)
            {
                if (arg[0].ToString() == "Backup")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                if (arg[0].ToString() == "Update")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Upgrader());
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
       
    }
}