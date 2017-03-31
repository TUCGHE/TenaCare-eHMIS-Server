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
//using General.Util;
//using General.Util.Logging;
using SqlManagement.Database;
using System.IO;

namespace eHMIS.HMIS.ReportAggregation.CustomReports
{
    public class CustomPHEMReportAggr : ICustomReport
    {
        DBConnHelper _helper = new DBConnHelper();

        Hashtable aggregateDataHash = new Hashtable();

        Hashtable ethMonth = new Hashtable();

        List<string> locationsToView = new List<string>();
        Hashtable locationIdToName = new Hashtable();
        DataTable reportWeeklyDataTable = new DataTable();
        DataTable reportImmediatelyDataTable = new DataTable();
        DataTable reportCaseBasedDataTable = new DataTable();
        DataTable reportDailyEpidemicDataTable = new DataTable();
        DataTable dtweek = new DataTable();
        Boolean singleFacility = false;
        private static int caseNo = 0;
        private static string CaseName = "";
        private volatile bool _shouldStop;

        string viewLabeIdTableName = "";

        int _startWeek;
        int _endWeek;
        int _columnadd;
        int _startMonth;
        int _endMonth;
        int _startYear;
        int _endYear;
        int _quarterStart;
        int _quarterEnd;
        int _repKind;
        int _repPeriod;
        string _startDate;
        string _endDate;
        bool _chkFacility;
        int year;

        int _phemNational = 0;
        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation


        public CustomPHEMReportAggr(List<string> locations, int startWeek, int endWeek, int yearStart, int repKind, int reportPeriod, bool chkfacility)
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
            _repPeriod = reportPeriod;
            _repKind = repKind;
            _chkFacility = chkfacility;


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
            else if (_phemNational == 1)
            {
                viewLabeIdTableName = "EthEhmis_ImmediatelyView";

                //reportImmediatelyDataTable.Columns.Add("Id", typeof(string));
                //reportImmediatelyDataTable.Columns.Add("Disease", typeof(string));
                //reportImmediatelyDataTable.Columns.Add("Disease_Case", typeof(string));
                //reportImmediatelyDataTable.Columns.Add("Disease_Death", typeof(string));               
            }
            else if (_repKind == 1)
            {
                viewLabeIdTableName = "vw_EthEhmis_CaseBasedView";
            }

