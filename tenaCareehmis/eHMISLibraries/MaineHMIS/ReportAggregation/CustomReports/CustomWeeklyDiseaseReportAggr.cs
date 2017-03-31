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


namespace eHMIS.HMIS.ReportAggregation.CustomReports
{

    public class CustomWeeklyDiseaseReportAggr : ICustomReport
    {
        DBConnHelper _helper = new DBConnHelper();

        Hashtable aggregateDataHash = new Hashtable();
        Hashtable ethMonth = new Hashtable();

        List<string> locationsToView = new List<string>();
        Hashtable locationIdToName = new Hashtable();
        DataTable reportWeeklyDataTable = new DataTable();
        DataTable reportImmediatelyDataTable = new DataTable();
        DataTable reportCaseBasedDataTable = new DataTable();       
        Boolean singleFacility = false;
        private static int caseNo = 0;
        private static string CaseName = "";
        private volatile bool _shouldStop;
        string viewLabeIdTableName = "";
        
        int _startWeek;
        int _endWeek;

        int _startMonth;
        int _endMonth;
        int _startYear;
        int _endYear;
        int _quarterStart;
        int _quarterEnd;
        int _repKind;
        int _repPeriod;
        DateTime _startDate;
        DateTime _endDate;
       
        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation


        public CustomWeeklyDiseaseReportAggr(List<string> locations, int startWeek, int endWeek, int yearStart, int repKind)
        {
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


            _startYear = yearStart;
            _startWeek = startWeek;
            _endWeek = endWeek;
           
            _repKind = repKind;
           

           locationsToView = locations;

            if (locationsToView.Count == 1)
                singleFacility = true; // Only for a single facility, thus show detail month to month data
            // or detail Quarter to Quarter data, or Week to Week Data including aggregate


            if (_repKind == 0)
            {
                viewLabeIdTableName = "EthEhmis_WeeklyView";

            //    reportWeeklyDataTable.Columns.Add("Id", typeof(string));
            //    reportWeeklyDataTable.Columns.Add("Disease", typeof(string));
            //    reportWeeklyDataTable.Columns.Add("OPD_Case", typeof(string));
            //    reportWeeklyDataTable.Columns.Add("IPD_Case", typeof(string));
            //    reportWeeklyDataTable.Columns.Add("IPD_Death", typeof(string));    


                
            }
            else if (_repKind == 1)
            {
                viewLabeIdTableName = "EthEhmis_ImmediatelyView";

                //reportImmediatelyDataTable.Columns.Add("Id", typeof(string));
                //reportImmediatelyDataTable.Columns.Add("Disease", typeof(string));
                //reportImmediatelyDataTable.Columns.Add("Disease_Case", typeof(string));
                //reportImmediatelyDataTable.Columns.Add("Disease_Death", typeof(string));               
            }
            reportWeeklyDataTable.Columns.Add("Region Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Zone Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Woreda Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Facility Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Year", typeof(string));
            reportWeeklyDataTable.Columns.Add("WHO Week", typeof(string));
            reportWeeklyDataTable.Columns.Add("Month", typeof(string));
                             
        }

