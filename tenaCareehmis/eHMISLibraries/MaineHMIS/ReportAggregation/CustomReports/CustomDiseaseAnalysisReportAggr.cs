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
using System.Text.RegularExpressions;


namespace eHMIS.HMIS.ReportAggregation.CustomReports
{

    public class CustomDiseaseAnalysisReportAggr : ICustomReport
    {
        DBConnHelper _helper = new DBConnHelper();

        Hashtable aggregateDataHash = new Hashtable();
        Hashtable ethMonth = new Hashtable();

        List<string> locationsToView = new List<string>();
        Hashtable locationIdToName = new Hashtable();
        DataTable reportOpdDataTable = new DataTable();
        DataTable reportIpdDataTable = new DataTable();

        Boolean singleFacility = false;
        private volatile bool _shouldStop;
        string viewLabeIdTableName = "";

        private string singleFacilityLocationId = "";

        int _startMonth;
        int _endMonth;
        int _startYear;
        int _endYear;
        int _quarterStart;
        int _quarterEnd;
        int _repKind;
        int _repPeriod;

        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation

        string hmisCodesSelected = "";
        string sha1Hash = "";
        bool level1Cache = false;
        bool level2Cache = false;

        public CustomDiseaseAnalysisReportAggr(List<string> locations, int startMonth, int endMonth,
            int yearStart, int yearEnd, int quarterStart, int quarterEnd, int repKind,
            int repPeriod, bool isCacheEnabled)
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
            _endYear = yearStart;
            _repKind = repKind;
            _repPeriod = repPeriod;

            string timeFrame = "";

            if (repPeriod == 0) // Monthly
            {
                _startMonth = startMonth;
                _endMonth = endMonth;

                timeFrame = "M" + _startMonth.ToString() + ",M" +
                   _endMonth.ToString() + ",Y" + _startYear.ToString() + ",Y" + _endYear.ToString();
            }
            else if (repPeriod == 1) // Quarterly
            {
                _startYear = yearStart;
                _quarterStart = quarterStart;
                _quarterEnd = quarterEnd;
                setStartingMonth(_quarterStart, _quarterEnd);

                timeFrame = "Q" + _quarterStart.ToString() + ",Q" +
                   _quarterEnd.ToString() + ",Y" + _startYear.ToString() + ",Y" + _endYear.ToString();
            }
            else if (repPeriod == 2) // Yearly
            {
                _endYear = yearEnd;

                timeFrame = "Y" + _startYear.ToString() + ",Y" + _endYear.ToString();
            }

            locationsToView = locations;

            List<string> sortedLocations = new List<string>();
            sortedLocations = locations;
            sortedLocations.Sort();

            if (_repKind == 1) // OPD
            {
                timeFrame = timeFrame + "," + "OPD_Custom";
            }
            else
            {
                timeFrame = timeFrame + "," + "IPD_Custom";
            }

            hmisCodesSelected = string.Join(",", sortedLocations.ToArray()) + "," + timeFrame;
            sha1Hash = UtilitiesNew.GeneralUtilities.CryptorEngine.createSHA1Hash(hmisCodesSelected);

            if (repPeriod != 2)
            {
                DataRow rw;

                if ((rw = getCachedReport(sha1Hash)) != null)
                {
                    processCachedReport(rw);
                }
                else
                {
                    // Here first check with the database if level2cache tables are available
                    // and their has never been any imports...
                    string cmdText = " select level2Cache from CacheLevel2 ";
                    SqlCommand toExecute = new SqlCommand(cmdText);
                    object dbLevel2Cache = _helper.GetScalar(toExecute);
                    level2Cache = (bool)dbLevel2Cache;
                }
            }
            else
            {
                level1Cache = false;
                level2Cache = false;
            }

            if (isCacheEnabled == false)
            {
                level1Cache = false;
                level2Cache = false;
            }

            if (!level1Cache)
            {

                if (locationsToView.Count == 1)
                {
                    singleFacility = true; // Only for a single facility, thus show detail month to month data
                    // or detail Quarter to Quarter data, including aggregate
                    singleFacilityLocationId = locationsToView[0].ToString();

                    int aggregationLevel = getAggregationLevel(singleFacilityLocationId);

                    if (aggregationLevel == 4)
                    {
                        level2Cache = false;
                    }
                }
                if (repKind == 1) // OPD Disease
                {
                    viewLabeIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";

                    reportOpdDataTable = new DataTable();

                    reportOpdDataTable.Columns.Add("SNO", typeof(string));
                    reportOpdDataTable.Columns.Add("Disease_Facility", typeof(string));
                    reportOpdDataTable.Columns.Add("Male<=4", typeof(string));
                    reportOpdDataTable.Columns.Add("Male5To14", typeof(string));
                    reportOpdDataTable.Columns.Add("Male>=15", typeof(string));
                    reportOpdDataTable.Columns.Add("Female<=4", typeof(string));
                    reportOpdDataTable.Columns.Add("Female5To14", typeof(string));
                    reportOpdDataTable.Columns.Add("Female>=15", typeof(string));
                    reportOpdDataTable.Columns.Add("Format", typeof(string));
                }
                else // IPD Disease
                {
                    viewLabeIdTableName = "EthioHIMS_QuarterIPDDiseaseView";

                    reportIpdDataTable = new DataTable();

                    reportIpdDataTable.Columns.Add("SNO", typeof(string));
                    reportIpdDataTable.Columns.Add("Disease_Facility", typeof(string));
                    reportIpdDataTable.Columns.Add("Morbidity Male <=4", typeof(string));
                    reportIpdDataTable.Columns.Add("Morbidity Male 5To14", typeof(string));
                    reportIpdDataTable.Columns.Add("Morbidity Male >=15", typeof(string));
                    reportIpdDataTable.Columns.Add("Morbidity Female <=4", typeof(string));
                    reportIpdDataTable.Columns.Add("Morbidity Female 5To14", typeof(string));
                    reportIpdDataTable.Columns.Add("Morbidity Female >=15", typeof(string));
                    reportIpdDataTable.Columns.Add("Mortality Male <=4", typeof(string));
                    reportIpdDataTable.Columns.Add("Mortality Male 5To14", typeof(string));
                    reportIpdDataTable.Columns.Add("Mortality Male >=15", typeof(string));
                    reportIpdDataTable.Columns.Add("Mortality Female <=4", typeof(string));
                    reportIpdDataTable.Columns.Add("Mortality Female 5To14", typeof(string));
                    reportIpdDataTable.Columns.Add("Mortality Female >=15", typeof(string));
                    reportIpdDataTable.Columns.Add("Format", typeof(string));
                }
            }
        }

        private void setStartingMonth(int quarterStart, int quarterEnd)
        {
            switch (quarterStart)
            {
                case 1: _startMonth = 11;
                    //_endMonth = 1;
                    _startYear = _startYear - 1;
                    break;
                case 2: _startMonth = 2;
                    // _endMonth = 4;
                    break;
                case 3: _startMonth = 5;
                    // _endMonth = 7;
                    break;
                case 4: _startMonth = 8;
                    // _endMonth = 10;
                    break;
            }

            switch (quarterEnd)
            {
                case 1:
                    _endMonth = 1;
                    break;
                case 2:
                    _endMonth = 4;
                    break;
                case 3:
                    _endMonth = 7;
                    break;
                case 4:
                    _endMonth = 10;
                    break;
            }
        }

