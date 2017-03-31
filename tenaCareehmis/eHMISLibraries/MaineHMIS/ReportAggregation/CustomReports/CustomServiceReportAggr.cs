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
using System.IO;
using SqlManagement.Database;
using System.Text.RegularExpressions;
using ProtoBuf;
//using Interfaces;
using eHMIS.HMIS.ReportAggregation.CustomReports.Serialization;
using eHMISWebApi.Controllers;


namespace eHMIS.HMIS.ReportAggregation.CustomReports
{

    public class CustomServiceReportAggr : ICustomReport
    {
        DBConnHelper _helper = new DBConnHelper();

        Hashtable aggregateDataHash = new Hashtable();
        Hashtable ethMonth = new Hashtable();

        List<string> locationsToView = new List<string>();
        Hashtable locationIdToName = new Hashtable();
        DataTable reportServiceDataTable = new DataTable();
        //DataTable reportIpdDataTable = new DataTable();

        string singleFacilityLocationId = "";

        Boolean singleFacility = false;

        Boolean totalFacilities = true;

        ArrayList facilityNames = new ArrayList();
        ArrayList aggregateList = new ArrayList();
        ArrayList verticalSumSequnceNo = new ArrayList();
        ArrayList IncludedList = new ArrayList();
        Hashtable verticalSumHash = new Hashtable();
        Hashtable seqLabelIDHash = new Hashtable();
        Hashtable higherAdmin = new Hashtable();
        ArrayList lastMonthLabelIds = new ArrayList();

        ArrayList totalNumFacilitiesList = new ArrayList();

        string viewLabeIdTableName = "";
        string verticalSumIdTableName = "";

        string activityDescription = "";
        string queryTable = "";
        string locationQuery = "";

        string periodType = "";
        bool higherSelected = false;

        int totalCountFacilities = 0;
        string labelIdNumFacilities = "";
        private volatile bool _shouldStop;

        int _startMonth;
        int _endMonth;
        int _startYear;
        int _endYear;
        int _quarterStart;
        int _quarterEnd;
        int _repKind;
        int _repPeriod;

        string viewQuery = "";

        string tempTableName = "";
        string tempTableName1 = "";
        string tempTableName2 = "";
        string tempTableName3 = "";
        string tempTableName4 = "";

        string hmisCodesSelected = "";
        string sha1Hash = "";
        bool level1cache = false;
        bool level2Cache = false;
        bool serializeCache = false;

        string ipdViewTable = string.Empty;
        string opdViewTable = string.Empty;
        string serviceViewTable = string.Empty;
        string facilityTable = string.Empty;
        string languageSet = LanguageController.languageSet;
        Hashtable languageHash = new Hashtable();

        private void setCorrectLanguageTable()
        {
            LanguageController.setCorrectLanguageTable();

            ipdViewTable = LanguageController.ipdViewTable;
            opdViewTable = LanguageController.opdViewTable;
            serviceViewTable = LanguageController.serviceViewTable;
            facilityTable = LanguageController.facilityTable;
            languageSet = LanguageController.languageSet;

            eHMISWebApi.Controllers.LanguageController langCtrl = new eHMISWebApi.Controllers.LanguageController();

            DataTable dtLang = langCtrl.Get(languageSet, "dataEntry");

            foreach (DataRow row in dtLang.Rows)
            {
                string indexName = row["indexName"].ToString();
                string languageName = row["languageName"].ToString();
                languageHash[indexName] = languageName;
            }
        }
        
        public CustomServiceReportAggr(List<string> locations,
            int startMonth, int endMonth, int yearStart,
            int yearEnd, int quarterStart, int quarterEnd,
            int repKind, int repPeriod, bool showOnlyQDE, bool isCacheEnabled)
        {
            System.Diagnostics.Stopwatch performanceWatch = new System.Diagnostics.Stopwatch();
            performanceWatch.Start();

            setCorrectLanguageTable();

            tempTableName1 = "#EthHmis_temp1";
            tempTableName2 = "#EthHmis_temp2";
            tempTableName3 = "#EthHmis_temp3";
            tempTableName4 = "#EthHmis_temp4";

            _helper.ManualCloseConnection = true;

            viewQuery =
            " ( \n" +
            " select " + tempTableName1 + ".LabelID, " + tempTableName1 + ".DataEleClass, " + tempTableName1 + ".Month, \n" +
                tempTableName1 + ".Quarter, " + tempTableName1 + ".Year, " + tempTableName1 + ".Value,  \n" +
                tempTableName1 + ".[LEVEL], " + tempTableName1 + ".LocationId, " + tempTableName1 + ".RegionId, \n" +
                tempTableName1 + ".ZoneId, " + tempTableName1 + ".WoredaID, " + tempTableName1 + ".FACILITTYPE   \n" +
            " from " + tempTableName1 + "  \n" +
                " union all  \n" +
            " select " + tempTableName2 + ".LabelID, " + tempTableName2 + ".DataEleClass, " + tempTableName2 + ".Month,  \n" +
                tempTableName2 + ".Quarter, " + tempTableName2 + ".Year, " + tempTableName2 + ".Value,  \n" +
                tempTableName2 + ".[LEVEL],  " + tempTableName2 + ".LocationId, " + tempTableName2 + ".RegionId, \n" +
                tempTableName2 + ".ZoneId, " + tempTableName2 + ".WoredaID, " + tempTableName2 + ".FACILITTYPE   \n" +
            " from " + tempTableName2 + "  \n" +
                " union all  \n" +
            " select " + tempTableName3 + ".LabelID, " + tempTableName3 + ".DataEleClass, " + tempTableName3 + ".Month,  \n" +
                tempTableName3 + ".Quarter, " + tempTableName3 + ".Year, " + tempTableName3 + ".Value,  \n" +
                tempTableName3 + ".[LEVEL],  " + tempTableName3 + ".LocationId, " + tempTableName3 + ".RegionId, \n" +
                tempTableName3 + ".ZoneId, " + tempTableName3 + ".WoredaID, " + tempTableName3 + ".FACILITTYPE   \n" +
            " from " + tempTableName3 + "  \n" +
                " union all \n" +
            " select " + tempTableName4 + ".LabelID, " + tempTableName4 + ".DataEleClass, " + tempTableName4 + ".Month,  \n" +
               tempTableName4 + ".Quarter, " + tempTableName4 + ".Year, " + tempTableName4 + ".Value,  \n" +
                tempTableName4 + ".[LEVEL],  " + tempTableName4 + ".LocationId, " + tempTableName4 + ".RegionId, \n" +
                tempTableName4 + ".ZoneId, " + tempTableName4 + ".WoredaID, " + tempTableName4 + ".FACILITTYPE   \n" +
            " from " + tempTableName4 + "  \n" +
           "  ) as t   \n";

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

            Hashtable MonthPeriodType = new Hashtable();

            MonthPeriodType.Add(1, 1);
            MonthPeriodType.Add(2, 0);
            MonthPeriodType.Add(3, 0);
            MonthPeriodType.Add(4, 1);
            MonthPeriodType.Add(5, 0);
            MonthPeriodType.Add(6, 0);
            MonthPeriodType.Add(7, 1);
            MonthPeriodType.Add(8, 0);
            MonthPeriodType.Add(9, 0);
            MonthPeriodType.Add(10, 1);
            MonthPeriodType.Add(11, 0);
            MonthPeriodType.Add(12, 0);
            MonthPeriodType.Add(13, 0);


            _startYear = yearStart;
            _endYear = yearStart;
            _repKind = repKind; // cmbreporttype
            _repPeriod = repPeriod; //cmbPeriod

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

            //locationsToView = locations;

            performanceWatch.Stop();
            System.Diagnostics.Trace.Write(performanceWatch.ElapsedMilliseconds);
            performanceWatch.Start();

            locationsToView = sortInstitutions(locations);
            locations.Sort();

            timeFrame = timeFrame + "," + "Service_Custom";

            hmisCodesSelected = string.Join(",", locations.ToArray()) + "," + timeFrame;
            sha1Hash = UtilitiesNew.GeneralUtilities.CryptorEngine.createSHA1Hash(hmisCodesSelected);

            level1cache = false;
            level2Cache = false;
            
            if (!level1cache)
            {
                if (locationsToView.Count == 1)
                {
                    singleFacility = true; // Only for a single facility, thus show detail month to month data
                    singleFacilityLocationId = locationsToView[0].ToString();
                }
                // or detail Quarter to Quarter data, including aggregate

                viewLabeIdTableName = serviceViewTable;
                verticalSumIdTableName = "EthioHIMS_VerticalSumNew";

                reportServiceDataTable.Clear();
                aggregateDataHash.Clear();

                reportServiceDataTable.Columns.Add(languageHash["sno"].ToString(), typeof(string));
                reportServiceDataTable.Columns.Add(languageHash["activity"].ToString(), typeof(string));
                reportServiceDataTable.Columns.Add("ReadOnly", typeof(string));
                reportServiceDataTable.Columns.Add("LabelID", typeof(string));//add new column label id 

                //string activityDescription = "ActivityHP";
                //string cmdText = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and periodtype !=2 order by sequenceno";

                Boolean hp = false;
                Boolean hc = false;
                Boolean hosp = false;
                Boolean admin = false;

                activityDescription = "ActivityWorHO";
                // Differentiate between Annual and Quarterly

                if (_repPeriod == 2) // Yearly            
                {
                    periodType = "  periodType = 2 ";
                    queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + "  order by sequenceno";
                }
                else if (_repPeriod == 1) // Quarterly
                {
                    if (HMISMainPage.UseNewVersion == true)
                    {
                        periodType = "  periodType = 3  ";
                    }
                    else
                    {
                        if (showOnlyQDE)
                            periodType = "  periodType = 1  ";
                        else
                            periodType = "  periodType = 0 or periodType = 1  ";
                    }

                    queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + "  order by sequenceno";
                }
                else if (_repPeriod == 0) // Monthly
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
                        if ((Convert.ToInt32(MonthPeriodType[startMonth]) == 1) || (Convert.ToInt32(MonthPeriodType[endMonth]) == 1))
                        {
                            periodType = "  periodType = 0 or periodType = 1  ";
                        }
                        else
                        {
                            periodType = "  periodType = 0  ";
                        }
                    }

                    queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + "  order by sequenceno";
                }