        public CustomWeeklyDiseaseReportAggr(List<string> locations, DateTime startDate, DateTime endDate, int repKind)
        {           
            _startDate = startDate;
            _endDate = endDate;

            _repKind = repKind;


            locationsToView = locations;

            if (locationsToView.Count == 1)
                singleFacility = true; // Only for a single facility, thus show detail month to month data
            // or detail Quarter to Quarter data, or Week to Week Data including aggregate

             if (_repKind == 2)
            {
                viewLabeIdTableName = "vw_EthEhmis_CaseBasedView";

                reportCaseBasedDataTable.Columns.Add("CaseIdNumber", typeof(string));
                reportCaseBasedDataTable.Columns.Add("CaseName", typeof(string));
                reportCaseBasedDataTable.Columns.Add("FirstName", typeof(string));
                reportCaseBasedDataTable.Columns.Add("FatherName", typeof(string));
                reportCaseBasedDataTable.Columns.Add("GrandFatherName", typeof(string));               
                reportCaseBasedDataTable.Columns.Add("Age", typeof(string));
                reportCaseBasedDataTable.Columns.Add("Gender", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateSent", typeof(string));
                reportCaseBasedDataTable.Columns.Add("Region", typeof(string));
                reportCaseBasedDataTable.Columns.Add("Zone", typeof(string));
                reportCaseBasedDataTable.Columns.Add("Woreda", typeof(string));
                reportCaseBasedDataTable.Columns.Add("Kebele", typeof(string));
                reportCaseBasedDataTable.Columns.Add("HouseNo", typeof(string));
                reportCaseBasedDataTable.Columns.Add("GISGrid", typeof(string));
                reportCaseBasedDataTable.Columns.Add("SymptomStartedGrid", typeof(string));
                reportCaseBasedDataTable.Columns.Add("Note", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateRecevied", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateOfOnSet", typeof(string));
                reportCaseBasedDataTable.Columns.Add("HMISCode", typeof(string));
                reportCaseBasedDataTable.Columns.Add("FacilityName", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateOfBirth", typeof(string));
                reportCaseBasedDataTable.Columns.Add("PatientId", typeof(string));
                reportCaseBasedDataTable.Columns.Add("InOutPatient", typeof(string));
                reportCaseBasedDataTable.Columns.Add("OutCome", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateSentGC", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateReceviedGC", typeof(string));
                reportCaseBasedDataTable.Columns.Add("DateOfOnSetGC", typeof(string));
                reportCaseBasedDataTable.Columns.Add("CaseId", typeof(string));
                reportCaseBasedDataTable.Columns.Add("CaseN", typeof(string));
            }

        }

        private string getFacilityName(string locationID)
        {
            // Given the location ID returns the Facility Name
            string cmdText = "select facilityname from facility where hmiscode = @locationID";
            string facilityName = "";

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            toExecute.Parameters.AddWithValue("locationID", locationID);

            DataSet ds = _helper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
            {
                facilityName = ds.Tables[0].Rows[0]["facilityName"].ToString();
            }

            return facilityName;
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


        private void DynamicQueryConstruct()
        {

            string idQuery = "";
            string dataEleClassQuery = "";
            aggregateDataHash.Clear();

            if (_repKind == 0)
            {
                dataEleClassQuery = " and DataEleClass = 25 "; // Weekly
            }
            else if (_repKind == 1)
            {
                dataEleClassQuery = " and DataEleClass = 26 "; // Immediately
            }

            if (_repKind == 0 || _repKind == 1)
            {
                foreach (string locationID in locationsToView)
                {
                    string id = locationID;

                    //if (id == "10") id = "14";
                    int aggregationLevel = getAggregationLevel(id);

                    if (aggregationLevel == 1)
                    {
                        idQuery = "";
                    }
                    else if (aggregationLevel == 2)
                    {
                        idQuery = " and RegionID = @newIdentification ";
                    }
                    else if (aggregationLevel == 3)
                    {
                        idQuery = " and WoredaID = @newIdentification ";
                    }
                    else if (aggregationLevel == 4)
                    {
                        idQuery = " and LocationID = @newIdentification ";
                    }
                    else if (aggregationLevel == 5)
                    {
                        idQuery = " and ZoneID = @newIdentification ";
                    }

                    // Morbidity for both IPD and OPD

                    string cmdText = "";

                    string monthQueryGroup = "";

                    monthQueryGroup = "	where  Week  >=@StartWeek and Week <= @EndWeek  and Year = @startYear and level = 0 ";


                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     "   sum(Value) as Value from EthEhmis_HmisValue  " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthQueryGroup +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startWeek, _endWeek, _startYear);

                    if (singleFacility == true)
                    {
                        monthQueryGroup = "	where  Week  = @StartWeek  and Year = @startYear   ";

                        for (int i = _startWeek; i <= _endWeek; i++)
                        {
                            int week = i;
                            cmdText =
                            "	select cast(LabelID as VarChar) + '_" + week + "_" + locationID + "' as LabelID, " +
                            "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                //"	where  Month  = @StartMonth  and Year = @startYear " +
                            monthQueryGroup +
                               dataEleClassQuery + idQuery +
                            "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, week, _endWeek, _startYear);
                        }
                    }
                }
            }
            else if (_repKind == 2)
            {
                foreach (string locationID in locationsToView)
                {
                    string id = locationID;

                    //if (id == "10") id = "14";
                    int aggregationLevel = getAggregationLevel(id);

                    if (aggregationLevel == 1)
                    {
                        idQuery = "";
                    }
                    else if (aggregationLevel == 2)
                    {
                        idQuery = " and RegionID = @newIdentification ";
                    }
                    else if (aggregationLevel == 3)
                    {
                        idQuery = " and WoredaID = @newIdentification ";
                    }
                    else if (aggregationLevel == 4)
                    {
                        idQuery = " and HMISCode = @newIdentification ";
                    }
                    else if (aggregationLevel == 5)
                    {
                        idQuery = " and ZoneID = @newIdentification ";
                    }

                    // Morbidity for both IPD and OPD

                    string cmdText = "Select * from vw_EthEhmis_CaseBasedView";

                    string monthQueryGroup = "";

                    monthQueryGroup = "	where  Week  >=@StartWeek and Week <= @EndWeek  and Year = @startYear   ";
                    //select FirstName, FatherName, GrandFatherName, DateOfBirth, Age, 
                    //Gender, Region, Zone, Woreda, Kebele, HouseNo, GISGrid, SymptomStartedGrid, Note,
                    //CaseIDNumber, CaseName, DateRecevied, DateSent, DateOfOnSet 

                    cmdText = "Select * from vw_EthEhmis_CaseBasedView " + 
                        monthQueryGroup +
                         idQuery ;
                    addToHashTable(cmdText, id, _startWeek, _endWeek, _startYear);

                    if (singleFacility == true)
                    {
                        monthQueryGroup = "	where  Week  = @StartWeek  and Year = @startYear   ";

                        for (int i = _startWeek; i <= _endWeek; i++)
                        {
                            int week = i;
                            cmdText = "Select * from vw_EthEhmis_CaseBasedView " +
                            monthQueryGroup +
                               idQuery;
                            addToHashTable(cmdText, id, week, _endWeek, _startYear);
                        }
                    }
                }
            }
        }

        private void addToHashTable(string cmdText, string id, int startMonth, int endMonth, int startYear)
        {
            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("newIdentification", id);
            toExecute.Parameters.AddWithValue("StartMonth", startMonth);
            toExecute.Parameters.AddWithValue("EndMonth", endMonth);
            toExecute.Parameters.AddWithValue("startYear", startYear);
            toExecute.Parameters.AddWithValue("endYear", _endYear);
            toExecute.Parameters.AddWithValue("StartWeek", startMonth);
            toExecute.Parameters.AddWithValue("EndWeek", _endWeek);

            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            if (_repKind != 2)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string LabelID = row["labelID"].ToString();
                    int value = Convert.ToInt32(row["value"]);
                    aggregateDataHash.Add(LabelID, value);
                }
            }
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        public DataTable CreateReport()
        {

          

            foreach (string locationID in locationsToView)
            {
                string id = locationID;

                // if (id == "10") id = "14";

                // Get the Facility Name for the LocationID
                string facilityName = getFacilityName(id);
                locationIdToName.Add(locationID, facilityName);
            }


            if (_repKind == 0 || _repKind == 1)
            {
                _repKind = 0;
                DynamicQueryConstruct();
                viewLabeIdTableName = "EthEhmis_WeeklyView";
                string cmdText = "SELECT Id, Disease, OPD_Case, IPD_Case, IPD_Death from  " + viewLabeIdTableName;
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                string id, disease, opd_case, ipd_case, ipd_death;

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    id = row["Id"].ToString();
                    disease = row["disease"].ToString();
                    opd_case = row["OPD_CASE"].ToString();
                    ipd_case = row["IPD_CASE"].ToString();
                    ipd_death = row["IPD_DEATH"].ToString();                    

                    // Call the insert statement

                    InsertAggregateData(id, disease, opd_case, ipd_case, ipd_death);
                }
               
                //Immed
                _repKind = 1;
                DynamicQueryConstruct();
                viewLabeIdTableName = "EthEhmis_ImmediatelyView";
                cmdText = "SELECT Id, Disease, DiseaseCase, DiseaseDeath from  " + viewLabeIdTableName;
              //  SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                string disease_case, disease_death;

                //DataTable
                    dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    id = row["ID"].ToString();
                    disease = row["disease"].ToString();
                    disease_case = row["DISEASECASE"].ToString();
                    disease_death = row["DISEASEDEATH"].ToString();

                    // Call the insert statement

                    InsertAggregateData(id, disease, disease_case, disease_death,"");
                }




                Dictionary<string, string> colMap = new Dictionary<string, string>();
                //colMap.Add("Id", "Id");
                //colMap.Add("Disease", "Disease");
                //colMap.Add("OPD_Case", "OPD_Case");
                //colMap.Add("IPD_Case", "IPD_Case");
                //colMap.Add("IPD_Death", "IPD_Death");
               

                //StringBuilder expr = new StringBuilder();
                //string[] fields = { "OPD_Case", "IPD_Case", "IPD_Death"}; 
                //for (int i = 0; i < fields.Length; i++)
                //{
                //    string nextField = fields[i];
                //    string fname = "[" + colMap[nextField] + "]";
                //    if (expr.Length > 0)
                //        expr.Append("+");
                //    expr.Append("IIF(LEN(" + fname + ")>0, Convert(" + fname + ", 'System.Decimal'), 0)");
                //}

                //DataColumn total = new DataColumn("Total", typeof(string), expr.ToString());
                //reportWeeklyDataTable.Columns.Add(total);

                ////DataColumn Chart = new DataColumn("Chart", typeof(string));
                ////reportWeeklyDataTable.Columns.Add(Chart);

                //if (singleFacility == false)
                //{
                //    calculateFacilityTototal(reportWeeklyDataTable, expr);
                //}

                //for (int i = 0; i < reportWeeklyDataTable.Rows.Count; i++)
                //{
                //    if (reportWeeklyDataTable.Rows[i][1].ToString().Contains("Total:"))
                //    {
                //        reportWeeklyDataTable.Rows[i].Delete();
                //    }
                //    //if (reportWeeklyDataTable.Rows[i][0].ToString() != "")
                //    //{
                //    //    reportWeeklyDataTable.Rows[i][6] = "Click Here To See in Chart";
                //    //}
                //}
               
                return reportWeeklyDataTable;
            }
            else if (_repKind == 1)
            {
                string cmdText = "SELECT Id, Disease, DiseaseCase, DiseaseDeath from  " + viewLabeIdTableName;
                SqlCommand toExecute;
                toExecute = new SqlCommand(cmdText);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                string id, disease, disease_case, disease_death;

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    id = row["ID"].ToString();
                    disease = row["disease"].ToString();
                    disease_case = row["DISEASECASE"].ToString();
                    disease_death = row["DISEASEDEATH"].ToString();

                    // Call the insert statement

                    InsertAggregateData(id, disease, disease_case, disease_death);
                }

