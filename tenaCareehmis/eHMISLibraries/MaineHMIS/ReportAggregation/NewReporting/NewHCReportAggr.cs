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
using SqlManagement.Database;

using System.IO;
using eHMIS.HMIS.ReportHelper;
using System.Net.Sockets;
using System.Net;

namespace eHMIS.HMIS.ReportAggregation.NewReporting
{

    public class NewHCReportAggr
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
        Hashtable moreIdQuery = new Hashtable();
        Hashtable moreIdQuery2 = new Hashtable();

        string tempStoredProcName = "#proc_Eth_HMIS_ServiceHCReport_Temp";
        string mainStoredProcName = "#proc_ETH_HMIS_Service_Report_reader";
        //string storedProcNoMonth = "proc_Eth_HMIS_ServiceHCReport_NoMonth_Aggregate";

        //string includeListProcName = "proc_Eth_HMIS_Service_HC_IncludedList";
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
        string viewQuery1 = "";
        string viewQuery2 = "";
        string viewQuery3 = "";
        string viewQuery4 = "";
        string _hostName = "";

        string viewName1 = "";
        string viewName2 = "";
        string viewName3 = "";
        string viewName4 = "";

        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation


        public NewHCReportAggr(reportObject reportObj)
        {
            string hostName = Dns.GetHostName();

            hostName = hostName.Trim();
            hostName = hostName.Replace(" ", "");
            hostName = hostName.Replace("-", "_");

            _hostName = hostName;

            viewName1 = "v_temp1" + hostName;
            viewName2 = "v_temp2" + hostName;
            viewName3 = "v_temp3" + hostName;
            viewName4 = "v_temp4" + hostName;

            _helper.ManualCloseConnection = true;

            hmisValueTable = eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable;

            //reportObj.ReportType
            viewLabeIdTableName = reportObj.ViewLabelIdTableName;
            reportTableName = reportObj.ReportTableName;

            GenerateReport.globalReportTableName = reportTableName;
            GenerateReport.globalStoredProcName = mainStoredProcName;

            this._reportObj = reportObj;
            setStartingMonth();

            setPeriodType();

            IncludedList.Clear();
            aggregateDataHash.Clear();


            viewQuery1 = " \n (  SELECT  *  FROM  " + hmisValueTable + " \n  " +
                "    inner join facility on facility.hmiscode = locationId    " +
                "    WHERE  LabelID IN \n  " +
                "          (SELECT labelid FROM   " + viewLabeIdTableName + "  where   \n  " +
                "          aggregationtype = 0  and labelid is not null)  \n  ) as tempQuery1";

            viewQuery2 = " \n (  SELECT  *  FROM  " + hmisValueTable + " \n  " +
               "    inner join facility on facility.hmiscode = locationId    " +
               "    WHERE  LabelID IN \n  " +
               "          (SELECT labelid FROM   " + viewLabeIdTableName + "  where   \n  " +
               "          aggregationtype = 1  and labelid is not null)  \n  ) as tempQuery2";

            viewQuery3 = " \n (  SELECT  *  FROM  " + hmisValueTable + " \n  " +
               "    inner join facility on facility.hmiscode = locationId    " +
               "    WHERE  LabelID IN \n  " +
               "          (SELECT labelid FROM   " + viewLabeIdTableName + "  where   \n  " +
               "          aggregationtype = 3  and labelid is not null)  \n  ) as tempQuery3";

            viewQuery4 = " \n (  SELECT  *  FROM  " + hmisValueTable + " \n  " +
               "    inner join facility on facility.hmiscode = locationId    " +
               "    WHERE  LabelID IN \n  " +
               "          (SELECT labelid FROM   " + viewLabeIdTableName + "  where   \n  " +
               "          aggregationtype = 2  and labelid is not null)  \n  ) as tempQuery4";

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

            newLabelIds.Add("_HP");
            newLabelIds.Add("_HC");
            newLabelIds.Add("_TOT");

            facilityTypes.Add("_HP", "  and FacilitType = 3 ");
            facilityTypes.Add("_HC", "  and FacilitType = 2 ");
            facilityTypes.Add("_TOT", "  and (FacilitType = 2 or FacilitType = 3)");

            moreIdQuery.Add("_HP", ".districtId");
            moreIdQuery.Add("_HC", ".hmisCode");
            moreIdQuery.Add("_TOT", ".hmisCode");

            moreIdQuery2.Add("_HP", ".districtId");
            moreIdQuery2.Add("_HC", ".hmisCode");
            moreIdQuery2.Add("_TOT", ".districtId");

            if (HMISMainPage.UseNewVersion == true)
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElementsNew";
                verticalSumIdTableName = "EthioHIMS_VerticalSumNew";
            }
            else if (HMISMainPage.UseNewServiceDataElement2014 == true)
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElementsNew";
                verticalSumIdTableName = "EthioHIMS_VerticalSumNew";
            }
            else
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElements";
                verticalSumIdTableName = "EthioHIMS_VerticalSum";
            }

            //if (eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable == "EthioHMIS_HMIS_Value_Temp")
            //{
            //    storedProcName = "proc_Eth_HMIS_ServiceHCReport_Import";
            //}
            //else
            //{
            //    storedProcName = "proc_Eth_HMIS_ServiceHCReport_Temp";
            //}

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

        private void setStartingMonth()
        {
            _seleYear = _reportObj.Year;

            switch (_reportObj.StartQuarter)
            {
                case 0: _startMonth = _reportObj.StartMonth;
                    _endMonth = _reportObj.StartMonth;
                    break;
                case 1: _startMonth = 11;
                    _endMonth = 1;
                  
                    break;
                case 2: _startMonth = 2;
                    _endMonth = 4;
                    break;
                case 3: _startMonth = 5;
                    _endMonth = 7;
                    break;
                case 4: _startMonth = 8;
                    _endMonth = 10;
                    break;
            }
        }

        public void deleteTables()
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

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            // If exists remove the Stored Procedure
            //cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proc_ETH_HMIS_Service_Report_reader]') AND type in (N'P', N'PC')) " +
            //           " DROP PROCEDURE [dbo].[#proc_ETH_HMIS_Service_Report_reader]\n " +

            //           "SET ANSI_NULLS ON  \n" +

            //           " SET QUOTED_IDENTIFIER ON \n ";

            cmdText =
                 " IF OBJECT_ID('tempdb.." + mainStoredProcName + "') IS NOT NULL \n " +
                 " DROP procedure " + mainStoredProcName;

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            // Create the Stored Procedure Dynamically


            //cmdText = " CREATE PROCEDURE  [dbo].[#proc_ETH_HMIS_Service_Report_reader] " +
            cmdText = " CREATE PROCEDURE  [dbo]." + mainStoredProcName +

                     " AS BEGIN " +
                     " SET NOCOUNT ON " +
                //" SELECT EthioHIMS_QuarterSrvReport.SNO, EthioHIMS_QuarterSrvReport.GroupID, " +
                //" Activity, Month1 as Amount FROM [dbo].[EthioHIMS_QuarterSrvReport] " +
                //" inner join  " + viewLabeIdTableName + "  on  " +
                //" EthioHIMS_QuarterSrvReport.GroupID =  " +
                //  viewLabeIdTableName + ".GroupID " +
                //  " END ";
                      " SELECT " + reportTableName + ".SNO, " + reportTableName + ".Activity, " +
                      reportTableName + ".HPData, " + reportTableName + ".HCData, " +
                      reportTableName + ".Total, " + viewLabeIdTableName + ".HP, " +
                      viewLabeIdTableName + ".HC, " + viewLabeIdTableName + ".Hospital, " +
                      viewLabeIdTableName + ".Readonly " +
                      " FROM " + reportTableName +
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
                      " [HPData] [decimal](18, 2) NULL, " +
                      " [HCData] [decimal](18, 2) NULL, " +
                      " [Total] [decimal](18, 2) NULL) ";
            //" CONSTRAINT [PK_EthioHIMS_HC_SrvReport] PRIMARY KEY CLUSTERED " +
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
                          "Alter Column HPData   varchar(50) " +
                          "ALTER TABLE " + reportTableName + "  " +
                          "Alter Column HCData  varchar(50) " +
                          "ALTER TABLE " + reportTableName + "  " +
                          "Alter Column Total   varchar(50)  ";


            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            GenerateReport.globalCreateStoredProc = storedProcExec;

            // Reset Identity, to resolve performance issues
            //cmdText = "DBCC CHECKIDENT (" + reportTableName + ", reseed, 0)";

            //toExecute = new SqlCommand(cmdText);

            //_helper.Execute(toExecute);
        }


        private void CreateViews()
        {
            string cmdText = "";

            cmdText = " IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo]." + viewName1 + "')) \n" +
                             " DROP VIEW [dbo]." + viewName1 + "\n" +
                      "SET ANSI_NULLS ON  \n" +

                      " SET QUOTED_IDENTIFIER ON \n ";

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            cmdText = " IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo]." + viewName2 + "')) \n" +
                             " DROP VIEW [dbo]." + viewName2 + "\n" +
                      "SET ANSI_NULLS ON  \n" +

                      " SET QUOTED_IDENTIFIER ON \n ";

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            cmdText = " IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo]." + viewName3 + "')) \n" +
                            " DROP VIEW [dbo]." + viewName3 + "\n" +
                      "SET ANSI_NULLS ON  \n" +

                      " SET QUOTED_IDENTIFIER ON \n ";

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            cmdText = " IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo]." + viewName4 + "')) \n" +
                            " DROP VIEW [dbo]." + viewName4 + "\n" +
                      "SET ANSI_NULLS ON  \n" +

                      " SET QUOTED_IDENTIFIER ON \n ";

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);
            // first create the views

            string view1 =
            " \n \n " +
                // Summation Group 1
                //"    insert into #temp1  \n  " +
                "    Create view " + viewName1 + "\n  " +
                "    AS   " +
                "    SELECT  *  FROM  " + hmisValueTable + " \n  " +
                "    inner join facility on facility.hmiscode = locationId    " +
                "    WHERE  LabelID IN \n  " +
                "          (SELECT labelid FROM   " + viewLabeIdTableName + "  where   \n  " +
                "          aggregationtype = 0  and labelid is not null)  \n  ";


            toExecute = new SqlCommand(view1);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            string view2 = " \n \n " +
                // Last Month Group 2, Last Month Data Only
                //"    insert into #temp2  \n  " +
                "    Create view " + viewName2 + "\n  " +
                     "    AS   " +
                     "    SELECT  *  FROM  " + hmisValueTable + " \n  " +
                     "    inner join facility on facility.hmiscode = locationId   " +
                     "    WHERE  LabelID IN \n  " +
                     "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
                     "          aggregationtype = 1  and labelid is not null)  \n  ";

            toExecute = new SqlCommand(view2);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            string view3 = " \n \n " +
                // Group 3, Anding
                //"    insert into #temp3  \n  " +
                "    Create view " + viewName3 + "\n  " +
                      "    AS   " +
                      "    SELECT  *  FROM  " + hmisValueTable + "  \n  " +
                      "    inner join facility on facility.hmiscode = locationId   " +
                      "    WHERE  LabelID IN \n  " +
                      "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
                      "          aggregationtype = 3  and labelid is not null)  \n  ";

            toExecute = new SqlCommand(view3);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            string view4 = " \n \n " +
                // Group 4  Average Data Quality Score
                //"    insert into #temp4  \n  " +
                "    Create view " + viewName4 + "\n  " +
                    "    AS   " +
                    "    SELECT  *  FROM  " + hmisValueTable + "  \n  " +
                    "    inner join facility on facility.hmiscode = locationId " +
                    "    WHERE  LabelID IN \n  " +
                    "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
                    "          aggregationtype = 2  and labelid is not null)  ";

            toExecute = new SqlCommand(view4);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

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
            string cmdText =
                    " IF OBJECT_ID('tempdb.." + tempStoredProcName + "') IS NOT NULL \n " +
                    " DROP procedure " + tempStoredProcName;

            //string cmdText = " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[#proc_Eth_HMIS_ServiceHCReport_Temp]') AND type in (N'P', N'PC')) " +
            //           " DROP PROCEDURE [dbo].[#proc_Eth_HMIS_ServiceHCReport_Temp]\n " +

            //           "SET ANSI_NULLS ON  \n" +

            //           " SET QUOTED_IDENTIFIER ON \n ";

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            CreateViews();

            // Use dynamic query instead of a view to avoid sharing issue between
            // different users

            string storedProc =
                //"set ANSI_NULLS ON \n" +
                //"set QUOTED_IDENTIFIER ON \n  " +
                //"GO \n  \n" +

                "   /*  \n  " +
                "      Proc: 		[#proc_Eth_HMIS_ServiceHCReport_Temp] \n  " +
                "       Author:	  	Merra Kokebie  \n  " +
                "       Created: 	Feb. 21, 2011 \n  " +

                "      Description: A simple stored proc return aggregate table records \n  " +
                "   */ \n  \n" +

                "   --exec [#proc_Eth_HMIS_ServiceHCReport_Temp] 2003, 5, 7, 3, 7, 14 \n  " +
                "   Create  procedure [dbo].[#proc_Eth_HMIS_ServiceHCReport_Temp] \n  " +
                "   as \n  " +

                "   begin --  \n  " +

                //"   --drop table AA_TestTemp \n  " +
                " CREATE TABLE #temp (DataEleClass varchar(10), LabelID varchar(30), HmisValue decimal(18,2)) \n  " +

                " \n \n ";


            string group1File = viewName1;
            string group2File = viewName2;
            string group3File = viewName3;
            string group4File = viewName4;

            //string group1File = viewQuery1;
            //string group2File = viewQuery2;
            //string group3File = viewQuery3;
            //string group4File = viewQuery4;

            //string tempQuery1 = "tempQuery1";
            //string tempQuery2 = "tempQuery2";
            //string tempquery3 = "tempQuery3";
            //string tempquery4 = "tempQuery4";

            string idQuery1 = "", idQuery2 = "", idQuery3 = "", idQuery4 = "";
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

            //idQuery1 = "   and v_temp1.districtId =    " + _reportObj.LocationHMISCode;
            //idQuery2 = "   and v_temp2.districtId =    " + _reportObj.LocationHMISCode;
            //idQuery3 = "   and v_temp3.districtId =    " + _reportObj.LocationHMISCode;
            //idQuery4 = "   and v_temp4.districtId =    " + _reportObj.LocationHMISCode;

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
                    //monthYearQueryGroup1 = " where   (((Month  = @StartMonth or Month = @StartMonth + 1) and  " +
                    // "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) ";
                    //monthYearQueryGroup2 = " where  Month = @EndMonth and Year = @startYear ";

                    monthYearQueryGroup1 = " where   (((Month  =  " + _startMonth + "  or Month = " + _startMonth + "  + 1) and  " +
                     "  (Year = " + _seleYear + " - 1)) or (Month =  " + _endMonth + "  and Year =  " + _seleYear + "  )) ";
                    monthYearQueryGroup2 = " where  Month =  " + _endMonth + "  and Year =  " + _seleYear;

                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
                {
                    monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;
                    monthYearQueryGroup2 = " where  Month = " + _endMonth + "  and Year =  " + _seleYear;
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
                {
                    //if (HMISMainPage.UseNewVersion == true)
                    //{
                    //    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Month =  " + _startMonth + "  and Year =  " + _seleYear;
                    //}
                    //else
                    //{
                    //monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Month =  " + _startMonth + "  and level > 0 and Year =  " + _seleYear;
                    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Month =  " + _startMonth + "  and level = 0 and Year =  " + _seleYear;
                    //}
                }
                else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
                {
                    monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Year =  " + _seleYear;
                }
            }

            // Facility
            //if ((_reportObj.RepGlobalType == reportObject.ReportGlobalType.facility) || (_reportObj.RepGlobalType == reportObject.ReportGlobalType.HealthPost))
            //{

            //    IncludedList.Clear();
            //    IncludedList.Add(_reportObj.LocationHMISCode);
            //    calculateNumFacilities();

            //    if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            //    {
            //        // Facility Monthly
            //        // Group 1 -> 
            //        cmdText1 =
            //           " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           "	select DataEleClass, LabelID, Value from   " + group1File + "  \n  " +
            //           monthYearQueryGroup1 + idQuery1;

            //        // Group 2 -> 
            //        cmdText2 =
            //           " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           "	select DataEleClass, LabelID, Value from   " + group2File + "  \n  " +
            //           monthYearQueryGroup1 + idQuery2;

            //        // Group 3 -> Value
            //        cmdText3 =
            //           " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           "	select DataEleClass, LabelID, Value from   " + group3File + "  \n  " +
            //           monthYearQueryGroup1 + idQuery3;

            //        // Group 4 -> Value
            //        cmdText4 =
            //           " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           "	select DataEleClass, LabelID, Value from   " + group4File + "  \n  " +
            //           monthYearQueryGroup1 + idQuery4;
            //    }
            //    else if (_reportObj.RepKind == reportObject.ReportKind.Quarterly_Service)
            //    {
            //        // Facility Quarterly
            //        // QuarterData

            //        // Group 1 -> Summation
            //        cmdText1 +=
            //        " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //        "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //        "   sum(Value) as Value from  " + group1File + "  \n  " +
            //        monthYearQueryGroup1 + idQuery1 +
            //        "	group by DataEleClass,  LabelID  \n\n";

            //        // Group 2 -> Last Value, End of Month
            //        cmdText2 +=
            //       " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //       "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //       "   sum(Value) as Value from  " + group2File + "  \n  " +
            //       monthYearQueryGroup2 + idQuery2 +
            //       "	group by DataEleClass,  LabelID  \n\n";

            //        // Group 3 -> Average
            //        cmdText3 +=
            //       " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //       "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //       "   cast (sum(Value)/3 as int) as Value from  " + group3File + "  \n  " +
            //       monthYearQueryGroup1 + idQuery3 +
            //       "	group by DataEleClass,  LabelID  \n\n";

            //        // Group 4 -> Average
            //        cmdText4 +=
            //       " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //       "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //       "   sum(Value)/3 as Value from  " + group4File + "  \n  " +
            //       monthYearQueryGroup1 + idQuery4 +
            //       "	group by DataEleClass,  LabelID  \n\n";

            //        // Individual Monthly Data
            //        for (int i = _startMonth; i <= _endMonth; i++)
            //        {
            //            int month = i;
            //            // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _seleYear;

            //            // Group 1 -> Summation  ethMonth[i]
            //            cmdText1 +=
            //            " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //            "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
            //            "   Value from  " + group1File + "  \n  " +
            //            "   where  Month  = " + i + " and Year =  " + _seleYear + idQuery1 +
            //            "	\n\n";

            //            // Group 2 -> Last Value, End of Month
            //            cmdText2 +=
            //           " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
            //           "    Value from  " + group2File + "  \n  " +
            //           "    where  Month  = " + i + " and Year =  " + _seleYear + idQuery2 +
            //           "	 \n\n";

            //            // Group 3 -> Average
            //            cmdText3 +=
            //           " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
            //           "    Value from  " + group3File + "  \n  " +
            //           "    where  Month  = " + i + " and Year =  " + _seleYear + idQuery3 +
            //           "	\n\n";

            //            // Group 3 -> Average
            //           // cmdText4 +=
            //           //" insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //           //"	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
            //           //"   avg(Value) as Value from  " + group4File + "  \n  " +
            //           //" where  Month  = " + i + " and Year =  " + _seleYear + idQuery +
            //           //"	group by DataEleClass,  LabelID  \n\n";
            //            cmdText4 +=
            //          " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //          "	select DataEleClass, cast(LabelID as VarChar) + '_" + i + "' as LabelID, \n  " +
            //          " Value from  " + group4File + "  \n  " +
            //          " where  Month  = " + i + " and Year =  " + _seleYear + idQuery4 +
            //          "	\n\n";
            //        }
            //    }
            //    else if (_reportObj.RepKind == reportObject.ReportKind.Annual_Service)
            //    {
            //        // Facility Quarterly
            //        // QuarterData

            //        // Group 1 -> Summation
            //        cmdText1 +=
            //        " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //        "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //        "   sum(Value) as Value from  " + group1File + "  \n  " +
            //        monthYearQueryGroup1 + idQuery1 +
            //        "	group by DataEleClass,  LabelID  \n\n";

            //        // Group 2 -> Last Value, End of Month
            //        cmdText2 +=
            //       " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //       "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //       "   sum(Value) as Value from  " + group2File + "  \n  " +
            //       monthYearQueryGroup2 + idQuery2 +
            //       "	group by DataEleClass,  LabelID  \n\n";

            //        // Group 3 -> Average
            //        cmdText3 +=
            //       " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //       "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //       "   cast (sum(Value)/3 as int) as Value from  " + group3File + "  \n  " +
            //       monthYearQueryGroup1 + idQuery3 +
            //       "	group by DataEleClass,  LabelID  \n\n";

            //        // Group 4 -> Average
            //        cmdText4 +=
            //       " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
            //       "	select DataEleClass, cast(LabelID as VarChar) as LabelID, \n  " +
            //       "   sum(Value)/3 as Value from  " + group4File + "  \n  " +
            //       monthYearQueryGroup1 + idQuery4 +
            //       "	group by DataEleClass,  LabelID  \n\n";
            //    }
            //}
            if (true)//else // Not a Facility
            {
                //cmdText1 +=
                //        " -- Here calculate the number of Locations Involved \n\n" +
                //        "    declare @locationCount int \n " +
                //        "    set @locationCount = (select count(distinct(locationId)) " +
                //        "    from EthEhmis_HmisValue  \n" +
                //             monthYearQueryGroup1 + idQuery1 + ") \n\n\n";

                //string includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                //                           " inner join facility on facility.hmiscode = locationid " +
                //                             monthYearQueryGroup1 + idQuery1 + " and " + dataEleClassQuery +
                //                             "  and labelId in  " +
                //                             "  (select labelId from   " + viewLabeIdTableName + 
                //                             "  where  " + periodType + " )";                

                //toExecute = new SqlCommand(includedLocations);
                //_helper.Execute(toExecute);                            

                //DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                //int numberOfFacilities = 0;
                //IncludedList.Clear();
                //foreach (DataRow row in dt2.Rows)
                //{
                //    string LocationID = row["LocationID"].ToString();
                //    IncludedList.Add(LocationID);
                //    numberOfFacilities++;
                //}   

                //// Calculate Number of Facilities, LabelID for Number of Facilities = 189
                //calculateNumFacilities();

                foreach (string newlabelId in newLabelIds)
                {

                    idQuery1 = "  and (" + group1File + moreIdQuery[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  " + " or " + group1File + moreIdQuery2[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  )";
                    idQuery2 = "  and (" + group2File + moreIdQuery[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  " + " or " + group2File + moreIdQuery2[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  )";
                    idQuery3 = "  and (" + group3File + moreIdQuery[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  " + " or " + group3File + moreIdQuery2[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  )";
                    idQuery4 = "  and (" + group4File + moreIdQuery[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  " + " or " + group4File + moreIdQuery2[newlabelId] + " = '" +
                        _reportObj.LocationHMISCode + "'  )";

                    //idQuery1 = "  and (v_temp1" + moreIdQuery[newlabelId] + " = '" +
                    //     _reportObj.LocationHMISCode + "'  " + " or v_temp1" + moreIdQuery2[newlabelId] + " = '" +
                    //     _reportObj.LocationHMISCode + "'  )";
                    //idQuery2 = "  and (v_temp2" + moreIdQuery[newlabelId] + " = '" +
                    //    _reportObj.LocationHMISCode + "'  " + " or v_temp2" + moreIdQuery2[newlabelId] + " = '" +
                    //    _reportObj.LocationHMISCode + "'  )";
                    //idQuery3 = "  and (v_temp3" + moreIdQuery[newlabelId] + " = '" +
                    //    _reportObj.LocationHMISCode + "'  " + " or v_temp3" + moreIdQuery2[newlabelId] + " = '" +
                    //    _reportObj.LocationHMISCode + "'  )";
                    //idQuery4 = "  and (v_temp4" + moreIdQuery[newlabelId] + " = '" +
                    //    _reportObj.LocationHMISCode + "'  " + " or v_temp4" + moreIdQuery2[newlabelId] + " = '" +
                    //    _reportObj.LocationHMISCode + "'  )";


                    // Group 1 -> Summation
                    cmdText1 += "\n\n";
                    cmdText1 +=
                      " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
                     "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                     "   sum(Value) as Value from  " + group1File + " " + innerJoinGroup1 +
                     monthYearQueryGroup1 +
                        idQuery1 + facilityTypes[newlabelId] +
                    "	group by DataEleClass,  LabelID  ";

                    // Group 2 -> Last Value End of Month
                    cmdText2 += "\n\n";
                    cmdText2 +=
                      " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
                     "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                     "   sum(Value) as Value from " + group2File + " " + innerJoinGroup2 +
                     monthYearQueryGroup2 +
                        idQuery2 + facilityTypes[newlabelId] +
                    "	group by DataEleClass,  LabelID  ";

                    // Group 3 -> Average
                    cmdText3 += "\n\n";
                    cmdText3 +=
                     " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
                     "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                        // "  cast(sum(Value)/@locationCount as int) as Value from " + group3File + " " + innerJoinGroup3 +
                        // This is for Tracer Drugs, for Higher Admin levels, you add the values
                        //"  cast(avg(Value) as int) as Value from " + group3File + " " + innerJoinGroup3 +
                    "  sum(value) as Value from " + group3File + " " + innerJoinGroup3 +
                    monthYearQueryGroup1 +
                        idQuery3 + facilityTypes[newlabelId] +
                    "	group by DataEleClass,  LabelID  ";

                    // Group 3 -> Average
                    cmdText4 += "\n\n";
                    cmdText4 +=
                      " insert into #temp (DataEleClass, LabelID, HmisValue) \n  " +
                     "	select DataEleClass, cast(LabelID as VarChar) + '" + newlabelId + "' as LabelID, \n  " +
                        //"   sum(Value)/@locationCount as Value from " + group4File + " " + innerJoinGroup4 +
                     "   avg(Value) as Value from " + group4File + " " + innerJoinGroup4 +
                     monthYearQueryGroup1 +
                        idQuery4 + facilityTypes[newlabelId] +
                    "	group by DataEleClass,  LabelID  ";
                }
            }

            storedProc += " \n \n  " + cmdText1 + " \n \n  " + cmdText2 + " \n \n  " + cmdText3 + " \n \n  " + cmdText4;
            storedProc += " \n\nselect LabelId, HmisValue from #temp  where  " + dataEleClassQuery + "  \n";
            storedProc += " \nEND ";

            toExecute = new SqlCommand(storedProc);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            resetNumFacilities();

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

                switch (facilityId)
                {
                    case 2:
                        aggregateDataHash[labelId + "_HC"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHC"]) + 1;
                        break;
                    case 3:
                        aggregateDataHash[labelId + "_HP"] = Convert.ToInt32(aggregateDataHash[labelId + "_PUBHP"]) + 1;
                        break;

                }


            }
            //newLabelIds.Add("_PUBHP"); newLabelIds.Add("_PUBHC"); newLabelIds.Add("_PUBHOS");
            //newLabelIds.Add("_PUBTOT"); newLabelIds.Add("_PRINOPROCLI"); newLabelIds.Add("_PRINOPROHOS");
            //newLabelIds.Add("_PRINOPROTOT"); newLabelIds.Add("_PRIPROCLI"); newLabelIds.Add("_PRIPROHOS");
            //newLabelIds.Add("_PRIPROTOT"); newLabelIds.Add("_AFTOT"); newLabelIds.Add("_WHOTOT");
            //newLabelIds.Add("_ZHDTOT"); newLabelIds.Add("_RHBTOT"); newLabelIds.Add("_FMOHTOT");
            //newLabelIds.Add("_TOT");

            aggregateDataHash[labelId + "_TOT"] = Convert.ToInt32(aggregateDataHash[labelId + "_HC"]) + Convert.ToInt32(aggregateDataHash[labelId + "_HP"]);
        }

        public void setPeriodType()
        {
            if (_reportObj.RepKind == reportObject.ReportKind.Monthly_Service)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    periodType = "  periodType = 0  ";
                }
                else
                {
                    if (HMISMainPage.UseNewServiceDataElement2014)
                        periodType = "  (periodType = 0)  ";
                    else
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
                deleteTables();
            }
            else
            {

                string cmdText = "";

                string sno, activity, LabelID, sequenceno, activityDescription = "", groupID;


                activityDescription = "ActivityHC";
                if (_reportObj.IsShowOnlyQuartDataElement)
                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where (HCHP = 1) and  periodType = 1  or quartertitlehp = 1 or quartertitlehc = 1  order by sequenceno";
                else
                    cmdText = "SELECT * from  " + viewLabeIdTableName + "  where (HCHP = 1) and  " + periodType + "  order by sequenceno";

                // You have to differenetiate between hospital and health center

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

                    InsertAggregateData(sno, activity, LabelID, sequenceno, groupID);
                }

                string cmdTextReportTable = "select * from " + reportTableName;
                SqlCommand toExecuteReportTable = new SqlCommand(cmdTextReportTable);

                toExecuteReportTable.CommandTimeout = 4000; //300 // = 1000000;

                GenerateReport.globalReportDataTable = _helper.GetDataSet(toExecuteReportTable).Tables[0];
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

        // OPD Disease Report (Main one for higher Admin Levels)
        private void InsertAggregateData(string sno, string Activity, string LabelID, string sequenceno, string groupID)
        {

            SqlCommand toExecute;
            string cmdText = " insert into " + reportTableName + " values (@sno, @Activity, @GroupID, @PFHealthPosts, " +
                                " @PFHealthCenters, @AHITotal )";


            string PFHealthPosts, PFHealthCenters, PFHospitals, AHITotal;

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
                    newLabelHP = labelHP + "_HP";
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
                    newLabelHP = labelHP + "_HC";
                    newLabelHP = (aggregateDataHash[newLabelHP] == null) ? "0" : aggregateDataHash[newLabelHP].ToString();
                    sumValue = sumValue + Convert.ToDecimal(newLabelHP);
                }

                if (sumValue == 0)
                    sumValue = -999;

                PFHealthCenters = sumValue.ToString();
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

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("PFHealthPosts", PFHealthPosts);
                toExecute.Parameters.AddWithValue("PFHealthCenters", PFHealthCenters);
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

                PFHealthPosts = LabelID + "_HP"; PFHealthPosts = (aggregateDataHash[PFHealthPosts] == null) ? "-999" : aggregateDataHash[PFHealthPosts].ToString();
                PFHealthCenters = LabelID + "_HC"; PFHealthCenters = (aggregateDataHash[PFHealthCenters] == null) ? "-999" : aggregateDataHash[PFHealthCenters].ToString();
                AHITotal = LabelID + "_TOT"; AHITotal = (aggregateDataHash[AHITotal] == null) ? "-999" : aggregateDataHash[AHITotal].ToString();


                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("PFHealthPosts", PFHealthPosts);
                toExecute.Parameters.AddWithValue("PFHealthCenters", PFHealthCenters);
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
                mo = _endMonth - 1;
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
                //}
                //else
                //{
                //    quarterData = "-999";
                //    month3Data = "-999";
                //    month2Data = "-999";
                //    month1Data = "-999";                    
                //}

                // Special Case for TB and Leprosy, this should be identified as Period 1 or some other
                // Items in the Service Element, temporarly put it as hard coded element which is not
                // good for long term maintainability
                // 2b.1.1 2b.1.3 2b.1.4

                // Merra Kokebie, July 25, 2011, We are commenting the special case for TB, we are 
                // showing it in both the monthly and quarterly for the previous report versions

                //----------//---------------------
                //if ((sno.ToUpper().Contains("2B.1.1")) || (sno.ToUpper().Contains("2B.1.3")) || (sno.ToUpper().Contains("2B.1.4")))
                //{
                //    month1Data = "-999";
                //    month2Data = "-999";
                //    month3Data = "-999";
                //}

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("quarterData", quarterData);
                toExecute.Parameters.AddWithValue("month3Data", month3Data);
                toExecute.Parameters.AddWithValue("month2Data", month2Data);
                toExecute.Parameters.AddWithValue("month1Data", month1Data);

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

                int mo = _endMonth;
                string month = mo.ToString();

                quarterData = LabelID; quarterData = (aggregateDataHash[quarterData] == null) ? "-999" : aggregateDataHash[quarterData].ToString();

                month3Data = LabelID + "_" + month; month3Data = (aggregateDataHash[month3Data] == null) ? "-999" : aggregateDataHash[month3Data].ToString();

                mo = _endMonth - 1;
                month = mo.ToString();
                month2Data = LabelID + "_" + month; month2Data = (aggregateDataHash[month2Data] == null) ? "-999" : aggregateDataHash[month2Data].ToString();

                mo = _startMonth;
                month = mo.ToString();
                month1Data = LabelID + "_" + month; month1Data = (aggregateDataHash[month1Data] == null) ? "-999" : aggregateDataHash[month1Data].ToString();

                // Special Case for TB and Leprosy, this should be identified as Period 1 or some other
                // Items in the Service Element, temporarly put it as hard coded element which is not
                // good for long term maintainability
                // 2b.1.1 2b.1.3 2b.1.4

                // Merra Kokebie, July 25, 2011, We are commenting the special case for TB, we are 
                // showing it in both the monthly and quarterly for the previous report versions

                //----------//---------------------
                //if ((sno.ToUpper().Contains("2B.1.1")) || (sno.ToUpper().Contains("2B.1.3")) || (sno.ToUpper().Contains("2B.1.4")))
                //{
                //    month1Data = "-999";
                //    month2Data = "-999";
                //    month3Data = "-999";
                //}

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                toExecute.Parameters.AddWithValue("sno", sno);
                toExecute.Parameters.AddWithValue("Activity", Activity);
                toExecute.Parameters.AddWithValue("GroupID", groupID);
                toExecute.Parameters.AddWithValue("quarterData", quarterData);
                toExecute.Parameters.AddWithValue("month3Data", month3Data);
                toExecute.Parameters.AddWithValue("month2Data", month2Data);
                toExecute.Parameters.AddWithValue("month1Data", month1Data);

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

        //public ArrayList GetLabels(string sno)
        //{
        //    ArrayList labels = new ArrayList();
        //    int value1 = 0;

        //    sno = sno.ToUpper().Trim();

        //    if (sno == "2D.2.1")
        //    {                                
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 66 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.2.2")
        //    {               
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 70 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.2.3")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 74 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.3.1")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 78 + i;
        //            labels.Add(value1.ToString());
        //        }
        //        labels.Add("2501");
        //        labels.Add("2502");
        //    }
        //    else if (sno == "2D.3.2")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 82 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.3.3")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 86 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.7.3.2")
        //    {
        //        for (int i = 0; i < 8; i++)
        //        {
        //            value1 = 120 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        labels.Add("1661");
        //        labels.Add("1662");
        //    }
        //    else if (sno == "2D.7.3.2.1")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 120 + i;
        //            labels.Add(value1.ToString());
        //        }
        //        labels.Add("1661");
        //    }
        //    else if (sno == "2D.7.3.2.2")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 124 + i;
        //            labels.Add(value1.ToString());
        //        }
        //        labels.Add("1662");
        //    }
        //    else if (sno == "2D.7.3.1")
        //    {
        //        for (int i = 0; i < 10; i++)
        //        {
        //            value1 = 110 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        for (int i = 0; i < 11; i++)
        //        {
        //            value1 = 1650 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        labels.Remove("1651");

        //    }
        //    else if (sno == "2D.7.3.1.1")
        //    {
        //        for (int i = 0; i < 6; i++)
        //        {
        //            value1 = 110 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        for (int i = 0; i < 10; i++)
        //        {
        //            value1 = 1650 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        labels.Remove("1651");
        //    }
        //    else if (sno == "2D.7.3.1.2")
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            value1 = 116 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        labels.Add("1660");
        //    }
        //    else if (sno == "1.2") // Total Family Planning Acceptors
        //    {
        //        for (int i = 0; i < 2; i++)
        //        {
        //            value1 = 3 + i;
        //            labels.Add(value1.ToString());
        //        }                
        //    }
        //    else if (sno == "2D.7.1") 
        //    {
        //        for (int i = 0; i < 6; i++)
        //        {
        //            value1 = 96 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.7.2")
        //    {
        //        for (int i = 0; i < 6; i++)
        //        {
        //            value1 = 103 + i;
        //            labels.Add(value1.ToString());
        //        }
        //    }
        //    else if (sno == "2D.7.3")
        //    {
        //        for (int i = 0; i < 10; i++)
        //        {
        //            value1 = 110 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        for (int i = 0; i < 11; i++)
        //        {
        //            value1 = 1650 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        labels.Remove("1651");

        //        for (int i = 0; i < 8; i++)
        //        {
        //            value1 = 120 + i;
        //            labels.Add(value1.ToString());
        //        }

        //        labels.Add("1661");
        //        labels.Add("1662");


        //    }    
        //    return labels;
        //}

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
            // string cmdText = "exec   " + storedProcName + "  @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;
            string cmdText = "exec   " + tempStoredProcName; // +"  @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            string Id = "";

            DataSet dtSet = _helper.GetDataSet(toExecute);

            //if ((dtSet.Tables.Count) != 0)
            //{
            //}

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
            UpdateReportingTable();
        }

        public void UpdateReportingTable()
        {
            string cmdText = "";


            cmdText = "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column HPData   Decimal(18,2) " +
                         "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column HCData  Decimal(18,2) " +
                         "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column TOTAL   Decimal(18,2)  ";

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            // Drop the views

            _helper.CloseConnection();

        }
    }
}
