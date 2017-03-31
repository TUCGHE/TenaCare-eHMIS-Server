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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using General.Util;
using System.IO;
using SqlManagement.Database;
using eHMIS.HMIS.ReportHelper;
using System.Text.RegularExpressions;
using eHMISWebApi.Controllers;

namespace eHMIS.HMIS.ReportAggregation.NewReporting
{

    public class NewDiseaseReportAggregation
    {
        DBConnHelper _helper = new DBConnHelper();

        //Dictionary<string, string> aggregateDataHash = new Dictionary<string, string>();

        Hashtable ethMonth = new Hashtable();
        Hashtable aggregateDataHash = new Hashtable();
        ArrayList aggregateList = new ArrayList();
        ArrayList healthPost_NoDisplay = new ArrayList();

        //string viewLabeIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
        //string reportTableName = "EthioHIMS_QuarterRHBOOPDDiseaseReport"; // Should be dynamically set
        string storedProcName = "proc_Eth_HMIS_OPD_IPD_DiseaseReport";
        string storedProcNameNew = "proc_Eth_HMIS_OPD_IPD_DiseaseReportNew";
        string includeListProcName = "proc_Eth_HMIS_OPD_IPD_IncludedList";
        string viewLabeIdTableName = "";
        string reportTableName = "";

        private int dataEleClassMorbidity;
        private int dataEleClassMortality;

        reportObject _reportObj;
        int _startMonth;
        int _endMonth;
        decimal _seleYear;
        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation

        private static List<string> IncludedList;
        string hmisCodesSelected = "";
        string sha1Hash = "";
        bool level1Cache = false;
        bool level2Cache = false;

        Hashtable languageHash = new Hashtable();

        public NewDiseaseReportAggregation(reportObject reportObj)
        {
            setLanguage();
            //reportObj.ReportType
            _helper.ManualCloseConnection = true;

            viewLabeIdTableName = reportObj.ViewLabelIdTableName;
            reportTableName = reportObj.ReportTableName;
            GenerateReport.globalReportTableName = reportTableName;
            GenerateReport.globalCreateStoredProc = "";

            this._reportObj = reportObj;
            setStartingMonth();

            level1Cache = false;
            level2Cache = false;

            ethMonth.Add(1, "Meskerem");
            ethMonth.Add(2, "Tikimt");
            ethMonth.Add(3, "Hidar");
            ethMonth.Add(4, "Tahisas");
            ethMonth.Add(5, "Tir");
            ethMonth.Add(6, "Yekatit");
            ethMonth.Add(7, "Megabit");
            ethMonth.Add(8, "Miyazia");
            ethMonth.Add(9, "Ginbot");
            ethMonth.Add(10, "Sene");
            ethMonth.Add(11, "Hamle");
            ethMonth.Add(12, "Nehase");
            ethMonth.Add(13, "pagume");

            int aggrLevel = getAggregationLevel(_reportObj.LocationHMISCode);

            if (aggrLevel == 4)
            {
                level2Cache = false;
            }


            if (!level1Cache)
            {

                if (eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable.Contains("EthioHMIS_HMIS_Value_Temp"))
                {
                    //_helper.Connection.ChangeDatabase(_helper.DatabaseName);
                    //_helper = ReportViewing.HMISImportandDownloadServerBased._helper;

                    storedProcName = "proc_Eth_HMIS_OPD_IPD_DiseaseReport_Import";
                    storedProcNameNew = "proc_Eth_HMIS_OPD_IPD_DiseaseReport_ImportNew";

                }
                else
                {
                    storedProcName = "proc_Eth_HMIS_OPD_IPD_DiseaseReport";
                    storedProcNameNew = "proc_Eth_HMIS_OPD_IPD_DiseaseReportNew";

                }

                string cmdText = "select LabelID_M04 from EthEhmis_OPD_NoHealthPost";

                SqlCommand toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    string labelId = dr["LabelID_M04"].ToString();
                    healthPost_NoDisplay.Add(labelId);
                }
            }
        }

        private void setLanguage()
        {
            LanguageController langCtrl = new eHMISWebApi.Controllers.LanguageController();

            DataTable dtLang = langCtrl.Get(LanguageController.languageSet, "dataEntry");

            foreach(DataRow row in dtLang.Rows)
            {
                string indexName = row["indexName"].ToString();
                string languageName = row["languageName"].ToString();
                languageHash[indexName] = languageName;
            }
        }
              
        private void setStartingMonth()
        {
            _seleYear = _reportObj.Year;

            switch (_reportObj.StartQuarter)
            {
                case 0: _startMonth = _reportObj.StartMonth;
                    _endMonth = _reportObj.EndMonth;
                    _reportObj.EndQuarter = 0;
                    break;
                case 1: _startMonth = 11;                    
                    break;
                case 2: _startMonth = 2;
                    //_endMonth = 4;
                    break;
                case 3: _startMonth = 5;
                    //_endMonth = 7;
                    break;
                case 4: _startMonth = 8;
                    //_endMonth = 10;
                    break;
            }

            switch (_reportObj.EndQuarter)
            {
                case 0: _startMonth = _reportObj.StartMonth;
                    _endMonth = _reportObj.EndMonth;
                    break;
                case 1: //_startMonth = 11;
                    _endMonth = 1;                    
                    break;
                case 2: //_startMonth = 2;
                    _endMonth = 4;
                    break;
                case 3: //_startMonth = 5;
                    _endMonth = 7;
                    break;
                case 4: //_startMonth = 8;
                    _endMonth = 10;
                    break;
            }
        }