            reportWeeklyDataTable.Columns.Add("Region Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Zone Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Woreda Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Reporting Admin Site", typeof(string));
            reportWeeklyDataTable.Columns.Add("Facility_Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("HMISCode", typeof(string));
            reportWeeklyDataTable.Columns.Add("Year", typeof(string));
            reportWeeklyDataTable.Columns.Add("WHO_Week", typeof(string));
           // reportWeeklyDataTable.Columns.Add("Month", typeof(string));


        }
        public CustomPHEMReportAggr(List<string> locations, int yearStart, int yearEnd, int repKind, int reportPeriod)
        {
            _startYear = yearStart;
            _endYear = yearEnd;
            _repPeriod = reportPeriod;
            _repKind = repKind;
            locationsToView = locations;

            if (locationsToView.Count == 1)
                singleFacility = true; // Only for a single facility, thus show detail month to month data
            // or detail Quarter to Quarter data, or Week to Week Data including aggregate


            if (_repKind == 0)
            {
                viewLabeIdTableName = "EthEhmis_WeeklyView";

            }
            else if (_phemNational == 1)
            {
                viewLabeIdTableName = "EthEhmis_ImmediatelyView";
            }


            reportWeeklyDataTable.Columns.Add("Region Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Zone Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Woreda Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("Reporting Admin Site", typeof(string));
            reportWeeklyDataTable.Columns.Add("Facility_Name", typeof(string));
            reportWeeklyDataTable.Columns.Add("HMISCode", typeof(string));
            reportWeeklyDataTable.Columns.Add("Year", typeof(string));
            reportWeeklyDataTable.Columns.Add("WHO_Week", typeof(string));
           // reportWeeklyDataTable.Columns.Add("Month", typeof(string));

        }

        public CustomPHEMReportAggr(List<string> locations, string startDate, string endDate, int repKind, int reportPeriod)
        {
            _startDate = startDate;
            _endDate = endDate;
            _repKind = repKind;
            _repPeriod = reportPeriod;

            locationsToView = locations;

            if (locationsToView.Count == 1)
                singleFacility = true; // Only for a single facility, thus show detail month to month data
            // or detail Quarter to Quarter data, or Week to Week Data including aggregate

            if (_repPeriod == 2 && repKind == 0)
            {
                // viewLabeIdTableName = "vw_EthEhmis_CaseBasedView";

                reportDailyEpidemicDataTable.Columns.Add("Epidemic Disease", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("Name of Kebeles Affected", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("Date of onset of the Epidemic", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("M", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("F", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("M ", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("F ", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("M  ", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("F  ", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add(" M", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add(" F", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("  M", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("  F", typeof(string));
                reportDailyEpidemicDataTable.Columns.Add("M+F", typeof(string));

            }
            else if (_repKind == 1 && reportPeriod == 2)
            {
                viewLabeIdTableName = "vw_EthEhmis_CaseBasedView";
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


        private DataTable DynamicQueryConstruct(string locationID)
        {

            string idQuery = "";
            string dataEleClassQuery = "";
            aggregateDataHash.Clear();
            DataTable table = new DataTable();
            string cmdText = "";

            if (_repKind == 0)
            {
                dataEleClassQuery = " and DataEleClass = 25 "; // Weekly
            }
            else if (_phemNational == 1)
            {
                dataEleClassQuery = " and DataEleClass = 26 "; // Immediately
            }

            if (_repKind == 0 || _phemNational == 1)  //if (_repKind == 0 || _repKind == 1)
            {
                // foreach (string locationID in locationsToView)
                //  {
                string id = null;
                if (locationID.Equals("AllFacilities"))
                {
                    id = HMISMainPage.SelectedLocationID.ToString();
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
                        idQuery = " and ReportingDistrictId like '" + id + "%'"; // this is added for the v_EthEhmis_HMISallData becuase the view that displays zoneid is null in some special zones.
                    }
                    cmdText = "select * from v_EthEhmis_HMISallData where week >= " + _startWeek + " and week <= " + _endWeek + " and Year = @startYear " + idQuery + " order by ReportingRegionName, ReportingAdminSite, FacilityName, week ";
                    table = addToHashTable(cmdText, id, _startWeek, _endWeek, _startYear);
                }
                else
                {
                    id = locationID;

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



                    string monthQueryGroup = "";



                    //monthQueryGroup = "	where  Week  >=@StartWeek and Week <= @EndWeek  and Year = @startYear and level = 0 ";
                    ////monthQueryGroup = "	where  Week  = @StartWeek and Year = @startYear and level = 0 ";

                    //cmdText =
                    // "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                    // "   sum(Value) as Value from EthEhmis_HmisValue  " +
                    //    //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                    // monthQueryGroup +
                    //    dataEleClassQuery + idQuery +
                    // "	group by DataEleClass,  LabelID  ";
                    //addToHashTable(cmdText, id, _startWeek, _endWeek, _startYear);


                    if (_endYear == 0)
                    {
                        monthQueryGroup = "	where  Week  = @StartWeek  and Year = @startYear";

                        /* for (int i = _startWeek; i <= _endWeek; i++)
                         {
                             int week = i;
                             cmdText =
                             "	select cast(LabelID as VarChar) + '_" + week + "_" + locationID + "' as LabelID, " +
                             "   sum(Value) as Value from EthEhmis_HmisValue   " +
                                 //"	where  Month  = @StartMonth  and Year = @startYear " +
                             monthQueryGroup +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";*/
                        cmdText = "select week, DataEleClass, LabelID, sum(value) as value from EthEhmis_HMISValue where week >= " + _startWeek + " and week <= " + _endWeek + " and Year = @startYear" + idQuery
                                    + " group by week, LabelID, DataEleClass order by week";
                        table = addToHashTable(cmdText, id, _startWeek, _endWeek, _startYear);


                        //  }

                    }
                    else
                    {
                        _startWeek = 1;
                        _endWeek = 52;
                        /*for (year = _startYear; year <= _endYear; year++)
                        {
                            
                            string cmdtxt = "select distinct week from EthEhmis_HmisValue where year = " + year + dataEleClassQuery + idQuery + " order by week";
                            SqlCommand toExecute = new SqlCommand(cmdtxt);
                            toExecute.Parameters.AddWithValue("newIdentification", id);
                            toExecute.CommandTimeout = 4000; //300 // = 1000000;
                             dtweek = _helper.GetDataSet(toExecute).Tables[0];
                            if (dtweek.Rows.Count > 0)
                            {
                                //_startWeek = Convert.ToInt16(dt.Rows[0]["week"].ToString());
                               // _endWeek = Convert.ToInt16((dt.Rows[dt.Rows.Count - 1]["week"]).ToString());
                              
                                monthQueryGroup = "	where  Week  = @StartWeek  and Year = @startYear";

                                foreach (DataRow row in dtweek.Rows)
                                {
                                    int week = Convert.ToInt16(row["week"].ToString());
                                    cmdText =
                                    "	select cast(LabelID as VarChar) + '_" + week + "_" + year + "_" + locationID + "' as LabelID, " +
                                    "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                        //"	where  Month  = @StartMonth  and Year = @startYear " +
                                    monthQueryGroup +
                                       dataEleClassQuery + idQuery +
                                    "	group by DataEleClass,  LabelID  ";*/
                        cmdText = "select week, DataEleClass, LabelID, sum(value) as value from EthEhmis_HMISValue where week >= " + _startWeek + " and week <= " + _endWeek + " and Year = @startYear" + idQuery
                                  + " group by week, LabelID, DataEleClass order by week";
                        table = addToHashTable(cmdText, id, 1, _endWeek, _startYear);
                        //}
                        // }
                        //}
                    }
                }
                // }
            }
            else if (_repKind == 1)
            {
                // foreach (string locationID in locationsToView)
                // {
                string id = locationID;

                //if (id == "10") id = "14";
                int aggregationLevel = getAggregationLevel(id);
                string regionName = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(id.ToString());
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

                cmdText = "Select * from vw_EthEhmis_CaseBasedView";

                string monthQueryGroup = "";

                monthQueryGroup = "	where  Week  >=@StartWeek and Week <= @EndWeek  and Year = @startYear   ";
                //select FirstName, FatherName, GrandFatherName, DateOfBirth, Age, 
                //Gender, Region, Zone, Woreda, Kebele, HouseNo, GISGrid, SymptomStartedGrid, Note,
                //CaseIDNumber, CaseName, DateRecevied, DateSent, DateOfOnSet 

                cmdText = "Select * from vw_EthEhmis_CaseBasedView " +
                    monthQueryGroup +
                     idQuery;
                table = addToHashTable(cmdText, id, _startWeek, _endWeek, _startYear);

                //if (singleFacility == true)
                //{
                //    monthQueryGroup = "	where  Week  = @StartWeek  and Year = @startYear   ";

                //    for (int i = _startWeek; i <= _endWeek; i++)
                //    {
                //        int week = i;
                //        cmdText = "Select * from vw_EthEhmis_CaseBasedView " +
                //        monthQueryGroup +
                //           idQuery;
                //        addToHashTable(cmdText, id, week, _endWeek, _startYear);
                //    }
                //}
            }
            //}
            return table;
        }

        private DataTable addToHashTable(string cmdText, string id, int startMonth, int endMonth, int startYear)
        {

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("newIdentification", id);
            toExecute.Parameters.AddWithValue("StartMonth", startMonth);
            toExecute.Parameters.AddWithValue("EndMonth", endMonth);
            toExecute.Parameters.AddWithValue("startYear", startYear);
            toExecute.Parameters.AddWithValue("endYear", _endYear);
            toExecute.Parameters.AddWithValue("StartWeek", startMonth);
            toExecute.Parameters.AddWithValue("EndWeek", _endWeek);

            toExecute.CommandTimeout = 0; //300 // = 1000000;
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            /* if (_repKind == 0 || _phemNational == 1)
             {
                 foreach (DataRow row in dt.Rows)
                 {
                     string LabelID = row["labelID"].ToString();
                     int value = Convert.ToInt32(row["value"]);
                     int week = Convert.ToInt16(row["week"]);
                     aggregateDataHash.Add(LabelID, value);
                     //aggregateDataWeekList.Add(LabelID, week);
                 }
             }*/
            return dt;
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        public DataTable CreateReport()
        {
            while (!_shouldStop)
            {

                DataTable dtImmediate = new DataTable();
                int swapweek = 0, indexfound = -1, rowindex = -1;
                string idQuery = "";
                locationIdToName.Clear();
                foreach (string locationID in locationsToView)
                {
                    string id = locationID;
                    if (_chkFacility == true)
                    {
                        break;
                    }
                    else
                    {
                        
                        string facilityName = getFacilityName(id);
                        locationIdToName.Add(locationID, facilityName);
                    }
                }

                if ((_repPeriod == 0 && _repKind == 0) || (_repPeriod == 1 && _repKind == 0) || _phemNational == 1)
                {
                    viewLabeIdTableName = "EthEhmis_WeeklyView";
                    string cmdText = "SELECT Id, Disease, OPD_Case, IPD_Case, IPD_Death from  " + viewLabeIdTableName;
                    SqlCommand toExecute;
                    toExecute = new SqlCommand(cmdText);

                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                    string id, disease, opd_case, ipd_case, ipd_death;

                    DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
                    foreach (DataRow row in dt1.Rows)
                    {
                        //if (_repKind == 0)
                        //{


                        reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " OPD Case");
                        reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " IPD Case");
                        reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " IPD Death");
                        /*  }
                          else if (_phemNational == 1)
                          {
                              reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " Case");
                              reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " Death");
                          }*/
                    }
                    viewLabeIdTableName = "EthEhmis_ImmediatelyView";
                    cmdText = "SELECT Id, Disease, DiseaseCase, DiseaseDeath from  " + viewLabeIdTableName;
                    //  SqlCommand toExecute;
                    toExecute = new SqlCommand(cmdText);

                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                    dtImmediate = _helper.GetDataSet(toExecute).Tables[0];
                    foreach (DataRow row in dtImmediate.Rows)
                    {
                        reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " Case");
                        reportWeeklyDataTable.Columns.Add(row["Disease"].ToString() + " Death");
                    }


                    DataTable datatable;
                    if (_endYear == 0)
                        _endYear = _startYear;
                    for (year = _startYear; year <= _endYear; year++)
                    {
                        if (_repPeriod == 0)
                            _endYear = 0;
                        _startYear = year;
                        // if all the facilities is checked it uses a view in oreder to decrease the amount of time that takes to display the report
                        if (_chkFacility == true)
                        {
                            int week1, week2 = 0, count = -1;
                            string sameLabelId = null, labelid = null;
                            string facility = "";
                            datatable = DynamicQueryConstruct("AllFacilities");
                            foreach (DataRow dtrow in datatable.Rows)
                            {
                                week1 = int.Parse(dtrow["week"].ToString());
                                sameLabelId = dtrow["LabelId"].ToString();
                                if((week1 == week2) && (sameLabelId == labelid ))
                                {
                                    reportWeeklyDataTable.Rows.Add(dtrow["ReportingRegionName"], dtrow["ZONENAME"], dtrow["WoredaName"], dtrow["ReportingAdminSite"], dtrow["FacilityName"], dtrow["HMISCode"],dtrow["Year"], dtrow["Week"]);
                                    count++;
                                }
                                if (week1 != week2 || facility != dtrow["HMISCode"].ToString())
                                {
                                    reportWeeklyDataTable.Rows.Add(dtrow["ReportingRegionName"], dtrow["ZONENAME"], dtrow["WoredaName"], dtrow["ReportingAdminSite"], dtrow["FacilityName"], dtrow["HMISCode"],dtrow["Year"], dtrow["Week"]);
                                    count++;
                                }

                                if (dtrow["ImmediateDisease"].ToString() == "")
                                {
                                    labelid = dtrow["LabelId"].ToString();
                                    DataRow[] rowweekly = dt1.Select("IPD_Case =" + labelid + " or OPD_Case = " + labelid + " or IPD_Death = " + labelid);
                                    if (rowweekly.Length > 0)
                                        rowindex = dt1.Rows.IndexOf(rowweekly[0]);
                                    if (rowindex != -1 && rowindex <= 10)
                                        if (dt1.Rows[rowindex]["IPD_Case"].ToString() == labelid)
                                        {
                                            string diseaseV = dt1.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " IPD Case";
                                            reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                        }
                                        else if (dt1.Rows[rowindex]["OPD_Case"].ToString() == labelid)
                                        {
                                            string diseaseV = dt1.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " OPD Case";
                                            reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                        }
                                        else if (dt1.Rows[rowindex]["IPD_Death"].ToString() == labelid)
                                        {
                                            string diseaseV = dt1.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " IPD Death";
                                            reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                        }
                                    // reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                }
                                else
                                {
                                    labelid = dtrow["LabelId"].ToString();
                                    DataRow[] rowimmidiate = dtImmediate.Select("DiseaseCase = " + labelid + " or DiseaseDeath = " + labelid);
                                    if (rowimmidiate.Length > 0)
                                        rowindex = dtImmediate.Rows.IndexOf(rowimmidiate[0]);
                                    if (rowindex != -1 && rowindex <= 16)
                                        if (dtImmediate.Rows[rowindex]["DiseaseCase"].ToString() == labelid)
                                        {
                                            string diseaseV = dtImmediate.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " Case";

                                            reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                        }
                                        else if (dtImmediate.Rows[rowindex]["DiseaseDeath"].ToString() == labelid)
                                        {
                                            string diseaseV = dtImmediate.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " Death";
                                            reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                        }
                                    // reportWeeklyDataTable.Rows[count][diseaseV] = dtrow["Value"].ToString();
                                }
                                week2 = week1;
                                facility = dtrow["HMISCode"].ToString();
                            }
                        }
                        else
                        {
                            foreach (string locationID in locationsToView)
                            {
                                //  _repKind = 0;

                                datatable = DynamicQueryConstruct(locationID);

                                //  string hmiscodeF = locationIdToName[locationID].ToString();
                                /* int aggregationLevel = getAggregationLevel(locationID);
                                 if (aggregationLevel == 1)
                                 {
                                     idQuery = "";
                                 }
                                 else if (aggregationLevel == 2)
                                 {
                                     idQuery = " where RegionID = @newIdentification ";
                                 }
                                 else if (aggregationLevel == 3)
                                 {
                                     idQuery = " where WoredaID = @newIdentification ";
                                 }
                                 else if (aggregationLevel == 4)
                                 {
                                     idQuery = " where HMISCode = @newIdentification ";
                                 }
                                 else if (aggregationLevel == 5)
                                 {
                                     idQuery = " where ZoneID = @newIdentification ";
                                 }
                                 cmdText = "select Distinct HMISCode,ReportingRegionName, ZoneName, WoredaName,ReportingAdminSite,FacilityName from v_EthEhmis_HMISallData "+ idQuery;
                                 toExecute = new SqlCommand(cmdText);
                                 toExecute.Parameters.AddWithValue("newIdentification", locationID);
                                 DataTable facilityrow = _helper.GetDataSet(toExecute).Tables[0];

                                // int regionId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(locationID);
                                 string regionName = facilityrow.Rows[0]["ReportingRegionName"].ToString();
                                    // Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(regionId.ToString()).Replace("Regional Health Bureau", "");
                                // int zoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(locationID);
                                 string zonename = facilityrow.Rows[0]["ZoneName"].ToString();
                                     //Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(zoneId.ToString()).Replace("Zonal Health Department", ""); ;
                                // int woredaId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getWoredaId(locationID);
                                 string woredaname = facilityrow.Rows[0]["WoredaName"].ToString();
                                     //Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(woredaId.ToString()).Replace("Woreda Health Office", "");
                                 //int districtid = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getHCId(locationID);
                                 string hcname = facilityrow.Rows[0]["ReportingAdminSite"].ToString();
                                     //Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(districtid.ToString());
                                 string facilityN = facilityrow.Rows[0]["FacilityName"].ToString();
                                 if (regionName == "Federal Ministry of Health")
                                     regionName = "FMOH";*/
                                //   reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationID], _startYear.ToString(), 1, _startMonth.ToString());
                                string hmiscodeF = locationIdToName[locationID].ToString();
                                int regionId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(locationID);
                                string regionName =
                                    Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(regionId.ToString()).Replace("Regional Health Bureau", "");
                                int zoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(locationID);
                                string zonename = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(zoneId.ToString()).Replace("Zonal Health Department", ""); ;
                                int woredaId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getWoredaId(locationID);
                                string woredaname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(woredaId.ToString()).Replace("Woreda Health Office", "");
                                int districtid = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getHCId(locationID);
                                string hcname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(districtid.ToString());
                                if (regionName == "Federal Ministry of Health")
                                    regionName = "FMOH";
                                for (int w = _startWeek; w <= _endWeek; w++)
                                    reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationID], locationID,_startYear.ToString(), w);

                                foreach (DataRow row2 in datatable.Rows)
                                {
                                    int week = Convert.ToInt16(row2["Week"].ToString());
                                  //  string name = locationIdToName[locationID].ToString();
                                    string weeks = week.ToString();
                                    string filter = "'HMISCode' = '" + locationID + "' and 'WHO Week' = '" + week.ToString() + "'";
                                    if (swapweek != week)
                                    {

                                        DataRow[] facilitiestoAdd = reportWeeklyDataTable.Select("HMISCode ='" + locationID + "' and WHO_Week = '" + weeks + "'");
                                        indexfound = reportWeeklyDataTable.Rows.IndexOf(facilitiestoAdd[0]);
                                        // indexfound++;
                                        //  reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationID], _startYear.ToString(), week, _startMonth.ToString());
                                    }
                                    if (_startWeek == _endWeek)
                                    {
                                        swapweek = 0;
                                    }
                                    else
                                    {
                                        swapweek = week;
                                    }

                                    string labelid = row2["LabelId"].ToString();
                                    DataRow[] row3 = dt1.Select("IPD_Case =" + labelid + " or OPD_Case = " + labelid + " or IPD_Death = " + labelid);
                                    DataRow[] rowimmidiate = dtImmediate.Select("DiseaseCase = " + labelid + " or DiseaseDeath = " + labelid);

                                    if (row3.Length > 0)
                                        rowindex = dt1.Rows.IndexOf(row3[0]);
                                    if (rowimmidiate.Length > 0)
                                        rowindex = dtImmediate.Rows.IndexOf(rowimmidiate[0]);
                                    if (rowindex != -1 && rowindex <= 10)
                                        if (dt1.Rows[rowindex]["IPD_Case"].ToString() == labelid)
                                        {
                                            string diseaseV = dt1.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " IPD Case";
                                            reportWeeklyDataTable.Rows[indexfound][diseaseV] = Decimal.Round(Convert.ToDecimal(row2["value"])).ToString();
                                        }
                                        else if (dt1.Rows[rowindex]["OPD_Case"].ToString() == labelid)
                                        {
                                            string diseaseV = dt1.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " OPD Case";
                                            reportWeeklyDataTable.Rows[indexfound][diseaseV] = Decimal.Round(Convert.ToDecimal(row2["value"])).ToString();
                                        }
                                        else if (dt1.Rows[rowindex]["IPD_Death"].ToString() == labelid)
                                        {
                                            string diseaseV = dt1.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " IPD Death";
                                            reportWeeklyDataTable.Rows[indexfound][diseaseV] = Decimal.Round(Convert.ToDecimal(row2["value"])).ToString();
                                        }

                                    if (rowindex != -1 && rowindex <= 16)
                                        if (dtImmediate.Rows[rowindex]["DiseaseCase"].ToString() == labelid)
                                        {
                                            string diseaseV = dtImmediate.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " Case";
                                            reportWeeklyDataTable.Rows[indexfound][diseaseV] = Decimal.Round(Convert.ToDecimal(row2["value"])).ToString();
                                        }
                                        else if (dtImmediate.Rows[rowindex]["DiseaseDeath"].ToString() == labelid)
                                        {
                                            string diseaseV = dtImmediate.Rows[rowindex]["Disease"].ToString();
                                            diseaseV = diseaseV + " Death";
                                            reportWeeklyDataTable.Rows[indexfound][diseaseV] = Decimal.Round(Convert.ToDecimal(row2["value"])).ToString();
                                        }


                                }
                                // This part of the code deletes facility names if is not Health center or Health post
                                int aggregationLevel = getAggregationLevel(locationID);

                                if (aggregationLevel != 4)
                                {
                                    for (int w = _startWeek; w <= _endWeek; w++)
                                    {
                                        string name = locationIdToName[locationID].ToString();
                                        string weeks = w.ToString();
                                        DataRow[] facilities = reportWeeklyDataTable.Select("Facility_Name ='" + name + "' and WHO_Week = '" + weeks + "'");
                                        indexfound = reportWeeklyDataTable.Rows.IndexOf(facilities[0]);
                                        reportWeeklyDataTable.Rows[indexfound]["Facility_Name"] = "";
                                    }
                                }

                                /*   foreach (DataRow row in dt1.Rows)
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
                                   _phemNational = 1;
                                   _repKind = -1;
                                   DynamicQueryConstruct(locationID);
                                   viewLabeIdTableName = "EthEhmis_ImmediatelyView";
                                   cmdText = "SELECT Id, Disease, DiseaseCase, DiseaseDeath from  " + viewLabeIdTableName;
                                 //  SqlCommand toExecute;
                                   toExecute = new SqlCommand(cmdText);

                                   toExecute.CommandTimeout = 4000; //300 // = 1000000;
                                   string disease_case, disease_death;

                                   //DataTable
                                       dt1 = _helper.GetDataSet(toExecute).Tables[0];

                                   foreach (DataRow row in dt1.Rows)
                                   {
                                       id = row["ID"].ToString();
                                       disease = row["disease"].ToString();
                                       disease_case = row["DISEASECASE"].ToString();
                                       disease_death = row["DISEASEDEATH"].ToString();

                                       // Call the insert statement

                                       InsertAggregateData(id, disease, disease_case, disease_death,"");
                                   }
                                   Dictionary<string, string> colMap = new Dictionary<string, string>();
               
                                  */

                            }
                        }
                    }
                    return reportWeeklyDataTable;
                }

                else if (_repKind == 1 && _repPeriod == 2)// linelist
                {
                    DataTable dt = null, dt2 = null;
                    idQuery = "";
                    int aggregationLevel = 0;
                    foreach (string locationID in locationsToView)
                    {
                        string id = locationID;

                        //if (id == "10") id = "14";
                       aggregationLevel = getAggregationLevel(id);
                        string[] regionName = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(id.ToString()).Split();
                        string regionId = locationID;
                        string zoneId = locationID;

                        if (aggregationLevel == 1)
                        {
                            idQuery = "";

                        }
                        else if (aggregationLevel == 2)
                        {
                            idQuery = " and RegionID = " + locationID;
                        }
                        else if (aggregationLevel == 3)
                        {
                            idQuery = " and WoredaID = " + locationID;
                        }

                        else if (aggregationLevel == 4)
                        {
                            idQuery = " and WoredaID = " + locationID;
                        }
                        else if (aggregationLevel == 5)
                        {
                            idQuery = " and ZoneID = " + locationID;


                        }
                        if(aggregationLevel != 4)
                        {
                            string cmdText = "SELECT * from " + viewLabeIdTableName + " where DateOfOnSet >= " + "'" + _startDate + "'" + " and DateOfOnSet <= " + "'" + _endDate + "'" + idQuery; // linelist + casebased
                        SqlCommand toExecute;
                        toExecute = new SqlCommand(cmdText);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        //   string id, disease, disease_case, disease_death;
                        if (dt != null)
                        {
                            dt2 = dt.Copy();
                        }
                        dt = _helper.GetDataSet(toExecute).Tables[0];

                        if (dt2 != null && locationsToView.Count > 1)
                        {
                            dt.Merge(dt2);
                        }
                        dt.Columns.Add("Age in Month").SetOrdinal(11);
                        dt.Columns.Add("Age in Days").SetOrdinal(12);
                        dt.Columns.Add("Affected Region").SetOrdinal(8);
                        dt.Columns.Add("Affected Zone").SetOrdinal(9);
                        dt.Columns.Add("Affected Woreda").SetOrdinal(10);

                        Age ageCalculate = null;
                        int index = 0;
                        DateTime nullDate = Convert.ToDateTime("01/01/1900");
                        foreach (DataRow row in dt.Rows)
                        {
                            string region = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(row["RegionID"].ToString());
                            string zone = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(row["ZoneID"].ToString());
                            string woreda = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(row["WoredaID"].ToString());
                            DateTime dateSpecimen = Convert.ToDateTime(row["DateSpecimenCollected"].ToString());
                            DateTime dateSeenHF = Convert.ToDateTime(row["DateSeenAtHF"].ToString());
                            DateTime dateofOnset = Convert.ToDateTime(row["DateOfOnSet"].ToString());
                            DateTime reportDate = Convert.ToDateTime(row["ReportDate"].ToString());
                            if (dateSpecimen == nullDate) 
                            {
                                dt.Rows[index]["DateSpecimenCollected"] = DBNull.Value;
                            }
                            if(dateSeenHF == nullDate)
                            {
                                dt.Rows[index]["DateSeenAtHF"] = DBNull.Value;
                            }
                            if(dateofOnset == nullDate)
                            {
                                dt.Rows[index]["DateOfOnSet"] = DBNull.Value;
                            }
                            if(reportDate == null)
                            {
                                dt.Rows[index]["ReportDate"] = DBNull.Value;
                            }
                            ageCalculate = new Age(Convert.ToDateTime(row["dateofbirth"]), Convert.ToDateTime(row["CreatedDate"]));
                            if (ageCalculate.Years == 0)
                            {
                                dt.Rows[index]["Age in Month"] = ageCalculate.Months;
                                dt.Rows[index]["Age in Days"] = ageCalculate.Days;

                            }
                            dt.Rows[index]["Affected Region"] = region;
                            dt.Rows[index]["Affected Zone"] = zone;
                            dt.Rows[index]["Affected Woreda"] = woreda;
                            index++;
                        }
                        InsertAggregateData(dt);
                    }

                    }
                    if (aggregationLevel != 4)
                    {
                        dt.Columns.Remove("WoredaID");
                        dt.Columns.Remove("ZoneID");
                        dt.Columns.Remove("RegionID");
                        dt.Columns.Remove("DateOfBirth");
                        dt.Columns.Remove("CreatedDate");
                        dt.Columns["DataEleClass"].ColumnName = "Report Type";
                        dt.Columns["Age"].ColumnName = "Age in Year";
                    }



                    return dt;

                }
                else if (_repKind == 0 && _repPeriod == 2) //Daily Epidemic
                {
                    DataTable dt = null, dt2 = null;
                    idQuery = "";
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
                        /*  else if (aggregationLevel == 4)
                          {
                              idQuery = " and HMISCode = @newIdentification ";
                          }*/
                        else if (aggregationLevel == 5)
                        {
                            idQuery = " and ZoneID = @newIdentification ";
                        }
                        if(aggregationLevel != 4)
                        {
                        string cmdText = "select Region, Zone, Woreda, EpdiemicDisease, NameofKebeleAffected, DateonsetEpidemic,reportType, DateEntered," +
                        "\"M<5years\", \"F<5years\", \"M5-14years\", \"F5-14years\", \"M15-44years\", \"F15-44years\", \"M45+years\", \"F45+years\", Week, Year from eth_PHEMDailyEpidemic where DateEntered >= " + "'" + _startDate + "'" + " and DateEntered <= " + "'" + _endDate + "'" + " " + idQuery + " order by reportType"; // Daily epidemic
                        SqlCommand toExecute = new SqlCommand(cmdText);
                        toExecute.Parameters.AddWithValue("newIdentification", id);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        //   string id, disease, disease_case, disease_death;
                        if (dt != null)
                        {
                            dt2 = dt.Copy();
                        }
                        dt = _helper.GetDataSet(toExecute).Tables[0];

                        if (dt2 != null && locationsToView.Count > 1)
                        {
                            dt.Merge(dt2);
                        }

                        // InsertAggregateData(dt);

                    }
                    }
                    return dt;

                }
                else if (_repKind == 3 && _repPeriod == 2) //Daily Epidemic Summary
                {
                    //try{
                    DataTable dt = null, dt2 = null;
                    idQuery = "";
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
                        /*  else if (aggregationLevel == 4)
                          {
                              idQuery = " and HMISCode = @newIdentification ";
                          }*/
                        else if (aggregationLevel == 5)
                        {
                            idQuery = " and ZoneID = @newIdentification ";
                        }
                        if(aggregationLevel != 4)
                        {
                        string cmdText = "select Region, Zone, Woreda, EpdiemicDisease, NameofKebeleAffected, DateonsetEpidemic,reportType, DateEntered," +
                        "\"M<5years\", \"F<5years\", \"M5-14years\", \"F5-14years\", \"M15-44years\", \"F15-44years\", \"M45+years\", \"F45+years\" from eth_PHEMDailyEpidemic where DateEntered >= " + "'" + _startDate + "'" + " and DateEntered <= " + "'" + _endDate + "'" + " " + idQuery + " order by reportType"; // Daily epidemic
                        SqlCommand toExecute = new SqlCommand(cmdText);
                        toExecute.Parameters.AddWithValue("newIdentification", id);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        //   string id, disease, disease_case, disease_death;
                        if (dt != null)
                        {
                            dt2 = dt.Copy();
                        }
                        dt = _helper.GetDataSet(toExecute).Tables[0];

                        if (dt2 != null && locationsToView.Count > 1)
                        {
                            dt.Merge(dt2);
                        }

                        DataTable dt3;
                        int age, cases = 0, rowIndex =0, ageHolder = 0;
                        int cases1 = 0, cases2 = 0, cases3 = 0, cases4 = 0, cases5 = 0, cases6 = 0, cases7 = 0, cases8 = 0;
                        string  dateReceived, nameOfAffectedKeble = null;
                        DateTime dateOnsetofEpidemic, dateseen, swapDateOfOnset;
                        string gender;
                        string cmdtext2 = "select RegionID, ZoneID, WoredaID, CaseName, NameOfKebeleAffected, DateOfOnSet, OutCome, DateSeenAtHF, Age, Gender from vw_EthEhmis_CaseBasedView" +
                            " where DateSeenAtHF >= " + "'" + _startDate + "'" + " and DateSeenAtHF <= " + "'" + _endDate + "'" + " " + idQuery + " order by DateSeenAtHF";
                        // InsertAggregateData(dt);
                        toExecute = new SqlCommand(cmdtext2);
                        toExecute.Parameters.AddWithValue("newIdentification", id);

                        toExecute.CommandTimeout = 4000; //300 // = 1000000;
                        dt3 = _helper.GetDataSet(toExecute).Tables[0];
                        int rowCount = dt.Rows.Count;

                        if (dt3.Rows.Count > 0)
                        {
                            foreach (DataRow datarow in dt3.Rows)
                            {
                                age = int.Parse(datarow["Age"].ToString());
                                gender = datarow["Gender"].ToString();
                                dateReceived = Convert.ToDateTime(datarow["DateSeenAtHF"]).ToShortDateString();
                                dateseen = Convert.ToDateTime(dateReceived);
                                dateOnsetofEpidemic = Convert.ToDateTime(datarow["DateOfOnSet"]);
                                
                                nameOfAffectedKeble = datarow["NameOfKebeleAffected"].ToString();
                                string region = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(datarow["RegionID"].ToString());
                                string zone = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(datarow["ZoneID"].ToString());
                                string woreda = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(datarow["WoredaID"].ToString());
                                DataRow[] result = dt.Select("NameOfKebeleAffected = '" + nameOfAffectedKeble + "' and DateEntered = '" + dateseen + "'");
                                if (result.Length > 0)
                                {
                                    rowIndex = dt.Rows.IndexOf(result[0]);
                                    swapDateOfOnset = Convert.ToDateTime(dt.Rows[rowIndex]["DateonsetEpidemic"]);
                                    if (dateOnsetofEpidemic < swapDateOfOnset)
                                        dt.Rows[rowIndex]["DateonsetEpidemic"] = dateOnsetofEpidemic;//   }
                              //  if (dateOfOnset.Equals(swapDateOfOnset))
                                //{
                                    if (age < 5 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["M<5years"].ToString());
                                        ageHolder++;
                                       // cases1++;
                                        dt.Rows[rowIndex]["M<5years"] = ageHolder;
                                    }
                                    else if (age < 5 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["F<5years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["F<5years"] = ageHolder;
                                       // dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                            //datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 1, 0, 0, 0, 0, 0, 0);
                                    }
                                    else if (age >= 5 && age <= 14 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["M5-14years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["M5-14years"] = ageHolder;
                                      //  dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                              // datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 0, 1, 0, 0, 0, 0, 0);
                                    }
                                    else if (age >= 5 && age <= 14 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["F5-14years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["F5-14years"] = ageHolder;
                                      //  dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                             //  datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 0, 0, 1, 0, 0, 0, 0);
                                    }
                                    else if (age >= 15 && age <= 44 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["M15-44years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["M15-44years"] = ageHolder;
                                      //  dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                              // datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 0, 0, 0, 1, 0, 0, 0);
                                    }
                                    else if (age >= 15 && age <= 44 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["F15-44years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["F15-44years"] = ageHolder;
                                       // dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                              // datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 0, 0, 0, 0, 1, 0, 0);
                                    }
                                    else if (age >= 45 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["M45+years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["M45+years"] = ageHolder;
                                       // dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                              // datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 0, 0, 0, 0, 0, 1, 0);
                                    }
                                    else if (age >= 45 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ageHolder = Convert.ToInt16(dt.Rows[rowIndex]["F45+years"].ToString());
                                        ageHolder++;
                                        dt.Rows[rowIndex]["F45+years"] = ageHolder;
                                       // dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                          //     datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateRecevied"].ToString(), 0, 0, 0, 0, 0, 0, 0, 1);
                                    }
                                }
                                else
                                {
                                    cases = 0;
                                    cases++;
                                   // rowCount++;
                                    if (age < 5 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                            datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), cases, 0, 0, 0, 0, 0, 0, 0);
                                    }
                                    else if (age < 5 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                            datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, cases, 0, 0, 0, 0, 0, 0);
                                    }
                                    else if (age >= 5 && age <= 14 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                               datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, 0, cases, 0, 0, 0, 0, 0);
                                    }
                                    else if (age >= 5 && age <= 14 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                               datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, 0, 0, cases, 0, 0, 0, 0);
                                    }
                                    else if (age >= 15 && age <= 44 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                               datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, 0, 0, 0, cases, 0, 0, 0);
                                    }
                                    else if (age >= 15 && age <= 44 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                               datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, 0, 0, 0, 0, cases, 0, 0);
                                    }
                                    else if (age >= 45 && gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                               datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, 0, 0, 0, 0, 0, cases, 0);
                                    }
                                    else if (age >= 45 && gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        dt.Rows.Add(region, zone, woreda, datarow["CaseName"].ToString(), datarow["NameOfKebeleAffected"].ToString(),
                                               datarow["DateOfOnSet"].ToString(), datarow["OutCome"].ToString(), datarow["DateSeenAtHF"].ToString(), 0, 0, 0, 0, 0, 0, 0, cases);
                                    }

                                }
                               // swapDateOfOnset = dateOfOnset;
                            }
                        }

                   // }
                    }
                    return dt;
                    }
                  //  catch(Exception ex)
                  //  {
                       
                   // }
                }
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

                /*  if (_repKind == 0)
                  { 
                
                  
                      reportWeeklyDataTable.Columns.Add(disease + " OPD Case");
                      reportWeeklyDataTable.Columns.Add(disease + " IPD Case");
                      reportWeeklyDataTable.Columns.Add(disease + " Death Case");
                  }
                  else if (_phemNational == 1)
                  {
                      reportWeeklyDataTable.Columns.Add(disease + " Case");
                      reportWeeklyDataTable.Columns.Add(disease + " Death");
                  }*/
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
                    bool foundfacility = false;
                    int indexfound = 0;
                    if (_endWeek != 0)
                    {
                        //int week1 = 1;
                        for (int week = _startWeek; week <= _endWeek; week++)
                        {


                            bool foundWeek = false;
                            string opd_case_pub_tot = opd_case + "_" + week + "_" + locationId; opd_case_pub_tot = (aggregateDataHash[opd_case_pub_tot] == null) ? "" : aggregateDataHash[opd_case_pub_tot].ToString();
                            string ipd_case_pub_tot = ipd_case + "_" + week + "_" + locationId; ipd_case_pub_tot = (aggregateDataHash[ipd_case_pub_tot] == null) ? "" : aggregateDataHash[ipd_case_pub_tot].ToString();
                            string ipd_death_pub_tot = ipd_death + "_" + week + "_" + locationId; ipd_death_pub_tot = (aggregateDataHash[ipd_death_pub_tot] == null) ? "" : aggregateDataHash[ipd_death_pub_tot].ToString();

                            // the first for the total


                            string hmiscodeF = locationIdToName[locationId].ToString();
                            int regionId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(locationId);
                            string regionName =
                                Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(regionId.ToString()).Replace("Regional Health Bureau", "");
                            int zoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(locationId);
                            string zonename = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(zoneId.ToString()).Replace("Zonal Health Department", ""); ;
                            int woredaId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getWoredaId(locationId);
                            string woredaname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(woredaId.ToString()).Replace("Woreda Health Office", "");
                            int districtid = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getHCId(locationId);
                            string hcname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(districtid.ToString());
                            if (regionName == "Federal Ministry of Health")
                                regionName = "FMOH";

                            reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], _startYear.ToString(), week.ToString(), _startMonth.ToString());


                            if (reportWeeklyDataTable.Rows.Count == 0)
                            {

                                reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], _startYear.ToString(), week.ToString(), _startMonth.ToString());
                                foundfacility = true;
                                foundWeek = true;
                                indexfound = 0;
                            }
                            else
                            {
                                for (int i = 0; i < reportWeeklyDataTable.Rows.Count; i++)
                                {
                                    if (reportWeeklyDataTable.Rows[i][4].ToString() == locationIdToName[locationId].ToString())
                                    {
                                        foundfacility = true;
                                        indexfound = i;
                                        break;
                                    }

                                }
                                for (int i = indexfound; i < reportWeeklyDataTable.Rows.Count; i++)
                                {
                                    if (reportWeeklyDataTable.Rows[i][6].ToString() == week.ToString())
                                    {
                                        foundWeek = true;
                                        indexfound = i;
                                        break;
                                    }

                                }
                            }

                            if (!foundfacility)
                            {
                                reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], _startYear.ToString(), week.ToString(), _startMonth.ToString());
                                indexfound = reportWeeklyDataTable.Rows.Count - 1;
                            }

                            if (!foundWeek)
                            {
                                reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], _startYear.ToString(), week.ToString(), _startMonth.ToString());
                                indexfound = reportWeeklyDataTable.Rows.Count - 1;

                            }
                            if (indexfound != -999)
                            {


                                if (_repKind == 0)
                                {
                                    string opd = disease + " OPD Case", ipd = disease + " IPD Case", ipddeath = disease + " Death Case";
                                    reportWeeklyDataTable.Rows[indexfound][opd] = opd_case_pub_tot;
                                    reportWeeklyDataTable.Rows[indexfound][ipd] = ipd_case_pub_tot;
                                    reportWeeklyDataTable.Rows[indexfound][ipddeath] = ipd_death_pub_tot;

                                }
                                else if (_phemNational == 1)
                                {


                                    string disease_case_pub_tot = opd_case + "_" + week + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                                    string disease_death_pub_tot = ipd_case + "_" + week + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();

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
                    }
                    else
                    {
                        for (int year = _startYear; year <= _endYear; year++)
                            for (int week = 1; week <= 53; week++)
                            {
                                bool foundWeek = false;
                                if (year == 2008 && week == 34)
                                {
                                    string h = "2008";
                                }
                                // int week = Convert.ToInt16(row["week"].ToString());
                                string opd_case_pub_tot = opd_case + "_" + week + "_" + year + "_" + locationId; opd_case_pub_tot = (aggregateDataHash[opd_case_pub_tot] == null) ? "" : aggregateDataHash[opd_case_pub_tot].ToString();
                                string ipd_case_pub_tot = ipd_case + "_" + week + "_" + year + "_" + locationId; ipd_case_pub_tot = (aggregateDataHash[ipd_case_pub_tot] == null) ? "" : aggregateDataHash[ipd_case_pub_tot].ToString();
                                string ipd_death_pub_tot = ipd_death + "_" + week + "_" + year + "_" + locationId; ipd_death_pub_tot = (aggregateDataHash[ipd_death_pub_tot] == null) ? "" : aggregateDataHash[ipd_death_pub_tot].ToString();

                                // the first for the total

                                if (!opd_case_pub_tot.Equals("") || !ipd_case_pub_tot.Equals("") || !ipd_death_pub_tot.Equals(""))
                                {
                                    string hmiscodeF = locationIdToName[locationId].ToString();
                                    int regionId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(locationId);
                                    string regionName =
                                        Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(regionId.ToString()).Replace("Regional Health Bureau", "");
                                    int zoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(locationId);
                                    string zonename = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(zoneId.ToString()).Replace("Zonal Health Department", "");
                                    int woredaId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getWoredaId(locationId);
                                    string woredaname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(woredaId.ToString()).Replace("Woreda Health Office", "");
                                    int districtid = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getHCId(locationId);
                                    string hcname = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(districtid.ToString());
                                    if (regionName == "Federal Ministry of Health")
                                        regionName = "FMOH";

                                    if (reportWeeklyDataTable.Rows.Count == 0)
                                    {

                                        reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], year.ToString(), week.ToString(), _startMonth.ToString());
                                        foundfacility = true;
                                        foundWeek = true;
                                        indexfound = 0;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < reportWeeklyDataTable.Rows.Count; i++)
                                        {
                                            if (reportWeeklyDataTable.Rows[i][4].ToString() == locationIdToName[locationId].ToString())
                                            {
                                                foundfacility = true;
                                                indexfound = i;
                                                break;
                                            }

                                        }
                                        for (int i = indexfound; i < reportWeeklyDataTable.Rows.Count; i++)
                                        {
                                            if (reportWeeklyDataTable.Rows[i][6].ToString() == week.ToString() && reportWeeklyDataTable.Rows[i][5].ToString() == year.ToString())
                                            {
                                                foundWeek = true;
                                                indexfound = i;
                                                break;
                                            }

                                        }
                                    }

                                    if (!foundfacility)
                                    {
                                        reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], year.ToString(), week.ToString(), _startMonth.ToString());
                                        indexfound = reportWeeklyDataTable.Rows.Count - 1;
                                    }

