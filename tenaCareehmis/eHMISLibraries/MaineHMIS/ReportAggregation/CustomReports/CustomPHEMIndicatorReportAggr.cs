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

namespace eHMIS.HMIS.ReportAggregation.CustomReports
{

    public class CustomPHEMIndicatorReportAggr : ICustomReport
    {
        DBConnHelper _helper = new DBConnHelper();

        Hashtable aggregateDataHash = new Hashtable();
        Hashtable ethMonth = new Hashtable();

        Hashtable monthIdHash = new Hashtable();

        List<string> locationsToView = new List<string>();
        Hashtable locationIdToName = new Hashtable();
        DataTable reportIndicatorDataTable = new DataTable();
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

        string viewLabeIdTableName = "";
        string verticalSumIdTableName = "";

        string activityDescription = "";
        string queryTable = "";
        string locationQuery = "";

        string periodType = "";
        bool higherSelected = false;

        int totalCountFacilities = 0;
        string labelIdNumFacilities = "";
        decimal denomMulitply = 1;

        int _startMonth;
        int _endMonth;
        int _startYear;
        int _endYear;
        int _quarterStart;
        int _quarterEnd;
        int _repKind;
        int _repPeriod;
        int _startweek;
        int _endweek;

        bool _showNumDenom = true;
        bool _showTarget = false;

        bool _noAdminLevel = true;

        bool _showOnlyQuarterlyDataElements = false;
        private volatile bool _shouldStop;
        // Choose Federal, Region ID or ZoneID or WoredaID or Facility
        // Report Period start Month / End Month and year
        // For the monthly reports
        // exceptional cases of aggregation

        ArrayList listOfMonths = new ArrayList();
        ArrayList endQuarterMonths = new ArrayList();
        bool onlyOneMonth = false;
        bool onlyOneQuarter = false;

        Hashtable targetValuesHash = new Hashtable();

        Hashtable labelIdAggrType = new Hashtable();

        string numDaysLabelId = "888888";

        public CustomPHEMIndicatorReportAggr(List<string> locations, int startMonth, int endMonth, int yearStart, int yearEnd, int quarterStart, int quarterEnd, int repKind, int repPeriod, bool showNumDenom, bool showTarget)
        {

            bool showOnlyQuartelyDE = false;

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

            monthIdHash.Add("Meskerem", 1);
            monthIdHash.Add("Tikimt", 2);
            monthIdHash.Add("Hidar", 3);
            monthIdHash.Add("Tahisas", 4);
            monthIdHash.Add("Tir", 5);
            monthIdHash.Add("Yekatit", 6);
            monthIdHash.Add("Megabit", 7);
            monthIdHash.Add("Miyazia", 8);
            monthIdHash.Add("Ginbot", 9);
            monthIdHash.Add("Sene", 10);
            monthIdHash.Add("Hamle", 11);
            monthIdHash.Add("Nehase", 12);
            monthIdHash.Add("Pagume", 13);

            _showNumDenom = showNumDenom;
            _showTarget = showTarget;

            _showOnlyQuarterlyDataElements = showOnlyQuartelyDE;

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

            endQuarterMonths.Add("1");
            endQuarterMonths.Add("4");
            endQuarterMonths.Add("7");
            endQuarterMonths.Add("10");


            _startYear = yearStart;
            _endYear = yearStart;
            _repKind = repKind;
            _repPeriod = repPeriod;

            if (repPeriod == 0) // Monthly
            {
                decimal numMonth = 12;

                _startMonth = startMonth;
                _endMonth = endMonth;

                if (_startMonth <= _endMonth)
                {
                    numMonth = (_endMonth - _startMonth) + 1;
                }
                else
                {
                    if (_startMonth == 11)
                    {
                        numMonth = _endMonth + 2;
                    }
                    else if (_startMonth == 12)
                    {
                        numMonth = _endMonth + 1;
                    }
                }
                denomMulitply = numMonth / 12;
            }
            else if (repPeriod == 1) // Quarterly
            {
                decimal numMonth = 3;
                _startYear = yearStart;
                _quarterStart = quarterStart;
                _quarterEnd = quarterEnd;
                setStartingMonth(_quarterStart, _quarterEnd);

                if (_quarterStart == _quarterEnd)
                {
                    denomMulitply = numMonth / 12;
                }
                else
                {
                    numMonth = ((_quarterEnd - _quarterStart) + 1) * 3;
                    denomMulitply = numMonth / 12;
                }
            }
            else if (repPeriod == 2) // Yearly
            {
                _endYear = yearEnd;
            }

            //locationsToView = locations;

            locationsToView = sortInstitutions(locations);

            if (locationsToView.Count == 1)
            {
                singleFacility = true; // Only for a single facility, thus show detail month to month data
                singleFacilityLocationId = locationsToView[0].ToString();
            }
            // or detail Quarter to Quarter data, including aggregate           

            reportIndicatorDataTable.Clear();
            aggregateDataHash.Clear();

            _noAdminLevel = true;

            foreach (string locationID in locationsToView)
            {
                string id = locationID;

                if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) != 4)
                {
                    _noAdminLevel = false;
                }
                //if (id == "10") id = "14";

                // Get the Facility Name for the LocationID
                string facilityName = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(id);
                locationIdToName.Add(locationID, facilityName);
            }