        private string getFacilityName(string locationID)
        {
            // Given the location ID returns the Facility Name
            string cmdText = "select facilityname from facility where hmiscode = @locationID";
            string facilityName = "";

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.CommandTimeout = 0; //300 // = 300; // = 1000000;

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

            toExecute.CommandTimeout = 0; //300 // = 1000000;
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
            string monthQuery = "";
            string monthQuery2 = "";
            aggregateDataHash.Clear();

            if (_repKind == 4)
            {
                dataEleClassQuery = " and (DataEleClass = 8 or DataEleClass = 2) ";
            }
            else if (_repKind == 5)
            {
                dataEleClassQuery = " and DataEleClass = 3 "; // Mortality
            }

            if ((_repPeriod == 1) && (_startMonth == 11)) // Quarterly Service Quarter 1
            {
                //monthYearQueryGroup1 = " where   (((Month  = @StartMonth or Month = @StartMonth + 1) and  " +
                // "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) ";
                //monthYearQueryGroup2 = " where  Month = @EndMonth and Year = @startYear ";
                int nextMonth = _startMonth + 1;
                monthQuery = "  where   (((Month  = " + _startMonth + "  or Month = " + nextMonth + ") and " +
                                       "  (Year = " + _startYear + ")) or (Month <= " + _endMonth + " and Year = " +
                                        _endYear + ")) ";

                monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + _endYear;

            }
            else if (_repPeriod == 1) // Quarterly Service
            {
                // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                monthQuery = "  where  Month  >=" + _startMonth + " and Month <= " +
                    _endMonth + "  and Year = " + _endYear;
                monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + _endYear;

            }
            else if (_repPeriod == 0)
            {
                if (_startMonth == 11)
                {
                    int prevYear = _startYear - 1;
                    if ((_endMonth == 11) || (_endMonth == 12))
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth +
                            " and Year = " + prevYear;
                        monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + prevYear;

                    }
                    else
                    {
                        monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                        " or ((Month  >= 1 and Month <= " + _endMonth + " ) and Year = " + _startYear + " )) ";
                        monthQuery2 = " where  Month =  " + _endMonth + " and Year = " + _startYear;
                    }
                }
                else if (_startMonth == 12)
                {
                    int prevYear = _startYear - 1;
                    if (_endMonth == 12)
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth +
                            " and Year = " + prevYear;
                        monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + prevYear;
                    }
                    else
                    {
                        monthQuery = "	where  (((Month = 12) and  Year = " + prevYear + " ) " +
                        " or ((Month  >= 1 and Month <= " + _endMonth + " ) and Year = " + _startYear + " )) ";
                        monthQuery2 = " where  Month =  " + _endMonth + " and Year = " + _startYear;
                    }
                }
                else
                {
                    monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _startYear;
                    monthQuery2 = " where  Month =  " + _endMonth + "  and Year =  " + _startYear;
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

            toExecute.CommandTimeout = 0; //300 // = 1000000;
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                string LabelID = row["labelID"].ToString();
                int value = Convert.ToInt32(row["value"]);
                aggregateDataHash.Add(LabelID, value);
            }
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        public DataTable CreateReport()
        {

            string cmdText = " select case when EthEhmis_HmisValue.DataEleClass = 2 then 'IPD Cases'  " +
                             " when EthEhmis_HmisValue.dataeleclass = 8 then 'OPD Cases' end as CaseType, " +
                             " Disease,gender, age, RegionId, Year, month, sum(value) from EthEhmis_HmisValue " +
                             " inner join v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Age " +
                             " on v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Age.DataEleClass = " +
                             " EthEhmis_HMISValue.DataEleClass " +
                             " and " +
                             " v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Age.LabelId = EthEhmis_HMISValue.LabelId " +
                             " where (EthEhmis_HmisValue.dataeleclass = 2 or EthEhmis_HmisValue.dataeleClass = 8) " +
                             " and value != 0 " +
                             " group by EthEhmis_HmisValue.DataEleClass, regionId, Disease, gender, age, year, month ";

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);