                                    if (!foundWeek)
                                    {
                                        reportWeeklyDataTable.Rows.Add(regionName, zonename, woredaname, hcname, locationIdToName[locationId], year.ToString(), week.ToString(), _startMonth.ToString());
                                        indexfound = reportWeeklyDataTable.Rows.Count - 1;

                                    }
                                    if (indexfound != -999)
                                    {


                                        if (_repKind == 0)
                                        {
                                            string opd = disease + " OPD Case", ipd = disease + " IPD Case", ipddeath = disease + " Death Case";
                                            reportWeeklyDataTable.Rows[indexfound][opd] = opd_case_pub_tot;
                                            reportWeeklyDataTable.Rows[indexfound][ipd] = ipd_case_pub_tot;
                                            reportWeeklyDataTable.Rows[indexfound][ipddeath] = ipd_death_pub_tot;

                                        }
                                        else if (_phemNational == 1)
                                        {

                                            string disease_case_pub_tot = opd_case + "_" + week + "_" + year + "_" + locationId; disease_case_pub_tot = (aggregateDataHash[disease_case_pub_tot] == null) ? "" : aggregateDataHash[disease_case_pub_tot].ToString();
                                            string disease_death_pub_tot = ipd_case + "_" + week + "_" + year + "_" + locationId; disease_death_pub_tot = (aggregateDataHash[disease_death_pub_tot] == null) ? "" : aggregateDataHash[disease_death_pub_tot].ToString();

                                            string caseIm = disease + " Case", deathIm = disease + " Death";

                                            reportWeeklyDataTable.Rows[indexfound][caseIm] = disease_case_pub_tot;
                                            reportWeeklyDataTable.Rows[indexfound][deathIm] = disease_death_pub_tot;
                                        }
                                    }
                                    // Then for each month, only if single facility and Monthly report