                Dictionary<string, string> colMap = new Dictionary<string, string>();
                //colMap.Add("Id", "Id");
                //colMap.Add("Disease", "Disease");
                colMap.Add("Disease_Case", "Disease_Case");
                colMap.Add("Disease_Death", "Disease_Death");


                StringBuilder expr = new StringBuilder();
                string[] fields = { "Disease_Case", "Disease_Death" };
                for (int i = 0; i < fields.Length; i++)
                {
                    string nextField = fields[i];
                    string fname = "[" + colMap[nextField] + "]";
                    if (expr.Length > 0)
                        expr.Append("+");
                    expr.Append("IIF(LEN(" + fname + ")>0, Convert(" + fname + ", 'System.Decimal'), 0)");
                }

                DataColumn total = new DataColumn("Total", typeof(string), expr.ToString());
                reportImmediatelyDataTable.Columns.Add(total);

                //DataColumn Chart = new DataColumn("Chart", typeof(string));
                //reportImmediatelyDataTable.Columns.Add(Chart);
                if (singleFacility == false)
                {
                    calculateFacilityTototal(reportImmediatelyDataTable, expr);
                }

                for (int i = 0; i < reportImmediatelyDataTable.Rows.Count; i++)
                {
                    if (reportImmediatelyDataTable.Rows[i][1].ToString().Contains("Total:"))
                    {
                        reportImmediatelyDataTable.Rows[i].Delete();
                    }
                    //if (reportImmediatelyDataTable.Rows[i][0].ToString() != "")
                    //{
                    //    reportImmediatelyDataTable.Rows[i][5] = "Click Here To See in Chart";
                    //}
                }


                return reportImmediatelyDataTable;
            }

