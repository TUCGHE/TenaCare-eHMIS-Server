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
using SqlManagement.Database;
//using CCP.Framework.Xml;
//using CCP.Framework.Sql.Tables;
//using CCP.FrameworkB;
using System.IO;
using eHMIS.HMIS.ReportHelper;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text.RegularExpressions;

namespace eHMIS.HMIS.ReportAggregation.NewReporting
{

    public class NewServiceReportAggregation
    {

        DBConnHelper _helper = new DBConnHelper();

        //Dictionary<string, string> aggregateDataHash = new Dictionary<string, string>();

        Hashtable ethMonth = new Hashtable();
        Hashtable aggregateDataHash = new Hashtable();
        ArrayList aggregateList = new ArrayList();
        ArrayList verticalSumSequnceNo = new ArrayList();
        Hashtable verticalSumHash = new Hashtable();
        Hashtable seqLabelIDHash = new Hashtable();
        Hashtable higherAdmin = new Hashtable();

        ArrayList newLabelIds = new ArrayList();
        Hashtable facilityTypes = new Hashtable();

        //string viewLabeIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
        //string reportTableName = "EthioHIMS_QuarterRHBOOPDDiseaseReport"; // Should be dynamically set
        //string storedProcName = "proc_Eth_HMIS_ServiceReport";
        string mainStoredProcName = "#proc_ETH_HMIS_Service_Report_reader";
        string tempStoredProcName = "#proc_Eth_HMIS_ServiceReport_Temp";
        string storedProcNoMonth = "proc_Eth_HMIS_ServiceReport_NoMonth_Aggregate";

        string includeListProcName = "proc_Eth_HMIS_Service_IncludedList";
        string viewLabeIdTableName = "";
        string verticalSumIdTableName = "";
        string hmisValueTable = "";

        string reportTableName = "";

        List<string> IncludedList = new List<string>();

        reportObject _reportObj;
        int _startMonth;
        int _endMonth;
        //int _seleQuarter;
        decimal _seleYear;

        string periodType = "";

        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation

        string tempTableName = "";
        string tempTableName1 = "";
        string tempTableName2 = "";
        string tempTableName3 = "";
        string tempTableName4 = "";             

        string _hostName = "";
        Hashtable languageHash = new Hashtable();

        private static string exportFilePath = "";

        string hmisCodesSelected = "";
        string sha1Hash = "";
        bool level1Cache = false;
        bool level2Cache = false;
        bool isExportSaved = false;

        public NewServiceReportAggregation(reportObject reportObj)
        {

                tempTableName = "#EthTemp";
                tempTableName1 = "#EthTemp1";
                tempTableName2 = "#EthTemp2";
                tempTableName3 = "#EthTemp3";
                tempTableName4 = "#EthTemp4";            

            //reportObj.ReportType
            _helper.CloseConnection();
            _helper.ManualCloseConnection = true;
            viewLabeIdTableName = reportObj.ViewLabelIdTableName;
            reportTableName = reportObj.ReportTableName;
            GenerateReport.globalReportTableName = reportTableName;
            GenerateReport.globalStoredProcName = mainStoredProcName;

            this._reportObj = reportObj;
            setStartingMonth();

            setPeriodType();

            level1Cache = false;
            level2Cache = false;
            

            hmisValueTable = eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable;
           

            int aggrLevel = getAggregationLevel(_reportObj.LocationHMISCode);

            if (aggrLevel == 4)
            {
                level2Cache = false;
            }

            IncludedList.Clear();
            aggregateDataHash.Clear();

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
            ethMonth.Add(13, "Pagume");

            if (!level1Cache)
            {
                if (!level2Cache)
                {
                    newLabelIds.Add("_PUBHP"); newLabelIds.Add("_PUBHC"); newLabelIds.Add("_PUBHOS");
                    newLabelIds.Add("_PUBTOT"); newLabelIds.Add("_PRINOPROCLI"); newLabelIds.Add("_PRINOPROHOS");
                    newLabelIds.Add("_PRINOPROTOT"); newLabelIds.Add("_PRIPROCLI"); newLabelIds.Add("_PRIPROHOS");
                    newLabelIds.Add("_PRINOPROCEN"); newLabelIds.Add("_PRIPROCEN");
                    newLabelIds.Add("_GOVCLI"); newLabelIds.Add("_GOVHOS"); newLabelIds.Add("_GOVCEN"); newLabelIds.Add("_GOVTOT");
                    newLabelIds.Add("_PRIPROTOT"); newLabelIds.Add("_AFTOT"); newLabelIds.Add("_WHOTOT");
                    newLabelIds.Add("_ZHDTOT"); newLabelIds.Add("_RHBTOT"); newLabelIds.Add("_FMOHTOT");
                    newLabelIds.Add("_TOT");

                    facilityTypes.Add("_PUBHP", " and FacilitType = 3 ");
                    facilityTypes.Add("_PUBHC", " and FacilitType = 2 ");
                    facilityTypes.Add("_PUBHOS", " and FacilitType = 1 ");
                    facilityTypes.Add("_PUBTOT", " and (FacilitType = 1 or FacilitType = 2 or FacilitType = 3) ");
                    facilityTypes.Add("_PRINOPROCLI", " and FacilitType = 4 ");
                    facilityTypes.Add("_PRINOPROHOS", " and FacilitType = 5 ");
                    facilityTypes.Add("_PRIPROCLI", " and FacilitType = 6 ");
                    facilityTypes.Add("_PRIPROHOS", " and FacilitType = 7 ");

                    facilityTypes.Add("_WHOTOT", " and FaciliTType = 8  ");
                    facilityTypes.Add("_ZHDTOT", " and FaciliTType = 9  ");
                    facilityTypes.Add("_RHBTOT", " and FaciliTType = 10  ");
                    facilityTypes.Add("_FMOHTOT", " and FaciliTType = 11  ");


                    facilityTypes.Add("_TOT", "");

                    //misganat@tiethio.org, changes for the new service data elements , July 2, 2014
                    if (HMISMainPage.UseNewServiceDataElement2014)
                    {
                        facilityTypes.Add("_GOVCLI", " and FacilitType = 50  ");
                        facilityTypes.Add("_GOVCEN", " and FacilitType = 51  ");
                        facilityTypes.Add("_GOVHOS", " and FacilitType = 52  ");
                        facilityTypes.Add("_GOVTOT", " and (FacilitType = 50 or FacilitType = 51 or FacilitType = 52) ");
                        facilityTypes.Add("_PRINOPROTOT", " and (FacilitType = 4 or FacilitType = 5 or FacilitType = 54)  ");
                        facilityTypes.Add("_PRIPROTOT", " and (FacilitType = 6 or FacilitType = 7 or FacilitType = 53)  ");
                        facilityTypes.Add("_PRINOPROCEN", " and FacilitType = 54 ");
                        facilityTypes.Add("_PRIPROCEN", " and FacilitType = 53 ");
                        facilityTypes.Add("_AFTOT", " and (FacilitType >= 50 or FacilitType <= 7)  ");
                    }
                    else
                    {
                        facilityTypes.Add("_AFTOT", " and (FacilitType >= 1 and FacilitType <= 7)  ");
                        facilityTypes.Add("_PRIPROTOT", " and (FacilitType = 6 or FacilitType = 7)  ");
                        facilityTypes.Add("_PRINOPROTOT", " and (FacilitType = 4 or FacilitType = 5)  ");
                    }
                }         

                viewLabeIdTableName = _reportObj.ViewLabelIdTableName;
                verticalSumIdTableName = "EthioHIMS_VerticalSum";

                string cmdText = " select SNO, LabelID, sequenceNo, VerticalSumID from  " + viewLabeIdTableName +
                                 " where verticalsumID is not null";

                SqlCommand toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                verticalSumSequnceNo.Clear();
                verticalSumHash.Clear();
                seqLabelIDHash.Clear();

                foreach (DataRow row in dt2.Rows)
                {
                    string sno = row["SNO"].ToString();
                    string sequenceno = row["sequenceNo"].ToString();
                    string verticalSumID = row["verticalSumID"].ToString();

                    verticalSumSequnceNo.Add(sequenceno);
                    verticalSumHash.Add(sequenceno, verticalSumID);
                }

                cmdText = " select LabelID, sequenceNo from  " + viewLabeIdTableName;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                dt2 = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt2.Rows)
                {
                    string LabelID = row["LabelID"].ToString();
                    string sequenceno = row["sequenceNo"].ToString();

                    seqLabelIDHash.Add(sequenceno, LabelID);
                }

                DynamicQueryConstruct();

            }
        }

        private void setLanguage()
        {
            eHMISWebApi.Controllers.LanguageController langCtrl = new eHMISWebApi.Controllers.LanguageController();

            DataTable dtLang = langCtrl.Get("", "dataEntry");

            foreach (DataRow row in dtLang.Rows)
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


        public void deleteTablesNew()
        {
            //string cmdText = " truncate table  " + reportTableName; // + " delete from AA_TestTemp ";
            //SqlCommand toExecute;
            //toExecute = new SqlCommand(cmdText);

            //_helper.Execute(toExecute);

            string storedProcExec = "";

            //string cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + reportTableName + "]') AND type in (N'U'))  " +
            //                  " DROP TABLE " + reportTableName;

            string cmdText =
                  " IF OBJECT_ID('tempdb.." + reportTableName + "') IS NOT NULL \n " +
                  " DROP TABLE " + reportTableName;


            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // If exists remove the Stored Procedure
                //cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + mainStoredProcName + "]') AND type in (N'P', N'PC')) " +
                //           " DROP PROCEDURE [dbo].[" + mainStoredProcName + "]\n " +

                //           "SET ANSI_NULLS ON  \n" +

                //           " SET QUOTED_IDENTIFIER ON \n ";


                cmdText =
                      " IF OBJECT_ID('tempdb.." + mainStoredProcName + "') IS NOT NULL \n " +
                      " DROP procedure " + mainStoredProcName;



                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // Create the Stored Procedure Dynamically

                if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                             " AS BEGIN " +
                             " SET NOCOUNT ON " +
                        //" SELECT EthioHIMS_QuarterSrvReport.SNO, EthioHIMS_QuarterSrvReport.GroupID, " +
                        //" Activity, Month1 as Amount FROM [dbo].[EthioHIMS_QuarterSrvReport] " +
                        //" inner join  " + viewLabeIdTableName + "  on  " +
                        //" EthioHIMS_QuarterSrvReport.GroupID =  " +
                        //  viewLabeIdTableName + ".GroupID " +
                        //  " END ";
                              " SELECT " + reportTableName + ".SNO, " + reportTableName + ".Activity, " +
                              " Month1 as Amount, " +
                              viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                              viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly " +
                              " FROM " + reportTableName +
                              " inner join  " + viewLabeIdTableName + "  on  " +
                              reportTableName + ".GroupID =  " +
                              viewLabeIdTableName + ".GroupID " +
                              " order by  " + viewLabeIdTableName + ".sequenceno" +
                             " END ";

                    storedProcExec = cmdText;
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    if (HMISMainPage.UseNewVersion == true)
                    {
                        cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                                  " AS BEGIN " +
                                  " SET NOCOUNT ON " +
                                  " SELECT " + reportTableName + ".SNO, " + reportTableName + ".Activity, " +
                                  reportTableName + ".Quarter, " +
                                  viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                                  viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly  " +
                                  " FROM [dbo].[EthioHIMS_QuarterSrvReport] " +
                                  " inner join  " + viewLabeIdTableName + "  on  " +
                                  reportTableName + ".GroupID =  " +
                                  viewLabeIdTableName + ".GroupID " +
                                  " order by  " + viewLabeIdTableName + ".sequenceno" +
                                  " END ";

                        storedProcExec = cmdText;
                    }
                    else
                    {
                        cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                                  " AS BEGIN " +
                                  " SET NOCOUNT ON " +
                                  " SELECT " + reportTableName + ".SNO, " + reportTableName + ".Activity, " +
                                  reportTableName + ".Quarter, " + reportTableName + ".Month1," +
                                  reportTableName + ".Month2," + reportTableName + ".Month3," +
                                  viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                                  viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly,  " +
                                  viewLabeIdTableName + ".PeriodType  " +
                                  " FROM " + reportTableName +
                                  " inner join  " + viewLabeIdTableName + "  on  " +
                                  reportTableName + ".GroupID =  " +
                                  viewLabeIdTableName + ".GroupID " +
                                  " order by  " + viewLabeIdTableName + ".sequenceno" +
                                  " END ";

                        storedProcExec = cmdText;
                    }

                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                              " AS BEGIN " +
                              " SET NOCOUNT ON " +
                              " SELECT " + reportTableName + ".SNO, " + reportTableName + ".Activity, " +
                              reportTableName + ".Month1, " +
                              viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                              viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly  " +
                              " FROM " + reportTableName +
                              " inner join  " + viewLabeIdTableName + "  on  " +
                              reportTableName + ".GroupID =  " +
                              viewLabeIdTableName + ".GroupID " +
                                " order by  " + viewLabeIdTableName + ".sequenceno" +
                              " END ";

                    storedProcExec = cmdText;
                }

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = " CREATE TABLE  " + reportTableName + "  ( " +
                          " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                          " [SNO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [Activity] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [GroupID] [int] NULL, " +
                          " [Month1] [decimal](18, 2) NULL, " +
                          " [Month2] [decimal](18, 2) NULL, " +
                          " [Month3] [decimal](18, 2) NULL, " +
                          " [Quarter] [decimal](18, 2) NULL) ";


                // SqlCommand toExecute;
                GenerateReport.globalCreateTable = cmdText;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Month1   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Month2  varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Month3   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Quarter   varchar(50) ";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }
            else //if ((_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service) || (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service))
            {
                // Drop the table if it exists, cmdText set at the beginning
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // If exists remove the Stored Procedure
                //cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + mainStoredProcName + "]') AND type in (N'P', N'PC')) " +
                //           " DROP PROCEDURE [dbo].[" + mainStoredProcName + "]\n " +

                //           " SET ANSI_NULLS ON  \n" +

                //           " SET QUOTED_IDENTIFIER ON \n ";

                cmdText =
                      " IF OBJECT_ID('tempdb.." + mainStoredProcName + "') IS NOT NULL \n " +
                      " DROP procedure " + mainStoredProcName;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);
                // Create the Stored Procedure Dynamically


                cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                   " AS BEGIN " +
                   " SET NOCOUNT ON " +
                    //" SELECT * FROM [dbo].[EthioHIMS_QuarterFMOHSrvReport] " +
                   " SELECT  " + reportTableName + ".[SNO],[Activity],[PFHealthPosts], " +
                    " [PFHealthCenters],[PFHospitals],[PFTotal],[GVClinics],[GVCenters],[GVHospitals], [GVTotal], " +
                    " [PPClinics], [PPCenters],[PPHospitals]  ,[PPTotal], " +
                    " [PNClinics],[PNCenters],[PNHospitals], [PNTotal],   " +
                    " [AFTotal],  [WHOTotal]  , " +
                    " [ZHDTotal], [RHBTotal], [MOHTotal], " +
                    viewLabeIdTableName + ".Readonly, " + viewLabeIdTableName + ".HP, " + viewLabeIdTableName +
                    ".HC, " + viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".WHO, " +
                    viewLabeIdTableName + ".ZHD, " + viewLabeIdTableName + ".RHB, " +
                    viewLabeIdTableName + ".FMOH, " + viewLabeIdTableName + ".Private,[AHITotal] " +
                   " from " + reportTableName +
                   " inner join  " + viewLabeIdTableName + "  on  " +
                   reportTableName + ".GroupID =  " +
                   viewLabeIdTableName + ".GroupID " +
                   " order by  " + viewLabeIdTableName + ".sequenceno" +
                   " END ";

                storedProcExec = cmdText;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);


                cmdText = " CREATE TABLE  " + reportTableName + "  ( " +
                          " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                          " [SNO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [Activity] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [GroupID] [int] NULL, " +
                          " [PFHealthPosts] [decimal](18, 2) NULL, " +
                          " [PFHealthCenters] [decimal](18, 2) NULL, " +
                          " [PFHospitals] [decimal](18, 2) NULL, " +
                          " [PFTotal] [decimal](18, 2) NULL, " +
                          " [GVClinics] [decimal](18,2) NULL," +
                          " [GVCenters] [decimal](18,2) NULL, " +
                          " [GVHospitals] [decimal](18,2) NULL, " +

                            " [GVTotal] [decimal](18,2) NULL," +
                            " [PPClinics] [decimal](18, 2) NULL, " +
                            " [PPCenters] [decimal] (18,2) NULL, " +
                            " [PPHospitals] [decimal](18, 2) NULL, " +
                            " [PPTotal] [decimal](18, 2) NULL, " +
                            " [PNClinics] [decimal](18, 2) NULL, " +
                            " [PNCenters] [decimal](18,2) NULL, " +
                            " [PNHospitals] [decimal](18, 2) NULL, " +
                            " [PNTotal] [decimal](18, 2) NULL, " +
                          " [AFTotal] [decimal](18, 2) NULL, " +
                          " [WHOTotal] [decimal](18, 2) NULL, " +
                          " [ZHDTotal] [decimal](18, 2) NULL, " +
                          " [RHBTotal] [decimal](18, 2) NULL, " +
                          " [MOHTotal] [decimal](18, 2) NULL, " +
                          " [AHITotal] [decimal](18, 2) NULL)"; //, " +
                //" CONSTRAINT [PK_EthioHIMS_QuarterFMOHSrvReport] PRIMARY KEY CLUSTERED " +
                //" ( " +
                //" [ID] ASC " +
                //" )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] " +
                //" ) ON [PRIMARY] ";

                GenerateReport.globalCreateTable = cmdText;

                // SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFHealthPosts   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFHealthCenters  varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFHospitals   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFTotal   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PNClinics  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PNCenters varchar(50)" +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PNHospitals   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "   " +
                              "Alter Column PNTotal  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PPClinics   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PPCenters varchar(50)" +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PPHospitals  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "   " +
                              "Alter Column PPTotal   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "   " +
                              "Alter Column GVClinics varchar(50)" +
                              "ALTER TABLE " + reportTableName + "   " +
                              "Alter Column GVCenters varchar(50)" +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column GVHospitals varchar(50)" +
                              "ALTER TABLE " + reportTableName + "   " +
                              "Alter Column AFTotal  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column WHOTotal   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column ZHDTotal   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column RHBTotal  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column MOHTotal   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column AHITotal   varchar(50) ";
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);



            }

