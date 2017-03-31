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
using eHMISWebApi.Controllers;


namespace eHMIS.HMIS.ReportAggregation.CustomReports
{

    public class CustomDiseaseReportAggr : ICustomReport
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

        public CustomDiseaseReportAggr(List<string> locations, int startMonth, int endMonth,
            int yearStart, int yearEnd, int quarterStart, int quarterEnd, int repKind,
            int repPeriod, bool isCacheEnabled)
        {
            setCorrectLanguageTable();

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

            level1Cache = false;
            level2Cache = false;

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
                    viewLabeIdTableName = opdViewTable;  //"EthioHIMS_QuarterOPDDiseaseView4";

                    reportOpdDataTable = new DataTable();

                    reportOpdDataTable.Columns.Add(languageHash["icd10"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["disease_facility"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["morbiditymaleslessthanfour"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["morbiditymalesfivetofourteen"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["morbiditymalesgreaterorequaltofifteen"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["morbidityfemaleslessthanfour"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["morbidityfemalesfivetofourteen"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add(languageHash["morbidityfemalesgreaterorequaltofifteen"].ToString(), typeof(string));
                    reportOpdDataTable.Columns.Add("Format", typeof(string));
                }
                else // IPD Disease
                {
                    viewLabeIdTableName = ipdViewTable; //"EthioHIMS_QuarterIPDDiseaseView";

                    reportIpdDataTable = new DataTable();

                    reportIpdDataTable.Columns.Add(languageHash["icd10"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["disease_facility"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["morbiditymaleslessthanfour"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["morbiditymalesfivetofourteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["morbiditymalesgreaterorequaltofifteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["morbidityfemaleslessthanfour"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["morbidityfemalesfivetofourteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["morbidityfemalesgreaterorequaltofifteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["mortalitymaleslessthanfour"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["mortalitymalesfivetofourteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["mortalitymalesgreaterorequaltofifteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["mortalityfemaleslessthanfour"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["mortalityfemalesfivetofourteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add(languageHash["mortalityfemalesgreaterorequaltofifteen"].ToString(), typeof(string));
                    reportIpdDataTable.Columns.Add("Format", typeof(string));
                }
            }
        }

        private void setStartingMonth(int quarterStart, int quarterEnd)
        {
            switch (quarterStart)
            {
                case 1:
                    _startMonth = 11;
                    //_endMonth = 1;
                    _startYear = _startYear - 1;
                    break;
                case 2:
                    _startMonth = 2;
                    // _endMonth = 4;
                    break;
                case 3:
                    _startMonth = 5;
                    // _endMonth = 7;
                    break;
                case 4:
                    _startMonth = 8;
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
            string cmdText = "select facilityname from " + facilityTable + " where hmiscode = @locationID";
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
            aggregateDataHash.Clear();

            if (_repKind == 1)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassQuery = " and DataEleClass = 10 ";
                }
                else
                {
                    dataEleClassQuery = " and DataEleClass = 8 ";
                }
            }
            else if (_repKind == 2)
            {
                if (HMISMainPage.UseNewVersion == true)
                {
                    dataEleClassQuery = " and DataEleClass = 11 ";
                }
                else
                {
                    dataEleClassQuery = " and DataEleClass = 2 "; // Morbidity
                }
            }

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
                if (_repPeriod == 0) // Monthly 
                {
                    string monthQueryGroup = "";
                    if (HMISMainPage.UseNewVersion == true)
                    {
                        monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear ";
                    }
                    else
                    {
                        //monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear and level = 0 ";

                        if (_startMonth == 11)
                        {
                            if ((_endMonth == 11) || (_endMonth == 12))
                            {
                                monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @startYear - 1 ";
                            }
                            else
                            {
                                monthQueryGroup = "	where  (((Month  = 11 or Month = 12) and  Year = @startYear - 1 ) " +
                                " or ((Month  >= 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                            }
                        }
                        else if (_startMonth == 12)
                        {
                            if ((_endMonth == 11) || (_endMonth == 12))
                            {
                                monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @startYear - 1 ";
                            }
                            else
                            {
                                monthQueryGroup = "	where  (((Month  = 11 or Month = 12) and  Year = @startYear - 1 ) " +
                                " or ((Month  >= 1 and Month <= @EndMonth) and Year = @startYear)) and level = 0";
                            }
                        }
                        else
                        {
                            monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @startYear ";
                        }
                    }

                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     "   sum(Value) as Value from EthEhmis_HmisValue  " +
                     //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                     monthQueryGroup +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    if (singleFacility == true)
                    {
                        if (HMISMainPage.UseNewVersion == true)
                        {
                            monthQueryGroup = "	where  Month  = @StartMonth  and Year = @startYear ";
                        }
                        else
                        {
                            monthQueryGroup = "	where  Month  = @StartMonth  and Year = @startYear and level = 0  ";

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
                            int month = i;

                            int yearToUse = 0;

                            if ((i == 11) || (i == 12))
                            {
                                yearToUse = _startYear - 1;
                            }
                            else
                            {
                                yearToUse = _startYear;
                            }

                            cmdText =
                            "	select cast(LabelID as VarChar) + '_" + ethMonth[i] + "_" + locationID + "' as LabelID, " +
                            "   sum(Value) as Value from EthEhmis_HmisValue  " +
                            //"	where  Month  = @StartMonth  and Year = @startYear " +
                            monthQueryGroup +
                               dataEleClassQuery + idQuery +
                            "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, month, _endMonth, yearToUse);
                        }
                    }
                }
                else if ((_repPeriod == 1) && (_quarterStart == 1)) // Quarterly for Quarter 1
                {
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     "  sum(Value) as Value from EthEhmis_HmisValue  " +
                     "  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                     "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);



                    if (singleFacility == true)
                    {
                        // to re-set the Start Year for Quarter 1
                        _startYear = _startYear + 1;

                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            setStartingMonth(i, i);
                            string theQuarter = "quarter" + i;

                            if (i == 1) // First Quarter
                            {

                                cmdText =
                                 "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                 "  where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                 "  (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";

                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);
                                _startYear = _startYear + 1; // to re-set the Start Year for Quarter 1
                            }
                            else
                            {
                                cmdText =
                                "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                                   dataEleClassQuery + idQuery +
                                "	group by DataEleClass,  LabelID  ";

                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);
                            }


                        }
                        setStartingMonth(_quarterStart, _quarterEnd);
                    }
                }
                else if (_repPeriod == 1) // Quarterly other than Quarter 1
                {
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     "   sum(Value) as Value from EthEhmis_HmisValue  " +
                     "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                        dataEleClassQuery + idQuery +
                     "	group by DataEleClass,  LabelID  ";
                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                    if (singleFacility == true)
                    {

                        for (int i = _quarterStart; i <= _quarterEnd; i++)
                        {
                            setStartingMonth(i, i);
                            string theQuarter = "quarter" + i;
                            cmdText =
                             "	select cast(LabelID as VarChar) + '_" + theQuarter + "_" + locationID + "' as LabelID, " +
                             "   sum(Value) as Value from EthEhmis_HmisValue  " +
                             "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                        }
                        setStartingMonth(_quarterStart, _quarterEnd);
                    }
                }
                else if (_repPeriod == 2) // Yearly
                {
                    cmdText =
                     "	select cast(LabelID as VarChar) + '_" + locationID + "' as LabelID, " +
                     "   sum(Value) as Value from EthEhmis_HmisValue  " +
                     "	where  Year >= @startYear and Year <= @endYear  " +
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
                             "   sum(Value) as Value from EthEhmis_HmisValue  " +
                             "	where  Year = @startYear " +
                                dataEleClassQuery + idQuery +
                             "	group by DataEleClass,  LabelID  ";
                            addToHashTable(cmdText, id, _startMonth, _endMonth, i);
                        }
                    }
                }

                if (_repKind == 2)
                {
                    // Mortality IPD

                    if (HMISMainPage.UseNewVersion == true)
                    {
                        dataEleClassQuery = " and DataEleClass = 12 ";
                    }
                    else
                    {
                        dataEleClassQuery = " and DataEleClass = 3 ";
                    }

                    if (_repPeriod == 0) // Monthly Report
                    {
                        string monthQueryGroup = "";
                        if (HMISMainPage.UseNewVersion == true)
                        {
                            monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear ";
                        }
                        else
                        {
                            // monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear and level = 0 ";

                            if (_startMonth == 11)
                            {
                                if ((_endMonth == 11) || (_endMonth == 12))
                                {
                                    monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear - 1 ";
                                }
                                else
                                {
                                    monthQueryGroup = "	where  (((Month  = 11 or Month = 12) and  Year = @StartYear - 1 ) " +
                                    " or ((Month  >= 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                                }
                            }
                            else if (_startMonth == 12)
                            {
                                if ((_endMonth == 11) || (_endMonth == 12))
                                {
                                    monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear - 1 ";
                                }
                                else
                                {
                                    monthQueryGroup = "	where  (((Month  = 11 or Month = 12) and  Year = @StartYear - 1 ) " +
                                    " or ((Month  >= 1 and Month <= @EndMonth) and Year = @StartYear)) and level = 0";
                                }
                            }
                            else
                            {
                                monthQueryGroup = "	where  Month  >=@StartMonth and Month <= @EndMonth  and level = 0 and Year = @StartYear ";
                            }
                        }

                        cmdText =
                        "	select cast(LabelID as VarChar) + 'MM_" + locationID + "' as LabelID, " +
                        "   sum(Value) as Value from EthEhmis_HmisValue  " +
                        //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                        monthQueryGroup +
                           dataEleClassQuery + idQuery +
                        "	group by DataEleClass,  LabelID  ";
                        addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                        if (singleFacility == true)
                        {
                            if (HMISMainPage.UseNewVersion == true)
                            {
                                monthQueryGroup = "	where  Month  = @StartMonth  and Year = @startYear ";

                            }
                            else
                            {
                                monthQueryGroup = "	where  Month  = @StartMonth  and Year = @startYear and level = 0 ";
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
                                int month = i;

                                int yearToUse = 0;

                                if ((i == 11) || (i == 12))
                                {
                                    yearToUse = _startYear - 1;
                                }
                                else
                                {
                                    yearToUse = _startYear;
                                }

                                cmdText =
                                "	select cast(LabelID as VarChar) + 'MM_" + ethMonth[i] + "_" + locationID + "' as LabelID, " +
                                "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                // Bug should only use the single month
                                //"	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                                //"	where  Month  = @StartMonth  and Year = @startYear " +
                                monthQueryGroup +
                                   dataEleClassQuery + idQuery +
                                "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, month, _endMonth, yearToUse);
                            }
                        }
                    }
                    else if ((_repPeriod == 1) && (_quarterStart == 1)) // Quarterly for Quarter 1 Only
                    {
                        cmdText =
                        "	select cast(LabelID as VarChar) + 'MM_" + locationID + "' as LabelID, " +
                        "   sum(Value) as Value from EthEhmis_HmisValue  " +
                        "   where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                        "   (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
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

                                if (i == 1) // First Quarter
                                {
                                    cmdText =
                                     "	select cast(LabelID as VarChar) + 'MM_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                     "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                     "   where   (((Month  = @StartMonth or Month = @StartMonth + 1) and " +
                                     "   (Year = @startYear)) or (Month <= @EndMonth and Year = @endYear)) " +
                                        dataEleClassQuery + idQuery +
                                     "	group by DataEleClass,  LabelID  ";

                                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);
                                    _startYear = _startYear + 1; // to re-set the Start Year for Quarter 1
                                }
                                else
                                {
                                    cmdText =
                                     "	select cast(LabelID as VarChar) + 'MM_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                     "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                     "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                                        dataEleClassQuery + idQuery +
                                     "	group by DataEleClass,  LabelID  ";

                                    addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                                }
                            }
                            setStartingMonth(_quarterStart, _quarterEnd);
                        }
                    }
                    else if (_repPeriod == 1) // Quarterly except quarter 1
                    {
                        cmdText =
                        "	select cast(LabelID as VarChar) + 'MM_" + locationID + "' as LabelID, " +
                        "   sum(Value) as Value from EthEhmis_HmisValue  " +
                        "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                           dataEleClassQuery + idQuery +
                        "	group by DataEleClass,  LabelID  ";
                        addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                        if (singleFacility == true)
                        {

                            for (int i = _quarterStart; i <= _quarterEnd; i++)
                            {
                                setStartingMonth(i, i);
                                string theQuarter = "quarter" + i;
                                cmdText =
                                 "	select cast(LabelID as VarChar) + 'MM_" + theQuarter + "_" + locationID + "' as LabelID, " +
                                 "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                 "	where  Month  >=@StartMonth and Month <= @EndMonth  and Year = @startYear " +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, _startYear);

                            }
                            setStartingMonth(_quarterStart, _quarterEnd);
                        }
                    }
                    else if (_repPeriod == 2) // Yearly Report
                    {
                        cmdText =
                         "	select cast(LabelID as VarChar) + 'MM_" + locationID + "' as LabelID, " +
                         "   sum(Value) as Value from EthEhmis_HmisValue  " +
                         "	where  Year >= @startYear and Year <= @endYear  " +
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
                                 "	select cast(LabelID as VarChar) + 'MM_" + theYear + "_" + locationID + "' as LabelID, " +
                                 "   sum(Value) as Value from EthEhmis_HmisValue  " +
                                 "	where  Year = @startYear " +
                                    dataEleClassQuery + idQuery +
                                 "	group by DataEleClass,  LabelID  ";
                                addToHashTable(cmdText, id, _startMonth, _endMonth, i);
                            }
                        }
                    }

                    //toExecute = new SqlCommand(cmdText);

                    //toExecute.Parameters.AddWithValue("newIdentification", id);
                    //toExecute.Parameters.AddWithValue("StartMonth", _startMonth);
                    //toExecute.Parameters.AddWithValue("EndMonth", _endMonth);
                    //toExecute.Parameters.AddWithValue("startYear", _startYear);
                    //toExecute.Parameters.AddWithValue("endYear", _endYear);

                    //dt = _helper.GetDataSet(toExecute).Tables[0];

                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    string LabelID = row["labelID"].ToString();
                    //    string value = row["value"].ToString();
                    //    aggregateDataHash.Add(LabelID, value);
                    //}

                    if (HMISMainPage.UseNewVersion == true)
                    {
                        dataEleClassQuery = " and DataEleClass = 11 ";
                    }
                    else
                    {
                        dataEleClassQuery = " and DataEleClass = 2 ";
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
            if (!level1Cache)
            {

                DynamicQueryConstruct();



                foreach (string locationID in locationsToView)
                {
                    string id = locationID;

                    // if (id == "10") id = "14";

                    // Get the Facility Name for the LocationID
                    string facilityName = getFacilityName(id);
                    locationIdToName.Add(locationID, facilityName);
                }


                if (_repKind == 1)
                {
                    string cmdText = "SELECT * from  " + viewLabeIdTableName;
                    SqlCommand toExecute;
                    toExecute = new SqlCommand(cmdText);

                    toExecute.CommandTimeout = 0; //300 // = 1000000;
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
                        format = row["format"].ToString() as string;

                        // Call the insert statement

                        InsertAggregateData(sno, disease, m04, m514, m15, f04, f514, f15, format);
                    }

                    Dictionary<string, string> colMap = new Dictionary<string, string>();
                    colMap.Add("m04", languageHash["morbiditymaleslessthanfour"].ToString());
                    colMap.Add("m514", languageHash["morbiditymalesfivetofourteen"].ToString());
                    colMap.Add("m15", languageHash["morbiditymalesgreaterorequaltofifteen"].ToString());
                    colMap.Add("f04", languageHash["morbidityfemaleslessthanfour"].ToString());
                    colMap.Add("f514", languageHash["morbidityfemalesfivetofourteen"].ToString());
                    colMap.Add("f15", languageHash["morbidityfemalesgreaterorequaltofifteen"].ToString());

                    StringBuilder expr = new StringBuilder();
                    string[] fields = { "m04", "m514", "m15", "f04", "f514", "f15" };
                    for (int i = 0; i < fields.Length; i++)
                    {
                        string nextField = fields[i];
                        string fname = "[" + colMap[nextField] + "]";
                        if (expr.Length > 0)
                            expr.Append("+");
                        expr.Append("IIF(LEN(" + fname + ")>0, Convert(" + fname + ", 'System.Decimal'), 0)");
                    }

                    DataColumn total = new DataColumn(languageHash["total"].ToString(), typeof(decimal), expr.ToString());
                    reportOpdDataTable.Columns.Add(total);
                    //DataColumn Chart = new DataColumn("Chart", typeof(string));
                    //reportOpdDataTable.Columns.Add(Chart);
                    calculateFacilityTototal(reportOpdDataTable, expr);
                    for (int i = 0; i < reportOpdDataTable.Rows.Count; i++)
                    {
                        if (reportOpdDataTable.Rows[i][1].ToString().Contains("Total:"))
                        {
                            reportOpdDataTable.Rows[i].Delete();
                        }
                        //if (reportOpdDataTable.Rows[i][0].ToString() != "" && reportOpdDataTable.Rows[i][reportOpdDataTable.Columns["Format"].ToString()].ToString() != "1")
                        //{
                        //    reportOpdDataTable.Rows[i][10] = "Click Here To See in Chart";
                        //}
                    }

                    _helper.CloseConnection();

                    return reportOpdDataTable;
                }
                else if (_repKind == 2)
                {
                    string cmdText = "SELECT * from  " + viewLabeIdTableName;
                    SqlCommand toExecute;
                    toExecute = new SqlCommand(cmdText);

                    toExecute.CommandTimeout = 0; //300 // = 1000000;
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
                        format = row["format"].ToString() as string;

                        // Call the insert statement

                        InsertAggregateData(sno, disease, m04, m514, m15, f04, f514, f15, mm04, mm514, mm15, mf04, mf514, mf15, format);
                    }

                    Dictionary<string, string> colMap = new Dictionary<string, string>();
                    colMap.Add("m04", languageHash["morbiditymaleslessthanfour"].ToString());
                    colMap.Add("m514", languageHash["morbiditymalesfivetofourteen"].ToString());
                    colMap.Add("m15", languageHash["morbiditymalesgreaterorequaltofifteen"].ToString());
                    colMap.Add("f04", languageHash["morbidityfemaleslessthanfour"].ToString());
                    colMap.Add("f514", languageHash["morbidityfemalesfivetofourteen"].ToString());
                    colMap.Add("f15", languageHash["morbidityfemalesgreaterorequaltofifteen"].ToString());
                    colMap.Add("mm04", languageHash["mortalitymaleslessthanfour"].ToString());
                    colMap.Add("mm514", languageHash["mortalitymalesfivetofourteen"].ToString());
                    colMap.Add("mm15", languageHash["mortalitymalesgreaterorequaltofifteen"].ToString());
                    colMap.Add("mf04", languageHash["mortalityfemaleslessthanfour"].ToString());
                    colMap.Add("mf514", languageHash["mortalityfemalesfivetofourteen"].ToString());
                    colMap.Add("mf15", languageHash["mortalityfemalesgreaterorequaltofifteen"].ToString());

                    StringBuilder expr = new StringBuilder();
                    string[] fields = { "m04", "m514", "m15", "f04", "f514", "f15", "mm04", "mm514", "mm15", "mf04", "mf514", "mf15" };
                    for (int i = 0; i < fields.Length; i++)
                    {
                        string nextField = fields[i];
                        string fname = "[" + colMap[nextField] + "]";
                        if (expr.Length > 0)
                            expr.Append("+");
                        expr.Append("IIF(LEN(" + fname + ")>0, Convert(" + fname + ", 'System.Decimal'), 0)");
                    }

                    DataColumn total = new DataColumn("Total", typeof(decimal), expr.ToString());
                    reportIpdDataTable.Columns.Add(total);
                    //DataColumn Chart = new DataColumn("Chart", typeof(string));
                    //reportIpdDataTable.Columns.Add(Chart);
                    calculateFacilityTototal(reportIpdDataTable, expr);
                    for (int i = 0; i < reportIpdDataTable.Rows.Count; i++)
                    {
                        if (reportIpdDataTable.Rows[i][1].ToString().Contains("Total:"))
                        {
                            reportIpdDataTable.Rows[i].Delete();
                        }
                        //if (reportIpdDataTable.Rows[i][0].ToString() != "" && reportIpdDataTable.Rows[i][reportIpdDataTable.Columns["Format"].ToString()].ToString() != "1")
                        //{
                        //    reportIpdDataTable.Rows[i][16] = "Click Here To See in Chart";
                        //}
                    }

                    _helper.CloseConnection();
                    return reportIpdDataTable;
                }

                return null;
            }
            else
            {
                if (_repKind == 1) // OPD Disease
                {
                    return reportOpdDataTable;
                }
                else if (_repKind == 2)
                {
                    return reportIpdDataTable;
                }

            }
            return null;
        }
     
        private void calculateFacilityTototal(DataTable dt, StringBuilder sb)
        {
            int noFacilitiesSelected = locationsToView.Count;

            if (_repKind == 1) // OPD # columns for OPD and IPD is different since IPD has mortality
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if ((dt.Rows[i][languageHash["icd10"].ToString()].ToString().Trim() != "") && (dt.Rows[i]["format"].ToString() != "1"))
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
                    if ((dt.Rows[i][languageHash["icd10"].ToString()].ToString().Trim() != "") && (dt.Rows[i]["format"].ToString() != "1"))
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
                        disease = languageHash["total"].ToString() + ": "; //"Total: ";
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
                            //disease = ethMonth[i].ToString();
                            string theMonth = ethMonth[i].ToString().ToLower();
                            disease = languageHash[theMonth].ToString();
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
                            // disease = "Quarter  " + i + ":";
                            disease = languageHash["quarter"].ToString() + "  " + i + ":";
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
                            //disease = "Year  " + i + ":";
                            disease = languageHash["year"].ToString() + "  " + i + ":";
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
                        disease = languageHash["total"] + ": ";
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
                            //disease = ethMonth[i].ToString();
                            string theMonth = ethMonth[i].ToString().ToLower();
                            disease = languageHash[theMonth].ToString();
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
                            //disease = "Quarter  " + i + ":";
                            disease = languageHash["quarter"] + "  " + i + ":";
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
                            //disease = "Year  " + i + ":";
                            disease = languageHash["year"] + "  " + i + ":";
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
    }
}