            if (_repPeriod == 0)
            {
                if (_startMonth != _endMonth)
                {
                    onlyOneMonth = false;

                    if ((_startMonth == 11) && (_endMonth == 12))
                    {
                        listOfMonths.Add(ethMonth[11]);
                        listOfMonths.Add(ethMonth[12]);
                    }
                    else if ((_startMonth == 11) && (_endMonth != 12))
                    {
                        listOfMonths.Add(ethMonth[11]);
                        listOfMonths.Add(ethMonth[12]);
                        for (int i = 1; i <= _endMonth; i++)
                        {
                            listOfMonths.Add(ethMonth[i]);
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        listOfMonths.Add(ethMonth[12]);
                        for (int i = 1; i <= _endMonth; i++)
                        {
                            listOfMonths.Add(ethMonth[i]);
                        }
                    }
                    else
                    {
                        for (int i = _startMonth; i <= _endMonth; i++)
                        {
                            listOfMonths.Add(ethMonth[i]);
                        }
                    }
                }
                else
                {
                    onlyOneMonth = true;
                }
            }
            else if (_repPeriod == 1)
            {
                if (_quarterStart == _quarterEnd)
                {
                    onlyOneQuarter = true;
                }
                else
                {
                    onlyOneQuarter = false;
                }
            }

            if (HMISMainPage.UseNewServiceDataElement2014)
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElementsNew";
                verticalSumIdTableName = "EthioHIMS_VerticalSumNew";
            }
            else
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElements";
                verticalSumIdTableName = "EthioHIMS_VerticalSum";
            }

            string cmdQuery = " select * from " + viewLabeIdTableName + " where aggregationType != 0 and labelId is not null";

            SqlCommand toExecute = new SqlCommand(cmdQuery);


            toExecute.CommandTimeout = 4000; //300 // = 1000000;


            DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
            string aggrLabelId = "";
            int aggrType = 0;
            labelIdAggrType.Clear();

            foreach (DataRow row in dt1.Rows)
            {
                aggrLabelId = row["labelId"].ToString();
                aggrType = Convert.ToInt16(row["aggregationType"].ToString());
                if (aggrLabelId != "")
                {
                    labelIdAggrType.Add(aggrLabelId, aggrType);
                }
            }
        }
        public CustomPHEMIndicatorReportAggr(List<string> locations, int startweek, int endweek, int yearStart, int yearEnd, int repKind, int repPeriod, bool showNumDenom, bool showTarget)
        {
            /* ethMonth.Add(1, "Meskerem");
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

             monthIdHash.Add("Meskerem", 1);
             monthIdHash.Add("Tikimt", 2);
             monthIdHash.Add("Hidar", 3);
             monthIdHash.Add("Tahisas", 4);
             monthIdHash.Add("Tir", 5);
             monthIdHash.Add("Yekatit", 6);
             monthIdHash.Add("Megabit", 7);
             monthIdHash.Add("Miyazia", 8);
             monthIdHash.Add("Ginbot", 9);
             monthIdHash.Add("Sene", 10);
             monthIdHash.Add("Hamle", 11);
             monthIdHash.Add("Nehase", 12);
             monthIdHash.Add("Pagume", 13);*/

            _showNumDenom = showNumDenom;
            _showTarget = showTarget;

            _showOnlyQuarterlyDataElements = false;

            Hashtable MonthPeriodType = new Hashtable();

            /* MonthPeriodType.Add(1, 1);
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

             endQuarterMonths.Add("1");
             endQuarterMonths.Add("4");
             endQuarterMonths.Add("7");
             endQuarterMonths.Add("10");*/


            _startYear = yearStart;
            _endYear = yearStart;
            _repKind = repKind;
            _repPeriod = repPeriod;

            //if (repPeriod == 0) // Monthly
            //  {
            //   decimal numMonth = 12;

            _startweek = startweek;
            _endweek = endweek;

            /*    if (_startMonth <= _endMonth)
                {
                    numMonth = (_endMonth - _startMonth) + 1;
                }
                else
                {
                    if (_startMonth == 11)
                    {
                        numMonth = _endMonth + 2;
                    }
                    else if (_startMonth == 12)
                    {
                        numMonth = _endMonth + 1;
                    }
                }
                denomMulitply = numMonth / 12;
            }
            else if (repPeriod == 1) // Quarterly
            {
                decimal numMonth = 3;
                _startYear = yearStart;
                _quarterStart = quarterStart;
                _quarterEnd = quarterEnd;
                setStartingMonth(_quarterStart, _quarterEnd);

                if (_quarterStart == _quarterEnd)
                {
                    denomMulitply = numMonth / 12;
                }
                else
                {
                    numMonth = ((_quarterEnd - _quarterStart) + 1) * 3;
                    denomMulitply = numMonth / 12;
                }
            }
            else if (repPeriod == 2) // Yearly
            {
                _endYear = yearEnd;
            }*/

            //locationsToView = locations;

            locationsToView = sortInstitutions(locations);

            if (locationsToView.Count == 1)
            {
                singleFacility = true; // Only for a single facility, thus show detail month to month data
                singleFacilityLocationId = locationsToView[0].ToString();
            }
            // or detail Quarter to Quarter data, including aggregate           

            reportIndicatorDataTable.Clear();
            aggregateDataHash.Clear();

            _noAdminLevel = true;

            foreach (string locationID in locationsToView)
            {
                string id = locationID;

                if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) != 4)
                {
                    _noAdminLevel = false;
                }
                //if (id == "10") id = "14";

                // Get the Facility Name for the LocationID
                string facilityName = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityName(id);
                locationIdToName.Add(locationID, facilityName);
            }

            if (_repPeriod == 0)
            {
                if (_startweek != _endweek)
                {
                    onlyOneMonth = false;

                    /*  if ((_startMonth == 11) && (_endMonth == 12))
                      {
                          listOfMonths.Add(ethMonth[11]);
                          listOfMonths.Add(ethMonth[12]);
                      }
                      else if ((_startMonth == 11) && (_endMonth != 12))
                      {
                          listOfMonths.Add(ethMonth[11]);
                          listOfMonths.Add(ethMonth[12]);
                          for (int i = 1; i <= _endMonth; i++)
                          {
                              listOfMonths.Add(ethMonth[i]);
                          }
                      }
                      else if (_startMonth == 12)
                      {
                          listOfMonths.Add(ethMonth[12]);
                          for (int i = 1; i <= _endMonth; i++)
                          {
                              listOfMonths.Add(ethMonth[i]);
                          }
                      }
                      else
                      {*/
                    for (int i = _startweek; i <= _endweek; i++)
                    {
                        listOfMonths.Add("Week" + i);
                    }
                }
                else
                {
                    onlyOneMonth = true;
                }
            }
           
            //   }
            /*else if (_repPeriod == 1)
            {
                if (_quarterStart == _quarterEnd)
                {
                    onlyOneQuarter = true;
                }
                else
                {
                    onlyOneQuarter = false;
                }
            }*/

            if (HMISMainPage.UseNewServiceDataElement2014)
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElementsNew";
                verticalSumIdTableName = "EthioHIMS_VerticalSumNew";
            }
            else
            {
                viewLabeIdTableName = "EthioHIMS_ServiceDataElements";
                verticalSumIdTableName = "EthioHIMS_VerticalSum";
            }

       //     string cmdQuery = " select * from " + viewLabeIdTableName + " where aggregationType != 0 and labelId is not null";

       //     SqlCommand toExecute = new SqlCommand(cmdQuery);


          //  toExecute.CommandTimeout = 4000; //300 // = 1000000;


        /*    DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
            string aggrLabelId = "";
            int aggrType = 0;
            labelIdAggrType.Clear();

            foreach (DataRow row in dt1.Rows)
            {
                aggrLabelId = row["labelId"].ToString();
                aggrType = Convert.ToInt16(row["aggregationType"].ToString());
                if (aggrLabelId != "")
                {
                    labelIdAggrType.Add(aggrLabelId, aggrType);
                }
            }*/
        }

        public static List<string> sortInstitutions(List<string> locatLst)
        {
            List<string> locationIdList = new List<string>();
            List<string> locationIdNotSorted = new List<string>();


            // Facility

            if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(locatLst[0]) != 4) // If the first ID is facility then no worries, it is already sorted 
            {

                foreach (string id in locatLst)
                {
                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) == 4)
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
                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) == 3)
                    {
                        locationIdList.Add(id);
                    }
                }

                // Zone
                foreach (string id in locationIdNotSorted)
                {
                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) == 5)
                    {
                        locationIdList.Add(id);
                    }
                }

                // Region
                foreach (string id in locationIdNotSorted)
                {
                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) == 2)
                    {
                        locationIdList.Add(id);
                    }
                }

                // Federal
                foreach (string id in locationIdNotSorted)
                {
                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id) == 1)
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

        private void initializeTarget()
        {
            int targetYear = _startYear;

            string locations = " and locationID in (";

            int count = 1;

            if ((_repPeriod == 1) && (_startMonth == 11)) // Quarterly Service Quarter 1
            {
                targetYear = targetYear + 1;
            }

            if (singleFacility == true)
            {
                locations = " and locationID = '" + singleFacilityLocationId + "'";
            }
            else
            {
                foreach (string locationID in locationsToView)
                {
                    if (locationsToView.Count == count) // last one no comma
                    {
                        locations += locationID + ")";
                    }
                    else
                    {
                        locations += locationID + ",";
                    }
                    count++;
                }
            }

            SqlCommand toExecute;
            queryTable = "select * from [EthEhmis_IndicatorsNewTarget]  where  " +
            " year = " + targetYear + locations;

            toExecute = new SqlCommand(queryTable);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;           

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                int theYear = Convert.ToInt16(row["year"]);
                int indicatorId = Convert.ToInt16(row["indicatorId"]);
                string thelocationId = row["locationID"].ToString();
                decimal targetValue = Convert.ToDecimal(row["value"]);

                string targetKey = indicatorId + "_" + thelocationId;
                targetValuesHash.Add(targetKey, targetValue);
            }

            //return targetValue;
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        public DataTable CreateReport()
        {
            //string cmdText = "SELECT * from  " + viewLabeIdTableName;
            // Use queryTable

            // Determine facilityType

            int aggregationLevel = 0;
            string facilityQuery = "";

            if (_showTarget)
            {
                initializeTarget();
            }

            if (singleFacility == true)
            {
                aggregationLevel = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(singleFacilityLocationId);

                if (aggregationLevel != 4) // not a facility but higher level
                {
                    facilityQuery = " where WorHo = 1 ";
                }
                else
                {
                    int selectedLocType = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityTypeId(singleFacilityLocationId);
                    //if (HMISMainPage.SelectedLocationType == 3)
                    if (selectedLocType == 3)
                    {
                        facilityQuery = " where hp = 1 ";
                    }
                    else if (selectedLocType == 2) // Government health center
                    {
                        facilityQuery = " where hc = 1 ";
                    }
                    else // other centers, clinics and hospitals use this variable
                    {
                        facilityQuery = " where hospital = 1 ";
                    }
                }
            }
            else
            {
                if (_noAdminLevel)
                {
                    facilityQuery = " where (hp = 1 or hospital = 1 or hc = 1) ";
                }
                else
                {
                    facilityQuery = " where WorHo = 1 ";
                }
            }

            string periodTypeQuery = "";
            if (_repPeriod == 0)
            {
                periodTypeQuery = " and periodType = 0 ";
            }
            else if (_repPeriod == 1)
            {
                if (_showOnlyQuarterlyDataElements == true)
                {
                    periodTypeQuery = "  and ( periodType = 1 or commonQuarterly = 1 ) ";
                }
                else
                {
                    periodTypeQuery = " and ( periodType = 1 or periodType = 0) ";
                }

            }
            else if (_repPeriod == 2)
            {
                periodTypeQuery = "  and ( periodType = 2 or commonAnnual = 1)";
            }
            //else if (_repPeriod == 2)
            //{
            //    annualQuery = " and (annual = 1 or common = 1) ";
            //}
            //else
            //{
            //    annualQuery = " and annual = 0 ";
            //}

            SqlCommand toExecute;
            queryTable = "select * from EthEhmis_PHEMIndicatorsNewDisplay  "
            + facilityQuery + periodTypeQuery +
            " order by sequenceNo";
            toExecute = new SqlCommand(queryTable);
            toExecute.CommandTimeout = 4000; //300 // = 1000000;

            string sno, indicatorName, target, baseline,
                numeratorName, numeratorLabelid, denominatorName, denominatorLabelid,
                actions, reportType, numDataEleClass, denomDataEleClass,
               indicatorValueString, numeratorValueString, denominatorValueString, indicatorMonthValueString,
               numeratorMonthValueString, denominatorMonthValueString = "", targetValueString = "",
               targetMonthValueString = "";

            bool readOnly;
            bool targetDivide = false;
            int sequenceNo;

            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];

            //foreach (string locationId in locationsToView)
            //{

            Hashtable numeratorHash = new Hashtable();
            Hashtable denominatorHash = new Hashtable();
            ArrayList numlistOfLabelIds = new ArrayList();
            string numlabelIdString = "(";

            Hashtable locationMonthLabelIdValue = new Hashtable();

            Hashtable locationLabelIdValue = getLocationIdLabelIdValue(out locationMonthLabelIdValue);

            decimal indicatorValue = 0;
            decimal targetValue = 0;
            decimal numeratorValue = 0;
            decimal denominatorValue = 0;
            decimal indicatorMonthValue = 0;
            decimal numeratorMonthValue = 0;
            decimal denominatorMonthValue = 0;
            decimal targetMonthValue = 0;
            reportIndicatorDataTable = new DataTable();

            reportIndicatorDataTable.Columns.Add("SNO", typeof(string));
            reportIndicatorDataTable.Columns.Add("Indicator_Name", typeof(string));

            if (_showNumDenom == true)
            {
                reportIndicatorDataTable.Columns.Add("Numerator_Name", typeof(string));
                reportIndicatorDataTable.Columns.Add("Denominator_Name", typeof(string));
            }

            foreach (string locationId in locationsToView)
            {
                string locationName = locationIdToName[locationId].ToString() + "_" + locationId;
                //string indicatorValCol = "Indicator Value Total " + locationName;
                //string numeratorValCol = "Numerator Value Total " + locationName;
                //string denominatorValCol = "Denominator Value Total " + locationName;
                string indicatorValCol = "";
                string numeratorValCol = "";
                string denominatorValCol = "";
                string targetValCol = "";

                if (singleFacility == true)
                {
                    indicatorValCol = "Indicator Value Total ";
                    numeratorValCol = "Numerator Value Total ";
                    denominatorValCol = "Denominator Value Total ";
                    targetValCol = "Target Value     Total ";
                }
                else
                {
                    indicatorValCol = "Indicator Value " + locationName;
                    numeratorValCol = "Numerator Value " + locationName;
                    denominatorValCol = "Denominator Value " + locationName;
                    targetValCol = "Target Value " + locationName;
                }

                string indicatorMonthCol = "";
                string numeratorMonthCol = "";
                string denominatorMonthCol = "";
                string targetMonthCol = "";

                if (singleFacility == true)
                {
                    if ((_repPeriod == 0) && (!onlyOneMonth))
                    {
                        //for (int i = _startMonth; i <= _endMonth; i++)
                        foreach (string theMonth in listOfMonths)
                        {
                            //indicatorMonthCol = "Indicator Value " + ethMonth[i] + " " + locationName;
                            indicatorMonthCol = "Indicator Value  " + theMonth; // +" " + locationName;
                            reportIndicatorDataTable.Columns.Add(indicatorMonthCol, typeof(string));
                            if (_showTarget)
                            {
                                targetMonthCol = "Target Value " + theMonth;
                                reportIndicatorDataTable.Columns.Add(targetMonthCol, typeof(string));
                            }
                        }
                    }
                    else if ((_repPeriod == 1) && (!onlyOneQuarter)) // Quarterly
                    {
                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            indicatorMonthCol = "Indicator Value   Q" + i + ":"; // +" " + locationName;
                            reportIndicatorDataTable.Columns.Add(indicatorMonthCol, typeof(string));

                            if (_showTarget)
                            {
                                targetMonthCol = "Target Value       Q" + i + ":"; // +" " + locationName;                            
                                reportIndicatorDataTable.Columns.Add(targetMonthCol, typeof(string));
                            }
                        }
                    }
                }

                reportIndicatorDataTable.Columns.Add(indicatorValCol, typeof(string));

                if (_showTarget)
                {
                    reportIndicatorDataTable.Columns.Add(targetValCol, typeof(string));
                }

                if (_showNumDenom == true)
                {
                    if (singleFacility == true)
                    {
                        if ((_repPeriod == 0) && (!onlyOneMonth))
                        {
                            //for (int i = _startMonth; i <= _endMonth; i++)
                            foreach (string theMonth in listOfMonths)
                            {
                                // numeratorMonthCol = "Numerator Value " + ethMonth[i] + " " + locationName;
                                numeratorMonthCol = "Numerator Value " + theMonth; // +" " + locationName;
                                reportIndicatorDataTable.Columns.Add(numeratorMonthCol, typeof(string));
                            }

                        }
                        else if ((_repPeriod == 1) && (!onlyOneQuarter)) // Quarterly
                        {
                            for (int i = _quarterStart; i <= _quarterEnd; i++)
                            {
                                numeratorMonthCol = "Numerator Value Q" + i + ":"; // +" " + locationName;
                                reportIndicatorDataTable.Columns.Add(numeratorMonthCol, typeof(string));
                            }
                        }
                    }

                    reportIndicatorDataTable.Columns.Add(numeratorValCol, typeof(string));

                    if (singleFacility == true)
                    {
                        if ((_repPeriod == 0) && (!onlyOneMonth))
                        {
                            //for (int i = _startMonth; i <= _endMonth; i++)
                            foreach (string theMonth in listOfMonths)
                            {
                                //denominatorMonthCol = "Denominator Value " + ethMonth[i] + " " + locationName;
                                denominatorMonthCol = "Denominator Value " + theMonth; // +" " + locationName;
                                reportIndicatorDataTable.Columns.Add(denominatorMonthCol, typeof(string));
                            }
                        }
                        else if ((_repPeriod == 1) && (!onlyOneQuarter)) // Quarterly
                        {
                            for (int i = _quarterStart; i <= _quarterEnd; i++)
                            {
                                denominatorMonthCol = "Denominator Value Q" + i + ":"; // +" " + locationName;
                                reportIndicatorDataTable.Columns.Add(denominatorMonthCol, typeof(string));
                            }
                        }
                    }
                    reportIndicatorDataTable.Columns.Add(denominatorValCol, typeof(string));
                }

                //// Columns related to Target

                //if (_showTarget == true)
                //{
                //    if (singleFacility == true)
                //    {
                //        if ((_repPeriod == 0) && (!onlyOneMonth))
                //        {
                //            //for (int i = _startMonth; i <= _endMonth; i++)
                //            foreach (string theMonth in listOfMonths)
                //            {
                //                targetMonthCol = "Target Value " + theMonth; // +" " + locationName;
                //                reportIndicatorDataTable.Columns.Add(targetMonthCol, typeof(string));
                //            }

                //        }
                //        else if ((_repPeriod == 1) && (!onlyOneQuarter)) // Quarterly
                //        {
                //            for (int i = _quarterStart; i <= _quarterEnd; i++)
                //            {
                //                targetMonthCol = "Target Value Q" + i + ":"; // +" " + locationName;
                //                reportIndicatorDataTable.Columns.Add(targetMonthCol, typeof(string));
                //            }
                //        }
                //    }                    
                //}
            }

            reportIndicatorDataTable.Columns.Add("ReadOnly", typeof(string));
            reportIndicatorDataTable.Columns.Add("Chart", typeof(string));
            int indicatorColSize = reportIndicatorDataTable.Columns.Count;

            // Total Month

            int numMonths = 0;
            if (_endweek == _startweek)
            {
                numMonths = 0;
            }
            else
            {
                numMonths = listOfMonths.Count;
            }

            int numQuarters = 0;
            if (_quarterStart == _quarterEnd)
            {
                numQuarters = 0;
            }
            else
            {
                numQuarters = (_quarterEnd - _quarterStart) + 1;
            }

            foreach (DataRow row in dt.Rows)
            {
                object[] indicatorCols = new object[indicatorColSize];
                sequenceNo = Convert.ToInt16(row["sequenceNo"]);

                sno = row["SNO"].ToString();
                indicatorName = row["indicatorName"].ToString();

                //indicator_value = row["indicator_value"].ToString();
                //target = row["target"].ToString();
                //baseline = row["baseline"].ToString();
                actions = row["actions"].ToString();
                numeratorName = row["numeratorName"].ToString();
                numeratorLabelid = row["numeratorLabelid"].ToString();
                readOnly = Convert.ToBoolean(Convert.ToByte(row["readOnly"].ToString()));
                targetDivide = Convert.ToBoolean(Convert.ToByte(row["targetDivide"].ToString()));

                numDataEleClass = row["NumeratorDataEleClass"].ToString();
                denomDataEleClass = row["DenominatorDataEleClass"].ToString();
                reportType = row["reportType"].ToString();

                denominatorName = row["denominatorName"].ToString();
                denominatorLabelid = row["denominatorLabelid"].ToString();

                if (indicatorName.Contains("á"))
                {
                    indicatorName = indicatorName.Replace("á", "");
                }

                if (denominatorName.Contains("á"))
                {
                    denominatorName = denominatorName.Replace("á", "");
                }

                if (numeratorName.Contains("á"))
                {
                    numeratorName = numeratorName.Replace("á", "");
                }

                indicatorCols[0] = sequenceNo;
                indicatorCols[1] = indicatorName;

                if (_showNumDenom == true)
                {
                    indicatorCols[2] = numeratorName;
                    indicatorCols[3] = denominatorName;
                }
                //reportIndicatorDataTable.Rows.Add(sno, indicatorName, numeratorName,
                //    denominatorName, indicatorValueString, numeratorValueString,
                //    denominatorValueString, readOnly);

                int i = 0;

                if (_showNumDenom == true)
                {
                    i = 4;
                }
                else
                {
                    i = 2;
                }

                int count = 0;

                if (singleFacility == true)
                {
                    if (_repPeriod == 0)
                    {
                        if (onlyOneMonth)
                        {
                            count++;
                            numeratorValue = 0;
                            denominatorValue = 0;
                            indicatorMonthValue = 0;
                            numeratorMonthValue = 0;
                            denominatorMonthValue = 0;

                            if (readOnly == false)
                            {
                                if (actions.Equals("Completeness"))
                                {
                                         
                                        toExecute  = new SqlCommand();
                                      //  DBConnHelper DBConnHelper = new DBConnHelper();
                                        toExecute.CommandText = "proc_Eth_WeeklyReportingCompleteness";
                                        toExecute.CommandType = CommandType.StoredProcedure;
                                        toExecute.Parameters.Add(new SqlParameter("@HMISCode", singleFacilityLocationId));
                                        toExecute.Parameters.Add(new SqlParameter("@Year", _startYear));
                                        toExecute.Parameters.Add(new SqlParameter("@StartWeek", _startweek));
                                        toExecute.Parameters.Add(new SqlParameter("@EndWeek", _startweek));
                                   
                                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                                    actions = "Completeness";

                                    DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
                                    indicatorMonthValue = Convert.ToDecimal( dt1.Rows[0]["WeeklyReportCompleteness"]);
                                    indicatorValue += indicatorMonthValue;
                                }
                                else if (actions.Equals("Timeliness"))
                                {
                                    // indicatorMonthValue = 0;
                                    indicatorValue = 0;
                                    actions = "Timeliness";
                                }
                                else
                                {
                                    indicatorValue = sumLabelIds(singleFacilityLocationId, locationLabelIdValue, locationMonthLabelIdValue, numeratorLabelid,
                                        denominatorLabelid, actions, numDataEleClass, denomDataEleClass,
                                        out numeratorValue, out denominatorValue, out indicatorMonthValue, out numeratorMonthValue, out denominatorMonthValue, _startMonth);
                                }

                                string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                                if (targetValuesHash[tarKey] != null)
                                {
                                    if (targetDivide == true)
                                    {
                                        targetValue =
                                            Convert.ToDecimal(targetValuesHash[tarKey]) * denomMulitply;
                                    }
                                    else
                                    {
                                        targetValue = Convert.ToDecimal(targetValuesHash[tarKey]);
                                    }

                                    targetValue = decimal.Round(targetValue, 2);
                                    if (actions.ToUpper() == "COUNT")
                                    {
                                        targetValue = decimal.Round(targetValue);
                                        targetValueString = targetValue.ToString();
                                    }
                                    else
                                    {
                                        targetValueString = targetValue.ToString() + "%";
                                    }
                                    if (actions.ToUpper() == "SUMNOPERCENT")
                                    {
                                        targetValueString = targetValueString.Replace("%", "");
                                    }
                                }
                                else
                                {
                                    targetValueString = "";
                                }

                                if (denominatorValue == -1)
                                {
                                    denominatorValueString = "";
                                }
                                else
                                {
                                    denominatorValueString = denominatorValue.ToString();
                                }

                                if ((numeratorValue != -1) && (indicatorValue != -1))
                                {
                                    if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                    {
                                        indicatorValue = decimal.Round(indicatorValue);
                                        indicatorValueString = indicatorValue.ToString();
                                        numeratorValueString = numeratorValue.ToString();
                                        indicatorMonthValueString = indicatorMonthValue.ToString();
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                    else if (actions.ToUpper() == "COMPLETENESS" || actions.ToUpper() == "TIMELINESS")
                                    {

                                        indicatorMonthValue = decimal.Round(indicatorMonthValue, 1);
                                        indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                       // completenessAverage = indicatorValue / Convert.ToDecimal(_startweek);
                                        //indicatorValue = indicatorValue / Convert.ToDecimal(numMonth);
                                        numeratorValueString = numeratorValue.ToString();
                                        indicatorValueString = decimal.Round(indicatorValue,1).ToString() + "%";
                                    }
                                    else
                                    {
                                        indicatorValueString = indicatorValue.ToString() + "%";
                                        numeratorValueString = numeratorValue.ToString();

                                        indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                }
                                else
                                {
                                    numeratorValueString = "";
                                    indicatorValueString = "";

                                    indicatorMonthValueString = "";
                                    numeratorMonthValueString = "";
                                }
                            }
                            else
                            {
                                indicatorValueString = "";
                                denominatorValueString = "";
                                numeratorValueString = "";
                                targetValueString = "";

                                indicatorMonthValueString = "";
                                denominatorMonthValueString = "";
                                numeratorMonthValueString = "";
                            }

                            indicatorCols[i] = indicatorValueString;

                            if ((_showNumDenom == true) && (_showTarget == false))
                            {
                                indicatorCols[i + 1] = numeratorValueString;
                                indicatorCols[i + 2] = denominatorValueString;
                                i = i + 3;
                            }
                            else if ((_showTarget == true) && (_showNumDenom == false))
                            {
                                indicatorCols[i + 1] = targetValueString;
                                i = i + 2;
                            }
                            else if ((_showNumDenom == true) && (_showTarget == true))
                            {
                                indicatorCols[i + 1] = targetValueString;
                                indicatorCols[i + 2] = numeratorValueString;
                                indicatorCols[i + 3] = denominatorValueString;
                                i = i + 4;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            int numColSize = 0;
                            int denomColSize = 0;

                            //for (int ct = _startMonth; ct <= _endMonth; ct++)
                            int colIncrement = i;
                            Decimal completenessAverage = 0;
                            foreach (string theMonth in listOfMonths)
                            {
                                count++;
                                numeratorValue = 0;
                                denominatorValue = 0;
                                indicatorMonthValue = 0;
                                numeratorMonthValue = 0;
                                denominatorMonthValue = 0;
                                completenessAverage = 0;
                                numeratorMonthValueString = "";
                                denominatorMonthValueString = "";
                                int numMonth = int.Parse(theMonth.Substring(4));

                                if (readOnly == false)
                                {
                                    if (actions.Equals("Completeness"))
                                {
                                         
                                        toExecute  = new SqlCommand();
                                      //  DBConnHelper DBConnHelper = new DBConnHelper();
                                        toExecute.CommandText = "proc_Eth_WeeklyReportingCompleteness";
                                        toExecute.CommandType = CommandType.StoredProcedure;
                                        toExecute.Parameters.Add(new SqlParameter("@HMISCode", singleFacilityLocationId));
                                        toExecute.Parameters.Add(new SqlParameter("@Year", _startYear));
                                        toExecute.Parameters.Add(new SqlParameter("@StartWeek", numMonth));
                                        toExecute.Parameters.Add(new SqlParameter("@EndWeek", numMonth));
                                   
                                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                                    actions = "Completeness";

                                    DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
                                    indicatorMonthValue = Convert.ToDecimal( dt1.Rows[0]["WeeklyReportCompleteness"]);
                                    indicatorValue += indicatorMonthValue;
                                }
                                    else if (actions.Equals("Timeliness"))
                                     {
                                        // indicatorMonthValue = 0;
                                         indicatorValue = 0;
                                         actions = "Timeliness";
                                     }
                                else
                                {
                                    indicatorValue = sumLabelIds(singleFacilityLocationId, locationLabelIdValue, locationMonthLabelIdValue, numeratorLabelid,
                                        denominatorLabelid, actions, numDataEleClass, denomDataEleClass,
                                        out numeratorValue, out denominatorValue, out indicatorMonthValue,
                                        out numeratorMonthValue, out denominatorMonthValue, numMonth);
                                     }
                                    string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                                  
                                    if (targetValuesHash[tarKey] != null)
                                    {
                                        decimal yearTarget = Convert.ToDecimal(targetValuesHash[tarKey]);

                                        if (targetDivide == true)
                                        {
                                            targetValue = yearTarget * denomMulitply;
                                        }
                                        else
                                        {
                                            targetValue = yearTarget;
                                        }

                                        targetValue = decimal.Round(targetValue, 2);
                                        if (actions.ToUpper() == "COUNT")
                                        {
                                            targetValue = decimal.Round(targetValue);
                                            targetValueString = targetValue.ToString();
                                        }
                                        else
                                        {
                                            targetValueString = targetValue.ToString() + "%";
                                        }
                                        if (actions.ToUpper() == "SUMNOPERCENT")
                                        {
                                            targetValueString = targetValueString.Replace("%", "");
                                        }
                                        if (targetDivide == true)
                                        {
                                            targetMonthValue = yearTarget / 12;
                                        }
                                        else
                                        {
                                            targetMonthValue = yearTarget;
                                        }
                                        targetMonthValue = decimal.Round(targetMonthValue, 2);
                                        if (actions.ToUpper() == "COUNT")
                                        {
                                            targetMonthValue = decimal.Round(targetMonthValue, 2);
                                            targetMonthValueString = targetMonthValue.ToString();
                                        }
                                        else
                                        {
                                            targetMonthValueString = targetMonthValue.ToString() + "%";
                                        }

                                        if (actions.ToUpper() == "SUMNOPERCENT")
                                        {
                                            targetMonthValueString = targetMonthValueString.Replace("%", "");
                                        }
                                    }
                                    else
                                    {
                                        targetValueString = "";
                                        targetMonthValueString = "";
                                    }


                                    if (denominatorValue == -1)
                                    {
                                        denominatorValueString = "";
                                        denominatorMonthValueString = "";
                                    }
                                    else
                                    {
                                        denominatorValueString = denominatorValue.ToString();
                                        denominatorMonthValueString = denominatorMonthValue.ToString();

                                        if (denominatorMonthValue == -1)
                                        {
                                            denominatorMonthValueString = "";
                                        }
                                    }

                                    if ((numeratorValue != -1) && (indicatorValue != -1))
                                    {
                                        if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                        {
                                            //Rounds the number into two decimal numbers
                                            indicatorValue = decimal.Round(indicatorValue);
                                            numeratorValue = decimal.Round(numeratorValue);
                                            indicatorMonthValue = decimal.Round(indicatorMonthValue);
                                            numeratorMonthValue = decimal.Round(numeratorMonthValue);
                                            indicatorValueString = indicatorValue.ToString();
                                            numeratorValueString = numeratorValue.ToString();
                                            indicatorMonthValueString = indicatorMonthValue.ToString();
                                            numeratorMonthValueString = numeratorMonthValue.ToString();
                                        }
                                        else if (actions.ToUpper() == "COMPLETENESS" || actions.ToUpper() == "TIMELINESS")
                                        {

                                            indicatorMonthValue = decimal.Round(indicatorMonthValue, 1);
                                            indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                            completenessAverage = indicatorValue / Convert.ToDecimal(numMonth);
                                            //indicatorValue = indicatorValue / Convert.ToDecimal(numMonth);
                                            indicatorValueString = completenessAverage.ToString() + "%";
                                        }

                                        else
                                        {
                                            indicatorValueString = indicatorValue.ToString() + "%";
                                            numeratorValueString = numeratorValue.ToString();

                                            indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                            numeratorMonthValueString = numeratorMonthValue.ToString();
                                        }

                                        if ((numeratorMonthValue == -1) && (indicatorMonthValue == -1))
                                        {
                                            indicatorMonthValueString = "";
                                            numeratorMonthValueString = "";
                                        }
                                    }
                                    else
                                    {
                                        numeratorValueString = "";
                                        indicatorValueString = "";

                                        indicatorMonthValueString = "";
                                        numeratorMonthValueString = "";
                                    }
                                }
                                else
                                {
                                    indicatorValueString = "";
                                    denominatorValueString = "";
                                    numeratorValueString = "";
                                    targetValueString = "";

                                    indicatorMonthValueString = "";
                                    denominatorMonthValueString = "";
                                    numeratorMonthValueString = "";
                                    targetMonthValueString = "";
                                }

                                indicatorCols[i] = indicatorMonthValueString;

                                if ((_showNumDenom == true) && (_showTarget == false))
                                {
                                    numColSize = i + numMonths + 1;
                                    indicatorCols[numColSize] = numeratorMonthValueString;
                                    denomColSize = numColSize + numMonths + 1;
                                    indicatorCols[denomColSize] = denominatorMonthValueString;
                                    i++;
                                }
                                else if ((_showNumDenom == false) && (_showTarget == true))
                                {
                                    indicatorCols[i + 1] = targetMonthValueString;
                                    i = i + 2;
                                }
                                else if ((_showNumDenom == true) && (_showTarget == true))
                                {
                                    i++;
                                    colIncrement++;
                                    indicatorCols[i] = targetMonthValueString;

                                    numColSize = colIncrement + (numMonths * 2) + 1;
                                    indicatorCols[numColSize] = numeratorMonthValueString;
                                    denomColSize = numColSize + numMonths + 1;
                                    indicatorCols[denomColSize] = denominatorMonthValueString;
                                    i++;
                                }
                                else
                                {
                                    i++;
                                }

                            }
                            //string indicatorValueString = "";

                            if (readOnly == false)
                            {
                                if (denominatorValue == -1)
                                {
                                    denominatorValueString = "";
                                }
                                else
                                {
                                    denominatorValueString = denominatorValue.ToString();
                                }

                                if ((numeratorValue != -1) && (indicatorValue != -1))
                                {
                                    if (actions.ToUpper() == "COMPLETENESS")
                                    {
                                        indicatorValue = decimal.Round(completenessAverage, 1);
                                        indicatorValueString = indicatorValue.ToString() + "%";
                                        numeratorValueString = numeratorValue.ToString(); 
                                    }
                                    else if (actions.ToUpper() == "TIMELINESS")
                                    {
                                        indicatorValue = decimal.Round(completenessAverage, 1);
                                        indicatorValueString = indicatorValue.ToString() + "%";
                                        numeratorValueString = numeratorValue.ToString(); 
                                    }
                                    else if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                    {
                                        indicatorValue = decimal.Round(indicatorValue);
                                        indicatorValueString = indicatorValue.ToString();
                                        numeratorValueString = numeratorValue.ToString();
                                        indicatorMonthValueString = indicatorMonthValue.ToString();
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }

                                    else
                                    {
                                        indicatorValueString = indicatorValue.ToString() + "%";
                                        numeratorValueString = numeratorValue.ToString();

                                        indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                }
                                else
                                {
                                    numeratorValueString = "";
                                    indicatorValueString = "";

                                    indicatorMonthValueString = "";
                                    numeratorMonthValueString = "";
                                }
                            }
                            else
                            {
                                indicatorValueString = "";
                                denominatorValueString = "";
                                numeratorValueString = "";
                                targetValueString = "";

                                indicatorMonthValueString = "";
                                denominatorMonthValueString = "";
                                numeratorMonthValueString = "";
                                targetMonthValueString = "";
                            }

                            indicatorCols[i] = indicatorValueString;

                            if ((_showNumDenom == true) && (_showTarget == false))
                            {
                                numColSize++;
                                denomColSize++;

                                indicatorCols[numColSize] = numeratorValueString;
                                indicatorCols[denomColSize] = denominatorValueString;
                                i = denomColSize + 1;
                            }
                            else if ((_showNumDenom == false) && (_showTarget == true))
                            {
                                indicatorCols[i + 1] = targetValueString;
                                i = i + 2;
                            }
                            else if ((_showNumDenom == true) && (_showTarget == true))
                            {
                                i++;
                                indicatorCols[i] = targetValueString;

                                numColSize++;
                                denomColSize++;

                                indicatorCols[numColSize] = numeratorValueString;
                                indicatorCols[denomColSize] = denominatorValueString;
                                i = denomColSize + 1;

                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                    else if (_repPeriod == 1)
                    {
                        if (onlyOneQuarter)
                        {
                            count++;
                            numeratorValue = 0;
                            denominatorValue = 0;
                            indicatorMonthValue = 0;
                            numeratorMonthValue = 0;
                            denominatorMonthValue = 0;

                            if (readOnly == false)
                            {
                                indicatorValue = sumLabelIds(singleFacilityLocationId, locationLabelIdValue, locationMonthLabelIdValue, numeratorLabelid,
                                    denominatorLabelid, actions, numDataEleClass, denomDataEleClass,
                                    out numeratorValue, out denominatorValue, out indicatorMonthValue, out numeratorMonthValue, out denominatorMonthValue, _startMonth);

                                string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                                if (targetValuesHash[tarKey] != null)
                                {
                                    if (targetDivide == true)
                                    {
                                        targetValue =
                                            Convert.ToDecimal(targetValuesHash[tarKey]) * denomMulitply;
                                    }
                                    else
                                    {
                                        targetValue = Convert.ToDecimal(targetValuesHash[tarKey]);
                                    }

                                    targetValue = decimal.Round(targetValue, 2);
                                    if (actions.ToUpper() == "COUNT")
                                    {
                                        targetValue = decimal.Round(targetValue);
                                        targetValueString = targetValue.ToString();
                                    }
                                    else
                                    {
                                        targetValueString = targetValue.ToString() + "%";
                                    }
                                    if (actions.ToUpper() == "SUMNOPERCENT")
                                    {
                                        targetValueString = targetValueString.Replace("%", "");
                                    }
                                }
                                else
                                {
                                    targetValueString = "";
                                }

                                if (denominatorValue == -1)
                                {
                                    denominatorValueString = "";
                                }
                                else
                                {
                                    denominatorValueString = denominatorValue.ToString();
                                }

                                if ((numeratorValue != -1) && (indicatorValue != -1))
                                {
                                    if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                    {
                                        indicatorValue = decimal.Round(indicatorValue);
                                        indicatorValueString = indicatorValue.ToString();
                                        numeratorValueString = numeratorValue.ToString();
                                        indicatorMonthValueString = indicatorMonthValue.ToString();
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                    else
                                    {
                                        indicatorValueString = indicatorValue.ToString() + "%";
                                        numeratorValueString = numeratorValue.ToString();

                                        indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                }
                                else
                                {
                                    numeratorValueString = "";
                                    indicatorValueString = "";

                                    indicatorMonthValueString = "";
                                    numeratorMonthValueString = "";
                                }
                            }
                            else
                            {
                                indicatorValueString = "";
                                denominatorValueString = "";
                                numeratorValueString = "";
                                targetValueString = "";

                                indicatorMonthValueString = "";
                                denominatorMonthValueString = "";
                                numeratorMonthValueString = "";
                            }

                            indicatorCols[i] = indicatorValueString;

                            if ((_showNumDenom == true) && (_showTarget == false))
                            {
                                indicatorCols[i + 1] = numeratorValueString;
                                indicatorCols[i + 2] = denominatorValueString;
                                i = i + 3;
                            }
                            else if ((_showTarget == true) && (_showNumDenom == false))
                            {
                                indicatorCols[i + 1] = targetValueString;
                                i = i + 2;
                            }
                            else if ((_showNumDenom == true) && (_showTarget == true))
                            {
                                indicatorCols[i + 1] = targetValueString;
                                indicatorCols[i + 2] = numeratorValueString;
                                indicatorCols[i + 3] = denominatorValueString;
                                i = i + 4;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            int numColSize = 0;
                            int denomColSize = 0;

                            //for (int ct = _startMonth; ct <= _endMonth; ct++)
                            int colIncrement = i;
                            for (int j = _quarterStart; j <= _quarterEnd; j++)
                            {
                                count++;
                                numeratorValue = 0;
                                denominatorValue = 0;
                                indicatorMonthValue = 0;
                                numeratorMonthValue = 0;
                                denominatorMonthValue = 0;
                                numeratorMonthValueString = "";
                                denominatorMonthValueString = "";

                                //int numMonth = Convert.ToInt32(monthIdHash[theMonth]);

                                if (readOnly == false)
                                {
                                    indicatorValue = sumLabelIds(singleFacilityLocationId, locationLabelIdValue, locationMonthLabelIdValue, numeratorLabelid,
                                        denominatorLabelid, actions, numDataEleClass, denomDataEleClass,
                                        out numeratorValue, out denominatorValue, out indicatorMonthValue,
                                        out numeratorMonthValue, out denominatorMonthValue, j);

                                    string tst = indicatorName;

                                    string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                                    if (targetValuesHash[tarKey] != null)
                                    {
                                        decimal yearTarget = Convert.ToDecimal(targetValuesHash[tarKey]);

                                        if (targetDivide == true)
                                        {
                                            targetValue = yearTarget * denomMulitply;
                                        }
                                        else
                                        {
                                            targetValue = yearTarget;
                                        }

                                        targetValue = decimal.Round(targetValue, 2);
                                        if (actions.ToUpper() == "COUNT")
                                        {
                                            targetValue = decimal.Round(targetValue);
                                            targetValueString = targetValue.ToString();
                                        }
                                        else
                                        {
                                            targetValueString = targetValue.ToString() + "%";
                                        }
                                        if (actions.ToUpper() == "SUMNOPERCENT")
                                        {
                                            targetValueString = targetValueString.Replace("%", "");
                                        }
                                        if (targetDivide == true)
                                        {
                                            targetMonthValue = yearTarget / 4;
                                        }
                                        else
                                        {
                                            targetMonthValue = yearTarget;
                                        }
                                        targetMonthValue = decimal.Round(targetMonthValue, 2);

                                        if (actions.ToUpper() == "COUNT")
                                        {
                                            targetMonthValue = decimal.Round(targetMonthValue);
                                            targetMonthValueString = targetMonthValue.ToString();
                                        }
                                        else
                                        {
                                            targetMonthValueString = targetMonthValue.ToString() + "%";
                                        }

                                        if (actions.ToUpper() == "SUMNOPERCENT")
                                        {
                                            targetMonthValueString = targetMonthValueString.Replace("%", "");
                                        }
                                    }
                                    else
                                    {
                                        targetValueString = "";
                                        targetMonthValueString = "";
                                    }

                                    if (denominatorValue == -1)
                                    {
                                        denominatorValueString = "";
                                    }
                                    else
                                    {
                                        denominatorValueString = denominatorValue.ToString();
                                        denominatorMonthValueString = denominatorMonthValue.ToString();

                                        if (denominatorMonthValue == -1)
                                        {
                                            denominatorMonthValueString = "";
                                        }
                                    }

                                    if ((numeratorValue != -1) && (indicatorValue != -1))
                                    {
                                        if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                        {
                                            indicatorValue = decimal.Round(indicatorValue);
                                            indicatorValueString = indicatorValue.ToString();
                                            numeratorValueString = numeratorValue.ToString();
                                            indicatorMonthValueString = indicatorMonthValue.ToString();
                                            numeratorMonthValueString = numeratorMonthValue.ToString();
                                        }
                                        else
                                        {
                                            indicatorValueString = indicatorValue.ToString() + "%";
                                            numeratorValueString = numeratorValue.ToString();

                                            indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                            numeratorMonthValueString = numeratorMonthValue.ToString();
                                        }

                                        if ((numeratorMonthValue == -1) && (indicatorMonthValue == -1))
                                        {
                                            indicatorMonthValueString = "";
                                            numeratorMonthValueString = "";
                                        }
                                    }
                                    else
                                    {
                                        numeratorValueString = "";
                                        indicatorValueString = "";

                                        indicatorMonthValueString = "";
                                        numeratorMonthValueString = "";
                                    }
                                }
                                else
                                {
                                    indicatorValueString = "";
                                    denominatorValueString = "";
                                    numeratorValueString = "";
                                    targetValueString = "";

                                    indicatorMonthValueString = "";
                                    denominatorMonthValueString = "";
                                    numeratorMonthValueString = "";
                                    targetMonthValueString = "";
                                }

                                indicatorCols[i] = indicatorMonthValueString;

                                if ((_showNumDenom == true) && (_showTarget == false))
                                {
                                    numColSize = i + numQuarters + 1;
                                    indicatorCols[numColSize] = numeratorMonthValueString;
                                    denomColSize = numColSize + numQuarters + 1;
                                    indicatorCols[denomColSize] = denominatorMonthValueString;
                                    i++;
                                }
                                else if ((_showNumDenom == false) && (_showTarget == true))
                                {
                                    indicatorCols[i + 1] = targetMonthValueString;
                                    i = i + 2;
                                }
                                else if ((_showNumDenom == true) && (_showTarget == true))
                                {
                                    i++;
                                    colIncrement++;
                                    indicatorCols[i] = targetMonthValueString;
                                    decimal test = targetValue;

                                    numColSize = colIncrement + (numQuarters * 2) + 1;
                                    indicatorCols[numColSize] = numeratorMonthValueString;
                                    denomColSize = numColSize + numQuarters + 1;
                                    indicatorCols[denomColSize] = denominatorMonthValueString;
                                    i++;

                                }
                                else
                                {
                                    i++;
                                }

                            }
                            //string indicatorValueString = "";

                            if (readOnly == false)
                            {
                                //string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                                //if (targetValuesHash[tarKey] != null)
                                //{
                                //    if (targetDivide == true)
                                //    {
                                //        targetValue =
                                //            Convert.ToDecimal(targetValuesHash[tarKey]) * denomMulitply;
                                //    }
                                //    else
                                //    {
                                //        targetValue = Convert.ToDecimal(targetValuesHash[tarKey]);
                                //    }

                                //    targetValue = decimal.Round(targetValue, 2);

                                //    if (actions.ToUpper() == "COUNT")
                                //    {
                                //        targetValue = decimal.Round(targetValue, 0);
                                //        targetValueString = targetValue.ToString();
                                //    }
                                //    else
                                //    {
                                //        targetValueString = targetValue.ToString() + "%";
                                //    }
                                //}
                                //else
                                //{
                                //    targetValueString = "";
                                //}

                                if (denominatorValue == -1)
                                {
                                    denominatorValueString = "";
                                }
                                else
                                {
                                    denominatorValueString = denominatorValue.ToString();
                                }

                                if ((numeratorValue != -1) && (indicatorValue != -1))
                                {
                                    if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                    {
                                        indicatorValue = decimal.Round(indicatorValue);
                                        indicatorValueString = indicatorValue.ToString();
                                        numeratorValueString = numeratorValue.ToString();
                                        indicatorMonthValueString = indicatorMonthValue.ToString();
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                    else
                                    {
                                        indicatorValueString = indicatorValue.ToString() + "%";
                                        numeratorValueString = numeratorValue.ToString();

                                        indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                        numeratorMonthValueString = numeratorMonthValue.ToString();
                                    }
                                }
                                else
                                {
                                    numeratorValueString = "";
                                    indicatorValueString = "";

                                    indicatorMonthValueString = "";
                                    numeratorMonthValueString = "";
                                }
                            }
                            else
                            {
                                indicatorValueString = "";
                                denominatorValueString = "";
                                numeratorValueString = "";
                                targetValueString = "";

                                indicatorMonthValueString = "";
                                denominatorMonthValueString = "";
                                numeratorMonthValueString = "";
                                targetMonthValueString = "";
                            }

                            indicatorCols[i] = indicatorValueString;

                            if ((_showNumDenom == true) && (_showTarget == false))
                            {
                                numColSize++;
                                denomColSize++;

                                indicatorCols[numColSize] = numeratorValueString;
                                indicatorCols[denomColSize] = denominatorValueString;
                                i = denomColSize + 1;
                            }
                            else if ((_showNumDenom == false) && (_showTarget == true))
                            {
                                indicatorCols[i + 1] = targetValueString;
                                i = i + 2;
                            }
                            else if ((_showNumDenom == true) && (_showTarget == true))
                            {
                                i++;
                                indicatorCols[i] = targetValueString;

                                numColSize++;
                                denomColSize++;

                                indicatorCols[numColSize] = numeratorValueString;
                                indicatorCols[denomColSize] = denominatorValueString;
                                i = denomColSize + 1;


                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                    else // yearly and anything else missed
                    {
                        count++;
                        numeratorValue = 0;
                        denominatorValue = 0;
                        indicatorMonthValue = 0;
                        numeratorMonthValue = 0;
                        denominatorMonthValue = 0;

                        if (readOnly == false)
                        {
                            indicatorValue = sumLabelIds(singleFacilityLocationId, locationLabelIdValue, locationMonthLabelIdValue, numeratorLabelid,
                                denominatorLabelid, actions, numDataEleClass, denomDataEleClass,
                                out numeratorValue, out denominatorValue, out indicatorMonthValue, out numeratorMonthValue, out denominatorMonthValue, _startMonth);
                            string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                            if (targetValuesHash[tarKey] != null)
                            {
                                if (targetDivide == true)
                                {
                                    targetValue =
                                        Convert.ToDecimal(targetValuesHash[tarKey]) * denomMulitply;
                                }
                                else
                                {
                                    targetValue = Convert.ToDecimal(targetValuesHash[tarKey]);
                                }

                                targetValue = decimal.Round(targetValue, 2);
                                if (actions.ToUpper() == "COUNT")
                                {
                                    targetValue = decimal.Round(targetValue);
                                    targetValueString = targetValue.ToString();
                                }
                                else
                                {
                                    targetValueString = targetValue.ToString() + "%";
                                }
                                if (actions.ToUpper() == "SUMNOPERCENT")
                                {
                                    targetValueString = targetValueString.Replace("%", "");
                                }
                            }
                            else
                            {
                                targetValueString = "";
                            }

                            if (denominatorValue == -1)
                            {
                                denominatorValueString = "";
                            }
                            else
                            {
                                denominatorValueString = denominatorValue.ToString();
                            }

                            if ((numeratorValue != -1) && (indicatorValue != -1))
                            {
                                if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                {
                                    indicatorValue = decimal.Round(indicatorValue);
                                    indicatorValueString = indicatorValue.ToString();
                                    numeratorValueString = numeratorValue.ToString();
                                    indicatorMonthValueString = indicatorMonthValue.ToString();
                                    numeratorMonthValueString = numeratorMonthValue.ToString();
                                }
                                else
                                {
                                    indicatorValueString = indicatorValue.ToString() + "%";
                                    numeratorValueString = numeratorValue.ToString();

                                    indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                    numeratorMonthValueString = numeratorMonthValue.ToString();
                                }
                            }
                            else
                            {
                                numeratorValueString = "";
                                indicatorValueString = "";

                                indicatorMonthValueString = "";
                                numeratorMonthValueString = "";
                            }
                        }
                        else
                        {
                            indicatorValueString = "";
                            denominatorValueString = "";
                            numeratorValueString = "";
                            targetValueString = "";

                            indicatorMonthValueString = "";
                            denominatorMonthValueString = "";
                            numeratorMonthValueString = "";
                        }

                        indicatorCols[i] = indicatorValueString;

                        if ((_showNumDenom == true) && (_showTarget == false))
                        {
                            indicatorCols[i + 1] = numeratorValueString;
                            indicatorCols[i + 2] = denominatorValueString;
                            i = i + 3;
                        }
                        else if ((_showTarget == true) && (_showNumDenom == false))
                        {
                            indicatorCols[i + 1] = targetValueString;
                            i = i + 2;
                        }
                        else if ((_showNumDenom == true) && (_showTarget == true))
                        {
                            indicatorCols[i + 1] = targetValueString;
                            indicatorCols[i + 2] = numeratorValueString;
                            indicatorCols[i + 3] = denominatorValueString;
                            i = i + 4;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (string locationId in locationsToView)
                    {
                        count++;
                        numeratorValue = 0;
                        denominatorValue = 0;
                        indicatorMonthValue = 0;
                        numeratorMonthValue = 0;
                        denominatorMonthValue = 0;

                        if (readOnly == false)
                        {
                            if (actions.Equals("Completeness"))
                                {
                                    indicatorValue = 0; 
                                        toExecute  = new SqlCommand();
                                      //  DBConnHelper DBConnHelper = new DBConnHelper();
                                        toExecute.CommandText = "proc_Eth_WeeklyReportingCompleteness";
                                        toExecute.CommandType = CommandType.StoredProcedure;
                                        toExecute.Parameters.Add(new SqlParameter("@HMISCode", locationId));
                                        toExecute.Parameters.Add(new SqlParameter("@Year", _startYear));
                                        toExecute.Parameters.Add(new SqlParameter("@StartWeek", _startweek));
                                        toExecute.Parameters.Add(new SqlParameter("@EndWeek", _endweek));
                                   
                                    toExecute.CommandTimeout = 4000; //300 // = 1000000;
                                    actions = "Completeness";

                                    DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
                                    indicatorMonthValue = Convert.ToDecimal( dt1.Rows[0]["WeeklyReportCompleteness"]);
                                    indicatorValue += indicatorMonthValue;
                                }
                            else if (actions.Equals("Timeliness"))
                             {
                                 // indicatorMonthValue = 0;
                                 indicatorValue = 0;
                                 actions = "Timeliness";
                             }
                             else
                             {
                                 indicatorValue = sumLabelIds(locationId, locationLabelIdValue, locationMonthLabelIdValue,
                                     numeratorLabelid, denominatorLabelid, actions, numDataEleClass, denomDataEleClass,
                                     out numeratorValue, out denominatorValue, out indicatorMonthValue, out numeratorMonthValue, out denominatorMonthValue, -1);
                             }
                            if (_showTarget == true)
                            {
                                // string tarKey = sequenceNo + "_" + singleFacilityLocationId;
                                string tarKey = sequenceNo + "_" + locationId;
                                if (targetValuesHash[tarKey] != null)
                                {
                                    if (targetDivide == true)
                                    {
                                        targetValue =
                                            Convert.ToDecimal(targetValuesHash[tarKey]) * denomMulitply;
                                    }
                                    else
                                    {
                                        targetValue =
                                           Convert.ToDecimal(targetValuesHash[tarKey]);
                                    }

                                    targetValue = decimal.Round(targetValue, 2);
                                    if (actions.ToUpper() == "COUNT")
                                    {
                                        targetValue = decimal.Round(targetValue);
                                        targetValueString = targetValue.ToString();
                                    }
                                    else
                                    {
                                        targetValueString = targetValue.ToString() + "%";
                                    }
                                    if (actions.ToUpper() == "SUMNOPERCENT")
                                    {
                                        targetValueString = targetValueString.Replace("%", "");
                                    }
                                }
                                else
                                {
                                    targetValueString = "";
                                }
                            }

                            if (denominatorValue == -1)
                            {
                                denominatorValueString = "";
                            }
                            else
                            {
                                denominatorValueString = denominatorValue.ToString();
                            }

                            if ((numeratorValue != -1) && (indicatorValue != -1))
                            {
                                if (actions.ToUpper() == "COUNT" || (actions.ToUpper() == "SUMNOPERCENT"))
                                {
                                    indicatorValue = decimal.Round(indicatorValue);
                                    indicatorValueString = indicatorValue.ToString();
                                    numeratorValueString = numeratorValue.ToString();
                                    indicatorMonthValueString = indicatorMonthValue.ToString();
                                    numeratorMonthValueString = numeratorMonthValue.ToString();
                                }
                                else if (actions.ToUpper() == "COMPLETENESS" || actions.ToUpper() == "TIMELINESS")
                                {

                                    indicatorMonthValue = decimal.Round(indicatorMonthValue, 1);
                                    indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                    // completenessAverage = indicatorValue / Convert.ToDecimal(_startweek);
                                    //indicatorValue = indicatorValue / Convert.ToDecimal(numMonth);
                                    numeratorValueString = numeratorValue.ToString();
                                    indicatorValueString = decimal.Round(indicatorValue, 1).ToString() + "%";
                                }
                                else
                                {
                                    indicatorValueString = indicatorValue.ToString() + "%";
                                    numeratorValueString = numeratorValue.ToString();

                                    indicatorMonthValueString = indicatorMonthValue.ToString() + "%";
                                    numeratorMonthValueString = numeratorMonthValue.ToString();
                                }
                            }
                            else
                            {
                                numeratorValueString = "";
                                indicatorValueString = "";

                                indicatorMonthValueString = "";
                                numeratorMonthValueString = "";
                            }
                        }
                        else
                        {
                            indicatorValueString = "";
                            denominatorValueString = "";
                            numeratorValueString = "";
                            targetValueString = "";


                            indicatorMonthValueString = "";
                            denominatorMonthValueString = "";
                            numeratorMonthValueString = "";
                        }

                        indicatorCols[i] = indicatorValueString;

                        if ((_showNumDenom == true) && (_showTarget == false))
                        {
                            indicatorCols[i + 1] = numeratorValueString;
                            indicatorCols[i + 2] = denominatorValueString;
                            i = i + 3;
                        }
                        else if ((_showTarget == true) && (_showNumDenom == false))
                        {
                            indicatorCols[i + 1] = targetValueString;
                            i = i + 2;
                        }
                        else if ((_showNumDenom == true) && (_showTarget == true))
                        {
                            indicatorCols[i + 1] = targetValueString;
                            indicatorCols[i + 2] = numeratorValueString;
                            indicatorCols[i + 3] = denominatorValueString;
                            i = i + 4;
                        }
                        else
                        {
                            i++;
                        }

                    }
                }

                indicatorCols[i] = readOnly;
                int co = reportIndicatorDataTable.Rows.Count;
                if (indicatorName.ToUpper().Contains("REPORT"))
                {
                }
                try
                {
                    //if (reportIndicatorDataTable.Rows[co - 1]["ReadOnly"].ToString() == "False")
                    //{
                    //    reportIndicatorDataTable.Rows[co - 1]["Chart"] = "Chart";
                    //}
                    if (readOnly == false)
                    {
                        indicatorCols[i + 1] = "Chart";
                    }
                }
                catch
                { }
                reportIndicatorDataTable.Rows.Add(indicatorCols);

                //reportIndicatorDataTable.Rows.Add(sno, indicatorName, numeratorName, 
                //    denominatorName,indicatorValueString,numeratorValueString,
                //    denominatorValueString, readOnly);
            }


            return reportIndicatorDataTable;
        }

        private Hashtable getLocationIdLabelIdValue(out Hashtable locationIdMonthLabelValue)
        {
            string idQuery = "";
            Hashtable locationIdLabelValue = new Hashtable();
            locationIdMonthLabelValue = new Hashtable();
            string dataEleClasses = " and (dataEleClass = 25 or dataEleClass = 26 ) ";

            int denominatorYear = _startYear;
            int monthCnt = 0;

            if (onlyOneMonth == true)
            {
                monthCnt = 1;
            }
            else
            {
                monthCnt = listOfMonths.Count;
            }

            int numDaysMonthTotal = monthCnt * 7;
            int numDaysMonth = 7;
            int numDaysQuarterTotal = ((_quarterEnd - _quarterStart) + 1) * 90;
            int numDaysQuarter = 90;

            foreach (string locationID in locationsToView)
            {
                string id = locationID;

                //if (id == "10")
                //    id = "14";
                //else if (id == "1033")
                //    id = "2010100";

                int aggregationLevel = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getAggregationLevelId(id);

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

                string monthYearQueryGroup1 = "";
                string monthYearQueryGroup2 = "";
                if (_repPeriod == 2) // Annual Service, should be fiscal year
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

                    denominatorYear = denominatorYear + 1;
                    //monthYearQ1QueryGroup1 = "  where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear ";
                    //monthYearQ1QueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";

                }
                else if (_repPeriod == 1) // Quarterly Service
                {
                    // monthYearQueryGroup1 = " where  Month  >= " + _startMonth + " and Month <=  " + _endMonth + " and Year =  " + _startYear;

                    monthYearQueryGroup1 = "  where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear ";

                    monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";

                }
                else if (_repPeriod == 0) // Monthly Service
                {
                    // monthYearQueryGroup1 = monthYearQueryGroup2 = " where  Month =  " + _startMonth + "  and level > 0 and Year =  " + _startYear;

                    // Change for the Fiscal Year using Months: Merra Kokebie, merraK@tiethio.org
                    // January 23, 2014
                    //monthYearQueryGroup1 = monthYearQueryGroup2 = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear ";

                    if (_startMonth == 11)
                    {
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthYearQueryGroup1 = "  where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear - 1 ";
                            monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";
                        }
                        else
                        {
                            monthYearQueryGroup1 = "	where  (((Month  = 11 or Month = 12) and  Year = @StartYear - 1 ) " +
                            " or ((Month  >= 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                            monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";
                        }
                    }
                    else if (_startMonth == 12)
                    {
                        if ((_endMonth == 11) || (_endMonth == 12))
                        {
                            monthYearQueryGroup1 = monthYearQueryGroup2 = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear - 1 ";
                            monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";
                        }
                        else
                        {
                            monthYearQueryGroup1 = "	where  (((Month  = 11 or Month = 12) and  Year = @StartYear - 1 ) " +
                            " or ((Month  = 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                            monthYearQueryGroup2 = "  where   Month  = @EndMonth and Year = @endYear  ";
                        }
                    }
                    else
                    {
                        monthYearQueryGroup1 = "	where  Week  >=@StartMonth and Week <= @EndMonth   and Year = @StartYear";
                        monthYearQueryGroup2 = "  where   Week  = @EndMonth and Year = @endYear  ";
                    }
                }
                // For denominators, you have to use locationId only in the EthEhmis_HmisValue, as
                // denominators will not aggregate
                string denominatorIdQuery = " and locationID = @newIdentification ";
                DateTime date = new System.DateTime();
                string startdate = WeekDayStart(denominatorYear, _endweek);
                string ethDate =  UtilitiesNew.GeneralUtilities.LocaleConfigurationProvider.GetDateDisplayString(DateTime.Parse(startdate));
                ethDate = ethDate.Substring(ethDate.Length - 4);
                string queryTable = " select week, dataEleClass,labelId, sum(value) as value \n" +
                                    " from EthEhmis_HmisValue " + monthYearQueryGroup1 +
                    //" and (labelId != 4492 and labelId != 4493) \n" +
                                    dataEleClasses +
                                    idQuery + " or ( DataEleClass = 4 and year = " + ethDate + " \n" +
                                    denominatorIdQuery + ")" + " group by dataEleClass,labelId,week \n";

                //string queryTable = " select month, dataEleClass,labelId, sum(value) as value \n" +
                //                    " from EthEhmis_HmisValue " + monthYearQueryGroup1 +
                //                    " and (labelId != 4492 and labelId != 4493) \n" +
                //                    dataEleClasses +
                //                    idQuery + " or ( DataEleClass = 4 and year = " + denominatorYear + " \n" +
                //                    denominatorIdQuery + ")" + " group by dataEleClass,labelId,month \n";

                //queryTable += "\n union \n " +
                //              " select EthEhmis_HmisValue.month, dataEleClass, EthEhmis_HmisValue.labelId, \n" +
                //              " sum(value) as value " + " from EthEhmis_HmisValue \n" +
                //              " inner join " + viewLabeIdTableName + " on \n" +
                //              viewLabeIdTableName + ".labelId" + " = \n" +
                //              " EthEhmis_HmisValue.labelId \n" +
                //              monthYearQueryGroup2 +
                //              idQuery + " and " + viewLabeIdTableName + ".aggregationType = 1 \n" +
                //              " and (EthEhmis_HmisValue.labelId = 4492 or EthEhmis_HmisValue.labelId = 4493) \n" +
                //              " group by EthEhmis_HmisValue.DataEleClass, \n" +
                //              " EthEhmis_HmisValue.LabelId, EthEhmis_HmisValue.month \n";

                SqlCommand toExecute = new SqlCommand(queryTable);

                toExecute.Parameters.AddWithValue("StartMonth", _startweek);
                toExecute.Parameters.AddWithValue("EndMonth", _endweek);
                toExecute.Parameters.AddWithValue("startYear", _startYear);
                toExecute.Parameters.AddWithValue("endYear", _endYear);
                toExecute.Parameters.AddWithValue("newIdentification", locationID);

                toExecute.CommandTimeout = 4000; //300 // = 1000000;


                DataTable dt1 = _helper.GetDataSet(toExecute).Tables[0];
                //int tstCnt = dt1.Rows.Count;
                Hashtable labelIdValue = new Hashtable();
                Hashtable labelIdMonthValue = new Hashtable();
                string labelId, hmisValue, dataEleClass = "";
                string monthValue = "";
                // Retrieve the data for the label IDs
                foreach (DataRow row in dt1.Rows)
                {
                    dataEleClass = row["dataEleClass"].ToString();
                    labelId = row["labelId"].ToString();
                    hmisValue = row["value"].ToString();
                    monthValue = row["week"].ToString();

                    string dEleClassLabelId = dataEleClass + "_" + labelId;
                    string dEleClassLabelIdNum = "6" + "_" + numDaysLabelId;

                    string monthDecLabelId = "M" + "_" + monthValue + "_" + dataEleClass + "_" + labelId;
                    string monthDecLabelIdNum = "M" + "_" + monthValue + "_" + "6" + "_" + numDaysLabelId;

                    string quarterDecLabelId = "";
                    string quarterDecLabelIdNum = "";

                    if ((monthValue == "11") || (monthValue == "12") || (monthValue == "1"))
                    {
                        quarterDecLabelId = "Q" + "_" + 1 + "_" + dataEleClass + "_" + labelId;
                        quarterDecLabelIdNum = "Q" + "_" + 1 + "_" + "6" + "_" + numDaysLabelId;
                    }
                    else if ((monthValue == "2") || (monthValue == "3") || (monthValue == "4"))
                    {
                        quarterDecLabelId = "Q" + "_" + 2 + "_" + dataEleClass + "_" + labelId;
                        quarterDecLabelIdNum = "Q" + "_" + 2 + "_" + "6" + "_" + numDaysLabelId;
                    }
                    else if ((monthValue == "5") || (monthValue == "6") || (monthValue == "7"))
                    {
                        quarterDecLabelId = "Q" + "_" + 3 + "_" + dataEleClass + "_" + labelId;
                        quarterDecLabelIdNum = "Q" + "_" + 3 + "_" + "6" + "_" + numDaysLabelId;
                    }
                    else if ((monthValue == "8") || (monthValue == "9") || (monthValue == "10"))
                    {
                        quarterDecLabelId = "Q" + "_" + 4 + "_" + dataEleClass + "_" + labelId;
                        quarterDecLabelIdNum = "Q" + "_" + 4 + "_" + "6" + "_" + numDaysLabelId;
                    }

                    //labelIdValue.Add(dEleClassLabelId, hmisValue);
                    // labelIdValue.Add(dEleClassLabelId, hmisValue);

                    if ((_repPeriod == 1) && (!onlyOneQuarter))
                    {
                        if (labelIdMonthValue[quarterDecLabelId] != null)
                        {
                            if (labelIdAggrType[labelId] == null) // for end of month values
                            {
                                //if (Convert.ToInt16(labelIdAggrType[labelId]) != 1)
                                //{
                                decimal prevVal = Convert.ToDecimal(labelIdMonthValue[quarterDecLabelId]);
                                decimal newVal = Convert.ToDecimal(hmisValue) + prevVal;
                                labelIdMonthValue[quarterDecLabelId] = newVal.ToString();
                                //}
                            }
                        }
                        else
                        {
                            if (labelIdAggrType[labelId] != null) // for end of month values
                            {
                                if (Convert.ToInt16(labelIdAggrType[labelId]) == 1)
                                {
                                    //if ((monthValue == "1") || (monthValue == "4") 
                                    //    || (monthValue == "7") || (monthValue == "10"))
                                    if (endQuarterMonths.Contains(monthValue))
                                    {
                                        labelIdMonthValue[quarterDecLabelId] = hmisValue;
                                    }
                                }
                            }
                            else
                            {
                                labelIdMonthValue[quarterDecLabelId] = hmisValue;
                                labelIdMonthValue[quarterDecLabelIdNum] = numDaysQuarter;
                            }
                        }

                        if (labelIdValue[dEleClassLabelId] != null)
                        {
                            if (labelIdAggrType[labelId] == null) // for end of month values
                            {
                                //if (Convert.ToInt16(labelIdAggrType[labelId]) != 1)
                                //{
                                decimal prevVal = Convert.ToDecimal(labelIdValue[dEleClassLabelId]);
                                decimal newVal = Convert.ToDecimal(hmisValue) + prevVal;
                                labelIdValue[dEleClassLabelId] = newVal.ToString();
                                //}
                            }
                        }
                        else
                        {
                            if (labelIdAggrType[labelId] != null) // for end of month values
                            {
                                if (Convert.ToInt16(labelIdAggrType[labelId]) == 1)
                                {
                                    if (monthValue == _endMonth.ToString())
                                    {
                                        labelIdValue[dEleClassLabelId] = hmisValue;
                                    }
                                }
                            }
                            else
                            {
                                labelIdValue[dEleClassLabelId] = hmisValue;
                                labelIdValue[dEleClassLabelIdNum] = numDaysQuarterTotal;
                            }
                        }
                    }
                    else
                    {
                        if (labelIdValue[dEleClassLabelId] != null)
                        {
                            if (labelIdAggrType[labelId] == null) // for end of month values
                            {
                                decimal prevVal = Convert.ToDecimal(labelIdValue[dEleClassLabelId]);
                                decimal newVal = Convert.ToDecimal(hmisValue) + prevVal;
                                labelIdValue[dEleClassLabelId] = newVal.ToString();
                            }
                            labelIdMonthValue[monthDecLabelId] = hmisValue;
                        }
                        else
                        {
                            if (labelIdAggrType[labelId] != null) // for end of month values
                            {
                                if (Convert.ToInt16(labelIdAggrType[labelId]) == 1)
                                {
                                    if (monthValue == _endMonth.ToString())
                                    {
                                        labelIdValue[dEleClassLabelId] = hmisValue;
                                    }
                                }
                            }
                            else
                            {
                                labelIdValue[dEleClassLabelId] = hmisValue;
                                labelIdValue[dEleClassLabelIdNum] = numDaysMonthTotal;
                            }

                            labelIdMonthValue[monthDecLabelId] = hmisValue;
                            labelIdMonthValue[monthDecLabelIdNum] = numDaysMonth;
                        }
                    }
                }

                locationIdLabelValue.Add(locationID, labelIdValue);
                locationIdMonthLabelValue.Add(locationID, labelIdMonthValue);
            }
            return locationIdLabelValue;
        }
        public static string WeekDayStart(int Year, int WeekNumber)
        {
            /*  DateTime start = new DateTime(Year, 1, 4);
               start = start.AddDays(-((int)start.DayOfWeek));
               start = start.AddDays(7 * (WeekNumber - 1) + 1);

               return start.ToString();*/
            DateTime jan1 = new DateTime(Year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            System.Globalization.CultureInfo cal = new System.Globalization.CultureInfo("es-ES", false);

            int firstWeek = cal.Calendar.GetWeekOfYear(firstThursday, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = WeekNumber;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            DateTime result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3).ToShortDateString();


        }
        private decimal sumLabelIds(string locationId, Hashtable locationLabelIdValue, Hashtable locationMonthLabelIdValue, string numeratorLabelid,
            string denominatorLabelid, string actions, string numDataEleClass,
            string denomDataEleClass, out decimal num, out decimal denom, out decimal indicatorMonthValue,
            out decimal numMonthValue, out decimal denomMonthValue, int monthNum)
        {

            Hashtable labelIdValue = new Hashtable();
            Hashtable labelIdMonthValue = new Hashtable();

            if (locationLabelIdValue[locationId] != null)
            {
                labelIdValue = (Hashtable)locationLabelIdValue[locationId];
                labelIdMonthValue = (Hashtable)locationMonthLabelIdValue[locationId];
            }

            char splitChar = ',';
            numeratorLabelid = numeratorLabelid.Trim();
            numDataEleClass = numDataEleClass.Trim();
            denominatorLabelid = denominatorLabelid.Trim();
            denomDataEleClass = denomDataEleClass.Trim();

            bool populationDenom = false;

            decimal indicatorValue = 0;

            string[] splitNumLabelIds = numeratorLabelid.Split(splitChar);
            string[] splitNumDataEleClass = numDataEleClass.Split(splitChar);

            string[] splitDenomLabelIds = denominatorLabelid.Split(splitChar);
            string[] splitDenomDataEleClass = denomDataEleClass.Split(splitChar);

            num = 0;
            denom = 0;

            indicatorMonthValue = 0;
            numMonthValue = 0;
            denomMonthValue = 0;

            bool numData = false;
            bool denomData = false;
            bool numMonthData = false;
            bool denomMonthData = false;

            if ((actions.ToUpper() == "SUM") && (numeratorLabelid.Trim().ToUpper() == "ALL"))
            {
                decimal val = 0;
                foreach (DictionaryEntry de in labelIdValue)
                {
                    string fieldName = de.Key as string;
                    foreach (string dataEleClass in splitNumDataEleClass)
                    {
                        if (fieldName.Contains(dataEleClass + "_"))
                        {
                            val += Convert.ToDecimal(de.Value);
                            numData = true;
                        }
                    }
                }
                num = val;
            }

            else if (actions.ToUpper() == "SUMSUBTRACT")
            {
                foreach (string labelid in splitNumLabelIds)
                {
                    // based on the label id, determine if you are going to do summation
                    // or subtraction
                    string numOperator = "sum";
                    string numNewLabelId = labelid;

                    if (labelid.Contains("-"))
                    {
                        numOperator = "sub";
                        numNewLabelId = numNewLabelId.Replace("-", "");
                    }

                    numNewLabelId = numNewLabelId.Replace("+", "");

                    foreach (string dataEleClass in splitNumDataEleClass)
                    {
                        string trimmedDataEleClass = dataEleClass.Trim();
                        string trimmedLabelId = trimmedDataEleClass + "_" + numNewLabelId.Trim();

                        if (labelIdValue[trimmedLabelId] != null)
                        {
                            if (numOperator == "sub")
                            {
                                num = num - Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                            }
                            else
                            {
                                num = num + Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                            }
                            numData = true;
                        }

                        if (singleFacility == true)
                        {
                            if ((_repPeriod == 0) && (!onlyOneMonth))
                            {
                                trimmedLabelId = "M_" + monthNum + "_" + trimmedLabelId;
                                if (labelIdMonthValue[trimmedLabelId] != null)
                                {
                                    if (numOperator == "sub")
                                    {
                                        numMonthValue = numMonthValue - Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    else
                                    {
                                        numMonthValue = numMonthValue + Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    numMonthData = true;
                                }
                            }
                            else if ((_repPeriod == 1) && (!onlyOneQuarter))
                            {
                                trimmedLabelId = "Q_" + monthNum + "_" + trimmedLabelId;
                                if (labelIdMonthValue[trimmedLabelId] != null)
                                {
                                    if (numOperator == "sub")
                                    {
                                        numMonthValue = numMonthValue - Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    else
                                    {
                                        numMonthValue = numMonthValue + Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    numMonthData = true;
                                }
                            }
                        }
                    }
                }
            }
           /* else if (actions.Equals("Completeness", System.StringComparison.CurrentCultureIgnoreCase))
            {
                
                foreach (string labelid in splitNumLabelIds)
                {
                    // based on the label id, determine if you are going to do summation
                    // or subtraction
                    string numOperator = "sum";
                    string numNewLabelId = labelid;

                    if (labelid.Contains("-"))
                    {
                        numOperator = "sub";
                        numNewLabelId = numNewLabelId.Replace("-", "");
                    }

                    numNewLabelId = numNewLabelId.Replace("+", "");

                    foreach (string dataEleClass in splitNumDataEleClass)
                    {
                        string trimmedDataEleClass = dataEleClass.Trim();
                        string trimmedLabelId = trimmedDataEleClass + "_" + numNewLabelId.Trim();

                        if (labelIdValue[trimmedLabelId] != null)
                        {
                            if (numOperator == "sub")
                            {
                                num = num - Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                            }
                            else
                            {
                                num = num + Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                            }
                            numData = true;
                        }

                        if (singleFacility == true)
                        {
                            if ((_repPeriod == 0) && (!onlyOneMonth))
                            {
                                trimmedLabelId = "M_" + monthNum + "_" + trimmedLabelId;
                                if (labelIdMonthValue[trimmedLabelId] != null)
                                {
                                    if (numOperator == "sub")
                                    {
                                        numMonthValue = numMonthValue - Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    else
                                    {
                                        numMonthValue = numMonthValue + Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    numMonthData = true;
                                }
                            }
                            else if ((_repPeriod == 1) && (!onlyOneQuarter))
                            {
                                trimmedLabelId = "Q_" + monthNum + "_" + trimmedLabelId;
                                if (labelIdMonthValue[trimmedLabelId] != null)
                                {
                                    if (numOperator == "sub")
                                    {
                                        numMonthValue = numMonthValue - Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    else
                                    {
                                        numMonthValue = numMonthValue + Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    }
                                    numMonthData = true;
                                }
                            }
                        }
                    }
                }
            }*/
            else
            {
                foreach (string labelid in splitNumLabelIds)
                {
                    foreach (string dataEleClass in splitNumDataEleClass)
                    {
                        string trimmedDataEleClass = dataEleClass.Trim();
                        string trimmedLabelId = trimmedDataEleClass + "_" + labelid.Trim();

                        if (labelIdValue[trimmedLabelId] != null)
                        {
                            num += Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                            numData = true;
                        }

                        if (singleFacility == true)
                        {
                            if ((_repPeriod == 0) && (!onlyOneMonth))
                            {
                                trimmedLabelId = "M_" + monthNum + "_" + trimmedLabelId;
                                if (labelIdMonthValue[trimmedLabelId] != null)
                                {
                                    numMonthValue += Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    numMonthData = true;
                                }
                            }
                            else if ((_repPeriod == 1) && (!onlyOneQuarter))
                            {
                                trimmedLabelId = "Q_" + monthNum + "_" + trimmedLabelId;
                                if (labelIdMonthValue[trimmedLabelId] != null)
                                {
                                    numMonthValue += Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                    numMonthData = true;
                                }
                            }
                        }
                    }
                }
            }

            // Denominator Calculations

            if ((actions.ToUpper() == "SUM") && (denominatorLabelid.Trim().ToUpper() == "ALL"))
            {
                decimal val = 0;
                foreach (DictionaryEntry de in labelIdValue)
                {
                    string fieldName = de.Key as string;
                    foreach (string dataEleClass in splitDenomDataEleClass)
                    {
                        if (fieldName.Contains(dataEleClass + "_"))
                        {
                            val += Convert.ToDecimal(de.Value);
                        }
                    }
                }
                denom = val;
                denomData = true;

                if (denom != 0)
                {
                    if (actions.ToUpper() == "SUMNOPERCENT")
                    {
                        indicatorValue = (num / denom);
                    }
                    else
                    {
                        indicatorValue = (num / denom) * 100;
                    }
                    //indicatorValue = (num / denom) * 100;
                }
            }
            else if (actions.ToUpper() != "COUNT")
            {
                if ((denominatorLabelid.Trim() != "") && (denominatorLabelid.Trim().ToUpper() != "N/A"))
                {
                    if (actions.ToUpper() == "SUMDENOMMULTIPLY")
                    {
                        denom = 1;
                        denomMonthValue = 1;
                        foreach (string labelid in splitDenomLabelIds)
                        {
                            foreach (string dataEleClass in splitDenomDataEleClass)
                            {
                                string trimmedDataEleClass = dataEleClass.Trim();
                                string trimmedLabelId = trimmedDataEleClass + "_" + labelid.Trim();

                                if (labelIdValue[trimmedLabelId] != null)
                                {
                                    decimal multiDenom = Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                                    denom *= Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                                    denomData = true;
                                    denomMonthData = true;
                                    if (dataEleClass == "4")
                                    {
                                        populationDenom = true;
                                    }
                                    else
                                    {
                                        populationDenom = false;
                                    }
                                }
                                else
                                {
                                    if (dataEleClass == "4")
                                    {
                                        populationDenom = true;
                                    }
                                    else
                                    {
                                        populationDenom = false;
                                    }
                                }


                                if (populationDenom != true)
                                {
                                    if (singleFacility == true)
                                    {
                                        if ((_repPeriod == 0) && (!onlyOneMonth))
                                        {
                                            trimmedLabelId = "M_" + monthNum + "_" + trimmedLabelId;
                                            if (labelIdMonthValue[trimmedLabelId] != null)
                                            {
                                                denomMonthValue *= Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                                denomMonthData = true;
                                            }
                                        }
                                        else if ((_repPeriod == 1) && (!onlyOneQuarter))
                                        {
                                            trimmedLabelId = "Q_" + monthNum + "_" + trimmedLabelId;
                                            if (labelIdMonthValue[trimmedLabelId] != null)
                                            {
                                                decimal testData = Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                                denomMonthValue *= Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                                denomMonthData = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (denom != 0)
                        {
                            if (populationDenom == true)
                            {
                                denom = denom * denomMulitply;
                            }
                            if (actions.ToUpper() == "SUMNOPERCENT")
                            {
                                indicatorValue = (num / denom);
                            }
                            else
                            {
                                indicatorValue = (num / denom) * 100;
                            }
                            //indicatorValue = (num / denom) * 100;
                        }
                    }
                    else
                    {

                        foreach (string labelid in splitDenomLabelIds)
                        {
                            foreach (string dataEleClass in splitDenomDataEleClass)
                            {
                                string trimmedDataEleClass = dataEleClass.Trim();
                                string trimmedLabelId = trimmedDataEleClass + "_" + labelid.Trim();

                                if (labelIdValue[trimmedLabelId] != null)
                                {
                                    denom += Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                                    denomData = true;
                                    if (dataEleClass == "4")
                                    {
                                        populationDenom = true;
                                        denomMonthData = true;
                                    }
                                    else
                                    {
                                        populationDenom = false;
                                    }
                                }
                                else
                                {
                                    if (dataEleClass == "4")
                                    {
                                        populationDenom = true;
                                    }
                                    else
                                    {
                                        populationDenom = false;
                                    }
                                }

                                if (populationDenom != true)
                                {
                                    if (singleFacility == true)
                                    {
                                        if ((_repPeriod == 0) && (!onlyOneMonth))
                                        {
                                            trimmedLabelId = "M_" + monthNum + "_" + trimmedLabelId;
                                            if (labelIdMonthValue[trimmedLabelId] != null)
                                            {
                                                denomMonthValue += Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                                denomMonthData = true;
                                            }
                                        }
                                        else if ((_repPeriod == 1) && (!onlyOneQuarter))
                                        {
                                            trimmedLabelId = "Q_" + monthNum + "_" + trimmedLabelId;
                                            if (labelIdMonthValue[trimmedLabelId] != null)
                                            {
                                                denomMonthValue += Convert.ToDecimal(labelIdMonthValue[trimmedLabelId]);
                                                denomMonthData = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (denom != 0)
                    {
                        if (populationDenom == true)
                        {
                            if (_repPeriod == 0)
                            {
                                denomMonthValue = denom / 1;
                            }
                            else if (_repPeriod == 1) // quarterly
                            {
                                denomMonthValue = denom / 4;
                            }
                            denom = denom * denomMulitply;
                            // we need the monthly
                        }
                        if (actions.ToUpper() == "SUMNOPERCENT")
                        {
                            indicatorValue = (num / denom);
                        }
                        else
                        {
                            if (denomDataEleClass == "4")
                            {
                                indicatorValue = (num / denom) * 100000;
                            }
                            else
                            indicatorValue = (num / denom) * 100;
                        }
                        //indicatorValue = (num / denom) * 100;

                        if (singleFacility == true)
                        {
                            if ((_repPeriod == 0) && (!onlyOneMonth))
                            {
                                if (denomMonthValue != 0)
                                {
                                    if (actions.ToUpper() == "SUMNOPERCENT")
                                    {

                                        indicatorMonthValue = (numMonthValue / denomMonthValue);
                                    }
                                    else
                                    {
                                        if (denomDataEleClass == "4")
                                        {
                                            indicatorMonthValue = (numMonthValue / denomMonthValue) * 100000;
                                        }
                                        else
                                        indicatorMonthValue = (numMonthValue / denomMonthValue) * 100;
                                    }
                                    //indicatorMonthValue = (numMonthValue / denomMonthValue) * 100;
                                }
                            }
                            else if ((_repPeriod == 1) && (!onlyOneQuarter))
                            {
                                if (denomMonthValue != 0)
                                {
                                    if (actions.ToUpper() == "SUMNOPERCENT")
                                    {

                                        indicatorMonthValue = (numMonthValue / denomMonthValue);
                                    }
                                    else
                                    {
                                        indicatorMonthValue = (numMonthValue / denomMonthValue) * 100;
                                    }
                                    //indicatorMonthValue = (numMonthValue / denomMonthValue) * 100;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                denom = -1;
                indicatorValue = decimal.Round(num, 1);
                indicatorMonthValue = decimal.Round(numMonthValue, 1); ;
            }

            if (denomData == true)
            {
                denom = decimal.Round(denom, 1);
                if (singleFacility == true)
                {
                    if ((_repPeriod == 0) && (!onlyOneMonth))
                    {
                        denomMonthValue = decimal.Round(denomMonthValue, 1);
                    }
                    else if ((_repPeriod == 1) && (!onlyOneQuarter))
                    {
                        denomMonthValue = decimal.Round(denomMonthValue, 1);
                    }
                }

                if (denomMonthData == false)
                {
                    denomMonthValue = -1;
                }
            }
            else
            {
                denom = -1;
                denomMonthValue = -1;
            }

            if (numData == true)
            {
                if (actions.ToUpper() == "SUMHUNDREDMINUS")
                {
                    if (indicatorValue != 0)
                    {
                        indicatorValue = 100 - indicatorValue;
                        indicatorMonthValue = 100 - indicatorMonthValue;
                    }
                }

                num = decimal.Round(num, 1);
                indicatorValue = decimal.Round(indicatorValue, 1);
                //Rounds the vale to two decimal numbers
                if (singleFacility == true)
                {
                    if ((_repPeriod == 0) && (!onlyOneMonth))
                    {
                        numMonthValue = decimal.Round(numMonthValue, 1);

                        indicatorMonthValue = decimal.Round(indicatorMonthValue, 1);
                    }
                    else if ((_repPeriod == 1) && (!onlyOneQuarter))
                    {
                        numMonthValue = decimal.Round(numMonthValue, 1);
                        indicatorMonthValue = decimal.Round(indicatorMonthValue, 1);
                    }
                }


                if (numMonthData == false)
                {
                    numMonthValue = -1;
                    indicatorMonthValue = -1;
                }
            }
            else
            {
                num = -1;
                indicatorValue = -1;
            }

            return indicatorValue;
        }

        public ArrayList GetLabels(string verticalSumID)
        {

            ArrayList labels = new ArrayList();
            string cmdText = " select sequenceNO  from  " + verticalSumIdTableName +
                             " where ID = " + verticalSumID;

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.CommandTimeout = 4000; //300 // = 1000000;
            DataTable dt2 = _helper.GetDataSet(toExecute).Tables[0];

            foreach (DataRow row in dt2.Rows)
            {
                string sequenceno = row["sequenceNo"].ToString();
                string labelID = seqLabelIDHash[sequenceno].ToString();
                labels.Add(labelID);
            }
            return labels;
        }
    }
}