            else if (_repKind == 2)
            {
                string locationId = "";
                for (int i=0;i< locationsToView.Count; i++)
                {
                    if (locationsToView.Count == 1)
                    {
                        locationId = "'"+locationsToView[i].ToString()+"'"; 
                    }
                    else
                    {
                        if (i == 0)
                        {
                            locationId = "'"+locationsToView[i].ToString()+"'";
                        }
                        else
                        {
                            locationId = locationId + " or HMISCode = '" + locationsToView[i].ToString() + "'";
                        }
                    }
                }
                
                string cmdText = "Select FirstName, FatherName, GrandFatherName, DateOfBirth, Age, " +
                                 "Gender, Region, Zone, Woreda, Kebele, HouseNo, GISGrid, SymptomStartedGrid, Note, " +
                                 "CaseIDNumber, CaseName, DateRecevied, DateSent, DateOfOnSet, HMISCode, FacilityName, "+
                                 "PatientId, InOutPatient, OutCome from  " + viewLabeIdTableName + " where ((DateSent between convert(varchar, '" + _startDate + "' , 102) and  convert(varchar,  '" + _endDate + "', 102)) and (HMISCode = " + locationId + "))";
                SqlCommand toExecute;             

                toExecute = new SqlCommand(cmdText);              
               

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                InsertAggregateData(dt);

                Dictionary<string, string> colMap = new Dictionary<string, string>();
                
                colMap.Add("FirstName", "FirstName");
                colMap.Add("FatherName", "FatherName");
                colMap.Add("GrandFatherName", "GrandFatherName");                
                colMap.Add("Age", "Age");
                colMap.Add("Gender", "Gender");
                colMap.Add("Region", "Region");
                colMap.Add("Zone", "Zone");
                colMap.Add("Woreda", "Woreda");
                colMap.Add("Kebele", "Kebele");
                colMap.Add("HouseNo", "HouseNo");
                colMap.Add("GISGrid", "GISGrid");
                colMap.Add("SymptomStartedGrid", "SymptomStartedGrid");
                colMap.Add("Note", "Note");
                colMap.Add("CaseIDNumber", "CaseIDNumber");
                colMap.Add("CaseName", "CaseName");
                colMap.Add("DateRecevied", "DateRecevied");
                colMap.Add("DateSent", "DateSent");
                colMap.Add("DateOfOnSet", "DateOfOnSet");
                colMap.Add("HMISCode", "HMISCode");
                colMap.Add("FacilityName", "FacilityName");
                colMap.Add("DateOfBirth", "DateOfBirth");
                colMap.Add("PatientId", "PatientId");
                colMap.Add("InOutPatient", "InOutPatient");
                colMap.Add("OutCome", "OutCome");
                colMap.Add("DateSentGC", "DateSent");
                colMap.Add("DateReceviedGC", "DateRecevied");
                colMap.Add("DateOfOnSetGC", "DateOfOnSet");
                colMap.Add("CaseId", "CaseIDNumber");
                colMap.Add("CaseN", "CaseName");

                StringBuilder expr = new StringBuilder();
                string[] fields = { "FirstName", "FatherName", "GrandFatherName", "Age", "Gender", "Region", "Zone", "Woreda", "Kebele", "HouseNo", "GISGrid", "SymptomStartedGrid", "Note", "CaseIDNumber", "CaseName", "DateRecevied", "DateSent", "DateOfOnSet", "HMISCode", "FacilityName", "DateOfBirth", "PatientId", "InOutPatient", "OutCome", "DateSentGC", "DateReceviedGC", "DateOfOnSetGC", "CaseId", "CaseN"};
                for (int i = 0; i < fields.Length; i++)
                {
                    string nextField = fields[i];
                    string fname = "[" + colMap[nextField] + "]";
                    if (expr.Length > 0)
                        expr.Append("+");
                    expr.Append("IIF(LEN(" + fname + ")>0, Convert(" + fname + ", 'System.Decimal'), 0)");
                }

                return reportCaseBasedDataTable;
               
            }
            return null;

        }

        private void calculateFacilityTototal(DataTable dt, StringBuilder sb)
        {
            int noFacilitiesSelected = locationsToView.Count;

            int finalRowPosition = 0;

            int id = (dt.Rows.Count / (noFacilitiesSelected + 1));

            if (_repKind == 0) // OPD # columns for OPD and IPD is different since IPD has mortality
            {
                for (int i = 0; i < id; i++)
                {

                    int rowPosition = finalRowPosition + 1;

                    int FacilityTotal1 = 0;
                    int FacilityTotal2 = 0;
                    int FacilityTotal3 = 0;

                    bool flag1 = true;
                    bool flag2 = true;
                    bool flag3 = true;

                    for (int j = rowPosition; j <= (rowPosition + noFacilitiesSelected - 1); j++)
                    {
                        if (DBNull.Value != dt.Rows[j][2] && dt.Rows[j][2].ToString() != "")
                        {
                            FacilityTotal1 = FacilityTotal1 + Convert.ToInt32(dt.Rows[j][2]);
                            flag1 = true;
                        }
                        if (DBNull.Value != dt.Rows[j][3] && dt.Rows[j][3].ToString() != "")
                        {
                            FacilityTotal2 = FacilityTotal2 + Convert.ToInt32(dt.Rows[j][3]);
                            flag2 = true;
                        }
                        if (DBNull.Value != dt.Rows[j][4] && dt.Rows[j][3].ToString() != "")
                        {
                            FacilityTotal3 = FacilityTotal3 + Convert.ToInt32(dt.Rows[j][4]);
                            flag3 = true;
                        }

                        finalRowPosition = j;

                    }

                    finalRowPosition = finalRowPosition + 1;
                    //finalRowPosition = j;

                    //dt.Rows[i]["SNO"] = FacilityTotal.ToString();

                    if (flag1 == true)
                    {
                        dt.Rows[rowPosition - 1][2] = FacilityTotal1.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][2] = "";
                    }

                    if (flag2 == true)
                    {
                        dt.Rows[rowPosition - 1][3] = FacilityTotal2.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][3] = "";
                    }

                    if (flag3 == true)
                    {
                        dt.Rows[rowPosition - 1][4] = FacilityTotal3.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][4] = "";
                    }
                }
            }
            else if (_repKind == 1)
            {
                for (int i = 0; i < id; i++)
                {

                    int rowPosition = finalRowPosition + 1;

                    int FacilityTotal1 = 0;
                    int FacilityTotal2 = 0;

                    bool flag1 = false;
                    bool flag2 = false;
                
                    for (int j = rowPosition; j <= (rowPosition + noFacilitiesSelected - 1); j++)
                    {
                        if (DBNull.Value != dt.Rows[j][2] && dt.Rows[j][2].ToString() != "")
                        {
                            FacilityTotal1 = FacilityTotal1 + Convert.ToInt32(dt.Rows[j][2]);
                            flag1 = true;
                        }
                        if (DBNull.Value != dt.Rows[j][3] && dt.Rows[j][3].ToString() != "")
                        {
                            FacilityTotal2 = FacilityTotal2 + Convert.ToInt32(dt.Rows[j][3]);
                            flag2 = true;
                        }

                        finalRowPosition = j;

                    }

                    finalRowPosition = finalRowPosition + 1;
                    //finalRowPosition = j;

                    //dt.Rows[i]["SNO"] = FacilityTotal.ToString();

                    if (flag1 == true)
                    {
                        dt.Rows[rowPosition - 1][2] = FacilityTotal1.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][2] = "";
                    }

                    if (flag2 == true)
                    {
                        dt.Rows[rowPosition - 1][3] = FacilityTotal2.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][3] = "";
                    }
                 }
            }

            else if (_repKind == 2)
            {
                for (int i = 0; i < id; i++)
                {

                    int rowPosition = finalRowPosition + 1;

                    int FacilityTotal1 = 0;
                    int FacilityTotal2 = 0;

                    bool flag1 = false;
                    bool flag2 = false;

                    for (int j = rowPosition; j <= (rowPosition + noFacilitiesSelected - 1); j++)
                    {
                        if (DBNull.Value != dt.Rows[j][2] && dt.Rows[j][2].ToString() != "")
                        {
                            FacilityTotal1 = FacilityTotal1 + Convert.ToInt32(dt.Rows[j][2]);
                            flag1 = true;
                        }
                        if (DBNull.Value != dt.Rows[j][3] && dt.Rows[j][3].ToString() != "")
                        {
                            FacilityTotal2 = FacilityTotal2 + Convert.ToInt32(dt.Rows[j][3]);
                            flag2 = true;
                        }

                        finalRowPosition = j;

                    }

                    finalRowPosition = finalRowPosition + 1;
                    //finalRowPosition = j;

                    //dt.Rows[i]["SNO"] = FacilityTotal.ToString();

                    if (flag1 == true)
                    {
                        dt.Rows[rowPosition - 1][2] = FacilityTotal1.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][2] = "";
                    }

                    if (flag2 == true)
                    {
                        dt.Rows[rowPosition - 1][3] = FacilityTotal2.ToString();
                    }
                    else
                    {
                        dt.Rows[rowPosition - 1][3] = "";
                    }
                }
            }
        }

        // OPD Disease Report (Main one for higher Admin Levels)
        private void InsertAggregateData(string id, string disease, string opd_case, string ipd_case,
            string ipd_death)
        {
            if (opd_case == "")
            {
                opd_case = "";
                ipd_case = "";
                ipd_death = "";

                reportWeeklyDataTable.Rows.Add(id, disease, opd_case, ipd_case, ipd_death);
            }
            else
            {

                string opd_case_tot = "";
                string ipd_case_tot = "";
                string ipd_death_tot = "";

                //if (singleFacility == false)
                //{
                //    reportWeeklyDataTable.Rows.Add(id, disease, opd_case_tot, ipd_case_tot, ipd_death_tot);
                //}
                if (_repKind == 0)
                { 
                
                
                    reportWeeklyDataTable.Columns.Add(disease + " OPD Case");
                    reportWeeklyDataTable.Columns.Add(disease + " IPD Case");
                    reportWeeklyDataTable.Columns.Add(disease + " Death Case");
                }
                else if (_repKind == 1)
                {
                    reportWeeklyDataTable.Columns.Add(disease + " Case");
                    reportWeeklyDataTable.Columns.Add(disease + " Death");
                }
                // 2). The Second Entry Loops through all the facilities

                foreach (string locationId in locationsToView)
                {


                    //if (singleFacility == true)
                    //{
                    //    disease = "Total: ";
                    //}
                    //if (singleFacility == false)
                    //{
                    //    //disease = "   " + locationIdToName[locationId];
                    //    id = "";
                    //}

                    string opd_case_pub_tot = opd_case + "_" + locationId; opd_case_pub_tot = (aggregateDataHash[opd_case_pub_tot] == null) ? "" : aggregateDataHash[opd_case_pub_tot].ToString();
                    string ipd_case_pub_tot = ipd_case + "_" + locationId; ipd_case_pub_tot = (aggregateDataHash[ipd_case_pub_tot] == null) ? "" : aggregateDataHash[ipd_case_pub_tot].ToString();
                    string ipd_death_pub_tot = ipd_death + "_" + locationId; ipd_death_pub_tot = (aggregateDataHash[ipd_death_pub_tot] == null) ? "" : aggregateDataHash[ipd_death_pub_tot].ToString();

                    // the first for the total
                    bool foundfacility=false;
                    int indexfound=-999;

                     string hmiscodeF = locationIdToName[locationId].ToString();
                     int regionId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(locationId);
                        string regionName =
                            Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(regionId.ToString()).Replace("Regional Health Bureau","");
                        int zoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(locationId);
                        string zonename = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(zoneId.ToString()).Replace("Zonal Health Department", ""); ;
                        int woredaId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getWoredaId(locationId);
                        string woredaname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(woredaId.ToString()).Replace("Woreda Health Office","");
                        if (regionName == "Federal Ministry of Health")
                            regionName = "FMOH";

                    if (reportWeeklyDataTable.Rows.Count == 0)
                    {
                       
                        reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, locationIdToName[locationId], _startYear.ToString(), _startWeek.ToString(), _startMonth.ToString());
                        foundfacility = true;
                        indexfound = 0;
                    }
                    else
                    {
                        for (int i = 0; i < reportWeeklyDataTable.Rows.Count; i++)
                        {
                            if (reportWeeklyDataTable.Rows[i][3].ToString() == locationIdToName[locationId])
                            {
                                foundfacility = true;
                                indexfound = i;
                                break;
                            }

                        }
                    }
                    if (!foundfacility)
                    {
                        reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, locationIdToName[locationId], _startYear.ToString(), _startWeek.ToString(), _startMonth.ToString());
                        indexfound = reportWeeklyDataTable.Rows.Count-1;
                    }
                    if (indexfound!=-999)
                    {

                       
                        if (_repKind == 0)
                        {
                            string opd = disease + " OPD Case", ipd = disease + " IPD Case", ipddeath = disease + " Death Case";
                            reportWeeklyDataTable.Rows[indexfound][opd] = opd_case_pub_tot;
                            reportWeeklyDataTable.Rows[indexfound][ipd] = ipd_case_pub_tot;
                            reportWeeklyDataTable.Rows[indexfound][ipddeath] = ipd_death_pub_tot;
                        }
                        else if (_repKind == 1)
                        {


                            string disease_case_pub_tot = opd_case + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                            string disease_death_pub_tot = ipd_case + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();

                            string caseIm = disease + " Case", deathIm = disease + " Death";

                            reportWeeklyDataTable.Rows[indexfound][caseIm] = disease_case_pub_tot;
                            reportWeeklyDataTable.Rows[indexfound][deathIm] = disease_death_pub_tot;
                        }
                    }
                    // Then for each month, only if single facility and Monthly report

                    id = "";
                    //if (singleFacility == true)
                    //{
                    //    for (int i = _startWeek; i <= _endWeek; i++)
                    //    {
                    //        disease = "     Week: " + i.ToString();
                    //        id = "";

                    //        opd_case_pub_tot = opd_case + "_" + i + "_" + locationId; opd_case_pub_tot = (aggregateDataHash[opd_case_pub_tot] == null) ? "" : aggregateDataHash[opd_case_pub_tot].ToString();
                    //        ipd_case_pub_tot = ipd_case + "_" + i + "_" + locationId; ipd_case_pub_tot = (aggregateDataHash[ipd_case_pub_tot] == null) ? "" : aggregateDataHash[ipd_case_pub_tot].ToString();
                    //        ipd_death_pub_tot = ipd_death + "_" + i + "_" + locationId; ipd_death_pub_tot = (aggregateDataHash[ipd_death_pub_tot] == null) ? "" : aggregateDataHash[ipd_death_pub_tot].ToString();

                    //        reportWeeklyDataTable.Rows.Add(id, disease, opd_case_pub_tot, ipd_case_pub_tot, ipd_death_pub_tot);
                    //    }
                    //}
                }

                // reportOpdDataTable.Rows.Add("---------", "---------", "---------", "---------", "---------", "---------", "---------", "---------");

            }
        }
        private void InsertAggregateData(string id, string disease, string opd_case, string ipd_case,
            string ipd_death,int OldCode)
        {
            if (opd_case == "")
            {
                opd_case = "";
                ipd_case = "";        
                ipd_death = "";
                      
                reportWeeklyDataTable.Rows.Add(id, disease, opd_case, ipd_case, ipd_death);
            }
            else
            {
               
                string opd_case_tot = "";
                string ipd_case_tot = "";
                string ipd_death_tot = "";

                if (singleFacility == false)
                {
                    reportWeeklyDataTable.Rows.Add(id, disease, opd_case_tot, ipd_case_tot, ipd_death_tot);
                }

                // 2). The Second Entry Loops through all the facilities

                foreach (string locationId in locationsToView)
                {
                    

                    //if (singleFacility == true)
                    //{
                    //    disease = "Total: ";
                    //}
                    if (singleFacility == false)
                    {
                        disease = "   " + locationIdToName[locationId];
                        id = "";
                    }

                    string opd_case_pub_tot = opd_case + "_" + locationId; opd_case_pub_tot = (aggregateDataHash[opd_case_pub_tot] == null) ? "" : aggregateDataHash[opd_case_pub_tot].ToString();
                    string ipd_case_pub_tot = ipd_case + "_" + locationId; ipd_case_pub_tot = (aggregateDataHash[ipd_case_pub_tot] == null) ? "" : aggregateDataHash[ipd_case_pub_tot].ToString();
                    string ipd_death_pub_tot = ipd_death + "_" + locationId; ipd_death_pub_tot = (aggregateDataHash[ipd_death_pub_tot] == null) ? "" : aggregateDataHash[ipd_death_pub_tot].ToString();
                   
                    // the first for the total
                     reportWeeklyDataTable.Rows.Add(id, disease, opd_case_pub_tot, ipd_case_pub_tot, ipd_death_pub_tot); 
                   
                    // Then for each month, only if single facility and Monthly report

                     id = "";
                    if (singleFacility == true)
                    {
                        for (int i = _startWeek; i <= _endWeek; i++)
                        {
                            disease = "     Week: " + i.ToString();
                            id = "";

                            opd_case_pub_tot = opd_case + "_" + i + "_" + locationId; opd_case_pub_tot = (aggregateDataHash[opd_case_pub_tot] == null) ? "" : aggregateDataHash[opd_case_pub_tot].ToString();
                            ipd_case_pub_tot = ipd_case + "_" + i + "_" + locationId; ipd_case_pub_tot = (aggregateDataHash[ipd_case_pub_tot] == null) ? "" : aggregateDataHash[ipd_case_pub_tot].ToString();
                            ipd_death_pub_tot = ipd_death + "_" + i + "_" + locationId; ipd_death_pub_tot = (aggregateDataHash[ipd_death_pub_tot] == null) ? "" : aggregateDataHash[ipd_death_pub_tot].ToString();
                           
                            reportWeeklyDataTable.Rows.Add(id, disease, opd_case_pub_tot, ipd_case_pub_tot, ipd_death_pub_tot); 
                        }
                    }                                        
                }

                // reportOpdDataTable.Rows.Add("---------", "---------", "---------", "---------", "---------", "---------", "---------", "---------");

            }
        }

        // For IPD Disease Report InsertAggregateData
        private void InsertAggregateData(string id, string disease, string disease_case, string disease_death)
        {
            if (disease_case == "")
            {
                disease_death = "";

                reportImmediatelyDataTable.Rows.Add(id, disease, disease_case, disease_death);
            }
            else
            {
                string disease_case_tot = "";
                string disease_death_tot = "";


                if (singleFacility == false)
                {
                    reportImmediatelyDataTable.Rows.Add(id, disease, disease_case_tot, disease_death_tot);
                }

                // 2). The Second Entry Loops through all the Health Facilities

                foreach (string locationId in locationsToView)
                {


                    //if (singleFacility == true)
                    //{
                    //    disease = "Total: ";
                    //}
                    if (singleFacility == false)
                    {
                        disease = "   " + locationIdToName[locationId];
                        id = "";
                    }


                    string disease_case_pub_tot = disease_case + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                    string disease_death_pub_tot = disease_death + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();


                    reportImmediatelyDataTable.Rows.Add(id, disease, disease_case_pub_tot, disease_death_pub_tot);

                    id = "";
                    if (singleFacility == true)
                    {
                        for (int i = _startWeek; i <= _endWeek; i++)
                        {
                            disease = "     Week: " + i.ToString();
                            id = "";

                            disease_case_pub_tot = disease_case + "_" + i + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                            disease_death_pub_tot = disease_death + "_" + i + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();


                            reportImmediatelyDataTable.Rows.Add(id, disease, disease_case_pub_tot, disease_death_pub_tot);
                        }
                    }
                }

                //reportIpdDataTable.Rows.Add("-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------");

            }
        }
        private void InsertAggregateData(string id, string disease, string disease_case, string disease_death,int oldcode)
        {
            if (disease_case == "")
            {
               disease_death = "";

                reportImmediatelyDataTable.Rows.Add(id, disease, disease_case, disease_death); 
            }
            else
            {               
                string disease_case_tot = "";
                string disease_death_tot = "";


                if (singleFacility == false)
                {
                    reportImmediatelyDataTable.Rows.Add(id, disease, disease_case_tot, disease_death_tot);
                }

                // 2). The Second Entry Loops through all the Health Facilities

                foreach (string locationId in locationsToView)
                {
                    

                    //if (singleFacility == true)
                    //{
                    //    disease = "Total: ";
                    //}
                    if (singleFacility == false)
                    {
                        disease = "   " + locationIdToName[locationId];
                        id = "";
                    }


                    string disease_case_pub_tot = disease_case + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                    string disease_death_pub_tot = disease_death + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();
                    

                    reportImmediatelyDataTable.Rows.Add(id, disease, disease_case_pub_tot, disease_death_pub_tot);

                    id = "";
                    if (singleFacility == true)
                    {
                        for (int i = _startWeek; i <= _endWeek; i++)
                        {
                            disease = "     Week: " + i.ToString();
                            id = "";

                            disease_case_pub_tot = disease_case + "_" + i + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                            disease_death_pub_tot = disease_death + "_" + i + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();
                            

                            reportImmediatelyDataTable.Rows.Add(id, disease, disease_case_pub_tot, disease_death_pub_tot); 
                        }
                    }                    
                }

                //reportIpdDataTable.Rows.Add("-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------");

            }
        }

        private void InsertAggregateData(DataTable dt)
        {            
            if (dt.Rows.Count != 0)
            {
                bool exist = false; string caseNumber = "", caseName = "";
                foreach (DataRow row in dt.Rows)
                {
                if (row["FirstName"].ToString() == "")
                {
                    caseNumber = row["CaseIDNumber"].ToString();
                    caseName = row["CaseName"].ToString();
                    

                    if (reportCaseBasedDataTable.Rows.Count > 0)
                    {
                        for (int i = 0; i < reportCaseBasedDataTable.Rows.Count; i++)
                        {
                            if (caseNumber == reportCaseBasedDataTable.Rows[i]["CaseIDNumber"].ToString() && caseName == reportCaseBasedDataTable.Rows[i]["CaseName"].ToString())
                            {
                                exist = true;
                            }
                        }
                    }

                    if (exist == false)
                    {
                        reportCaseBasedDataTable.Rows.Add(row["CaseIDNumber"].ToString(), row["CaseName"].ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                    }
                }
                else
                {                  

                    caseNumber = row["CaseIDNumber"].ToString();
                    caseName = row["CaseName"].ToString();

                    if (reportCaseBasedDataTable.Rows.Count > 0)
                    {
                        for (int i = 0; i < reportCaseBasedDataTable.Rows.Count; i++)
                        {
                            if (caseNumber == reportCaseBasedDataTable.Rows[i]["CaseIDNumber"].ToString() && caseName == reportCaseBasedDataTable.Rows[i]["CaseName"].ToString())
                            {
                                exist = true;
                            }
                        }
                    }
                    if (singleFacility == false && exist == false)
                    {
                        reportCaseBasedDataTable.Rows.Add(row["CaseIDNumber"].ToString(), row["CaseName"].ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                    }
                  
                    string disease = "";
                    string id = "";
                    if (singleFacility == false)
                    {
                        disease = "   " + locationIdToName[row["HMISCode"].ToString()];
                        id = "";

                        string  DateRecevied_pub_tot = "", DateSent_pub_tot = "", DateOfOnSet_pub_tot = "";
                        DateTime DateOfBirth_pub_tot = General.Util.Constants.NULLDATE, DateReceviedGC_pub_tot = General.Util.Constants.NULLDATE, DateSentGC_pub_tot = General.Util.Constants.NULLDATE, DateOfOnSetGC_pub_tot = General.Util.Constants.NULLDATE;

                        string FirstName_pub_tot = row["FirstName"].ToString();
                        string FatherName_pub_tot = row["FatherName"].ToString(); 
                        string GrandFatherName_pub_tot = row["GrandFatherName"].ToString();
                        if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateOfBirth"].ToString()))
                        {
                            DateOfBirth_pub_tot = DateTime.Parse(row["DateOfBirth"].ToString()); 
                        }
                        string Age_pub_tot = row["Age"].ToString();
                        string Gender_pub_tot = row["Gender"].ToString(); 
                        string Region_pub_tot = row["Region"].ToString(); 
                        string Zone_pub_tot = row["Zone"].ToString();
                        string Woreda_pub_tot = row["Woreda"].ToString(); 
                        string Kebele_pub_tot = row["Kebele"].ToString(); 
                        string HouseNo_pub_tot = row["HouseNo"].ToString(); 
                        string GISGrid_pub_tot = row["GISGrid"].ToString(); 
                        string SymptomStartedGrid_pub_tot = row["SymptomStartedGrid"].ToString();
                        string Note_pub_tot = row["Note"].ToString();
                        string HMISCode_pub_tot = row["HMISCode"].ToString();
                        string FacilityName_pub_tot = row["FacilityName"].ToString();
                        string PatientId_pub_tot = row["PatientId"].ToString();
                        string InOutPatient_pub_tot = row["InOutPatient"].ToString();
                        string OutCome_pub_tot = row["OutCome"].ToString();
                        string caseid_pub_tot = row["CaseIDNumber"].ToString();
                        string caseN_pub_tot = row["CaseName"].ToString();

                        if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateRecevied"].ToString()))
                        {
                            DateReceviedGC_pub_tot = DateTime.Parse(row["DateRecevied"].ToString());
                            DateRecevied_pub_tot = UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateRecevied"].ToString())); 
                        }
                        if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateSent"].ToString()))
                        {
                            DateSentGC_pub_tot = DateTime.Parse(row["DateSent"].ToString());
                            DateSent_pub_tot = UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateSent"].ToString())); 
                        }
                        if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateOfOnSet"].ToString()))
                        {
                            DateOfOnSetGC_pub_tot = DateTime.Parse(row["DateOfOnSet"].ToString());
                            DateOfOnSet_pub_tot = UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateOfOnSet"].ToString())); 
                        }

                        reportCaseBasedDataTable.Rows.Add(id, disease, FirstName_pub_tot, FatherName_pub_tot, GrandFatherName_pub_tot, Age_pub_tot, Gender_pub_tot, DateSent_pub_tot, Region_pub_tot, Zone_pub_tot, Woreda_pub_tot, Kebele_pub_tot, HouseNo_pub_tot, GISGrid_pub_tot, SymptomStartedGrid_pub_tot, Note_pub_tot, DateRecevied_pub_tot, DateOfOnSet_pub_tot, HMISCode_pub_tot, FacilityName_pub_tot, DateOfBirth_pub_tot, PatientId_pub_tot, InOutPatient_pub_tot, OutCome_pub_tot, DateSentGC_pub_tot, DateReceviedGC_pub_tot, DateOfOnSetGC_pub_tot, caseid_pub_tot, caseN_pub_tot);
                    }
                        id = "";
                        if (singleFacility == true)
                        {
                            if (reportCaseBasedDataTable.Rows.Count > 0)
                            {
                                for (int i = 0; i < reportCaseBasedDataTable.Rows.Count; i++)
                                {
                                    if (caseNumber == reportCaseBasedDataTable.Rows[i]["CaseIDNumber"].ToString() && caseName == reportCaseBasedDataTable.Rows[i]["CaseName"].ToString())
                                    {
                                        exist = true;
                                    }
                                }
                            }

                            if (exist == false)
                            {
                                reportCaseBasedDataTable.Rows.Add(row["CaseIDNumber"].ToString(), row["CaseName"].ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                            }

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string disease2 = "     Date: " + UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateSent"].ToString()));
                                string id2 = "";
                                string  DateRecevied_pub_tot = "", DateSent_pub_tot = "", DateOfOnSet_pub_tot = "";
                                DateTime DateOfBirth_pub_tot = General.Util.Constants.NULLDATE, DateReceviedGC_pub_tot = General.Util.Constants.NULLDATE, DateSentGC_pub_tot = General.Util.Constants.NULLDATE, DateOfOnSetGC_pub_tot = General.Util.Constants.NULLDATE;

                                string FirstName_pub_tot = row["FirstName"].ToString();
                                string FatherName_pub_tot = row["FatherName"].ToString();
                                string GrandFatherName_pub_tot = row["GrandFatherName"].ToString();
                                if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateOfBirth"].ToString()))
                                {
                                    DateOfBirth_pub_tot = DateTime.Parse(row["DateOfBirth"].ToString());
                                }
                                string Age_pub_tot = row["Age"].ToString();
                                string Gender_pub_tot = row["Gender"].ToString(); 
                                string Region_pub_tot = row["Region"].ToString(); 
                                string Zone_pub_tot = row["Zone"].ToString(); 
                                string Woreda_pub_tot = row["Woreda"].ToString(); 
                                string Kebele_pub_tot = row["Kebele"].ToString(); 
                                string HouseNo_pub_tot = row["HouseNo"].ToString(); 
                                string GISGrid_pub_tot = row["GISGrid"].ToString(); 
                                string SymptomStartedGrid_pub_tot = row["SymptomStartedGrid"].ToString(); 
                                string Note_pub_tot = row["Note"].ToString();
                                string HMIS_pub_tot = row["HMISCode"].ToString();
                                string FacilityName_pub_tot = row["FacilityName"].ToString();
                                string PatientId_pub_tot = row["PatientId"].ToString();
                                string InOutPatient_pub_tot = row["InOutPatient"].ToString();
                                string OutCome_pub_tot = row["OutCome"].ToString();
                                string CaseId_pub_tot = row["CaseIDNumber"].ToString();
                                string CaseN_pub_tot = row["CaseName"].ToString();

                                if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateRecevied"].ToString()))
                                {
                                    DateReceviedGC_pub_tot = DateTime.Parse(row["DateRecevied"].ToString());
                                    DateRecevied_pub_tot = UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateRecevied"].ToString())); 
                                    
                                }
                                if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateSent"].ToString()))
                                {
                                    DateSentGC_pub_tot = DateTime.Parse(row["DateSent"].ToString());
                                    DateSent_pub_tot = UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateSent"].ToString()));
                                }
                                if (General.Util.Constants.NULLDATE != DateTime.Parse(row["DateOfOnSet"].ToString()))
                                {
                                    DateOfOnSetGC_pub_tot = DateTime.Parse(row["DateOfOnSet"].ToString());
                                    DateOfOnSet_pub_tot = UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(row["DateOfOnSet"].ToString())); 
                                }
                                reportCaseBasedDataTable.Rows.Add(id2, disease2, FirstName_pub_tot, FatherName_pub_tot, GrandFatherName_pub_tot, Age_pub_tot, Gender_pub_tot, DateSent_pub_tot, Region_pub_tot, Zone_pub_tot, Woreda_pub_tot, Kebele_pub_tot, HouseNo_pub_tot, GISGrid_pub_tot, SymptomStartedGrid_pub_tot, Note_pub_tot, DateRecevied_pub_tot, DateOfOnSet_pub_tot, HMIS_pub_tot, FacilityName_pub_tot, DateOfBirth_pub_tot, PatientId_pub_tot, InOutPatient_pub_tot, OutCome_pub_tot, DateSentGC_pub_tot, DateReceviedGC_pub_tot, DateOfOnSetGC_pub_tot, CaseId_pub_tot, CaseN_pub_tot);
                            }
                        }

                        exist = false;
                    

                }
            }
        }
        }

    }
}