            GenerateReport.globalCreateStoredProc = storedProcExec;
            // Reset Identity, to resolve performance issues
            //cmdText = "DBCC CHECKIDENT (" + reportTableName + ", reseed, 0)";

            //toExecute = new SqlCommand(cmdText);

            //_helper.Execute(toExecute);
        }


        public void deleteTables()
        {
            //string cmdText = " truncate table  " + reportTableName; // + " delete from AA_TestTemp ";
            //SqlCommand toExecute;
            //toExecute = new SqlCommand(cmdText);

            //_helper.Execute(toExecute);

            //string cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + reportTableName + "]') AND type in (N'U'))  " +
            //                  " DROP TABLE " + reportTableName;

            string cmdText =
                    " IF OBJECT_ID('tempdb.." + reportTableName + "') IS NOT NULL \n " +
                    " DROP table " + reportTableName;

            string storedProcExec = "";

            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // If exists remove the Stored Procedure
                //cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + mainStoredProcName + "]') AND type in (N'P', N'PC')) " +
                //           " DROP PROCEDURE [dbo].[" + mainStoredProcName + "]\n " +

                //           "SET ANSI_NULLS ON  \n" +

                //           " SET QUOTED_IDENTIFIER ON \n ";       

                cmdText =
                    " IF OBJECT_ID('tempdb.." + mainStoredProcName + "') IS NOT NULL \n " +
                    " DROP procedure " + mainStoredProcName;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // Create the Stored Procedure Dynamically

                if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                             " AS BEGIN " +
                             " SET NOCOUNT ON " +
                        //" SELECT EthioHIMS_QuarterSrvReport.SNO, EthioHIMS_QuarterSrvReport.GroupID, " +
                        //" Activity, Month1 as Amount FROM [dbo].[EthioHIMS_QuarterSrvReport] " +
                        //" inner join  " + viewLabeIdTableName + "  on  " +
                        //" EthioHIMS_QuarterSrvReport.GroupID =  " +
                        //  viewLabeIdTableName + ".GroupID " +
                        //  " END ";
                              " SELECT " + reportTableName + ".SNO,  " +
                              reportTableName + ".Activity, " +
                              " Month1 as Amount, " +
                              viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                              viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly " +
                              " FROM " + reportTableName +
                              " inner join  " + viewLabeIdTableName + "  on  " +
                              reportTableName + ".GroupID =  " +
                                viewLabeIdTableName + ".GroupID " +
                              " order by  " + viewLabeIdTableName + ".sequenceno" +
                             " END ";

                    storedProcExec = cmdText;
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    if (HMISMainPage.UseNewVersion == true)
                    {
                        cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                                  " AS BEGIN " +
                                  " SET NOCOUNT ON " +
                                  " SELECT " + reportTableName + ".SNO,  " +
                                  reportTableName + ".Activity, " +
                                  reportTableName + ".Quarter, " +
                                  viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                                  viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly  " +
                                  " FROM " + reportTableName +
                                  " inner join  " + viewLabeIdTableName + "  on  " +
                                 reportTableName + ".GroupID =  " +
                                    viewLabeIdTableName + ".GroupID " +
                                  " order by  " + viewLabeIdTableName + ".sequenceno" +
                                  " END ";

                        storedProcExec = cmdText;
                    }
                    else
                    {
                        cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                                  " AS BEGIN " +
                                  " SET NOCOUNT ON " +
                                  " SELECT " + reportTableName + ".SNO, " +
                                  reportTableName + ".Activity, " +
                                  reportTableName + ".Quarter, " +
                                  reportTableName + ".Month1," +
                                  reportTableName + ".Month2, " +
                                  reportTableName + ".Month3," +
                                  viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                                  viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly  " +
                                  " FROM " + reportTableName +
                                  " inner join  " + viewLabeIdTableName + "  on  " +
                                  reportTableName + ".GroupID =  " +
                                    viewLabeIdTableName + ".GroupID " +
                                  " order by  " + viewLabeIdTableName + ".sequenceno" +
                                  " END ";

                        storedProcExec = cmdText;
                    }

                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                              " AS BEGIN " +
                              " SET NOCOUNT ON " +
                              " SELECT " + reportTableName + ".SNO, " +
                              reportTableName + ".Activity, " +
                              reportTableName + ".Month1, " +
                              viewLabeIdTableName + ".HP, " + viewLabeIdTableName + ".HC, " +
                              viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".Readonly  " +
                              " FROM " + reportTableName +
                              " inner join  " + viewLabeIdTableName + "  on  " +
                              reportTableName + ".GroupID =  " +
                                viewLabeIdTableName + ".GroupID " +
                                " order by  " + viewLabeIdTableName + ".sequenceno" +
                              " END ";

                    storedProcExec = cmdText;
                }

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = " CREATE TABLE  " + reportTableName + "  ( " +
                          " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                          " [SNO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [Activity] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [GroupID] [int] NULL, " +
                          " [Month1] [decimal](18, 2) NULL, " +
                          " [Month2] [decimal](18, 2) NULL, " +
                          " [Month3] [decimal](18, 2) NULL, " +
                          " [Quarter] [decimal](18, 2) NULL)"; //, " +
                //" CONSTRAINT [PK_EthioHIMS_QuarterSrvReport] PRIMARY KEY CLUSTERED " +
                //" ( " +
                //" [ID] ASC " +
                //" )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] " +
                //" ) ON [PRIMARY] ";

                // SqlCommand toExecute;
                GenerateReport.globalCreateTable = cmdText;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Month1   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Month2  varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Month3   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column Quarter   varchar(50) ";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }
            else //if ((_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service) || (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service))
            {
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // If exists remove the Stored Procedure
                //cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + mainStoredProcName + "]') AND type in (N'P', N'PC')) " +
                //           " DROP PROCEDURE [dbo].[" + mainStoredProcName + "]\n " +

                //           "SET ANSI_NULLS ON  \n" +

                //           " SET QUOTED_IDENTIFIER ON \n ";       

                cmdText =
                    " IF OBJECT_ID('tempdb.." + mainStoredProcName + "') IS NOT NULL \n " +
                    " DROP procedure " + mainStoredProcName;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                // Create the Stored Procedure Dynamically
                cmdText = " CREATE PROCEDURE  [dbo].[" + mainStoredProcName + "] " +
                         " AS BEGIN " +
                         " SET NOCOUNT ON " +
                    //" SELECT * FROM [dbo].[EthioHIMS_QuarterFMOHSrvReport] " +
                         " SELECT  " + reportTableName + ".[SNO],[Activity],[PFHealthPosts], " +
                         " [PFHealthCenters],[PFHospitals],[PFTotal],[PNClinics],[PNHospitals],  " +
                         " [PNTotal],[PPClinics],[PPHospitals]  ,[PPTotal],[AFTotal],  [WHOTotal]  , " +
                         " [ZHDTotal], [RHBTotal], [MOHTotal], " +
                          viewLabeIdTableName + ".Readonly, " + viewLabeIdTableName + ".HP, " + viewLabeIdTableName +
                          ".HC, " + viewLabeIdTableName + ".Hospital, " + viewLabeIdTableName + ".WHO, " +
                          viewLabeIdTableName + ".ZHD, " + viewLabeIdTableName + ".RHB, " +
                          viewLabeIdTableName + ".FMOH, " + viewLabeIdTableName + ".Private,[AHITotal] " +
                         " from " + reportTableName +
                         " inner join  " + viewLabeIdTableName + "  on  " +
                          reportTableName + ".GroupID =  " +
                         viewLabeIdTableName + ".GroupID " +
                         " order by  " + viewLabeIdTableName + ".sequenceno" +
                         " END ";

                storedProcExec = cmdText;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = " CREATE TABLE  " + reportTableName + "  ( " +
                          " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                          " [SNO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [Activity] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                          " [GroupID] [int] NULL, " +
                          " [PFHealthPosts] [decimal](18, 2) NULL, " +
                          " [PFHealthCenters] [decimal](18, 2) NULL, " +
                          " [PFHospitals] [decimal](18, 2) NULL, " +
                          " [PFTotal] [decimal](18, 2) NULL, " +
                          " [PNClinics] [decimal](18, 2) NULL, " +
                          " [PNHospitals] [decimal](18, 2) NULL, " +
                          " [PNTotal] [decimal](18, 2) NULL, " +
                          " [PPClinics] [decimal](18, 2) NULL, " +
                          " [PPHospitals] [decimal](18, 2) NULL, " +
                          " [PPTotal] [decimal](18, 2) NULL, " +
                          " [AFTotal] [decimal](18, 2) NULL, " +
                          " [WHOTotal] [decimal](18, 2) NULL, " +
                          " [ZHDTotal] [decimal](18, 2) NULL, " +
                          " [RHBTotal] [decimal](18, 2) NULL, " +
                          " [MOHTotal] [decimal](18, 2) NULL, " +
                          " [AHITotal] [decimal](18, 2) NULL)"; //, " +
                //" CONSTRAINT [PK_EthioHIMS_QuarterFMOHSrvReport] PRIMARY KEY CLUSTERED " +
                //" ( " +
                //" [ID] ASC " +
                //" )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] " +
                //" ) ON [PRIMARY] ";

                // SqlCommand toExecute;

                GenerateReport.globalCreateTable = cmdText;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFHealthPosts   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFHealthCenters  varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFHospitals   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PFTotal   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PNClinics  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PNHospitals   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PNTotal  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PPClinics   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PPHospitals  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column PPTotal   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column AFTotal  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column WHOTotal   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column ZHDTotal   varchar(50) " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column RHBTotal  varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column MOHTotal   varchar(50)  " +
                              "ALTER TABLE " + reportTableName + "  " +
                              "Alter Column AHITotal   varchar(50) ";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }

            GenerateReport.globalCreateStoredProc = storedProcExec;

            // Reset Identity, to resolve performance issues
            //cmdText = "DBCC CHECKIDENT (" + reportTableName + ", reseed, 0)";

            //toExecute = new SqlCommand(cmdText);

            //_helper.Execute(toExecute);
        }

        // Dynamically construct the Needed Stored Procedures based on Features that
        // Need to be applied Group 1 = Summation/Aggregation Group 2 = Only Last Month Data reported, if
        // no last month data, Do not report anything Group 3 = Anding all the available data, one 0
        // will make everything 0, one non-reporting makes everything non-reporting
        // Group 4 = Averaging all the available data, currently this is done for LQAS (Data Quality Score)
        // In the table EthioHims_ServiceDataElements, we can readily get all the labelIDs and know
        // what group they below to, based on the, we will create a temporary table, to save 
        // the constricted data and remove at a later time in the UpdateReportingTable functions
        // So each group would have it's own temporary table

        private void DynamicQueryConstruct()
        {
            if (hmisValueTable == "")
            {
                hmisValueTable = "EthEhmis_HmisValue";
            }

            // If exists remove the Stored Procedure
            //string cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[#proc_Eth_HMIS_ServiceReport_Temp]') AND type in (N'P', N'PC')) " +
            //           " DROP PROCEDURE [dbo].[#proc_Eth_HMIS_ServiceReport_Temp]\n " +

            //           "SET ANSI_NULLS ON  \n" +

            //           " SET QUOTED_IDENTIFIER ON \n ";

            string cmdText =
                    " IF OBJECT_ID('tempdb.." + tempStoredProcName + "') IS NOT NULL \n " +
                    " DROP procedure " + tempStoredProcName;

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);


            string group1File = tempTableName1;
            string group2File = tempTableName2;
            string group3File = tempTableName3;
            string group4File = tempTableName4;

            string idQuery = "";
            string dataEleClassQuery = "";
            string facilityTypeQuery = "";
            string monthYearQueryGroup1 = "", monthYearQueryGroup2 = "";
            string innerJoinGroup1 = "", innerJoinGroup2 = "", innerJoinGroup3 = "", innerJoinGroup4 = "";

            string cmdText1 = "", cmdText2 = "", cmdText3 = "", cmdText4 = "";

            if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                dataEleClassQuery = "  DataEleClass = 7 ";
            }
            else
            {
                dataEleClassQuery = "  DataEleClass = 6 ";
            }

            if (HMISMainPage.UseNewVersion == true)
            {
                if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    dataEleClassQuery = "  DataEleClass = 17 ";
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    dataEleClassQuery = "  DataEleClass = 16 ";
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    dataEleClassQuery = "  DataEleClass = 13 ";
                }
            }

            int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);


            if (aggregationLevel == 1)
            {
                idQuery = "";
                higherAdmin.Add("_WHOTOT", true);
                higherAdmin.Add("_ZHDTOT", true);
                higherAdmin.Add("_RHBTOT", true);
                higherAdmin.Add("_FMOHTOT", true);
            }
            else if (aggregationLevel == 2)
            {
                idQuery = " and RegionID =  " + _reportObj.LocationHMISCode;
                higherAdmin.Add("_WHOTOT", true);
                higherAdmin.Add("_ZHDTOT", true);
                higherAdmin.Add("_RHBTOT", true);
            }
            else if (aggregationLevel == 3)
            {
                idQuery = " and WoredaID  =  " + _reportObj.LocationHMISCode;
                higherAdmin.Add("_WHOTOT", true);
            }
            else if (aggregationLevel == 4)
            {
                idQuery = " and LocationID =  '" + _reportObj.LocationHMISCode + "'";
            }
            else if (aggregationLevel == 5)
            {
                idQuery = " and ZoneID =  " + _reportObj.LocationHMISCode;
                higherAdmin.Add("_WHOTOT", true);
                higherAdmin.Add("_ZHDTOT", true);
            }

            if (HMISMainPage.UseNewVersion == true)
            {
                if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Year =  " + _seleYear;
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Quarter =  " + _reportObj.StartQuarter + "  and Year =  " + _seleYear;
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Month =  " + _startMonth + "  and Year =  " + _seleYear;
                }
            }
            else
            {
                if ((_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service) && (_startMonth == 11))
                {
                    decimal tempYear = _seleYear - 1;

                    monthYearQueryGroup1 = " where   (((Month between 11 and 12) and  " +
                     "  (Year = " + tempYear + ")) or ((Month between 1 and  " + _endMonth + ")  and Year =  " + _seleYear + "  )) ";
                    monthYearQueryGroup2 = " where  Month =  " + _endMonth + "  and Year =  " + _seleYear;

                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;
                    monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;
                    monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;

                    if (_startMonth == 11)
                    {
                        decimal tempYear = _seleYear - 1;
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth +
                                " and Year =  " + tempYear;
                            monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + tempYear;
                        }
                        else
                        {
                            monthYearQueryGroup1 = " where  (((Month between 11 and 12) and year = " + tempYear + ") OR " +
                                                   " ( Month >=1 and Month <= " + _endMonth + " and Year = " + _seleYear + " )) ";
                            monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        decimal tempYear = _seleYear - 1;
                        if (_endMonth == 12)
                        {
                            monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth +
                                " and Year =  " + tempYear;
                            monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + tempYear;
                        }
                        else
                        {
                            monthYearQueryGroup1 = " where  (((Month = 12) and year = " + tempYear + ") OR " +
                                                   " ( Month >=1 and Month <= " + _endMonth + " and Year = " + _seleYear + " )) ";
                            monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;
                        }
                    }
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Year =  " + _seleYear;
                }
            }

            string storedProc = getInitialStoredProc(hmisValueTable, monthYearQueryGroup1,
                                idQuery, dataEleClassQuery);

            // Facility
            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {

                IncludedList.Clear();
                IncludedList.Add(_reportObj.LocationHMISCode);
                if (HMISMainPage.UseNewServiceDataElement2014)
                {
                    calculateNumFacilitiesNew();
                }
                else
                {
                    calculateNumFacilities();
                }

                if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    // Facility Monthly
                    // Group 1 -> 
                    cmdText1 =
                       " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                       "	select DataEleClass, LabelID, sum(Value) from   " + group1File + "  \n  " +
                       monthYearQueryGroup1 + idQuery + 
                       "	group by DataEleClass,  LabelID  \n\n";

                    // Group 2 -> 
                    cmdText2 =
                       " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                       "	select DataEleClass, LabelID, sum(Value) from   " + group2File + "  \n  " +
                       monthYearQueryGroup2 + idQuery +
                        "	group by DataEleClass,  LabelID  \n\n";

                    // Group 3 -> Value
                    cmdText3 =
                       " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                       "	select DataEleClass, LabelID, sum(Value) from   " + group3File + "  \n  " +
                       monthYearQueryGroup1 + idQuery +
                        "	group by DataEleClass,  LabelID  \n\n";

                    // Group 4 -> Value
                    cmdText4 =
                       " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                       "	select DataEleClass, LabelID, sum(Value) from   " + group4File + "  \n  " +
                       monthYearQueryGroup1 + idQuery +
                        "	group by DataEleClass,  LabelID  \n\n";
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    // Facility Quarterly
                    // QuarterData

                    // Group 1 -> Summation
                    cmdText1 +=
                    " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                    "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                    "   sum(Value) as Value from  " + group1File + "  \n  " +
                    monthYearQueryGroup1 + idQuery +
                    "	group by DataEleClass,  LabelID  \n\n";

                    // Group 2 -> Last Value, End of Month
                    cmdText2 +=
                   " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                   "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                   "   sum(Value) as Value from  " + group2File + "  \n  " +
                   monthYearQueryGroup2 + idQuery +
                   "	group by DataEleClass,  LabelID  \n\n";

                    // Group 3 -> Average
                    cmdText3 +=
                   " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                   "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                   "   cast (sum(Value)/3 as int) as Value from  " + group3File + "  \n  " +
                   monthYearQueryGroup1 + idQuery +
                   "	group by DataEleClass,  LabelID  \n\n";

                    // Group 4 -> Average
                    cmdText4 +=
                   " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                   "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                   "   sum(Value)/3 as Value from  " + group4File + "  \n  " +
                   monthYearQueryGroup1 + idQuery +
                   "	group by DataEleClass,  LabelID  \n\n";

                    // Individual Monthly Data
                    if (_startMonth == 11) // Quarter 1
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int setMonth = 0;
                            decimal setYear = 0;
                            if (i == 0)
                            {
                                setMonth = 11;
                                setYear = _seleYear - 1;
                            }
                            else if (i == 1)
                            {
                                setMonth = 12;
                                setYear = _seleYear - 1;
                            }
                            else if (i == 2)
                            {
                                setMonth = 1;
                                setYear = _seleYear;
                            }

                            // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;

                            // Group 1 -> Summation  ethMonth[i]
                            cmdText1 +=
                            " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                            "	select DataEleClass, cast(LabelID as VarChar) + '_" + setMonth + "' as LabelID, \n  " +
                            "   Value from  " + group1File + "  \n  " +
                            "   where  Month  = " + setMonth + " and Year =  " + setYear + idQuery +
                            "	\n\n";

                            // Group 2 -> Last Value, End of Month
                            cmdText2 +=
                           " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                           "	select DataEleClass, cast(LabelID as VarChar) + '_" + setMonth + "' as LabelID, \n  " +
                           "    Value from  " + group2File + "  \n  " +
                           "    where  Month  = " + setMonth + " and Year =  " + setYear + idQuery +
                           "	 \n\n";

                            // Group 3 -> Average
                            cmdText3 +=
                           " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                           "	select DataEleClass, cast(LabelID as VarChar) + '_" + setMonth + "' as LabelID, \n  " +
                           "    Value from  " + group3File + "  \n  " +
                           "    where  Month  = " + setMonth + " and Year =  " + setYear + idQuery +
                           "	\n\n";

                            // Group 3 -> Average                          
                            cmdText4 +=
                          " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                          "	select DataEleClass, cast(LabelID as VarChar) + '_" + setMonth + "' as LabelID, \n  " +
                          " Value from  " + group4File + "  \n  " +
                          " where  Month  = " + setMonth + " and Year =  " + setYear + idQuery +
                          "	\n\n";
                        }
                    }
                    else
                    {
                        for (int i = _startMonth; i <= _endMonth; i++)
                        {
                            int month = i;
                            // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;

                            // Group 1 -> Summation  ethMonth[i]
                            cmdText1 +=
                            " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                            "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
                            "   Value from  " + group1File + "  \n  " +
                            "   where  Month  = " + i + " and Year =  " + _seleYear + idQuery +
                            "	\n\n";

                            // Group 2 -> Last Value, End of Month
                            cmdText2 +=
                           " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                           "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
                           "    Value from  " + group2File + "  \n  " +
                           "    where  Month  = " + i + " and Year =  " + _seleYear + idQuery +
                           "	 \n\n";

                            // Group 3 -> Average
                            cmdText3 +=
                           " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                           "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
                           "    Value from  " + group3File + "  \n  " +
                           "    where  Month  = " + i + " and Year =  " + _seleYear + idQuery +
                           "	\n\n";

                            // Group 3 -> Average
                            // cmdText4 +=
                            //" insert into #Ethtemp (DataEleClass, LabelID, HmisValue) \n  " +
                            //"	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
                            //"   avg(Value) as Value from  " + group4File + "  \n  " +
                            //" where  Month  = " + i + " and Year =  " + _seleYear + idQuery +
                            //"	group by DataEleClass,  LabelID  \n\n";
                            cmdText4 +=
                          " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                          "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
                          " Value from  " + group4File + "  \n  " +
                          " where  Month  = " + i + " and Year =  " + _seleYear + idQuery +
                          "	\n\n";
                        }
                    }
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    // Facility Quarterly
                    // QuarterData

                    // Group 1 -> Summation
                    cmdText1 +=
                    " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                    "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                    "   sum(Value) as Value from  " + group1File + "  \n  " +
                    monthYearQueryGroup1 + idQuery +
                    "	group by DataEleClass,  LabelID  \n\n";

                    // Group 2 -> Last Value, End of Month
                    cmdText2 +=
                   " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                   "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                   "   sum(Value) as Value from  " + group2File + "  \n  " +
                   monthYearQueryGroup2 + idQuery +
                   "	group by DataEleClass,  LabelID  \n\n";

                    // Group 3 -> Average
                    cmdText3 +=
                   " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                   "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                   "   cast (sum(Value)/3 as int) as Value from  " + group3File + "  \n  " +
                   monthYearQueryGroup1 + idQuery +
                   "	group by DataEleClass,  LabelID  \n\n";

                    // Group 4 -> Average
                    cmdText4 +=
                   " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                   "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
                   "   sum(Value)/3 as Value from  " + group4File + "  \n  " +
                   monthYearQueryGroup1 + idQuery +
                   "	group by DataEleClass,  LabelID  \n\n";
                }
            }
            else // Not a Facility
            {
                //cmdText1 +=
                //        " -- Here calculate the number of Locations Involved \n\n" +
                //        "    declare @locationCount int \n " +
                //        "    set @locationCount = (select count(distinct(locationId)) " +
                //        "    from " +  hmisValueTable + "  \n" +
                //             monthYearQueryGroup1 + idQuery + ") \n\n\n";

                //string includedLocations = " select distinct(locationID) from " + hmisValueTable +
                //                             monthYearQueryGroup1 + idQuery + " and " + dataEleClassQuery;
                string includedLocations = " select distinct(locationID),facilitType from " + hmisValueTable +
                                            monthYearQueryGroup1 + idQuery + " and " + dataEleClassQuery;
                //"  and labelId in  " +o
                //"  (select labelId from   " + viewLabeIdTableName + 
                //"  where  " + periodType + " )";                

                toExecute = new SqlCommand(includedLocations);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                // _helper.Execute(toExecute);                            

                DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                int numberOfFacilities = 0;
                IncludedList.Clear();
                Hashtable locationFacilityTypeId = new Hashtable();
                foreach (DataRow row in dt2.Rows)
                {
                    string LocationID = row["LocationID"].ToString();
                    int facilityTypeId = Convert.ToInt32(row["facilitType"]);

                    IncludedList.Add(LocationID);
                    if (locationFacilityTypeId[LocationID] == null)
                    {
                        locationFacilityTypeId.Add(LocationID, facilityTypeId);
                    }
                    numberOfFacilities++;
                }

                // Calculate Number of Facilities, LabelID for Number of Facilities = 189
                if (HMISMainPage.UseNewServiceDataElement2014)
                {
                    calculateNumFacilitiesNew(locationFacilityTypeId);
                }
                else
                {
                    calculateNumFacilities(locationFacilityTypeId);
                }

                foreach (string newlabelId in newLabelIds)
                {
                    Boolean showAdmin = true;
                    if ((newlabelId == "_WHOTOT") || (newlabelId == "_ZHDTOT") ||
                        (newlabelId == "_RHBTOT") || (newlabelId == "_FMOHTOT"))
                    {
                        //innerJoinGroup1 = " inner join facility on "  + group1File + ".LocationID = facility.HmisCode ";
                        //innerJoinGroup2 = " inner join facility on " + group2File + ".LocationID = facility.HmisCode ";
                        //innerJoinGroup3 = " inner join facility on " + group3File + ".LocationID = facility.HmisCode ";
                        //innerJoinGroup4 = " inner join facility on " + group4File + ".LocationID = facility.HmisCode ";

                        innerJoinGroup1 = ""; innerJoinGroup2 = ""; innerJoinGroup3 = ""; innerJoinGroup4 = "";

                        if (higherAdmin[newlabelId] == null)
                        {
                            showAdmin = false;
                        }
                        else
                        {
                            showAdmin = true;
                        }
                    }
                    else
                    {
                        innerJoinGroup1 = " ";
                        innerJoinGroup2 = " ";
                        innerJoinGroup3 = " ";
                        innerJoinGroup4 = " ";
                    }

                    if (showAdmin == true)
                    {
                        // Group 1 -> Summation
                        cmdText1 += "\n\n";
                        cmdText1 +=
                          " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                         "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                         "   sum(Value) as Value from  " + group1File + " " + innerJoinGroup1 +
                         monthYearQueryGroup1 +
                            idQuery + facilityTypes[newlabelId] +
                        "	group by DataEleClass,  LabelID  ";

                        // Group 2 -> Last Value End of Month
                        cmdText2 += "\n\n";
                        cmdText2 +=
                          " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                         "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                         "   sum(Value) as Value from " + group2File + " " + innerJoinGroup2 +
                         monthYearQueryGroup2 +
                            idQuery + facilityTypes[newlabelId] +
                        "	group by DataEleClass,  LabelID  ";

                        // Group 3 -> Average
                        cmdText3 += "\n\n";
                        cmdText3 +=
                         " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                         "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                            // "  cast(sum(Value)/@locationCount as int) as Value from " + group3File + " " + innerJoinGroup3 +
                            // This is for Tracer Drugs, for Higher Admin levels, you add the values
                            //"  cast(avg(Value) as int) as Value from " + group3File + " " + innerJoinGroup3 +
                        "  sum(value) as Value from " + group3File + " " + innerJoinGroup3 +
                        monthYearQueryGroup1 +
                            idQuery + facilityTypes[newlabelId] +
                        "	group by DataEleClass,  LabelID  ";

                        // Group 3 -> Average
                        cmdText4 += "\n\n";
                        cmdText4 +=
                          " insert into " + tempTableName + " (DataEleClass, LabelID, HmisValue) \n  " +
                         "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                            //"   sum(Value)/@locationCount as Value from " + group4File + " " + innerJoinGroup4 +
                         "   avg(Value) as Value from " + group4File + " " + innerJoinGroup4 +
                         monthYearQueryGroup1 +
                            idQuery + facilityTypes[newlabelId] +
                        "	group by DataEleClass,  LabelID  ";
                    }
                }
            }

            storedProc += " \n \n  " + cmdText1 + " \n \n  " + cmdText2 + " \n \n  " + cmdText3 + " \n \n  " + cmdText4;
            storedProc += " \n\nselect LabelId, HmisValue from " + tempTableName + "  where  " + dataEleClassQuery + "  \n";
            storedProc += " \nEND ";

            toExecute = new SqlCommand(storedProc);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            resetNumFacilities();

            // createServiceView();

        }

        private void createServiceView()
        {
            string dropView =

                    "IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_aaEthEhmisService]')) \n " +
                    "DROP VIEW [dbo].[v_aaEthEhmisService] \n ";

            SqlCommand toExecute = new SqlCommand(dropView);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            _helper.Execute(toExecute);

            string createView =

                    "CREATE VIEW [dbo].[v_aaEthEhmisService] \n " +
                    "AS \n " +
                    "SELECT     " + tempTableName1 + ".LabelID, " + tempTableName1 + ".DataEleClass, " + tempTableName1 + ".Month, " + tempTableName1 + ".Quarter, " + tempTableName1 + ".Year, " + tempTableName1 + ".Value, " + tempTableName1 + ".[LEVEL],  \n " +
                    "                      " + tempTableName1 + ".LocationId, " + tempTableName1 + ".RegionId, " + tempTableName1 + ".ZoneId, " + tempTableName1 + ".WoredaID, " + tempTableName1 + ".FACILITTYPE \n " +
                    "FROM         " + tempTableName1 + " \n " +
                    "UNION ALL \n " +
                    "SELECT     " + tempTableName2 + ".LabelID, " + tempTableName2 + ".DataEleClass, " + tempTableName2 + ".Month, " + tempTableName2 + ".Quarter, " + tempTableName2 + ".Year, " + tempTableName2 + ".Value, " + tempTableName2 + ".[LEVEL],  \n " +
                    "                      " + tempTableName2 + ".LocationId, " + tempTableName2 + ".RegionId, " + tempTableName2 + ".ZoneId, " + tempTableName2 + ".WoredaID, " + tempTableName2 + ".FACILITTYPE \n " +
                    "FROM         " + tempTableName2 + " \n " +
                    "UNION ALL \n " +
                    "SELECT     " + tempTableName3 + ".LabelID, " + tempTableName3 + ".DataEleClass, " + tempTableName3 + ".Month, " + tempTableName3 + ".Quarter, " + tempTableName3 + ".Year, " + tempTableName3 + ".Value, " + tempTableName3 + ".[LEVEL],  \n " +
                    "                      " + tempTableName3 + ".LocationId, " + tempTableName3 + ".RegionId, " + tempTableName3 + ".ZoneId, " + tempTableName3 + ".WoredaID, " + tempTableName3 + ".FACILITTYPE \n " +
                    "FROM         " + tempTableName3 + " \n " +
                    "UNION ALL \n " +
                    "SELECT     " + tempTableName4 + ".LabelID, " + tempTableName4 + ".DataEleClass, " + tempTableName4 + ".Month, " + tempTableName4 + ".Quarter, " + tempTableName4 + ".Year, " + tempTableName4 + ".Value, " + tempTableName4 + ".[LEVEL],  \n " +
                    "                      " + tempTableName4 + ".LocationId, " + tempTableName4 + ".RegionId, " + tempTableName4 + ".ZoneId, " + tempTableName4 + ".WoredaID, " + tempTableName4 + ".FACILITTYPE \n " +
                    "FROM         " + tempTableName4 + " \n ";


            toExecute = new SqlCommand(createView);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            _helper.Execute(toExecute);

        }

        private string getInitialStoredProc(string hmisValueTable, string monthQuery, string idQuery,
            string dataEleClassQuery)
        {

            //"set ANSI_NULLS ON \n" +
            //"set QUOTED_IDENTIFIER ON \n  " +
            //"GO \n  \n" +
            string storedProc =
            "   /*  \n  " +
            "      Proc: 		[#proc_Eth_HMIS_ServiceReport_Temp] \n  " +            
            "      Created: 	Feb. 21, 2011 \n  " +

            "      Description: A simple stored proc return aggregate table records \n  " +
            "   */ \n  \n" +

            "   --exec [#proc_Eth_HMIS_ServiceReport_Temp] 2003, 5, 7, 3, 7, 14 \n  " +
            "   Create  procedure [dbo].[#proc_Eth_HMIS_ServiceReport_Temp] \n  " +
            "   as \n  " +

            "   begin --  \n  " +

            "   --drop table AA_TestTemp \n  " +
            " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo]." + tempTableName + "') AND type in (N'U'))\n " +
            " DROP TABLE " + tempTableName + "\n " +
            " \n " +
            " CREATE TABLE " + tempTableName + " (DataEleClass varchar(10), LabelID varchar(30), HmisValue decimal(18,2)) \n  " +
            " \n \n " +
            " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo]." + tempTableName1 + "') AND type in (N'U'))\n " +
            " DROP TABLE " + tempTableName1 + "\n " +
            " \n " +
            " CREATE TABLE " + tempTableName1 + " ( \n  " +
            "    [ValueID] [int] NOT NULL, \n  " +
            "    [LabelID] [int] NOT NULL, \n " +
            "    [DataEleClass] [int] NOT NULL , \n " +
            "    [FederalID] [int] NOT NULL, \n " +
            "    [RegionID] [int] NOT NULL, \n " +
            "    [ZoneID] [int] NOT NULL, \n " +
            "    [WoredaID] [int] NOT NULL, \n " +
            "    [LocationID] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, \n " +
            "    [Week] [int] NOT NULL, \n " +
            "    [Month] [int] NOT NULL, \n " +
            "    [Quarter] [int] NOT NULL, \n " +
            "    [Year] [int] NOT NULL, \n " +
            "    [Value] [decimal](18, 2) NOT NULL, \n " +
            "    [Level] [int] NOT NULL, \n " +
            "    [FACILITTYPE] [int] NOT NULL, \n " +
            "    [DateEntered] [datetime] NOT NULL DEFAULT (getdate()), \n  " +
            "    [Editable] [bit] NOT NULL) " +
                //"    CONSTRAINT [PK_Ethtemp1_New] PRIMARY KEY CLUSTERED  \n  " +
                //"    ( \n  " +
                //"        [ValueID] ASC \n  " +
                //"    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] \n  " +
                //"    ) ON [PRIMARY] \n  " +

            " \n \n  " +

            " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo]." + tempTableName2 + "') AND type in (N'U'))\n " +
            " DROP TABLE " + tempTableName2 + "\n " +
            " \n " +
            " CREATE TABLE " + tempTableName2 + " ( \n  " +
            "    [ValueID] [int] NOT NULL, \n  " +
            "    [LabelID] [int] NOT NULL, \n " +
            "    [DataEleClass] [int] NOT NULL , \n " +
            "    [FederalID] [int] NOT NULL, \n " +
            "    [RegionID] [int] NOT NULL, \n " +
            "    [ZoneID] [int] NOT NULL, \n " +
            "    [WoredaID] [int] NOT NULL, \n " +
            "    [LocationID] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, \n " +
            "    [Week] [int] NOT NULL, \n " +
            "    [Month] [int] NOT NULL, \n " +
            "    [Quarter] [int] NOT NULL, \n " +
            "    [Year] [int] NOT NULL, \n " +
            "    [Value] [decimal](18, 2) NOT NULL, \n " +
            "    [Level] [int] NOT NULL, \n " +
            "    [FACILITTYPE] [int] NOT NULL, \n " +
            "    [DateEntered] [datetime] NOT NULL DEFAULT (getdate()), \n  " +
            "    [Editable] [bit] NOT NULL) " +
                //"    CONSTRAINT [PK_Ethtemp2_New] PRIMARY KEY CLUSTERED  \n  " +
                //"    ( \n  " +
                //"        [ValueID] ASC \n  " +
                //"    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] \n  " +
                //"    ) ON [PRIMARY] \n  " +

            " \n \n  " +
            " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo]." + tempTableName3 + "') AND type in (N'U'))\n " +
            " DROP TABLE " + tempTableName3 + "\n " +
            " \n " +
        " CREATE TABLE " + tempTableName3 + " ( \n  " +
            "    [ValueID] [int] NOT NULL, \n  " +
            "    [LabelID] [int] NOT NULL, \n " +
            "    [DataEleClass] [int] NOT NULL , \n " +
            "    [FederalID] [int] NOT NULL, \n " +
            "    [RegionID] [int] NOT NULL, \n " +
            "    [ZoneID] [int] NOT NULL, \n " +
            "    [WoredaID] [int] NOT NULL, \n " +
            "    [LocationID] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, \n " +
            "    [Week] [int] NOT NULL, \n " +
            "    [Month] [int] NOT NULL, \n " +
            "    [Quarter] [int] NOT NULL, \n " +
            "    [Year] [int] NOT NULL, \n " +
            "    [Value] [decimal](18, 2) NOT NULL, \n " +
            "    [Level] [int] NOT NULL, \n " +
            "    [FACILITTYPE] [int] NOT NULL, \n " +
            "    [DateEntered] [datetime] NOT NULL DEFAULT (getdate()), \n  " +
            "    [Editable] [bit] NOT NULL) " +
                //"    CONSTRAINT [PK_Ethtemp3_New] PRIMARY KEY CLUSTERED  \n  " +
                //"    ( \n  " +
                //"        [ValueID] ASC \n  " +
                //"    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] \n  " +
                //"    ) ON [PRIMARY] \n  " +

            " \n \n  " +
            " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo]." + tempTableName4 + "') AND type in (N'U'))\n " +
            " DROP TABLE " + tempTableName4 + "\n " +
            " \n " +
       " CREATE TABLE " + tempTableName4 + " ( \n  " +
            "    [ValueID] [int] NOT NULL, \n  " +
            "    [LabelID] [int] NOT NULL, \n " +
            "    [DataEleClass] [int] NOT NULL , \n " +
            "    [FederalID] [int] NOT NULL, \n " +
            "    [RegionID] [int] NOT NULL, \n " +
            "    [ZoneID] [int] NOT NULL, \n " +
            "    [WoredaID] [int] NOT NULL, \n " +
            "    [LocationID] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, \n " +
            "    [Week] [int] NOT NULL, \n " +
            "    [Month] [int] NOT NULL, \n " +
            "    [Quarter] [int] NOT NULL, \n " +
            "    [Year] [int] NOT NULL, \n " +
            "    [Value] [decimal](18, 2) NOT NULL, \n " +
            "    [Level] [int] NOT NULL, \n " +
            "    [FACILITTYPE] [int] NOT NULL, \n " +
            "    [DateEntered] [datetime] NOT NULL DEFAULT (getdate()), \n  " +
            "    [Editable] [bit] NOT NULL) " +
                //"    CONSTRAINT [PK_Ethtemp4_New] PRIMARY KEY CLUSTERED  \n  " +
                //"    ( \n  " +
                //"        [ValueID] ASC \n  " +
                //"    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] \n  " +
                //"    ) ON [PRIMARY] \n  " +


            " \n \n " +
                // Summation Group 1
            "    insert into " + tempTableName1 + "  \n  " +
            "    SELECT  *  FROM   " + hmisValueTable + " \n  " + monthQuery + idQuery +
            "    and " + dataEleClassQuery +
            "    and  LabelID IN \n  " +
            "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
            "          aggregationtype = 0  and labelid is not null)  \n  " +

            " \n \n " +
                // Last Month Group 2, Last Month Data Only
            "    insert into " + tempTableName2 + "  \n  " +
            "    SELECT  *  FROM   " + hmisValueTable + " \n  " + monthQuery + idQuery +
            "    and " + dataEleClassQuery +
            "    and  LabelID IN \n  " +
            "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
            "          aggregationtype = 1  and labelid is not null)  \n  " +

            " \n \n " +
                // Group 3, Anding
            "    insert into " + tempTableName3 + "  \n  " +
            "    SELECT  *  FROM   " + hmisValueTable + " \n  " + monthQuery + idQuery +
            "    and " + dataEleClassQuery +
            "    and  LabelID IN \n  " +
            "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
            "          aggregationtype = 3  and labelid is not null)  \n  " +

            " \n \n " +
                // Group 4  Average Data Quality Score
            "    insert into " + tempTableName4 + "  \n  " +
            "    SELECT  *  FROM   " + hmisValueTable + " \n  " + monthQuery + idQuery +
            "    and " + dataEleClassQuery +
            "    and  LabelID IN \n  " +
            "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
            "          aggregationtype = 2  and labelid is not null)  \n  ";

            return storedProc;
        }

        public void resetNumFacilities()
        {
            string labelId = "";

            if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }

            foreach (string labels in newLabelIds)
            {
                if (Convert.ToInt32(aggregateDataHash[labelId + labels]) == 0)
                {
                    aggregateDataHash[labelId + labels] = "-999";
                }
            }

        }

        public void calculateNumFacilities(Hashtable locationIdFacilityTypeId)
        {

            string labelId = "";

            if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }

            foreach (string labels in newLabelIds)
            {
                aggregateDataHash[labelId + labels] = 0;
            }

            //aggregateDataHash[labelId + "_PUBHP"] = 0;
            //aggregateDataHash[labelId + "_PUBHC"] = 0;
            //aggregateDataHash[labelId + "_PUBHOS"] = 0;
            //aggregateDataHash[labelId + "_PUBTOT"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROTOT"] = 0;
            //aggregateDataHash[labelId + "_PRIPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRIPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRIPROTOT"] = 0;
            //aggregateDataHash[labelId + "_AFTOT"] = 0;
            //aggregateDataHash[labelId + "_WHOTOT"] = 0;
            //aggregateDataHash[labelId + "_ZHDTOT"] = 0;
            //aggregateDataHash[labelId + "_RHBTOT"] = 0;
            //aggregateDataHash[labelId + "_FMOHTOT"] = 0;
            //aggregateDataHash[labelId + "_TOT"] = 0; 

            foreach (string locationId in IncludedList)
            {
                //int facilityId = getFacilityType(locationId);
                //int facilityId = 1;

                int facilityId = Convert.ToInt32(locationIdFacilityTypeId[locationId]);

                switch (facilityId)
                {
                    case 1:
                        aggregateDataHash[labelId + "_PUBHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + 1;
                        break;
                    case 2:
                        aggregateDataHash[labelId + "_PUBHC"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + 1;
                        break;
                    case 3:
                        aggregateDataHash[labelId + "_PUBHP"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]) + 1;
                        break;
                    case 4:
                        aggregateDataHash[labelId + "_PRINOPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + 1;
                        break;
                    case 5:
                        aggregateDataHash[labelId + "_PRINOPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]) + 1;
                        break;
                    case 6:
                        aggregateDataHash[labelId + "_PRIPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + 1;
                        break;
                    case 7:
                        aggregateDataHash[labelId + "_PRIPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]) + 1;
                        break;
                    case 8:
                        aggregateDataHash[labelId + "_WHOTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]) + 1;
                        break;
                    case 9:
                        aggregateDataHash[labelId + "_ZHDTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + 1;
                        break;
                    case 10:
                        aggregateDataHash[labelId + "_RHBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + 1;
                        break;
                    case 11:
                        aggregateDataHash[labelId + "_FMOHTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + 1;
                        break;
                }


            }
            //newLabelIds.Add("_PUBHP"); newLabelIds.Add("_PUBHC"); newLabelIds.Add("_PUBHOS");
            //newLabelIds.Add("_PUBTOT"); newLabelIds.Add("_PRINOPROCLI"); newLabelIds.Add("_PRINOPROHOS");
            //newLabelIds.Add("_PRINOPROTOT"); newLabelIds.Add("_PRIPROCLI"); newLabelIds.Add("_PRIPROHOS");
            //newLabelIds.Add("_PRIPROTOT"); newLabelIds.Add("_AFTOT"); newLabelIds.Add("_WHOTOT");
            //newLabelIds.Add("_ZHDTOT"); newLabelIds.Add("_RHBTOT"); newLabelIds.Add("_FMOHTOT");
            //newLabelIds.Add("_TOT");

            aggregateDataHash[labelId + "_PUBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]);
            aggregateDataHash[labelId + "_PRINOPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]);
            aggregateDataHash[labelId + "_PRIPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]);
            aggregateDataHash[labelId + "_AFTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROTOT"]);
            aggregateDataHash[labelId + "_TOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_AFTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]);
        }

        public void calculateNumFacilitiesNew(Hashtable locationIdFacilityTypeId)
        {

            string labelId = "";

            if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }

            foreach (string labels in newLabelIds)
            {
                aggregateDataHash[labelId + labels] = 0;
            }

            //aggregateDataHash[labelId + "_PUBHP"] = 0;
            //aggregateDataHash[labelId + "_PUBHC"] = 0;
            //aggregateDataHash[labelId + "_PUBHOS"] = 0;
            //aggregateDataHash[labelId + "_PUBTOT"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROTOT"] = 0;
            //aggregateDataHash[labelId + "_PRIPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRIPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRIPROTOT"] = 0;
            //aggregateDataHash[labelId + "_AFTOT"] = 0;
            //aggregateDataHash[labelId + "_WHOTOT"] = 0;
            //aggregateDataHash[labelId + "_ZHDTOT"] = 0;
            //aggregateDataHash[labelId + "_RHBTOT"] = 0;
            //aggregateDataHash[labelId + "_FMOHTOT"] = 0;
            //aggregateDataHash[labelId + "_TOT"] = 0; 

            foreach (string locationId in IncludedList)
            {
                //int facilityId = getFacilityType(locationId);
                //int facilityId = 1;

                int facilityId = Convert.ToInt32(locationIdFacilityTypeId[locationId]);

                switch (facilityId)
                {
                    case 1:
                        aggregateDataHash[labelId + "_PUBHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + 1;
                        break;
                    case 2:
                        aggregateDataHash[labelId + "_PUBHC"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + 1;
                        break;
                    case 3:
                        aggregateDataHash[labelId + "_PUBHP"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]) + 1;
                        break;
                    case 4:
                        aggregateDataHash[labelId + "_PRINOPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + 1;
                        break;
                    case 5:
                        aggregateDataHash[labelId + "_PRINOPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]) + 1;
                        break;
                    case 6:
                        aggregateDataHash[labelId + "_PRIPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + 1;
                        break;
                    case 7:
                        aggregateDataHash[labelId + "_PRIPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]) + 1;
                        break;
                    case 8:
                        aggregateDataHash[labelId + "_WHOTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]) + 1;
                        break;
                    case 9:
                        aggregateDataHash[labelId + "_ZHDTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + 1;
                        break;
                    case 10:
                        aggregateDataHash[labelId + "_RHBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + 1;
                        break;
                    case 11:
                        aggregateDataHash[labelId + "_FMOHTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + 1;
                        break;

                    // New Facility types, misganat@tiethio.org, July 3, 2014
                    case 50:
                        aggregateDataHash[labelId + "_GOVCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVCLI"]) + 1;
                        break;
                    case 51:
                        aggregateDataHash[labelId + "_GOVCEN"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVCEN"]) + 1;
                        break;
                    case 52:
                        aggregateDataHash[labelId + "_GOVHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVHOS"]) + 1;
                        break;
                    case 53:
                        aggregateDataHash[labelId + "_PRIPROCEN"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCEN"]) + 1;
                        break;
                    case 54:
                        aggregateDataHash[labelId + "_PRINOPROCEN"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCEN"]) + 1;
                        break;

                }


            }
            //newLabelIds.Add("_PUBHP"); newLabelIds.Add("_PUBHC"); newLabelIds.Add("_PUBHOS");
            //newLabelIds.Add("_PUBTOT"); newLabelIds.Add("_PRINOPROCLI"); newLabelIds.Add("_PRINOPROHOS");
            //newLabelIds.Add("_PRINOPROTOT"); newLabelIds.Add("_PRIPROCLI"); newLabelIds.Add("_PRIPROHOS");
            //newLabelIds.Add("_PRIPROTOT"); newLabelIds.Add("_AFTOT"); newLabelIds.Add("_WHOTOT");
            //newLabelIds.Add("_ZHDTOT"); newLabelIds.Add("_RHBTOT"); newLabelIds.Add("_FMOHTOT");
            //newLabelIds.Add("_TOT");

            aggregateDataHash[labelId + "_PUBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]);
            aggregateDataHash[labelId + "_PRINOPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCEN"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]);
            aggregateDataHash[labelId + "_PRIPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCEN"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]);
            aggregateDataHash[labelId + "_GOVTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_GOVHOS"]) + Convert.ToInt32(aggregateDataHash[labelId + "_GOVCEN"]);
            aggregateDataHash[labelId + "_AFTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_GOVTOT"]);
            //aggregateDataHash[labelId + "_AFTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROTOT"]);
            aggregateDataHash[labelId + "_TOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_AFTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]);
        }

        public void calculateNumFacilities()
        {

            string labelId = "";

            if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }

            foreach (string labels in newLabelIds)
            {
                aggregateDataHash[labelId + labels] = 0;
            }

            //aggregateDataHash[labelId + "_PUBHP"] = 0;
            //aggregateDataHash[labelId + "_PUBHC"] = 0;
            //aggregateDataHash[labelId + "_PUBHOS"] = 0;
            //aggregateDataHash[labelId + "_PUBTOT"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROTOT"] = 0;
            //aggregateDataHash[labelId + "_PRIPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRIPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRIPROTOT"] = 0;
            //aggregateDataHash[labelId + "_AFTOT"] = 0;
            //aggregateDataHash[labelId + "_WHOTOT"] = 0;
            //aggregateDataHash[labelId + "_ZHDTOT"] = 0;
            //aggregateDataHash[labelId + "_RHBTOT"] = 0;
            //aggregateDataHash[labelId + "_FMOHTOT"] = 0;
            //aggregateDataHash[labelId + "_TOT"] = 0; 

            foreach (string locationId in IncludedList)
            {
                int facilityId = getFacilityType(locationId);
                ////int facilityId = 1;

                //int facilityId = Convert.ToInt32(locationIdFacilityTypeId[locationId]);

                switch (facilityId)
                {
                    case 1:
                        aggregateDataHash[labelId + "_PUBHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + 1;
                        break;
                    case 2:
                        aggregateDataHash[labelId + "_PUBHC"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + 1;
                        break;
                    case 3:
                        aggregateDataHash[labelId + "_PUBHP"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]) + 1;
                        break;
                    case 4:
                        aggregateDataHash[labelId + "_PRINOPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + 1;
                        break;
                    case 5:
                        aggregateDataHash[labelId + "_PRINOPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]) + 1;
                        break;
                    case 6:
                        aggregateDataHash[labelId + "_PRIPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + 1;
                        break;
                    case 7:
                        aggregateDataHash[labelId + "_PRIPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]) + 1;
                        break;
                    case 8:
                        aggregateDataHash[labelId + "_WHOTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]) + 1;
                        break;
                    case 9:
                        aggregateDataHash[labelId + "_ZHDTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + 1;
                        break;
                    case 10:
                        aggregateDataHash[labelId + "_RHBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + 1;
                        break;
                    case 11:
                        aggregateDataHash[labelId + "_FMOHTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + 1;
                        break;
                }


            }
            //newLabelIds.Add("_PUBHP"); newLabelIds.Add("_PUBHC"); newLabelIds.Add("_PUBHOS");
            //newLabelIds.Add("_PUBTOT"); newLabelIds.Add("_PRINOPROCLI"); newLabelIds.Add("_PRINOPROHOS");
            //newLabelIds.Add("_PRINOPROTOT"); newLabelIds.Add("_PRIPROCLI"); newLabelIds.Add("_PRIPROHOS");
            //newLabelIds.Add("_PRIPROTOT"); newLabelIds.Add("_AFTOT"); newLabelIds.Add("_WHOTOT");
            //newLabelIds.Add("_ZHDTOT"); newLabelIds.Add("_RHBTOT"); newLabelIds.Add("_FMOHTOT");
            //newLabelIds.Add("_TOT");

            aggregateDataHash[labelId + "_PUBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]);
            aggregateDataHash[labelId + "_PRINOPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]);
            aggregateDataHash[labelId + "_PRIPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]);
            aggregateDataHash[labelId + "_AFTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROTOT"]);
            aggregateDataHash[labelId + "_TOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_AFTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]);
        }

        public void calculateNumFacilitiesNew()
        {

            string labelId = "";

            if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                }
            }

            foreach (string labels in newLabelIds)
            {
                aggregateDataHash[labelId + labels] = 0;
            }

            //aggregateDataHash[labelId + "_PUBHP"] = 0;
            //aggregateDataHash[labelId + "_PUBHC"] = 0;
            //aggregateDataHash[labelId + "_PUBHOS"] = 0;
            //aggregateDataHash[labelId + "_PUBTOT"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRINOPROTOT"] = 0;
            //aggregateDataHash[labelId + "_PRIPROCLI"] = 0;
            //aggregateDataHash[labelId + "_PRIPROHOS"] = 0;
            //aggregateDataHash[labelId + "_PRIPROTOT"] = 0;
            //aggregateDataHash[labelId + "_AFTOT"] = 0;
            //aggregateDataHash[labelId + "_WHOTOT"] = 0;
            //aggregateDataHash[labelId + "_ZHDTOT"] = 0;
            //aggregateDataHash[labelId + "_RHBTOT"] = 0;
            //aggregateDataHash[labelId + "_FMOHTOT"] = 0;
            //aggregateDataHash[labelId + "_TOT"] = 0; 

            foreach (string locationId in IncludedList)
            {
                int facilityId = getFacilityType(locationId);
                ////int facilityId = 1;

                //int facilityId = Convert.ToInt32(locationIdFacilityTypeId[locationId]);

                switch (facilityId)
                {
                    case 1:
                        aggregateDataHash[labelId + "_PUBHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + 1;
                        break;
                    case 2:
                        aggregateDataHash[labelId + "_PUBHC"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + 1;
                        break;
                    case 3:
                        aggregateDataHash[labelId + "_PUBHP"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]) + 1;
                        break;
                    case 4:
                        aggregateDataHash[labelId + "_PRINOPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + 1;
                        break;
                    case 5:
                        aggregateDataHash[labelId + "_PRINOPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]) + 1;
                        break;
                    case 6:
                        aggregateDataHash[labelId + "_PRIPROCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + 1;
                        break;
                    case 7:
                        aggregateDataHash[labelId + "_PRIPROHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]) + 1;
                        break;
                    case 8:
                        aggregateDataHash[labelId + "_WHOTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]) + 1;
                        break;
                    case 9:
                        aggregateDataHash[labelId + "_ZHDTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + 1;
                        break;
                    case 10:
                        aggregateDataHash[labelId + "_RHBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + 1;
                        break;
                    case 11:
                        aggregateDataHash[labelId + "_FMOHTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + 1;
                        break;

                    // New Facility types, misganat@tiethio.org, July 3, 2014
                    case 50:
                        aggregateDataHash[labelId + "_GOVCLI"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVCLI"]) + 1;
                        break;
                    case 51:
                        aggregateDataHash[labelId + "_GOVCEN"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVCEN"]) + 1;
                        break;
                    case 52:
                        aggregateDataHash[labelId + "_GOVHOS"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVHOS"]) + 1;
                        break;
                    case 53:
                        aggregateDataHash[labelId + "_PRIPROCEN"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCEN"]) + 1;
                        break;
                    case 54:
                        aggregateDataHash[labelId + "_PRINOPROCEN"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCEN"]) + 1;
                        break;

                }


            }
            //newLabelIds.Add("_PUBHP"); newLabelIds.Add("_PUBHC"); newLabelIds.Add("_PUBHOS");
            //newLabelIds.Add("_PUBTOT"); newLabelIds.Add("_PRINOPROCLI"); newLabelIds.Add("_PRINOPROHOS");
            //newLabelIds.Add("_PRINOPROTOT"); newLabelIds.Add("_PRIPROCLI"); newLabelIds.Add("_PRIPROHOS");
            //newLabelIds.Add("_PRIPROTOT"); newLabelIds.Add("_AFTOT"); newLabelIds.Add("_WHOTOT");
            //newLabelIds.Add("_ZHDTOT"); newLabelIds.Add("_RHBTOT"); newLabelIds.Add("_FMOHTOT");
            //newLabelIds.Add("_TOT");

            aggregateDataHash[labelId + "_PUBTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHOS"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]);
            aggregateDataHash[labelId + "_PRINOPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROCEN"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROHOS"]);
            aggregateDataHash[labelId + "_PRIPROTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROCEN"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROHOS"]);
            aggregateDataHash[labelId + "_GOVTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_GOVCLI"]) + Convert.ToInt32(aggregateDataHash[labelId + "_GOVHOS"]) + Convert.ToInt32(aggregateDataHash[labelId + "_GOVCEN"]);
            aggregateDataHash[labelId + "_AFTOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRINOPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_PRIPROTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_GOVTOT"]);
            aggregateDataHash[labelId + "_TOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_AFTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_FMOHTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_RHBTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_ZHDTOT"]) + Convert.ToInt32(aggregateDataHash[labelId + "_WHOTOT"]);
        }


        public void setPeriodType()
        {
            if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    periodType = "  periodType = 0  ";
                }
                else if (HMISMainPage.UseNewServiceDataElement2014 == true)
                {
                    periodType = "  periodType = 0  ";
                }
                else
                {
                    periodType = "  (periodType = 0 or periodType = 1)  ";
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    periodType = "  periodType = 3  ";
                }
                else
                {
                    periodType = "  (periodType = 0 or periodType = 1)  ";
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            {
                periodType = "  periodType = 2  ";

            }
        }

        public void startReportTableGeneration(Boolean first)
        {

            if (first == true)
            {
                if (HMISMainPage.UseNewServiceDataElement2014)
                {
                    deleteTablesNew();
                }
                else
                {
                    deleteTables();
                }
            }
            else
            {
                if (!level1Cache)
                {
                    if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
                    {
                        if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                        {
                            string cmdText = "";

                            string sno, activity, LabelID, sequenceno, activityDescription = "", groupID;

                            if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost)
                            {
                                activityDescription = "ActivityHP";
                                //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and  " + periodType + "  order by sequenceno";
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and  periodType = 0  order by sequenceno";
                            }
                            // You have to differenetiate between hospital and health center
                            else if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) // HC and Hosp.
                            {
                                activityDescription = "ActivityHC";
                                // Now determine if Hospital or HC because different things
                                // get reported for them
                                int facilityType = getFacilityType(_reportObj.LocationHMISCode);

                                if (facilityType == 1) // Government Hospital
                                {
                                    //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and  " + periodType + "  order by sequenceno";
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and  periodType = 0  order by sequenceno";
                                }
                                else if ((facilityType == 5) || (facilityType == 7)) // Private Hospital (Profit + Non-Profit)
                                {
                                    //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where private = 1  and  " + periodType + "  order by sequenceno";
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where private = 1  and  periodType = 0  order by sequenceno";
                                }
                                else if (facilityType == 2) // Government HC
                                {
                                    //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 and  " + periodType + "  order by sequenceno";
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 and  periodType = 0  order by sequenceno";
                                }
                                else if ((facilityType == 4) || (facilityType == 6))// Private Clinic (Profit + Non-Profit)
                                {
                                    //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where private = 1 and  " + periodType + "  order by sequenceno";
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where private = 1 and  periodType = 0  order by sequenceno";
                                }
                                // misganat@tiethio.org, July 2, 2014 for the new facilities with
                                // negative number
                                else if ((facilityType >= 50))
                                {
                                    //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where private = 1 and  " + periodType + "  order by sequenceno";
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where private = 1 and  periodType = 0   order by sequenceno";
                                }
                                else
                                {
                                    throw new Exception("SHOULD NEVER GET HERE");
                                }
                            }

                            SqlCommand toExecute;
                            toExecute = new SqlCommand(cmdText);
                            toExecute.CommandTimeout = 4000; //300 // = 1000000;

                            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                            foreach (DataRow row in dt.Rows)
                            {
                                sno = row["SNO"].ToString();
                                sequenceno = row["sequenceno"].ToString();
                                activity = row[activityDescription].ToString();
                                LabelID = row["LabelID"].ToString();
                                groupID = row["GroupID"].ToString();
                                // Call the insert statement

                                InsertServiceFacilityMonthlyData(sno, activity, LabelID, sequenceno, groupID);
                            }
                        }
                        else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                        {

                            string cmdText = "";

                            string sno, activity, LabelID, sequenceno, activityDescription = "", groupID;

                            if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost)
                            {
                                activityDescription = "ActivityHP";

                                if (_reportObj.IsShowOnlyQuartDataElement)
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and  periodType = 1 or quartertitlehp =1  order by sequenceno";
                                else
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and  " + periodType + "  order by sequenceno";
                            }
                            else if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) // HC and Hosp.
                            {
                                activityDescription = "ActivityHC";
                                // Now determine if Hospital or HC because different things
                                // get reported for them
                                int facilityType = getFacilityType(_reportObj.LocationHMISCode);

                                if ((facilityType == 1) || (facilityType == 5) || (facilityType == 7)) // Hospital
                                {
                                    if (_reportObj.IsShowOnlyQuartDataElement)
                                        cmdText = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and (periodType = 1  or quartertitlehc = 1) order by sequenceno";
                                    else
                                        cmdText = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and  " + periodType + "  order by sequenceno";
                                }
                                else if ((facilityType == 2) || (facilityType == 4) || (facilityType == 6))// HC
                                {
                                    if (_reportObj.IsShowOnlyQuartDataElement)
                                        cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 and  (periodType = 1 or quartertitlehc = 1)  order by sequenceno";
                                    else
                                        cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 and  " + periodType + "  order by sequenceno";

                                }
                                // misganat@tiethio.org, July 2, 2014 for the new facilities with
                                // negative number
                                else if ((facilityType >= 50))
                                {
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC= 1 and  " + periodType + "  order by sequenceno";
                                }
                                else
                                {
                                    throw new Exception("SHOULD NEVER GET HERE");
                                }

                            }

                            SqlCommand toExecute;
                            toExecute = new SqlCommand(cmdText);
                            toExecute.CommandTimeout = 4000; //300 // = 1000000;

                            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                            foreach (DataRow row in dt.Rows)
                            {
                                sno = row["SNO"].ToString();
                                sequenceno = row["sequenceno"].ToString();
                                activity = row[activityDescription].ToString();
                                LabelID = row["LabelID"].ToString();
                                groupID = row["GroupID"].ToString();
                                // Call the insert statement

                                InsertServiceFacilityQuarterlyData(sno, activity, LabelID, sequenceno, groupID);
                            }
                        }
                        else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                        {

                            string cmdText = "";

                            string sno, activity, LabelID, sequenceno, activityDescription = "", groupID;

                            if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost)
                            {
                                activityDescription = "ActivityHP";
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and  " + periodType + "  order by sequenceno";
                            }
                            else if (_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) // HC and Hosp.
                            {
                                activityDescription = "ActivityHC";
                                // Now determine if Hospital or HC because different things
                                // get reported for them
                                int facilityType = getFacilityType(_reportObj.LocationHMISCode);

                                if ((facilityType == 1) || (facilityType == 5) || (facilityType == 7)) // Hospital
                                {
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and  " + periodType + "  order by sequenceno";
                                }
                                else if ((facilityType == 2) || (facilityType == 4) || (facilityType == 6))// HC
                                {
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 and  " + periodType + "  order by sequenceno";
                                }
                                // misganat@tiethio.org, July 2, 2014 for the new facilities with
                                // negative number
                                else if ((facilityType >= 50))
                                {
                                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 = 1 and  " + periodType + "  order by sequenceno";
                                }
                                else
                                {
                                    throw new Exception("SHOULD NEVER GET HERE");
                                }

                            }

                            SqlCommand toExecute;
                            toExecute = new SqlCommand(cmdText);
                            toExecute.CommandTimeout = 4000; //300 // = 1000000;

                            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                            foreach (DataRow row in dt.Rows)
                            {
                                sno = row["SNO"].ToString();
                                sequenceno = row["sequenceno"].ToString();
                                activity = row[activityDescription].ToString();
                                LabelID = row["LabelID"].ToString();
                                groupID = row["GroupID"].ToString();
                                // Call the insert statement

                                InsertServiceFacilityMonthlyData(sno, activity, LabelID, sequenceno, groupID);
                            }
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                    {

                        string cmdText = "";


                        string sno, activity, LabelID, activityDescription, sequenceno, groupID;

                        activityDescription = "ActivityWorHO";
                        //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";


                        //// Determine aggregation level and see which data element to filter out

                        int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);

                        if (aggregationLevel == 1) // Federal
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and FMOH = 1 order by sequenceno";
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 2) // Regional
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and RHB = 1 order by sequenceno";
                            // cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 5) // Zonal
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and ZHD = 1 order by sequenceno";
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 3) // Woreda
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 and WHO = 1  order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";

                        }
                        else
                        {
                            throw new Exception("SHOULD NEVER GET HERE");
                        }

                        SqlCommand toExecute;

                        toExecute = new SqlCommand(cmdText);
                        toExecute.CommandTimeout = 4000; //300 // = 1000000;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"].ToString();
                            sequenceno = row["sequenceno"].ToString();
                            activity = row[activityDescription].ToString();
                            LabelID = row["LabelID"].ToString();
                            groupID = row["GroupID"].ToString();
                            // Call the insert statement

                            if (HMISMainPage.UseNewServiceDataElement2014)
                            {
                                InsertAggregateDataNew(sno, activity, LabelID, sequenceno, groupID);
                            }
                            else
                            {
                                InsertAggregateData(sno, activity, LabelID, sequenceno, groupID);
                            }
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    {

                        string cmdText = "";


                        string sno, activity, LabelID, activityDescription, sequenceno, groupID;

                        activityDescription = "ActivityWorHO";
                        //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";


                        //// Determine aggregation level and see which data element to filter out

                        int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);

                        if (aggregationLevel == 1) // Federal
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and FMOH = 1 order by sequenceno";
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            if (_reportObj.IsShowOnlyQuartDataElement)
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where periodType = 1 or (quartertitlehc = 1 or quartertitlehp = 1) order by sequenceno";
                            else
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 2) // Regional
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and RHB = 1 order by sequenceno";
                            // cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            if (_reportObj.IsShowOnlyQuartDataElement)
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where periodType = 1 or (quartertitlehc = 1 or quartertitlehp = 1) order by sequenceno";
                            else
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";

                        }
                        else if (aggregationLevel == 5) // Zonal
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and ZHD = 1 order by sequenceno";
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            if (_reportObj.IsShowOnlyQuartDataElement)
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where periodType = 1 or (quartertitlehc = 1 or quartertitlehp = 1) order by sequenceno";
                            else
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 3) // Woreda
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 and WHO = 1  order by sequenceno";
                            if (_reportObj.IsShowOnlyQuartDataElement)
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where periodType = 1 or (quartertitlehc = 1 or quartertitlehp = 1) order by sequenceno";
                            else
                                cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";

                        }
                        else
                        {
                            throw new Exception("SHOULD NEVER GET HERE");
                        }

                        SqlCommand toExecute;

                        toExecute = new SqlCommand(cmdText);
                        toExecute.CommandTimeout = 4000; //300 // = 1000000;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"].ToString();
                            sequenceno = row["sequenceno"].ToString();
                            activity = row[activityDescription].ToString();
                            LabelID = row["LabelID"].ToString();
                            groupID = row["GroupID"].ToString();
                            // Call the insert statement

                            if (HMISMainPage.UseNewServiceDataElement2014)
                            {
                                InsertAggregateDataNew(sno, activity, LabelID, sequenceno, groupID);
                            }
                            else
                            {
                                InsertAggregateData(sno, activity, LabelID, sequenceno, groupID);
                            }
                        }
                    }
                    else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                    {
                        string cmdText = "";


                        string sno, activity, LabelID, activityDescription, sequenceno, groupId;

                        activityDescription = "ActivityWorHO";
                        //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";


                        //// Determine aggregation level and see which data element to filter out

                        int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);

                        if (aggregationLevel == 1) // Federal
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and FMOH = 1 order by sequenceno";
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + "  order by sequenceno";
                        }
                        else if (aggregationLevel == 2) // Regional
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and RHB = 1 order by sequenceno";
                            // cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 5) // Zonal
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300  and ZHD = 1 order by sequenceno";
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";
                        }
                        else if (aggregationLevel == 3) // Woreda
                        {
                            //cmdText = "SELECT * from  " + viewLabeIdTableName + "  where sequenceno < 300 and WHO = 1  order by sequenceno";
                            cmdText = "SELECT * from  " + viewLabeIdTableName + "  where " + periodType + " order by sequenceno";

                        }
                        else
                        {
                            throw new Exception("SHOULD NEVER GET HERE");
                        }

                        SqlCommand toExecute;

                        toExecute = new SqlCommand(cmdText);
                        toExecute.CommandTimeout = 4000; //300 // = 1000000;

                        DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                        foreach (DataRow row in dt.Rows)
                        {
                            sno = row["SNO"].ToString();
                            sequenceno = row["sequenceno"].ToString();
                            activity = row[activityDescription].ToString();
                            LabelID = row["LabelID"].ToString();
                            groupId = row["GroupID"].ToString();
                            // Call the insert statement

                            if (HMISMainPage.UseNewServiceDataElement2014)
                            {
                                InsertAggregateDataNew(sno, activity, LabelID, sequenceno, groupId);
                            }
                            else
                            {
                                InsertAggregateData(sno, activity, LabelID, sequenceno, groupId);
                            }
                        }
                    }

                    string cmdTextReportTable = "select * from " + reportTableName;
                    SqlCommand toExecuteReportTable = new SqlCommand(cmdTextReportTable);

                    toExecuteReportTable.CommandTimeout = 0; //300 // = 1000000;

                    GenerateReport.globalReportDataTable = _helper.GetDataSet(toExecuteReportTable).Tables[0];

                    string cmdTextStoredProc = "exec " + mainStoredProcName;
                    SqlCommand toExecuteStoredProc = new SqlCommand(cmdTextStoredProc);

                    toExecuteStoredProc.CommandTimeout = 0; //300 // = 1000000;

                    GenerateReport.globalServiceStoreProc = _helper.GetDataSet(toExecuteStoredProc).Tables[0];
                }
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

        private int getFacilityType(string locationID)
        {
            // Given the location ID returns the Facility Name
            string cmdText = "select FacilityTypeID from facility where hmiscode = @locationID";
            int facilityType = 0;

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            toExecute.Parameters.AddWithValue("locationID", locationID);

            DataSet ds = _helper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
            {
                facilityType = Convert.ToInt16(ds.Tables[0].Rows[0]["FacilityTypeID"]);
            }

            return facilityType;
        }

        // Service Report (Main one for higher Admin Levels)
        private void InsertAggregateData(string sno, string Activity, string LabelID, string sequenceno, string groupID)
        {

            SqlCommand toExecute;
            string cmdText = " insert into " + reportTableName + " values (@sno, @Activity, @GroupID, @PFHealthPosts, " +
                                " @PFHealthCenters, @PFHospitals, @PFTotal, @PNClinics, @PNHospitals, @PNTotal, " +
                                " @PPClinics, @PPHospitals, @PPTotal, @AFTotal, @WHOTotal, @ZHDTotal, @RHBTotal, " +
                                " @FMOHTotal, @AHITotal )";


            string PFHealthPosts, PFHealthCenters, PFHospitals, PFTotal, PNClinics, PNHospitals, PNTotal,
                             PPClinics, PPHospitals, PPTotal, AFTotal, WHOTotal, ZHDTotal, RHBTotal,
                             FMOHTotal, AHITotal;

            //if ((LabelID == "") || (LabelID == null))
            if (verticalSumSequnceNo.Contains(sequenceno))
            {
                //Boolean summNeeded = false;

                ArrayList labels = new ArrayList();
                //if (verticalSumSequnceNo.Contains(sequenceno))
                //{
                //    summNeeded = true;
                labels = GetLabels(verticalSumHash[sequenceno].ToString());
                //}                           

                //if (labels.Count > 0)
                //{
                //    summNeeded = true;
                //}

                decimal sumValue = 0;
                //if (summNeeded == true)
                //{
                // Health Post

                string newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBHP";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHealthPosts = sumValue.ToString();
                sumValue = 0;

                // Health Center
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBHC";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHealthCenters = sumValue.ToString();
                sumValue = 0;

                // Hospital
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHospitals = sumValue.ToString();
                sumValue = 0;


                // Public Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFTotal = sumValue.ToString();
                sumValue = 0;

                // Private Non-Profit Clinic
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROCLI";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNClinics = sumValue.ToString();
                sumValue = 0;

                // Private Non-Profit Hospitals
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNHospitals = sumValue.ToString();
                sumValue = 0;

                // Private Non-Profit Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNTotal = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Clinic
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROCLI";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPClinics = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Hospital
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPHospitals = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPTotal = sumValue.ToString();
                sumValue = 0;

                // Public + Private Facilities total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_AFTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                AFTotal = sumValue.ToString();
                sumValue = 0;

                // Woreda Health Office total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_WHOTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                WHOTotal = sumValue.ToString();
                sumValue = 0;

                // Zonal Health Office total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_ZHDTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                ZHDTotal = sumValue.ToString();
                sumValue = 0;

                // RHB total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_RHBTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                RHBTotal = sumValue.ToString();
                sumValue = 0;

                // FMOH total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_FMOHTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                FMOHTotal = sumValue.ToString();
                sumValue = 0;

                // ALL Health Institution total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_TOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                AHITotal = sumValue.ToString();
                sumValue = 0;
                //}
                //else
                //{
                //    PFHealthPosts = "-999";
                //    PFHealthCenters = "-999";
                //    PFHospitals = "-999";
                //    PFTotal = "-999";
                //    PNClinics = "-999";
                //    PNHospitals = "-999";
                //    PNTotal = "-999";
                //    PPClinics = "-999";
                //    PPHospitals = "-999";
                //    PPTotal = "-999";
                //    AFTotal = "-999";
                //    WHOTotal = "-999";
                //    ZHDTotal = "-999";
                //    RHBTotal = "-999";
                //    FMOHTotal = "-999";
                //    AHITotal = "-999";
                //}

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("PFHealthPosts", PFHealthPosts);
                toExecute.Parameters.AddWithValue("PFHealthCenters", PFHealthCenters);
                toExecute.Parameters.AddWithValue("PFHospitals", PFHospitals);
                toExecute.Parameters.AddWithValue("PFTotal", PFTotal);
                toExecute.Parameters.AddWithValue("PNClinics", PNClinics);
                toExecute.Parameters.AddWithValue("PNHospitals", PNHospitals);
                toExecute.Parameters.AddWithValue("PNTotal", PNTotal);
                toExecute.Parameters.AddWithValue("PPClinics", PPClinics);
                toExecute.Parameters.AddWithValue("PPHospitals", PPHospitals);
                toExecute.Parameters.AddWithValue("PPTotal", PPTotal);
                toExecute.Parameters.AddWithValue("AFTotal", AFTotal);
                toExecute.Parameters.AddWithValue("WHOTotal", WHOTotal);
                toExecute.Parameters.AddWithValue("ZHDTotal", ZHDTotal);
                toExecute.Parameters.AddWithValue("RHBTotal", RHBTotal);
                toExecute.Parameters.AddWithValue("FMOHTotal", FMOHTotal);
                toExecute.Parameters.AddWithValue("AHITotal", AHITotal);

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

                PFHealthPosts = LabelID + "_PUBHP"; PFHealthPosts = (aggregateDataHash[PFHealthPosts] == null) ? "-999" : aggregateDataHash[PFHealthPosts].ToString();
                PFHealthCenters = LabelID + "_PUBHC"; PFHealthCenters = (aggregateDataHash[PFHealthCenters] == null) ? "-999" : aggregateDataHash[PFHealthCenters].ToString();
                PFHospitals = LabelID + "_PUBHOS"; PFHospitals = (aggregateDataHash[PFHospitals] == null) ? "-999" : aggregateDataHash[PFHospitals].ToString();
                PFTotal = LabelID + "_PUBTOT"; PFTotal = (aggregateDataHash[PFTotal] == null) ? "-999" : aggregateDataHash[PFTotal].ToString();
                PNClinics = LabelID + "_PRINOPROCLI"; PNClinics = (aggregateDataHash[PNClinics] == null) ? "-999" : aggregateDataHash[PNClinics].ToString();
                PNHospitals = LabelID + "_PRINOPROHOS"; PNHospitals = (aggregateDataHash[PNHospitals] == null) ? "-999" : aggregateDataHash[PNHospitals].ToString();
                PNTotal = LabelID + "_PRINOPROTOT"; PNTotal = (aggregateDataHash[PNTotal] == null) ? "-999" : aggregateDataHash[PNTotal].ToString();
                PPClinics = LabelID + "_PRIPROCLI"; PPClinics = (aggregateDataHash[PPClinics] == null) ? "-999" : aggregateDataHash[PPClinics].ToString();
                PPHospitals = LabelID + "_PRIPROHOS"; PPHospitals = (aggregateDataHash[PPHospitals] == null) ? "-999" : aggregateDataHash[PPHospitals].ToString();
                PPTotal = LabelID + "_PRIPROTOT"; PPTotal = (aggregateDataHash[PPTotal] == null) ? "-999" : aggregateDataHash[PPTotal].ToString();
                AFTotal = LabelID + "_AFTOT"; AFTotal = (aggregateDataHash[AFTotal] == null) ? "-999" : aggregateDataHash[AFTotal].ToString();
                WHOTotal = LabelID + "_WHOTOT"; WHOTotal = (aggregateDataHash[WHOTotal] == null) ? "-999" : aggregateDataHash[WHOTotal].ToString();
                ZHDTotal = LabelID + "_ZHDTOT"; ZHDTotal = (aggregateDataHash[ZHDTotal] == null) ? "-999" : aggregateDataHash[ZHDTotal].ToString();
                RHBTotal = LabelID + "_RHBTOT"; RHBTotal = (aggregateDataHash[RHBTotal] == null) ? "-999" : aggregateDataHash[RHBTotal].ToString();
                FMOHTotal = LabelID + "_FMOHTOT"; FMOHTotal = (aggregateDataHash[FMOHTotal] == null) ? "-999" : aggregateDataHash[FMOHTotal].ToString();
                AHITotal = LabelID + "_TOT"; AHITotal = (aggregateDataHash[AHITotal] == null) ? "-999" : aggregateDataHash[AHITotal].ToString();


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("PFHealthPosts", PFHealthPosts);
                toExecute.Parameters.AddWithValue("PFHealthCenters", PFHealthCenters);
                toExecute.Parameters.AddWithValue("PFHospitals", PFHospitals);
                toExecute.Parameters.AddWithValue("PFTotal", PFTotal);
                toExecute.Parameters.AddWithValue("PNClinics", PNClinics);
                toExecute.Parameters.AddWithValue("PNHospitals", PNHospitals);
                toExecute.Parameters.AddWithValue("PNTotal", PNTotal);
                toExecute.Parameters.AddWithValue("PPClinics", PPClinics);
                toExecute.Parameters.AddWithValue("PPHospitals", PPHospitals);
                toExecute.Parameters.AddWithValue("PPTotal", PPTotal);
                toExecute.Parameters.AddWithValue("AFTotal", AFTotal);
                toExecute.Parameters.AddWithValue("WHOTotal", WHOTotal);
                toExecute.Parameters.AddWithValue("ZHDTotal", ZHDTotal);
                toExecute.Parameters.AddWithValue("RHBTotal", RHBTotal);
                toExecute.Parameters.AddWithValue("FMOHTotal", FMOHTotal);
                toExecute.Parameters.AddWithValue("AHITotal", AHITotal);

                //foreach (SqlParameter Parameter in toExecute.Parameters)
                //{
                //    if (Parameter.Value == null)
                //    {
                //        Parameter.Value = DBNull.Value;
                //    }
                //}

                _helper.Execute(toExecute);

            }
        }

        // Service Report (Main one for higher Admin Levels)
        private void InsertAggregateDataNew(string sno, string Activity, string LabelID, string sequenceno, string groupID)
        {

            SqlCommand toExecute;
            string cmdText;

            cmdText = " insert into " + reportTableName + " values (@sno, @Activity, @GroupID, @PFHealthPosts, " +
                                " @PFHealthCenters, @PFHospitals, @PFTotal, @GVClinics, @GVCenters, @GVHospitals,  @GVTotal, " +
                                " @PPClinics,  @PPCenters, @PPHospitals ,@PPTotal, @PNClinics, @PNCenters, @PNHospitals,  @PNTotal, " +
                                " @AFTotal, @WHOTotal, @ZHDTotal, @RHBTotal, " +
                                " @FMOHTotal, @AHITotal )";


            string PFHealthPosts, PFHealthCenters, PFHospitals, PFTotal, PNClinics, PNHospitals, PNTotal,
                             PPClinics, PPHospitals, PPTotal, GVClinics, GVHospitals, GVCenters, GVTotal, PNCenters, PPCenters,
                             AFTotal, WHOTotal, ZHDTotal, RHBTotal, FMOHTotal, AHITotal;

            //if ((LabelID == "") || (LabelID == null))
            if (verticalSumSequnceNo.Contains(sequenceno))
            {
                //Boolean summNeeded = false;

                ArrayList labels = new ArrayList();
                //if (verticalSumSequnceNo.Contains(sequenceno))
                //{
                //    summNeeded = true;
                labels = GetLabels(verticalSumHash[sequenceno].ToString());
                //}                           

                //if (labels.Count > 0)
                //{
                //    summNeeded = true;
                //}

                decimal sumValue = 0;
                //if (summNeeded == true)
                //{
                // Health Post

                string newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBHP";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHealthPosts = sumValue.ToString();
                sumValue = 0;

                // Health Center
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBHC";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHealthCenters = sumValue.ToString();
                sumValue = 0;

                // Hospital
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHospitals = sumValue.ToString();
                sumValue = 0;


                // Public Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PUBTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFTotal = sumValue.ToString();
                sumValue = 0;
                //// Other Goverment Clinics
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_GOVCLI";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                GVClinics = sumValue.ToString();
                sumValue = 0;

                // Other Goverment Centers
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_GOVCEN";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                GVCenters = sumValue.ToString();
                sumValue = 0;

                // Other Goverment Hospitals
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_GOVHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                GVHospitals = sumValue.ToString();
                sumValue = 0;


                // Other Goverment Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_GOVTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                GVTotal = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Clinic
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROCLI";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPClinics = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Center
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROCEN";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPCenters = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Hospital
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPHospitals = sumValue.ToString();
                sumValue = 0;

                // Private for Profit Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRIPROTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PPTotal = sumValue.ToString();
                sumValue = 0;

                // Private Non-Profit Clinic
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROCLI";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNClinics = sumValue.ToString();
                sumValue = 0;

                // Private Non-Profit Centers
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROCEN";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNCenters = sumValue.ToString();
                sumValue = 0;
                // Private Non-Profit Hospitals
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROHOS";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNHospitals = sumValue.ToString();
                sumValue = 0;


                // Private Non-Profit Total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_PRINOPROTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PNTotal = sumValue.ToString();
                sumValue = 0;



                // Public + Private Facilities total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_AFTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                AFTotal = sumValue.ToString();
                sumValue = 0;

                // Woreda Health Office total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_WHOTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                WHOTotal = sumValue.ToString();
                sumValue = 0;

                // Zonal Health Office total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_ZHDTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                ZHDTotal = sumValue.ToString();
                sumValue = 0;

                // RHB total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_RHBTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                RHBTotal = sumValue.ToString();
                sumValue = 0;

                // FMOH total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_FMOHTOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                FMOHTotal = sumValue.ToString();
                sumValue = 0;

                // ALL Health Institution total
                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_TOT";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                AHITotal = sumValue.ToString();
                sumValue = 0;
                //}
                //else
                //{
                //    PFHealthPosts = "-999";
                //    PFHealthCenters = "-999";
                //    PFHospitals = "-999";
                //    PFTotal = "-999";
                //    PNClinics = "-999";
                //    PNHospitals = "-999";
                //    PNTotal = "-999";
                //    PPClinics = "-999";
                //    PPHospitals = "-999";
                //    PPTotal = "-999";
                //    AFTotal = "-999";
                //    WHOTotal = "-999";
                //    ZHDTotal = "-999";
                //    RHBTotal = "-999";
                //    FMOHTotal = "-999";
                //    AHITotal = "-999";
                //}

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("PFHealthPosts", PFHealthPosts);
                toExecute.Parameters.AddWithValue("PFHealthCenters", PFHealthCenters);
                toExecute.Parameters.AddWithValue("PFHospitals", PFHospitals);
                toExecute.Parameters.AddWithValue("PFTotal", PFTotal);
                toExecute.Parameters.AddWithValue("GVClinics", GVClinics);
                toExecute.Parameters.AddWithValue("GVCenters", GVCenters);
                toExecute.Parameters.AddWithValue("GVHospitals", GVHospitals);
                toExecute.Parameters.AddWithValue("GVTotal", GVTotal);
                toExecute.Parameters.AddWithValue("PPClinics", PPClinics);
                toExecute.Parameters.AddWithValue("PPCenters", PPCenters);
                toExecute.Parameters.AddWithValue("PPHospitals", PPHospitals);
                toExecute.Parameters.AddWithValue("PPTotal", PPTotal);
                toExecute.Parameters.AddWithValue("PNClinics", PNClinics);
                toExecute.Parameters.AddWithValue("PNCenters", PNCenters);
                toExecute.Parameters.AddWithValue("PNHospitals", PNHospitals);
                toExecute.Parameters.AddWithValue("PNTotal", PNTotal);
                toExecute.Parameters.AddWithValue("AFTotal", AFTotal);
                toExecute.Parameters.AddWithValue("WHOTotal", WHOTotal);
                toExecute.Parameters.AddWithValue("ZHDTotal", ZHDTotal);
                toExecute.Parameters.AddWithValue("RHBTotal", RHBTotal);
                toExecute.Parameters.AddWithValue("FMOHTotal", FMOHTotal);
                toExecute.Parameters.AddWithValue("AHITotal", AHITotal);

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

                PFHealthPosts = LabelID + "_PUBHP"; PFHealthPosts = (aggregateDataHash[PFHealthPosts] == null) ? "-999" : aggregateDataHash[PFHealthPosts].ToString();
                PFHealthCenters = LabelID + "_PUBHC"; PFHealthCenters = (aggregateDataHash[PFHealthCenters] == null) ? "-999" : aggregateDataHash[PFHealthCenters].ToString();
                PFHospitals = LabelID + "_PUBHOS"; PFHospitals = (aggregateDataHash[PFHospitals] == null) ? "-999" : aggregateDataHash[PFHospitals].ToString();
                PFTotal = LabelID + "_PUBTOT"; PFTotal = (aggregateDataHash[PFTotal] == null) ? "-999" : aggregateDataHash[PFTotal].ToString();
                GVClinics = LabelID + "_GOVCLI"; GVClinics = (aggregateDataHash[GVClinics] == null) ? "-999" : aggregateDataHash[GVClinics].ToString();
                GVCenters = LabelID + "_GOVCEN"; GVCenters = (aggregateDataHash[GVCenters] == null) ? "-999" : aggregateDataHash[GVCenters].ToString();
                GVHospitals = LabelID + "_GOVHOS"; GVHospitals = (aggregateDataHash[GVHospitals] == null) ? "-999" : aggregateDataHash[GVHospitals].ToString();
                GVTotal = LabelID + "_GOVTOT"; GVTotal = (aggregateDataHash[GVTotal] == null) ? "-999" : aggregateDataHash[GVTotal].ToString();
                PPClinics = LabelID + "_PRIPROCLI"; PPClinics = (aggregateDataHash[PPClinics] == null) ? "-999" : aggregateDataHash[PPClinics].ToString();
                PPCenters = LabelID + "_PRIPROCEN"; PPCenters = (aggregateDataHash[PPCenters] == null) ? "-999" : aggregateDataHash[PPCenters].ToString();
                PPHospitals = LabelID + "_PRIPROHOS"; PPHospitals = (aggregateDataHash[PPHospitals] == null) ? "-999" : aggregateDataHash[PPHospitals].ToString();
                PPTotal = LabelID + "_PRIPROTOT"; PPTotal = (aggregateDataHash[PPTotal] == null) ? "-999" : aggregateDataHash[PPTotal].ToString();
                PNClinics = LabelID + "_PRINOPROCLI"; PNClinics = (aggregateDataHash[PNClinics] == null) ? "-999" : aggregateDataHash[PNClinics].ToString();
                PNCenters = LabelID + "_PRINOPROCEN"; PNCenters = (aggregateDataHash[PNCenters] == null) ? "-999" : aggregateDataHash[PNCenters].ToString();
                PNHospitals = LabelID + "_PRINOPROHOS"; PNHospitals = (aggregateDataHash[PNHospitals] == null) ? "-999" : aggregateDataHash[PNHospitals].ToString();
                PNTotal = LabelID + "_PRINOPROTOT"; PNTotal = (aggregateDataHash[PNTotal] == null) ? "-999" : aggregateDataHash[PNTotal].ToString();
                AFTotal = LabelID + "_AFTOT"; AFTotal = (aggregateDataHash[AFTotal] == null) ? "-999" : aggregateDataHash[AFTotal].ToString();
                WHOTotal = LabelID + "_WHOTOT"; WHOTotal = (aggregateDataHash[WHOTotal] == null) ? "-999" : aggregateDataHash[WHOTotal].ToString();
                ZHDTotal = LabelID + "_ZHDTOT"; ZHDTotal = (aggregateDataHash[ZHDTotal] == null) ? "-999" : aggregateDataHash[ZHDTotal].ToString();
                RHBTotal = LabelID + "_RHBTOT"; RHBTotal = (aggregateDataHash[RHBTotal] == null) ? "-999" : aggregateDataHash[RHBTotal].ToString();
                FMOHTotal = LabelID + "_FMOHTOT"; FMOHTotal = (aggregateDataHash[FMOHTotal] == null) ? "-999" : aggregateDataHash[FMOHTotal].ToString();
                AHITotal = LabelID + "_TOT"; AHITotal = (aggregateDataHash[AHITotal] == null) ? "-999" : aggregateDataHash[AHITotal].ToString();


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("PFHealthPosts", PFHealthPosts);
                toExecute.Parameters.AddWithValue("PFHealthCenters", PFHealthCenters);
                toExecute.Parameters.AddWithValue("PFHospitals", PFHospitals);
                toExecute.Parameters.AddWithValue("PFTotal", PFTotal);
                toExecute.Parameters.AddWithValue("GVClinics", GVClinics);
                toExecute.Parameters.AddWithValue("GVCenters", GVCenters);
                toExecute.Parameters.AddWithValue("GVHospitals", GVHospitals);
                toExecute.Parameters.AddWithValue("GVTotal", GVTotal);
                toExecute.Parameters.AddWithValue("PPClinics", PPClinics);
                toExecute.Parameters.AddWithValue("PPCenters", PPCenters);
                toExecute.Parameters.AddWithValue("PPHospitals", PPHospitals);
                toExecute.Parameters.AddWithValue("PPTotal", PPTotal);
                toExecute.Parameters.AddWithValue("PNClinics", PNClinics);
                toExecute.Parameters.AddWithValue("PNCenters", PNCenters);
                toExecute.Parameters.AddWithValue("PNHospitals", PNHospitals);
                toExecute.Parameters.AddWithValue("PNTotal", PNTotal);
                toExecute.Parameters.AddWithValue("AFTotal", AFTotal);
                toExecute.Parameters.AddWithValue("WHOTotal", WHOTotal);
                toExecute.Parameters.AddWithValue("ZHDTotal", ZHDTotal);
                toExecute.Parameters.AddWithValue("RHBTotal", RHBTotal);
                toExecute.Parameters.AddWithValue("FMOHTotal", FMOHTotal);
                toExecute.Parameters.AddWithValue("AHITotal", AHITotal);

                //foreach (SqlParameter Parameter in toExecute.Parameters)
                //{
                //    if (Parameter.Value == null)
                //    {
                //        Parameter.Value = DBNull.Value;
                //    }
                //}

                _helper.Execute(toExecute);

            }
        }

        // Service Facility Monthly Data
        private void InsertServiceFacilityMonthlyData(string sno, string Activity, string LabelID, string sequenceno, string groupID)
        {

            SqlCommand toExecute;

            string cmdText = " insert into " + reportTableName + " values (@sno, @Activity, @GroupID, @monthData, " +
                               " @month2Data, @month3Data, @quarter ) ";


            string monthData, month2Data, month3Data, quarter;


            //if ((LabelID == "") || (LabelID == null))
            if (verticalSumSequnceNo.Contains(sequenceno))
            {
                //Boolean summNeeded = false;

                ArrayList labels = new ArrayList();
                //if (verticalSumSequnceNo.Contains(sequenceno))
                //{
                //    summNeeded = true;
                labels = GetLabels(verticalSumHash[sequenceno].ToString());
                //}                           

                //if (labels.Count > 0)
                //{
                //    summNeeded = true;
                //}

                decimal sumValue = 0;
                //if (summNeeded == true)
                //{
                // Month Data

                string newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP;
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                monthData = sumValue.ToString();
                sumValue = 0;
                //}
                //else
                //{
                //    monthData = "-999";
                //}

                month2Data = "-999";
                month3Data = "-999";
                quarter = "-999";

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("monthData", monthData);
                toExecute.Parameters.AddWithValue("month2Data", month2Data);
                toExecute.Parameters.AddWithValue("month3Data", month3Data);
                toExecute.Parameters.AddWithValue("quarter", quarter);

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
                month2Data = "-999";
                month3Data = "-999";
                quarter = "-999";

                monthData = LabelID; monthData = (aggregateDataHash[monthData] == null) ? "-999" : aggregateDataHash[monthData].ToString();

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("monthData", monthData);
                toExecute.Parameters.AddWithValue("month2Data", month2Data);
                toExecute.Parameters.AddWithValue("month3Data", month3Data);
                toExecute.Parameters.AddWithValue("quarter", quarter);

                //foreach (SqlParameter Parameter in toExecute.Parameters)
                //{
                //    if (Parameter.Value == null)
                //    {
                //        Parameter.Value = DBNull.Value;
                //    }
                //}
                _helper.Execute(toExecute);

            }
        }

        // Service Quarterly Data
        private void InsertServiceFacilityQuarterlyData(string sno, string Activity, string LabelID, string sequenceno, string groupID)
        {

            SqlCommand toExecute;
            string cmdText = " insert into " + reportTableName + " values (@sno, @Activity, @GroupID, @month1Data, " +
                             "  @month2Data, @month3Data, @quarterData)";


            string @month1Data, @month2Data, @month3Data, @quarterData;

            //if ((LabelID == "") || (LabelID == null))
            if (verticalSumSequnceNo.Contains(sequenceno))
            {
                //Boolean summNeeded = false;

                ArrayList labels = new ArrayList();
                //if (verticalSumSequnceNo.Contains(sequenceno))
                //{
                //    summNeeded = true;
                labels = GetLabels(verticalSumHash[sequenceno].ToString());
                //}                           

                //if (labels.Count > 0)
                //{
                //    summNeeded = true;
                //}


                decimal sumValue = 0;
                //if (summNeeded == true)
                //{
                // Quarter

                string newLabelHP = "";

                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP;
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                quarterData = sumValue.ToString();
                sumValue = 0;

                // Month 3
                int mo = _endMonth;
                string month = mo.ToString();

                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_" + month;
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                month3Data = sumValue.ToString();
                sumValue = 0;

                // Month 2
                if (_endMonth == 1) // quarter 1 unique case
                {
                    mo = 12;
                }
                else
                {
                    mo = _endMonth - 1;
                }

                month = mo.ToString();

                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_" + month;
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                month2Data = sumValue.ToString();
                sumValue = 0;


                // Month 1
                mo = _startMonth;
                month = mo.ToString();

                newLabelHP = "";
                foreach (string labelHP in labels)
                {
                    newLabelHP = labelHP + "_" + month;
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                month1Data = sumValue.ToString();
                sumValue = 0;                

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("quarterData", quarterData);
                toExecute.Parameters.AddWithValue("month3Data", month3Data);
                toExecute.Parameters.AddWithValue("month2Data", month2Data);
                toExecute.Parameters.AddWithValue("month1Data", month1Data);
             
                _helper.Execute(toExecute);
            }
            else
            {

                int mo = _endMonth;
                string month = mo.ToString();

                quarterData = LabelID; quarterData = (aggregateDataHash[quarterData] == null) ? "-999" : aggregateDataHash[quarterData].ToString();

                month3Data = LabelID + "_" + month; month3Data = (aggregateDataHash[month3Data] == null) ? "-999" : aggregateDataHash[month3Data].ToString();

                // Month 2
                if (_endMonth == 1) // quarter 1 unique case
                {
                    mo = 12;
                }
                else
                {
                    mo = _endMonth - 1;
                }
                month = mo.ToString();
                month2Data = LabelID + "_" + month; month2Data = (aggregateDataHash[month2Data] == null) ? "-999" : aggregateDataHash[month2Data].ToString();

                mo = _startMonth;
                month = mo.ToString();
                month1Data = LabelID + "_" + month; month1Data = (aggregateDataHash[month1Data] == null) ? "-999" : aggregateDataHash[month1Data].ToString();             

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("quarterData", quarterData);
                toExecute.Parameters.AddWithValue("month3Data", month3Data);
                toExecute.Parameters.AddWithValue("month2Data", month2Data);
                toExecute.Parameters.AddWithValue("month1Data", month1Data);
              
                _helper.Execute(toExecute);

            }
        }

        public ArrayList GetLabels(string verticalSumID)
        {
            ArrayList labels = new ArrayList();
            string cmdText = "";

            if (HMISMainPage.UseNewServiceDataElement2014)
            {
                cmdText = "  select labelId  from  " + verticalSumIdTableName +
                            "  where ID = " + verticalSumID;

                SqlCommand toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt2.Rows)
                {
                    string labelID = row["labelID"].ToString();
                    //string labelID = seqLabelIDHash[sequenceno].ToString();
                    labels.Add(labelID);
                }
            }
            else
            {
                cmdText = "  select sequenceNO  from  " + verticalSumIdTableName +
                                 "  where ID = " + verticalSumID;

                SqlCommand toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt2.Rows)
                {
                    string sequenceno = row["sequenceNo"].ToString();
                    string labelID = seqLabelIDHash[sequenceno].ToString();
                    labels.Add(labelID);
                }
            }
            return labels;
        }      

        public List<string> GetIncludedLocations()
        {
            string Id = "";

            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {
                Id = _reportObj.LocationHMISCode;
                IncludedList.Add(Id);
            }

            return IncludedList;
        }

        public void UpdateHashTable()
        {
            if ((!level1Cache) && (!level2Cache))
            {
                // string cmdText = "exec   " + storedProcName + "  @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;
                string cmdText = "exec   " + tempStoredProcName; // +"  @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;

                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                string Id = "";

                DataSet dtSet = _helper.GetDataSet(toExecute);

                if ((dtSet.Tables.Count) != 0)
                {
                }

                DataTable dt = dtSet.Tables[0];
                //DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                //aggregateDataHash.Clear();
                aggregateList.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    string LabelID = row["LabelID"].ToString();
                    //decimal HmisValue = Convert.ToDecimal(row["HmisValue"].ToString());
                    string HmisValue = row["HmisValue"].ToString();
                    aggregateDataHash.Add(LabelID, HmisValue);
                }
              

                startReportTableGeneration(false);
            }           

            if (HMISMainPage.UseNewServiceDataElement2014)
            {
                UpdateReportingTableNew();
            }
            else
            {
                UpdateReportingTable();
            }
            GenerateReport.globalServiceStoreProc = AddReadOnlytoReportDataTable();
        }

        private void updateHashTableLevel2Cache()
        {
            level2Cache = true;

            if (HMISMainPage.UseNewServiceDataElement2014)
            {
                deleteTablesNew();
            }
            else
            {
                deleteTables();
            }

            string idQuery = "";
            string monthQuery = "";
            string monthQuery2 = "";
            string cacheFileName = "";
            string cacheAdminType = "";
            string cachePeriodType = "";

            string dataEleClassQuery = " and dataEleClass = 6 ";

            if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                monthQuery = " where  quarter = " + _reportObj.StartQuarter + " and Year =  " + _seleYear;
                cachePeriodType = "Quarter";

                if (_reportObj.StartQuarter == 1)
                {
                    decimal prevYear = _seleYear - 1;
                    monthQuery = "	where  (((Month  between 11 and 12 ) and  Year = " + prevYear + " ) " +
                                            " or ((Month between 1 and " + _endMonth + ") and Year = " + _seleYear + " )) ";
                    monthQuery2 = " where  Month =  " + _endMonth + " and Year = " + _seleYear;
                }
                else
                {
                    monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _seleYear;
                    monthQuery2 = " where  Month =  " + _endMonth + "  and Year =  " + _seleYear;
                }

                cachePeriodType = "Month";
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                //monthQuery = " where  Month =  " + _startMonth + "  and Year =  " + _seleYear;

                monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;
                monthQuery2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;

                if (_startMonth == 11)
                {
                    decimal tempYear = _seleYear - 1;
                    if ((_endMonth == 11) || (_endMonth == 12))
                    {
                        monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth +
                            " and Year =  " + tempYear;
                        monthQuery2 = " where  Month = " + _endMonth + "  and Year =  " + tempYear;
                    }
                    else
                    {
                        monthQuery = " where  (((Month between 11 and 12) and year = " + tempYear + ") OR " +
                                               " ( Month between 1 and " + _endMonth + " and Year = " + _seleYear + " )) ";
                        monthQuery2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;
                    }
                }
                else if (_startMonth == 12)
                {
                    decimal tempYear = _seleYear - 1;
                    if (_endMonth == 12)
                    {
                        monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth +
                            " and Year =  " + tempYear;
                        monthQuery2 = " where  Month = " + _endMonth + "  and Year =  " + tempYear;
                    }
                    else
                    {
                        monthQuery = " where  (((Month = 12) and year = " + tempYear + ") OR " +
                                               " ( Month between 1 and  " + _endMonth + " and Year = " + _seleYear + " )) ";
                        monthQuery2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;
                    }
                }


                cachePeriodType = "Month";
            }

            int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);

            if (aggregationLevel == 1)
            {
                idQuery = "";
                cacheAdminType = "Region";
            }
            else if (aggregationLevel == 2)
            {
                idQuery = " and id =  " + _reportObj.LocationHMISCode;
                cacheAdminType = "Region";
            }
            else if (aggregationLevel == 5)
            {
                idQuery = " and id =  " + _reportObj.LocationHMISCode;
                cacheAdminType = "Zone";
            }
            else if (aggregationLevel == 3)
            {
                idQuery = " and id  =  " + _reportObj.LocationHMISCode;
                cacheAdminType = "Woreda";
            }

            cacheFileName = "CacheLevel2" + cacheAdminType + cachePeriodType;

            if (aggregationLevel != 1)
            {
                idQuery = " and " + cacheFileName + ".id  =  " + _reportObj.LocationHMISCode;
            }
            else
            {
                idQuery = "";
            }

            string cmdText = "";

            //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            //{
            string cmdText1 = "select * from " + cacheFileName + " \ninner join " +
                       viewLabeIdTableName + " on " + viewLabeIdTableName + ".LabelId = " +
                       cacheFileName + ".LabelID \n"
                       + monthQuery + idQuery + dataEleClassQuery + " and aggregationType = 0 ";

            string cmdText2 = "select * from " + cacheFileName + " \ninner join " +
                viewLabeIdTableName + " on " + viewLabeIdTableName + ".LabelId = " +
                cacheFileName + ".LabelID \n"
                + monthQuery2 + idQuery + dataEleClassQuery + " and aggregationType = 1 ";
            cmdText = cmdText1 + "\nUnion \n" + cmdText2;
            //}
            //else
            //{
            //    cmdText = "select * from " + cacheFileName + monthQuery + idQuery + dataEleClassQuery;
            //}

            SqlCommand toExecute = new SqlCommand(cmdText);

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            string numFacilitiesLabelId = "189";

            int pubHos = 0, pubHc = 0, pubHp = 0, priNoProCli = 0, priNoProHos = 0, priProHos = 0,
                priProCli = 0, govCli = 0, govHos = 0, govCen = 0, priNoProCen = 0, priProCen = 0,
                whoTot = 0, zhdTot = 0, rhbTot = 0, fmohTot = 0;

            foreach (DataRow row in dt.Rows)
            {
                string labelId1 = "";
                string labelId2 = "";
                string labelId3 = "";
                string labelId4 = "";

                int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                string LabelID = row["LabelID"].ToString();
                decimal value = Convert.ToDecimal(row["sumValue"].ToString());

                if (LabelID != "188")
                {
                    if (facilityType == 1)
                    {
                        labelId1 = LabelID + "_PUBHOS";
                        labelId2 = LabelID + "_PUBTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 2)
                    {
                        labelId1 = LabelID + "_PUBHC";
                        labelId2 = LabelID + "_PUBTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 3)
                    {
                        labelId1 = LabelID + "_PUBHP";
                        labelId2 = LabelID + "_PUBTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 4)
                    {
                        labelId1 = LabelID + "_PRINOPROCLI";
                        labelId2 = LabelID + "_PRINOPROTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 5)
                    {
                        labelId1 = LabelID + "_PRINOPROHOS";
                        labelId2 = LabelID + "_PRINOPROTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 6)
                    {
                        labelId1 = LabelID + "_PRIPROCLI";
                        labelId2 = LabelID + "_PRIPROTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 7)
                    {
                        labelId1 = LabelID + "_PRIPROHOS";
                        labelId2 = LabelID + "_PRIPROTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 50)
                    {
                        labelId1 = LabelID + "_GOVCLI";
                        labelId2 = LabelID + "_GOVTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 51)
                    {
                        labelId1 = LabelID + "_GOVCEN";
                        labelId2 = LabelID + "_GOVTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 52)
                    {
                        labelId1 = LabelID + "_GOVHOS";
                        labelId2 = LabelID + "_GOVTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 53)
                    {
                        labelId1 = LabelID + "_PRIPROCEN";
                        labelId2 = LabelID + "_PRIPROTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 54)
                    {
                        labelId1 = LabelID + "_PRINOPROCEN";
                        labelId2 = LabelID + "_PRINOPROTOT";
                        labelId3 = LabelID + "_AFTOT";
                        labelId4 = LabelID + "_TOT";
                    }
                    else if (facilityType == 8)
                    {
                        labelId1 = LabelID + "_WHOTOT";
                        labelId3 = LabelID + "_TOT";
                    }
                    else if (facilityType == 9)
                    {
                        labelId1 = LabelID + "_ZHDTOT";
                        labelId3 = LabelID + "_TOT";
                    }
                    else if (facilityType == 10)
                    {
                        labelId1 = LabelID + "_RHBTOT";
                        labelId3 = LabelID + "_TOT";
                    }
                    else if (facilityType == 11)
                    {
                        labelId1 = LabelID + "_FMOHTOT";
                        labelId3 = LabelID + "_TOT";
                    }

                    string HmisValue = value.ToString();

                    if (aggregateDataHash[labelId1] != null)
                    {
                        decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                        newValue = newValue + value;
                        HmisValue = newValue.ToString();
                        aggregateDataHash[labelId1] = HmisValue;
                    }
                    else
                    {
                        aggregateDataHash.Add(labelId1, HmisValue);
                    }

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

                    if (aggregateDataHash[labelId4] != null)
                    {
                        decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId4]);
                        newValue = newValue + value;
                        HmisValue = newValue.ToString();
                        aggregateDataHash[labelId4] = HmisValue;
                    }
                    else
                    {
                        aggregateDataHash.Add(labelId4, HmisValue);
                    }

                }
            }

            calulateNumFacilitiesNew2();

            startReportTableGeneration(false);

            if (HMISMainPage.UseNewServiceDataElement2014)
            {
                UpdateReportingTableNew();
            }
            else
            {
                UpdateReportingTable();
            }

        }

        private void calulateNumFacilitiesNew2()
        {
            string idQuery = "";
            string monthQuery = "";
            string facilityCalcFileName = "";
            string adminType = "";

            string dataEleClassQuery = " and dataEleClass = 6 ";

            //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            //{
            //    monthQuery = " where  quarter = " + _reportObj.StartQuarter + " and Year =  " + _seleYear;

            //    if (_reportObj.StartQuarter == 1)
            //    {
            //        monthQuery = " where   (((Month  =  " + _startMonth + "  or Month = " + _startMonth + "  + 1) and  " +
            //           "  (Year = " + _seleYear + " - 1)) or (Month =  " + _endMonth + "  and Year =  " + _seleYear + "  )) ";
            //    }
            //    else
            //    {
            //        monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;
            //    }

            //}
            //else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            //{
            //    monthQuery = " where  Month =  " + _startMonth + "  and Year =  " + _seleYear;
            //}

            if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            {
                monthQuery = " where  quarter = " + _reportObj.StartQuarter + " and Year =  " + _seleYear;

                if (_reportObj.StartQuarter == 1)
                {
                    decimal prevYear = _seleYear - 1;
                    monthQuery = "	where  (((Month  between 11 and 12 ) and  Year = " + prevYear + " ) " +
                                            " or ((Month between 1 and " + _endMonth + ") and Year = " + _seleYear + " )) ";
                }
                else
                {
                    monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _seleYear;
                }
            }
            else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                //monthQuery = " where  Month =  " + _startMonth + "  and Year =  " + _seleYear;

                monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;

                if (_startMonth == 11)
                {
                    decimal tempYear = _seleYear - 1;
                    if ((_endMonth == 11) || (_endMonth == 12))
                    {
                        monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth +
                            " and Year =  " + tempYear;
                    }
                    else
                    {
                        monthQuery = " where  (((Month between 11 and 12) and year = " + tempYear + ") OR " +
                                               " ( Month between 1 and " + _endMonth + " and Year = " + _seleYear + " )) ";
                    }
                }
                else if (_startMonth == 12)
                {
                    decimal tempYear = _seleYear - 1;
                    if (_endMonth == 12)
                    {
                        monthQuery = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth +
                            " and Year =  " + tempYear;
                    }
                    else
                    {
                        monthQuery = " where  (((Month = 12) and year = " + tempYear + ") OR " +
                                               " ( Month between 1 and  " + _endMonth + " and Year = " + _seleYear + " )) ";
                    }
                }
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

            string numFacilitiesLabelId = "189";

            int pubHos = 0, pubHc = 0, pubHp = 0, priNoProCli = 0, priNoProHos = 0, priProHos = 0,
                priProCli = 0, govCli = 0, govHos = 0, govCen = 0, priNoProCen = 0, priProCen = 0,
                whoTot = 0, zhdTot = 0, rhbTot = 0, fmohTot = 0;

            ArrayList pubHosList = new ArrayList(); ArrayList pubHcList = new ArrayList();
            ArrayList pubHpList = new ArrayList(); ArrayList priNoProCliList = new ArrayList();
            ArrayList priNoProHosList = new ArrayList(); ArrayList priProHosList = new ArrayList();
            ArrayList priProCliList = new ArrayList(); ArrayList govCliList = new ArrayList();
            ArrayList govHosList = new ArrayList(); ArrayList govCenList = new ArrayList();
            ArrayList priNoProCenList = new ArrayList(); ArrayList priProCenList = new ArrayList();
            ArrayList whoTotList = new ArrayList(); ArrayList zhdTotList = new ArrayList();
            ArrayList rhbTotList = new ArrayList(); ArrayList fmohTotList = new ArrayList();

            Hashtable pubHosHash = new Hashtable(); Hashtable pubHcHash = new Hashtable();
            Hashtable pubHpHash = new Hashtable(); Hashtable priNoProCliHash = new Hashtable();
            Hashtable priNoProHosHash = new Hashtable(); Hashtable priProHosHash = new Hashtable();
            Hashtable priProCliHash = new Hashtable(); Hashtable govCliHash = new Hashtable();
            Hashtable govHosHash = new Hashtable(); Hashtable govCenHash = new Hashtable();
            Hashtable priNoProCenHash = new Hashtable(); Hashtable priProCenHash = new Hashtable();
            Hashtable whoTotHash = new Hashtable(); Hashtable zhdTotHash = new Hashtable();
            Hashtable rhbTotHash = new Hashtable(); Hashtable fmohTotHash = new Hashtable();

            ArrayList numFacilitiesQuarter = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {
                int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                string locations = row["locations"].ToString();
                string[] locSplit = locations.Split(',');

                int numFacilities = 0;

                //if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                //{
                //    numFacilities = locSplit.Length;
                //}               

                if (facilityType == 1)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{                        
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];
                        if (pubHosHash[locId] == null)
                        {
                            //if (!pubHosList.Contains(locId))
                            //{
                            pubHosHash[locId] = 1;
                            pubHosList.Add(locId);
                            //}
                        }
                    }
                    numFacilities = pubHosList.Count;
                    //}

                    pubHos = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PUBHOS"] = pubHos;
                }
                else if (facilityType == 2)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (pubHcHash[locId] == null)
                        {
                            //if (!pubHcList.Contains(locId))
                            //{
                            pubHcHash[locId] = 1;
                            pubHcList.Add(locId);
                            //}
                        }
                    }
                    numFacilities = pubHcList.Count;
                    //}

                    pubHc = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PUBHC"] = pubHc;
                }
                else if (facilityType == 3)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (pubHpHash[locId] == null)
                        {
                            //if (!pubHpList.Contains(locId))
                            //{
                            //    pubHpList.Add(locId);
                            //}
                            pubHpHash[locId] = 1;
                            pubHpList.Add(locId);
                        }
                    }
                    numFacilities = pubHpList.Count;
                    // }
                    pubHp = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PUBHP"] = pubHp;
                }
                else if (facilityType == 4)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (priNoProCliHash[locId] == null)
                        {
                            //if (!priNoProCliList.Contains(locId))
                            //{
                            //    priNoProCliList.Add(locId);
                            //}
                            priNoProCliHash[locId] = 1;
                            priNoProCliList.Add(locId);
                        }
                    }
                    numFacilities = priNoProCliList.Count;
                    // }

                    priNoProCli = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PRINOPROCLI"] = priNoProCli;
                }
                else if (facilityType == 5)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (priNoProHosHash[locId] == null)
                        {
                            //if (!priNoProHosList.Contains(locId))
                            //{
                            //    priNoProHosList.Add(locId);
                            //}
                            priNoProHosHash[locId] = 1;
                            priNoProHosList.Add(locId);
                        }
                    }
                    numFacilities = priNoProHosList.Count;
                    // }

                    priNoProHos = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PRINOPROHOS"] = priNoProHos;
                }
                else if (facilityType == 6)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (priProCliHash[locId] == null)
                        {
                            //if (!priProCliList.Contains(locId))
                            //{
                            //    priProCliList.Add(locId);
                            //}
                            priProCliHash[locId] = 1;
                            priProCliList.Add(locId);
                        }
                    }
                    numFacilities = priProCliList.Count;
                    //}

                    priProCli = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PRIPROCLI"] = priProCli;
                }
                else if (facilityType == 7)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (priProHosHash[locId] == null)
                        {
                            //if (!priProHosList.Contains(locId))
                            //{
                            //    priProHosList.Add(locId);
                            //}
                            priProHosHash[locId] = 1;
                            priProHosList.Add(locId);
                        }
                    }
                    numFacilities = priProHosList.Count;
                    //}

                    priProHos = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PRIPROHOS"] = priProHos;
                }
                else if (facilityType == 50)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (govCliHash[locId] == null)
                        {
                            //if (!govCliList.Contains(locId))
                            //{
                            //    govCliList.Add(locId);
                            //}
                            govCliHash[locId] = 1;
                            govCliList.Add(locId);
                        }
                    }
                    numFacilities = govCliList.Count;
                    // }

                    govCli = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_GOVCLI"] = govCli;
                }
                else if (facilityType == 51)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (govCenHash[locId] == null)
                        {
                            //if (!govCenList.Contains(locId))
                            //{
                            //    govCenList.Add(locId);
                            //}
                            govCenHash[locId] = 1;
                            govCenList.Add(locId);
                        }
                    }
                    numFacilities = govCenList.Count;
                    //}

                    govCen = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_GOVCEN"] = govCen; ;
                }
                else if (facilityType == 52)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (govHosHash[locId] == null)
                        {
                            //if (!govHosList.Contains(locId))
                            //{
                            //    govHosList.Add(locId);
                            //}
                            govHosHash[locId] = 1;
                            govHosList.Add(locId);
                        }
                    }
                    numFacilities = govHosList.Count;
                    //}

                    govHos = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_GOVHOS"] = govHos;
                }
                else if (facilityType == 53)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (priProCenHash[locId] == null)
                        {
                            //if (!priProCenList.Contains(locId))
                            //{
                            //    priProCenList.Add(locId);
                            //}
                            priProCenHash[locId] = 1;
                            priProCenList.Add(locId);
                        }
                    }
                    numFacilities = priProCenList.Count;
                    //}

                    priProCen = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PRIPROCEN"] = priProCen;
                }
                else if (facilityType == 54)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (priNoProCenHash[locId] == null)
                        {
                            //if (!priNoProCenList.Contains(locId))
                            //{
                            //    priNoProCenList.Add(locId);
                            //}
                            priNoProCenHash[locId] = 1;
                            priNoProCenList.Add(locId);
                        }
                    }
                    numFacilities = priNoProCenList.Count;
                    //}

                    priNoProCen = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_PRINOPROCEN"] = priNoProCen;
                }
                else if (facilityType == 8)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (whoTotHash[locId] == null)
                        {
                            //if (!whoTotList.Contains(locId))
                            //{
                            //    whoTotList.Add(locId);
                            //}
                            whoTotHash[locId] = 1;
                            whoTotList.Add(locId);
                        }
                    }
                    numFacilities = whoTotList.Count;
                    //}

                    whoTot = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_WHOTOT"] = whoTot;
                }
                else if (facilityType == 9)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (zhdTotHash[locId] == null)
                        {
                            //if (!zhdTotList.Contains(locId))
                            //{
                            //    zhdTotList.Add(locId);
                            //}
                            zhdTotHash[locId] = 1;
                            zhdTotList.Add(locId);
                        }
                    }
                    numFacilities = zhdTotList.Count;
                    //}

                    zhdTot = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_ZHDTOT"] = zhdTot;
                }
                else if (facilityType == 10)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (rhbTotHash[locId] == null)
                        {
                            //if (!rhbTotList.Contains(locId))
                            //{
                            //    rhbTotList.Add(locId);
                            //}
                            rhbTotHash[locId] = 1;
                            rhbTotList.Add(locId);
                        }
                    }
                    numFacilities = rhbTotList.Count;
                    //}

                    rhbTot = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_RHBTOT"] = rhbTot;
                }
                else if (facilityType == 11)
                {
                    //if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                    //{
                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];

                        if (fmohTotHash[locId] == null)
                        {
                            //if (!fmohTotList.Contains(locId))
                            //{
                            //    fmohTotList.Add(locId);
                            //}
                            fmohTotHash[locId] = 1;
                            fmohTotList.Add(locId);
                        }
                    }
                    numFacilities = fmohTotList.Count;
                    //}

                    fmohTot = numFacilities;
                    aggregateDataHash[numFacilitiesLabelId + "_FMOHTOT"] = fmohTot;
                }
            }

            int pubTot = 0, priNoProTot = 0, priProTot = 0, govTot = 0, afTot = 0, tot = 0;
            aggregateDataHash[numFacilitiesLabelId + "_PUBTOT"] = pubTot = pubHos + pubHc + pubHp;
            aggregateDataHash[numFacilitiesLabelId + "_PRINOPROTOT"] = priNoProTot = priNoProCli + priNoProCen + priNoProHos;
            aggregateDataHash[numFacilitiesLabelId + "_PRIPROTOT"] = priProTot = priProCli + priProCen + priProHos;
            aggregateDataHash[numFacilitiesLabelId + "_GOVTOT"] = govTot = govCli + govCen + govHos;
            aggregateDataHash[numFacilitiesLabelId + "_AFTOT"] = afTot = pubTot + priNoProTot + priProTot + govTot;
            aggregateDataHash[numFacilitiesLabelId + "_TOT"] = tot = afTot + fmohTot + rhbTot + zhdTot + whoTot;

        }

       
        public void UpdateReportingTable()
        {
            string cmdText = "";

            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {
                cmdText = "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Month1   Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Month2  Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Month3   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Quarter   Decimal(18,2) ";

                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }
            else //if ((_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service) || (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service))
            {

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PFHealthPosts   Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PFHealthCenters  Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PFHospitals   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PFTotal   Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PNClinics  Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PNHospitals   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PNTotal  Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PPClinics   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PPHospitals  Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column PPTotal   Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column AFTotal  Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column WHOTotal   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column ZHDTotal   Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column RHBTotal  Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column MOHTotal   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column AHITotal   Decimal(18,2) ";


                SqlCommand toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);
            }

            if (!hmisValueTable.Contains("EthioHMIS_HMIS_Value_Temp"))
            {
                _helper.CloseConnection();
            }
        }
        public void UpdateReportingTableNew()
        {
            string cmdText = "";

            if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            {
                cmdText = "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Month1   Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Month2  Decimal(18,2) " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Month3   Decimal(18,2)  " +
                             "ALTER TABLE " + reportTableName + "  " +
                             "Alter Column Quarter   Decimal(18,2) ";

                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }
            else //if ((_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service) || (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service))
            {

                cmdText = "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PFHealthPosts   Decimal(18,2) " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PFHealthCenters  Decimal(18,2) " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PFHospitals   Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PFTotal   Decimal(18,2) " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PNClinics  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PNCenters   Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PNHospitals   Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PNTotal  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PPClinics   Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PPHospitals  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PPCenters  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column PPTotal   Decimal(18,2) " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column GVClinics  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column GVHospitals Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column GVCenters  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column GVTotal  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column AFTotal  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column WHOTotal   Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column ZHDTotal   Decimal(18,2) " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column RHBTotal  Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column MOHTotal   Decimal(18,2)  " +
                                  "ALTER TABLE " + reportTableName + "  " +
                                  "Alter Column AHITotal   Decimal(18,2) ";


                SqlCommand toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);
            }

            if (!hmisValueTable.Contains("EthioHMIS_HMIS_Value_Temp"))
            {
                _helper.CloseConnection();
            }
        } 
     
        /// <summary>
        /// Misgana - Added for the case of Web based ehmis and called during enabling cache. This function adds readonly column to
        /// the existing reportDataTable for the case of shading. At the desktop eHMIS application this function is called
        /// at the user interface which is HMISReportViwer.
        /// </summary>
        /// <returns></returns>
        public DataTable AddReadOnlytoReportDataTable()
        {
            SqlCommand toExecute;
            DataTable dt;

            if (GenerateReport.globalCreateStoredProc != "")
            {
                //string dropStoredProc = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + GenerateReport.globalStoredProcName + "]') AND type in (N'P', N'PC')) " +
                //           " DROP PROCEDURE [dbo].["+ GenerateReport.globalStoredProcName + "]\n ";

                string dropStoredProc =
                         " IF OBJECT_ID('tempdb.." + GenerateReport.globalStoredProcName + "') IS NOT NULL \n " +
                         " DROP PROCEDURE " + GenerateReport.globalStoredProcName;


                toExecute = new SqlCommand(dropStoredProc);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                _helper.Execute(toExecute);

                string storedProcExec = GenerateReport.globalCreateStoredProc;


                toExecute = new SqlCommand(storedProcExec);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                _helper.Execute(toExecute);

            }

            string reportTableName = GenerateReport.globalReportTableName;
            DataTable reportDataTable = GenerateReport.globalReportDataTable;

            //string cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + reportTableName + "]') AND type in (N'U'))  " +
            //              " DROP TABLE " + reportTableName;

            string cmdText =
                             " IF OBJECT_ID('tempdb.." + reportTableName + "') IS NOT NULL \n " +
                             " DROP TABLE " + reportTableName;

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            string cmdTextCreateTable = GenerateReport.globalCreateTable;

            // create the table

            toExecute = new SqlCommand(cmdTextCreateTable);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);


            SqlBulkCopy bulkCopy = new SqlBulkCopy(_helper.Connection);
            bulkCopy.DestinationTableName = reportTableName;

            bulkCopy.WriteToServer(reportDataTable);

            string proc = "EXEC " + mainStoredProcName;
            SqlDataAdapter da = new SqlDataAdapter(proc, _helper.Connection);

            DataSet ds2 = new DataSet();

            da.Fill(ds2);

            dt = ds2.Tables[0];

            return dt;
        }


    }
}