                                    id = "";


                                }
                            }

                    }
                }

                // reportOpdDataTable.Rows.Add("---------", "---------", "---------", "---------", "---------", "---------", "---------", "---------");

            }
        }
        private void InsertAggregateData(string id, string disease, string opd_case, string ipd_case,
            string ipd_death, int OldCode)
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
        private void InsertAggregateData(string id, string disease, string disease_case, string disease_death, int oldcode)
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
            if (_repKind == 1 && _repPeriod == 2)
            {
                int index = 0;
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["DataEleClass"].ToString().Trim() == "29")// && row["Case / Linelist"].ToString().Equals(""))
                        {
                            dt.Rows[index]["DataEleClass"] = "Linelist";
                        }
                        else if (row["DataEleClass"].ToString().Trim() == "28")
                        {
                            dt.Rows[index]["DataEleClass"] = "Casebased";
                        }

                        if (row["IsSpecimenTaken"].ToString().Trim().Equals("Yes") || row["IsSpecimenTaken"].ToString().Trim().Equals("No"))
                        {
                        }
                        else if (!row["IsSpecimenTaken"].ToString().Trim().Equals(""))
                        {
                            dt.Rows[index]["IsSpecimenTaken"] = "Yes";
                        }
                        if (row["LabResult"].ToString().Trim().Equals("Yes") || row["LabResult"].ToString().Trim().Equals("No"))
                        {
                        }
                        else if (!row["LabResult"].ToString().Trim().Equals(""))
                        {
                            dt.Rows[index]["LabResult"] = "Yes";
                        }
                        index++;
                    }

                }
                dt.Columns.Remove("LabelID");
            }

        }

    }
}


