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

namespace eHMIS.HMIS.ReportAggregation.NewReporting
{

    public class NewWeeklyCaseListReportAggr
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
        string tempStoredProcName = "#proc_Eth_HMIS_WeeklyReport_Temp";

        string includeListProcName = "proc_Eth_HMIS_Service_IncludedList";
        string viewLabeIdTableName = "";

        string reportTableName = "";

        List<string> IncludedList = new List<string>();

        reportObject _reportObj;
        int _startMonth;
        int _endMonth;
        int _seleStartWeek;
        int _seleEndWeek;
        //int _seleQuarter;
        decimal _seleYear;

        string periodType = "";
        string hmisValueTable = "";

        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation


        public NewWeeklyCaseListReportAggr(reportObject reportObj)
        {
            _helper.ManualCloseConnection = true;

            //reportObj.ReportType
            viewLabeIdTableName = reportObj.ViewLabelIdTableName;
            reportTableName = reportObj.ReportTableName;

            GenerateReport.globalReportTableName = reportTableName;
            GenerateReport.globalCreateStoredProc = "";


            this._reportObj = reportObj;
            setStartingMonth();

            _seleStartWeek = reportObj.StartWeek;
            _seleEndWeek = reportObj.EndWeek;

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
           
            newLabelIds.Add("_TOT");         

            facilityTypes.Add("_TOT", "");


            viewLabeIdTableName = "v_EthWeeklyImmediately";

            hmisValueTable = eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable;          

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
            string cmdText =
                    " IF OBJECT_ID('tempdb.." + reportTableName + "') IS NOT NULL \n " +
                    " DROP TABLE " + reportTableName;

            // string cmdText = " drop table  " + reportTableName;

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);


            cmdText = " CREATE TABLE  " + reportTableName + "  ( " +
                      " [ID] [int] IDENTITY(1,1) NOT NULL, " +
                      " [Disease] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, " +
                      " [OPD_Case] [Decimal](18,2) NULL, " +
                      " [IPD_Case] [Decimal](18,2) NULL, " +
                      " [IPD_Death] [Decimal](18,2) NULL, " +
                      " [Format] [Decimal](18,2) NULL " +
                      "  ) ON [PRIMARY] ";