                if (singleFacility == true)
                {
                    int aggregationType = getAggregationLevel(singleFacilityLocationId);

                    if (aggregationType == 4) // Facility level
                    {
                        level2Cache = false;
                    }

                    int facilityType = getFacilityType(singleFacilityLocationId);

                    if ((facilityType == 1) || (facilityType == 5) || (facilityType == 7)) // Hospital
                    {
                        activityDescription = "ActivityHC";
                        //queryTable = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and periodtype != 2 order by sequenceno";
                        if (HMISMainPage.UseNewServiceDataElement2014 == true)
                        {
                            if ((_repPeriod == 1) && (showOnlyQDE))
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (Hospital = 1) and  (" + periodType + " or quartertitlehc = 1)  order by sequenceno";
                            }
                            else
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (Hospital = 1) and  (" + periodType + ")  order by sequenceno";
                            }
                        }
                        else
                        {
                            queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (Hospital = 1) and  (" + periodType + ")  order by sequenceno";
                        }
                    }
                    else if ((facilityType == 2) || (facilityType == 4) || (facilityType == 6))// HC
                    {
                        activityDescription = "ActivityHC";
                        // queryTable = "SELECT * from  " + viewLabeIdTableName + "  where HC = 1 and periodtype != 2 order by sequenceno";

                        if (HMISMainPage.UseNewServiceDataElement2014 == true)
                        {
                            if ((_repPeriod == 1) && (showOnlyQDE))
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (HC = 1) and  (" + periodType + " or quartertitlehc = 1)  order by sequenceno";
                            }
                            else
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (HC = 1) and  (" + periodType + ")  order by sequenceno";
                            }
                        }
                        else
                        {
                            queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (HC = 1) and  (" + periodType + ")  order by sequenceno";
                        }
                    }
                    else if (facilityType == 3) // Health Post
                    {
                        activityDescription = "ActivityHP";
                        // queryTable = "SELECT * from  " + viewLabeIdTableName + "  where HP = 1 and periodtype != 2 order by sequenceno"; 
                        if (HMISMainPage.UseNewServiceDataElement2014 == true)
                        {
                            if ((_repPeriod == 1) && (showOnlyQDE))
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (HP = 1) and  (" + periodType + " or quartertitlehp = 1)  order by sequenceno";
                            }
                            else
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (HP = 1) and  (" + periodType + ")  order by sequenceno";
                            }
                        }
                        else
                        {
                            queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (HP = 1) and  (" + periodType + ")  order by sequenceno";
                        }
                    }
                    else if (facilityType >= 50)
                    {
                        activityDescription = "ActivityHC";
                        //queryTable = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and periodtype != 2 order by sequenceno";

                        if (HMISMainPage.UseNewServiceDataElement2014 == true)
                        {
                            if ((_repPeriod == 1) && (showOnlyQDE))
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (Hospital = 1) and  (" + periodType + " or quartertitlehc = 1)  order by sequenceno";
                            }
                            else
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (Hospital = 1) and  (" + periodType + ")  order by sequenceno";
                            }
                        }
                        else
                        {
                            queryTable = "SELECT * from  " + viewLabeIdTableName + "  where (Hospital = 1) and  (" + periodType + ")  order by sequenceno";
                        }
                    }
                    else if ((facilityType >= 8) && (facilityType < 50)) // higher admin level
                    {
                        activityDescription = "ActivityWorHO";
                        //queryTable = "SELECT * from  " + viewLabeIdTableName + "  where Hospital = 1 and periodtype != 2 order by sequenceno";

                        if (HMISMainPage.UseNewServiceDataElement2014 == true)
                        {
                            if ((_repPeriod == 1) && (showOnlyQDE))
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + " or quartertitlehc = 1 or quartertitlehp = 1 order by sequenceno";
                            }
                            else
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + "  order by sequenceno";
                            }
                        }
                        else
                        {
                            queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + "  order by sequenceno";
                        }
                    }

                    // else higher Admin level use the already provided list


                    if (_repPeriod == 0)
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
                            string theMonth = languageHash[ethMonth[i].ToString().ToLower()].ToString();
                            reportServiceDataTable.Columns.Add(theMonth, typeof(string));
                        }
                        reportServiceDataTable.Columns.Add(languageHash["total"].ToString(), typeof(string));
                        //reportServiceDataTable.Columns.Add("Chart", typeof(string));
                    }
                    else if (_repPeriod == 1) // Quarter
                    {
                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            //string quarter = "Quarter  " + i;
                            string quarter = languageHash["quarter"] + "  " + i;
                            reportServiceDataTable.Columns.Add(quarter, typeof(string));
                        }
                        reportServiceDataTable.Columns.Add(languageHash["total"].ToString(), typeof(string));
                        //reportServiceDataTable.Columns.Add("Chart", typeof(string));
                    }
                    else if (_repPeriod == 2) // Yearly
                    {
                        for (int i = _startYear; i <= _endYear; i++)
                        {
                            //string theYear = "Year  " + i;
                            string theYear = languageHash["year"] + "  " + i;
                            reportServiceDataTable.Columns.Add(theYear, typeof(string));
                        }
                        reportServiceDataTable.Columns.Add(languageHash["total"].ToString(), typeof(string));
                        //reportServiceDataTable.Columns.Add("Chart", typeof(string));
                    }
                }
                else
                {
                    if (HMISMainPage.UseNewServiceDataElement2014)
                    {
                        if (_repPeriod == 1) // Quarterly, handle the special case
                        {
                            if (showOnlyQDE)
                            {
                                queryTable = "SELECT * from  " + viewLabeIdTableName + "  where  " + periodType + " or quartertitlehc = 1 or quartertitlehp = 1 order by sequenceno";
                            }
                        }
                    }

                    Boolean countStart = false;
                    int countFacilities = 0;
                    string numFacilityLabelId = "189";
                    locationQuery = "";
                    Boolean federal = false;
                    foreach (string locationID in locationsToView)
                    {

                        if (HMISMainPage.SelectedLocationID == locationID)
                        {
                            higherSelected = true;
                        }

                        aggregateDataHash[numFacilityLabelId + "_" + locationID] = 0;

                        string facilityName = getFacilityName(locationID);
                        facilityNames.Add(facilityName);

                        //if (getAggregationLevel(locationID) == 4) 
                        //{

                        int aggregationLevel = getAggregationLevel(locationID);

                        string locationString = "";
                        string locationidQuery = "";

                        //if (aggregationLevel == 1)
                        //{
                        //    idQuery = "";
                        //}
                        //else 
                        if (aggregationLevel == 2)
                        {
                            locationString = "RegionID";
                        }
                        else if (aggregationLevel == 3)
                        {
                            locationString = "WoredaID";
                        }
                        else if (aggregationLevel == 4) // This is for a facility
                        {
                            locationString = "LocationID";
                            // level2Cache = false; // disable level2Cache for facility
                        }
                        else if (aggregationLevel == 5)
                        {
                            locationString = "ZoneID";
                        }

                        if (countFacilities == 0)
                        {
                            locationQuery += " and (" + locationString + " = '" + locationID + "'";
                        }
                        else
                        {
                            locationQuery += " or " + locationString + " = '" + locationID + "'";
                        }

                        if (aggregationLevel == 1)
                        {                           
                            federal = true;
                        }
                        countFacilities++;
                        //}


                        // Here we get the aggregation level
                        if ((getAggregationLevel(locationID) == 4) && (countStart == false))
                        {
                            countStart = true;
                        }

                        if (countStart == true)
                        {
                            if (getAggregationLevel(locationID) != 4)
                            {
                                countStart = false;
                                //if (countFacilities > 1)
                                //{
                                //    reportServiceDataTable.Columns.Add("Total: Selected Facilities", typeof(string));
                                //    totalFacilities = true;
                                //}
                            }
                        }

                        string columnName = facilityName + "_" + locationID;

                        //reportServiceDataTable.Columns.Add(facilityName, typeof(string));
                        reportServiceDataTable.Columns.Add(columnName, typeof(string));

                    }

                    if (higherSelected == true)
                    {
                        string higherFacilityName = getFacilityName(HMISMainPage.SelectedLocationID);
                        //reportServiceDataTable.Columns.Remove(higherFacilityName);
                        //reportServiceDataTable.Columns.Add(higherFacilityName);

                        string columnName = higherFacilityName + "_" + HMISMainPage.SelectedLocationID;
                        reportServiceDataTable.Columns.Remove(columnName);
                        reportServiceDataTable.Columns.Add(columnName);

                    }

                    if (singleFacility == false)
                    {
                        reportServiceDataTable.Columns.Add(languageHash["total"].ToString(), typeof(string));
                        //reportServiceDataTable.Columns.Add("Chart", typeof(string));             
                    }                  

                    if (federal == true) // Not Federal
                    {
                        locationQuery = "";
                    }
                    else
                    {
                        locationQuery += " ) ";
                    }                   
                }

                performanceWatch.Stop();
                System.Diagnostics.Trace.Write(performanceWatch.ElapsedMilliseconds);
                performanceWatch.Start();

                string cmdText = "  select SNO, LabelID, sequenceNo, VerticalSumID from  " + viewLabeIdTableName +
                                 "  where verticalsumID is not null";

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
                    string LabelID = row["LabelID"].ToString();

                    verticalSumSequnceNo.Add(sequenceno);
                    verticalSumHash.Add(sequenceno, verticalSumID);
                }

                cmdText = " select LabelID, sequenceNo, aggregationType from " + viewLabeIdTableName;

                toExecute = new SqlCommand(cmdText);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;
                dt2 = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt2.Rows)
                {
                    string LabelID = row["LabelID"].ToString();
                    string sequenceno = row["sequenceNo"].ToString();
                    // int aggregationType = Convert.ToInt16(row["aggregationType"].ToString());
                    string aggregationType = row["aggregationType"].ToString();

                    if ((aggregationType == "1") && (LabelID != ""))
                    {
                        lastMonthLabelIds.Add(LabelID);
                    }

                    seqLabelIDHash.Add(sequenceno, LabelID);
                }                  
            }

            performanceWatch.Stop();
            System.Diagnostics.Trace.Write(performanceWatch.ElapsedMilliseconds);
            
        }

        public void resetNumFacilities()
        {
            string labelId = "189";

            if (_repPeriod == 2)
            {
                labelId = "2507";  // Label Id for Number of Facilities
            }
            else
            {
                labelId = "189"; // Label Id for Number of Facilities
            }

            foreach (string locationId in locationsToView)
            {
                if (Convert.ToInt32(aggregateDataHash[labelId + "_" + locationId]) == 0)
                {
                    aggregateDataHash[labelId + "_" + locationId] = "";
                }
            }

        }



        public void calculateNumFacilities(string locationId)
        {
            string labelId = "";

            if (_repPeriod == 2) // Annually
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                    labelIdNumFacilities = labelId;
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                    labelIdNumFacilities = labelId;
                }
            }
            else if (_repPeriod == 1) // Quarterly
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                    labelIdNumFacilities = labelId;
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                    labelIdNumFacilities = labelId;
                }
            }
            else if (_repPeriod == 0) // Monthly
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                    labelIdNumFacilities = labelId;
                }
                else
                {
                    labelId = "189";  // Label Id for Number of Facilities
                    labelIdNumFacilities = labelId;
                }
            }

            int count = IncludedList.Count;

            aggregateDataHash[labelId + "_" + locationId] = count;

        }

        public void calculateNumFacilities(string locationId, string ethMonth)
        {
            string labelId = "";

            if (_repPeriod == 2) // Annually
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4282";
                    labelIdNumFacilities = labelId;
                }
                else
                {
                    labelId = "2507";  // Label Id for Number of Facilities
                    labelIdNumFacilities = labelId;
                }
            }
            else if (_repPeriod == 1) // Quarterly
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4178";
                    labelIdNumFacilities = labelId;
                }
                else
                {
                    labelId = "189"; // Label Id for Number of Facilities
                    labelIdNumFacilities = labelId;
                }
            }
            else if (_repPeriod == 0) // Monthly
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    labelId = "4001";
                    labelIdNumFacilities = labelId;
                }
                else
                {
                    labelId = "189";  // Label Id for Number of Facilities
                    labelIdNumFacilities = labelId;
                }
            }

            int count = IncludedList.Count;

            //string newLabelId = labelId + "_" + ethMonth + "_" + locationId;

            //aggregateDataHash.Add(newLabelId, count);

            aggregateDataHash[labelId + "_" + ethMonth + "_" + locationId] = count;
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

            _helper.CloseConnection();
            return facilityType;
        }

        private List<string> sortInstitutions(List<string> locatLst)
        {
            List<string> locationIdList = new List<string>();
            List<string> locationIdNotSorted = new List<string>();


            // Facility

            if (getAggregationLevel(locatLst[0]) != 4) // If the first ID is facility then no worries, it is already sorted 
            {

                foreach (string id in locatLst)
                {
                    if (getAggregationLevel(id) == 4)
                    {
                        locationIdList.Add(id);
                    }
                    else
                    {
                        locationIdNotSorted.Add(id);
                    }
                }

                // Woreda
                foreach (string id in locationIdNotSorted)
                {
                    if (getAggregationLevel(id) == 3)
                    {
                        locationIdList.Add(id);
                    }
                }

                // Zone
                foreach (string id in locationIdNotSorted)
                {
                    if (getAggregationLevel(id) == 5)
                    {
                        locationIdList.Add(id);
                    }
                }

                // Region
                foreach (string id in locationIdNotSorted)
                {
                    if (getAggregationLevel(id) == 2)
                    {
                        locationIdList.Add(id);
                    }
                }

                // Federal
                foreach (string id in locationIdNotSorted)
                {
                    if (getAggregationLevel(id) == 1)
                    {
                        locationIdList.Add(id);
                        break;
                    }
                }
            }
            else
            {
                locationIdList = locatLst;
            }

            return locationIdList;
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
            
            string cmdText = "select facilityname from " + facilityTable + " where hmiscode = @locationID";
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
            createTable();

            string group1File = tempTableName1;
            string group2File = tempTableName2;
            string group3File = tempTableName3;
            string group4File = tempTableName4;

            string group1Command = "  sum(Value)  as Value ";
            string group2Command = "  sum(Value)  as Value "; // This is the last month group
            string group3Command = "  sum(Value) as Value  ";
            string group4Command = "  avg(value) as Value  ";

            string monthYearQueryGroup1 = " ";
            string monthYearQueryGroup2 = " ";
            string monthYearQueryGroup3 = " ";
            string monthYearQueryGroup4 = " ";

            string monthYearQ1QueryGroup1 = " ";
            string monthYearQ1QueryGroup2 = " ";
            string monthYearQ1QueryGroup3 = " ";
            string monthYearQ1QueryGroup4 = " ";

            string idQuery = "";
            string dataEleClassQuery = "";
            aggregateDataHash.Clear();

            SqlCommand toExecute = new SqlCommand();

            if (_repPeriod == 2) // Annual Service
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassQuery = " and DataEleClass = 17 ";
                }
                else
                {
                    dataEleClassQuery = " and DataEleClass = 7 ";
                }
            }
            else if (_repPeriod == 1) // Quarterly Service
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassQuery = " and DataEleClass = 16 ";
                }
                else
                {
                    dataEleClassQuery = " and DataEleClass = 6 ";
                }
            }
            else if (_repPeriod == 0) //Monthly Service
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassQuery = " and DataEleClass = 13 ";
                }
                else
                {
                    dataEleClassQuery = " and DataEleClass = 6 ";
                }
            }

            totalCountFacilities = 0;
            foreach (string locationID in locationsToView)
            {
                string id = locationID;

                //if (id == "10")
                //    id = "14";
                //else if (id == "1033")
                //    id = "2010100";

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
                else if (aggregationLevel == 4) // This is for a facility
                {
                    //group1Command = "  sum(Value)  as Value ";
                    //group2Command = "  sum(Value)  as Value "; // This is the last month group
                    //group3Command = "  sum(Value) as Value  ";
                    //group4Command = "  avg(value) as Value  ";

                    idQuery = " and LocationID = @newIdentification ";
                }
                else if (aggregationLevel == 5)
                {
                    idQuery = " and ZoneID = @newIdentification ";
                }

                // Main Calculation starts here, 

                if (HMISMainPage.UseNewVersion == true)
                {
                    if (_repPeriod == 2) // Annual Service
                    {
                        monthYearQueryGroup1 = monthYearQueryGroup2 = "	where  Year >= @startYear and Year <= @endYear  ";
                    }
                    else if (_repPeriod == 1) // Quarterly Serivce
                    {
                        monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Quarter >=  " + _quarterStart + "  and Quarter <=  " + _quarterEnd + " and Year =  " + _endYear;
                    }
                    else if (_repPeriod == 0) // Monthly Service
                    {
                        monthYearQueryGroup1 = monthYearQueryGroup2 = "	where  Month  >=@StartMonth and Month <= @EndMonth and Year = @StartYear ";
                    }
                }
                else
                {
                    if (_repPeriod == 2) // Annual Service
                    {
                        monthYearQueryGroup1 = monthYearQueryGroup2 = "	where  Year >= @startYear and Year <= @endYear  ";
                    }
                    else if ((_repPeriod == 1) && (_startMonth == 11)) // Quarterly Service Quarter 1
                    {
                        //monthYearQueryGroup1 = " where   (((Month  = @StartMonth or Month = @StartMonth + 1) and  " +
                        // "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) ";
                        //monthYearQueryGroup2 = " where  Month = @EndMonth and Year = @startYear ";
                        monthYearQueryGroup1 = "  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                               "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) ";
                        monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";

                        monthYearQ1QueryGroup1 = "  where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear ";
                        monthYearQ1QueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";

                    }
                    else if (_repPeriod == 1) // Quarterly Service
                    {
                        // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                        monthYearQueryGroup1 = "  where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear ";

                        monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";

                    }
                    else if (_repPeriod == 0) // Monthly Service
                    {          
                        if (_startMonth == 11)
                        {
                            if ((_endMonth == 11) || (_endMonth == 12))
                            {
                                monthYearQueryGroup1 = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear - 1 ";
                                monthYearQueryGroup2 = " where  Month =  @EndMonth  and Year =  @StartYear - 1 ";
                            }
                            else
                            {
                                monthYearQueryGroup1 = monthYearQueryGroup2 = "	where  (((Month  = 11 or Month = 12) and  Year = @StartYear - 1 ) " +
                                " or ((Month  >= 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                                monthYearQueryGroup2 = " where  Month =  @EndMonth  and Year =  @StartYear ";
                            }
                        }
                        else if (_startMonth == 12)
                        {
                            if ((_endMonth == 11) || (_endMonth == 12))
                            {
                                monthYearQueryGroup1 = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear - 1 ";
                                monthYearQueryGroup2 = " where  Month =  @EndMonth  and Year =  @StartYear - 1 ";

                            }
                            else
                            {
                                monthYearQueryGroup1 = "	where  (((Month  = 11 or Month = 12) and  Year = @StartYear - 1 ) " +
                                " or ((Month  >= 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                                monthYearQueryGroup2 = " where  Month =  @EndMonth  and Year =  @StartYear ";
                            }
                        }
                        else
                        {
                            monthYearQueryGroup1 = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear ";
                            monthYearQueryGroup2 = " where  Month =  @EndMonth  and Year =  @StartYear ";
                        }


                    }
                }

                string cmdText = "";
                if (_repPeriod == 0) // Monthly 
                {

                    //string includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                    string includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                        // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @StartYear " +
                                            monthYearQueryGroup1 +
                                            idQuery + dataEleClassQuery;
                    //"  and labelId in  " +
                    //"  (select labelId from   " + viewLabeIdTableName +
                    //"  where  " + periodType + " )";

                    toExecute = new SqlCommand(includedLocations);
                    toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                    toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                    toExecute.Parameters.AddWithValue("StartYear", _startYear);
                    toExecute.Parameters.AddWithValue("newIdentification", locationID);

                    toExecute.CommandTimeout = 4000; //300 // = 1000000;

                    //_helper.Execute(toExecute);

                    DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                    IncludedList.Clear();
                    foreach (DataRow row in dt2.Rows)
                    {
                        string LocationID = row["LocationID"].ToString();
                        IncludedList.Add(LocationID);
                    }

                    totalCountFacilities = totalCountFacilities + IncludedList.Count;
                    calculateNumFacilities(locationID);

                    // Group 1
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group1Command + "  from  " + group1File + " " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 2
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group2Command + "  from  " + group2File + " " +
                        //"	where  Month  = @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup2 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 3
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group3Command + "  from  " + group3File + " " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 4
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group4Command + "  from " + group4File + " " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    if (singleFacility == true)
                    {
                        string monthQuery = "";
                        if (HMISMainPage.UseNewVersion == true)
                        {
                            monthQuery = "	where  Month  = @StartMonth  and Year = @startYear ";
                        }
                        else
                        {
                            monthQuery = "	where  Month  = @StartMonth  and Year = @startYear and level = 0";
                        }

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
                            // includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                            includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                                // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @StartYear " +
                                            monthYearQueryGroup1 +
                                            idQuery + dataEleClassQuery;
                            //"  and labelId in  " +
                            //"  (select labelId from   " + viewLabeIdTableName +
                            //"  where  " + periodType + " )";

                            toExecute = new SqlCommand(includedLocations);
                            toExecute.Parameters.AddWithValue("StartMonth", i);
                            toExecute.Parameters.AddWithValue("EndMonth", i);
                            toExecute.Parameters.AddWithValue("StartYear", _startYear);
                            toExecute.Parameters.AddWithValue("newIdentification", locationID);

                            toExecute.CommandTimeout = 4000; //300 // = 1000000;

                            //_helper.Execute(toExecute);

                            dt2 = _helper.GetDataSet(toExecute).Tables[0];

                            IncludedList.Clear();
                            foreach (DataRow row in dt2.Rows)
                            {
                                string LocationID = row["LocationID"].ToString();
                                IncludedList.Add(LocationID);
                            }

                            int yearToUse = 0;

                            if ((i == 11) || (i == 12))
                            {
                                yearToUse = _startYear - 1;
                            }
                            else
                            {
                                yearToUse = _startYear;
                            }

                            int month = i;
                            cmdText =
                            "	select cast(LabelID as VarChar) + '_" + ethMonth[i] + "_" + locationID + "' as LabelID, " +
                                //"   sum(Value) as Value from EthEhmis_HmisValue  " +
                            "   sum(Value) as Value from " + viewQuery + " " +
                                //"	where  Month  = @StartMonth  and Year = @startYear " +
                            monthQuery +
                               dataEleClassQuery + idQuery + " group by LabelID ";
                            //addToHashTable(cmdText, id, month, _endMonth, _startYear);
                            addToHashTable(cmdText, id, month, _endMonth, yearToUse);

                            calculateNumFacilities(locationID, ethMonth[i].ToString());
                        }
                    }
                }
                else if ((_repPeriod == 1) && (_quarterStart == 1)) // Quarterly for Quarter 1
                {

                    //string includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                    string includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                        //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                        //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                     monthYearQueryGroup1 +
                                            dataEleClassQuery + idQuery;
                    //"  and labelId in  " +
                    //"  (select labelId from   " + viewLabeIdTableName +
                    //"  where  " + periodType + " )";

                    toExecute = new SqlCommand(includedLocations);
                    toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                    toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                    toExecute.Parameters.AddWithValue("startYear", _startYear);
                    toExecute.Parameters.AddWithValue("endYear", _endYear);
                    toExecute.Parameters.AddWithValue("newIdentification", locationID);

                    toExecute.CommandTimeout = 4000; //300 // = 100000;
                    //_helper.Execute(toExecute);

                    DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                    IncludedList.Clear();
                    foreach (DataRow row in dt2.Rows)
                    {
                        string LocationID = row["LocationID"].ToString();
                        IncludedList.Add(LocationID);
                    }

                    totalCountFacilities = totalCountFacilities + IncludedList.Count;
                    calculateNumFacilities(locationID);

                    // Group 1
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group1Command + "  from  " + group1File + " " +
                        //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                        //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 2
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group2Command + "  from  " + group2File + " " +
                        //"  where   Month  = @EndMonth and Year = @endYear  " +
                     monthYearQueryGroup2 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 3
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group3Command + "  from  " + group3File + " " +
                        //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                        //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 4
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group4Command + "  from  " + group4File + " " +
                        //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                        //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);



                    if (singleFacility == true)
                    {
                        _startYear = _startYear + 1; // to re-set the Start Year for Quarter 1

                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            setStartingMonth(i, i);
                            string theQuarter = "quarter" + i;

                            string monthQuery = "";

                            if (i == 1) // First Quarter
                            {
                                //if (HMISMainPage.UseNewVersion == true)
                                //{
                                //    monthQuery = "  where  Quarter =  " + i + " and Year =  " + _endYear;
                                //}
                                //else
                                //{
                                //    monthQuery = "   where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                //                 "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) ";
                                //}

                                //cmdText =
                                // "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                // "  sum(Value) as Value from EthEhmis_HmisValue  " +
                                //    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                //    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                // monthQuery +
                                //    dataEleClassQuery + idQuery +
                                //    "	group by DataEleClass,  LabelID  ";

                                //addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 1
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group1Command + "  from  " + group1File + " " +
                                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                 monthYearQueryGroup1 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 2
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group2Command + "  from  " + group2File + " " +
                                    //"  where   Month  = @EndMonth and Year = @endYear  " +
                                 monthYearQueryGroup2 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 3
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group3Command + "  from  " + group3File + " " +
                                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                 monthYearQueryGroup1 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 4
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group4Command + "  from  " + group4File + " " +
                                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                 monthYearQueryGroup1 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                                includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                                    // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @StartYear " +
                                            monthYearQueryGroup1 +
                                            idQuery + dataEleClassQuery;
                                //"  and labelId in  " +
                                //"  (select labelId from   " + viewLabeIdTableName +
                                //"  where  " + periodType + " )";

                                toExecute = new SqlCommand(includedLocations);
                                toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                                toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                                toExecute.Parameters.AddWithValue("StartYear", _startYear);
                                toExecute.Parameters.AddWithValue("EndYear", _endYear);
                                toExecute.Parameters.AddWithValue("newIdentification", locationID);

                                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                                //_helper.Execute(toExecute);

                                dt2 = _helper.GetDataSet(toExecute).Tables[0];

                                IncludedList.Clear();
                                foreach (DataRow row in dt2.Rows)
                                {
                                    string LocationID = row["LocationID"].ToString();
                                    IncludedList.Add(LocationID);
                                }
                                calculateNumFacilities(locationID, theQuarter);

                                _startYear = _startYear + 1; // to re-set the Start Year for Quarter 1
                            }
                            else
                            {
                                //if (HMISMainPage.UseNewVersion == true)
                                //{
                                //    monthQuery = "  where  Quarter =  " + i + " and Year =  " + _startYear;
                                //}
                                //else
                                //{
                                //    monthQuery = "	where  Month  >= @StartMonth and Month <= @EndMonth  and Year = @startYear ";
                                //}

                                //cmdText =
                                //"	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                //"   sum(Value) as Value from EthEhmis_HmisValue  " +
                                //    //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                                //monthQuery +
                                //   dataEleClassQuery + idQuery +
                                //   "	group by DataEleClass,  LabelID  ";

                                //addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);


                                // Group 1
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group1Command + "  from  " + group1File + " " +
                                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                 monthYearQ1QueryGroup1 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 2
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group2Command + "  from  " + group2File + " " +
                                    //"  where   Month  = @EndMonth and Year = @endYear  " +
                                 monthYearQ1QueryGroup2 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 3
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group3Command + "  from  " + group3File + " " +
                                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                 monthYearQ1QueryGroup1 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                // Group 4
                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 group4Command + "  from  " + group4File + " " +
                                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                 monthYearQ1QueryGroup1 +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);


                                //includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                                includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                                    // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @StartYear " +
                                            monthYearQ1QueryGroup1 +
                                            idQuery + dataEleClassQuery;
                                //"  and labelId in  " +
                                //"  (select labelId from   " + viewLabeIdTableName +
                                //"  where  " + periodType + " )";

                                toExecute = new SqlCommand(includedLocations);
                                toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                                toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                                toExecute.Parameters.AddWithValue("StartYear", _startYear);
                                toExecute.Parameters.AddWithValue("EndYear", _endYear);
                                toExecute.Parameters.AddWithValue("newIdentification", locationID);

                                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                                //_helper.Execute(toExecute);

                                dt2 = _helper.GetDataSet(toExecute).Tables[0];

                                IncludedList.Clear();
                                foreach (DataRow row in dt2.Rows)
                                {
                                    string LocationID = row["LocationID"].ToString();
                                    IncludedList.Add(LocationID);
                                }
                                calculateNumFacilities(locationID, theQuarter);
                            }
                        }
                        setStartingMonth(_quarterStart, _quarterEnd);
                    }
                }
                else if (_repPeriod == 1) // Quarterly other than Quarter 1
                {
                    //string includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                    string includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                            monthYearQueryGroup1 +
                            dataEleClassQuery + idQuery;
                    //"  and labelId in  " +
                    //"  (select labelId from   " + viewLabeIdTableName +
                    //"  where  " + periodType + " )";

                    toExecute = new SqlCommand(includedLocations);
                    toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                    toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                    toExecute.Parameters.AddWithValue("startYear", _startYear);
                    toExecute.Parameters.AddWithValue("newIdentification", locationID);

                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                    //_helper.Execute(toExecute);

                    DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                    IncludedList.Clear();
                    foreach (DataRow row in dt2.Rows)
                    {
                        string LocationID = row["LocationID"].ToString();
                        IncludedList.Add(LocationID);
                    }

                    totalCountFacilities = totalCountFacilities + IncludedList.Count;
                    calculateNumFacilities(locationID);

                    // Group 1
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group1Command + "  from  " + group1File + " " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 2
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group2Command + "  from  " + group2File + " " +
                        //"	where  Month  = @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup2 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 3
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group3Command + "  from  " + group3File + " " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 4
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group4Command + "  from  " + group4File + " " +
                        // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                    monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    if (singleFacility == true)
                    {
                        string monthQuery = "";
                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {                          

                            setStartingMonth(i, i);
                            string theQuarter = "quarter" + i;                         

                            // Group 1
                            cmdText =
                             "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                             group1Command + "  from  " + group1File + " " +
                                //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                             monthYearQueryGroup1 +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                            // Group 2
                            cmdText =
                             "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                             group2Command + "  from  " + group2File + " " +
                                //"	where  Month  = @EndMonth  and Year = @startYear " +
                             monthYearQueryGroup2 +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                            // Group 3
                            cmdText =
                             "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                             group3Command + "  from  " + group3File + " " +
                                //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                             monthYearQueryGroup1 +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                            // Group 4
                            cmdText =
                             "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                             group4Command + "  from  " + group4File + " " +
                                // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                            monthYearQueryGroup1 +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);


                            //includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                            includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                                // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @StartYear " +
                                        monthYearQueryGroup1 +
                                        idQuery + dataEleClassQuery;
                            //"  and labelId in  " +
                            //"  (select labelId from   " + viewLabeIdTableName +
                            //"  where  " + periodType + " )";

                            toExecute = new SqlCommand(includedLocations);
                            toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                            toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                            toExecute.Parameters.AddWithValue("StartYear", _startYear);
                            toExecute.Parameters.AddWithValue("newIdentification", locationID);

                            toExecute.CommandTimeout = 4000; //300 // = 1000000;

                            //_helper.Execute(toExecute);

                            dt2 = _helper.GetDataSet(toExecute).Tables[0];

                            IncludedList.Clear();
                            foreach (DataRow row in dt2.Rows)
                            {
                                string LocationID = row["LocationID"].ToString();
                                IncludedList.Add(LocationID);
                            }
                            calculateNumFacilities(locationID, theQuarter);
                        }
                        setStartingMonth(_quarterStart, _quarterEnd);
                    }
                }
                else if (_repPeriod == 2) // Yearly
                {
                    //string includedLocations = " select distinct(locationID) from EthEhmis_HmisValue " +
                    string includedLocations = " select distinct(locationID) from " + viewQuery + " " +
                        //"	where  Year >= @startYear and Year <= @endYear  " +
                        monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery;
                    //"  and labelId in  " +
                    //"  (select labelId from   " + viewLabeIdTableName +
                    //"  where  " + periodType + " )";

                    toExecute = new SqlCommand(includedLocations);
                    toExecute.Parameters.AddWithValue("startYear", _startYear);
                    toExecute.Parameters.AddWithValue("endYear", _endYear);
                    toExecute.Parameters.AddWithValue("newIdentification", locationID);

                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                    //_helper.Execute(toExecute);

                    DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

                    IncludedList.Clear();
                    foreach (DataRow row in dt2.Rows)
                    {
                        string LocationID = row["LocationID"].ToString();
                        IncludedList.Add(LocationID);
                    }

                    totalCountFacilities = totalCountFacilities + IncludedList.Count;
                    calculateNumFacilities(locationID);

                    // Group 1
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     group1Command + "  from  " + group1File + " " +
                        //"	where  Year >= @startYear and Year <= @endYear  " +
                     monthYearQueryGroup1 +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 2
                    cmdText =
                    "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                    group2Command + "  from  " + group2File + " " +
                        //"	where  Year >= @startYear and Year <= @endYear  " +
                    monthYearQueryGroup2 +
                       dataEleClassQuery + idQuery +
                    "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 3
                    cmdText =
                    "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                    group3Command + "  from  " + group3File + " " +
                        // "	where  Year >= @startYear and Year <= @endYear  " +
                   monthYearQueryGroup1 +
                       dataEleClassQuery + idQuery +
                    "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    // Group 4
                    cmdText =
                    "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                    group4Command + "  from  " + group4File + " " +
                        //"	where  Year >= @startYear and Year <= @endYear  " +
                    monthYearQueryGroup1 +
                       dataEleClassQuery + idQuery +
                    "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    if (singleFacility == true)
                    {
                        for (int i = _startYear; i <= _endYear; i++)
                        {
                            string theYear = "year" + i;
                            int year = i;
                            cmdText =
                             "	select cast(LabelID as VarChar) + '_" + theYear + "_" + locationID + "' as LabelID, " +
                                //"  sum(Value) as Value from EthEhmis_HmisValue  " +
                             "  sum(Value) as Value from " + viewQuery + " " +
                             "	where  Year = @startYear " +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, i);
                        }
                    }
                }
            }

            if (singleFacility == false)
            {
                allFacilitiesTotal(group1Command, group2Command, group3Command, group4Command, group1File,
                                       group2File, group3File, group4File, monthYearQueryGroup1, monthYearQueryGroup2,
                                       dataEleClassQuery);
            }

            //if (totalFacilities == true)
            //{
            //    allFacilitiesTotal(group1Command, group2Command, group3Command, group4Command, group1File,
            //                       group2File, group3File, group4File, monthYearQueryGroup1, monthYearQueryGroup2,
            //                       dataEleClassQuery);
            //}

            string tableDrop = " drop table " + tempTableName1 + " \n " + " drop table " + tempTableName2 + " \n " +
                         " drop table " + tempTableName3 + " \n " + " drop table " + tempTableName4 + " \n ";

            toExecute = new SqlCommand(tableDrop);

            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            _helper.Execute(toExecute);
            resetNumFacilities();
        }

        private void createTable()
        {
            // Create the largest dataeleclass that is needed based on report type
            // Create the largest months and year created

            string dataEleClassQuery = "";
            string monthQuery = "";
            string yearQuery = "";

            if (_repPeriod == 2) // Annual Service
            {
                dataEleClassQuery = " and DataEleClass = 7 ";
            }
            else // Quarterly Service or Monthly service
            {

                dataEleClassQuery = " and DataEleClass = 6 ";
            }


            if (_repPeriod == 2) // Annual Service
            {
                monthQuery = "	where  Year >= " + _startYear + " and Year <= " + _endYear;
            }
            else if ((_repPeriod == 1) && (_startMonth == 11)) // Quarterly Service Quarter 1
            {
                int monthAdd = _startMonth + 1;
                monthQuery = "  where   (((Month  = " + _startMonth + " or Month = " + monthAdd + " ) and " +
                                       "  (Year = " + _startYear + " )) or (Month <= " + _endMonth + " and Year = " + _endYear + " )) ";


                //monthQuery += "  and  (Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _startYear + ")";

            }
            else if (_repPeriod == 1) // Quarterly Service
            {
                // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                monthQuery = "  where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _startYear;

            }
            else if (_repPeriod == 0) // Monthly Service
            {             
                int prevYear = _endYear - 1;
                if (_startMonth == 11)
                {
                    if ((_endMonth == 11) || (_endMonth == 12))
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + "  and level = 0 and Year = " + prevYear;
                    }
                    else
                    {
                        monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                        " or ((Month  >= 1 and Month <= " + _endMonth + " ) and Year = " + _endYear + " )) and level = 0";
                    }
                }
                else if (_startMonth == 12)
                {
                    if ((_endMonth == 11) || (_endMonth == 12))
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and level = 0 and Year = " + prevYear;
                    }
                    else
                    {
                        monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                        " or ((Month  >= 1 and Month <= " + _endMonth + " ) and Year = " + _endYear + " )) and level = 0";
                    }
                }
                else
                {
                    monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + "  and level = 0 and Year = " + _endYear;
                }

            }


            string tableCreate =

                " \n \n " +
                " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + tempTableName1 + "]') AND type in (N'U'))\n " +
                " DROP TABLE [dbo].[" + tempTableName1 + "]\n " +
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
                " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + tempTableName2 + "]') AND type in (N'U'))\n " +
                " DROP TABLE [dbo].[" + tempTableName2 + "]\n " +
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
                " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + tempTableName3 + "]') AND type in (N'U'))\n " +
                " DROP TABLE [dbo].[" + tempTableName3 + "]\n " +
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
                //"    CONSTRAINT [PK_Ethtemp2_New] PRIMARY KEY CLUSTERED  \n  " +
                //"    ( \n  " +
                //"        [ValueID] ASC \n  " +
                //"    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] \n  " +
                //"    ) ON [PRIMARY] \n  " +


                " \n \n  " +
                " IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + tempTableName4 + "]') AND type in (N'U'))\n " +
                " DROP TABLE [dbo].[" + tempTableName4 + "]\n " +
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
                //"    CONSTRAINT [PK_Ethtemp2_New] PRIMARY KEY CLUSTERED  \n  " +
                //"    ( \n  " +
                //"        [ValueID] ASC \n  " +
                //"    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY] \n  " +
                //"    ) ON [PRIMARY] \n  " +



                " \n \n " +
                // Summation Group 1
                "    insert into " + tempTableName1 + "  \n  " +
                "    SELECT  *  FROM  dbo.EthEhmis_HMISValue \n  " +
                monthQuery + dataEleClassQuery +
                "    and  LabelID IN \n  " +
                "          (SELECT labelid FROM  " + viewLabeIdTableName + " where   \n  " +
                "          aggregationtype = 0  and labelid is not null)  \n  " +

                " \n \n " +
                // Last Month Group 2, Last Month Data Only
                "    insert into " + tempTableName2 + "  \n  " +
                "    SELECT  *  FROM  dbo.EthEhmis_HMISValue \n  " +
                 monthQuery + dataEleClassQuery +
                "    and  LabelID IN \n  " +
                "          (SELECT labelid FROM    " + viewLabeIdTableName + " where   \n  " +
                "          aggregationtype = 1  and labelid is not null)  \n  " +

                " \n \n " +
                // Group 3, Anding
                "    insert into " + tempTableName3 + "  \n  " +
                "    SELECT  *  FROM  dbo.EthEhmis_HMISValue \n  " +
                 monthQuery + dataEleClassQuery +
                "    and  LabelID IN \n  " +
                "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
                "          aggregationtype = 3  and labelid is not null)  \n  " +

                " \n \n " +
                // Group 4  Average Data Quality Score
                "    insert into " + tempTableName4 + "  \n  " +
                "    SELECT  *  FROM  dbo.EthEhmis_HMISValue \n  " +
                 monthQuery + dataEleClassQuery +
                "    and  LabelID IN \n  " +
                "          (SELECT labelid FROM    " + viewLabeIdTableName + "  where   \n  " +
                "          aggregationtype = 2  and labelid is not null)  ";


            SqlCommand toExecute = new SqlCommand(tableCreate);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            _helper.Execute(toExecute);   

        }

        private void allFacilitiesTotal(string group1Command, string group2Command,
                                        string group3Command, string group4Command,
                                        string group1File, string group2File,
                                        string group3File, string group4File,
                                        string monthYearQueryGroup1, string monthYearQueryGroup2,
                                        string dataEleClassQuery)
        {
            string cmdText = "";
            string idQuery = locationQuery;
            string id = "";
            if (_repPeriod == 0) // Monthly 
            {

                // Group 1
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group1Command + "  from  " + group1File + " " +
                    //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 2
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group2Command + "  from  " + group2File + " " +
                    //"	where  Month  = @EndMonth  and Year = @startYear " +
                 monthYearQueryGroup2 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 3
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group3Command + "  from  " + group3File + " " +
                    // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 4
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group4Command + "  from " + group4File + " " +
                    //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);
            }
            else if ((_repPeriod == 1) && (_quarterStart == 1)) // Quarterly for Quarter 1
            {

                // Group 1
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group1Command + "  from  " + group1File + " " +
                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 2
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group2Command + "  from  " + group2File + " " +
                    // "  where   Month  = @EndMonth and Year = @endYear  " +
                monthYearQueryGroup2 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 3
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group3Command + "  from  " + group3File + " " +
                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 4
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group4Command + "  from  " + group4File + " " +
                    //"  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                    //"  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

            }
            else if (_repPeriod == 1) // Quarterly other than Quarter 1
            {
                // Group 1
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group1Command + "  from  " + group1File + " " +
                    // "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 2
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group2Command + "  from  " + group2File + " " +
                    //"	where  Month  = @EndMonth  and Year = @startYear " +
                 monthYearQueryGroup2 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 3
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group3Command + "  from  " + group3File + " " +
                    //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 4
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group4Command + "  from  " + group4File + " " +
                    //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);
            }
            else if (_repPeriod == 2) // Yearly
            {

                // Group 1
                cmdText =
                 "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                 group1Command + "  from  " + group1File + " " +
                    //"	where  Year >= @startYear and Year <= @endYear  " +
                 monthYearQueryGroup1 +
                    dataEleClassQuery + idQuery +
                 "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 2
                cmdText =
                "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                group2Command + "  from  " + group2File + " " +
                    //"	where  Year >= @startYear and Year <= @endYear  " +
                monthYearQueryGroup2 +
                   dataEleClassQuery + idQuery +
                "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 3
                cmdText =
                "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                group3Command + "  from  " + group3File + " " +
                    //"	where  Year >= @startYear and Year <= @endYear  " +
                monthYearQueryGroup1 +
                   dataEleClassQuery + idQuery +
                "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                // Group 4
                cmdText =
                "	select cast(LabelID as VarChar) + '_TotalFacilities' as LabelID, " +
                group4Command + "  from  " + group4File + " " +
                    // "	where  Year >= @startYear and Year <= @endYear  " +
               monthYearQueryGroup1 +
                   dataEleClassQuery + idQuery +
                "	group by DataEleClass,  LabelID  ";
                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);
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

            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                string LabelID = row["labelID"].ToString();
                string value = row["value"].ToString();

                if (value.Contains(".00"))
                {
                    decimal decResult = Convert.ToDecimal(value);
                    decResult = decimal.Round(decResult, 0);
                    //int changeToInt = Convert.ToInt32(decResult);
                    //long changeToInt = Convert.ToInt64(decResult);
                    //value = changeToInt.ToString();
                    value = decResult.ToString();
                }
                aggregateDataHash.Add(LabelID, value);
            }
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }
        public DataTable CreateReport()
        {

            if (!level1cache)
            {
                if (level2Cache == false)
                {
                    DynamicQueryConstruct();
                }
                else
                {
                    updateHashTableLevel2Cache();
                }

                foreach (string locationID in locationsToView)
                {
                    string id = locationID;

                    //if (id == "10") id = "14";

                    // Get the Facility Name for the LocationID
                    string facilityName = getFacilityName(id);
                    locationIdToName.Add(locationID, facilityName);
                }



                //string cmdText = "SELECT * from  " + viewLabeIdTableName;
                // Use queryTable
                SqlCommand toExecute;
                toExecute = new SqlCommand(queryTable);
                toExecute.CommandTimeout = 4000; //300 // = 1000000;

                string sno, activity, sequenceno, labelId, readOnly;

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                //foreach (string locationId in locationsToView)
                //{
                foreach (DataRow row in dt.Rows)
                {
                    sno = row["SNO"].ToString();
                    sequenceno = row["sequenceno"].ToString();
                    activity = row[activityDescription].ToString();
                    labelId = row["LabelID"].ToString();
                    readOnly = row["readonly"].ToString();

                    // Call the insert statement
                    //foreach (string locationId in locationsToView)
                    //{
                    InsertAggregateData(sno, sequenceno, labelId, activity, readOnly); //, locationId);
                    //}
                }
                //}

                // If the current location is selected then the total column is redundant
                // so remove it
                if ((higherSelected == true) && (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityTypeId(HMISMainPage.SelectedLocationID) != 2))
                {
                    reportServiceDataTable.Columns.Remove(languageHash["total"].ToString());
                }
                
                _helper.CloseConnection();
                return reportServiceDataTable;

                //return null;
            }
            else
            {
                return reportServiceDataTable;
            }

        }

        // OPD Disease Report (Main one for higher Admin Levels)
        private void InsertAggregateData(string sno, string sequenceno, string labelId, string activity, string readOnly) //, string locationId)
        {
            if (verticalSumSequnceNo.Contains(sequenceno))
            {
                ArrayList labels = new ArrayList();

                labels = GetLabels(verticalSumHash[sequenceno].ToString());

                decimal sumValue = 0;
                string newLabel = "";

                DataRow rowsToAdd = reportServiceDataTable.NewRow();
                rowsToAdd[languageHash["sno"].ToString()] = sno;
                rowsToAdd[languageHash["activity"].ToString()] = activity;
                rowsToAdd["readOnly"] = readOnly;
                rowsToAdd["LabelID"] = labelId;
                //string result = labelId + "_" + locationId; result = (aggregateDataHash[result] == null) ? "" : aggregateDataHash[result].ToString();

                foreach (string locationId in locationsToView)
                {
                    foreach (string label in labels)
                    {
                        newLabel = label + "_" + locationId;
                        newLabel = (aggregateDataHash[newLabel] == null) ? "0" : aggregateDataHash[newLabel].ToString();
                        sumValue = sumValue + Convert.ToDecimal(newLabel);
                    }

                    string result = sumValue.ToString();
                    sumValue = 0;
                    if (result.Contains("-"))
                    {
                        result = "";
                    }

                    string facilityName = locationIdToName[locationId].ToString();
                    //string facilityName = getFacilityName(locationId);

                    if (singleFacility != true)
                    {
                        // rowsToAdd[facilityName] = result;
                        string columName = facilityName + "_" + locationId;
                        rowsToAdd[columName] = result;

                        if (labelId == labelIdNumFacilities)
                        {
                            rowsToAdd[languageHash["total"].ToString()] = totalCountFacilities;
                        }
                    }
                    else
                    {
                        rowsToAdd[languageHash["total"].ToString()] = result;  // For single Facility total
                    }


                    sumValue = 0;
                }



                if (singleFacility == false)
                {
                    if (totalFacilities == true)
                    {
                        foreach (string label in labels)
                        {
                            newLabel = label + "_TotalFacilities";
                            newLabel = (aggregateDataHash[newLabel] == null) ? "0" : aggregateDataHash[newLabel].ToString();
                            sumValue = sumValue + Convert.ToDecimal(newLabel);
                        }

                        string totalFacilityResult = sumValue.ToString();
                        sumValue = 0;

                        if (totalFacilityResult.Contains("-"))
                        {
                            totalFacilityResult = "";
                        }

                        rowsToAdd[languageHash["total"].ToString()] = totalFacilityResult;

                        if (labelId == labelIdNumFacilities)
                        {
                            rowsToAdd[languageHash["total"].ToString()] = totalCountFacilities;
                        }
                    }
                }


                // Then for each month, only if single facility and Monthly report

                if (singleFacility != true)
                {
                    /*if (!Convert.ToBoolean(readOnly))
                    {
                        rowsToAdd["Chart"] = "Click ";
                    }*/

                    int co = reportServiceDataTable.Rows.Count;

                    reportServiceDataTable.Rows.Add(rowsToAdd);
                }
                else
                {


                    string monthresult = "";
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
                        // for (int i = _startMonth; i <= _endMonth; i++)

                        foreach (int i in monthIdLists)
                        {
                            //string monthColumn = ethMonth[i].ToString();
                            string monthColumn = languageHash[ethMonth[i].ToString().ToLower()].ToString();

                            foreach (string label in labels)
                            {
                                newLabel = label + "_" + monthColumn + "_" + singleFacilityLocationId;
                                newLabel = (aggregateDataHash[newLabel] == null) ? "0" : aggregateDataHash[newLabel].ToString();
                                sumValue = sumValue + Convert.ToDecimal(newLabel);
                            }

                            monthresult = sumValue.ToString();
                            if (monthresult.Contains("-"))
                            {
                                monthresult = "";
                            }
                            //else if (monthresult.Contains(".00"))
                            //{
                            //    int changeToIntValue = Convert.ToInt32(sumValue);
                            //    monthresult = changeToIntValue.ToString();
                            //}
                            sumValue = 0;

                            rowsToAdd[monthColumn] = monthresult;
                        }
                    }
                    else if ((singleFacility == true) && (_repPeriod == 1)) // Quarterly Single Facility
                    {
                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            //string theQuarter = "quarter" + i;
                            string theQuarter = languageHash["quarter"].ToString() + i;

                            //string quarterColumn = "Quarter  " + i;
                            string quarterColumn = languageHash["quarter"].ToString() + "  " + i;

                            foreach (string label in labels)
                            {
                                newLabel = label + "_" + theQuarter + "_" + singleFacilityLocationId;
                                newLabel = (aggregateDataHash[newLabel] == null) ? "0" : aggregateDataHash[newLabel].ToString();
                                sumValue = sumValue + Convert.ToDecimal(newLabel);
                            }

                            monthresult = sumValue.ToString();

                            if (monthresult.Contains("-"))
                            {
                                monthresult = "";
                            }
                            //else if (monthresult.Contains(".00"))
                            //{
                            //    int changeToIntValue = Convert.ToInt32(sumValue);
                            //    monthresult = changeToIntValue.ToString();
                            //}
                            sumValue = 0;

                            rowsToAdd[quarterColumn] = monthresult;
                        }
                    }
                    else if ((singleFacility == true) && (_repPeriod == 2)) // Yearly Single Facility
                    {
                        for (int i = _startYear; i <= _endYear; i++)
                        {
                            //string theYear = "year" + i;
                            string theYear = languageHash["year"].ToString() + i;

                            //string yearColumn = "Year  " + i;
                            string yearColumn = languageHash["year"].ToString() + "  " + i;

                            foreach (string label in labels)
                            {
                                newLabel = label + "_" + theYear + "_" + singleFacilityLocationId;
                                newLabel = (aggregateDataHash[newLabel] == null) ? "0" : aggregateDataHash[newLabel].ToString();
                                sumValue = sumValue + Convert.ToDecimal(newLabel);
                            }

                            monthresult = sumValue.ToString();

                            if (monthresult.Contains("-"))
                            {
                                monthresult = "";
                            }
                            //else if (monthresult.Contains(".00"))
                            //{
                            //    int changeToIntValue = Convert.ToInt32(sumValue);
                            //    monthresult = changeToIntValue.ToString();
                            //}

                            sumValue = 0;
                            rowsToAdd[yearColumn] = monthresult;
                        }
                    }
                    /*  if (!Convert.ToBoolean(readOnly))
                      {
                          rowsToAdd["Chart"] = "Click ";
                      }*/
                    reportServiceDataTable.Rows.Add(rowsToAdd);

                    int co = reportServiceDataTable.Rows.Count;
                   
                }

            }
            else
            {
                // 2). The Second Entry Loops through all the facilities
                DataRow rowsToAdd = reportServiceDataTable.NewRow();
                rowsToAdd[languageHash["sno"].ToString()] = sno;
                rowsToAdd[languageHash["activity"].ToString()] = activity;
                rowsToAdd["readOnly"] = readOnly;
                rowsToAdd["LabelID"] = labelId;//add new column label id 

                //reportServiceDataTable.Rows.InsertAt(rowsToAdd, 100);

                foreach (string locationId in locationsToView)
                {
                    //string activityData = "   " + locationIdToName[locationId];

                    string result = labelId + "_" + locationId;
                    result = (aggregateDataHash[result] == null) ? "" : aggregateDataHash[result].ToString();

                    string facilityName = locationIdToName[locationId].ToString();
                    //string facilityName = getFacilityName(locationId);

                    if (singleFacility == true)
                    {
                        rowsToAdd[languageHash["total"].ToString()] = result;
                    }
                    else
                    {
                        //rowsToAdd[facilityName] = result;
                        string columnName = facilityName + "_" + locationId;
                        rowsToAdd[columnName] = result;
                    }
                }

                if (singleFacility == false)
                {
                    if (totalFacilities == true)
                    {
                        string totalFacilityResult = labelId + "_TotalFacilities";
                        totalFacilityResult = (aggregateDataHash[totalFacilityResult] == null) ? "" : aggregateDataHash[totalFacilityResult].ToString();

                        rowsToAdd[languageHash["total"].ToString()] = totalFacilityResult;
                    }
                }

                // Then for each month, only if single facility and Monthly report
                string monthresult = "";
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
                    // for (int i = _startMonth; i <= _endMonth; i++)

                    //for (int i = _startMonth; i <= _endMonth; i++)
                    foreach (int i in monthIdLists)
                    {
                        //string monthColumn = ethMonth[i].ToString();
                        string monthColumn = languageHash[ethMonth[i].ToString().ToLower()].ToString();

                        monthresult = labelId + "_" + ethMonth[i] + "_" + singleFacilityLocationId;
                        monthresult = (aggregateDataHash[monthresult] == null) ? "" : aggregateDataHash[monthresult].ToString();

                        //if (monthresult.Contains(".00"))
                        //{
                        //    decimal sumResult = Convert.ToDecimal(monthresult);
                        //    int changeToIntValue = Convert.ToInt32(sumResult);
                        //    monthresult = changeToIntValue.ToString();
                        //}
                        rowsToAdd[monthColumn] = monthresult;
                    }
                }
                else if ((singleFacility == true) && (_repPeriod == 1)) // Quarterly Single Facility
                {
                    for (int i = _quarterStart; i <= _quarterEnd; i++)
                    {
                        //string theQuarter = "quarter" + i;
                        string theQuarter = languageHash["quarter"].ToString() + i;

                        //string quarterColumn = "Quarter  " + i;

                        string quarterColumn = languageHash["quarter"].ToString() + "  " + i;

                        monthresult = labelId + "_" + theQuarter + "_" + singleFacilityLocationId;
                        monthresult = (aggregateDataHash[monthresult] == null) ? "" : aggregateDataHash[monthresult].ToString();

                        if (monthresult.Contains(".00"))
                        {
                            decimal sumResult = Convert.ToDecimal(monthresult);
                            int changeToIntValue = Convert.ToInt32(sumResult);
                            monthresult = changeToIntValue.ToString();
                        }
                        rowsToAdd[quarterColumn] = monthresult;
                    }
                }
                else if ((singleFacility == true) && (_repPeriod == 2)) // Yearly Single Facility
                {
                    for (int i = _startYear; i <= _endYear; i++)
                    {
                        //string theYear = "year" + i;
                        string theYear = languageHash["year"].ToString() + i;

                        //string yearColumn = "Year  " + i;
                        string yearColumn = languageHash["year"].ToString() + "  " + i;

                        monthresult = labelId + "_" + theYear + "_" + singleFacilityLocationId;
                        monthresult = (aggregateDataHash[monthresult] == null) ? "" : aggregateDataHash[monthresult].ToString();

                        if (monthresult.Contains(".00"))
                        {
                            decimal sumResult = Convert.ToDecimal(monthresult);
                            int changeToIntValue = Convert.ToInt32(sumResult);
                            monthresult = changeToIntValue.ToString();
                        }
                        rowsToAdd[yearColumn] = monthresult;
                    }
                }


                // reportOpdDataTable.Rows.Add("---------", "---------", "---------", "---------", "---------", "---------", "---------", "---------");

                if (singleFacility == false)
                {
                    if (labelId == labelIdNumFacilities)
                    {
                        rowsToAdd[languageHash["total"].ToString()] = totalCountFacilities;
                    }
                }
                // if (!Convert.ToBoolean(readOnly))
                //{
                //    rowsToAdd["Chart"] = "Click ";
                //}
                reportServiceDataTable.Rows.Add(rowsToAdd);

                int co = reportServiceDataTable.Rows.Count;               

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

        private void updateHashTableLevel2Cache()
        {
            serializeCache = false;
            if (serializeCache == true)
            {
                updateHashTableSerialized();
                return;
            }

            level2Cache = true;

            string idQuery = "";
            string monthQuery = "";
            string monthQuery2 = "";
            string cacheFileName = "";
            string cacheAdminType = "";
            string cachePeriodType = "";

            string labelId2 = "";

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

                    string dataEleClassQuery = " and dataEleClass = 6 ";
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
                        monthQuery2 = " where  Month =  " + i + " and Year =  " + yearToUse;
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

                    string cmdText1 = "select * from " + cacheFileName + " \ninner join " +
                       viewLabeIdTableName + " on " + viewLabeIdTableName + ".LabelId = " +
                       cacheFileName + ".LabelID \n"
                       + monthQuery + idQuery + dataEleClassQuery + " and (aggregationType = 0 or aggregationType = 2) ";

                    string cmdText2 = "select * from " + cacheFileName + " \ninner join " +
                        viewLabeIdTableName + " on " + viewLabeIdTableName + ".LabelId = " +
                        cacheFileName + ".LabelID \n"
                        + monthQuery2 + idQuery + dataEleClassQuery + " and aggregationType = 1 ";

                    string cmdText = cmdText1 + "\nUnion \n" + cmdText2;

                    SqlCommand toExecute = new SqlCommand(cmdText);

                    DataTable dt = _helper.GetDataSet(toExecute).Tables[0];


                    calulateNumFacilitiesNew2(idQuery, currentPeriod, yearToUse);

                    foreach (DataRow row in dt.Rows)
                    {
                        string labelId1 = "";

                        int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                        string LabelID = row["LabelID"].ToString();
                        decimal value = Convert.ToDecimal(row["sumValue"].ToString());
                        decimal avgValue = Convert.ToDecimal(row["avgValue"].ToString());

                        string periodName = "";
                        if (_repPeriod == 0)
                        {
                            periodName = ethMonth[i].ToString();
                        }
                        else if (_repPeriod == 1)
                        {
                            periodName = theQuarter;
                        }

                        labelId1 = LabelID + "_" + periodName + "_" + singleFacilityLocationId;
                        labelId2 = LabelID + "_" + singleFacilityLocationId;

                        string HmisValue = string.Empty;

                        if (LabelID != "188")
                        {
                            HmisValue = value.ToString();
                        }
                        else
                        {
                            HmisValue = avgValue.ToString();
                        }

                        if (LabelID != "188")
                        {
                            if (aggregateDataHash[labelId1] != null)
                            {
                                decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                                newValue = newValue + value;
                                HmisValue = newValue.ToString();
                                if (HmisValue.Contains(".00"))
                                {
                                    decimal decResult = Convert.ToDecimal(newValue);
                                    long changeToInt = Convert.ToInt64(decResult);
                                    HmisValue = changeToInt.ToString();
                                }
                                aggregateDataHash[labelId1] = HmisValue;
                            }
                            else
                            {
                                if (HmisValue.Contains(".00"))
                                {
                                    decimal decResult = Convert.ToDecimal(value);
                                    long changeToInt = Convert.ToInt64(decResult);
                                    HmisValue = changeToInt.ToString();
                                }
                                aggregateDataHash.Add(labelId1, HmisValue);
                            }

                            int lstPeriod = Convert.ToInt16(periodList[periodList.Count - 1]);

                            if (((lastMonthLabelIds.Contains(LabelID)) && (lstPeriod == i))
                                || (!lastMonthLabelIds.Contains(LabelID)))
                            {
                                if (aggregateDataHash[labelId2] != null)
                                {
                                    decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId2]);
                                    newValue = newValue + value;
                                    HmisValue = newValue.ToString();
                                    if (HmisValue.Contains(".00"))
                                    {
                                        decimal decResult = Convert.ToDecimal(newValue);
                                        long changeToInt = Convert.ToInt64(decResult);
                                        HmisValue = changeToInt.ToString();
                                    }
                                    aggregateDataHash[labelId2] = HmisValue;
                                }
                                else
                                {
                                    if (HmisValue.Contains(".00"))
                                    {
                                        decimal decResult = Convert.ToDecimal(value);
                                        long changeToInt = Convert.ToInt64(decResult);
                                        HmisValue = changeToInt.ToString();
                                    }
                                    aggregateDataHash.Add(labelId2, HmisValue);
                                }
                            }
                        }
                        else if (LabelID == "188")
                        {
                            if (aggregateDataHash[labelId1] != null)
                            {
                                decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                                newValue = decimal.Round((newValue + avgValue) / 2, 2);
                                HmisValue = newValue.ToString();
                                if (HmisValue.Contains(".00"))
                                {
                                    decimal decResult = Convert.ToDecimal(newValue);
                                    long changeToInt = Convert.ToInt64(decResult);
                                    HmisValue = changeToInt.ToString();
                                }
                                aggregateDataHash[labelId1] = HmisValue;
                            }
                            else
                            {
                                if (HmisValue.Contains(".00"))
                                {
                                    long changeToInt = Convert.ToInt64(avgValue);
                                    HmisValue = changeToInt.ToString();
                                }
                                aggregateDataHash.Add(labelId1, HmisValue);
                            }

                            int lstPeriod = Convert.ToInt16(periodList[periodList.Count - 1]);

                            if (aggregateDataHash[labelId2] != null)
                            {
                                decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId2]);
                                newValue = decimal.Round((newValue + avgValue) / 2, 2);
                                HmisValue = newValue.ToString();
                                if (HmisValue.Contains(".00"))
                                {
                                    long changeToInt = Convert.ToInt64(newValue);
                                    HmisValue = changeToInt.ToString();
                                }
                                aggregateDataHash[labelId2] = HmisValue;
                            }
                            else
                            {
                                if (HmisValue.Contains(".00"))
                                {
                                    long changeToInt = Convert.ToInt64(avgValue);
                                    HmisValue = changeToInt.ToString();
                                }
                                aggregateDataHash.Add(labelId2, HmisValue);
                            }
                        }
                    }
                }
            }
            else // single facility not true
            {

                string dataEleClassQuery = " and dataEleClass = 6 ";

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
                string regionCmdText1 = "";
                string regionCmdText2 = "";
                string zoneCmdText = "";
                string zoneCmdText1 = "";
                string zoneCmdText2 = "";
                string woredaCmdText = "";
                string woredaCmdText1 = "";
                string woredaCmdText2 = "";
                string facilityCmdText1 = "";
                string facilityCmdText2 = "";
                string facilityCmdText = "";
                string regionLocId = "(";
                string zoneLocId = "(";
                string woredaLocId = "(";
                string facilityLocId = "(";

                string facilityListTableName = "CacheHmisCodesListFacility";
                string woredaListTableName = "CacheHmisCodesListWoreda";
                string zoneListTableName = "CacheHmisCodesListZone";
                string regionListTableName = "CacheHmisCodesListRegion";

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

                        string federalCmdText1 = " select id=20, dataeleclass, cacheLevel2RegionMonth.labelid, " +
                               " facilityType, year, month, " +
                               " sum(sumValue) as sumValue from cacheLevel2RegionMonth " +
                               " inner join " + viewLabeIdTableName + " \n on " +
                               viewLabeIdTableName + ".LabelId = cacheLevel2RegionMonth.LabelId " +
                               monthQuery + dataEleClassQuery + " and aggregationType = 0 " +
                               " group by dataeleclass, cacheLevel2RegionMonth.labelid, facilityType, year, month ";

                        string federalCmdText2 = " select id=20, dataeleclass, cacheLevel2RegionMonth.labelid, " +
                               " facilityType, year, month, " +
                               " sum(sumValue) as sumValue from cacheLevel2RegionMonth " +
                               " inner join " + viewLabeIdTableName + "\n on " +
                               viewLabeIdTableName + ".LabelId = cacheLevel2RegionMonth.LabelId " +
                               monthQuery2 + dataEleClassQuery + " and aggregationType = 1 " +
                               " group by dataeleclass, cacheLevel2RegionMonth.labelid, facilityType, year, month ";

                        federalCmdText = federalCmdText1 + "\n\n Union All \n\n" + federalCmdText2;
                    }
                    else if (aggregationLevel == 2)
                    {                        
                        string cmdInsert = " insert into " + regionListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        regionCmdText = "True";

                    }
                    else if (aggregationLevel == 5)
                    {   
                        string cmdInsert = " insert into " + zoneListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        zoneCmdText = "True";

                    }
                    else if (aggregationLevel == 3)
                    {                       
                        string cmdInsert = " insert into " + woredaListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        woredaCmdText = "True";
                    }
                    else if (aggregationLevel == 4)
                    {
                        //facilityLocId += "'" + locationID + "',";

                        string cmdInsert = " insert into " + facilityListTableName + " Values ('" + locationID + "')";

                        toExecute = new SqlCommand(cmdInsert);
                        _helper.Execute(toExecute);

                        facilityCmdText = "True";                       
                    }
                }

                if (facilityCmdText != "")
                {
                    string joinCmd = " inner join " + facilityListTableName + " on \n" +
                                     " EthEhmis_HmisValue.LocationId = " + facilityListTableName + ".HmisCode\n";

                    facilityCmdText1 = " select locationId as Id, dataeleclass, EthEhmis_HmisValue.labelid, \n" +
                           " facilitType as FacilityType, year, month, \n" +
                           " value as sumValue from EthEhmis_HmisValue \n" +
                             " inner join " + viewLabeIdTableName + "\n on " +
                               viewLabeIdTableName + ".LabelId = EthEhmis_HmisValue.LabelId \n" +
                          joinCmd + monthQuery + dataEleClassQuery + " and (aggregationType = 0 or aggregationType = 2) \n";

                    facilityCmdText2 = " select locationId as Id, dataeleclass, EthEhmis_HmisValue.labelid, \n" +
                       " facilitType as FacilityType, year, month, \n" +
                       " value as sumValue from EthEhmis_HmisValue \n" +
                        " inner join " + viewLabeIdTableName + "\n on " +
                          viewLabeIdTableName + ".LabelId = EthEhmis_HmisValue.LabelId \n" +
                           joinCmd + monthQuery2 + dataEleClassQuery + " and aggregationType = 1 \n";

                    facilityCmdText = facilityCmdText1 + "\n\n Union All \n\n" + facilityCmdText2;
                }

                if (woredaCmdText != "")
                {                    
                    string joinCmd = " inner join " + woredaListTableName + " on \n" +
                                     " cacheLevel2WoredaMonth.Id = " + woredaListTableName + ".HmisCode\n";

                    woredaCmdText1 = " select cacheLevel2WoredaMonth.id, dataeleclass, cacheLevel2WoredaMonth.labelid, \n" +
                          " facilityType, year, month, \n" +
                          " sumValue from cacheLevel2WoredaMonth \n" +
                            " inner join " + viewLabeIdTableName + "\n on " +
                              viewLabeIdTableName + ".LabelId = cacheLevel2WoredaMonth.LabelId \n" +
                            joinCmd + monthQuery + dataEleClassQuery + " and (aggregationType = 0 or aggregationType = 2) \n";

                    woredaCmdText2 = " select cacheLevel2WoredaMonth.id, dataeleclass, cacheLevel2WoredaMonth.labelid, \n" +
                       " facilityType, year, month, \n" +
                       " sumValue from cacheLevel2WoredaMonth \n" +
                        " inner join " + viewLabeIdTableName + "\n on " +
                          viewLabeIdTableName + ".LabelId = cacheLevel2WoredaMonth.LabelId \n" +
                         joinCmd + monthQuery2 + dataEleClassQuery + " and aggregationType = 1 \n";

                    woredaCmdText = woredaCmdText1 + "\n\n Union All \n\n" + woredaCmdText2;
                }

                if (zoneCmdText != "")
                {                    

                    string joinCmd = " inner join " + zoneListTableName + " on \n" +
                                    " cacheLevel2ZoneMonth.Id = " + zoneListTableName + ".HmisCode\n";

                    zoneCmdText1 = " select cacheLevel2ZoneMonth.id, dataeleclass, cacheLevel2ZoneMonth.labelid, \n" +
                          " facilityType, year, month, \n" +
                          " sumValue from cacheLevel2ZoneMonth \n" +
                            " inner join " + viewLabeIdTableName + "\n on " +
                              viewLabeIdTableName + ".LabelId = cacheLevel2ZoneMonth.LabelId \n" +
                          joinCmd + monthQuery + dataEleClassQuery + " and (aggregationType = 0 or aggregationType = 2) \n";

                    zoneCmdText2 = " select cacheLevel2ZoneMonth.id, dataeleclass, cacheLevel2ZoneMonth.labelid, \n" +
                       " facilityType, year, month, \n" +
                       " sumValue from cacheLevel2ZoneMonth \n" +
                        " inner join " + viewLabeIdTableName + "\n on " +
                          viewLabeIdTableName + ".LabelId = cacheLevel2ZoneMonth.LabelId \n" +
                        joinCmd + monthQuery2 + dataEleClassQuery + " and aggregationType = 1 \n";

                    zoneCmdText = zoneCmdText1 + "\n\n Union All \n\n" + zoneCmdText2;
                }

                if (regionCmdText != "")
                {                   
                    string joinCmd = " inner join " + regionListTableName + " on \n" +
                                   " cacheLevel2RegionMonth.Id = " + regionListTableName + ".HmisCode\n";

                    regionCmdText1 = " select cacheLevel2RegionMonth.id, dataeleclass, cacheLevel2RegionMonth.labelid, \n" +
                          " facilityType, year, month, \n" +
                          " sumValue from cacheLevel2RegionMonth \n" +
                            " inner join " + viewLabeIdTableName + "\n on " +
                              viewLabeIdTableName + ".LabelId = cacheLevel2RegionMonth.LabelId \n" +
                           joinCmd + monthQuery + dataEleClassQuery + " and (aggregationType = 0 or aggregationType = 2) \n";

                    regionCmdText2 = " select cacheLevel2RegionMonth.id, dataeleclass, cacheLevel2RegionMonth.labelid, \n" +
                       " facilityType, year, month, \n" +
                       " sumValue from cacheLevel2RegionMonth \n" +
                        " inner join " + viewLabeIdTableName + "\n on " +
                          viewLabeIdTableName + ".LabelId = cacheLevel2RegionMonth.LabelId \n" +
                          joinCmd + monthQuery2 + dataEleClassQuery + " and aggregationType = 1 \n";

                    regionCmdText = regionCmdText1 + "\n\n Union All \n\n" + regionCmdText2;
                }

                // cacheFileName = "aa" + cacheAdminType + "Level2Cache" + cachePeriodType;
                //cacheFileName = "CacheLevel2" + cacheAdminType + cachePeriodType;


                //if (aggregationLevel != 1)
                //{
                //    idQuery = " and " + cacheFileName + ".id  =  " + locationID;
                //}

                string unionAll = "";

                unionAll = constructTheUnion(federalCmdText, regionCmdText, zoneCmdText, woredaCmdText, facilityCmdText);

                string cmdText = unionAll;

                toExecute = new SqlCommand(cmdText);

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    string labelId1 = "";


                    int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                    string LabelID = row["LabelID"].ToString();
                    string locationID = row["ID"].ToString();
                    decimal value = Convert.ToDecimal(row["sumValue"].ToString());
                    decimal avgValue = Convert.ToDecimal(row["avgValue"].ToString());

                    labelId1 = LabelID + "_" + locationID;
                    labelId2 = LabelID + "_TotalFacilities";

                    string HmisValue = string.Empty;

                    if (LabelID != "188")
                    {
                        HmisValue = value.ToString();
                    }
                    else
                    {
                        HmisValue = avgValue.ToString();
                    }

                    if (LabelID != "188")
                    {
                        if (aggregateDataHash[labelId1] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                            newValue = newValue + value;
                            HmisValue = newValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(newValue);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash[labelId1] = HmisValue;
                        }
                        else
                        {
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(value);
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
                                long changeToInt = Convert.ToInt64(newValue);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash[labelId2] = HmisValue;
                        }
                        else
                        {
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(value);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash.Add(labelId2, HmisValue);
                        }
                    }
                    else if (LabelID == "188")
                    {
                        if (aggregateDataHash[labelId1] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId1]);
                            newValue = (newValue + avgValue)/2;
                            HmisValue = newValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(avgValue);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash[labelId1] = HmisValue;
                        }
                        else
                        {
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(avgValue);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash.Add(labelId1, HmisValue);
                        }

                        if (aggregateDataHash[labelId2] != null)
                        {
                            decimal newValue = Convert.ToDecimal(aggregateDataHash[labelId2]);
                            newValue = (newValue + avgValue)/2;
                            HmisValue = newValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(avgValue);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash[labelId2] = HmisValue;
                        }
                        else
                        {
                            if (HmisValue.Contains(".00"))
                            {
                                long changeToInt = Convert.ToInt64(avgValue);
                                HmisValue = changeToInt.ToString();
                            }
                            aggregateDataHash.Add(labelId2, HmisValue);
                        }
                    }
                }
                calulateNumFacilitiesNew3(idQuery, 0, 0);
            }
            //}

            //calulateNumFacilitiesNew2();

            //CreateReport();          
        }

        public static string constructTheUnion(string federalCmdText, string regionCmdText, string zoneCmdText, string woredaCmdText, string facilityCmdText)
        {

            string unionAll = "";

            if (federalCmdText != "")
            {
                if ((regionCmdText != "") && (zoneCmdText != "") && (woredaCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n Union All \n" + woredaCmdText +
                                         "\n Union All \n" + facilityCmdText + "\n) as allUnion \n";
                }
                else if ((regionCmdText != "") && (zoneCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n Union All \n" + facilityCmdText + "\n) as allUnion \n";
                }
                else if ((regionCmdText != "") && (zoneCmdText != "") && (woredaCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n Union All \n" + woredaCmdText + "\n) as allUnion \n";
                }
                else if ((regionCmdText != "") && (facilityCmdText != "") && (woredaCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         facilityCmdText + "\n Union All \n" + woredaCmdText + "\n) as allUnion \n";
                }
                else if ((regionCmdText != "") && (zoneCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n) as allUnion \n";
                }
                else if ((regionCmdText != "") && (woredaCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         woredaCmdText + "\n) as allUnion \n";
                }
                else if ((regionCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                           regionCmdText + "\n Union All \n" +
                                         facilityCmdText + "\n) as allUnion \n";
                }
                else if ((zoneCmdText != "") && (woredaCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n Union All \n" +
                                         woredaCmdText + "\n Union All \n" + facilityCmdText + "\n) as allUnion \n";
                }
                else if ((zoneCmdText != "") && (woredaCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n Union All \n" +
                                         woredaCmdText + "\n) as allUnion \n";
                }
                else if ((zoneCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n Union All \n" +
                                         facilityCmdText + "\n) as allUnion \n";
                }
                else if (regionCmdText != "")
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                         regionCmdText + "\n) as allUnion \n";
                }
                else if (zoneCmdText != "")
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                         zoneCmdText + "\n) as allUnion \n";
                }
                else if (woredaCmdText != "")
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                            woredaCmdText + "\n) as allUnion \n";
                }
                else if (facilityCmdText != "")
                {
                    unionAll =
                    " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                           " from (" + federalCmdText + "\n Union All \n" +
                                            facilityCmdText + "\n) as allUnion \n";
                }
            }
            else if (regionCmdText != "")
            {
                if ((zoneCmdText != "") && (woredaCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                        zoneCmdText + "\n Union All \n" +
                                        woredaCmdText + "\n Union All \n" +
                                        facilityCmdText + "\n) as allUnion \n";
                }
                else if ((zoneCmdText != "") && (woredaCmdText != ""))
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                        zoneCmdText + "\n Union All \n" +
                                        woredaCmdText + "\n) as allUnion \n";
                }
                else if ((zoneCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                        zoneCmdText + "\n Union All \n" +
                                        facilityCmdText + "\n) as allUnion \n";
                }
                else if ((woredaCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                        woredaCmdText + "\n Union All \n" +
                                        facilityCmdText + "\n) as allUnion \n";
                }
                else if (zoneCmdText != "")
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                           zoneCmdText + "\n) as allUnion \n";
                }
                else if (woredaCmdText != "")
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                           woredaCmdText + "\n) as allUnion \n";
                }
                else if (facilityCmdText != "")
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n Union All \n" +
                                           facilityCmdText + "\n) as allUnion \n";
                }
                else
                {
                    unionAll =
                   " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                          " from (" + regionCmdText + "\n) as allUnion \n";
                }
            }
            else if (zoneCmdText != "")
            {
                if ((woredaCmdText != "") && (facilityCmdText != ""))
                {
                    unionAll =
                  " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                         " from (" + zoneCmdText + "\n Union All \n" +
                                          woredaCmdText + "\n Union All \n" +
                                          facilityCmdText + "\n) as allUnion \n";
                }
                else if (woredaCmdText != "")
                {
                    unionAll =
                  " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                         " from (" + zoneCmdText + "\n Union All \n" +
                                          woredaCmdText + "\n) as allUnion \n";
                }
                else if (facilityCmdText != "")
                {
                    unionAll =
                  " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                         " from (" + zoneCmdText + "\n Union All \n" +
                                          facilityCmdText + "\n) as allUnion \n";
                }
                else
                {
                    unionAll =
                  " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                         " from (" + zoneCmdText + "\n) as allUnion \n";
                }
            }
            else if (woredaCmdText != "")
            {
                if (facilityCmdText != "")
                {
                    unionAll =
                  " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                         " from (" + woredaCmdText + "\n Union All \n" +
                                          facilityCmdText + "\n) as allUnion \n";
                }
                else
                {
                    unionAll =
                       " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                              " from (" + woredaCmdText + "\n) as allUnion \n";
                }
            }
            else if (facilityCmdText != "")
            {
                unionAll =
                      " select allUnion.id, dataeleclass, allUnion.labelid, facilityType, year, month, sumValue " +
                                             " from (" + facilityCmdText + "\n) as allUnion \n";
            }

            return unionAll;
        }

        private void calulateNumFacilitiesNew3(string idQuery, int thePeriod, int theYear)
        {
            string monthQuery = "";
            string facilityCalcFileName = "";
            string adminType = "";

            string dataEleClassQuery = " and dataEleClass = 6 ";
            string numFacilitiesLabelId = "189";

            if (singleFacility == true)
            {
                int aggregationLevel = getAggregationLevel(singleFacilityLocationId);

                if (aggregationLevel == 1)
                {
                    adminType = "";
                    idQuery = "";
                }
                else if (aggregationLevel == 2)
                {
                    adminType = "";
                    idQuery = " and regionId = " + singleFacilityLocationId;
                }
                else if (aggregationLevel == 5)
                {
                    adminType = "";
                    idQuery = " and zoneId = " + singleFacilityLocationId;
                }
                else if (aggregationLevel == 3)
                {
                    adminType = "";
                    idQuery = " and woredaId = " + singleFacilityLocationId;
                }

                if (_repPeriod == 1)
                {
                    setStartingMonth(thePeriod, thePeriod);
                    if (thePeriod == 1) // Quarterly Service Quarter 1
                    {
                        int prevYear = _endYear - 1;

                        monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                                " or (Month  = 1 and Year = " + _endYear + " )) ";
                    }
                    else
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _endYear;
                    }

                }
                else
                {
                    int yearToUse = _startYear;
                    if ((thePeriod == 11) || (thePeriod == 12))
                    {
                        yearToUse = _startYear - 1;
                    }

                    monthQuery = "  where  Month  = " + thePeriod + " and  Year = " + yearToUse;
                }

                facilityCalcFileName = "Cache" + adminType + "NumFacilities";

                //if (aggregationLevel != 1)
                //{
                //    idQuery = " and " + facilityCalcFileName + ".id  =  " + locationID;
                //}
                //else
                //{
                //    idQuery = "";
                //}

                string cmdText = "select * from " + facilityCalcFileName + monthQuery + idQuery + dataEleClassQuery;

                SqlCommand toExecute = new SqlCommand(cmdText);

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                string periodName = "";
                if (_repPeriod == 0)
                {
                    periodName = ethMonth[thePeriod].ToString();
                }
                else if (_repPeriod == 1)
                {
                    periodName = "quarter" + thePeriod;
                }

                ArrayList numFacilitiesList = new ArrayList();

                string HmisValue = "";
                int numFacilities = 0;
                foreach (DataRow row in dt.Rows)
                {
                    int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                    string locations = row["locations"].ToString();
                    string[] locSplit = locations.Split(',');

                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];
                        if (!numFacilitiesList.Contains(locId))
                        {
                            numFacilitiesList.Add(locId);
                        }

                        if (!totalNumFacilitiesList.Contains(locId))
                        {
                            totalNumFacilitiesList.Add(locId);
                        }
                    }
                }

                numFacilities = numFacilitiesList.Count;

                string theLabelId = numFacilitiesLabelId + "_" + periodName + "_" + singleFacilityLocationId;
                HmisValue = numFacilities.ToString();

                string totalLabelIds = numFacilitiesLabelId + "_" + singleFacilityLocationId;

                aggregateDataHash[theLabelId] = HmisValue;

                aggregateDataHash[totalLabelIds] = totalNumFacilitiesList.Count.ToString();
            }
            else
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

                }
                else if (_repPeriod == 1) // Quarterly Service
                {
                    // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                    monthQuery = "  where  Month  >=" + _startMonth + " and Month <= " +
                        _endMonth + "  and Year = " + _startYear;
                }
                else if (_repPeriod == 0) // Monthly Service
                {
                    int prevYear = _startYear - 1;

                    if (_startMonth == 11)
                    {
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthQuery = "	where  Month  >=" + _startMonth + " and Month <= " + _endMonth +
                                "  and Year = " + prevYear;
                        }
                        else
                        {
                            monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                             " or ((Month  >= 1 and Month <= " + _endMonth + ") and Year = " + _startYear + "))";
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthQuery = "	where  Month  >=" + _startMonth + " and Month <= " + _endMonth +
                            " and Year = @StartYear - 1 ";
                        }
                        else
                        {
                            monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                            " or ((Month  >= 1 and Month <= " + _endMonth + ") and Year = " + _startYear + "))";
                        }
                    }
                    else
                    {
                        monthQuery = "	where  Month  >=" + _startMonth + " and Month <= " + _endMonth +
                            " and Year = " + _startYear;
                    }


                }

                facilityCalcFileName = "Cache" + adminType + "NumFacilities";

                string woredaLocId = "";
                string zoneLocId = "";
                string regionLocId = "";
                bool federalNumFacilities = false;
                string regionNumFacilities = "";
                string zoneNumFacilities = "";
                string woredaNumFacilities = "";

                foreach (string locationId in locationsToView)
                {
                    int aggregationLevel = getAggregationLevel(locationId);

                    if (aggregationLevel == 1)
                    {
                        federalNumFacilities = true;
                        break;
                    }
                    else if (aggregationLevel == 2)
                    {
                        regionLocId += "'" + locationId + "',";
                    }
                    else if (aggregationLevel == 5)
                    {
                        zoneLocId = "'" + locationId + "',";
                    }
                    else if (aggregationLevel == 3)
                    {
                        woredaLocId += "'" + locationId + "',";
                    }
                    else if (aggregationLevel == 4) //Facility
                    {
                        string facilityLabelId = numFacilitiesLabelId + "_" + locationId;
                        aggregateDataHash[facilityLabelId] = 1;
                    }
                }



                if (federalNumFacilities == false)
                {

                    if (woredaLocId != "")
                    {
                        woredaLocId = woredaLocId.Remove(woredaLocId.Length - 1, 1);
                        woredaNumFacilities += " (woredaId in (" + woredaLocId + ")) ";
                    }

                    if (zoneLocId != "")
                    {
                        zoneLocId = zoneLocId.Remove(zoneLocId.Length - 1, 1);
                        zoneNumFacilities += " (zoneId in (" + zoneLocId + ")) ";
                    }

                    if (regionLocId != "")
                    {
                        regionLocId = regionLocId.Remove(regionLocId.Length - 1, 1);
                        regionNumFacilities += " (regionId in (" + regionLocId + ")) ";
                    }

                    if (regionLocId != "")
                    {
                        if ((zoneLocId != "") && (woredaLocId != ""))
                        {
                            idQuery = " and (" + regionNumFacilities + " or " + zoneNumFacilities + " or " +
                                woredaNumFacilities + ")";
                        }
                        else if (zoneLocId != "")
                        {
                            idQuery = " and (" + regionNumFacilities + " or " + zoneNumFacilities + ")";
                        }
                        else if (woredaLocId != "")
                        {
                            idQuery = " and (" + regionNumFacilities + " or " + woredaNumFacilities + ")";
                        }
                    }
                    else if (zoneLocId != "")
                    {
                        if (woredaLocId != "")
                        {
                            idQuery = " and (" + zoneNumFacilities + " or " + woredaNumFacilities + ")";
                        }
                    }
                    else if (woredaLocId != "")
                    {
                        idQuery = " and (" + woredaNumFacilities + ")";
                    }
                }

                string cmdText = "select * from " + facilityCalcFileName + monthQuery + idQuery + dataEleClassQuery;

                SqlCommand toExecute = new SqlCommand(cmdText);

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                ArrayList numFacilitiesListRegion = new ArrayList();
                ArrayList numFacilitiesListZone = new ArrayList();
                ArrayList numFacilitiesListWoreda = new ArrayList();

                string HmisValue = "";
                int numFacilities = 0;

                Hashtable facilityListsHash = new Hashtable();
                string regionIndex = "";
                string zoneIndex = "";
                string woredaIndex = "";

                foreach (DataRow row in dt.Rows)
                {
                    int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                    string locations = row["locations"].ToString();
                    string regionId = row["regionId"].ToString();
                    string zoneId = row["zoneId"].ToString();
                    string woredaId = row["woredaId"].ToString();

                    numFacilitiesListRegion = new ArrayList();
                    numFacilitiesListZone = new ArrayList();
                    numFacilitiesListWoreda = new ArrayList();

                    if (facilityListsHash[regionId] != null)
                    {
                        numFacilitiesListRegion = (ArrayList)facilityListsHash[regionId];
                    }

                    if (facilityListsHash[zoneId] != null)
                    {
                        numFacilitiesListZone = (ArrayList)facilityListsHash[zoneId];
                    }

                    if (facilityListsHash[woredaId] != null)
                    {
                        numFacilitiesListWoreda = (ArrayList)facilityListsHash[woredaId];
                    }

                    string[] locSplit = locations.Split(',');

                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];
                        if (!numFacilitiesListRegion.Contains(locId))
                        {
                            numFacilitiesListRegion.Add(locId);
                        }

                        if (!numFacilitiesListZone.Contains(locId))
                        {
                            numFacilitiesListZone.Add(locId);
                        }

                        if (!numFacilitiesListWoreda.Contains(locId))
                        {
                            numFacilitiesListWoreda.Add(locId);
                        }

                        if (!totalNumFacilitiesList.Contains(locId))
                        {
                            totalNumFacilitiesList.Add(locId);
                        }
                    }

                    facilityListsHash[regionId] = numFacilitiesListRegion;
                    facilityListsHash[zoneId] = numFacilitiesListZone;
                    facilityListsHash[woredaId] = numFacilitiesListWoreda;


                    string regionFacilityLabel = numFacilitiesLabelId + "_" + regionId;
                    string zoneFacilityLabel = numFacilitiesLabelId + "_" + zoneId;
                    string woredaFacilityLabel = numFacilitiesLabelId + "_" + woredaId;

                    aggregateDataHash[regionFacilityLabel] = numFacilitiesListRegion.Count.ToString();
                    aggregateDataHash[zoneFacilityLabel] = numFacilitiesListZone.Count.ToString();
                    aggregateDataHash[woredaFacilityLabel] = numFacilitiesListWoreda.Count.ToString();

                    //if (aggregateDataHash[numFacilitiesLabelId] != null)
                    //{
                    //    decimal newValue = Convert.ToDecimal(aggregateDataHash[numFacilitiesLabelId]);
                    //    newValue = newValue + numFacilities;
                    //    HmisValue = newValue.ToString();
                    //    if (HmisValue.Contains(".00"))
                    //    {
                    //        decimal decResult = Convert.ToDecimal(newValue);
                    //        int changeToInt = Convert.ToInt32(decResult);
                    //        HmisValue = changeToInt.ToString();
                    //    }
                    //    aggregateDataHash[numFacilitiesLabelId] = HmisValue;
                    //}
                    //else
                    //{
                    //    aggregateDataHash[numFacilitiesLabelId] = HmisValue;
                    //}                
                }
                string totalLabelIds = numFacilitiesLabelId + "_TotalFacilities";
                aggregateDataHash[totalLabelIds] = totalNumFacilitiesList.Count.ToString();

                string federalFacilityLabel = numFacilitiesLabelId + "_" + 20;
                aggregateDataHash[federalFacilityLabel] = totalNumFacilitiesList.Count.ToString();
            }
        }

        private void calulateNumFacilitiesNew2(string idQuery, int thePeriod, int theYear)
        {
            string monthQuery = "";
            string facilityCalcFileName = "";
            string adminType = "";

            string dataEleClassQuery = " and dataEleClass = 6 ";


            if (singleFacility == true)
            {
                int aggregationLevel = getAggregationLevel(singleFacilityLocationId);

                if (aggregationLevel == 1)
                {
                    adminType = "";
                    idQuery = "";
                }
                else if (aggregationLevel == 2)
                {
                    adminType = "";
                    idQuery = " and regionId = " + singleFacilityLocationId;
                }
                else if (aggregationLevel == 5)
                {
                    adminType = "";
                    idQuery = " and zoneId = " + singleFacilityLocationId;
                }
                else if (aggregationLevel == 3)
                {
                    adminType = "";
                    idQuery = " and woredaId = " + singleFacilityLocationId;
                }

                if (_repPeriod == 1)
                {
                    setStartingMonth(thePeriod, thePeriod);
                    if (thePeriod == 1) // Quarterly Service Quarter 1
                    {
                        int prevYear = _endYear - 1;

                        monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                                " or (Month  = 1 and Year = " + _endYear + " )) ";
                    }
                    else
                    {
                        monthQuery = "	where  Month  >= " + _startMonth + " and Month <= " + _endMonth + " and Year = " + _endYear;
                    }

                }
                else
                {
                    int yearToUse = _startYear;
                    if ((thePeriod == 11) || (thePeriod == 12))
                    {
                        yearToUse = _startYear - 1;
                    }

                    monthQuery = "  where  Month  = " + thePeriod + " and  Year = " + yearToUse;
                }

                facilityCalcFileName = "Cache" + adminType + "NumFacilities";

                //if (aggregationLevel != 1)
                //{
                //    idQuery = " and " + facilityCalcFileName + ".id  =  " + locationID;
                //}
                //else
                //{
                //    idQuery = "";
                //}

                string cmdText = "select * from " + facilityCalcFileName + monthQuery + idQuery + dataEleClassQuery;

                SqlCommand toExecute = new SqlCommand(cmdText);

                DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                string numFacilitiesLabelId = "189";

                string periodName = "";
                if (_repPeriod == 0)
                {
                    periodName = ethMonth[thePeriod].ToString();
                }
                else if (_repPeriod == 1)
                {
                    periodName = "quarter" + thePeriod;
                }

                ArrayList numFacilitiesList = new ArrayList();

                string HmisValue = "";
                int numFacilities = 0;
                foreach (DataRow row in dt.Rows)
                {
                    int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                    string locations = row["locations"].ToString();
                    string[] locSplit = locations.Split(',');

                    for (int i = 0; i < locSplit.Length; i++)
                    {
                        string locId = locSplit[i];
                        if (!numFacilitiesList.Contains(locId))
                        {
                            numFacilitiesList.Add(locId);
                        }

                        if (!totalNumFacilitiesList.Contains(locId))
                        {
                            totalNumFacilitiesList.Add(locId);
                        }
                    }
                }

                numFacilities = numFacilitiesList.Count;

                string theLabelId = numFacilitiesLabelId + "_" + periodName + "_" + singleFacilityLocationId;
                HmisValue = numFacilities.ToString();

                string totalLabelIds = numFacilitiesLabelId + "_" + singleFacilityLocationId;

                aggregateDataHash[theLabelId] = HmisValue;

                aggregateDataHash[totalLabelIds] = totalNumFacilitiesList.Count.ToString();
            }
            else
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

                }
                else if (_repPeriod == 1) // Quarterly Service
                {
                    // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                    monthQuery = "  where  Month  >=" + _startMonth + " and Month <= " +
                        _endMonth + "  and Year = " + _startYear;
                }
                else if (_repPeriod == 0) // Monthly Service
                {
                    int prevYear = _startYear - 1;

                    if (_startMonth == 11)
                    {
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthQuery = "	where  Month  >=" + _startMonth + " and Month <= " + _endMonth +
                                "  and Year = " + prevYear;
                        }
                        else
                        {
                            monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                             " or ((Month  >= 1 and Month <= " + _endMonth + ") and Year = " + _startYear + "))";
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthQuery = "	where  Month  >=" + _startMonth + " and Month <= " + _endMonth +
                            " and Year = @StartYear - 1 ";
                        }
                        else
                        {
                            monthQuery = "	where  (((Month  = 11 or Month = 12) and  Year = " + prevYear + " ) " +
                            " or ((Month  >= 1 and Month <= " + _endMonth + ") and Year = " + _startYear + "))";
                        }
                    }
                    else
                    {
                        monthQuery = "	where  Month  >=" + _startMonth + " and Month <= " + _endMonth +
                            " and Year = " + _startYear;
                    }


                }

                facilityCalcFileName = "Cache" + adminType + "NumFacilities";

                //if (aggregationLevel != 1)
                //{
                //    idQuery = " and " + facilityCalcFileName + ".id  =  " + locationID;
                //}
                //else
                //{
                //    idQuery = "";
                //}

                foreach (string locationId in locationsToView)
                {
                    int aggregationLevel = getAggregationLevel(locationId);

                    if (aggregationLevel == 1)
                    {
                        idQuery = "";
                    }
                    else if (aggregationLevel == 2)
                    {
                        idQuery = " and regionId = " + locationId;
                    }
                    else if (aggregationLevel == 5)
                    {
                        idQuery = " and zoneId = " + locationId;
                    }
                    else if (aggregationLevel == 3)
                    {
                        idQuery = " and woredaId = " + locationId;
                    }


                    string cmdText = "select * from " + facilityCalcFileName + monthQuery + idQuery + dataEleClassQuery;

                    SqlCommand toExecute = new SqlCommand(cmdText);

                    DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

                    string numFacilitiesLabelId = "189";

                    ArrayList numFacilitiesList = new ArrayList();
                    string HmisValue = "";
                    int numFacilities = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        int facilityType = Convert.ToInt16(row["facilityType"].ToString());
                        string locations = row["locations"].ToString();
                        string[] locSplit = locations.Split(',');

                        for (int i = 0; i < locSplit.Length; i++)
                        {
                            string locId = locSplit[i];
                            if (!numFacilitiesList.Contains(locId))
                            {
                                numFacilitiesList.Add(locId);
                            }

                            if (!totalNumFacilitiesList.Contains(locId))
                            {
                                totalNumFacilitiesList.Add(locId);
                            }
                        }
                    }

                    numFacilities = numFacilitiesList.Count;

                    HmisValue = numFacilities.ToString();
                    aggregateDataHash[numFacilitiesLabelId + "_" + locationId] = HmisValue;

                    string totalLabelIds = numFacilitiesLabelId + "_TotalFacilities";
                    aggregateDataHash[totalLabelIds] = totalNumFacilitiesList.Count.ToString();


                    //if (aggregateDataHash[numFacilitiesLabelId] != null)
                    //{
                    //    decimal newValue = Convert.ToDecimal(aggregateDataHash[numFacilitiesLabelId]);
                    //    newValue = newValue + numFacilities;
                    //    HmisValue = newValue.ToString();
                    //    if (HmisValue.Contains(".00"))
                    //    {
                    //        decimal decResult = Convert.ToDecimal(newValue);
                    //        int changeToInt = Convert.ToInt32(decResult);
                    //        HmisValue = changeToInt.ToString();
                    //    }
                    //    aggregateDataHash[numFacilitiesLabelId] = HmisValue;
                    //}
                    //else
                    //{
                    //    aggregateDataHash[numFacilitiesLabelId] = HmisValue;
                    //}
                }
            }
        }

        private void updateHashTableSerialized()
        {
            System.Diagnostics.Stopwatch performanceWatch = new System.Diagnostics.Stopwatch();

            performanceWatch.Start();
            System.Diagnostics.Trace.WriteLine("---------" + performanceWatch.ElapsedMilliseconds + "---------");

            string idQuery = "";
            string monthQuery = "";
            string monthQuery2 = "";

            string labelId2 = "";
            string dataEleClassQuery = " and dataEleClass = 6 ";

            if (singleFacility == true)
            {
                // Start building the query for the time period
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

                    //monthQuery = " where  Month >=  " + _startMonth + "  and Month <= " + _endMonth + " and Year =  " + _startYear;
                    //monthQuery2 = " where  Month =  " + _endMonth + " and Year =  " + _startYear;                    
                }

                int aggregationLevel = getAggregationLevel(singleFacilityLocationId);


                if (aggregationLevel == 1)
                {
                    idQuery = "";
                }

                System.Diagnostics.Trace.WriteLine("---------" + performanceWatch.ElapsedMilliseconds + "---------");

                //GetHmisDescriptionsFederal(monthQuery, dataEleClassQuery);
                GetHmisDescriptionsFederalObjects(monthQuery, dataEleClassQuery);


                System.Diagnostics.Trace.WriteLine("---------" + performanceWatch.ElapsedMilliseconds + "---------");


                //calulateNumFacilitiesSerialized(monthQuery, idQuery, dataEleClassQuery);
            }
        }

        private void calulateNumFacilitiesSerialized(string monthQuery, string idQuery, string dataEleClassQuery)
        {
            //throw new NotImplementedException();
        }

        //private List<HmisDescription> GetHmisDescriptions(string monthQuery, string idQuery, string dataEleClassQuery)
        private void GetHmisDescriptions(string monthQuery, string idQuery, string dataEleClassQuery)
        {
            string queryBuilder = string.Empty;
            string cmdText = "select * from EthEhmis_HMISValueSerialize" + monthQuery + idQuery + dataEleClassQuery;

            ISerializer serializer = new ProtoBufferSerializer();
            Serializer.PrepareSerializer<Dictionary<int, decimal>>();
            DBConnHelper _helper = new DBConnHelper();

            SqlCommand findCommand = new SqlCommand(cmdText, _helper.Connection);
            SqlDataReader reader = findCommand.ExecuteReader();

            List<HmisDescription> objects = new List<HmisDescription>();
            //Dictionary<string, decimal> aggregateHash = new Dictionary<int, decimal>();
            string periodName = "";

            StringBuilder builderMonth = new StringBuilder();
            StringBuilder builderTotal = new StringBuilder();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    System.Diagnostics.Stopwatch performanceWatch = new System.Diagnostics.Stopwatch();
                    performanceWatch.Start();


                    var item = new HmisDescription();
                    var value = reader["SerializedObject"] as byte[];
                    //item.Year = Convert.ToInt16(reader["year"]);
                    item.Month = Convert.ToInt16(reader["month"]);
                    item.ZoneId = Convert.ToInt16(reader["zoneId"]);
                    item.LabelIdValues = serializer.Deserialize<Dictionary<int, decimal>>(value);

                    if (_repPeriod == 0)
                    {
                        periodName = ethMonth[item.Month].ToString();
                    }
                    string HmisValue = string.Empty;
                    string labelIdMonth = string.Empty;
                    string labelIdTotal = string.Empty;

                    foreach (int labelId in item.LabelIdValues.Keys)
                    {
                        builderMonth = new StringBuilder();

                        builderMonth.Append(labelId.ToString());
                        builderMonth.Append("_");
                        builderMonth.Append(periodName);
                        builderMonth.Append("_");
                        builderMonth.Append(singleFacilityLocationId);

                        //string labelIdMonth = labelId.ToString() + "_" + periodName + "_" + singleFacilityLocationId;
                        labelIdMonth = builderMonth.ToString();

                        //string HmisValue = value.ToString();

                        if (!aggregateDataHash.ContainsKey(labelIdMonth))
                        {
                            decimal finalValue = item.LabelIdValues[labelId];

                            HmisValue = finalValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                finalValue = decimal.Round(finalValue, 0);
                            }
                            aggregateDataHash[labelIdMonth] = finalValue;
                        }
                        else
                        {
                            decimal prevValue = Convert.ToDecimal(aggregateDataHash[labelIdMonth]);
                            decimal finalValue = prevValue + item.LabelIdValues[labelId];

                            HmisValue = finalValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                finalValue = decimal.Round(finalValue, 0);
                            }
                            aggregateDataHash[labelIdMonth] = finalValue;
                        }

                        builderTotal = new StringBuilder();

                        builderTotal.Append(labelId.ToString());
                        builderTotal.Append("_");
                        builderTotal.Append(singleFacilityLocationId);

                        //string labelIdTotal = labelId.ToString() + "_" + singleFacilityLocationId;
                        labelIdTotal = builderTotal.ToString();

                        if (!aggregateDataHash.ContainsKey(labelIdTotal))
                        {
                            decimal finalValue = item.LabelIdValues[labelId];

                            HmisValue = finalValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                finalValue = decimal.Round(finalValue, 0);
                            }
                            aggregateDataHash[labelIdTotal] = finalValue;
                        }
                        else
                        {
                            decimal prevValue = Convert.ToDecimal(aggregateDataHash[labelIdTotal]);
                            decimal finalValue = prevValue + item.LabelIdValues[labelId];

                            HmisValue = finalValue.ToString();
                            if (HmisValue.Contains(".00"))
                            {
                                finalValue = decimal.Round(finalValue, 0);
                            }
                            aggregateDataHash[labelIdTotal] = finalValue;
                        }
                    }

                    //objects.Add(item);
                    performanceWatch.Stop();
                    System.Diagnostics.Trace.WriteLine("---------" + performanceWatch.ElapsedMilliseconds + "--------------------");
                }

                reader.Close();
            }

            //return objects;
        }

        private void GetHmisDescriptionsFederal(string monthQuery, string dataEleClassQuery)
        {
            string queryBuilder = string.Empty;
            string cmdText = "select * from EthEhmis_HMISValueSerializeFederalNew1 " + monthQuery + dataEleClassQuery;

            ISerializer serializer = new ProtoBufferSerializer();
            Serializer.PrepareSerializer<Dictionary<int, decimal>>();
            DBConnHelper _helper = new DBConnHelper();

            SqlCommand findCommand = new SqlCommand(cmdText, _helper.Connection);
            SqlDataReader reader = findCommand.ExecuteReader();

            List<HmisDescription> objects = new List<HmisDescription>();
            //Dictionary<string, decimal> aggregateHash = new Dictionary<int, decimal>();
            string periodName = "";

            StringBuilder builderMonth = new StringBuilder();
            StringBuilder builderTotal = new StringBuilder();

            const char underScore = '_';

            Dictionary<string, decimal> AggregateData = new Dictionary<string, decimal>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    System.Diagnostics.Stopwatch performanceWatch = new System.Diagnostics.Stopwatch();
                    performanceWatch.Start();


                    var item = new HmisDescriptionFederal();
                    var value = reader["SerializedObject"] as byte[];                    
                    item.Month = Convert.ToInt16(reader["month"]);                   
                    item.serializedHmisValuesString = serializer.Deserialize<List<string>>(value);
                    //item.serializedHmisValues = serializer.Deserialize<List<HmisValues>>(value);


                    if (_repPeriod == 0)
                    {
                        periodName = ethMonth[item.Month].ToString();
                    }
                    string HmisValue = string.Empty;
                    string labelIdMonth = string.Empty;
                    string labelIdTotal = string.Empty;

                    foreach (string serializedVal in item.serializedHmisValuesString)
                    {
                        
                        string[] splitLabelIdValue = serializedVal.Split(underScore);
                        string splitLabelId = splitLabelIdValue[0];
                        decimal splitValue = Convert.ToDecimal(splitLabelIdValue[1]);

                        builderMonth = new StringBuilder();

                        builderMonth.Append(splitLabelIdValue[0]);
                        builderMonth.Append(underScore);
                        builderMonth.Append(periodName);
                        builderMonth.Append(underScore);
                        builderMonth.Append(singleFacilityLocationId);

                        //string labelIdMonth = labelId.ToString() + "_" + periodName + "_" + singleFacilityLocationId;
                        labelIdMonth = builderMonth.ToString();

                        //string HmisValue = value.ToString();

                        if (!AggregateData.ContainsKey(labelIdMonth))
                        {
                            decimal finalValue = splitValue;

                            AggregateData[labelIdMonth] = finalValue;
                        }
                        else
                        {
                            decimal prevValue = AggregateData[labelIdMonth];
                            decimal finalValue = prevValue + splitValue;

                            AggregateData[labelIdMonth] = finalValue;
                        }

                        builderTotal = new StringBuilder();

                        builderTotal.Append(splitLabelId);
                        builderTotal.Append(underScore);
                        builderTotal.Append(singleFacilityLocationId);

                        //string labelIdTotal = labelId.ToString() + "_" + singleFacilityLocationId;
                        labelIdTotal = builderTotal.ToString();

                        if (!AggregateData.ContainsKey(labelIdTotal))
                        {
                            decimal finalValue = splitValue;

                            AggregateData[labelIdTotal] = finalValue;
                        }
                        else
                        {
                            decimal prevValue = AggregateData[labelIdTotal];
                            decimal finalValue = prevValue + splitValue;

                            AggregateData[labelIdTotal] = finalValue;
                        }
                    }

                    //if (HmisValue.Contains(".00"))
                    //{
                    //}
                    //objects.Add(item);
                    performanceWatch.Stop();
                    System.Diagnostics.Trace.WriteLine("---------" + performanceWatch.ElapsedMilliseconds + "--------------------");
                }

                foreach (var item in AggregateData)
                {
                    aggregateDataHash.Add(item.Key, item.Value);
                }
                
                reader.Close();
            }

            //return objects;
        }

        private void GetHmisDescriptionsFederalObjects(string monthQuery, string dataEleClassQuery)
        {
            string queryBuilder = string.Empty;
            string cmdText = "select * from EthEhmis_HMISValueSerializeFederalNew1 " + monthQuery + dataEleClassQuery;

            ISerializer serializer = new ProtoBufferSerializer();
            Serializer.PrepareSerializer<Dictionary<int, decimal>>();
            DBConnHelper _helper = new DBConnHelper();

            SqlCommand findCommand = new SqlCommand(cmdText, _helper.Connection);
            SqlDataReader reader = findCommand.ExecuteReader();

            List<HmisDescription> objects = new List<HmisDescription>();
            //Dictionary<string, decimal> aggregateHash = new Dictionary<int, decimal>();
            string periodName = "";

            StringBuilder builderMonth = new StringBuilder();
            StringBuilder builderTotal = new StringBuilder();

            const char underScore = '_';

            Dictionary<string, decimal> AggregateData = new Dictionary<string, decimal>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    System.Diagnostics.Stopwatch performanceWatch = new System.Diagnostics.Stopwatch();
                    performanceWatch.Start();


                    var item = new HmisDescriptionFederal();
                    var value = reader["SerializedObject"] as byte[];
                    item.Month = Convert.ToInt16(reader["month"]);
                    item.serializedHmisValues = serializer.Deserialize<List<HmisValues>>(value);

                    if (_repPeriod == 0)
                    {
                        periodName = ethMonth[item.Month].ToString();
                    }
                    string HmisValue = string.Empty;
                    string labelIdMonth = string.Empty;
                    string labelIdTotal = string.Empty;

                    //foreach (int labelId in item.LabelIdValues.Keys)
                    foreach (HmisValues serializedVal in item.serializedHmisValues)
                    {

                        foreach (int labelId in serializedVal.labelIdValues.Keys)
                        {

                            builderMonth = new StringBuilder();

                            builderMonth.Append(labelId.ToString());
                            builderMonth.Append(underScore);
                            builderMonth.Append(periodName);
                            builderMonth.Append(underScore);
                            builderMonth.Append(singleFacilityLocationId);

                            //string labelIdMonth = labelId.ToString() + "_" + periodName + "_" + singleFacilityLocationId;
                            labelIdMonth = builderMonth.ToString();

                            //string HmisValue = value.ToString();

                            if (!AggregateData.ContainsKey(labelIdMonth))
                            {
                                decimal finalValue = serializedVal.labelIdValues[labelId];

                                AggregateData[labelIdMonth] = finalValue;
                            }
                            else
                            {
                                decimal prevValue = AggregateData[labelIdMonth];
                                decimal finalValue = prevValue + serializedVal.labelIdValues[labelId];

                                AggregateData[labelIdMonth] = finalValue;
                            }

                            builderTotal = new StringBuilder();

                            builderTotal.Append(labelId.ToString());
                            builderTotal.Append(underScore);
                            builderTotal.Append(singleFacilityLocationId);

                            //string labelIdTotal = labelId.ToString() + "_" + singleFacilityLocationId;
                            labelIdTotal = builderTotal.ToString();

                            if (!AggregateData.ContainsKey(labelIdTotal))
                            {
                                decimal finalValue = serializedVal.labelIdValues[labelId];

                                AggregateData[labelIdTotal] = finalValue;
                            }
                            else
                            {
                                decimal prevValue = AggregateData[labelIdTotal];
                                decimal finalValue = prevValue + serializedVal.labelIdValues[labelId];

                                AggregateData[labelIdTotal] = finalValue;
                            }
                        }
                    }

                    //if (HmisValue.Contains(".00"))
                    //{
                    //}
                    //objects.Add(item);
                    performanceWatch.Stop();
                    System.Diagnostics.Trace.WriteLine("---------" + performanceWatch.ElapsedMilliseconds + "--------------------");
                }

                foreach (var item in AggregateData)
                {
                    aggregateDataHash.Add(item.Key, item.Value);
                }

                reader.Close();
            }

            //return objects;
        }

    }
}