        public void deleteTables()
        {
           
            string cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + reportTableName + "]') AND type in (N'U'))  " +
                             " DROP TABLE " + reportTableName;


            if ((_reportObj.RepKind == reportObject.ReportKind.OPD_Disease) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease_Facility_Monthly) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease_Facility_Quarterly) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_HC_Aggregate_Monthly) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_HC_Aggregate_Quarterly))
            {
                // Set DataEleClass
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassMorbidity = 10;
                }
                else
                {
                    dataEleClassMorbidity = 8;
                }

                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                _helper.Execute(toExecute);

                cmdText = " CREATE TABLE  " + reportTableName + "  ( " +
                        " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                        " [SNO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                        " [Disease] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                        " [M04] [decimal](18, 0) NULL, " +
                        " [M514] [decimal](18, 0) NULL, " +
                        " [M15] [decimal](18, 0) NULL, " +
                        " [F04] [decimal](18, 0) NULL, " +
                        " [F514] [decimal](18, 0) NULL, " +
                        " [F15] [decimal](18, 0) NULL, " +
                        " [Format][int] NULL)";
               
                GenerateReport.globalCreateTable = cmdText;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                _helper.Execute(toExecute);

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column M04   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column M514  varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column M15   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column F04   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column F514  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column F15   varchar(50)  ";
                //"ALTER TABLE " + reportTableName + "  " +
                //"Alter Column Format int    ";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }
            else if ((_reportObj.RepKind == reportObject.ReportKind.IPD_Disease) ||
                (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease_Facility_Monthly) ||
                (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease_Facility_Quarterly))
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassMorbidity = 11;
                    dataEleClassMortality = 12;
                }
                else
                {
                    dataEleClassMorbidity = 2;
                    dataEleClassMortality = 3;
                }

                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = "  CREATE TABLE  " + reportTableName + " ( " +
                          " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                          " [SNO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [Disease] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [M04] [decimal](18, 2) NULL, " +
                          " [M514] [decimal](18, 2) NULL, " +
                          " [M15] [decimal](18, 2) NULL, " +
                          " [F04] [decimal](18, 2) NULL, " +
                          " [F514] [decimal](18, 2) NULL, " +
                          " [F15] [decimal](18, 2) NULL, " +
                          " [MM04] [decimal](18, 2) NULL, " +
                          " [MM514] [decimal](18, 2) NULL, " +
                          " [MM15] [decimal](18, 2) NULL, " +
                          " [MF04] [decimal](18, 2) NULL, " +
                          " [MF514] [decimal](18, 2) NULL, " +
                          " [MF15] [decimal](18, 2) NULL, " +
                          " [Format][int] NULL) ";               

                GenerateReport.globalCreateTable = cmdText;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column M04   varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column M514  varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column M15   varchar(50)  " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column F04   varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column F514  varchar(50)  " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column F15   varchar(50)  " +

                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column MM04   varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column MM514  varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column MM15   varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column MF04   varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column MF514   varchar(50) " +
                              " ALTER TABLE " + reportTableName + "  " +
                              " Alter Column MF15   varchar(50) ";
                //" ALTER TABLE " + reportTableName + "  " +
                // "Alter Column Format int    ";


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }

            // Reset Identity, to resolve performance issues
            //cmdText = "DBCC CHECKIDENT (" + reportTableName + ", reseed, 0)";

            //toExecute = new SqlCommand(cmdText);

            //_helper.Execute(toExecute);
        }

        public void startReportTableGeneration(Boolean first)
        {           
            // Initially delete the tables data

            if (first == true)
            {
                deleteTables();
            }
            else
            {
                if (!level1Cache)
                {
                    if (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease)
                    {
                        string cmdText = "delete from  " + reportTableName;

                        //SqlCommand toExecute;
                        //toExecute = new SqlCommand(cmdText);

                        //_helper.Execute(toExecute);

                        //cmdText = "SELECT * from EthioHIMS_QuarterOPDDiseaseView4";
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        string sno, disease, m04, m514, m15, f04, f514, f15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            format = row["format"].ToString();
                            
                            InsertAggregateData(sno, disease, m04, m514, m15, f04, f514, f15, format);
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease)
                    {
                        string cmdText = "delete from  " + reportTableName;
                       
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        string sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            mm04 = row["mm04"].ToString() as string;
                            mm514 = row["mm514"].ToString() as string;
                            mm15 = row["mm15"].ToString() as string;
                            mf04 = row["mf04"].ToString() as string;
                            mf514 = row["mf514"].ToString() as string;
                            mf15 = row["mf15"].ToString() as string;
                            format = row["format"].ToString();
                            // Call the insert statement

                            InsertAggregateData(sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format);
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease_Facility_Monthly)
                    {
                        string cmdText = "delete from  " + reportTableName;

                        //SqlCommand toExecute;
                        //toExecute = new SqlCommand(cmdText);

                        //_helper.Execute(toExecute);

                        //cmdText = "SELECT * from EthioHIMS_QuarterOPDDiseaseView4";
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        string sno, disease, m04, m514, m15, f04, f514, f15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            format = row["format"].ToString();
                            // Call the insert statement

                            InsertDataOPDFacilityMonthly(sno, disease, m04, m514, m15, f04, f514, f15, format);
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease_Facility_Quarterly)
                    {
                        string cmdText = "delete from  " + reportTableName;

                        //SqlCommand toExecute;
                        //toExecute = new SqlCommand(cmdText);

                        //_helper.Execute(toExecute);

                        //cmdText = "SELECT * from EthioHIMS_QuarterOPDDiseaseView4";
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        string sno, disease, m04, m514, m15, f04, f514, f15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            format = row["format"].ToString();
                            // Call the insert statement

                            InsertDataOPDFacilityQuarterly(sno, disease, m04, m514, m15, f04, f514, f15, format);
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease_Facility_Monthly)
                    {
                        string cmdText = "delete from  " + reportTableName;
                       
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);
                        toExecute.CommandTimeout = 4000; //300 // = 1000000;

                        string sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            mm04 = row["mm04"].ToString() as string;
                            mm514 = row["mm514"].ToString() as string;
                            mm15 = row["mm15"].ToString() as string;
                            mf04 = row["mf04"].ToString() as string;
                            mf514 = row["mf514"].ToString() as string;
                            mf15 = row["mf15"].ToString() as string;
                            format = row["format"].ToString();

                            // Call the insert statement

                            InsertDataIPDFacilityMonthly(sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format);
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease_Facility_Quarterly)
                    {
                        string cmdText = "delete from  " + reportTableName;                     

                        //cmdText = "SELECT * from EthioHIMS_QuarterOPDDiseaseView4";
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        string sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            mm04 = row["mm04"].ToString() as string;
                            mm514 = row["mm514"].ToString() as string;
                            mm15 = row["mm15"].ToString() as string;
                            mf04 = row["mf04"].ToString() as string;
                            mf514 = row["mf514"].ToString() as string;
                            mf15 = row["mf15"].ToString() as string;
                            format = row["format"].ToString();

                            // Call the insert statement

                            InsertDataIPDFacilityQuarterly(sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format);
                        }
                    }
                    else if ((_reportObj.RepKind == reportObject.ReportKind.OPD_HC_Aggregate_Monthly)
                        || (_reportObj.RepKind == reportObject.ReportKind.OPD_HC_Aggregate_Quarterly))
                    {
                        string cmdText = "delete from  " + reportTableName;

                        //SqlCommand toExecute;
                        //toExecute = new SqlCommand(cmdText);

                        //_helper.Execute(toExecute);

                        //cmdText = "SELECT * from EthioHIMS_QuarterOPDDiseaseView4";
                        cmdText = "SELECT * from  " + viewLabeIdTableName;
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        string sno, disease, m04, m514, m15, f04, f514, f15, format;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"] as string;
                            disease = row["disease"] as string;
                            m04 = row["m04"].ToString() as string;
                            m514 = row["m514"].ToString() as string;
                            m15 = row["m15"].ToString() as string;
                            f04 = row["f04"].ToString() as string;
                            f514 = row["f514"].ToString() as string;
                            f15 = row["f15"].ToString() as string;
                            format = row["format"].ToString();
                            // Call the insert statement

                            InsertOPDHCAggregateData(sno, disease, m04, m514, m15, f04, f514, f15, format);
                        }
                    }

                    string cmdTextReportTable = "select * from " + reportTableName;
                    SqlCommand toExecuteReportTable = new SqlCommand(cmdTextReportTable);

                    toExecuteReportTable.CommandTimeout = 4000; //300 // = 1000000;

                    GenerateReport.globalReportDataTable = _helper.GetDataSet(toExecuteReportTable).Tables[0];                   
                }
            }
        }

        // Facility OPD Monthly
        private void InsertDataOPDFacilityMonthly(string sno, string disease, string m04, string m514,
           string m15, string f04, string f514, string f15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);
              
                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";               

                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("format", format);
               
                _helper.Execute(toExecute);
            }
            else
            {
                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly total";
                }


                // 1). The first Entry with the Disease name total
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);
            }
        }

        // OPD Disease Report Facility Quarterly
        private void InsertDataOPDFacilityQuarterly(string sno, string disease, string m04, string m514,
           string m15, string f04, string f514, string f15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);
             
                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";              

                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("format", format);
               
                _helper.Execute(toExecute);
            }
            else
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";

                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly";
                }

                // Disease Total:
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                // New check report Grey color
                if (sno == "")
                    sno = null;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("format", format);
             
                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                if (_reportObj.StartQuarter == _reportObj.EndQuarter)
                {
                    // 1). The First Month in the Quarter               
                    // New check report Grey color
                    //sno = "";
                    sno = null;

                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    //disease = "Month: ";
                    //disease = "Month: " + ethMonth[_startMonth];
                    disease = languageHash["month"].ToString() + ":" + languageHash[ethMonth[_startMonth].ToString().ToLower()].ToString();
                    int mo = _startMonth;
                    string month = mo.ToString();


                    string m04PubTot = m04 + "_" + month; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "-999" : aggregateDataHash[m04PubTot].ToString();
                    string m514PubTot = m514 + "_" + month; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "-999" : aggregateDataHash[m514PubTot].ToString();
                    string m15PubTot = m15 + "_" + month; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "-999" : aggregateDataHash[m15PubTot].ToString();
                    string f04PubTot = f04 + "_" + month; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "-999" : aggregateDataHash[f04PubTot].ToString();
                    string f514PubTot = f514 + "_" + month; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "-999" : aggregateDataHash[f514PubTot].ToString();
                    string f15PubTot = f15 + "_" + month; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "-999" : aggregateDataHash[f15PubTot].ToString();

                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubTot);
                    toExecute.Parameters.AddWithValue("m514", m514PubTot);
                    toExecute.Parameters.AddWithValue("m15", m15PubTot);
                    toExecute.Parameters.AddWithValue("f04", f04PubTot);
                    toExecute.Parameters.AddWithValue("f514", f514PubTot);
                    toExecute.Parameters.AddWithValue("f15", f15PubTot);
                    toExecute.Parameters.AddWithValue("format", format);

                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);

                    // 2). The Second Month in the Quarter
                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    mo = _startMonth + 1;
                    //disease = "Month: " + ethMonth[mo];
                    string theMonth = ethMonth[mo].ToString().ToLower();
                    disease = languageHash["month"].ToString() + ":" + languageHash[theMonth].ToString();
                    month = mo.ToString();                   

                    string m04PubHp = m04 + "_" + month; m04PubHp = (aggregateDataHash[m04PubHp] == null) ? "-999" : aggregateDataHash[m04PubHp].ToString();
                    string m514PubHp = m514 + "_" + month; m514PubHp = (aggregateDataHash[m514PubHp] == null) ? "-999" : aggregateDataHash[m514PubHp].ToString();
                    string m15PubHp = m15 + "_" + month; m15PubHp = (aggregateDataHash[m15PubHp] == null) ? "-999" : aggregateDataHash[m15PubHp].ToString();
                    string f04PubHp = f04 + "_" + month; f04PubHp = (aggregateDataHash[f04PubHp] == null) ? "-999" : aggregateDataHash[f04PubHp].ToString();
                    string f514PubHp = f514 + "_" + month; f514PubHp = (aggregateDataHash[f514PubHp] == null) ? "-999" : aggregateDataHash[f514PubHp].ToString();
                    string f15PubHp = f15 + "_" + month; f15PubHp = (aggregateDataHash[f15PubHp] == null) ? "-999" : aggregateDataHash[f15PubHp].ToString();


                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubHp);
                    toExecute.Parameters.AddWithValue("m514", m514PubHp);
                    toExecute.Parameters.AddWithValue("m15", m15PubHp);
                    toExecute.Parameters.AddWithValue("f04", f04PubHp);
                    toExecute.Parameters.AddWithValue("f514", f514PubHp);
                    toExecute.Parameters.AddWithValue("f15", f15PubHp);
                    toExecute.Parameters.AddWithValue("format", format);


                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);

                    // 3). The Third Month in the Quarter
                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    mo = _endMonth;
                    //disease = "Month: " + ethMonth[mo];
                    theMonth = ethMonth[mo].ToString().ToLower();
                    disease = languageHash["month"].ToString() + ":" + languageHash[theMonth].ToString();
                    month = mo.ToString();

                    string m04PubHc = m04 + "_" + month; m04PubHc = (aggregateDataHash[m04PubHc] == null) ? "-999" : aggregateDataHash[m04PubHc].ToString();
                    string m514PubHc = m514 + "_" + month; m514PubHc = (aggregateDataHash[m514PubHc] == null) ? "-999" : aggregateDataHash[m514PubHc].ToString();
                    string m15PubHc = m15 + "_" + month; m15PubHc = (aggregateDataHash[m15PubHc] == null) ? "-999" : aggregateDataHash[m15PubHc].ToString();
                    string f04PubHc = f04 + "_" + month; f04PubHc = (aggregateDataHash[f04PubHc] == null) ? "-999" : aggregateDataHash[f04PubHc].ToString();
                    string f514PubHc = f514 + "_" + month; f514PubHc = (aggregateDataHash[f514PubHc] == null) ? "-999" : aggregateDataHash[f514PubHc].ToString();
                    string f15PubHc = f15 + "_" + month; f15PubHc = (aggregateDataHash[f15PubHc] == null) ? "-999" : aggregateDataHash[f15PubHc].ToString();

                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubHc);
                    toExecute.Parameters.AddWithValue("m514", m514PubHc);
                    toExecute.Parameters.AddWithValue("m15", m15PubHc);
                    toExecute.Parameters.AddWithValue("f04", f04PubHc);
                    toExecute.Parameters.AddWithValue("f514", f514PubHc);
                    toExecute.Parameters.AddWithValue("f15", f15PubHc);
                    toExecute.Parameters.AddWithValue("format", format);              

                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);
                }
            }
        }

        // OPD Disease Report (Main one for higher Admin Levels)
        private void InsertAggregateData(string sno, string disease, string m04, string m514,
            string m15, string f04, string f514, string f15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);         

                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";               

                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("format", format);
               
                _helper.Execute(toExecute);
            }
            else
            {
                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly";
                }

                // 1). The first Entry with the Disease name total
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @format)";


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04 + "TOT"; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514 + "TOT"; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15 + "TOT"; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04 + "TOT"; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514 + "TOT"; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15 + "TOT"; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 2). The Second Entry with the Public Facilities Total
                sno = "";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                disease = languageHash["publicFacilitiesTotal"].ToString(); //  Public Facilities – total";

                string m04PubTot = m04 + "PUBTOT"; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "-999" : aggregateDataHash[m04PubTot].ToString();
                string m514PubTot = m514 + "PUBTOT"; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "-999" : aggregateDataHash[m514PubTot].ToString();
                string m15PubTot = m15 + "PUBTOT"; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "-999" : aggregateDataHash[m15PubTot].ToString();
                string f04PubTot = f04 + "PUBTOT"; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "-999" : aggregateDataHash[f04PubTot].ToString();
                string f514PubTot = f514 + "PUBTOT"; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "-999" : aggregateDataHash[f514PubTot].ToString();
                string f15PubTot = f15 + "PUBTOT"; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "-999" : aggregateDataHash[f15PubTot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubTot);
                toExecute.Parameters.AddWithValue("m514", m514PubTot);
                toExecute.Parameters.AddWithValue("m15", m15PubTot);
                toExecute.Parameters.AddWithValue("f04", f04PubTot);
                toExecute.Parameters.AddWithValue("f514", f514PubTot);
                toExecute.Parameters.AddWithValue("f15", f15PubTot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 3). The Third Entry with the Public Facilities Total              

                if (!healthPost_NoDisplay.Contains(m04))
                {
                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    //disease = "          " + "Health Posts";
                    disease = languageHash["healthPosts"].ToString();

                    //(CheckNull(row["VCT_Refered_For_Tb"])) ? false : Boolean.Parse(row["VCT_Refered_For_Tb"].ToString())

                    string m04PubHp = m04 + "PUBHP"; m04PubHp = (aggregateDataHash[m04PubHp] == null) ? "-999" : aggregateDataHash[m04PubHp].ToString();
                    string m514PubHp = m514 + "PUBHP"; m514PubHp = (aggregateDataHash[m514PubHp] == null) ? "-999" : aggregateDataHash[m514PubHp].ToString();
                    string m15PubHp = m15 + "PUBHP"; m15PubHp = (aggregateDataHash[m15PubHp] == null) ? "-999" : aggregateDataHash[m15PubHp].ToString();
                    string f04PubHp = f04 + "PUBHP"; f04PubHp = (aggregateDataHash[f04PubHp] == null) ? "-999" : aggregateDataHash[f04PubHp].ToString();
                    string f514PubHp = f514 + "PUBHP"; f514PubHp = (aggregateDataHash[f514PubHp] == null) ? "-999" : aggregateDataHash[f514PubHp].ToString();
                    string f15PubHp = f15 + "PUBHP"; f15PubHp = (aggregateDataHash[f15PubHp] == null) ? "-999" : aggregateDataHash[f15PubHp].ToString();


                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubHp);
                    toExecute.Parameters.AddWithValue("m514", m514PubHp);
                    toExecute.Parameters.AddWithValue("m15", m15PubHp);
                    toExecute.Parameters.AddWithValue("f04", f04PubHp);
                    toExecute.Parameters.AddWithValue("f514", f514PubHp);
                    toExecute.Parameters.AddWithValue("f15", f15PubHp);
                    toExecute.Parameters.AddWithValue("format", format);


                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);
                }

                // 4). The Fourth Entry Health Centers
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Health Centers";
                disease = languageHash["healthCenters"].ToString();

                string m04PubHc = m04 + "PUBHC"; m04PubHc = (aggregateDataHash[m04PubHc] == null) ? "-999" : aggregateDataHash[m04PubHc].ToString();
                string m514PubHc = m514 + "PUBHC"; m514PubHc = (aggregateDataHash[m514PubHc] == null) ? "-999" : aggregateDataHash[m514PubHc].ToString();
                string m15PubHc = m15 + "PUBHC"; m15PubHc = (aggregateDataHash[m15PubHc] == null) ? "-999" : aggregateDataHash[m15PubHc].ToString();
                string f04PubHc = f04 + "PUBHC"; f04PubHc = (aggregateDataHash[f04PubHc] == null) ? "-999" : aggregateDataHash[f04PubHc].ToString();
                string f514PubHc = f514 + "PUBHC"; f514PubHc = (aggregateDataHash[f514PubHc] == null) ? "-999" : aggregateDataHash[f514PubHc].ToString();
                string f15PubHc = f15 + "PUBHC"; f15PubHc = (aggregateDataHash[f15PubHc] == null) ? "-999" : aggregateDataHash[f15PubHc].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubHc);
                toExecute.Parameters.AddWithValue("m514", m514PubHc);
                toExecute.Parameters.AddWithValue("m15", m15PubHc);
                toExecute.Parameters.AddWithValue("f04", f04PubHc);
                toExecute.Parameters.AddWithValue("f514", f514PubHc);
                toExecute.Parameters.AddWithValue("f15", f15PubHc);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);


                // 5). The fifth Entry with Public Hospitals
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Hospitals";
                disease = languageHash["hospitals"].ToString();

                string m04PubHos = m04 + "PUBHOS"; m04PubHos = (aggregateDataHash[m04PubHos] == null) ? "-999" : aggregateDataHash[m04PubHos].ToString();
                string m514PubHos = m514 + "PUBHOS"; m514PubHos = (aggregateDataHash[m514PubHos] == null) ? "-999" : aggregateDataHash[m514PubHos].ToString();
                string m15PubHos = m15 + "PUBHOS"; m15PubHos = (aggregateDataHash[m15PubHos] == null) ? "-999" : aggregateDataHash[m15PubHos].ToString();
                string f04PubHos = f04 + "PUBHOS"; f04PubHos = (aggregateDataHash[f04PubHos] == null) ? "-999" : aggregateDataHash[f04PubHos].ToString();
                string f514PubHos = f514 + "PUBHOS"; f514PubHos = (aggregateDataHash[f514PubHos] == null) ? "-999" : aggregateDataHash[f514PubHos].ToString();
                string f15PubHos = f15 + "PUBHOS"; f15PubHos = (aggregateDataHash[f15PubHos] == null) ? "-999" : aggregateDataHash[f15PubHos].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubHos);
                toExecute.Parameters.AddWithValue("m514", m514PubHos);
                toExecute.Parameters.AddWithValue("m15", m15PubHos);
                toExecute.Parameters.AddWithValue("f04", f04PubHos);
                toExecute.Parameters.AddWithValue("f514", f514PubHos);
                toExecute.Parameters.AddWithValue("f15", f15PubHos);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);


                // 6). The sixth Entry with Private not for profit Facilities – total
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "  " + "Private not for profit Facilities – total";
                disease = languageHash["privateNotForProfitFacilitiesTotal"].ToString();

                string m04PriNoProTot = m04 + "PRINOPROTOT"; m04PriNoProTot = (aggregateDataHash[m04PriNoProTot] == null) ? "-999" : aggregateDataHash[m04PriNoProTot].ToString();
                string m514PriNoProTot = m514 + "PRINOPROTOT"; m514PriNoProTot = (aggregateDataHash[m514PriNoProTot] == null) ? "-999" : aggregateDataHash[m514PriNoProTot].ToString();
                string m15PriNoProTot = m15 + "PRINOPROTOT"; m15PriNoProTot = (aggregateDataHash[m15PriNoProTot] == null) ? "-999" : aggregateDataHash[m15PriNoProTot].ToString();
                string f04PriNoProTot = f04 + "PRINOPROTOT"; f04PriNoProTot = (aggregateDataHash[f04PriNoProTot] == null) ? "-999" : aggregateDataHash[f04PriNoProTot].ToString();
                string f514PriNoProTot = f514 + "PRINOPROTOT"; f514PriNoProTot = (aggregateDataHash[f514PriNoProTot] == null) ? "-999" : aggregateDataHash[f514PriNoProTot].ToString();
                string f15PriNoProTot = f15 + "PRINOPROTOT"; f15PriNoProTot = (aggregateDataHash[f15PriNoProTot] == null) ? "-999" : aggregateDataHash[f15PriNoProTot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriNoProTot);
                toExecute.Parameters.AddWithValue("m514", m514PriNoProTot);
                toExecute.Parameters.AddWithValue("m15", m15PriNoProTot);
                toExecute.Parameters.AddWithValue("f04", f04PriNoProTot);
                toExecute.Parameters.AddWithValue("f514", f514PriNoProTot);
                toExecute.Parameters.AddWithValue("f15", f15PriNoProTot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 7). The seventh Entry with Private not for profit Facilities – clinics
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Clinics";
                disease = languageHash["clinics"].ToString();

                string m04PriNoProCli = m04 + "PRINOPROCLI"; m04PriNoProCli = (aggregateDataHash[m04PriNoProCli] == null) ? "-999" : aggregateDataHash[m04PriNoProCli].ToString();
                string m514PriNoProCli = m514 + "PRINOPROCLI"; m514PriNoProCli = (aggregateDataHash[m514PriNoProCli] == null) ? "-999" : aggregateDataHash[m514PriNoProCli].ToString();
                string m15PriNoProCli = m15 + "PRINOPROCLI"; m15PriNoProCli = (aggregateDataHash[m15PriNoProCli] == null) ? "-999" : aggregateDataHash[m15PriNoProCli].ToString();
                string f04PriNoProCli = f04 + "PRINOPROCLI"; f04PriNoProCli = (aggregateDataHash[f04PriNoProCli] == null) ? "-999" : aggregateDataHash[f04PriNoProCli].ToString();
                string f514PriNoProCli = f514 + "PRINOPROCLI"; f514PriNoProCli = (aggregateDataHash[f514PriNoProCli] == null) ? "-999" : aggregateDataHash[f514PriNoProCli].ToString();
                string f15PriNoProCli = f15 + "PRINOPROCLI"; f15PriNoProCli = (aggregateDataHash[f15PriNoProCli] == null) ? "-999" : aggregateDataHash[f15PriNoProCli].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriNoProCli);
                toExecute.Parameters.AddWithValue("m514", m514PriNoProCli);
                toExecute.Parameters.AddWithValue("m15", m15PriNoProCli);
                toExecute.Parameters.AddWithValue("f04", f04PriNoProCli);
                toExecute.Parameters.AddWithValue("f514", f514PriNoProCli);
                toExecute.Parameters.AddWithValue("f15", f15PriNoProCli);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 8). The Eight Entry with Private not for profit Facilities – Hospitals
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Hospitals";
                disease = languageHash["hospitals"].ToString();

                string m04PriNoProHos = m04 + "PRINOPROHOS"; m04PriNoProHos = (aggregateDataHash[m04PriNoProHos] == null) ? "-999" : aggregateDataHash[m04PriNoProHos].ToString();
                string m514PriNoProHos = m514 + "PRINOPROHOS"; m514PriNoProHos = (aggregateDataHash[m514PriNoProHos] == null) ? "-999" : aggregateDataHash[m514PriNoProHos].ToString();
                string m15PriNoProHos = m15 + "PRINOPROHOS"; m15PriNoProHos = (aggregateDataHash[m15PriNoProHos] == null) ? "-999" : aggregateDataHash[m15PriNoProHos].ToString();
                string f04PriNoProHos = f04 + "PRINOPROHOS"; f04PriNoProHos = (aggregateDataHash[f04PriNoProHos] == null) ? "-999" : aggregateDataHash[f04PriNoProHos].ToString();
                string f514PriNoProHos = f514 + "PRINOPROHOS"; f514PriNoProHos = (aggregateDataHash[f514PriNoProHos] == null) ? "-999" : aggregateDataHash[f514PriNoProHos].ToString();
                string f15PriNoProHos = f15 + "PRINOPROHOS"; f15PriNoProHos = (aggregateDataHash[f15PriNoProHos] == null) ? "-999" : aggregateDataHash[f15PriNoProHos].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriNoProHos);
                toExecute.Parameters.AddWithValue("m514", m514PriNoProHos);
                toExecute.Parameters.AddWithValue("m15", m15PriNoProHos);
                toExecute.Parameters.AddWithValue("f04", f04PriNoProHos);
                toExecute.Parameters.AddWithValue("f514", f514PriNoProHos);
                toExecute.Parameters.AddWithValue("f15", f15PriNoProHos);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 9). The Eight Entry with Private for profit Facilities – total 
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "  " + "Private for profit Facilities – total ";
                disease = languageHash["privateForProfitFacilitiesTotal"].ToString();

                string m04PriProTot = m04 + "PRIPROTOT"; m04PriProTot = (aggregateDataHash[m04PriProTot] == null) ? "-999" : aggregateDataHash[m04PriProTot].ToString();
                string m514PriProTot = m514 + "PRIPROTOT"; m514PriProTot = (aggregateDataHash[m514PriProTot] == null) ? "-999" : aggregateDataHash[m514PriProTot].ToString();
                string m15PriProTot = m15 + "PRIPROTOT"; m15PriProTot = (aggregateDataHash[m15PriProTot] == null) ? "-999" : aggregateDataHash[m15PriProTot].ToString();
                string f04PriProTot = f04 + "PRIPROTOT"; f04PriProTot = (aggregateDataHash[f04PriProTot] == null) ? "-999" : aggregateDataHash[f04PriProTot].ToString();
                string f514PriProTot = f514 + "PRIPROTOT"; f514PriProTot = (aggregateDataHash[f514PriProTot] == null) ? "-999" : aggregateDataHash[f514PriProTot].ToString();
                string f15PriProTot = f15 + "PRIPROTOT"; f15PriProTot = (aggregateDataHash[f15PriProTot] == null) ? "-999" : aggregateDataHash[f15PriProTot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriProTot);
                toExecute.Parameters.AddWithValue("m514", m514PriProTot);
                toExecute.Parameters.AddWithValue("m15", m15PriProTot);
                toExecute.Parameters.AddWithValue("f04", f04PriProTot);
                toExecute.Parameters.AddWithValue("f514", f514PriProTot);
                toExecute.Parameters.AddWithValue("f15", f15PriProTot);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 10). The Eight Entry with Private for profit Facilities – Clinics
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Clinics";
                disease = languageHash["clinics"].ToString();

                string m04PriProCli = m04 + "PRIPROCLI"; m04PriProCli = (aggregateDataHash[m04PriProCli] == null) ? "-999" : aggregateDataHash[m04PriProCli].ToString();
                string m514PriProCli = m514 + "PRIPROCLI"; m514PriProCli = (aggregateDataHash[m514PriProCli] == null) ? "-999" : aggregateDataHash[m514PriProCli].ToString();
                string m15PriProCli = m15 + "PRIPROCLI"; m15PriProCli = (aggregateDataHash[m15PriProCli] == null) ? "-999" : aggregateDataHash[m15PriProCli].ToString();
                string f04PriProCli = f04 + "PRIPROCLI"; f04PriProCli = (aggregateDataHash[f04PriProCli] == null) ? "-999" : aggregateDataHash[f04PriProCli].ToString();
                string f514PriProCli = f514 + "PRIPROCLI"; f514PriProCli = (aggregateDataHash[f514PriProCli] == null) ? "-999" : aggregateDataHash[f514PriProCli].ToString();
                string f15PriProCli = f15 + "PRIPROCLI"; f15PriProCli = (aggregateDataHash[f15PriProCli] == null) ? "-999" : aggregateDataHash[f15PriProCli].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriProCli);
                toExecute.Parameters.AddWithValue("m514", m514PriProCli);
                toExecute.Parameters.AddWithValue("m15", m15PriProCli);
                toExecute.Parameters.AddWithValue("f04", f04PriProCli);
                toExecute.Parameters.AddWithValue("f514", f514PriProCli);
                toExecute.Parameters.AddWithValue("f15", f15PriProCli);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 11). The Eight Entry with Private for profit Facilities – Hospitals
                cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Hospitals";
                disease = languageHash["hospitals"].ToString();

                string m04PriProHos = m04 + "PRIPROHOS"; m04PriProHos = (aggregateDataHash[m04PriProHos] == null) ? "-999" : aggregateDataHash[m04PriProHos].ToString();
                string m514PriProHos = m514 + "PRIPROHOS"; m514PriProHos = (aggregateDataHash[m514PriProHos] == null) ? "-999" : aggregateDataHash[m514PriProHos].ToString();
                string m15PriProHos = m15 + "PRIPROHOS"; m15PriProHos = (aggregateDataHash[m15PriProHos] == null) ? "-999" : aggregateDataHash[m15PriProHos].ToString();
                string f04PriProHos = f04 + "PRIPROHOS"; f04PriProHos = (aggregateDataHash[f04PriProHos] == null) ? "-999" : aggregateDataHash[f04PriProHos].ToString();
                string f514PriProHos = f514 + "PRIPROHOS"; f514PriProHos = (aggregateDataHash[f514PriProHos] == null) ? "-999" : aggregateDataHash[f514PriProHos].ToString();
                string f15PriProHos = f15 + "PRIPROHOS"; f15PriProHos = (aggregateDataHash[f15PriProHos] == null) ? "-999" : aggregateDataHash[f15PriProHos].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriProHos);
                toExecute.Parameters.AddWithValue("m514", m514PriProHos);
                toExecute.Parameters.AddWithValue("m15", m15PriProHos);
                toExecute.Parameters.AddWithValue("f04", f04PriProHos);
                toExecute.Parameters.AddWithValue("f514", f514PriProHos);
                toExecute.Parameters.AddWithValue("f15", f15PriProHos);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);
            }
        }

        // Facility Monthly IPD Disease Report
        private void InsertDataIPDFacilityMonthly(string sno, string disease, string m04, string m514,
         string m15, string f04, string f514, string f15, string mm04, string mm514,
          string mm15, string mf04, string mf514, string mf15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @mm04, @mm514, @mm15, @mf04, @mf514, @mf15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);
            
                m04 = "-998";
                m514 = "-998";
                m15 = "-998";
                f04 = "-998";
                f514 = "-998";
                f15 = "-998";
                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";
                mm04 = "-999";
                mm514 = "-999";
                mm15 = "-999";
                mf04 = "-999";
                mf514 = "-999";
                mf15 = "-999";
                //m04 = "-998";
                //m514 = "-998";
                //m15 = "-998";
                //f04 = "-998";
                //f514 = "-998";
                //f15 = "-998";
                //m04 = "-998";
                //m514 = "-998";
                //m15 = "-998";
                //f04 = "-998";
                //f514 = "-998";
                //f15 = "-998";
                //mm04 = "-998";
                //mm514 = "-998";
                //mm15 = "-998";
                //mf04 = "-998";
                //mf514 = "-998";
                //mf15 = "-998";


                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("mm04", mm04);
                toExecute.Parameters.AddWithValue("mm514", mm514);
                toExecute.Parameters.AddWithValue("mm15", mm15);
                toExecute.Parameters.AddWithValue("mf04", mf04);
                toExecute.Parameters.AddWithValue("mf514", mf514);
                toExecute.Parameters.AddWithValue("mf15", mf15);
                toExecute.Parameters.AddWithValue("format", format);
              
                _helper.Execute(toExecute);
            }
            else
            {
                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly";
                }


                // 1). The first Entry with the Disease name total
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @mm04, @mm514, @mm15, @mf04, @mf514, @mf15,@format)";


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();
                string mm04Tot = "MM" + mm04; mm04Tot = (aggregateDataHash[mm04Tot] == null) ? "-999" : aggregateDataHash[mm04Tot].ToString();
                string mm514Tot = "MM" + mm514; mm514Tot = (aggregateDataHash[mm514Tot] == null) ? "-999" : aggregateDataHash[mm514Tot].ToString();
                string mm15Tot = "MM" + mm15; mm15Tot = (aggregateDataHash[mm15Tot] == null) ? "-999" : aggregateDataHash[mm15Tot].ToString();
                string mf04Tot = "MM" + mf04; mf04Tot = (aggregateDataHash[mf04Tot] == null) ? "-999" : aggregateDataHash[mf04Tot].ToString();
                string mf514Tot = "MM" + mf514; mf514Tot = (aggregateDataHash[mf514Tot] == null) ? "-999" : aggregateDataHash[mf514Tot].ToString();
                string mf15Tot = "MM" + mf15; mf15Tot = (aggregateDataHash[mf15Tot] == null) ? "-999" : aggregateDataHash[mf15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("mm04", mm04Tot);
                toExecute.Parameters.AddWithValue("mm514", mm514Tot);
                toExecute.Parameters.AddWithValue("mm15", mm15Tot);
                toExecute.Parameters.AddWithValue("mf04", mf04Tot);
                toExecute.Parameters.AddWithValue("mf514", mf514Tot);
                toExecute.Parameters.AddWithValue("mf15", mf15Tot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);
            }
        }

        // Facility Quarterly IPD Disease Report
        private void InsertDataIPDFacilityQuarterly(string sno, string disease, string m04, string m514,
         string m15, string f04, string f514, string f15, string mm04, string mm514,
          string mm15, string mf04, string mf514, string mf15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @mm04, @mm514, @mm15, @mf04, @mf514, @mf15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);
         
                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";
                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";
                mm04 = "-999";
                mm514 = "-999";
                mm15 = "-999";
                mf04 = "-999";
                mf514 = "-999";
                mf15 = "-999";
                //m04 = "-998";
                //m514 = "-998";
                //m15 = "-998";
                //f04 = "-998";
                //f514 = "-998";
                //f15 = "-998";
                //m04 = "-998";
                //m514 = "-998";
                //m15 = "-998";
                //f04 = "-998";
                //f514 = "-998";
                //f15 = "-998";
                //mm04 = "-998";
                //mm514 = "-998";
                //mm15 = "-998";
                //mf04 = "-998";
                //mf514 = "-998";
                //mf15 = "-998";



                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("mm04", mm04);
                toExecute.Parameters.AddWithValue("mm514", mm514);
                toExecute.Parameters.AddWithValue("mm15", mm15);
                toExecute.Parameters.AddWithValue("mf04", mf04);
                toExecute.Parameters.AddWithValue("mf514", mf514);
                toExecute.Parameters.AddWithValue("mf15", mf15);
                toExecute.Parameters.AddWithValue("format", format);
                //foreach (SqlParameter Parameter in toExecute.Parameters)
                //{
                //    if (Parameter.Value == null)
                //    {
                //        Parameter.Value = DBNull.Value;
                //    }
                //}
                _helper.Execute(toExecute);
            }
            else
            {
                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly";
                }

                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @mm04, @mm514, @mm15, @mf04, @mf514, @mf15,@format)";

                // Disease Total 
                // New check report Grey color

                if (sno == "")
                    sno = null;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();
                string mm04Tot = "MM" + mm04; mm04Tot = (aggregateDataHash[mm04Tot] == null) ? "-999" : aggregateDataHash[mm04Tot].ToString();
                string mm514Tot = "MM" + mm514; mm514Tot = (aggregateDataHash[mm514Tot] == null) ? "-999" : aggregateDataHash[mm514Tot].ToString();
                string mm15Tot = "MM" + mm15; mm15Tot = (aggregateDataHash[mm15Tot] == null) ? "-999" : aggregateDataHash[mm15Tot].ToString();
                string mf04Tot = "MM" + mf04; mf04Tot = (aggregateDataHash[mf04Tot] == null) ? "-999" : aggregateDataHash[mf04Tot].ToString();
                string mf514Tot = "MM" + mf514; mf514Tot = (aggregateDataHash[mf514Tot] == null) ? "-999" : aggregateDataHash[mf514Tot].ToString();
                string mf15Tot = "MM" + mf15; mf15Tot = (aggregateDataHash[mf15Tot] == null) ? "-999" : aggregateDataHash[mf15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("mm04", mm04Tot);
                toExecute.Parameters.AddWithValue("mm514", mm514Tot);
                toExecute.Parameters.AddWithValue("mm15", mm15Tot);
                toExecute.Parameters.AddWithValue("mf04", mf04Tot);
                toExecute.Parameters.AddWithValue("mf514", mf514Tot);
                toExecute.Parameters.AddWithValue("mf15", mf15Tot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                if (_reportObj.StartQuarter == _reportObj.EndQuarter)
                {
                    // 1). The First Month in the Quarter
                    // New check report Grey color
                    //sno = "";
                    sno = null;

                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    //disease = "Month: " + ethMonth[_startMonth];
                    string theMonth = ethMonth[_startMonth].ToString().ToLower();
                    disease = languageHash["month"].ToString() + ":" + languageHash[theMonth].ToString();
                    int mo = _startMonth;
                    string month = mo.ToString();

                    string m04PubTot = m04 + "_" + month; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "-999" : aggregateDataHash[m04PubTot].ToString();
                    string m514PubTot = m514 + "_" + month; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "-999" : aggregateDataHash[m514PubTot].ToString();
                    string m15PubTot = m15 + "_" + month; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "-999" : aggregateDataHash[m15PubTot].ToString();
                    string f04PubTot = f04 + "_" + month; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "-999" : aggregateDataHash[f04PubTot].ToString();
                    string f514PubTot = f514 + "_" + month; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "-999" : aggregateDataHash[f514PubTot].ToString();
                    string f15PubTot = f15 + "_" + month; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "-999" : aggregateDataHash[f15PubTot].ToString();
                    string mm04PubTot = "MM" + mm04 + "_" + month; mm04PubTot = (aggregateDataHash[mm04PubTot] == null) ? "-999" : aggregateDataHash[mm04PubTot].ToString();
                    string mm514PubTot = "MM" + mm514 + "_" + month; mm514PubTot = (aggregateDataHash[mm514PubTot] == null) ? "-999" : aggregateDataHash[mm514PubTot].ToString();
                    string mm15PubTot = "MM" + mm15 + "_" + month; mm15PubTot = (aggregateDataHash[mm15PubTot] == null) ? "-999" : aggregateDataHash[mm15PubTot].ToString();
                    string mf04PubTot = "MM" + mf04 + "_" + month; mf04PubTot = (aggregateDataHash[mf04PubTot] == null) ? "-999" : aggregateDataHash[mf04PubTot].ToString();
                    string mf514PubTot = "MM" + mf514 + "_" + month; mf514PubTot = (aggregateDataHash[mf514PubTot] == null) ? "-999" : aggregateDataHash[mf514PubTot].ToString();
                    string mf15PubTot = "MM" + mf15 + "_" + month; mf15PubTot = (aggregateDataHash[mf15PubTot] == null) ? "-999" : aggregateDataHash[mf15PubTot].ToString();

                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubTot);
                    toExecute.Parameters.AddWithValue("m514", m514PubTot);
                    toExecute.Parameters.AddWithValue("m15", m15PubTot);
                    toExecute.Parameters.AddWithValue("f04", f04PubTot);
                    toExecute.Parameters.AddWithValue("f514", f514PubTot);
                    toExecute.Parameters.AddWithValue("f15", f15PubTot);
                    toExecute.Parameters.AddWithValue("mm04", mm04PubTot);
                    toExecute.Parameters.AddWithValue("mm514", mm514PubTot);
                    toExecute.Parameters.AddWithValue("mm15", mm15PubTot);
                    toExecute.Parameters.AddWithValue("mf04", mf04PubTot);
                    toExecute.Parameters.AddWithValue("mf514", mf514PubTot);
                    toExecute.Parameters.AddWithValue("mf15", mf15PubTot);
                    toExecute.Parameters.AddWithValue("format", format);


                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);

                    // 2). The Second Month in the Quarter
                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    //disease = "Month: " + ethMonth[_startMonth + 1];
                    theMonth = ethMonth[_startMonth + 1].ToString().ToLower();
                    disease = languageHash["month"].ToString() + ":" + languageHash[theMonth].ToString();
                    mo = _startMonth + 1;
                    month = mo.ToString();

                    //(CheckNull(row["VCT_Refered_For_Tb"])) ? false : Boolean.Parse(row["VCT_Refered_For_Tb"].ToString())

                    string m04PubHp = m04 + "_" + month; m04PubHp = (aggregateDataHash[m04PubHp] == null) ? "-999" : aggregateDataHash[m04PubHp].ToString();
                    string m514PubHp = m514 + "_" + month; m514PubHp = (aggregateDataHash[m514PubHp] == null) ? "-999" : aggregateDataHash[m514PubHp].ToString();
                    string m15PubHp = m15 + "_" + month; m15PubHp = (aggregateDataHash[m15PubHp] == null) ? "-999" : aggregateDataHash[m15PubHp].ToString();
                    string f04PubHp = f04 + "_" + month; f04PubHp = (aggregateDataHash[f04PubHp] == null) ? "-999" : aggregateDataHash[f04PubHp].ToString();
                    string f514PubHp = f514 + "_" + month; f514PubHp = (aggregateDataHash[f514PubHp] == null) ? "-999" : aggregateDataHash[f514PubHp].ToString();
                    string f15PubHp = f15 + "_" + month; f15PubHp = (aggregateDataHash[f15PubHp] == null) ? "-999" : aggregateDataHash[f15PubHp].ToString();
                    string mm04PubHp = "MM" + mm04 + "_" + month; mm04PubHp = (aggregateDataHash[mm04PubHp] == null) ? "-999" : aggregateDataHash[mm04PubHp].ToString();
                    string mm514PubHp = "MM" + mm514 + "_" + month; mm514PubHp = (aggregateDataHash[mm514PubHp] == null) ? "-999" : aggregateDataHash[mm514PubHp].ToString();
                    string mm15PubHp = "MM" + mm15 + "_" + month; mm15PubHp = (aggregateDataHash[mm15PubHp] == null) ? "-999" : aggregateDataHash[mm15PubHp].ToString();
                    string mf04PubHp = "MM" + mf04 + "_" + month; mf04PubHp = (aggregateDataHash[mf04PubHp] == null) ? "-999" : aggregateDataHash[mf04PubHp].ToString();
                    string mf514PubHp = "MM" + mf514 + "_" + month; mf514PubHp = (aggregateDataHash[mf514PubHp] == null) ? "-999" : aggregateDataHash[mf514PubHp].ToString();
                    string mf15PubHp = "MM" + mf15 + "_" + month; mf15PubHp = (aggregateDataHash[mf15PubHp] == null) ? "-999" : aggregateDataHash[mf15PubHp].ToString();


                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubHp);
                    toExecute.Parameters.AddWithValue("m514", m514PubHp);
                    toExecute.Parameters.AddWithValue("m15", m15PubHp);
                    toExecute.Parameters.AddWithValue("f04", f04PubHp);
                    toExecute.Parameters.AddWithValue("f514", f514PubHp);
                    toExecute.Parameters.AddWithValue("f15", f15PubHp);
                    toExecute.Parameters.AddWithValue("mm04", mm04PubHp);
                    toExecute.Parameters.AddWithValue("mm514", mm514PubHp);
                    toExecute.Parameters.AddWithValue("mm15", mm15PubHp);
                    toExecute.Parameters.AddWithValue("mf04", mf04PubHp);
                    toExecute.Parameters.AddWithValue("mf514", mf514PubHp);
                    toExecute.Parameters.AddWithValue("mf15", mf15PubHp);
                    toExecute.Parameters.AddWithValue("format", format);


                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);

                    // 3). The Third Month in the Quarter
                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    toExecute.Parameters.AddWithValue("sno", sno);

                    //disease = "Month: " + ethMonth[_endMonth];
                    theMonth = ethMonth[_endMonth].ToString().ToLower();
                    disease = languageHash["month"].ToString() + ":" + languageHash[theMonth].ToString();
                    mo = _endMonth;
                    month = mo.ToString();

                    string m04PubHc = m04 + "_" + month; m04PubHc = (aggregateDataHash[m04PubHc] == null) ? "-999" : aggregateDataHash[m04PubHc].ToString();
                    string m514PubHc = m514 + "_" + month; m514PubHc = (aggregateDataHash[m514PubHc] == null) ? "-999" : aggregateDataHash[m514PubHc].ToString();
                    string m15PubHc = m15 + "_" + month; m15PubHc = (aggregateDataHash[m15PubHc] == null) ? "-999" : aggregateDataHash[m15PubHc].ToString();
                    string f04PubHc = f04 + "_" + month; f04PubHc = (aggregateDataHash[f04PubHc] == null) ? "-999" : aggregateDataHash[f04PubHc].ToString();
                    string f514PubHc = f514 + "_" + month; f514PubHc = (aggregateDataHash[f514PubHc] == null) ? "-999" : aggregateDataHash[f514PubHc].ToString();
                    string f15PubHc = f15 + "_" + month; f15PubHc = (aggregateDataHash[f15PubHc] == null) ? "-999" : aggregateDataHash[f15PubHc].ToString();
                    string mm04PubHc = "MM" + mm04 + "_" + month; mm04PubHc = (aggregateDataHash[mm04PubHc] == null) ? "-999" : aggregateDataHash[mm04PubHc].ToString();
                    string mm514PubHc = "MM" + mm514 + "_" + month; mm514PubHc = (aggregateDataHash[mm514PubHc] == null) ? "-999" : aggregateDataHash[mm514PubHc].ToString();
                    string mm15PubHc = "MM" + mm15 + "_" + month; mm15PubHc = (aggregateDataHash[mm15PubHc] == null) ? "-999" : aggregateDataHash[mm15PubHc].ToString();
                    string mf04PubHc = "MM" + mf04 + "_" + month; mf04PubHc = (aggregateDataHash[mf04PubHc] == null) ? "-999" : aggregateDataHash[mf04PubHc].ToString();
                    string mf514PubHc = "MM" + mf514 + "_" + month; mf514PubHc = (aggregateDataHash[mf514PubHc] == null) ? "-999" : aggregateDataHash[mf514PubHc].ToString();
                    string mf15PubHc = "MM" + mf15 + "_" + month; mf15PubHc = (aggregateDataHash[mf15PubHc] == null) ? "-999" : aggregateDataHash[mf15PubHc].ToString();

                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubHc);
                    toExecute.Parameters.AddWithValue("m514", m514PubHc);
                    toExecute.Parameters.AddWithValue("m15", m15PubHc);
                    toExecute.Parameters.AddWithValue("f04", f04PubHc);
                    toExecute.Parameters.AddWithValue("f514", f514PubHc);
                    toExecute.Parameters.AddWithValue("f15", f15PubHc);
                    toExecute.Parameters.AddWithValue("mm04", mm04PubHc);
                    toExecute.Parameters.AddWithValue("mm514", mm514PubHc);
                    toExecute.Parameters.AddWithValue("mm15", mm15PubHc);
                    toExecute.Parameters.AddWithValue("mf04", mf04PubHc);
                    toExecute.Parameters.AddWithValue("mf514", mf514PubHc);
                    toExecute.Parameters.AddWithValue("mf15", mf15PubHc);
                    toExecute.Parameters.AddWithValue("format", format);


                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);
                }
            }
        }

        // For IPD Disease Report
        private void InsertAggregateData(string sno, string disease, string m04, string m514,
           string m15, string f04, string f514, string f15, string mm04, string mm514,
            string mm15, string mf04, string mf514, string mf15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @mm04, @mm514, @mm15, @mf04, @mf514, @mf15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);
           
                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";
                mm04 = "-999";
                mm514 = "-999";
                mm15 = "-999";
                mf04 = "-999";
                mf514 = "-999";
                mf15 = "-999";

                //m04 = "-998";
                //m514 = "-998";
                //m15 = "-998";
                //f04 = "-998";
                //f514 = "-998";
                //f15 = "-998";
                //mm04 = "-998";
                //mm514 = "-998";
                //mm15 = "-998";
                //mf04 = "-998";
                //mf514 = "-998";
                //mf15 = "-998";


                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("mm04", mm04);
                toExecute.Parameters.AddWithValue("mm514", mm514);
                toExecute.Parameters.AddWithValue("mm15", mm15);
                toExecute.Parameters.AddWithValue("mf04", mf04);
                toExecute.Parameters.AddWithValue("mf514", mf514);
                toExecute.Parameters.AddWithValue("mf15", mf15);
                toExecute.Parameters.AddWithValue("format", format);
        
                _helper.Execute(toExecute);
            }
            else
            {
                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly";
                }


                // 1). The first Entry with the Disease name total
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @mm04, @mm514, @mm15, @mf04, @mf514, @mf15,@format)";


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04 + "TOT"; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514 + "TOT"; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15 + "TOT"; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04 + "TOT"; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514 + "TOT"; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15 + "TOT"; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();
                string mm04Tot = "MM" + mm04 + "TOT"; mm04Tot = (aggregateDataHash[mm04Tot] == null) ? "-999" : aggregateDataHash[mm04Tot].ToString();
                string mm514Tot = "MM" + mm514 + "TOT"; mm514Tot = (aggregateDataHash[mm514Tot] == null) ? "-999" : aggregateDataHash[mm514Tot].ToString();
                string mm15Tot = "MM" + mm15 + "TOT"; mm15Tot = (aggregateDataHash[mm15Tot] == null) ? "-999" : aggregateDataHash[mm15Tot].ToString();
                string mf04Tot = "MM" + mf04 + "TOT"; mf04Tot = (aggregateDataHash[mf04Tot] == null) ? "-999" : aggregateDataHash[mf04Tot].ToString();
                string mf514Tot = "MM" + mf514 + "TOT"; mf514Tot = (aggregateDataHash[mf514Tot] == null) ? "-999" : aggregateDataHash[mf514Tot].ToString();
                string mf15Tot = "MM" + mf15 + "TOT"; mf15Tot = (aggregateDataHash[mf15Tot] == null) ? "-999" : aggregateDataHash[mf15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("mm04", mm04Tot);
                toExecute.Parameters.AddWithValue("mm514", mm514Tot);
                toExecute.Parameters.AddWithValue("mm15", mm15Tot);
                toExecute.Parameters.AddWithValue("mf04", mf04Tot);
                toExecute.Parameters.AddWithValue("mf514", mf514Tot);
                toExecute.Parameters.AddWithValue("mf15", mf15Tot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 2). The Second Entry with the Public Facilities Total
                sno = "";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "  Public Facilities – total";
                disease = languageHash["publicFacilitiesTotal"].ToString();

                string m04PubTot = m04 + "PUBTOT"; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "-999" : aggregateDataHash[m04PubTot].ToString();
                string m514PubTot = m514 + "PUBTOT"; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "-999" : aggregateDataHash[m514PubTot].ToString();
                string m15PubTot = m15 + "PUBTOT"; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "-999" : aggregateDataHash[m15PubTot].ToString();
                string f04PubTot = f04 + "PUBTOT"; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "-999" : aggregateDataHash[f04PubTot].ToString();
                string f514PubTot = f514 + "PUBTOT"; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "-999" : aggregateDataHash[f514PubTot].ToString();
                string f15PubTot = f15 + "PUBTOT"; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "-999" : aggregateDataHash[f15PubTot].ToString();
                string mm04PubTot = "MM" + mm04 + "PUBTOT"; mm04PubTot = (aggregateDataHash[mm04PubTot] == null) ? "-999" : aggregateDataHash[mm04PubTot].ToString();
                string mm514PubTot = "MM" + mm514 + "PUBTOT"; mm514PubTot = (aggregateDataHash[mm514PubTot] == null) ? "-999" : aggregateDataHash[mm514PubTot].ToString();
                string mm15PubTot = "MM" + mm15 + "PUBTOT"; mm15PubTot = (aggregateDataHash[mm15PubTot] == null) ? "-999" : aggregateDataHash[mm15PubTot].ToString();
                string mf04PubTot = "MM" + mf04 + "PUBTOT"; mf04PubTot = (aggregateDataHash[mf04PubTot] == null) ? "-999" : aggregateDataHash[mf04PubTot].ToString();
                string mf514PubTot = "MM" + mf514 + "PUBTOT"; mf514PubTot = (aggregateDataHash[mf514PubTot] == null) ? "-999" : aggregateDataHash[mf514PubTot].ToString();
                string mf15PubTot = "MM" + mf15 + "PUBTOT"; mf15PubTot = (aggregateDataHash[mf15PubTot] == null) ? "-999" : aggregateDataHash[mf15PubTot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubTot);
                toExecute.Parameters.AddWithValue("m514", m514PubTot);
                toExecute.Parameters.AddWithValue("m15", m15PubTot);
                toExecute.Parameters.AddWithValue("f04", f04PubTot);
                toExecute.Parameters.AddWithValue("f514", f514PubTot);
                toExecute.Parameters.AddWithValue("f15", f15PubTot);
                toExecute.Parameters.AddWithValue("mm04", mm04PubTot);
                toExecute.Parameters.AddWithValue("mm514", mm514PubTot);
                toExecute.Parameters.AddWithValue("mm15", mm15PubTot);
                toExecute.Parameters.AddWithValue("mf04", mf04PubTot);
                toExecute.Parameters.AddWithValue("mf514", mf514PubTot);
                toExecute.Parameters.AddWithValue("mf15", mf15PubTot);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);
               
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Health Centers";
                disease = languageHash["healthCenters"].ToString();

                string m04PubHc = m04 + "PUBHC"; m04PubHc = (aggregateDataHash[m04PubHc] == null) ? "-999" : aggregateDataHash[m04PubHc].ToString();
                string m514PubHc = m514 + "PUBHC"; m514PubHc = (aggregateDataHash[m514PubHc] == null) ? "-999" : aggregateDataHash[m514PubHc].ToString();
                string m15PubHc = m15 + "PUBHC"; m15PubHc = (aggregateDataHash[m15PubHc] == null) ? "-999" : aggregateDataHash[m15PubHc].ToString();
                string f04PubHc = f04 + "PUBHC"; f04PubHc = (aggregateDataHash[f04PubHc] == null) ? "-999" : aggregateDataHash[f04PubHc].ToString();
                string f514PubHc = f514 + "PUBHC"; f514PubHc = (aggregateDataHash[f514PubHc] == null) ? "-999" : aggregateDataHash[f514PubHc].ToString();
                string f15PubHc = f15 + "PUBHC"; f15PubHc = (aggregateDataHash[f15PubHc] == null) ? "-999" : aggregateDataHash[f15PubHc].ToString();
                string mm04PubHc = "MM" + mm04 + "PUBHC"; mm04PubHc = (aggregateDataHash[mm04PubHc] == null) ? "-999" : aggregateDataHash[mm04PubHc].ToString();
                string mm514PubHc = "MM" + mm514 + "PUBHC"; mm514PubHc = (aggregateDataHash[mm514PubHc] == null) ? "-999" : aggregateDataHash[mm514PubHc].ToString();
                string mm15PubHc = "MM" + mm15 + "PUBHC"; mm15PubHc = (aggregateDataHash[mm15PubHc] == null) ? "-999" : aggregateDataHash[mm15PubHc].ToString();
                string mf04PubHc = "MM" + mf04 + "PUBHC"; mf04PubHc = (aggregateDataHash[mf04PubHc] == null) ? "-999" : aggregateDataHash[mf04PubHc].ToString();
                string mf514PubHc = "MM" + mf514 + "PUBHC"; mf514PubHc = (aggregateDataHash[mf514PubHc] == null) ? "-999" : aggregateDataHash[mf514PubHc].ToString();
                string mf15PubHc = "MM" + mf15 + "PUBHC"; mf15PubHc = (aggregateDataHash[mf15PubHc] == null) ? "-999" : aggregateDataHash[mf15PubHc].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubHc);
                toExecute.Parameters.AddWithValue("m514", m514PubHc);
                toExecute.Parameters.AddWithValue("m15", m15PubHc);
                toExecute.Parameters.AddWithValue("f04", f04PubHc);
                toExecute.Parameters.AddWithValue("f514", f514PubHc);
                toExecute.Parameters.AddWithValue("f15", f15PubHc);
                toExecute.Parameters.AddWithValue("mm04", mm04PubHc);
                toExecute.Parameters.AddWithValue("mm514", mm514PubHc);
                toExecute.Parameters.AddWithValue("mm15", mm15PubHc);
                toExecute.Parameters.AddWithValue("mf04", mf04PubHc);
                toExecute.Parameters.AddWithValue("mf514", mf514PubHc);
                toExecute.Parameters.AddWithValue("mf15", mf15PubHc);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);


                // 5). The fifth Entry with Public Hospitals
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Hospitals";
                disease = languageHash["hospitals"].ToString();

                string m04PubHos = m04 + "PUBHOS"; m04PubHos = (aggregateDataHash[m04PubHos] == null) ? "-999" : aggregateDataHash[m04PubHos].ToString();
                string m514PubHos = m514 + "PUBHOS"; m514PubHos = (aggregateDataHash[m514PubHos] == null) ? "-999" : aggregateDataHash[m514PubHos].ToString();
                string m15PubHos = m15 + "PUBHOS"; m15PubHos = (aggregateDataHash[m15PubHos] == null) ? "-999" : aggregateDataHash[m15PubHos].ToString();
                string f04PubHos = f04 + "PUBHOS"; f04PubHos = (aggregateDataHash[f04PubHos] == null) ? "-999" : aggregateDataHash[f04PubHos].ToString();
                string f514PubHos = f514 + "PUBHOS"; f514PubHos = (aggregateDataHash[f514PubHos] == null) ? "-999" : aggregateDataHash[f514PubHos].ToString();
                string f15PubHos = f15 + "PUBHOS"; f15PubHos = (aggregateDataHash[f15PubHos] == null) ? "-999" : aggregateDataHash[f15PubHos].ToString();
                string mm04PubHos = "MM" + mm04 + "PUBHOS"; mm04PubHos = (aggregateDataHash[mm04PubHos] == null) ? "-999" : aggregateDataHash[mm04PubHos].ToString();
                string mm514PubHos = "MM" + mm514 + "PUBHOS"; mm514PubHos = (aggregateDataHash[mm514PubHos] == null) ? "-999" : aggregateDataHash[mm514PubHos].ToString();
                string mm15PubHos = "MM" + mm15 + "PUBHOS"; mm15PubHos = (aggregateDataHash[mm15PubHos] == null) ? "-999" : aggregateDataHash[mm15PubHos].ToString();
                string mf04PubHos = "MM" + mf04 + "PUBHOS"; mf04PubHos = (aggregateDataHash[mf04PubHos] == null) ? "-999" : aggregateDataHash[mf04PubHos].ToString();
                string mf514PubHos = "MM" + mf514 + "PUBHOS"; mf514PubHos = (aggregateDataHash[mf514PubHos] == null) ? "-999" : aggregateDataHash[mf514PubHos].ToString();
                string mf15PubHos = "MM" + mf15 + "PUBHOS"; mf15PubHos = (aggregateDataHash[mf15PubHos] == null) ? "-999" : aggregateDataHash[mf15PubHos].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubHos);
                toExecute.Parameters.AddWithValue("m514", m514PubHos);
                toExecute.Parameters.AddWithValue("m15", m15PubHos);
                toExecute.Parameters.AddWithValue("f04", f04PubHos);
                toExecute.Parameters.AddWithValue("f514", f514PubHos);
                toExecute.Parameters.AddWithValue("f15", f15PubHos);
                toExecute.Parameters.AddWithValue("mm04", mm04PubHos);
                toExecute.Parameters.AddWithValue("mm514", mm514PubHos);
                toExecute.Parameters.AddWithValue("mm15", mm15PubHos);
                toExecute.Parameters.AddWithValue("mf04", mf04PubHos);
                toExecute.Parameters.AddWithValue("mf514", mf514PubHos);
                toExecute.Parameters.AddWithValue("mf15", mf15PubHos);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);


                // 6). The sixth Entry with Private not for profit Facilities – total
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "  " + "Private not for profit Facilities – total";
                disease = languageHash["privateNotForProfitFacilitiesTotal"].ToString();

                string m04PriNoProTot = m04 + "PRINOPROTOT"; m04PriNoProTot = (aggregateDataHash[m04PriNoProTot] == null) ? "-999" : aggregateDataHash[m04PriNoProTot].ToString();
                string m514PriNoProTot = m514 + "PRINOPROTOT"; m514PriNoProTot = (aggregateDataHash[m514PriNoProTot] == null) ? "-999" : aggregateDataHash[m514PriNoProTot].ToString();
                string m15PriNoProTot = m15 + "PRINOPROTOT"; m15PriNoProTot = (aggregateDataHash[m15PriNoProTot] == null) ? "-999" : aggregateDataHash[m15PriNoProTot].ToString();
                string f04PriNoProTot = f04 + "PRINOPROTOT"; f04PriNoProTot = (aggregateDataHash[f04PriNoProTot] == null) ? "-999" : aggregateDataHash[f04PriNoProTot].ToString();
                string f514PriNoProTot = f514 + "PRINOPROTOT"; f514PriNoProTot = (aggregateDataHash[f514PriNoProTot] == null) ? "-999" : aggregateDataHash[f514PriNoProTot].ToString();
                string f15PriNoProTot = f15 + "PRINOPROTOT"; f15PriNoProTot = (aggregateDataHash[f15PriNoProTot] == null) ? "-999" : aggregateDataHash[f15PriNoProTot].ToString();
                string mm04PriNoProTot = "MM" + mm04 + "PRINOPROTOT"; mm04PriNoProTot = (aggregateDataHash[mm04PriNoProTot] == null) ? "-999" : aggregateDataHash[mm04PriNoProTot].ToString();
                string mm514PriNoProTot = "MM" + mm514 + "PRINOPROTOT"; mm514PriNoProTot = (aggregateDataHash[mm514PriNoProTot] == null) ? "-999" : aggregateDataHash[mm514PriNoProTot].ToString();
                string mm15PriNoProTot = "MM" + mm15 + "PRINOPROTOT"; mm15PriNoProTot = (aggregateDataHash[mm15PriNoProTot] == null) ? "-999" : aggregateDataHash[mm15PriNoProTot].ToString();
                string mf04PriNoProTot = "MM" + mf04 + "PRINOPROTOT"; mf04PriNoProTot = (aggregateDataHash[mf04PriNoProTot] == null) ? "-999" : aggregateDataHash[mf04PriNoProTot].ToString();
                string mf514PriNoProTot = "MM" + mf514 + "PRINOPROTOT"; mf514PriNoProTot = (aggregateDataHash[mf514PriNoProTot] == null) ? "-999" : aggregateDataHash[mf514PriNoProTot].ToString();
                string mf15PriNoProTot = "MM" + mf15 + "PRINOPROTOT"; mf15PriNoProTot = (aggregateDataHash[mf15PriNoProTot] == null) ? "-999" : aggregateDataHash[mf15PriNoProTot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriNoProTot);
                toExecute.Parameters.AddWithValue("m514", m514PriNoProTot);
                toExecute.Parameters.AddWithValue("m15", m15PriNoProTot);
                toExecute.Parameters.AddWithValue("f04", f04PriNoProTot);
                toExecute.Parameters.AddWithValue("f514", f514PriNoProTot);
                toExecute.Parameters.AddWithValue("f15", f15PriNoProTot);
                toExecute.Parameters.AddWithValue("mm04", mm04PriNoProTot);
                toExecute.Parameters.AddWithValue("mm514", mm514PriNoProTot);
                toExecute.Parameters.AddWithValue("mm15", mm15PriNoProTot);
                toExecute.Parameters.AddWithValue("mf04", mf04PriNoProTot);
                toExecute.Parameters.AddWithValue("mf514", mf514PriNoProTot);
                toExecute.Parameters.AddWithValue("mf15", mf15PriNoProTot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 7). The seventh Entry with Private not for profit Facilities – clinics
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Clinics";
                disease = languageHash["clinics"].ToString();

                string m04PriNoProCli = m04 + "PRINOPROCLI"; m04PriNoProCli = (aggregateDataHash[m04PriNoProCli] == null) ? "-999" : aggregateDataHash[m04PriNoProCli].ToString();
                string m514PriNoProCli = m514 + "PRINOPROCLI"; m514PriNoProCli = (aggregateDataHash[m514PriNoProCli] == null) ? "-999" : aggregateDataHash[m514PriNoProCli].ToString();
                string m15PriNoProCli = m15 + "PRINOPROCLI"; m15PriNoProCli = (aggregateDataHash[m15PriNoProCli] == null) ? "-999" : aggregateDataHash[m15PriNoProCli].ToString();
                string f04PriNoProCli = f04 + "PRINOPROCLI"; f04PriNoProCli = (aggregateDataHash[f04PriNoProCli] == null) ? "-999" : aggregateDataHash[f04PriNoProCli].ToString();
                string f514PriNoProCli = f514 + "PRINOPROCLI"; f514PriNoProCli = (aggregateDataHash[f514PriNoProCli] == null) ? "-999" : aggregateDataHash[f514PriNoProCli].ToString();
                string f15PriNoProCli = f15 + "PRINOPROCLI"; f15PriNoProCli = (aggregateDataHash[f15PriNoProCli] == null) ? "-999" : aggregateDataHash[f15PriNoProCli].ToString();
                string mm04PriNoProCli = "MM" + mm04 + "PRINOPROCLI"; mm04PriNoProCli = (aggregateDataHash[mm04PriNoProCli] == null) ? "-999" : aggregateDataHash[mm04PriNoProCli].ToString();
                string mm514PriNoProCli = "MM" + mm514 + "PRINOPROCLI"; mm514PriNoProCli = (aggregateDataHash[mm514PriNoProCli] == null) ? "-999" : aggregateDataHash[mm514PriNoProCli].ToString();
                string mm15PriNoProCli = "MM" + mm15 + "PRINOPROCLI"; mm15PriNoProCli = (aggregateDataHash[mm15PriNoProCli] == null) ? "-999" : aggregateDataHash[mm15PriNoProCli].ToString();
                string mf04PriNoProCli = "MM" + mf04 + "PRINOPROCLI"; mf04PriNoProCli = (aggregateDataHash[mf04PriNoProCli] == null) ? "-999" : aggregateDataHash[mf04PriNoProCli].ToString();
                string mf514PriNoProCli = "MM" + mf514 + "PRINOPROCLI"; mf514PriNoProCli = (aggregateDataHash[mf514PriNoProCli] == null) ? "-999" : aggregateDataHash[mf514PriNoProCli].ToString();
                string mf15PriNoProCli = "MM" + mf15 + "PRINOPROCLI"; mf15PriNoProCli = (aggregateDataHash[mf15PriNoProCli] == null) ? "-999" : aggregateDataHash[mf15PriNoProCli].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriNoProCli);
                toExecute.Parameters.AddWithValue("m514", m514PriNoProCli);
                toExecute.Parameters.AddWithValue("m15", m15PriNoProCli);
                toExecute.Parameters.AddWithValue("f04", f04PriNoProCli);
                toExecute.Parameters.AddWithValue("f514", f514PriNoProCli);
                toExecute.Parameters.AddWithValue("f15", f15PriNoProCli);
                toExecute.Parameters.AddWithValue("mm04", mm04PriNoProCli);
                toExecute.Parameters.AddWithValue("mm514", mm514PriNoProCli);
                toExecute.Parameters.AddWithValue("mm15", mm15PriNoProCli);
                toExecute.Parameters.AddWithValue("mf04", mf04PriNoProCli);
                toExecute.Parameters.AddWithValue("mf514", mf514PriNoProCli);
                toExecute.Parameters.AddWithValue("mf15", mf15PriNoProCli);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 8). The Eight Entry with Private not for profit Facilities – Hospitals
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Hospitals";
                disease = languageHash["hospitals"].ToString();

                string m04PriNoProHos = m04 + "PRINOPROHOS"; m04PriNoProHos = (aggregateDataHash[m04PriNoProHos] == null) ? "-999" : aggregateDataHash[m04PriNoProHos].ToString();
                string m514PriNoProHos = m514 + "PRINOPROHOS"; m514PriNoProHos = (aggregateDataHash[m514PriNoProHos] == null) ? "-999" : aggregateDataHash[m514PriNoProHos].ToString();
                string m15PriNoProHos = m15 + "PRINOPROHOS"; m15PriNoProHos = (aggregateDataHash[m15PriNoProHos] == null) ? "-999" : aggregateDataHash[m15PriNoProHos].ToString();
                string f04PriNoProHos = f04 + "PRINOPROHOS"; f04PriNoProHos = (aggregateDataHash[f04PriNoProHos] == null) ? "-999" : aggregateDataHash[f04PriNoProHos].ToString();
                string f514PriNoProHos = f514 + "PRINOPROHOS"; f514PriNoProHos = (aggregateDataHash[f514PriNoProHos] == null) ? "-999" : aggregateDataHash[f514PriNoProHos].ToString();
                string f15PriNoProHos = f15 + "PRINOPROHOS"; f15PriNoProHos = (aggregateDataHash[f15PriNoProHos] == null) ? "-999" : aggregateDataHash[f15PriNoProHos].ToString();
                string mm04PriNoProHos = "MM" + mm04 + "PRINOPROHOS"; mm04PriNoProHos = (aggregateDataHash[mm04PriNoProHos] == null) ? "-999" : aggregateDataHash[mm04PriNoProHos].ToString();
                string mm514PriNoProHos = "MM" + mm514 + "PRINOPROHOS"; mm514PriNoProHos = (aggregateDataHash[mm514PriNoProHos] == null) ? "-999" : aggregateDataHash[mm514PriNoProHos].ToString();
                string mm15PriNoProHos = "MM" + mm15 + "PRINOPROHOS"; mm15PriNoProHos = (aggregateDataHash[mm15PriNoProHos] == null) ? "-999" : aggregateDataHash[mm15PriNoProHos].ToString();
                string mf04PriNoProHos = "MM" + mf04 + "PRINOPROHOS"; mf04PriNoProHos = (aggregateDataHash[mf04PriNoProHos] == null) ? "-999" : aggregateDataHash[mf04PriNoProHos].ToString();
                string mf514PriNoProHos = "MM" + mf514 + "PRINOPROHOS"; mf514PriNoProHos = (aggregateDataHash[mf514PriNoProHos] == null) ? "-999" : aggregateDataHash[mf514PriNoProHos].ToString();
                string mf15PriNoProHos = "MM" + mf15 + "PRINOPROHOS"; mf15PriNoProHos = (aggregateDataHash[mf15PriNoProHos] == null) ? "-999" : aggregateDataHash[mf15PriNoProHos].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriNoProHos);
                toExecute.Parameters.AddWithValue("m514", m514PriNoProHos);
                toExecute.Parameters.AddWithValue("m15", m15PriNoProHos);
                toExecute.Parameters.AddWithValue("f04", f04PriNoProHos);
                toExecute.Parameters.AddWithValue("f514", f514PriNoProHos);
                toExecute.Parameters.AddWithValue("f15", f15PriNoProHos);
                toExecute.Parameters.AddWithValue("mm04", mm04PriNoProHos);
                toExecute.Parameters.AddWithValue("mm514", mm514PriNoProHos);
                toExecute.Parameters.AddWithValue("mm15", mm15PriNoProHos);
                toExecute.Parameters.AddWithValue("mf04", mf04PriNoProHos);
                toExecute.Parameters.AddWithValue("mf514", mf514PriNoProHos);
                toExecute.Parameters.AddWithValue("mf15", mf15PriNoProHos);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 9). The Eight Entry with Private for profit Facilities – total 
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "  " + "Private for profit Facilities – total ";
                disease = languageHash["privateForProfitFacilitiesTotal"].ToString();

                string m04PriProTot = m04 + "PRIPROTOT"; m04PriProTot = (aggregateDataHash[m04PriProTot] == null) ? "-999" : aggregateDataHash[m04PriProTot].ToString();
                string m514PriProTot = m514 + "PRIPROTOT"; m514PriProTot = (aggregateDataHash[m514PriProTot] == null) ? "-999" : aggregateDataHash[m514PriProTot].ToString();
                string m15PriProTot = m15 + "PRIPROTOT"; m15PriProTot = (aggregateDataHash[m15PriProTot] == null) ? "-999" : aggregateDataHash[m15PriProTot].ToString();
                string f04PriProTot = f04 + "PRIPROTOT"; f04PriProTot = (aggregateDataHash[f04PriProTot] == null) ? "-999" : aggregateDataHash[f04PriProTot].ToString();
                string f514PriProTot = f514 + "PRIPROTOT"; f514PriProTot = (aggregateDataHash[f514PriProTot] == null) ? "-999" : aggregateDataHash[f514PriProTot].ToString();
                string f15PriProTot = f15 + "PRIPROTOT"; f15PriProTot = (aggregateDataHash[f15PriProTot] == null) ? "-999" : aggregateDataHash[f15PriProTot].ToString();
                string mm04PriProTot = "MM" + mm04 + "PRIPROTOT"; mm04PriProTot = (aggregateDataHash[mm04PriProTot] == null) ? "-999" : aggregateDataHash[mm04PriProTot].ToString();
                string mm514PriProTot = "MM" + mm514 + "PRIPROTOT"; mm514PriProTot = (aggregateDataHash[mm514PriProTot] == null) ? "-999" : aggregateDataHash[mm514PriProTot].ToString();
                string mm15PriProTot = "MM" + mm15 + "PRIPROTOT"; mm15PriProTot = (aggregateDataHash[mm15PriProTot] == null) ? "-999" : aggregateDataHash[mm15PriProTot].ToString();
                string mf04PriProTot = "MM" + mf04 + "PRIPROTOT"; mf04PriProTot = (aggregateDataHash[mf04PriProTot] == null) ? "-999" : aggregateDataHash[mf04PriProTot].ToString();
                string mf514PriProTot = "MM" + mf514 + "PRIPROTOT"; mf514PriProTot = (aggregateDataHash[mf514PriProTot] == null) ? "-999" : aggregateDataHash[mf514PriProTot].ToString();
                string mf15PriProTot = "MM" + mf15 + "PRIPROTOT"; mf15PriProTot = (aggregateDataHash[mf15PriProTot] == null) ? "-999" : aggregateDataHash[mf15PriProTot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriProTot);
                toExecute.Parameters.AddWithValue("m514", m514PriProTot);
                toExecute.Parameters.AddWithValue("m15", m15PriProTot);
                toExecute.Parameters.AddWithValue("f04", f04PriProTot);
                toExecute.Parameters.AddWithValue("f514", f514PriProTot);
                toExecute.Parameters.AddWithValue("f15", f15PriProTot);
                toExecute.Parameters.AddWithValue("mm04", mm04PriProTot);
                toExecute.Parameters.AddWithValue("mm514", mm514PriProTot);
                toExecute.Parameters.AddWithValue("mm15", mm15PriProTot);
                toExecute.Parameters.AddWithValue("mf04", mf04PriProTot);
                toExecute.Parameters.AddWithValue("mf514", mf514PriProTot);
                toExecute.Parameters.AddWithValue("mf15", mf15PriProTot);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 10). The Eight Entry with Private for profit Facilities – Clinics
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Clinics";
                disease = languageHash["clinics"].ToString();

                string m04PriProCli = m04 + "PRIPROCLI"; m04PriProCli = (aggregateDataHash[m04PriProCli] == null) ? "-999" : aggregateDataHash[m04PriProCli].ToString();
                string m514PriProCli = m514 + "PRIPROCLI"; m514PriProCli = (aggregateDataHash[m514PriProCli] == null) ? "-999" : aggregateDataHash[m514PriProCli].ToString();
                string m15PriProCli = m15 + "PRIPROCLI"; m15PriProCli = (aggregateDataHash[m15PriProCli] == null) ? "-999" : aggregateDataHash[m15PriProCli].ToString();
                string f04PriProCli = f04 + "PRIPROCLI"; f04PriProCli = (aggregateDataHash[f04PriProCli] == null) ? "-999" : aggregateDataHash[f04PriProCli].ToString();
                string f514PriProCli = f514 + "PRIPROCLI"; f514PriProCli = (aggregateDataHash[f514PriProCli] == null) ? "-999" : aggregateDataHash[f514PriProCli].ToString();
                string f15PriProCli = f15 + "PRIPROCLI"; f15PriProCli = (aggregateDataHash[f15PriProCli] == null) ? "-999" : aggregateDataHash[f15PriProCli].ToString();
                string mm04PriProCli = "MM" + mm04 + "PRIPROCLI"; mm04PriProCli = (aggregateDataHash[mm04PriProCli] == null) ? "-999" : aggregateDataHash[mm04PriProCli].ToString();
                string mm514PriProCli = "MM" + mm514 + "PRIPROCLI"; mm514PriProCli = (aggregateDataHash[mm514PriProCli] == null) ? "-999" : aggregateDataHash[mm514PriProCli].ToString();
                string mm15PriProCli = "MM" + mm15 + "PRIPROCLI"; mm15PriProCli = (aggregateDataHash[mm15PriProCli] == null) ? "-999" : aggregateDataHash[mm15PriProCli].ToString();
                string mf04PriProCli = "MM" + mf04 + "PRIPROCLI"; mf04PriProCli = (aggregateDataHash[mf04PriProCli] == null) ? "-999" : aggregateDataHash[mf04PriProCli].ToString();
                string mf514PriProCli = "MM" + mf514 + "PRIPROCLI"; mf514PriProCli = (aggregateDataHash[mf514PriProCli] == null) ? "-999" : aggregateDataHash[mf514PriProCli].ToString();
                string mf15PriProCli = "MM" + mf15 + "PRIPROCLI"; mf15PriProCli = (aggregateDataHash[mf15PriProCli] == null) ? "-999" : aggregateDataHash[mf15PriProCli].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriProCli);
                toExecute.Parameters.AddWithValue("m514", m514PriProCli);
                toExecute.Parameters.AddWithValue("m15", m15PriProCli);
                toExecute.Parameters.AddWithValue("f04", f04PriProCli);
                toExecute.Parameters.AddWithValue("f514", f514PriProCli);
                toExecute.Parameters.AddWithValue("f15", f15PriProCli);
                toExecute.Parameters.AddWithValue("mm04", mm04PriProCli);
                toExecute.Parameters.AddWithValue("mm514", mm514PriProCli);
                toExecute.Parameters.AddWithValue("mm15", mm15PriProCli);
                toExecute.Parameters.AddWithValue("mf04", mf04PriProCli);
                toExecute.Parameters.AddWithValue("mf514", mf514PriProCli);
                toExecute.Parameters.AddWithValue("mf15", mf15PriProCli);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 11). The Eight Entry with Private for profit Facilities – Hospitals
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Hospitals";
                disease = languageHash["hospitals"].ToString();

                string m04PriProHos = m04 + "PRIPROHOS"; m04PriProHos = (aggregateDataHash[m04PriProHos] == null) ? "-999" : aggregateDataHash[m04PriProHos].ToString();
                string m514PriProHos = m514 + "PRIPROHOS"; m514PriProHos = (aggregateDataHash[m514PriProHos] == null) ? "-999" : aggregateDataHash[m514PriProHos].ToString();
                string m15PriProHos = m15 + "PRIPROHOS"; m15PriProHos = (aggregateDataHash[m15PriProHos] == null) ? "-999" : aggregateDataHash[m15PriProHos].ToString();
                string f04PriProHos = f04 + "PRIPROHOS"; f04PriProHos = (aggregateDataHash[f04PriProHos] == null) ? "-999" : aggregateDataHash[f04PriProHos].ToString();
                string f514PriProHos = f514 + "PRIPROHOS"; f514PriProHos = (aggregateDataHash[f514PriProHos] == null) ? "-999" : aggregateDataHash[f514PriProHos].ToString();
                string f15PriProHos = f15 + "PRIPROHOS"; f15PriProHos = (aggregateDataHash[f15PriProHos] == null) ? "-999" : aggregateDataHash[f15PriProHos].ToString();
                string mm04PriProHos = "MM" + mm04 + "PRIPROHOS"; mm04PriProHos = (aggregateDataHash[mm04PriProHos] == null) ? "-999" : aggregateDataHash[mm04PriProHos].ToString();
                string mm514PriProHos = "MM" + mm514 + "PRIPROHOS"; mm514PriProHos = (aggregateDataHash[mm514PriProHos] == null) ? "-999" : aggregateDataHash[mm514PriProHos].ToString();
                string mm15PriProHos = "MM" + mm15 + "PRIPROHOS"; mm15PriProHos = (aggregateDataHash[mm15PriProHos] == null) ? "-999" : aggregateDataHash[mm15PriProHos].ToString();
                string mf04PriProHos = "MM" + mf04 + "PRIPROHOS"; mf04PriProHos = (aggregateDataHash[mf04PriProHos] == null) ? "-999" : aggregateDataHash[mf04PriProHos].ToString();
                string mf514PriProHos = "MM" + mf514 + "PRIPROHOS"; mf514PriProHos = (aggregateDataHash[mf514PriProHos] == null) ? "-999" : aggregateDataHash[mf514PriProHos].ToString();
                string mf15PriProHos = "MM" + mf15 + "PRIPROHOS"; mf15PriProHos = (aggregateDataHash[mf15PriProHos] == null) ? "-999" : aggregateDataHash[mf15PriProHos].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PriProHos);
                toExecute.Parameters.AddWithValue("m514", m514PriProHos);
                toExecute.Parameters.AddWithValue("m15", m15PriProHos);
                toExecute.Parameters.AddWithValue("f04", f04PriProHos);
                toExecute.Parameters.AddWithValue("f514", f514PriProHos);
                toExecute.Parameters.AddWithValue("f15", f15PriProHos);
                toExecute.Parameters.AddWithValue("mm04", mm04PriProHos);
                toExecute.Parameters.AddWithValue("mm514", mm514PriProHos);
                toExecute.Parameters.AddWithValue("mm15", mm15PriProHos);
                toExecute.Parameters.AddWithValue("mf04", mf04PriProHos);
                toExecute.Parameters.AddWithValue("mf514", mf514PriProHos);
                toExecute.Parameters.AddWithValue("mf15", mf15PriProHos);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);
            }
        }

        // Aggregate HC OPD Disease Report (Main one for higher Admin Levels)
        private void InsertOPDHCAggregateData(string sno, string disease, string m04, string m514,
            string m15, string f04, string f514, string f15, string format)
        {

            SqlCommand toExecute;

            if (m04 == "")
            {
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15,@format)";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("disease", disease);    

                m04 = "-999";
                m514 = "-999";
                m15 = "-999";
                f04 = "-999";
                f514 = "-999";
                f15 = "-999";              

                toExecute.Parameters.AddWithValue("m04", m04);
                toExecute.Parameters.AddWithValue("m514", m514);
                toExecute.Parameters.AddWithValue("m15", m15);
                toExecute.Parameters.AddWithValue("f04", f04);
                toExecute.Parameters.AddWithValue("f514", f514);
                toExecute.Parameters.AddWithValue("f15", f15);
                toExecute.Parameters.AddWithValue("format", format);
             
                _helper.Execute(toExecute);
            }
            else
            {
                string replacement = "";
                if (_reportObj.StartQuarter == 0) // This is a Montly report
                {
                    replacement = "";
                }
                else
                {
                    replacement = "q`ly";
                }

                // 1). The first Entry with the Disease name total
                string cmdText = "insert into " + reportTableName + " values (@sno, @disease, @m04, @m514, @m15, @f04, @f514, @f15, @format)";


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                disease = disease.Replace("q`ly total", replacement);
                //disease = disease + "   " + "total";

                string m04Tot = m04 + "TOT"; m04Tot = (aggregateDataHash[m04Tot] == null) ? "-999" : aggregateDataHash[m04Tot].ToString();
                string m514Tot = m514 + "TOT"; m514Tot = (aggregateDataHash[m514Tot] == null) ? "-999" : aggregateDataHash[m514Tot].ToString();
                string m15Tot = m15 + "TOT"; m15Tot = (aggregateDataHash[m15Tot] == null) ? "-999" : aggregateDataHash[m15Tot].ToString();
                string f04Tot = f04 + "TOT"; f04Tot = (aggregateDataHash[f04Tot] == null) ? "-999" : aggregateDataHash[f04Tot].ToString();
                string f514Tot = f514 + "TOT"; f514Tot = (aggregateDataHash[f514Tot] == null) ? "-999" : aggregateDataHash[f514Tot].ToString();
                string f15Tot = f15 + "TOT"; f15Tot = (aggregateDataHash[f15Tot] == null) ? "-999" : aggregateDataHash[f15Tot].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04Tot);
                toExecute.Parameters.AddWithValue("m514", m514Tot);
                toExecute.Parameters.AddWithValue("m15", m15Tot);
                toExecute.Parameters.AddWithValue("f04", f04Tot);
                toExecute.Parameters.AddWithValue("f514", f514Tot);
                toExecute.Parameters.AddWithValue("f15", f15Tot);
                toExecute.Parameters.AddWithValue("format", format);

                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);

                // 3). The Third Entry with the Health Total  

                if (!healthPost_NoDisplay.Contains(m04))
                {
                    toExecute = new SqlCommand(cmdText);
                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    sno = "";
                    toExecute.Parameters.AddWithValue("sno", sno);

                    //disease = "          " + "Health Posts";
                    disease = languageHash["healthPosts"].ToString();

                    //(CheckNull(row["VCT_Refered_For_Tb"])) ? false : Boolean.Parse(row["VCT_Refered_For_Tb"].ToString())

                    string m04PubHp = m04 + "PUBHP"; m04PubHp = (aggregateDataHash[m04PubHp] == null) ? "-999" : aggregateDataHash[m04PubHp].ToString();
                    string m514PubHp = m514 + "PUBHP"; m514PubHp = (aggregateDataHash[m514PubHp] == null) ? "-999" : aggregateDataHash[m514PubHp].ToString();
                    string m15PubHp = m15 + "PUBHP"; m15PubHp = (aggregateDataHash[m15PubHp] == null) ? "-999" : aggregateDataHash[m15PubHp].ToString();
                    string f04PubHp = f04 + "PUBHP"; f04PubHp = (aggregateDataHash[f04PubHp] == null) ? "-999" : aggregateDataHash[f04PubHp].ToString();
                    string f514PubHp = f514 + "PUBHP"; f514PubHp = (aggregateDataHash[f514PubHp] == null) ? "-999" : aggregateDataHash[f514PubHp].ToString();
                    string f15PubHp = f15 + "PUBHP"; f15PubHp = (aggregateDataHash[f15PubHp] == null) ? "-999" : aggregateDataHash[f15PubHp].ToString();


                    toExecute.Parameters.AddWithValue("disease", disease);
                    toExecute.Parameters.AddWithValue("m04", m04PubHp);
                    toExecute.Parameters.AddWithValue("m514", m514PubHp);
                    toExecute.Parameters.AddWithValue("m15", m15PubHp);
                    toExecute.Parameters.AddWithValue("f04", f04PubHp);
                    toExecute.Parameters.AddWithValue("f514", f514PubHp);
                    toExecute.Parameters.AddWithValue("f15", f15PubHp);
                    toExecute.Parameters.AddWithValue("format", format);


                    foreach (SqlParameter Parameter in toExecute.Parameters)
                    {
                        if (Parameter.Value == null)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                    }
                    _helper.Execute(toExecute);
                }

                // 4). The Fourth Entry Health Centers
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                sno = "";
                toExecute.Parameters.AddWithValue("sno", sno);

                //disease = "          " + "Health Centers";
                disease = languageHash["healthCenters"].ToString();

                string m04PubHc = m04 + "PUBHC"; m04PubHc = (aggregateDataHash[m04PubHc] == null) ? "-999" : aggregateDataHash[m04PubHc].ToString();
                string m514PubHc = m514 + "PUBHC"; m514PubHc = (aggregateDataHash[m514PubHc] == null) ? "-999" : aggregateDataHash[m514PubHc].ToString();
                string m15PubHc = m15 + "PUBHC"; m15PubHc = (aggregateDataHash[m15PubHc] == null) ? "-999" : aggregateDataHash[m15PubHc].ToString();
                string f04PubHc = f04 + "PUBHC"; f04PubHc = (aggregateDataHash[f04PubHc] == null) ? "-999" : aggregateDataHash[f04PubHc].ToString();
                string f514PubHc = f514 + "PUBHC"; f514PubHc = (aggregateDataHash[f514PubHc] == null) ? "-999" : aggregateDataHash[f514PubHc].ToString();
                string f15PubHc = f15 + "PUBHC"; f15PubHc = (aggregateDataHash[f15PubHc] == null) ? "-999" : aggregateDataHash[f15PubHc].ToString();

                toExecute.Parameters.AddWithValue("disease", disease);
                toExecute.Parameters.AddWithValue("m04", m04PubHc);
                toExecute.Parameters.AddWithValue("m514", m514PubHc);
                toExecute.Parameters.AddWithValue("m15", m15PubHc);
                toExecute.Parameters.AddWithValue("f04", f04PubHc);
                toExecute.Parameters.AddWithValue("f514", f514PubHc);
                toExecute.Parameters.AddWithValue("f15", f15PubHc);
                toExecute.Parameters.AddWithValue("format", format);


                foreach (SqlParameter Parameter in toExecute.Parameters)
                {
                    if (Parameter.Value == null)
                    {
                        Parameter.Value = DBNull.Value;
                    }
                }
                _helper.Execute(toExecute);
            }
        }


        public List<string> GetIncludedLocations()
        {

            if (!level1Cache)
            {              
                IncludedList = calulateNumFacilitiesNew2();
            }

            return IncludedList;
        }

        public void UpdateHashTable()
        {

            //string cmdText = "exec   " + storedProcName + " @DataEleClassMorbidity, @DataEleClassMortality, " +
            //" @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;
            string cmdText = "exec   " + storedProcNameNew + " @DataEleClassMorbidity, @DataEleClassMortality, " +
            " @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;
            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            string Id = "";
            //toExecute = new SqlCommand();

            //toExecute.CommandType = CommandType.StoredProcedure;
            //toExecute.Connection = _helper.Connection;
            //toExecute.CommandText = "proc_Eth_AATest_EthioHIMS_QuarterRHBOOPDDiseaseReport";

            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility)
                || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthCenter)
                || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {
                Id = _reportObj.LocationHMISCode;
            }
            else if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.woreda)
            {
                Id = _reportObj.WoredaID.ToString();
            }
            else if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.zone)
            {
                Id = _reportObj.ZoneID.ToString();
            }
            else if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.region)
            {
                Id = _reportObj.RegionID.ToString();
            }

            int type = (int)_reportObj.RepGlobalType;
            int repKind = (int)_reportObj.RepKind;


            toExecute.Parameters.AddWithValue("DataEleClassMorbidity", dataEleClassMorbidity);
            toExecute.Parameters.AddWithValue("DataEleClassMortality", dataEleClassMortality);
            toExecute.Parameters.AddWithValue("Year", _reportObj.Year);
            toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
            toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
            toExecute.Parameters.AddWithValue("Type", type);
            toExecute.Parameters.AddWithValue("RepKind", repKind);
            toExecute.Parameters.AddWithValue("Id", Id);

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            aggregateDataHash.Clear();
            aggregateList.Clear();

            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility)
                          || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthCenter)
                          || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {

                foreach (DataRow row in dt.Rows)
                {
                    string LabelID = row["LabelID"].ToString();
                    //decimal HmisValue = Convert.ToDecimal(row["HmisValue"].ToString());
                    string HmisValue = row["HmisValue"].ToString();
                    aggregateDataHash.Add(LabelID, HmisValue);
                    aggregateList.Add(LabelID);
                }
            }
            else
            {

                int dataEleClass = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string labelId1 = "";
                    string labelId2 = "";
                    string labelId3 = "";

                    if (repKind == 1)
                    {
                        dataEleClass = Convert.ToInt32(row["dataEleClass"]);

                        if (dataEleClass == 3)
                        {
                            labelId1 = "MM";
                            labelId2 = "MM";
                            labelId3 = "MM";
                        }
                        else
                        {
                            labelId1 = "";
                            labelId2 = "";
                            labelId3 = "";
                        }
                    }

                    bool wrongFacilityType = false;

                    int facilitType = Convert.ToInt32(row["facilitType"].ToString());
                    string LabelID = row["LabelID"].ToString();
                    //decimal HmisValue = Convert.ToDecimal(row["HmisValue"].ToString());
                    decimal value = Convert.ToDecimal(row["value"].ToString());
                    // TOT, PUBTOT, PUBHOS, PUBHC, PUBHP, PRINOPROTOT, PRINOPROCLI,
                    // PRINOPROHOS, PRIPROTOT, PRIPROCLI, PRIPROHOS   


                    if ((facilitType == 8) // Woreda 
                        || (facilitType == 9) // Zone
                        || (facilitType == 10) // Region
                        || (facilitType == 50) // Other Government Clinic
                        || (facilitType == 51) // Other Government Center
                        || (facilitType == 52) // Other Government Hospital
                        || (facilitType == 53) // Private for Profit Center
                        || (facilitType == 54) // Private not for Profit Center
                        || (facilitType == 11)) // Federal
                    {
                        // data should not exist with such facilityTypes
                        // General.Util.UI.MyMessageDialogSmall.Show("Dear user, because there is no reporting format for facility type list here, Other Government Clinic,Other Government Center,Other Government Hospital,Private for Profit,Center,Private not for Profit Center. Their data (IPD and OPD only) will not be displayed.");
                        wrongFacilityType = true;
                    }
                    else
                    {
                        wrongFacilityType = false;
                    }


                    if (facilitType == 1)
                    {
                        labelId1 = labelId1 + LabelID + "PUBHOS";
                        labelId2 = labelId2 + LabelID + "PUBTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }
                    else if (facilitType == 2)
                    {
                        labelId1 = labelId1 + LabelID + "PUBHC";
                        labelId2 = labelId2 + LabelID + "PUBTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }
                    else if (facilitType == 3)
                    {
                        labelId1 = labelId1 + LabelID + "PUBHP";
                        labelId2 = labelId2 + LabelID + "PUBTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }
                    else if (facilitType == 4)
                    {
                        labelId1 = labelId1 + LabelID + "PRINOPROCLI";
                        labelId2 = labelId2 + LabelID + "PRINOPROTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }
                    else if (facilitType == 5)
                    {
                        labelId1 = labelId1 + LabelID + "PRINOPROHOS";
                        labelId2 = labelId2 + LabelID + "PRINOPROTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }
                    else if (facilitType == 6)
                    {
                        labelId1 = labelId1 + LabelID + "PRIPROCLI";
                        labelId2 = labelId2 + LabelID + "PRIPROTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }
                    else if (facilitType == 7)
                    {
                        labelId1 = labelId1 + LabelID + "PRIPROHOS";
                        labelId2 = labelId2 + LabelID + "PRIPROTOT";
                        labelId3 = labelId3 + LabelID + "TOT";
                    }

                    if (wrongFacilityType == false)
                    {
                        string HmisValue = value.ToString();

                        aggregateDataHash.Add(labelId1, HmisValue);
                        if (aggregateDataHash[labelId2] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId2]);
                            newValue = newValue + value;
                            HmisValue = newValue.ToString();
                            aggregateDataHash[labelId2] = HmisValue;
                        }
                        else
                        {
                            aggregateDataHash.Add(labelId2, HmisValue);
                        }

                        if (aggregateDataHash[labelId3] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId3]);
                            newValue = newValue + value;
                            HmisValue = newValue.ToString();
                            aggregateDataHash[labelId3] = HmisValue;
                        }
                        else
                        {
                            aggregateDataHash.Add(labelId3, HmisValue);
                        }


                        aggregateList.Add(labelId1);

                        if (!aggregateList.Contains(labelId2))
                        {
                            aggregateList.Add(labelId2);
                        }

                        if (!aggregateList.Contains(labelId3))
                        {
                            aggregateList.Add(labelId3);
                        }

                    }

                }
            }

            startReportTableGeneration(false);
            UpdateReportingTable();

        }

        private List<string> calulateNumFacilitiesNew2()
        {
            string idQuery = "";
            string monthQuery = "";
            string facilityCalcFileName = "";
            string adminType = "";

            int repKind = (int)_reportObj.RepKind;

            string dataEleClassQuery = " ";

            if (repKind == 0) // OPD
            {
                dataEleClassQuery = " and dataEleClass = 8 ";
            }
            else if (repKind == 1) // IPD
            {
                dataEleClassQuery = " and (dataEleClass = 2 or dataEleClass = 3) ";
            }

            if ((_startMonth == 11) && ((_endMonth == 11) || (_endMonth == 12)))
            {
                decimal prevYear = _seleYear - 1;
                monthQuery = "	where  ((Month  between " + _startMonth + " and " + _endMonth + " ) and  Year = " + prevYear + " ) ";
            }
            else if ((_startMonth == 11) && ((_endMonth != 11) && (_endMonth != 12)))
            {
                decimal prevYear = _seleYear - 1;
                monthQuery = "	where  (((Month  between 11 and 12 ) and  Year = " + prevYear + " ) " +
                             "  or (Month between 1 and " + _endMonth + " and Year = " + _seleYear + "))";
            }
            else if ((_startMonth == 12) && (_endMonth == 12))
            {
                decimal prevYear = _seleYear - 1;
                monthQuery = "	where  ((Month  = 12 ) and  Year = " + prevYear + " ) ";
            }
            else if ((_startMonth == 12) && (_endMonth != 12))
            {
                decimal prevYear = _seleYear - 1;
                monthQuery = "	where  (((Month  = 12 ) and  Year = " + prevYear + " ) " +
                             "  or (Month between 1 and " + _endMonth + " and Year = " + _seleYear + "))";
            }
            else // Other Quarters
            {
                monthQuery = "	where  Month  between " + _startMonth + " and  " + _endMonth + " and Year = " + _seleYear;
            }

            int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);

            if (aggregationLevel == 1)
            {
                idQuery = "";
                adminType = "";
            }
            else if (aggregationLevel == 2)
            {
                idQuery = " and regionId =  " + _reportObj.LocationHMISCode;
                adminType = "";
            }
            else if (aggregationLevel == 5)
            {
                idQuery = " and zoneId =  " + _reportObj.LocationHMISCode;
                adminType = "";
            }
            else if (aggregationLevel == 3)
            {
                idQuery = " and woredaId  =  " + _reportObj.LocationHMISCode;
                adminType = "";
            }

            facilityCalcFileName = "Cache" + adminType + "NumFacilities";

            string cmdText = "select * from " + facilityCalcFileName + monthQuery + idQuery + dataEleClassQuery;

            SqlCommand toExecute = new SqlCommand(cmdText);

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            List<string> numFacilitiesList = new List<string>();

            Hashtable numFacilitiesHash = new Hashtable();

            foreach (DataRow row in dt.Rows)
            {
                //int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                string locations = row["locations"].ToString();
                string[] locSplit = locations.Split(',');

                for (int i = 0; i < locSplit.Length; i++)
                {
                    string locId = locSplit[i];

                    if (numFacilitiesHash[locId] == null)
                    {
                        numFacilitiesHash[locId] = 1;
                        //if (!numFacilitiesList.Contains(locId))
                        //{
                        numFacilitiesList.Add(locId);
                        //}
                    }
                }
            }

            return numFacilitiesList;
        }

        public void UpdateReportingTable()
        {

            string cmdText = "";

            if ((_reportObj.RepKind == reportObject.ReportKind.OPD_Disease) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease_Facility_Monthly) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_Disease_Facility_Quarterly) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_HC_Aggregate_Monthly) ||
                (_reportObj.RepKind == reportObject.ReportKind.OPD_HC_Aggregate_Quarterly))
            {
                cmdText = "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column M04   Decimal(18,0) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column M514  Decimal(18,0) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column M15   Decimal(18,0) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column F04   Decimal(18,0) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column F514  Decimal(18,0) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column F15   Decimal(18,0) ";
                //"ALTER TABLE " + reportTableName + "  " +
                //"Alter Column Format int    ";

                SqlCommand toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);
            }
            else if ((_reportObj.RepKind == reportObject.ReportKind.IPD_Disease) ||
                (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease_Facility_Monthly) ||
                (_reportObj.RepKind == reportObject.ReportKind.IPD_Disease_Facility_Quarterly))
            {
                cmdText = "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column M04   Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column M514  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column M15   Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column F04   Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column F514  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column F15   Decimal(18,0) " +

                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MM04  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MM514  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MM15  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MF04  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MF514  Decimal(18,0) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MF15  Decimal(18,0) ";
                //"ALTER TABLE " + reportTableName + "  " +
                // "Alter Column Format int    ";


                SqlCommand toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);
            }

            if (!eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable.Contains("EthioHMIS_HMIS_Value_Temp"))
            {
                _helper.CloseConnection();
            }
        }     

        private int getAggregationLevel(string locationID)
        {
            // Given the location ID returns the Facility Name
            string cmdText = "select AggregationLevelID from facility where hmiscode = @locationID";
            int aggregationLevel = 0;

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            toExecute.Parameters.AddWithValue("locationID", locationID);

            DataSet ds = _helper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
            {
                aggregationLevel = Convert.ToInt16(ds.Tables[0].Rows[0]["AggregationLevelID"]);
            }

            return aggregationLevel;
        }

    }
}
