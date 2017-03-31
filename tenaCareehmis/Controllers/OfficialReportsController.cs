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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eHMIS.HMIS.ReportHelper;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Web.Http.Cors;
using eHMISWebApi.Models;
using Newtonsoft.Json.Linq;
using System.Collections;
using SqlManagement.Database;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class OfficialReportsController : ApiController
    {
        string ipdViewTable = string.Empty;
        string opdViewTable = string.Empty;
        string serviceViewTable = string.Empty;
        string facilityTable = string.Empty;
        string languageSet = LanguageController.languageSet;
        Hashtable languageHash = new Hashtable();

        Dictionary<int, string> ethMonth = new Dictionary<int, string>();        

        enum reportTypeIndex
        {
            Service = 0,
            OPD_Disease = 1,
            IPD_Disease = 2,
            Indicators = 3,
            DiseaseAnalysis = 4,
        }

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

        // GET: api/OfficialReports
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
      
        // GET: api/OfficialReports/5
        [EnableCors("*", "*", "*")]
        public IEnumerable<object> Get(int id, int id1, int id4, int id2, string id3, bool id5, bool id6, int id7, int id8)
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

            setCorrectLanguageTable();

            int month = id1;
            int endMonth = id7;
            int quarter = id4;
            int endQuarter = id8;
            int year = id2;
            string locationID = id3;
            int reporType = id;
            bool isCached = id5;
            bool showQuarterlyreport = id6;
            int FacilityTypeId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityTypeId(id3);
            DataTable dt = new DataTable();
            reportObject fillReport = new reportObject();
            fillReport.Year = year;
            fillReport.RepGlobalType = reportObject.ReportGlobalType.federal;
            fillReport.WoredaID = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getWoredaId(locationID);
            fillReport.RegionID = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(locationID);
            fillReport.ZoneID = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(locationID);
            fillReport.LocationHMISCode = locationID;
            fillReport.SelectedLocationType = FacilityTypeId;
            fillReport.IsCacheEnabled = isCached;
            fillReport.StartMonth = month;
            fillReport.EndMonth = endMonth;
            fillReport.StartQuarter = quarter;
            fillReport.EndQuarter = endQuarter;
           
            eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable = null;
            eHMIS.HMIS.ReportAggregation.ReportPeriod reportPeriod = null;
            string[] subTitle = null;
            string[] title = new string[3];

            /////////////////////////////////////////// IPD MONTHLY REPORT FOR EACH FACILIY LEVEL /////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (id == 1 && FacilityTypeId == 3)
            {
                dt = null;
            }
            if (id == 1 && ( FacilityTypeId == 2 || FacilityTypeId == 1 || FacilityTypeId == 4 || FacilityTypeId == 5 || FacilityTypeId == 7 || FacilityTypeId == 6))// hc ipd
            {
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.IPD_Disease_Facility_Monthly;
                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.IPD_Disease_Facility_Quarterly;
                    fillReport.ReportType = 62;
                }
                // fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterIPDDiseaseView";
                fillReport.ViewLabelIdTableName = ipdViewTable;
                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBIPDDiseaseReport";
                fillReport.StartMonth = month;
                fillReport.EndMonth = endMonth;
                fillReport.StartQuarter = quarter;
                fillReport.EndQuarter = endQuarter;
                fillReport.RepGlobalType = reportObject.ReportGlobalType.facility;
                eHMIS.HMIS.ReportAggregation.NewReporting.NewDiseaseReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewDiseaseReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();
                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
            }
            if (id == 1 && FacilityTypeId == 8)//woreda ipd
            {                
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }

                //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterIPDDiseaseView";
                fillReport.ViewLabelIdTableName = ipdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBIPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.IPD_Disease;
                fillReport.RepGlobalType = reportObject.ReportGlobalType.woreda;
                fillReport.SelectedLocationLevel = 1;
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                       fillReport.SelectedLocationLevel, fillReport.SelectedLocationLevel, fillReport.RegionID , reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
            }
            if (id == 1 && FacilityTypeId == 9)//zone ipd
            {               
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.ReportType = 22;
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterIPDDiseaseView";
                fillReport.ViewLabelIdTableName = ipdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBIPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.IPD_Disease;
                fillReport.RepGlobalType = reportObject.ReportGlobalType.zone;
                fillReport.SelectedLocationLevel = 2;
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                       fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID, reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
            }
            if (id == 1 && FacilityTypeId == 10)//region ipd
            {               
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                // fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterIPDDiseaseView";
                fillReport.ViewLabelIdTableName = ipdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBIPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.IPD_Disease;
                fillReport.RepGlobalType = reportObject.ReportGlobalType.region;
                fillReport.SelectedLocationLevel = 3;
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                       fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID, reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
            }
            if (id == 1 && FacilityTypeId == 11)//federal ipd
            {               
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                // fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterIPDDiseaseView";
                fillReport.ViewLabelIdTableName = ipdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBIPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.IPD_Disease;
                fillReport.RepGlobalType = reportObject.ReportGlobalType.federal;
                fillReport.SelectedLocationLevel = 4;
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                       fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID, reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
            }
            /////////////////////////////////////////// OPD QUARTERLY and MONTHLY REPORT FOR EACH FACILIY LEVEL /////////////////////////////////////////////////////////////////////////////////////////////////////////
            else if (id == 2 && FacilityTypeId == 3)//Health Post
            {
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.OPD_Disease_Facility_Monthly;
                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.OPD_Disease_Facility_Quarterly;
                    fillReport.ReportType = 62;
                }
                else if (month == 0 && quarter == 0)
                {
                    //annual
                }
                if (month != 0 || quarter != 0)
                {
                    //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterHPDiseaseView";
                    fillReport.ViewLabelIdTableName = opdViewTable;

                    fillReport.ReportTableName = "#EthioHIMS_QuarterRHBOOPDDiseaseReport";

                    fillReport.RepGlobalType = reportObject.ReportGlobalType.facility;
                    fillReport.SelectedLocationLevel = 0;
                    eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                        fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID
                        , null);
                    reportGen.newGenerateReport(fillReport);
                    dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
                }

            }
            else if (id == 2 && (FacilityTypeId == 2 || FacilityTypeId == 1 || FacilityTypeId == 4 || FacilityTypeId == 5 || FacilityTypeId == 7 || FacilityTypeId == 6))//Health Center
            {
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.OPD_Disease_Facility_Monthly;
                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.OPD_Disease_Facility_Quarterly;
                    fillReport.ReportType = 2;
                }
                else if (month == 0 && quarter == 0)
                {
                    //Annual
                }
                if (month != 0 || quarter != 0)
                {
                    //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
                    fillReport.ViewLabelIdTableName = opdViewTable;

                    fillReport.ReportTableName = "#EthioHIMS_QuarterRHBOOPDDiseaseReport";
                   
                    fillReport.RepGlobalType = reportObject.ReportGlobalType.facility;
                    fillReport.SelectedLocationLevel = 0;

                    eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                        fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID
                        , null);
                    reportGen.newGenerateReport(fillReport);
                    dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
                }
            }
            else if (id == 2 && FacilityTypeId == 8)//Woreda 
            {                
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.ReportType = 22;
                }
                else if (month == 0 && quarter == 0)
                {

                }
                if (month != 0 || quarter != 0)
                {
                    //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
                    fillReport.ViewLabelIdTableName = opdViewTable;

                    fillReport.ReportTableName = "#EthioHIMS_QuarterRHBOOPDDiseaseReport";
                    fillReport.RepKind = reportObject.ReportKind.OPD_Disease;
                    fillReport.RepGlobalType = reportObject.ReportGlobalType.woreda;
                    fillReport.SelectedLocationLevel = 1;

                    eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                          fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID
                          , reportPeriod);
                    fillReport.SubmittingInstId = reportGen.IncludedList;
                    reportGen.newGenerateReport(fillReport);
                    dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
                }

            }
            else if (id == 2 && FacilityTypeId == 9)//Zone 
            {              
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.ReportType = 32;
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
                fillReport.ViewLabelIdTableName = opdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBOOPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.OPD_Disease;

                fillReport.RepGlobalType = reportObject.ReportGlobalType.zone;
                fillReport.SelectedLocationLevel = 2;
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                      fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID
                      , reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;

            }
            else if (id == 2 && FacilityTypeId == 10)//Region 
            {               
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
                fillReport.ViewLabelIdTableName = opdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBOOPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.OPD_Disease;

                fillReport.RepGlobalType = reportObject.ReportGlobalType.region;
                fillReport.SelectedLocationLevel = 3;
                reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, 0, fillReport.StartQuarter, fillReport.Year);
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                                          fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID, reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;

            }
            else if (id == 2 && FacilityTypeId == 11)//Federal
            {                
                if (month != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                else if (quarter != 0)
                {
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.ReportType = 52;
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterOPDDiseaseView4";
                fillReport.ViewLabelIdTableName = opdViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterRHBOOPDDiseaseReport";
                fillReport.RepKind = reportObject.ReportKind.OPD_Disease;

                fillReport.RepGlobalType = reportObject.ReportGlobalType.federal;
                fillReport.SelectedLocationLevel = 4;
                eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR reportGen = new eHMIS.HMIS.ReportAggregation.HMISReportGeneratorQRODR(fillReport.LocationHMISCode,
                      fillReport.SelectedLocationType, fillReport.SelectedLocationLevel, fillReport.RegionID
                      , reportPeriod);
                fillReport.SubmittingInstId = reportGen.IncludedList;
                reportGen.newGenerateReport(fillReport);
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;

            }
            //////////////////////////////////////MONTHLY, QUARTERLY, ANNUAL SERVICE REPORT////////////////////////////////////////////////////////////////////

            else if (id == 3 && FacilityTypeId == 3) // health post
            {
                eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Monthly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Quarterly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.IsShowOnlyQuartDataElement = showQuarterlyreport;

                }
                else if (quarter == 0 && month == 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Annual_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.YEARLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }

                // fillReport.ViewLabelIdTableName = "EthioHIMS_ServiceDataElementsNew";
                fillReport.ViewLabelIdTableName = serviceViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterSrvReport";

                fillReport.RepGlobalType = reportObject.ReportGlobalType.HealthPost;
                fillReport.SelectedLocationLevel = 0;
                eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();
               
                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalServiceStoreProc;
                if (month != 0)
                {
                    dt.Columns.Remove("HP");
                    dt.Columns.Remove("HC");
                    dt.Columns.Remove("Hospital");
                    subTitle = new string[3];
                    subTitle[0] = languageHash["sno"].ToString();
                    subTitle[1] = languageHash["activity"].ToString();
                    subTitle[2] = getMonthNameInAmharic(fillReport.StartMonth, fillReport.EndMonth);
                }
                else if (quarter != 0)
                {

                    string[] quarterMonths = GetQuarterMonths(quarter);
                    dt.Columns.Remove("HP");
                    dt.Columns.Remove("HC");
                    dt.Columns.Remove("Hospital");
                    dt.Columns.Remove("PeriodType");
                    if (fillReport.StartQuarter == fillReport.EndQuarter)
                    {
                        subTitle = new string[6];
                        subTitle[0] = languageHash["sno"].ToString();
                        subTitle[1] = languageHash["activity"].ToString();
                        subTitle[2] = languageHash["quarter"].ToString();
                        subTitle[3] = quarterMonths[0];
                        subTitle[4] = quarterMonths[1];
                        subTitle[5] = quarterMonths[2];
                    }
                    else
                    {
                        subTitle = new string[3];
                        subTitle[0] = languageHash["sno"].ToString();
                        subTitle[1] = languageHash["activity"].ToString();
                        subTitle[2] = languageHash["quarter"].ToString() +
                            "  " + fillReport.StartQuarter + "- " + fillReport.EndQuarter;
                        dt.Columns.Remove("Month1");
                        dt.Columns.Remove("Month2");
                        dt.Columns.Remove("Month3");
                    }
                }
                else
                {
                    dt.Columns.Remove("HP");
                    dt.Columns.Remove("HC");
                    dt.Columns.Remove("Hospital");
                    subTitle = new string[3];
                    subTitle[0] = languageHash["sno"].ToString();
                    subTitle[1] = languageHash["activity"].ToString();
                    subTitle[2] = languageHash["amount"].ToString();

                }
            }
            else if (id == 3 && (FacilityTypeId == 2 || FacilityTypeId == 1 || FacilityTypeId == 4 || FacilityTypeId == 5 || FacilityTypeId == 7 || FacilityTypeId == 6))// health center
            {
                eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Monthly_Service;                    
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Quarterly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter == 0 && month == 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Annual_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.YEARLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.IsShowOnlyQuartDataElement = showQuarterlyreport;
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_ServiceDataElementsNew";
                fillReport.ViewLabelIdTableName = serviceViewTable;

                fillReport.ReportTableName = "#EthioHIMS_QuarterSrvReport";

                fillReport.RepGlobalType = reportObject.ReportGlobalType.facility;
                fillReport.SelectedLocationLevel = 0;
                eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();
           

                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalServiceStoreProc;
                if (month != 0)
                {
                    dt.Columns.Remove("HP");
                    dt.Columns.Remove("HC");
                    dt.Columns.Remove("Hospital");
                    subTitle = new string[3];
                    subTitle[0] = languageHash["sno"].ToString();
                    subTitle[1] = languageHash["activity"].ToString();
                    subTitle[2] = getMonthNameInAmharic(fillReport.StartMonth, fillReport.EndMonth);
                }
                else if (quarter != 0)
                {

                    string[] quarterMonths = GetQuarterMonths(quarter);
                    dt.Columns.Remove("HP");
                    dt.Columns.Remove("HC");
                    dt.Columns.Remove("Hospital");
                    dt.Columns.Remove("PeriodType");
                    if (fillReport.StartQuarter == fillReport.EndQuarter)
                    {
                        subTitle = new string[6];
                        subTitle[0] = languageHash["sno"].ToString();
                        subTitle[1] = languageHash["activity"].ToString();
                        subTitle[2] = languageHash["quarter"].ToString();
                        subTitle[3] = quarterMonths[0];
                        subTitle[4] = quarterMonths[1];
                        subTitle[5] = quarterMonths[2];
                    }
                    else
                    {
                        subTitle = new string[3];
                        subTitle[0] = languageHash["sno"].ToString();
                        subTitle[1] = languageHash["activity"].ToString();
                        subTitle[2] = languageHash["quarter"].ToString() +
                            "  " + fillReport.StartQuarter + "- " + fillReport.EndQuarter;
                        dt.Columns.Remove("Month1");
                        dt.Columns.Remove("Month2");
                        dt.Columns.Remove("Month3");
                    }
                }
                else {
                    dt.Columns.Remove("HP");
                    dt.Columns.Remove("HC");
                    dt.Columns.Remove("Hospital");
                    subTitle = new string[3];
                    subTitle[0] = languageHash["sno"].ToString();
                    subTitle[1] = languageHash["activity"].ToString();
                    subTitle[2] = languageHash["amount"].ToString(); //"Amount";
                   
                }
            }
            else if (id == 3 && FacilityTypeId == 8)//woreda
            {
                eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
                eHMIS.HMIS.HMISMainPage.UseNewVersion = false;
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Monthly_Service;                    
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Quarterly_Service;                    
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.IsShowOnlyQuartDataElement = showQuarterlyreport;
                }
                else if (quarter == 0 && month == 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Annual_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.YEARLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_ServiceDataElementsNew";
                fillReport.ViewLabelIdTableName = serviceViewTable;

                // fillReport.ReportTableName = "#EthioHIMS_QuarterSrvReport";
                fillReport.ReportTableName = "#EthioHIMS_QuarterFMOHSrvReport";
               
                fillReport.RepGlobalType = reportObject.ReportGlobalType.woreda;
                fillReport.SelectedLocationLevel = 1;
                fillReport.ReportType = 624;
                eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();
                

                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalServiceStoreProc;
                dt.Columns.Remove("HP");
                dt.Columns.Remove("HC");
                dt.Columns.Remove("Hospital");
                dt.Columns.Remove("ZHD");
                dt.Columns.Remove("RHB");
                dt.Columns.Remove("WHO");
                dt.Columns.Remove("FMOH");
                dt.Columns.Remove("ZHDTotal");
                dt.Columns.Remove("RHBTotal");
                dt.Columns.Remove("MOHTotal");
                dt.Columns.Remove("Private");
                title = new string[3];
                title[0] = languageHash["allFacilities"].ToString();
                title[1] = languageHash["district"].ToString(); //"District";
                title[2] = languageHash["allHis"].ToString(); //"All HIs";

            }
            else if (id == 3 && FacilityTypeId == 9)//zone
            {
                eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
                eHMIS.HMIS.HMISMainPage.UseNewVersion = false;

                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Monthly_Service;                  
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Quarterly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.IsShowOnlyQuartDataElement = showQuarterlyreport;

                }
                else if (quarter == 0 && month == 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Annual_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.YEARLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_ServiceDataElementsNew";
                fillReport.ViewLabelIdTableName = serviceViewTable;

                // fillReport.ReportTableName = "#EthioHIMS_QuarterSrvReport";
                fillReport.ReportTableName = "#EthioHIMS_QuarterFMOHSrvReport";
               
                fillReport.RepGlobalType = reportObject.ReportGlobalType.zone;
                fillReport.SelectedLocationLevel = 2;
                eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();

                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalServiceStoreProc;
                dt.Columns.Remove("HP");
                dt.Columns.Remove("HC");
                dt.Columns.Remove("Hospital");
                dt.Columns.Remove("ZHD");
                dt.Columns.Remove("RHB");
                dt.Columns.Remove("WHO");
                dt.Columns.Remove("FMOH");
                dt.Columns.Remove("RHBTotal");
                dt.Columns.Remove("MOHTotal");
                dt.Columns.Remove("Private");
                title = new string[4];
                title[0] = languageHash["allFacilities"].ToString();
                title[1] = languageHash["allDistricts"].ToString(); //"All District";
                title[2] = languageHash["zone"].ToString(); //"Zone HD";
                title[3] = languageHash["allHis"].ToString(); //"All HIs";

            }
            else if (id == 3 && FacilityTypeId == 10)//region
            {
                eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
                eHMIS.HMIS.HMISMainPage.UseNewVersion = false;
                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Monthly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Quarterly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.IsShowOnlyQuartDataElement = showQuarterlyreport;

                }
                else if (quarter == 0 && month == 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Annual_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.YEARLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_ServiceDataElementsNew";
                fillReport.ViewLabelIdTableName = serviceViewTable;

                // fillReport.ReportTableName = "#EthioHIMS_QuarterSrvReport";
                fillReport.ReportTableName = "#EthioHIMS_QuarterFMOHSrvReport";
             
                fillReport.RepGlobalType = reportObject.ReportGlobalType.region;
                fillReport.SelectedLocationLevel = 3;
                eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();
                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalServiceStoreProc;
                dt.Columns.Remove("HP");
                dt.Columns.Remove("HC");
                dt.Columns.Remove("Hospital");
                dt.Columns.Remove("ZHD");
                dt.Columns.Remove("RHB");
                dt.Columns.Remove("WHO");
                dt.Columns.Remove("FMOH");
                dt.Columns.Remove("MOHTotal");
                dt.Columns.Remove("Private");
                title = new string[5];
                title[0] = languageHash["allFacilities"].ToString();
                title[1] = languageHash["allDistricts"].ToString();  //"All Districts";
                title[2] = languageHash["allZones"].ToString(); //"All Zones";
                title[3] = languageHash["region"].ToString(); //"Province";
                title[4] = languageHash["allHis"].ToString(); //"All HIs";

            }
            else if (id == 3 && FacilityTypeId == 11)//federal
            {
                eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
                eHMIS.HMIS.HMISMainPage.UseNewVersion = false;

                if (month != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Monthly_Service;                   
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.MONTHLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);

                }
                else if (quarter != 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Quarterly_Service;                  
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.QUARTERLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                    fillReport.IsShowOnlyQuartDataElement = showQuarterlyreport;

                }
                else if (quarter == 0 && month == 0)
                {
                    fillReport.RepKind = reportObject.ReportKind.Annual_Service;               
                    reportPeriod = new eHMIS.HMIS.ReportAggregation.ReportPeriod(eHMIS.HMIS.ReportAggregation.PeriodType.YEARLY, fillReport.StartMonth, fillReport.StartQuarter, fillReport.Year);
                }
                //fillReport.ViewLabelIdTableName = "EthioHIMS_ServiceDataElementsNew";
                fillReport.ViewLabelIdTableName = serviceViewTable;

                // fillReport.ReportTableName = "#EthioHIMS_QuarterSrvReport";
                fillReport.ReportTableName = "#EthioHIMS_QuarterFMOHSrvReport";
                eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewServiceReportAggregation(fillReport);
                fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();
                
                diseaseReport.startReportTableGeneration(true);
                diseaseReport.UpdateHashTable();
                dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalServiceStoreProc;

                if (dt.Columns.Contains("HP"))
                {
                    dt.Columns.Remove("HP");
                }
                if (dt.Columns.Contains("HC"))
                {
                    dt.Columns.Remove("HC");
                }
                if (dt.Columns.Contains("Hospital"))
                {
                    dt.Columns.Remove("Hospital");
                }
                if (dt.Columns.Contains("ZHD"))
                {
                    dt.Columns.Remove("ZHD");
                }
                if (dt.Columns.Contains("RHB"))
                {
                    dt.Columns.Remove("RHB");
                }
                if (dt.Columns.Contains("WHO"))
                {
                    dt.Columns.Remove("WHO");
                }
                if (dt.Columns.Contains("FMOH"))
                {
                    dt.Columns.Remove("FMOH");
                }
                if (dt.Columns.Contains("Private"))
                {
                    dt.Columns.Remove("Private");
                }
                                                                                                            
                title = new string[6];
                title[0] = languageHash["allFacilities"].ToString(); //"All Facilites";
                title[1] = languageHash["allDistricts"].ToString(); //"All District";
                title[2] = languageHash["allZones"].ToString(); //"All Zones";
                title[3] = languageHash["allProvinces"].ToString(); //"All Provinces";
                title[4] = languageHash["national"].ToString(); //"National";
                title[5] = languageHash["allHis"].ToString(); //"All HIs";

            }
            //   eHMIS.HMIS.ReportAggregation.NewReporting.NewDiseaseReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewDiseaseReportAggregation(fillReport);

            List<object> stringAndDataTable = null;
            if (dt != null)
            {
                string[] columnNames = dt.Columns.Cast<DataColumn>()
                                     .Select(x => x.ColumnName)
                                     .ToArray();
                 
                if (id == 3 && FacilityTypeId != 2 && FacilityTypeId !=3 && FacilityTypeId != 1 && FacilityTypeId != 4 && FacilityTypeId != 5 && FacilityTypeId != 6 && FacilityTypeId != 7)
                {
                    subTitle = new string[columnNames.Count()];
                    for (int i = 2; i <= 18; i++)
                    {
                          if (columnNames[i] == "PFHealthCenters")
                            subTitle[i] = languageHash["healthCenters"].ToString();
                           else if (columnNames[i] == "PFHealthPosts")
                            subTitle[i] = languageHash["healthPosts"].ToString();
                           else
                           subTitle[i] = languageHash[columnNames[i].Substring(2).ToLower()].ToString();  //columnNames[i].Substring(2);
                    }
                    for (int j = 19; j <= columnNames.Count() - 1; j++)
                    {
                        if (columnNames[j] == "Readonly")
                            subTitle[j] = columnNames[j];
                       
                        else
                            subTitle[j] = languageHash[columnNames[j].Substring(3).ToLower()].ToString(); //columnNames[j].Substring(3);
                    }
                    subTitle[0] = languageHash[columnNames[0].ToLower()].ToString(); //columnNames[0];
                    subTitle[1] = languageHash[columnNames[1].ToLower()].ToString(); //columnNames[1];
                }
                stringAndDataTable = new List<object>();
                stringAndDataTable.Add(columnNames);
                stringAndDataTable.Add(dt);
                stringAndDataTable.Add(subTitle);
                stringAndDataTable.Add(title);
            }
            return stringAndDataTable;
        }

        // GET: api/OfficialReports/5
        [EnableCors("*", "*", "*")]
        public IEnumerable<object> Get(int id, int id2, int id3)
        {
            reportObject fillReport = new reportObject();

            fillReport.StartMonth = 1;
            fillReport.Year = 2008;
            //fillReport.ViewLabelIdTableName = "EthioHIMS_QuarterIPDDiseaseView";
            fillReport.ViewLabelIdTableName = ipdViewTable;

            fillReport.ReportTableName = "#EthioHIMS_QuarterRHBIPDDiseaseReport";
            fillReport.RepGlobalType = reportObject.ReportGlobalType.facility;
            fillReport.RepKind = reportObject.ReportKind.IPD_Disease_Facility_Monthly;
            fillReport.StartQuarter = 0;
            fillReport.LocationHMISCode = "141090011";
            fillReport.IsCacheEnabled = false;

            eHMIS.HMIS.ReportAggregation.NewReporting.NewDiseaseReportAggregation diseaseReport = new eHMIS.HMIS.ReportAggregation.NewReporting.NewDiseaseReportAggregation(fillReport);

            fillReport.SubmittingInstId = diseaseReport.GetIncludedLocations();

            diseaseReport.startReportTableGeneration(true);
            diseaseReport.UpdateHashTable();

            DataTable dt = eHMIS.HMIS.ReportHelper.GenerateReport.globalReportDataTable;
            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            //System.Collections.ArrayList valueList = new System.Collections.ArrayList();

            //foreach (string column in columnNames)
            //{
            //    string value = dt.Rows[0][column].ToString();
            //    string newItem = column + ',' + value;
            //    valueList.Add(newItem);
            //}
            List<object> stringAndDataTable = new List<object>();
            stringAndDataTable.Add(columnNames);
            stringAndDataTable.Add(dt);

            return stringAndDataTable;
        }

        // POST: api/OfficialReports
        public IEnumerable<object> Post([FromBody]CustomReportParameters value)
        {
            setCorrectLanguageTable();

            JObject obj = (Newtonsoft.Json.Linq.JObject)value.Location;
            List<string> locations = new List<string>(); 

            foreach(var jObj in obj)
            {
                string locationId = jObj.Key;
                locations.Add(locationId);              
            }
            int startMonth = 0, endMonth = 0, quarterStart = 0, quarterEnd = 0, repKind = 0, repPeriod = 0,
                yearStart = 0, yearEnd = 0;

            bool showOnlyQDE = value.showOnlyQDE;
            bool isCacheEnabled = value.isCacheEnabled;
            bool showNumDenom = value.showNumDenom;
            bool showTarget = value.showTarget;           

            yearStart = value.FiscalYear;
            yearEnd = value.FiscalYear;

            eHMIS.HMIS.HMISMainPage.UseNewVersion = false;
            eHMIS.HMIS.HMISMainPage.UseNewServiceDataElement2014 = true;
            eHMIS.HMIS.HMISMainPage.UseNewServiceDataElementValidation = true;

            if (value.Period.ToUpper() == "MONTHLY")
            {
                quarterStart = 0;
                quarterEnd = 0;
                startMonth = value.StartPeriod;
                endMonth = value.EndPeriod;               
                repPeriod = 0;                
                //if ((startMonth == 11) || (startMonth == 12))
                //{
                //    yearStart = value.FiscalYear - 1;
                //}

                //if ((endMonth == 11) || (endMonth == 12))
                //{
                //    yearEnd = value.FiscalYear - 1;
                //}
            }
            else if (value.Period.ToUpper() == "QUARTERLY")
            {
                quarterStart = value.StartPeriod;
                quarterEnd = value.EndPeriod;
                startMonth = 0;
                endMonth = 0;
                repPeriod = 1;
            }
            else if (value.Period.ToUpper() == "YEARLY")
            {
                quarterStart = 0;
                quarterEnd = 0;
                startMonth = 0;
                endMonth = 0;
                repPeriod = 2;
            }

            DataTable reportDataTable = null;

            if (value.ReportType.ToUpper() == "SERVICE")
            {
                eHMIS.HMIS.ReportAggregation.CustomReports.CustomServiceReportAggr generReport =
                new eHMIS.HMIS.ReportAggregation.CustomReports.CustomServiceReportAggr(locations, startMonth,
                   endMonth, yearStart, yearEnd, quarterStart, quarterEnd, repKind, repPeriod,
                   showOnlyQDE, isCacheEnabled);

                reportDataTable = generReport.CreateReport();
                //reportDataTable.Columns.Remove("LabelID");
            }
            else if (value.ReportType.ToUpper() == "OPD DISEASE")
            {               
                eHMIS.HMIS.ReportAggregation.CustomReports.CustomDiseaseReportAggr generReport =
                                       new eHMIS.HMIS.ReportAggregation.CustomReports.CustomDiseaseReportAggr(locations,
                                           startMonth, endMonth,
                                           yearStart, yearEnd,
                                           quarterStart, quarterEnd,
                                           //cmbReportType.SelectedIndex, 
                                           (int)reportTypeIndex.OPD_Disease,
                                           repPeriod, isCacheEnabled);

                reportDataTable = generReport.CreateReport();
            }
            else if (value.ReportType.ToUpper() == "IPD DISEASE")
            {               
                eHMIS.HMIS.ReportAggregation.CustomReports.CustomDiseaseReportAggr generReport =
                       new eHMIS.HMIS.ReportAggregation.CustomReports.CustomDiseaseReportAggr(locations,
                          startMonth, endMonth, yearStart, yearEnd, quarterStart, quarterEnd,
                          (int)reportTypeIndex.IPD_Disease, repPeriod, isCacheEnabled);

                reportDataTable = generReport.CreateReport();
            }
            else if (value.ReportType.ToUpper() == "INDICATORS")
            {                                     
                eHMIS.HMIS.ReportAggregation.CustomReports.CustomIndicatorReportAggr generReport =
                      new eHMIS.HMIS.ReportAggregation.CustomReports.CustomIndicatorReportAggr(locations,
                          startMonth, endMonth, yearStart, yearEnd, quarterStart, quarterEnd,                        
                          //cmbReportType.SelectedIndex, 
                          (int)reportTypeIndex.Indicators,
                          repPeriod, showNumDenom, showOnlyQDE,
                          showTarget, isCacheEnabled);

                reportDataTable = generReport.CreateReport();
                reportDataTable.Columns.Remove("Chart");
            }
           
            string[] columnNames = reportDataTable.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();       


            List<object> stringAndDataTable = new List<object>();
            stringAndDataTable.Add(columnNames);
            stringAndDataTable.Add(reportDataTable);

            return stringAndDataTable;
        }

        // PUT: api/OfficialReports/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/OfficialReports/5
        public void Delete(int id)
        {
        }

        public string getMonthNameInAmharic(int startMonth, int endMonth)
        {
            string monthName = "Unknown";

            if (startMonth == endMonth)
            {
                monthName = languageHash[ethMonth[startMonth].ToLower()].ToString();
            }
            else
            {
                monthName = languageHash[ethMonth[startMonth].ToLower()].ToString() +
                             " - " + languageHash[ethMonth[endMonth].ToLower()].ToString();
            }

            return monthName;
          
        }
        public string[] GetQuarterMonths(int quarter)
        {
            string[] quarterMonths = new string[3];
            switch (quarter)
            {
                case 1:
                    quarterMonths[0] = languageHash["hamle"].ToString(); //"Hamle";
                    quarterMonths[1] = languageHash["nehase"].ToString(); //"Nehase";
                    quarterMonths[2] = languageHash["meskerem"].ToString(); //"Meskerem";
                    break;
                case 2:
                    quarterMonths[0] = languageHash["tikimt"].ToString(); //"Tikimt";
                    quarterMonths[1] = languageHash["hidar"].ToString(); //"Hidar";
                    quarterMonths[2] = languageHash["tahisas"].ToString(); //"Tahisas";
                    break;
                case 3:
                    quarterMonths[0] = languageHash["tir"].ToString(); //"Tir";
                    quarterMonths[1] = languageHash["yekatit"].ToString(); // "Yekatit";
                    quarterMonths[2] = languageHash["megabit"].ToString(); //"Megabit";
                    break;
                case 4:
                    quarterMonths[0] = languageHash["miyazia"].ToString(); //"Miyazia";
                    quarterMonths[1] = languageHash["ginbot"].ToString(); //"Ginbot";
                    quarterMonths[2] = languageHash["sene"].ToString(); //"Sene";
                    break;

            }
            return quarterMonths;
        }
    }
}