            // SqlCommand toExecute;
            GenerateReport.globalCreateTable = cmdText;

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            cmdText = "ALTER TABLE " + reportTableName + "  " +
                          "Alter Column OPD_Case   varchar(50) " +
                          "ALTER TABLE " + reportTableName + "  " +
                          "Alter Column IPD_Case  varchar(50) " +
                          "ALTER TABLE " + reportTableName + "  " +
                          "Alter Column IPD_Death   varchar(50)  ";


            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);
           
        }

        // Dynamically construct the Needed Stored Procedures based on Features that
        // Need to be applied 

        private void DynamicQueryConstruct()
        {
            if (hmisValueTable == "")
            {
                hmisValueTable = "EthEhmis_HmisValue";
            }

            string cmdText =
                  " IF OBJECT_ID('tempdb.." + tempStoredProcName + "') IS NOT NULL \n " +
                  " DROP procedure " + tempStoredProcName;

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            string storedProc =
                //"set ANSI_NULLS ON \n" +
                //"set QUOTED_IDENTIFIER ON \n  " +
                //"GO \n  \n" +

                "   /*  \n  " +
                "      Proc: 		[#proc_Eth_HMIS_WeeklyReport_Temp] \n  " +              
                "       Created: 	Feb. 21, 2011 \n  " +

                "      Description: A simple stored proc return aggregate table records \n  " +
                "   */ \n  \n" +

                "   --exec [proc_Eth_HMIS_WeeklyReport_Temp] 2003, 5, 7, 3, 7, 14 \n  " +
                "   Create  procedure  " + tempStoredProcName + " \n" +
                "   as \n  " +

                "   begin --  \n  ";


            string idQuery = "";
            // Weekly 25, Immediately 26
            string dataEleClassQuery = " and ( DataEleClass = 25 or DataEleClass = 26) ";

            int aggregationLevel = getAggregationLevel(_reportObj.LocationHMISCode);

            if (aggregationLevel == 1)
            {
                idQuery = "";
            }
            else if (aggregationLevel == 2)
            {
                idQuery = " and RegionID =  " + _reportObj.LocationHMISCode;
            }
            else if (aggregationLevel == 3)
            {
                idQuery = " and WoredaID  =  " + _reportObj.LocationHMISCode;
            }
            else if (aggregationLevel == 4)
            {
                idQuery = " and LocationID =  '" + _reportObj.LocationHMISCode + "'";
            }
            else if (aggregationLevel == 5)
            {
                idQuery = " and ZoneID =  " + _reportObj.LocationHMISCode;
            }

            // if (eHMIS.HMIS.DatabaseInterace.HmisDatabase.hmisValueTable == "EthioHMIS_HMIS_Value_Temp")
            //{

            storedProc +=
                    "	select cast(LabelID as VarChar) as LabelID, \n  " +
                    "   sum(Value) as Value from   " + hmisValueTable + "  where Week >=  " +
                    _seleStartWeek.ToString() + " and Week <= " + _seleEndWeek.ToString() +
                    " and Year = " + _seleYear.ToString() + idQuery + dataEleClassQuery + "  group by LabelID  ";
            storedProc += " \nEND ";

            toExecute = new SqlCommand(storedProc);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

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

                string Disease, OPD_Case, IPD_Case, IPD_Death, Format;

                cmdText = "SELECT * from  " + viewLabeIdTableName + "  order by format ";
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    Disease = row["Disease"].ToString();
                    OPD_Case = row["OPD_Case"].ToString();
                    IPD_Case = row["IPD_Case"].ToString();
                    IPD_Death = row["IPD_Death"].ToString();
                    Format = row["Format"].ToString();
                    // Call the insert statement

                    InsertAggregateWeeklyData(Disease, OPD_Case, IPD_Case, IPD_Death, Format);
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

        // PHEM Report
        private void InsertAggregateWeeklyData(string Disease, string OPD_Case, string IPD_Case, string IPD_Death, string Format)
        {

            SqlCommand toExecute;

            string cmdText = " insert into " + reportTableName + " values (@Disease, @OPD_Case, @IPD_Case, @IPD_Death, @Format)";


            //string OPD_Case, IPD_Case, IPD_Death, Format;            

            OPD_Case = (aggregateDataHash[OPD_Case] == null) ? "-999" : aggregateDataHash[OPD_Case].ToString();
            IPD_Case = (aggregateDataHash[IPD_Case] == null) ? "-999" : aggregateDataHash[IPD_Case].ToString();
            IPD_Death = (aggregateDataHash[IPD_Death] == null) ? "-999" : aggregateDataHash[IPD_Death].ToString();
            //Format = (aggregateDataHash[Format] == null) ? "-999" : aggregateDataHash[Format].ToString();                

            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            toExecute.Parameters.AddWithValue("Disease", Disease);
            toExecute.Parameters.AddWithValue("OPD_Case", OPD_Case);
            toExecute.Parameters.AddWithValue("IPD_Case", IPD_Case);
            toExecute.Parameters.AddWithValue("IPD_Death", IPD_Death);
            toExecute.Parameters.AddWithValue("Format", Format);
                      
            _helper.Execute(toExecute);


        }

        public void UpdateHashTable()
        {
            // string cmdText = "exec   " + storedProcName + "  @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;
            string cmdText = "exec   " + tempStoredProcName; // +"  @Year, @StartMonth, @EndMonth, @Type, @RepKind, @Id"; // +storedProcName;

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            string Id = "";

            _helper.Execute(toExecute);

            DataSet dtSet = _helper.GetDataSet(toExecute);


            DataTable dt = dtSet.Tables[0];
         
            aggregateList.Clear();  

            foreach (DataRow row in dt.Rows)
            {
                string LabelID = row["LabelID"].ToString();
                //decimal HmisValue = Convert.ToDecimal(row["HmisValue"].ToString());
                string HmisValue = row["Value"].ToString();
                aggregateDataHash.Add(LabelID, HmisValue);

            }

            startReportTableGeneration(false);
            UpdateReportingTable();
        }

        public void UpdateReportingTable()
        {
            string cmdText = "";

            cmdText = "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column OPD_Case   Decimal(18,2) " +
                         "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column IPD_Case  Decimal(18,2) " +
                         "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column IPD_Death   Decimal(18,2)  " +
                         "ALTER TABLE " + reportTableName + "  " +
                         "Alter Column Format   Decimal(18,2)  ";

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);

            if (!hmisValueTable.Contains("EthioHMIS_HMIS_Value_Temp"))
            {
                _helper.CloseConnection();
            }

        }
    }
}
