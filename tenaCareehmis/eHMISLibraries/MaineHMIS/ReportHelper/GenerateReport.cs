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
using eHMIS.HMIS.ReportAggregation;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using eHMIS.HMIS.ReportAggregation.NewReporting;
using SqlManagement.Database;
using UtilitiesNew.GeneralUtilities;

namespace eHMIS.HMIS.ReportHelper
{


    public class GenerateReport
    {
        
        public static DataTable globalReportDataTable = new DataTable();
        public static string globalCreateTable = "";
        public static string globalCreateStoredProc = "";
        public static string globalReportTableName = "";
        public static string globalStoredProcName = "";

        public static DataTable globalServiceStoreProc = new DataTable();

        private SavedReports _savedReport;
        private reportObject reportPar;
        private string smartCareVrsion;
        private string fmohHmisVersion = "2/02"; // Current HMIS Version FMOH uses
        private string ehnriPhemVersion = "Unknown"; // The version of PHEM Ehnri uses
        private DBConnHelper DBConnHelper;
        private ReportName _reportName;
        private string reportString;
        //private string reportStartPath = eHMIS.Program.ApplicationStartupPath + @"\";
        private string reportStartPath = AppDomain.CurrentDomain.BaseDirectory; // + @"\";



        private DateTime _weekStartDate;

        public DateTime WeekStartDate
        {
            get { return _weekStartDate; }
            set { _weekStartDate = value; }
        }

        private DateTime _weekEndDate;

        public DateTime WeekEndDate
        {
            get { return _weekEndDate; }
            set { _weekEndDate = value; }
        }

        public GenerateReport(reportObject repoPar)
        {
            DBConnHelper = new DBConnHelper();
            reportPar = repoPar;

            _savedReport = new SavedReports();
            getSmartCareVrsion();
            setReportName();
        }

        private void setReportName()
        {
            if (reportPar != null)
            {
                _reportName = _savedReport.getReportName(reportPar.ReportType);

            }
            else
            {

                _reportName = ReportName.UNKNOWN;
            }


        }

        public reportObject createReportObject()
        {
            return reportPar;           
        }


        private void getSmartCareVrsion()
        {           
        }


        private string getMonthNameInAmharic(int monthID)
        {
            string monthName = "Unknown";

            if (monthID == 1)
            {
                return monthName = "Meskerem";
            }
            if (monthID == 2)
            {
                return monthName = "Tikimt";
            }
            if (monthID == 3)
            {
                return monthName = "Hidar";
            }
            if (monthID == 4)
            {
                return monthName = "Tahisas";
            }
            if (monthID == 5)
            {
                return monthName = "Tir";
            }
            if (monthID == 6)
            {
                return monthName = "Yekatit";
            }
            if (monthID == 7)
            {
                return monthName = "Megabit";
            } if (monthID == 8)
            {
                return monthName = "Miyazia";
            } if (monthID == 9)
            {
                return monthName = "Ginbot";
            } if (monthID == 10)
            {
                return monthName = "Sene";
            } if (monthID == 11)
            {
                return monthName = "Hamle";
            } if (monthID == 12)
            {
                return monthName = "Nehase";
            }
            return monthName;
        }


    }



}