            toExecute.CommandTimeout = 0; //300 // = 1000000;

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            return dt;
        }

        private void SaveToCache(DataTable reportServiceDataTable, string hmisCodesSelected, string sha1Hash)
        {
            StringBuilder reportColumns = new StringBuilder();

            int count = 0;
            foreach (DataColumn reportColumn in reportServiceDataTable.Columns)
            {
                count++;

                reportColumns.Append(reportColumn.ColumnName);
                if (reportServiceDataTable.Columns.Count != count)
                {
                    reportColumns.Append("|||");
                }
            }
            reportColumns.Append("\n");

            count = 0;

            foreach (DataRow row in reportServiceDataTable.Rows)
            {
                count = 0;
                foreach (object obj in row.ItemArray)
                {
                    count++;

                    reportColumns.Append(obj.ToString());
                    if (row.ItemArray.Length != count)
                    {
                        reportColumns.Append("|||");
                    }
                }
                reportColumns.Append("\n");
            }

            string dataString = reportColumns.ToString();

            saveToDb(dataString, hmisCodesSelected, sha1Hash);

            //string.Join(", ", Array.ConvertAll(reportServiceDataTable.Columns, 
        }

        private void saveToDb(string dataString, string hmisCodesSelected, string sha1Hash)
        {
            string cmdText = " select * from EthEhmis_Report where hashKey = @sha1Hash ";
            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("sha1Hash", sha1Hash);

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            if (dt.Rows.Count == 0)
            {

                cmdText =
                         " insert into  EthEhmis_Report (hashKey, hmisCodesSelected, tableData, DateGenerated) \n  " +
                          " Values (@sha1Hash, @hmisCodesSelected, @tableData, @DateGenerated)";
            }
            else
            {
                cmdText =
                " update  EthEhmis_Report set hashKey = @sha1Hash,  " +
                " hmisCodesSelected =  @hmisCodesSelected, tableData = @tableData, DateGenerated = @DateGenerated where hashKey = @sha1Hash \n  ";

            }

            toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("sha1Hash", sha1Hash);
            toExecute.Parameters.AddWithValue("hmisCodesSelected", hmisCodesSelected);
            toExecute.Parameters.AddWithValue("tableData", dataString);
            toExecute.Parameters.AddWithValue("DateGenerated", DateTime.Now);

            // check if data is saved and do an update

            _helper.Execute(toExecute);
        }


        private void calculateFacilityTototal(DataTable dt, StringBuilder sb)
        {
            int noFacilitiesSelected = locationsToView.Count;

            if (_repKind == 1) // OPD # columns for OPD and IPD is different since IPD has mortality
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if ((dt.Rows[i]["SNO"].ToString().Trim() != "") && (dt.Rows[i]["format"].ToString() != "1"))
                    {
                        int rowPosion = i;

                        int FacilityTotal1 = 0;
                        int FacilityTotal2 = 0;
                        int FacilityTotal3 = 0;
                        int FacilityTotal4 = 0;
                        int FacilityTotal5 = 0;
                        int FacilityTotal6 = 0;
                        bool flag1 = false;
                        bool flag2 = false;
                        bool flag3 = false;
                        bool flag4 = false;
                        bool flag5 = false;
                        bool flag6 = false;


                        for (int j = rowPosion; j <= (rowPosion + noFacilitiesSelected); j++)
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
                            if (DBNull.Value != dt.Rows[j][4] && dt.Rows[j][4].ToString() != "")
                            {
                                FacilityTotal3 = FacilityTotal3 + Convert.ToInt32(dt.Rows[j][4]);
                                flag3 = true;
                            }
                            if (DBNull.Value != dt.Rows[j][5] && dt.Rows[j][5].ToString() != "")
                            {
                                FacilityTotal4 = FacilityTotal4 + Convert.ToInt32(dt.Rows[j][5]);
                                flag4 = true;
                            }
                            if (DBNull.Value != dt.Rows[j][6] && dt.Rows[j][6].ToString() != "")
                            {
                                FacilityTotal5 = FacilityTotal5 + Convert.ToInt32(dt.Rows[j][6]);
                                flag5 = true;
                            }
                            if (DBNull.Value != dt.Rows[j][7] && dt.Rows[j][7].ToString() != "")
                            {
                                FacilityTotal6 = FacilityTotal6 + Convert.ToInt32(dt.Rows[j][7]);
                                flag6 = true;
                            }
                        }


                        //dt.Rows[i]["SNO"] = FacilityTotal.ToString();

                        // Merra Kokebie, merraK@tutape.org, Mar. 10, 2012
                        // Solution for the total showing 0 when there is no data
                        // 
                        //dt.Rows[i][2] = FacilityTotal1.ToString();
                        //dt.Rows[i][3] = FacilityTotal2.ToString();
                        //dt.Rows[i][4] = FacilityTotal3.ToString();
                        //dt.Rows[i][5] = FacilityTotal4.ToString();
                        //dt.Rows[i][6] = FacilityTotal5.ToString();
                        //dt.Rows[i][7] = FacilityTotal6.ToString();

                        if (flag1 == true)
                        {
                            dt.Rows[i][2] = FacilityTotal1.ToString();
                        }
                        else
                        {
                            dt.Rows[i][2] = "";
                        }

                        if (flag2 == true)
                        {
                            dt.Rows[i][3] = FacilityTotal2.ToString();
                        }
                        else
                        {
                            dt.Rows[i][3] = "";
                        }

                        if (flag3 == true)
                        {
                            dt.Rows[i][4] = FacilityTotal3.ToString();
                        }
                        else
                        {
                            dt.Rows[i][4] = "";
                        }

                        if (flag4 == true)
                        {
                            dt.Rows[i][5] = FacilityTotal4.ToString();
                        }
                        else
                        {
                            dt.Rows[i][5] = "";
                        }

                        if (flag5 == true)
                        {
                            dt.Rows[i][6] = FacilityTotal5.ToString();
                        }
                        else
                        {
                            dt.Rows[i][6] = "";
                        }

                        if (flag6 == true)
                        {
                            dt.Rows[i][7] = FacilityTotal6.ToString();
                        }
                        else
                        {
                            dt.Rows[i][7] = "";
                        }
                    }
                }
            }
            else if (_repKind == 2)
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if ((dt.Rows[i]["SNO"].ToString().Trim() != "") && (dt.Rows[i]["format"].ToString() != "1"))
                    {
                        int rowPosion = i;

                        int FacilityTotal1 = 0;
                        int FacilityTotal2 = 0;
                        int FacilityTotal3 = 0;
                        int FacilityTotal4 = 0;
                        int FacilityTotal5 = 0;
                        int FacilityTotal6 = 0;
                        int FacilityTotal7 = 0;
                        int FacilityTotal8 = 0;
                        int FacilityTotal9 = 0;
                        int FacilityTotal10 = 0;
                        int FacilityTotal11 = 0;
                        int FacilityTotal12 = 0;

                        bool flag1 = false;
                        bool flag2 = false;
                        bool flag3 = false;
                        bool flag4 = false;
                        bool flag5 = false;
                        bool flag6 = false;
                        bool flag7 = false;
                        bool flag8 = false;
                        bool flag9 = false;
                        bool flag10 = false;
                        bool flag11 = false;
                        bool flag12 = false;


                        try
                        {
                            for (int j = rowPosion; j <= (rowPosion + noFacilitiesSelected); j++)
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
                                if (DBNull.Value != dt.Rows[j][4] && dt.Rows[j][4].ToString() != "")
                                {
                                    FacilityTotal3 = FacilityTotal3 + Convert.ToInt32(dt.Rows[j][4]);
                                    flag3 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][5] && dt.Rows[j][5].ToString() != "")
                                {
                                    FacilityTotal4 = FacilityTotal4 + Convert.ToInt32(dt.Rows[j][5]);
                                    flag4 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][6] && dt.Rows[j][6].ToString() != "")
                                {
                                    FacilityTotal5 = FacilityTotal5 + Convert.ToInt32(dt.Rows[j][6]);
                                    flag5 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][7] && dt.Rows[j][7].ToString() != "")
                                {
                                    FacilityTotal6 = FacilityTotal6 + Convert.ToInt32(dt.Rows[j][7]);
                                    flag6 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][8] && dt.Rows[j][8].ToString() != "")
                                {
                                    FacilityTotal7 = FacilityTotal7 + Convert.ToInt32(dt.Rows[j][8]);
                                    flag7 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][9] && dt.Rows[j][9].ToString() != "")
                                {
                                    FacilityTotal8 = FacilityTotal8 + Convert.ToInt32(dt.Rows[j][9]);
                                    flag8 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][10] && dt.Rows[j][10].ToString() != "")
                                {
                                    FacilityTotal9 = FacilityTotal9 + Convert.ToInt32(dt.Rows[j][10]);
                                    flag9 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][11] && dt.Rows[j][11].ToString() != "")
                                {
                                    FacilityTotal10 = FacilityTotal10 + Convert.ToInt32(dt.Rows[j][11]);
                                    flag10 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][12] && dt.Rows[j][12].ToString() != "")
                                {
                                    FacilityTotal11 = FacilityTotal11 + Convert.ToInt32(dt.Rows[j][12]);
                                    flag11 = true;
                                }
                                if (DBNull.Value != dt.Rows[j][13] && dt.Rows[j][13].ToString() != "")
                                {
                                    FacilityTotal12 = FacilityTotal12 + Convert.ToInt32(dt.Rows[j][13]);
                                    flag12 = true;
                                }
                            }
                        }
                        catch
                        {
                        }

                        // Merra Kokebie, merraK@tutape.org, Mar. 10, 2012
                        // Solution for the total showing 0 when there is no data
                        // 
                        //dt.Rows[i]["SNO"] = FacilityTotal.ToString();

                        //dt.Rows[i][2] = FacilityTotal1.ToString();
                        //dt.Rows[i][3] = FacilityTotal2.ToString();
                        //dt.Rows[i][4] = FacilityTotal3.ToString();
                        //dt.Rows[i][5] = FacilityTotal4.ToString();
                        //dt.Rows[i][6] = FacilityTotal5.ToString();
                        //dt.Rows[i][7] = FacilityTotal6.ToString();

                        //dt.Rows[i][8] = FacilityTotal7.ToString();
                        //dt.Rows[i][9] = FacilityTotal8.ToString();
                        //dt.Rows[i][10] = FacilityTotal9.ToString();
                        //dt.Rows[i][11] = FacilityTotal10.ToString();
                        //dt.Rows[i][12] = FacilityTotal11.ToString();
                        //dt.Rows[i][13] = FacilityTotal12.ToString();

                        if (flag1 == true)
                        {
                            dt.Rows[i][2] = FacilityTotal1.ToString();
                        }
                        else
                        {
                            dt.Rows[i][2] = "";
                        }

                        if (flag2 == true)
                        {
                            dt.Rows[i][3] = FacilityTotal2.ToString();
                        }
                        else
                        {
                            dt.Rows[i][3] = "";
                        }

                        if (flag3 == true)
                        {
                            dt.Rows[i][4] = FacilityTotal3.ToString();
                        }
                        else
                        {
                            dt.Rows[i][4] = "";
                        }

                        if (flag4 == true)
                        {
                            dt.Rows[i][5] = FacilityTotal4.ToString();
                        }
                        else
                        {
                            dt.Rows[i][5] = "";
                        }

                        if (flag5 == true)
                        {
                            dt.Rows[i][6] = FacilityTotal5.ToString();
                        }
                        else
                        {
                            dt.Rows[i][6] = "";
                        }

                        if (flag6 == true)
                        {
                            dt.Rows[i][7] = FacilityTotal6.ToString();
                        }
                        else
                        {
                            dt.Rows[i][7] = "";
                        }

                        if (flag7 == true)
                        {
                            dt.Rows[i][8] = FacilityTotal7.ToString();
                        }
                        else
                        {
                            dt.Rows[i][8] = "";
                        }

                        if (flag8 == true)
                        {
                            dt.Rows[i][9] = FacilityTotal8.ToString();
                        }
                        else
                        {
                            dt.Rows[i][9] = "";
                        }

                        if (flag9 == true)
                        {
                            dt.Rows[i][10] = FacilityTotal9.ToString();
                        }
                        else
                        {
                            dt.Rows[i][10] = "";
                        }

                        if (flag10 == true)
                        {
                            dt.Rows[i][11] = FacilityTotal10.ToString();
                        }
                        else
                        {
                            dt.Rows[i][11] = "";
                        }

                        if (flag11 == true)
                        {
                            dt.Rows[i][12] = FacilityTotal11.ToString();
                        }
                        else
                        {
                            dt.Rows[i][12] = "";
                        }

                        if (flag12 == true)
                        {
                            dt.Rows[i][13] = FacilityTotal12.ToString();
                        }
                        else
                        {
                            dt.Rows[i][13] = "";
                        }
                    }
                }
            }



        }

        // OPD Disease Report (Main one for higher Admin Levels)
        private void InsertAggregateData(string sno, string disease, string m04, string m514,
            string m15, string f04, string f514, string f15, string format)
        {
            if (m04 == "")
            {
                m04 = "";
                m514 = "";
                m15 = "";
                f04 = "";
                f514 = "";
                f15 = "";

                reportOpdDataTable.Rows.Add(sno, disease, m04, m514, m15, f04, f514, f15, format);
            }
            else
            {
                string replacement = "";
                replacement = "";

                // 1). The first Entry with the Disease name total

                disease = disease.Replace("q`ly total", replacement);


                string m04Tot = "";
                string m514Tot = "";
                string m15Tot = "";
                string f04Tot = "";
                string f514Tot = "";
                string f15Tot = "";

                reportOpdDataTable.Rows.Add(sno, disease, m04Tot, m514Tot, m15Tot, f04Tot, f514Tot, f15Tot, format);

                // 2). The Second Entry Loops through all the facilities

                foreach (string locationId in locationsToView)
                {
                    sno = "";

                    //int aggregationType = getAggregationLevel(locationId);

                    //if (aggregationType == 4) // Facility level
                    //{
                    //    level2Cache = false;
                    //}

                    if (singleFacility == true)
                    {
                        disease = "Total: ";
                    }
                    else
                    {
                        disease = "   " + locationIdToName[locationId];
                    }

                    string m04PubTot = m04 + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                    string m514PubTot = m514 + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                    string m15PubTot = m15 + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                    string f04PubTot = f04 + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                    string f514PubTot = f514 + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                    string f15PubTot = f15 + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();

                    // the first for the total
                    reportOpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, format);


                    // Then for each month, only if single facility and Monthly report

                    if ((singleFacility == true) && (_repPeriod == 0)) // Monthly Single Facility
                    {
                        ArrayList monthIdLists = new ArrayList();

                        if (_startMonth == 11)
                        {
                            monthIdLists.Add(_startMonth);

                            if (_endMonth != 11)
                            {
                                if (_endMonth == 12)
                                {
                                    monthIdLists.Add(_endMonth);
                                }
                                else
                                {
                                    monthIdLists.Add(12);
                                    for (int i = 1; i <= _endMonth; i++)
                                    {
                                        monthIdLists.Add(i);
                                    }
                                }
                            }
                        }
                        else if (_startMonth == 12)
                        {
                            monthIdLists.Add(_startMonth);

                            if (_endMonth != 12)
                            {
                                for (int i = 1; i <= _endMonth; i++)
                                {
                                    monthIdLists.Add(i);
                                }
                            }
                        }
                        else
                        {
                            for (int i = _startMonth; i <= _endMonth; i++)
                            {
                                monthIdLists.Add(i);
                            }
                        }

                        //for (int i = _startMonth; i <= _endMonth; i++)
                        foreach (int i in monthIdLists)
                        {
                            disease = ethMonth[i].ToString();
                            sno = "";

                            m04PubTot = m04 + "_" + ethMonth[i] + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                            m514PubTot = m514 + "_" + ethMonth[i] + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                            m15PubTot = m15 + "_" + ethMonth[i] + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                            f04PubTot = f04 + "_" + ethMonth[i] + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                            f514PubTot = f514 + "_" + ethMonth[i] + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                            f15PubTot = f15 + "_" + ethMonth[i] + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                            reportOpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, format);
                        }
                    }
                    else if ((singleFacility == true) && (_repPeriod == 1)) // Quarterly Single Facility
                    {
                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            string theQuarter = "quarter" + i;
                            disease = "Quarter  " + i + ":";
                            sno = "";

                            m04PubTot = m04 + "_" + theQuarter + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                            m514PubTot = m514 + "_" + theQuarter + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                            m15PubTot = m15 + "_" + theQuarter + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                            f04PubTot = f04 + "_" + theQuarter + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                            f514PubTot = f514 + "_" + theQuarter + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                            f15PubTot = f15 + "_" + theQuarter + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                            reportOpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, format);

                        }
                    }
                    else if ((singleFacility == true) && (_repPeriod == 2)) // Yearly Single Facility
                    {
                        for (int i = _startYear; i <= _endYear; i++)
                        {
                            string theYear = "year" + i;
                            disease = "Year  " + i + ":";
                            sno = "";

                            m04PubTot = m04 + "_" + theYear + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                            m514PubTot = m514 + "_" + theYear + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                            m15PubTot = m15 + "_" + theYear + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                            f04PubTot = f04 + "_" + theYear + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                            f514PubTot = f514 + "_" + theYear + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                            f15PubTot = f15 + "_" + theYear + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                            reportOpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, format);

                        }
                    }
                }

                // reportOpdDataTable.Rows.Add("---------", "---------", "---------", "---------", "---------", "---------", "---------", "---------");

            }
        }

        // For IPD Disease Report
        private void InsertAggregateData(string sno, string disease, string m04, string m514,
           string m15, string f04, string f514, string f15, string mm04, string mm514,
            string mm15, string mf04, string mf514, string mf15, string format)
        {
            if (m04 == "")
            {
                m04 = "";
                m514 = "";
                m15 = "";
                f04 = "";
                f514 = "";
                f15 = "";
                mm04 = "";
                mm514 = "";
                mm15 = "";
                mf04 = "";
                mf514 = "";
                mf15 = "";

                reportIpdDataTable.Rows.Add(sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format);

            }
            else
            {
                string replacement = "";


                // 1). The first Entry with the Disease name total

                disease = disease.Replace("q`ly total", replacement);

                string m04Tot = "";
                string m514Tot = "";
                string m15Tot = "";
                string f04Tot = "";
                string f514Tot = "";
                string f15Tot = "";
                string mm04Tot = "";
                string mm514Tot = "";
                string mm15Tot = "";
                string mf04Tot = "";
                string mf514Tot = "";
                string mf15Tot = "";

                reportIpdDataTable.Rows.Add(sno, disease, m04Tot, m514Tot, m15Tot, f04Tot, f514Tot, f15Tot, mm04Tot, mm514Tot, mm15Tot, mf04Tot, mf514Tot, mf15Tot, format);

                // 2). The Second Entry Loops through all the Health Facilities

                foreach (string locationId in locationsToView)
                {
                    sno = "";

                    if (singleFacility == true)
                    {
                        disease = "Total: ";
                    }
                    else
                    {
                        disease = "   " + locationIdToName[locationId];
                    }

                    string m04PubTot = m04 + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                    string m514PubTot = m514 + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                    string m15PubTot = m15 + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                    string f04PubTot = f04 + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                    string f514PubTot = f514 + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                    string f15PubTot = f15 + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                    string mm04PubTot = mm04 + "MM_" + locationId; mm04PubTot = (aggregateDataHash[mm04PubTot] == null) ? "" : aggregateDataHash[mm04PubTot].ToString();
                    string mm514PubTot = mm514 + "MM_" + locationId; mm514PubTot = (aggregateDataHash[mm514PubTot] == null) ? "" : aggregateDataHash[mm514PubTot].ToString();
                    string mm15PubTot = mm15 + "MM_" + locationId; mm15PubTot = (aggregateDataHash[mm15PubTot] == null) ? "" : aggregateDataHash[mm15PubTot].ToString();
                    string mf04PubTot = mf04 + "MM_" + locationId; mf04PubTot = (aggregateDataHash[mf04PubTot] == null) ? "" : aggregateDataHash[mf04PubTot].ToString();
                    string mf514PubTot = mf514 + "MM_" + locationId; mf514PubTot = (aggregateDataHash[mf514PubTot] == null) ? "" : aggregateDataHash[mf514PubTot].ToString();
                    string mf15PubTot = mf15 + "MM_" + locationId; mf15PubTot = (aggregateDataHash[mf15PubTot] == null) ? "" : aggregateDataHash[mf15PubTot].ToString();

                    reportIpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, mm04PubTot, mm514PubTot, mm15PubTot, mf04PubTot, mf514PubTot, mf15PubTot, format);

                    if ((singleFacility == true) && (_repPeriod == 0)) // Monthly Single Facility
                    {
                        ArrayList monthIdLists = new ArrayList();

                        if (_startMonth == 11)
                        {
                            monthIdLists.Add(_startMonth);

                            if (_endMonth != 11)
                            {
                                if (_endMonth == 12)
                                {
                                    monthIdLists.Add(_endMonth);
                                }
                                else
                                {
                                    monthIdLists.Add(12);
                                    for (int i = 1; i <= _endMonth; i++)
                                    {
                                        monthIdLists.Add(i);
                                    }
                                }
                            }
                        }
                        else if (_startMonth == 12)
                        {
                            monthIdLists.Add(_startMonth);

                            if (_endMonth != 12)
                            {
                                for (int i = 1; i <= _endMonth; i++)
                                {
                                    monthIdLists.Add(i);
                                }
                            }
                        }
                        else
                        {
                            for (int i = _startMonth; i <= _endMonth; i++)
                            {
                                monthIdLists.Add(i);
                            }
                        }

                        //for (int i = _startMonth; i <= _endMonth; i++)
                        foreach (int i in monthIdLists)
                        {
                            disease = ethMonth[i].ToString();
                            sno = "";

                            m04PubTot = m04 + "_" + ethMonth[i] + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                            m514PubTot = m514 + "_" + ethMonth[i] + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                            m15PubTot = m15 + "_" + ethMonth[i] + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                            f04PubTot = f04 + "_" + ethMonth[i] + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                            f514PubTot = f514 + "_" + ethMonth[i] + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                            f15PubTot = f15 + "_" + ethMonth[i] + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                            mm04PubTot = mm04 + "MM_" + ethMonth[i] + "_" + locationId; mm04PubTot = (aggregateDataHash[mm04PubTot] == null) ? "" : aggregateDataHash[mm04PubTot].ToString();
                            mm514PubTot = mm514 + "MM_" + ethMonth[i] + "_" + locationId; mm514PubTot = (aggregateDataHash[mm514PubTot] == null) ? "" : aggregateDataHash[mm514PubTot].ToString();
                            mm15PubTot = mm15 + "MM_" + ethMonth[i] + "_" + locationId; mm15PubTot = (aggregateDataHash[mm15PubTot] == null) ? "" : aggregateDataHash[mm15PubTot].ToString();
                            mf04PubTot = mf04 + "MM_" + ethMonth[i] + "_" + locationId; mf04PubTot = (aggregateDataHash[mf04PubTot] == null) ? "" : aggregateDataHash[mf04PubTot].ToString();
                            mf514PubTot = mf514 + "MM_" + ethMonth[i] + "_" + locationId; mf514PubTot = (aggregateDataHash[mf514PubTot] == null) ? "" : aggregateDataHash[mf514PubTot].ToString();
                            mf15PubTot = mf15 + "MM_" + ethMonth[i] + "_" + locationId; mf15PubTot = (aggregateDataHash[mf15PubTot] == null) ? "" : aggregateDataHash[mf15PubTot].ToString();

                            reportIpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, mm04PubTot, mm514PubTot, mm15PubTot, mf04PubTot, mf514PubTot, mf15PubTot, format);
                        }
                    }
                    else if ((singleFacility == true) && (_repPeriod == 1)) // Quarterly Single Facility
                    {
                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            string theQuarter = "quarter" + i;
                            disease = "Quarter  " + i + ":";
                            sno = "";

                            m04PubTot = m04 + "_" + theQuarter + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                            m514PubTot = m514 + "_" + theQuarter + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                            m15PubTot = m15 + "_" + theQuarter + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                            f04PubTot = f04 + "_" + theQuarter + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                            f514PubTot = f514 + "_" + theQuarter + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                            f15PubTot = f15 + "_" + theQuarter + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                            mm04PubTot = mm04 + "MM_" + theQuarter + "_" + locationId; mm04PubTot = (aggregateDataHash[mm04PubTot] == null) ? "" : aggregateDataHash[mm04PubTot].ToString();
                            mm514PubTot = mm514 + "MM_" + theQuarter + "_" + locationId; mm514PubTot = (aggregateDataHash[mm514PubTot] == null) ? "" : aggregateDataHash[mm514PubTot].ToString();
                            mm15PubTot = mm15 + "MM_" + theQuarter + "_" + locationId; mm15PubTot = (aggregateDataHash[mm15PubTot] == null) ? "" : aggregateDataHash[mm15PubTot].ToString();
                            mf04PubTot = mf04 + "MM_" + theQuarter + "_" + locationId; mf04PubTot = (aggregateDataHash[mf04PubTot] == null) ? "" : aggregateDataHash[mf04PubTot].ToString();
                            mf514PubTot = mf514 + "MM_" + theQuarter + "_" + locationId; mf514PubTot = (aggregateDataHash[mf514PubTot] == null) ? "" : aggregateDataHash[mf514PubTot].ToString();
                            mf15PubTot = mf15 + "MM_" + theQuarter + "_" + locationId; mf15PubTot = (aggregateDataHash[mf15PubTot] == null) ? "" : aggregateDataHash[mf15PubTot].ToString();

                            reportIpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, mm04PubTot, mm514PubTot, mm15PubTot, mf04PubTot, mf514PubTot, mf15PubTot, format);

                        }
                    }
                    else if ((singleFacility == true) && (_repPeriod == 2)) // Yearly Single Facility
                    {
                        for (int i = _startYear; i <= _endYear; i++)
                        {
                            string theYear = "year" + i;
                            disease = "Year  " + i + ":";
                            sno = "";


                            m04PubTot = m04 + "_" + theYear + "_" + locationId; m04PubTot = (aggregateDataHash[m04PubTot] == null) ? "" : aggregateDataHash[m04PubTot].ToString();
                            m514PubTot = m514 + "_" + theYear + "_" + locationId; m514PubTot = (aggregateDataHash[m514PubTot] == null) ? "" : aggregateDataHash[m514PubTot].ToString();
                            m15PubTot = m15 + "_" + theYear + "_" + locationId; m15PubTot = (aggregateDataHash[m15PubTot] == null) ? "" : aggregateDataHash[m15PubTot].ToString();
                            f04PubTot = f04 + "_" + theYear + "_" + locationId; f04PubTot = (aggregateDataHash[f04PubTot] == null) ? "" : aggregateDataHash[f04PubTot].ToString();
                            f514PubTot = f514 + "_" + theYear + "_" + locationId; f514PubTot = (aggregateDataHash[f514PubTot] == null) ? "" : aggregateDataHash[f514PubTot].ToString();
                            f15PubTot = f15 + "_" + theYear + "_" + locationId; f15PubTot = (aggregateDataHash[f15PubTot] == null) ? "" : aggregateDataHash[f15PubTot].ToString();
                            mm04PubTot = mm04 + "MM_" + theYear + "_" + locationId; mm04PubTot = (aggregateDataHash[mm04PubTot] == null) ? "" : aggregateDataHash[mm04PubTot].ToString();
                            mm514PubTot = mm514 + "MM_" + theYear + "_" + locationId; mm514PubTot = (aggregateDataHash[mm514PubTot] == null) ? "" : aggregateDataHash[mm514PubTot].ToString();
                            mm15PubTot = mm15 + "MM_" + theYear + "_" + locationId; mm15PubTot = (aggregateDataHash[mm15PubTot] == null) ? "" : aggregateDataHash[mm15PubTot].ToString();
                            mf04PubTot = mf04 + "MM_" + theYear + "_" + locationId; mf04PubTot = (aggregateDataHash[mf04PubTot] == null) ? "" : aggregateDataHash[mf04PubTot].ToString();
                            mf514PubTot = mf514 + "MM_" + theYear + "_" + locationId; mf514PubTot = (aggregateDataHash[mf514PubTot] == null) ? "" : aggregateDataHash[mf514PubTot].ToString();
                            mf15PubTot = mf15 + "MM_" + theYear + "_" + locationId; mf15PubTot = (aggregateDataHash[mf15PubTot] == null) ? "" : aggregateDataHash[mf15PubTot].ToString();

                            reportIpdDataTable.Rows.Add(sno, disease, m04PubTot, m514PubTot, m15PubTot, f04PubTot, f514PubTot, f15PubTot, mm04PubTot, mm514PubTot, mm15PubTot, mf04PubTot, mf514PubTot, mf15PubTot, format);

                        }
                    }
                }

                //reportIpdDataTable.Rows.Add("-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------", "-------");

            }
        }

        private void processCachedReport(DataRow rw)
        {
            DataTable reportDt = new DataTable();

            string[] reportLines = rw["tableData"].ToString().Split('\n');


            string[] columns = Regex.Split(reportLines[0], "[|][|][|]");

            foreach (string col in columns)
            {
                reportDt.Columns.Add(col);
            }

            for (int i = 1; i < reportLines.Length; i++)
            {
                string[] lineToProcess = Regex.Split(reportLines[i], "[|][|][|]");

                reportDt.Rows.Add(lineToProcess);
            }

            if (_repKind == 1) // OPD Disease
            {
                reportOpdDataTable = reportDt;
            }
            else if (_repKind == 2) // IPD Disease
            {
                reportIpdDataTable = reportDt;
            }

        }

        private DataRow getCachedReport(string sha1Hash)
        {
            string cmdText = " select * from EthEhmis_Report where hashKey = @hashKey";

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("hashKey", sha1Hash);


            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            if (dt.Rows.Count == 0)
            {
                level1Cache = false;
                return null;
            }
            else
            {
                level1Cache = true;
                return dt.Rows[0];
            }
        }

        private void updateHashTableLevel2Cache()
        {
            level2Cache = true;

            string idQuery = "";
            string monthQuery = "";
            string monthQuery2 = "";
            string cacheFileName = "";
            string cacheAdminType = "";
            string cachePeriodType = "";
            string dataEleClassQuery = "";

            string labelId2 = "";

            if (_repKind == 1) // OPD
            {
                dataEleClassQuery = " and (dataEleClass = 8) ";
            }
            else if (_repKind == 2) // IPD
            {
                dataEleClassQuery = " and (dataEleClass = 2 or dataEleClass = 3) ";
            }

            if (singleFacility == true)
            {
                ArrayList periodList = new ArrayList();

                if (_repPeriod == 0)
                {

                    if (_startMonth == 11)
                    {
                        periodList.Add(_startMonth);

                        if (_endMonth != 11)
                        {
                            if (_endMonth == 12)
                            {
                                periodList.Add(_endMonth);
                            }
                            else
                            {
                                periodList.Add(12);
                                for (int i = 1; i <= _endMonth; i++)
                                {
                                    periodList.Add(i);
                                }
                            }
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        periodList.Add(_startMonth);

                        if (_endMonth != 12)
                        {
                            for (int i = 1; i <= _endMonth; i++)
                            {
                                periodList.Add(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = _startMonth; i <= _endMonth; i++)
                        {
                            periodList.Add(i);
                        }
                    }
                }
                else
                {
                    for (int i = _quarterStart; i <= _quarterEnd; i++)
                    {
                        periodList.Add(i);
                    }
                }

                //for (int i = _startMonth; i <= _endMonth; i++)
                foreach (int i in periodList)
                {
                    int currentPeriod = i;
                    string theQuarter = "";

                    int aggregationLevel = getAggregationLevel(singleFacilityLocationId);

                    int yearToUse = 0;

                    if (_repPeriod == 1)
                    {
                        setStartingMonth(i, i);
                        theQuarter = "quarter" + i;
                        yearToUse = _endYear;

                        //monthQuery = " where  quarter = " + i + " and Year =  " + yearToUse;
                        //cachePeriodType = "Quarter";
                        if (i == 1) // Quarter 1
                        {
                            int prevYear = _endYear - 1;

                            monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                            " or (Month  = 1 and Year = " + _endYear + " )) ";
                            monthQuery2 = " where  Month =  " + _endMonth + " and Year = " + _endYear;

                        }
                        else
                        {
                            monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _endYear;
                            monthQuery2 = " where  Month =  " + _endMonth + "  and Year =  " + _endYear;
                        }

                        cachePeriodType = "Month";
                    }
                    else if (_repPeriod == 0)
                    {

                        if ((i == 11) || (i == 12))
                        {
                            yearToUse = _startYear - 1;
                        }
                        else
                        {
                            yearToUse = _startYear;
                        }

                        monthQuery = " where  Month =  " + i + " and Year =  " + yearToUse;
                        cachePeriodType = "Month";
                    }


                    if (aggregationLevel == 1)
                    {
                        idQuery = "";
                        cacheAdminType = "Region";
                    }
                    else if (aggregationLevel == 2)
                    {
                        cacheAdminType = "Region";
                    }
                    else if (aggregationLevel == 5)
                    {
                        cacheAdminType = "Zone";
                    }
                    else if (aggregationLevel == 3)
                    {
                        cacheAdminType = "Woreda";
                    }

                    //cacheFileName = "aa" + cacheAdminType + "Level2Cache" + cachePeriodType;
                    cacheFileName = "CacheLevel2" + cacheAdminType + cachePeriodType;

                    if (aggregationLevel != 1)
                    {
                        idQuery = " and " + cacheFileName + ".id  =  " + singleFacilityLocationId;
                    }

                    //string cmdText = "select * from " + cacheFileName + " \ninner join " +
                    //    viewLabeIdTableName + " on " + viewLabeIdTableName + ".LabelId = " +
                    //    cacheFileName + ".LabelID \n"
                    //    + monthQuery + idQuery + dataEleClassQuery;

                    string cmdText = "select * from " + cacheFileName +
                        monthQuery + idQuery + dataEleClassQuery;

                    SqlCommand toExecute = new SqlCommand(cmdText);

                    DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        string labelId1 = "";

                        int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                        string LabelID = row["LabelID"].ToString();
                        decimal value = Convert.ToDecimal(row["sumValue"].ToString());
                        string dEleClass = row["DataEleClass"].ToString();
                        string appendLabel = "";

                        if (dEleClass == "3") // Mortality
                        {
                            appendLabel = "MM";
                        }

                        string periodName = "";
                        if (_repPeriod == 0)
                        {
                            periodName = ethMonth[i].ToString();
                        }
                        else if (_repPeriod == 1)
                        {
                            periodName = theQuarter;
                        }

                        labelId1 = LabelID + appendLabel + "_" + periodName + "_" + singleFacilityLocationId;
                        labelId2 = LabelID + appendLabel + "_" + singleFacilityLocationId;

                        string HmisValue = value.ToString();

                        if (aggregateDataHash[labelId1] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                            newValue = newValue + value;
                            HmisValue = newValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                decimal decResult = Convert.ToDecimal(newValue);
                                int changeToInt = Convert.ToInt32(decResult);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash[labelId1] = HmisValue;
                        }
                        else
                        {
                            if (HmisValue.Contains(".00"))
                            {
                                decimal decResult = Convert.ToDecimal(value);
                                int changeToInt = Convert.ToInt32(decResult);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash.Add(labelId1, HmisValue);
                        }


                        if (aggregateDataHash[labelId2] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId2]);
                            newValue = newValue + value;
                            HmisValue = newValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                decimal decResult = Convert.ToDecimal(newValue);
                                int changeToInt = Convert.ToInt32(decResult);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash[labelId2] = HmisValue;
                        }
                        else
                        {
                            if (HmisValue.Contains(".00"))
                            {
                                decimal decResult = Convert.ToDecimal(value);
                                int changeToInt = Convert.ToInt32(decResult);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash.Add(labelId2, HmisValue);
                        }
                    }
                }
            }
            else // Single Facility Not true
            {


                if ((_repPeriod == 1) && (_startMonth == 11)) // Quarterly Service Quarter 1
                {
                    //monthYearQueryGroup1 = " where   (((Month  = @StartMonth or Month = @StartMonth + 1) and  " +
                    // "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) ";
                    //monthYearQueryGroup2 = " where  Month = @EndMonth and Year = @startYear ";
                    int nextMonth = _startMonth + 1;
                    monthQuery = "  where   (((Month  = " + _startMonth + "  or Month = " + nextMonth + ") and " +
                                           "  (Year = " + _startYear + ")) or (Month <= " + _endMonth + " and Year = " +
                                            _endYear + ")) ";

                    monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + _endYear;
                    cachePeriodType = "Month";
                }
                else if (_repPeriod == 1) // Quarterly Service
                {
                    // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                    monthQuery = "  where  Month  >=" + _startMonth + " and Month <= " +
                        _endMonth + "  and Year = " + _endYear;
                    monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + _endYear;
                    cachePeriodType = "Month";
                }
                else if (_repPeriod == 0)
                {
                    if (_startMonth == 11)
                    {
                        int prevYear = _startYear - 1;
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth +
                                " and Year = " + prevYear;
                            monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + prevYear;

                        }
                        else
                        {
                            monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                            " or ((Month  >= 1 and Month <= " + _endMonth + " ) and Year = " + _startYear + " )) ";
                            monthQuery2 = " where  Month =  " + _endMonth + " and Year = " + _startYear;
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        int prevYear = _startYear - 1;
                        if (_endMonth == 12)
                        {
                            monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth +
                                " and Year = " + prevYear;
                            monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + prevYear;
                        }
                        else
                        {
                            monthQuery = "	where  (((Month = 12) and  Year = " + prevYear + " ) " +
                            " or ((Month  >= 1 and Month <= " + _endMonth + " ) and Year = " + _startYear + " )) ";
                            monthQuery2 = " where  Month =  " + _endMonth + " and Year = " + _startYear;
                        }
                    }
                    else
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _startYear;
                        monthQuery2 = " where  Month =  " + _endMonth + "  and Year =  " + _startYear;
                    }

                    //monthQuery = " where  Month >=  " + _startMonth + "  and Month <= " + _endMonth + " and Year =  " + _startYear;
                    //monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + _startYear;
                    cachePeriodType = "Month";
                }

                string federalCmdText = "";
                string regionCmdText = "";
                string zoneCmdText = "";
                string woredaCmdText = "";
                string facilityCmdText = "";               

                string facilityListTableName = "CacheHmisCodesListFacility";
                string woredaListTableName = "CacheHmisCodesListWoreda";
                string zoneListTableName = "CacheHmisCodesListZone";
                string regionListTableName = "CacheHmisCodesListRegion";
                
                ArrayList parentQueryList = new ArrayList();

                // Create the temp tables, region,zone,woreda,facility
                // insert into the temp tables
                // do an inner join...

                // Delete the tables

                string cmdDelete = "Truncate table  " + facilityListTableName;
                SqlCommand toExecute = new SqlCommand(cmdDelete);
                _helper.Execute(toExecute);

                cmdDelete = "Truncate table  " + woredaListTableName;
                toExecute = new SqlCommand(cmdDelete);
                _helper.Execute(toExecute);

                cmdDelete = "Truncate table  " + zoneListTableName;
                toExecute = new SqlCommand(cmdDelete);
                _helper.Execute(toExecute);

                cmdDelete = "Truncate table  " + regionListTableName;
                toExecute = new SqlCommand(cmdDelete);
                _helper.Execute(toExecute);

                foreach (string locationID in locationsToView)
                {
                    string id = locationID;

                    int aggregationLevel = getAggregationLevel(locationID);

                    if (aggregationLevel == 1)
                    {
                        idQuery = "";
                        cacheAdminType = "Region";

                        federalCmdText = " select id=20, dataeleclass, cacheLevel2RegionMonth.labelid, " +
                               " facilityType, year, month, " +
                               " sum(sumValue) as sumValue from cacheLevel2RegionMonth " +
                               monthQuery + dataEleClassQuery + 
                               " group by dataeleclass, cacheLevel2RegionMonth.labelid, facilityType, year, month ";

            
                    }
                    else if (aggregationLevel == 2)
                    {
                        //cacheAdminType = "Region";

                        //regionLocId += "'" + locationID + "',";

                        //regionCmdText = "True";

                        //regionCmdText = " select cacheLevel2RegionMonth.id, dataeleclass, cacheLevel2RegionMonth.labelid, " +
                        //   " facilityType, year, month, " +
                        //   " sumValue from cacheLevel2RegionMonth " +
                        //  monthQuery + dataEleClassQuery + 
                        //    " and cacheLevel2RegionMonth.id in " + regionLocId;

                        string cmdInsert = " insert into " + regionListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        regionCmdText = "True";
                    }
                    else if (aggregationLevel == 5)
                    {
                        //cacheAdminType = "Zone";
                        //zoneLocId += "'" + locationID + "',";

                        //zoneCmdText = " select cacheLevel2ZoneMonth.id, dataeleclass, cacheLevel2ZoneMonth.labelid, " +
                        //   " facilityType, year, month, " +
                        //   " sumValue from cacheLevel2ZoneMonth " +
                        //  monthQuery + dataEleClassQuery + 
                        //    " and cacheLevel2ZoneMonth.id in " + zoneLocId;
                        string cmdInsert = " insert into " + zoneListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        zoneCmdText = "True";

                    }
                    else if (aggregationLevel == 3)
                    {
                        //cacheAdminType = "Woreda";

                        //woredaLocId += "'" + locationID + "',";

                        //woredaCmdText = " select cacheLevel2WoredaMonth.id, dataeleclass, cacheLevel2WoredaMonth.labelid, " +
                        //   " facilityType, year, month, " +
                        //   " sumValue from cacheLevel2WoredaMonth " +
                        //  monthQuery + dataEleClassQuery + 
                        //    " and cacheLevel2WoredaMonth.id in " + woredaLocId;

                        string cmdInsert = " insert into " + woredaListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        woredaCmdText = "True";
                    }
                    else if (aggregationLevel == 4)
                    {
                       // facilityLocId += "'" + locationID + "',";

                        string cmdInsert = " insert into " + facilityListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        facilityCmdText = "True";
                        // Get the parent of the locationID and use that...
                        //string parentId = "";

                        //if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityTypeId(locationID) == 3)
                        //{
                        //    // Health Post handle differently...
                        //    parentId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getDistrictId(locationID);
                        //    parentId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getDistrictId(parentId);
                        //}
                        //else
                        //{
                        //    parentId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getDistrictId(locationID);
                        //}

                        //if (getAggregationLevel(parentId) == 2) // Region
                        //{
                        //    parentToAdd = " ( regionId = " + parentId + " ) ";
                        //    if (!parentQueryList.Contains(parentToAdd))
                        //    {
                        //        parentQueryList.Add(parentToAdd);
                        //    }
                        //}
                        //else if (getAggregationLevel(parentId) == 5) // Zone
                        //{
                        //    parentToAdd = " ( zoneId = " + parentId + " ) ";
                        //    if (!parentQueryList.Contains(parentToAdd))
                        //    {
                        //        parentQueryList.Add(parentToAdd);
                        //    }
                        //}
                        //else if (getAggregationLevel(parentId) == 3) // Woreda
                        //{
                        //    parentToAdd = " ( woredaId = " + parentId + " ) ";
                        //    if (!parentQueryList.Contains(parentToAdd))
                        //    {
                        //        parentQueryList.Add(parentToAdd);
                        //    }
                        //}

                        //facilityCmdText = " select locationId as Id, dataeleclass, EthEhmis_HmisValue.labelid, \n" +
                        //   " facilitType, year, month, \n" +
                        //   " value from EthEhmis_HmisValue \n";
                          //monthQuery + dataEleClassQuery;
                         // " and EthEhmis_HmisValue.LocationId in " + facilityLocId;
                           //" and " + parentQuery;
                    }
                }

                if (facilityCmdText != "")
                {
                    facilityCmdText = " select locationId as Id, dataeleclass, EthEhmis_HmisValue.labelid, \n" +
                           " facilitType as FacilityType, year, month, \n" +
                           " value as sumValue from EthEhmis_HmisValue \n";

                    string joinCmd = " inner join " + facilityListTableName + " on \n" +
                                      " EthEhmis_HmisValue.LocationId = " + facilityListTableName + ".HmisCode\n" +
                                      monthQuery + dataEleClassQuery;

                    facilityCmdText += joinCmd;
                    //facilityCmdText = facilityCmdText.Remove(facilityCmdText.Length - 1, 1);
                    //facilityCmdText += ")";

                    //string parentQuery = "or (";
                    //int count = 0;
                    //foreach (string query in parentQueryList)
                    //{
                    //    count++;

                    //    parentQuery += " and " + query;

                    //    //if (parentQueryList.Count != count)
                    //    //{
                    //    //    parentQuery += query + " and ";
                    //    //}
                    //    //else
                    //    //{
                    //    //    parentQuery += query;
                    //    //}
                    //}

                    //facilityCmdText += parentQuery + ")";
                }

                if (woredaCmdText != "")
                {
                    //woredaCmdText = woredaCmdText.Remove(woredaCmdText.Length - 1, 1);
                    //woredaCmdText += ")";

                    woredaCmdText = " select cacheLevel2WoredaMonth.id, dataeleclass, cacheLevel2WoredaMonth.labelid, \n" +
                          " facilityType, year, month, \n" +
                          " sumValue from cacheLevel2WoredaMonth \n";

                    string joinCmd = " inner join " + woredaListTableName + " on \n" +
                                      " cacheLevel2WoredaMonth.id = " + woredaListTableName + ".HmisCode\n" +
                                      monthQuery + dataEleClassQuery;

                    woredaCmdText += joinCmd;
                }

                if (zoneCmdText != "")
                {
                    //zoneCmdText = zoneCmdText.Remove(zoneCmdText.Length - 1, 1);
                    //zoneCmdText += ")";

                    zoneCmdText = " select cacheLevel2ZoneMonth.id, dataeleclass, cacheLevel2ZoneMonth.labelid, \n" +
                          " facilityType, year, month, \n" +
                          " sumValue from cacheLevel2ZoneMonth \n";

                    string joinCmd = " inner join " + zoneListTableName + " on \n" +
                                      " cacheLevel2ZoneMonth.id = " + zoneListTableName + ".HmisCode\n" +
                                      monthQuery + dataEleClassQuery;

                    zoneCmdText += joinCmd;
                }

                if (regionCmdText != "")
                {
                    //regionCmdText = regionCmdText.Remove(regionCmdText.Length - 1, 1);
                    //regionCmdText += ")";

                    regionCmdText = " select cacheLevel2RegionMonth.id, dataeleclass, cacheLevel2RegionMonth.labelid, \n" +
                           " facilityType, year, month, \n" +
                           " sumValue from cacheLevel2RegionMonth \n";
                                           
                    string joinCmd = " inner join " + regionListTableName + " on \n" +
                                      " cacheLevel2RegionMonth.id = " + regionListTableName + ".HmisCode\n" +
                                      monthQuery + dataEleClassQuery;

                    regionCmdText += joinCmd;
                }


                //if (aggregationLevel == 1)
                //{
                //    idQuery = "";
                //    cacheAdminType = "Region";
                //}
                //else if (aggregationLevel == 2)
                //{
                //    cacheAdminType = "Region";
                //}
                //else if (aggregationLevel == 5)
                //{
                //    cacheAdminType = "Zone";
                //}
                //else if (aggregationLevel == 3)
                //{
                //    cacheAdminType = "Woreda";
                //}

                // cacheFileName = "aa" + cacheAdminType + "Level2Cache" + cachePeriodType;
                //cacheFileName = "CacheLevel2" + cacheAdminType + cachePeriodType;


                ////if (aggregationLevel != 1)
                ////{
                ////    idQuery = " and " + cacheFileName + ".id  =  " + locationID;
                ////}

                //string cmdText = "select * from " + cacheFileName + monthQuery
                //    + idQuery + dataEleClassQuery;

                string unionAll = "";

                unionAll = CustomServiceReportAggr.constructTheUnion(federalCmdText, regionCmdText, zoneCmdText, woredaCmdText, facilityCmdText);

                //string cmdText = unionAll;

                toExecute = new SqlCommand(unionAll);

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    string labelId1 = "";


                    int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                    string LabelID = row["LabelID"].ToString();
                    string locationId = row["id"].ToString();
                    decimal value = Convert.ToDecimal(row["sumValue"].ToString());
                    string dEleClass = row["DataEleClass"].ToString();
                    string appendLabel = "";


                    if (dEleClass == "3") // Mortality
                    {
                        appendLabel = "MM";
                    }

                    labelId1 = LabelID + appendLabel + "_" + locationId;
                    //labelId2 = LabelID + appendLabel + "_TotalFacilities";

                    string HmisValue = value.ToString();

                    if (aggregateDataHash[labelId1] != null)
                    {
                        decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                        newValue = newValue + value;
                        HmisValue = newValue.ToString();
                        if (HmisValue.Contains(".00"))
                        {
                            decimal decResult = Convert.ToDecimal(newValue);
                            int changeToInt = Convert.ToInt32(decResult);
                            HmisValue = changeToInt.ToString();
                        }
                        aggregateDataHash[labelId1] = HmisValue;
                    }
                    else
                    {
                        if (HmisValue.Contains(".00"))
                        {
                            decimal decResult = Convert.ToDecimal(value);
                            int changeToInt = Convert.ToInt32(decResult);
                            HmisValue = changeToInt.ToString();
                        }
                        aggregateDataHash.Add(labelId1, HmisValue);
                    }
                }
            }
        }
    }
}
