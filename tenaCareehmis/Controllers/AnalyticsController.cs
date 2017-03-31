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
using SqlManagement.Database;
using System.Data.SqlClient;
using System.Data;
using System.Web.Http.Cors;
using System.Collections;
using eHMISWebApi.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using OfficeOpenXml;
using eHMISWebApi.Caching;
using System.Threading;
using System.Web;
using eHMISWebApi.Utils;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AnalyticsController : ApiController
    {
        private ICacheClient _cacheClient;// = new MemcachedClientWrapper();

        private Hashtable indicatorLocationIdValueHash = new Hashtable();
        private string FacilityTable = "EthEhmis_AllFacilityWithID";
        private string ServiceDictionaryTable = "EthioHIMS_ServiceDataElementsNew";
        private string ServiceVerticalSumTable = "EthioHIMS_VerticalSumNew";
        private string populationMultiplier = " 1000 ";
        private string languageSet = LanguageController.languageSet;
        private string DiseaseDictionaryTable = "DiseaseDictionary";

        enum dataElementType
        {
            sumAll = 0,
            sumAllTotal = 1,
            lastMonth = 2,
            lastMonthTotal = 3,
            disease = 4
        }

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

            DiseaseDictionaryTable = LanguageController.diseaseDictionaryTable;
            ServiceDictionaryTable = LanguageController.serviceViewTable;
            FacilityTable = LanguageController.facilityTable;
            languageSet = LanguageController.languageSet;

            eHMISWebApi.Controllers.LanguageController langCtrl = new eHMISWebApi.Controllers.LanguageController();

            DataTable dtLang = langCtrl.Get(languageSet, "analytics");

            foreach (DataRow row in dtLang.Rows)
            {
                string indexName = row["indexName"].ToString();
                string languageName = row["languageName"].ToString();
                languageHash[indexName] = languageName;
            }
        }


        public AnalyticsController(ICacheClient client)
        {
            _cacheClient = client;
            // Get the language settings
            setCorrectLanguageTable();
        }

        // GET: api/Analytics/5
        public DataTable Get(int id)
        {
            DataTable dt = new DataTable();

            if (id == 0)
            {
                DBConnHelper _helper = new DBConnHelper();

                string diseaseListText = " select distinct Disease, 'False' as Checked, '' as groupName from " +
                                         DiseaseDictionaryTable;
                                         //" v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Age " +
                                         //"  where dataEleClass = 8 or dataEleClass = 2 or dataEleClass = 3";

                SqlCommand diseaseListCommand = new SqlCommand(diseaseListText);
                dt = _helper.GetDataSet(diseaseListCommand).Tables[0];
            }
            else if (id == 1)
            {
                // get all the file names stored under the img folder
                string path = @"C:\WebBasedeHMIS\FrontEnd\web-app\img\Dashboards";
                dt.Columns.Add("FileName");
                foreach (string s in Directory.GetFiles(path, "*.png").Select(Path.GetFileName))
                {
                    DataRow newName = dt.NewRow();
                    newName["FileName"] = s;
                    dt.Rows.Add(newName);
                }
            }
            else if (id == 2)
            {
                DBConnHelper _helper = new DBConnHelper();
                string facilityListText = " select * from facilityType where (facilityTypeId >=50 or facilityTypeId <=7)";

                SqlCommand diseaseListCommand = new SqlCommand(facilityListText);
                dt = _helper.GetDataSet(diseaseListCommand).Tables[0];
            }
            else if (id == 3)
            {
                DBConnHelper _helper = new DBConnHelper();

                string dashBoardListsText = " select * from dashboardLists ";

                SqlCommand dashBoardListCommand = new SqlCommand(dashBoardListsText);
                dt = _helper.GetDataSet(dashBoardListCommand).Tables[0];
            }

            return dt;
        }

        public DataTable Get(int id1, int id2, string category, string serviceElementType)
        {   

            DataTable dt = new DataTable();

            // id1 = 1 Service
            // id1 = 2 Disease
            // id1 = 3 Indicators
            // id5 = 5 get the actual data

            // id2 = 1 Category1
            // id2 = 2 Category2
            // id2 = 3 Category3

            DBConnHelper _helper = new DBConnHelper();
            SqlCommand categoryCmd = null;
            string categorySql = "";

            if (id1 == 1)
            {
                if (id2 == 1)
                {
                    categorySql = "select distinct category1 from " + ServiceDictionaryTable + " where category1 != ''";

                    categoryCmd = new SqlCommand(categorySql);
                    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                }
                else if (id2 == 2)
                {
                    categorySql = "  select distinct category2 from " + ServiceDictionaryTable + "  where category2 != '' " +
                                  "  and category1 = '" + category + "'";

                    categoryCmd = new SqlCommand(categorySql);
                    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                }
                else if (id2 == 3)
                {
                    categorySql = "select distinct category3 from " + ServiceDictionaryTable + "  where category3 != '' " +
                                  "  and category3 = '" + category + "'";
                    categoryCmd = new SqlCommand(categorySql);
                    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                }
            }
            else if (id1 == 3)
            {
                if (id2 == 1)
                {
                    categorySql = "select distinct category1 from EthEhmis_IndicatorsNewDisplay where category1 != ''";

                    categoryCmd = new SqlCommand(categorySql);
                    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                }
                //else if (id2 == 2)
                //{
                //    categorySql = "  select distinct category2 from EthEhmis_IndicatorsNewDisplay where category1 != ''" +
                //                  "  and category1 = '" + category + "'";

                //    categoryCmd = new SqlCommand(categorySql);
                //    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                //}               
            }
            else if (id1 == 5)
            {
                if (category == "service")
                {
                    // everything
                    categorySql = "  select labelid, FullDescription from " + ServiceDictionaryTable +
                                  "  where labelId is not null " +
                                  "  order by SequenceNo ";
                }
                else
                {
                    categorySql = "  select labelid, FullDescription from " + ServiceDictionaryTable +
                                    "  where labelId is not null and category1 = '" + category + "'" +
                                    "  order by SequenceNo ";
                    //categorySql = "  select labelid, fullDescription from " + ServiceDictionaryTable + " where category2 != '' " +
                    //                  "  and category1 = '" + category + "'";
                }
                categoryCmd = new SqlCommand(categorySql);
                dt = _helper.GetDataSet(categoryCmd).Tables[0];
            }

            return dt;
        }

        private void ProcessFacilityTypes(AnalyticsParameters value, out string facilityWhereQuery)
        {
            //SelectedFacilityTypes[] obj = value.FacilityTypes;

            int count = 0;
            facilityWhereQuery = string.Empty;

            JObject obj = (Newtonsoft.Json.Linq.JObject)value.FacilityTypes;

            if (obj.Count != 0)
            {
                facilityWhereQuery = " and FacilityTypeName in (";

                foreach (var jObj in obj)
                {
                    string facilityType = jObj.Key;
                    count++;
                    if (obj.Count == count) // last one
                    {
                        facilityWhereQuery += "'" + facilityType + "')";
                    }
                    else
                    {
                        facilityWhereQuery += "'" + facilityType + "',";
                    }
                }
            }
        }

        [EnableCors("*", "*", "*")]
        [Route("api/Analytics/DashBoards/Save")]
        // POST: api/Analytics/Reports
        public void Post([FromBody]DashBoardParameters parameters)
        {
            DataTable reportDataTable = new DataTable();
            List<object> stringAndDataTable = new List<object>();

            string insertDashBoardData = "INSERT INTO[dbo].[DashBoardLists] ([UserId], [DashBoardName], [ChartName], [SelectedXAxis], [SelectedYAxis]" +
            " , [GroupBy], [AggregationType], [ChartType], [ChartData])" +
            " VALUES ('" + parameters.UserId + "', '" + parameters.DashBoardName + "', '" + parameters.ChartName + "', '" + parameters.SelectedXAxis +
            " ', '" + parameters.SelectedYAxis + "', '" + parameters.GroupBy + "', '" + parameters.AggregationType +
            " ', '" + parameters.ChartType + "', '" + parameters.ChartData + "')";

            SqlCommand insertDashBoardCmd = new SqlCommand(insertDashBoardData);
            DBConnHelper _helper = new DBConnHelper();
            _helper.Execute(insertDashBoardCmd);
            // return stringAndDataTable;
        }

        [EnableCors("*", "*", "*")]
        [Route("api/Analytics/Reports")]
        // POST: api/Analytics/Reports
        public List<object> Post([FromBody]AnalyticsParameters parameterValues)
        {

            if (parameterValues.diseaseOptions.per1000Population)
            {
                if (parameterValues.diseaseOptions.populationMultiplier != "")
                {
                    populationMultiplier = parameterValues.diseaseOptions.populationMultiplier;
                }
            }
      

            DataTable reportDataTable = new DataTable();

            string serviceParameterSql = string.Empty;
            reportDataTable = ReportHelper(parameterValues, out serviceParameterSql);
            if (Constants.UseMemcached)
                _cacheClient.AddToCache(parameterValues.GetKey(), reportDataTable);

            //sha1Hash = UtilitiesNew.GeneralUtilities.CryptorEngine.createSHA1Hash()

            string test = parameterValues.ToString();

            List<object> stringAndDataTable = new List<object>();
            string[] columnNames = reportDataTable.Columns.Cast<DataColumn>()
                                   .Select(x => x.ColumnName)
                                   .ToArray();

            string[] realColumnNames = getRealColumnNames(columnNames);

            stringAndDataTable.Add(columnNames);
            stringAndDataTable.Add(reportDataTable);
            // Inlucde the queries in this collection
            stringAndDataTable.Add(serviceParameterSql);
            // Language filtered columns
            stringAndDataTable.Add(realColumnNames);
            return stringAndDataTable;
        }

        string[] getRealColumnNames(string[] columnNames)
        {           
            string columns = string.Empty;
            columns = "(";
            for(int i = 0; i < columnNames.Length; i++)
            {
                if (i == columnNames.Length-1)
                {
                    columns += "'" + columnNames[i] + "'";
                }
                else
                {
                    columns += "'" + columnNames[i] + "',";
                }
            }
            columns += ")";
            string txtRealColumns = "select * from languageColumnSetting where [language] = '" + languageSet + "'" +
                " and columnName in " + columns;
            DBConnHelper _helper = new DBConnHelper();
            SqlCommand cmdRealColumn = new SqlCommand(txtRealColumns);
            DataTable dtRealColumn = _helper.GetDataSet(cmdRealColumn).Tables[0];

            string[] realColumnNames = new string[dtRealColumn.Rows.Count];
           
            Hashtable colHash = new Hashtable();
            foreach(DataRow row in dtRealColumn.Rows)
            {                
                string realColName = row["RealColumnName"].ToString().ToUpper();
                string colName = row["ColumnName"].ToString().ToUpper();
                colHash[colName] = realColName;               
            }

            int columnCount = 0;
            foreach (string colName in columnNames)
            {

                realColumnNames[columnCount] = colHash[colName.ToUpper()].ToString();
                columnCount++;
            }

            return realColumnNames;
        }

        private DataTable ReportHelper(AnalyticsParameters parameterValues, out string serviceParameterSql)
        {
            DataTable reportDataTable = new DataTable();
            string facilityWhereQuery = string.Empty;

            serviceParameterSql = string.Empty;

            ProcessFacilityTypes(parameterValues, out facilityWhereQuery);

            if (parameterValues.reportType == "disease")
            {
                return DiseaseProcessing(parameterValues, facilityWhereQuery);
            }

            if (parameterValues.reportType == "indicator")
            {
                return indicatorCalc(parameterValues);
            }

            if (parameterValues.reportType == "service")
            {
                return ServiceProcessing(parameterValues, facilityWhereQuery, out serviceParameterSql);
            }

            return new DataTable();
        }

        private DataTable ServiceProcessing(AnalyticsParameters parameterValues, string facilityWhereQuery,
            out string serviceParameterSql)
        {
            serviceParameterSql = string.Empty;
            DBConnHelper connHelper = new DBConnHelper();

            if (parameterValues.dataElements != null)
            {
                SelectedDataElements[] obj = parameterValues.dataElements;

                string fullDescriptionsAll = "";
                string fullDescriptionsAllPivot = "";
                string labelIdsSumAll = "";
                string labelIdsSumLastMonth = "";
                string labelIdsSumAllTotal = "";
                string labelIdsSumLastMonthTotal = "";
                ArrayList lastMonthlabelIdList = new ArrayList();
                ArrayList totalLabelIdList = new ArrayList();
                Hashtable totalLabelIdHash = new Hashtable();
                ArrayList totalLabelIdListLastMonth = new ArrayList();
                Hashtable totalLabelIdHashLastMonth = new Hashtable();
                int count = 0;
                bool totalSumAll = false;
                bool lastMonthCalc = false;
                bool totalCalcLabelIdLastMonth = false;
                bool totalCalcLabelId = false;
                bool yearlyRange = false;
                string totalCalcText;

                if ((parameterValues.PeriodRange == "PeriodRangeYearly") || (parameterValues.PeriodRange == "PeriodRangeQuarterly")
                    || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
                    || (parameterValues.periodSelect.aggregate == true))
                {
                    yearlyRange = true;

                    totalCalcText = " select labelId, VerticalSumID from " + ServiceDictionaryTable +
                                     " where aggregationType = 0 and VerticalSumID is not null and labelId is not null";
                    SqlCommand totalCalcCmd = new SqlCommand(totalCalcText);
                    DataTable dtTotalCalcCmd = connHelper.GetDataSet(totalCalcCmd).Tables[0];
                    foreach (DataRow row in dtTotalCalcCmd.Rows)
                    {
                        int labelId = Convert.ToInt32(row["labelId"].ToString());
                        int verticalSumId = Convert.ToInt16(row["verticalSumId"].ToString());
                        totalLabelIdList.Add(labelId);
                        totalLabelIdHash.Add(labelId, verticalSumId);
                    }

                    string labelIdsLastMonthText =
                                        " select labelId from " + ServiceDictionaryTable +
                                        " where aggregationType = 1 and VerticalSumID is null and labelId is not null";

                    SqlCommand lastMonthCmd = new SqlCommand(labelIdsLastMonthText);
                    DataTable dt = connHelper.GetDataSet(lastMonthCmd).Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        int labelId = Convert.ToInt32(row["labelId"].ToString());
                        lastMonthlabelIdList.Add(labelId);
                    }

                    string labelIdsLastMonthTotalText =
                                       " select labelId, VerticalSumID from " + ServiceDictionaryTable +
                                       " where aggregationType = 1 and VerticalSumID is not null and labelId is not null";

                    SqlCommand totalCalcLastMonthCmd = new SqlCommand(labelIdsLastMonthTotalText);
                    DataTable dtTotalCalcLastMonthCmd = connHelper.GetDataSet(totalCalcLastMonthCmd).Tables[0];

                    foreach (DataRow row in dtTotalCalcLastMonthCmd.Rows)
                    {
                        int labelId = Convert.ToInt32(row["labelId"].ToString());
                        int verticalSumId = Convert.ToInt16(row["verticalSumId"].ToString());
                        totalLabelIdListLastMonth.Add(labelId);
                        totalLabelIdHashLastMonth.Add(labelId, verticalSumId);
                    }
                }
                else
                {
                    totalCalcText = " select labelId, VerticalSumID from " + ServiceDictionaryTable +
                                      " where VerticalSumID is not null and labelId is not null";
                    SqlCommand totalCalcCmd = new SqlCommand(totalCalcText);
                    DataTable dtTotalCalcCmd = connHelper.GetDataSet(totalCalcCmd).Tables[0];
                    foreach (DataRow row in dtTotalCalcCmd.Rows)
                    {
                        int labelId = Convert.ToInt32(row["labelId"].ToString());
                        int verticalSumId = Convert.ToInt16(row["verticalSumId"].ToString());
                        totalLabelIdList.Add(labelId);
                        totalLabelIdHash.Add(labelId, verticalSumId);
                    }
                }

                foreach (SelectedDataElements elements in parameterValues.dataElements)
                {
                    string fullDescription = elements.FullDescription;

                    //if (parameterValues.serviceElementType == "old")
                    //{
                    //    if (fullDescription[fullDescription.Length-1] == '_')
                    //    {
                    //        fullDescription = fullDescription.Remove(fullDescription.Length - 1, 1);
                    //    }
                    //}

                    fullDescriptionsAll += "\ncast([" + fullDescription + "] as bigint) as [" +
                        fullDescription.Trim() + "],";

                    fullDescriptionsAllPivot += "\n[" + fullDescription + "],";

                    if (yearlyRange)
                    {
                        if (lastMonthlabelIdList.Contains(elements.labelId))
                        {
                            lastMonthCalc = true;
                            labelIdsSumLastMonth += elements.labelId + ",";
                        }
                        else if (totalLabelIdList.Contains(elements.labelId))
                        {
                            totalCalcLabelId = true;
                            // First get the Id
                            int id = Convert.ToInt32(totalLabelIdHash[elements.labelId]);
                            labelIdsSumAllTotal += id + ",";
                        }
                        else if (totalLabelIdListLastMonth.Contains(elements.labelId))
                        {
                            totalCalcLabelIdLastMonth = true;
                            // First get the Id
                            int id = Convert.ToInt32(totalLabelIdHashLastMonth[elements.labelId]);
                            labelIdsSumLastMonthTotal += id + ",";
                        }
                        else
                        {
                            labelIdsSumAll += elements.labelId + ",";
                            totalSumAll = true;
                        }
                    }
                    else
                    {
                        if (totalLabelIdList.Contains(elements.labelId))
                        {
                            totalCalcLabelId = true;
                            // First get the Id
                            int id = Convert.ToInt16(totalLabelIdHash[elements.labelId]);
                            labelIdsSumAllTotal += id + ",";
                        }
                        else
                        {
                            labelIdsSumAll += elements.labelId + ",";
                            totalSumAll = true;
                        }
                    }
                }

                if (totalSumAll)
                {
                    labelIdsSumAll = labelIdsSumAll.Remove(labelIdsSumAll.Length - 1, 1);
                }

                if (totalCalcLabelId)
                {
                    labelIdsSumAllTotal = labelIdsSumAllTotal.Remove(labelIdsSumAllTotal.Length - 1, 1);
                }

                if (totalCalcLabelIdLastMonth)
                {
                    labelIdsSumLastMonthTotal = labelIdsSumLastMonthTotal.Remove(labelIdsSumLastMonthTotal.Length - 1, 1);
                }

                if (lastMonthCalc)
                {
                    labelIdsSumLastMonth = labelIdsSumLastMonth.Remove(labelIdsSumLastMonth.Length - 1, 1);
                }

                if (fullDescriptionsAll != "")
                {
                    fullDescriptionsAll = fullDescriptionsAll.Remove(fullDescriptionsAll.Length - 1, 1);
                }

                if (fullDescriptionsAllPivot != "")
                {
                    fullDescriptionsAllPivot = fullDescriptionsAllPivot.Remove(fullDescriptionsAllPivot.Length - 1, 1);
                }

                serviceParameterSql = getServiceQuery(parameterValues, fullDescriptionsAll,
                    fullDescriptionsAllPivot, labelIdsSumAll, labelIdsSumLastMonth, labelIdsSumAllTotal, labelIdsSumLastMonthTotal,
                    lastMonthCalc, totalCalcLabelId, totalCalcLabelIdLastMonth, totalSumAll, facilityWhereQuery);

                DBConnHelper _helper = new DBConnHelper();
                SqlCommand parameterCmd = new SqlCommand(serviceParameterSql);
                return _helper.GetDataSet(parameterCmd).Tables[0];
            }
            else return new DataTable();
        }

        [HttpPost]
        [Route("api/Analytics/Export")]
        public HttpResponseMessage Export(AnalyticsParameters parameters)
        {
            var fileExtension = "xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var filename = $"export_{parameters.reportType}_{DateTime.Now.ToShortDateString()}.{fileExtension}";

            var serviceParam = string.Empty;
            DataTable data = null;
            if (Constants.UseMemcached)
                data = _cacheClient.GetFromCache<DataTable>(parameters.GetKey(), () => ReportHelper(parameters, out serviceParam));

            if (data == null)
                data = ReportHelper(parameters, out serviceParam);

            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Export");
                ws.Cells["A1"].LoadFromDataTable(data, true);
                var ms = new MemoryStream();
                pck.SaveAs(ms);
                ms.Position = 0;

                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                response.Content.Headers.Add("X-Filename", filename);

                return response;
            }
        }

        private string getTotalAggregateQuery(AnalyticsParameters parameterValues,
            string labelIdsSumAllTotal, bool totalSumAll, bool totalCalcLabelIdLastMonth, bool lastMonthCalc)
        {
            string aggregateQuery = string.Empty;

            string aggregationOptions = string.Empty;
            string aggregationGroupBy = string.Empty;
            string institutionWhere = string.Empty;
            string aggregationIdsPivot = string.Empty;
            string aggregationIdsRows = string.Empty;
            string institutionType = string.Empty;
            string lastMonthAggrIdComma = string.Empty;
            string lastMonthAggrId = string.Empty;

            string vaggregationOptions = string.Empty;
            string vaggregationOptionsAs = string.Empty;
            string vaggregationIdsRows = string.Empty;
            string lastMonthInnerJoins = string.Empty;
            string vaggregationIdsPivot = string.Empty;
            string taggregationGroupBy = string.Empty;
            string facilityWhereQuery = string.Empty;

            ProcessFacilityTypes(parameterValues, out facilityWhereQuery);

            processAggregationType(parameterValues, out aggregationOptions, out aggregationGroupBy, out aggregationIdsPivot,
                out aggregationIdsRows, out institutionWhere, out institutionType,
                out lastMonthAggrIdComma, out vaggregationOptions, out vaggregationOptionsAs,
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy,
                dataElementType.sumAllTotal);

            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;
            bool lastMonth = false;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection, lastMonth);

            string frontListingAggrOption = string.Empty;
            string frontListingIds = string.Empty;
            string orderByListing = string.Empty;
            string groupByListing = string.Empty;

            if (parameterValues.dataElementsPivot == "pivotdataelements")
            {
                frontListingAggrOption = aggregationGroupBy + aggregationIdsPivot + " FullDescription, \n ";

                //  orderByListing =
                //     "       order by aggregationGroupBy + aggregationIdsPivot " +
                //periodGroupByListing + "\n";
            }
            else if (parameterValues.dataElementsPivot == "pivotinstitutions")
            {
                frontListingAggrOption = aggregationIdsPivot + "sequenceNo, " + " FullDescription, \n ";
                aggregationIdsRows = "";

                //  orderByListing =
                //     "       order by SequenceNo, " +
                //aggregationGroupBy + aggregationIdsPivot +
                //periodGroupByListing + "\n";
            }
            else if (parameterValues.dataElementsPivot == "row")
            {
                frontListingAggrOption = aggregationOptions + aggregationIdsPivot + "sequenceNo, " + " FullDescription, \n ";

                orderByListing =
                    "       order by SequenceNo, " +
               aggregationGroupBy + aggregationIdsPivot +
               periodGroupByListing + "\n";
            }

            if (totalCalcLabelIdLastMonth)
            {
                orderByListing = "";
            }

            aggregateQuery =
            " select  " + frontListingAggrOption +
            //" FiscalYear, FiscalMonth, \n " + 
            periodListing +

            //" ReportingRegionName, ZoneName, WoredaName, \n " +                 
            " Value from \n " +
            "       ( \n" +
                "      select " + ServiceVerticalSumTable + ".ID, " +
                //"      FiscalYear, FiscalMonth, \n" +
                periodListing +
                //"      ReportingRegionName, ZoneName, WoredaName, \n" + 
                aggregationGroupBy + aggregationIdsRows +
                "      sum(value) as Value  \n " +
                "      from " + ServiceDictionaryTable + "  \n " +
                "      inner join " + ServiceVerticalSumTable + " on \n " +
                ServiceDictionaryTable + ".LabelID = " + ServiceVerticalSumTable + ".LabelId  \n " +
                "      inner  join v_EthEhmis_HmisValue  \n " +
                "      on v_EthEhmis_HmisValue.LabelID = " + ServiceDictionaryTable + ".LabelID   \n " +
                "      inner  join " + FacilityTable + " on \n " +
                FacilityTable + ".HMISCode = v_EthEhmis_HmisValue.LocationID \n " +
                "      where " + ServiceVerticalSumTable + ".ID in  " + "\n" +
                "      ( " + labelIdsSumAllTotal + " ) " + "\n" +
                "       and (dataClass = 6 or dataClass =7 )" + institutionWhere + facilityWhereQuery + "\n" +
                 // "       FiscalYear = 2008 and FiscalMonth = 12 \n " +
                 periodWhereSelection + "\n" +
                "       group by " + ServiceVerticalSumTable + ".ID, \n " +
                 //"       FiscalMonth, FiscalYear, \n" +
                 //"       ReportingRegionName, ZoneName, WoredaName   \n" +
                 aggregationGroupBy + aggregationIdsRows + "\n" +
                //" FiscalYear " +
                periodGroupByListing + "\n" +
            "       ) as t   \n " +
            "       inner join " + ServiceDictionaryTable + "  \n " +
            "       on " + ServiceDictionaryTable + ".VerticalSumID = t.ID \n " +
                    orderByListing;

            return aggregateQuery;
        }

        private string getLastMonthServiceQueryTotal(AnalyticsParameters parameterValues,
            string labelIdsSumLastMonthTotal, bool totalSumAll, bool totalCalcLabelId, bool lastMonthCalc)
        {

            string aggregationOptions = string.Empty;
            string aggregationGroupBy = string.Empty;
            string institutionWhere = string.Empty;
            string aggregationIdsPivot = string.Empty;
            string aggregationIdsRows = string.Empty;
            string institutionType = string.Empty;
            string lastMonthAggrIdComma = string.Empty;
            string lastMonthAggrId = string.Empty;

            string vaggregationOptions = string.Empty;
            string vaggregationOptionsAs = string.Empty;
            string vaggregationIdsRows = string.Empty;
            string lastMonthInnerJoins = string.Empty;
            string vaggregationIdsPivot = string.Empty;
            string taggregationGroupBy = string.Empty;
            string facilityWhereQuery = string.Empty;

            processAggregationType(parameterValues, out aggregationOptions, out aggregationGroupBy, out aggregationIdsPivot,
                out aggregationIdsRows, out institutionWhere, out institutionType,
                out lastMonthAggrIdComma, out vaggregationOptions, out vaggregationOptionsAs,
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy,
                dataElementType.lastMonthTotal);

            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;
            bool lastMonth = true;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection, lastMonth);

            string frontListingAggrOption = string.Empty;
            string frontListingIds = string.Empty;
            string orderByListing = string.Empty;
            string groupByListing = string.Empty;

            if (parameterValues.dataElementsPivot == "row")
            {
                frontListingAggrOption = vaggregationOptionsAs;
                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    periodListing = "v.FiscalYear, v.Quarter, ";

                    if (parameterValues.periodSelect.aggregate == true)
                    {
                        if (parameterValues.PeriodRange == "PeriodRangeQuarterly")
                        {
                            string quarterRange = "Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter;
                            periodListing = " v.FiscalYear, + " + "'" + quarterRange + "'" + " as Quarter, ";
                        }
                        else if (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
                        {
                            int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                            string quarterRange =
                                " case   \n" +
                                " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                                  endFiscalYearMinusOne + " then " + "'Quarter:" +
                                  parameterValues.periodSelect.StartYearStartQuarter + "-" +
                                  parameterValues.periodSelect.StartYearEndQuarter + "'" + "\n" +
                                " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                " then " + "'Quarter:" +
                                  parameterValues.periodSelect.EndYearStartQuarter + "-" +
                                  parameterValues.periodSelect.EndYearEndQuarter + "'" + " end as Quarter, ";

                            periodListing = " v.FiscalYear, " + quarterRange;
                        }
                    }

                    frontListingIds = vaggregationIdsRows + "  " + ServiceDictionaryTable + ".sequenceNo as sequenceNo, \n" +
                                 " " + ServiceDictionaryTable + ".FullDescription, " + periodListing + "\n";
                }
                else
                {
                    periodListing = " v.FiscalYear, ";
                    if (parameterValues.periodSelect.aggregate == true)
                    {
                        if (parameterValues.PeriodRange == "PeriodRangeMonthly")
                        {
                            string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                            periodListing = " v.FiscalYear, " + "'" + monthRange + "'" + " as FiscalMonth, ";
                        }
                        else if (parameterValues.PeriodRange == "PeriodRangeYearMonthly")
                        {
                            int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                            string monthRange =
                                " case   \n" +
                                " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                                  endFiscalYearMinusOne + " then " + "'Month:" +
                                  parameterValues.periodSelect.StartYearStartMonth + "-" +
                                  parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                                " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                " then " + "'Month:" +
                                  parameterValues.periodSelect.EndYearStartMonth + "-" +
                                  parameterValues.periodSelect.EndYearEndMonth + "'" + " end as FiscalMonth, ";

                            periodListing = " v.FiscalYear, " + monthRange;
                        }
                    }


                    frontListingIds = vaggregationIdsRows + "  " + ServiceDictionaryTable + ".sequenceNo as sequenceNo, \n" +
                                      " " + ServiceDictionaryTable + ".FullDescription, " + periodListing + "\n";

                }

                if ((totalSumAll) || (totalCalcLabelId))
                {
                    orderByListing =
                       "    order by FiscalYear, \n" +
                 //"    v.ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsPivot +
                 "    sequenceNo  \n";
                }
                else
                {
                    if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                    {
                        orderByListing =
                         "    order by v.FiscalYear, v.Quarter, \n" +
                   //"    v.ReportingRegionName, " +
                   vaggregationOptions + vaggregationIdsRows +
                   "    sequenceNo  \n";
                    }
                    else
                    {
                        orderByListing =
                            "    order by v.FiscalYear, \n" +
                            //"    v.ReportingRegionName, " +
                            vaggregationOptions + vaggregationIdsRows +
                            "    sequenceNo  \n";
                    }
                }

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    groupByListing =
                              "    group by " + ServiceDictionaryTable + ".FullDescription, v.FiscalYear, v.Quarter, " +
                             //"    v.ReportingRegionName, \n" +
                             vaggregationOptions + vaggregationIdsRows +
                            "    " + ServiceDictionaryTable + ".sequenceNo \n";
                }
                else
                {
                    groupByListing =
                               "    group by " + ServiceDictionaryTable + ".FullDescription, v.FiscalYear, " +
               //"    v.ReportingRegionName, \n" +
               vaggregationOptions + vaggregationIdsRows +
               "    " + ServiceDictionaryTable + ".sequenceNo \n";
                }
            }
            else if (parameterValues.dataElementsPivot == "pivotinstitutions")
            {
                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    periodListing = "v.FiscalYear, v.Quarter, ";

                    if (parameterValues.periodSelect.aggregate == true)
                    {
                        if (parameterValues.PeriodRange == "PeriodRangeQuarterly")
                        {
                            string quarterRange = "Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter;
                            periodListing = " v.FiscalYear, + " + "'" + quarterRange + "'" + " as Quarter, ";
                        }
                        else if (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
                        {
                            int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                            string quarterRange =
                                " case   \n" +
                                " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                                  endFiscalYearMinusOne + " then " + "'Quarter:" +
                                  parameterValues.periodSelect.StartYearStartQuarter + "-" +
                                  parameterValues.periodSelect.StartYearEndQuarter + "'" + "\n" +
                                " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                " then " + "'Quarter:" +
                                  parameterValues.periodSelect.EndYearStartQuarter + "-" +
                                  parameterValues.periodSelect.EndYearEndQuarter + "'" + " end as Quarter, ";

                            periodListing = " v.FiscalYear, " + quarterRange;
                        }
                    }

                    frontListingAggrOption = vaggregationIdsPivot + " sequenceNo,  " + ServiceDictionaryTable + ".FullDescription, " + periodListing + "\n";
                }
                else
                {
                    periodListing = " v.FiscalYear, ";
                    if (parameterValues.periodSelect.aggregate == true)
                    {
                        if (parameterValues.PeriodRange == "PeriodRangeMonthly")
                        {
                            string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                            periodListing = " v.FiscalYear, " + "'" + monthRange + "'" + " as FiscalMonth, ";
                        }
                        else if (parameterValues.PeriodRange == "PeriodRangeYearMonthly")
                        {
                            int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                            string monthRange =
                                " case   \n" +
                                " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                                  endFiscalYearMinusOne + " then " + "'Month:" +
                                  parameterValues.periodSelect.StartYearStartMonth + "-" +
                                  parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                                " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                " then " + "'Month:" +
                                  parameterValues.periodSelect.EndYearStartMonth + "-" +
                                  parameterValues.periodSelect.EndYearEndMonth + "'" + " end as FiscalMonth, ";

                            periodListing = " v.FiscalYear, " + monthRange;
                        }
                    }

                    frontListingAggrOption = vaggregationIdsPivot + " sequenceNo,  " + ServiceDictionaryTable + ".FullDescription, " + periodListing + "\n";
                }

                //vaggregationOptions = "";        
                aggregationIdsRows = "";
                vaggregationIdsRows = "";
                lastMonthAggrIdComma = "";

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    groupByListing =
                            "    group by " + vaggregationOptions + " sequenceNo, " +
                            "    " + ServiceDictionaryTable + ".FullDescription, v.FiscalYear, v.Quarter " + " \n";
                }
                else
                {
                    groupByListing =
                               "    group by " + vaggregationOptions + " sequenceNo, " +
                               "    " + ServiceDictionaryTable + ".FullDescription, v.FiscalYear " + " \n";
                }
            }
            else if (parameterValues.dataElementsPivot == "pivotdataelements")
            {
                frontListingAggrOption = vaggregationOptions;
                frontListingIds = vaggregationIdsRows;

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    periodListing = "v.FiscalYear, v.Quarter, ";

                    if (parameterValues.periodSelect.aggregate == true)
                    {
                        if (parameterValues.PeriodRange == "PeriodRangeQuarterly")
                        {
                            string quarterRange = "Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter;
                            periodListing = " v.FiscalYear, + " + "'" + quarterRange + "'" + " as Quarter, ";
                        }
                        else if (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
                        {
                            int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                            string quarterRange =
                                " case   \n" +
                                " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                                  endFiscalYearMinusOne + " then " + "'Quarter:" +
                                  parameterValues.periodSelect.StartYearStartQuarter + "-" +
                                  parameterValues.periodSelect.StartYearEndQuarter + "'" + "\n" +
                                " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                " then " + "'Quarter:" +
                                  parameterValues.periodSelect.EndYearStartQuarter + "-" +
                                  parameterValues.periodSelect.EndYearEndQuarter + "'" + " end as Quarter, ";

                            periodListing = " v.FiscalYear, " + quarterRange;
                        }
                    }

                    frontListingIds = vaggregationIdsRows + "\n" +
                                  " " + ServiceDictionaryTable + ".FullDescription, " + periodListing + "\n";
                }
                else
                {
                    periodListing = " v.FiscalYear, ";
                    if (parameterValues.periodSelect.aggregate == true)
                    {
                        if (parameterValues.PeriodRange == "PeriodRangeMonthly")
                        {
                            string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                            periodListing = " v.FiscalYear, " + "'" + monthRange + "'" + " as FiscalMonth, ";
                        }
                        else if (parameterValues.PeriodRange == "PeriodRangeYearMonthly")
                        {
                            int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                            string monthRange =
                                " case   \n" +
                                " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                                  endFiscalYearMinusOne + " then " + "'Month:" +
                                  parameterValues.periodSelect.StartYearStartMonth + "-" +
                                  parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                                " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                " then " + "'Month:" +
                                  parameterValues.periodSelect.EndYearStartMonth + "-" +
                                  parameterValues.periodSelect.EndYearEndMonth + "'" + " end as FiscalMonth, ";

                            periodListing = " v.FiscalYear, " + monthRange;
                        }
                    }

                    frontListingIds = vaggregationIdsRows + "\n" +
                                  " " + ServiceDictionaryTable + ".FullDescription, " + periodListing + "\n";
                }

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    groupByListing =
                              "    group by " + ServiceDictionaryTable + ".FullDescription, v.FiscalYear, v.Quarter, " +
                               vaggregationOptions + vaggregationIdsRows +
                              "    " + ServiceDictionaryTable + ".sequenceNo \n";
                }
                else
                {
                    groupByListing =
                               "    group by " + ServiceDictionaryTable + ".FullDescription, v.FiscalYear, " +
                                vaggregationOptions + vaggregationIdsRows +
                               "    " + ServiceDictionaryTable + ".sequenceNo \n";
                }

            }

            ProcessFacilityTypes(parameterValues, out facilityWhereQuery);

            string yearPeriodComma = string.Empty;
            string yearPeriodNoComma = string.Empty;

            if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
            {
                yearPeriodComma = " FiscalYear, Quarter,  ";
                yearPeriodNoComma = " FiscalYear, Quarter ";
            }
            else
            {
                yearPeriodComma = " FiscalYear, ";
                yearPeriodNoComma = " FiscalYear ";
            }



            string totalLastMonthQuery =

             "   select " + frontListingAggrOption + frontListingIds +
             "   Value from \n" +
             "   (   " +
             "            select " + ServiceVerticalSumTable + ".ID, " + yearPeriodComma + " [FiscalMonth], \n" +
                          // "            ReportingRegionName, EthEhmis_AllFacilityWithID.RegionId, \n" +
                          aggregationGroupBy + aggregationIdsRows +
             "            sum(value) as Value  \n" +
             "            from " + ServiceDictionaryTable + "  \n" +
             "            inner join " + ServiceVerticalSumTable + " on  \n" +
                             ServiceDictionaryTable + ".LabelID = " + ServiceVerticalSumTable + ".LabelId    \n" +
             "            inner join v_EthEhmis_HmisValue   \n " +
             "            on v_EthEhmis_HmisValue.LabelID = " + ServiceDictionaryTable + ".LabelID    \n" +
             "            inner join " + FacilityTable + " on   \n" +
             FacilityTable + ".HMISCode = v_EthEhmis_HmisValue.LocationID  \n" +
             "            where " + ServiceVerticalSumTable + ".ID in    (" + labelIdsSumLastMonthTotal + ")\n" +
             "            and (dataClass = 6 or dataClass = 7)   \n" +
                          //  "            (FiscalYear >= 2008 and FiscalYear <= 2008) " +
                          periodWhereSelection + institutionWhere + facilityWhereQuery + "\n" +
             "            group by " + aggregationGroupBy + aggregationIdsRows + yearPeriodComma + " FiscalMonth, " + ServiceVerticalSumTable + ".ID \n" +
             //"            " + ServiceVerticalSumTable + ".ID,  " +
             //"            ReportingRegionName, EthEhmis_AllFacilityWithID.RegionId, FiscalYear, FiscalMonth  " +
             "    ) s    \n" +
         "    inner join \n" +
         "   (   \n" +
         "   select    " + taggregationGroupBy + lastMonthAggrIdComma + "\n" +
         //"   select    " + aggregationIdsPivot + "\n" +
         // "   ReportingRegionName, RegionId, " + 
         "   ID,    \n" +
         yearPeriodComma + "  max(FiscalMonth) as FiscalMonth from    \n" +
             "   (   \n" +
             "       select " + ServiceVerticalSumTable + ".ID, " + yearPeriodComma + " [FiscalMonth],  \n" +
                     // "       ReportingRegionName, EthEhmis_AllFacilityWithID.RegionId, " + 
                     aggregationGroupBy + aggregationIdsRows + "\n" +
             "       sum(value) as Value   \n" +
             "       from " + ServiceDictionaryTable + "   \n" +
             "       inner join " + ServiceVerticalSumTable + " on  \n" +
                         ServiceDictionaryTable + ".LabelID = " + ServiceVerticalSumTable + ".LabelId \n" +
             "       inner join v_EthEhmis_HmisValue \n" +
             "       on v_EthEhmis_HmisValue.LabelID = " + ServiceDictionaryTable + ".LabelID  \n" +
             "       inner  join " + FacilityTable + " on   \n" +
             FacilityTable +  ".HMISCode = v_EthEhmis_HmisValue.LocationID   \n" +
             "       where " + ServiceVerticalSumTable + ".ID in    (" + labelIdsSumLastMonthTotal + ")\n" +
             "       and (dataClass = 6 or dataClass = 7) \n" +
                     //  "            (FiscalYear >= 2008 and FiscalYear <= 2008) " +
                     periodWhereSelection + institutionWhere + facilityWhereQuery + "\n" +
             "       group by " + aggregationGroupBy + aggregationIdsRows + yearPeriodComma + " FiscalMonth, " + ServiceVerticalSumTable + ".ID \n" +
             //"            " + ServiceVerticalSumTable + ".ID,  " +
             //"            ReportingRegionName, EthEhmis_AllFacilityWithID.RegionId, FiscalYear, FiscalMonth  " +
             "   ) t \n" +
         " group by " + taggregationGroupBy + lastMonthAggrIdComma + "\n" +
         //" group by " + aggregationGroupBy + "\n" +
         "   ID, " + yearPeriodNoComma + " \n" +
         //" ReportingRegionName, RegionId    " +
         " ) v   " +
         "   on v.ID = s.ID and  \n" +
         "      v.fiscalMonth = s.FiscalMonth and    \n" +
         "      v.fiscalYear = s.FiscalYear    \n" +
                 lastMonthInnerJoins + "\n" +
         "      inner join " + ServiceDictionaryTable + "   \n" +
         "      on " + ServiceDictionaryTable + ".VerticalSumID = v.ID \n" +
        orderByListing;

            return totalLastMonthQuery;
        }

        private string getLastMonthServiceQuery(AnalyticsParameters parameterValues, string labelIdsSumLastMonth, bool totalSumAll,
            bool totalCalcLabelId, bool totalCalcLabelIdLastMonth)
        {

            string aggregationOptions = string.Empty;
            string aggregationGroupBy = string.Empty;
            string institutionWhere = string.Empty;
            string aggregationIdsPivot = string.Empty;
            string aggregationIdsRows = string.Empty;
            string institutionType = string.Empty;
            string lastMonthAggrIdComma = string.Empty;
            string lastMonthAggrId = string.Empty;

            string vaggregationOptions = string.Empty;
            string vaggregationOptionsAs = string.Empty;
            string vaggregationIdsRows = string.Empty;
            string lastMonthInnerJoins = string.Empty;
            string vaggregationIdsPivot = string.Empty;
            string taggregationGroupBy = string.Empty;
            string facilityWhereQuery = string.Empty;

            processAggregationType(parameterValues, out aggregationOptions, out aggregationGroupBy, out aggregationIdsPivot,
                out aggregationIdsRows, out institutionWhere, out institutionType,
                out lastMonthAggrIdComma, out vaggregationOptions, out vaggregationOptionsAs,
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy,
                dataElementType.lastMonth);

            ProcessFacilityTypes(parameterValues, out facilityWhereQuery);

            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;
            bool lastMonth = true;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection, lastMonth);

            string frontListingAggrOption = string.Empty;
            string frontListingIds = string.Empty;
            string orderByListing = string.Empty;
            string groupByListing = string.Empty;

            if (parameterValues.dataElementsPivot == "row")
            {
                frontListingAggrOption = vaggregationOptionsAs;
                frontListingIds = vaggregationIdsRows + " v.sequenceNo as sequenceNo, \n";

                if (totalSumAll)
                {

                    if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                    {
                        orderByListing =
                        "    order by FiscalYear, Quarter, \n" +
                        aggregationGroupBy + aggregationIdsPivot +
                        "    sequenceNo  \n";
                    }
                    else
                    {
                        orderByListing =
                       "    order by FiscalYear, \n" +
                       aggregationGroupBy + aggregationIdsPivot +
                       "    sequenceNo  \n";
                    }
                }
                else
                {
                    if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                    {
                        orderByListing =
                       "    order by  v.FiscalYear, v.Quarter, \n" +
                         vaggregationOptions + vaggregationIdsRows +
                         "    sequenceNo  \n";
                    }
                    else
                    {
                        orderByListing =
                        "    order by  v.FiscalYear, \n" +
                          vaggregationOptions + vaggregationIdsRows +
                          "    sequenceNo  \n";
                    }
                }

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    groupByListing =
                               "    group by v.FullDescription, v.FiscalYear, v.Quarter,  " +
                               vaggregationOptions + vaggregationIdsRows +
                               "    v.sequenceNo \n";
                }
                else
                {
                    groupByListing =
                               "    group by v.FullDescription, v.FiscalYear, " +
                               vaggregationOptions + vaggregationIdsRows +
                               "    v.sequenceNo \n";
                }
            }
            else if (parameterValues.dataElementsPivot == "pivotinstitutions")
            {
                frontListingAggrOption = vaggregationIdsPivot + " v.sequenceNo as sequenceNo, ";
                aggregationIdsRows = "";
                vaggregationIdsRows = "";
                lastMonthAggrIdComma = "";

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    groupByListing =
                               "    group by " + vaggregationOptions + " v.sequenceNo, " +
                               "    v.FullDescription, v.FiscalYear, v.Quarter " + " \n";
                }
                else
                {
                    groupByListing =
                               "    group by " + vaggregationOptions + " v.sequenceNo, " +
                               "    v.FullDescription, v.FiscalYear " + " \n";
                }
            }
            else if (parameterValues.dataElementsPivot == "pivotdataelements")
            {
                frontListingAggrOption = vaggregationOptions;
                frontListingIds = vaggregationIdsRows;

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    groupByListing =
                              "    group by v.FullDescription, v.FiscalYear, v.Quarter, " +
                               vaggregationOptions + vaggregationIdsRows +
                              "    v.sequenceNo \n";
                }
                else
                {
                    groupByListing =
                               "    group by v.FullDescription, v.FiscalYear, " +
                                vaggregationOptions + vaggregationIdsRows +
                               "    v.sequenceNo \n";
                }
            }
            else
            {
                frontListingAggrOption = vaggregationOptions;
                frontListingIds = vaggregationIdsRows;

                if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
                {
                    orderByListing =
                    "    order by v.FiscalYear, v.Quarter, \n" +
                      vaggregationOptions + vaggregationIdsRows +
                   "    v.sequenceNo  \n";

                    groupByListing =
                                   "    group by v.FullDescription, v.FiscalYear, v.Quarter, " +
                   //"    v.ReportingRegionName, \n" +
                   vaggregationOptions + vaggregationIdsRows +
                   "    v.sequenceNo \n";
                }
                else
                {
                    orderByListing =
                     "    order by v.FiscalYear, \n" +
                       vaggregationOptions + vaggregationIdsRows +
                    "    v.sequenceNo  \n";

                    groupByListing =
                                   "    group by v.FullDescription, v.FiscalYear, " +
                   //"    v.ReportingRegionName, \n" +
                   vaggregationOptions + vaggregationIdsRows +
                   "    v.sequenceNo \n";
                }
            }

            if (totalCalcLabelId || totalCalcLabelIdLastMonth)
            {
                orderByListing = "";
            }

            string yearPeriodComma = string.Empty;
            string vYearPeriodComma = string.Empty;
            string yearPeriodNoComma = string.Empty;
            string vYearPeriodNoComma = string.Empty;

            if ((parameterValues.PeriodRange == "PeriodRangeQuarterly") || (parameterValues.PeriodRange == "PeriodRangeYearQuarterly"))
            {
                yearPeriodComma = " FiscalYear, Quarter,  ";
                yearPeriodNoComma = " FiscalYear, Quarter ";

                vYearPeriodComma = " v.FiscalYear, v.Quarter,  ";
                vYearPeriodNoComma = " v.FiscalYear, v.Quarter ";

                if (parameterValues.periodSelect.aggregate == true)
                {
                    if (parameterValues.PeriodRange == "PeriodRangeQuarterly")
                    {
                        string quarterRange = "Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter;
                        vYearPeriodComma = " v.FiscalYear, + " + "'" + quarterRange + "'" + " as Quarter, ";
                    }
                    else if (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
                    {
                        int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                        string quarterRange =
                            " case   \n" +
                            " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                              endFiscalYearMinusOne + " then " + "'Quarter:" +
                              parameterValues.periodSelect.StartYearStartQuarter + "-" +
                              parameterValues.periodSelect.StartYearEndQuarter + "'" + "\n" +
                            " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                            " then " + "'Quarter:" +
                              parameterValues.periodSelect.EndYearStartQuarter + "-" +
                              parameterValues.periodSelect.EndYearEndQuarter + "'" + " end as Quarter, ";

                        vYearPeriodComma = " v.FiscalYear, " + quarterRange;
                    }
                }
            }
            else
            {
                yearPeriodComma = " FiscalYear, ";
                yearPeriodNoComma = " FiscalYear ";

                vYearPeriodComma = " v.FiscalYear,  ";
                vYearPeriodNoComma = " v.FiscalYear ";

                if (parameterValues.periodSelect.aggregate == true)
                {
                    if (parameterValues.PeriodRange == "PeriodRangeMonthly")
                    {
                        string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                        vYearPeriodComma = " v.FiscalYear, " + "'" + monthRange + "'" + " as FiscalMonth, ";
                    }
                    else if (parameterValues.PeriodRange == "PeriodRangeYearMonthly")
                    {
                        int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                        string monthRange =
                            " case   \n" +
                            " when v.FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                              endFiscalYearMinusOne + " then " + "'Month:" +
                              parameterValues.periodSelect.StartYearStartMonth + "-" +
                              parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                            " when v.FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                            " then " + "'Month:" +
                              parameterValues.periodSelect.EndYearStartMonth + "-" +
                              parameterValues.periodSelect.EndYearEndMonth + "'" + " end as FiscalMonth, ";

                        vYearPeriodComma = " v.FiscalYear, " + monthRange;
                    }
                }
            }

            string cmdText =
                 "   select " +
                 // "   v.ReportingRegionName, " +
                 frontListingAggrOption + frontListingIds +
                 "   v.FullDescription, " + vYearPeriodComma +
                 "   sum(value) as Value from \n" +
                 "   ( " +
                 "           select " +
                 //"           ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 "  FullDescription, sequenceNo, " +
                 yearPeriodComma + " [FiscalMonth], \n" +
                 "           sum(value) as Value from v_EthEhmis_HmisValue   " +
                 "  INNER JOIN " + FacilityTable + " on    " +
                 FacilityTable + ".hmiscode = v_EthEhmis_HmisValue.LocationId \n" +
                 "  INNER JOIN " + ServiceDictionaryTable + "  on  " +
                  ServiceDictionaryTable + ".LabelId = v_EthEhmis_HmisValue.LabelID \n" +
                 "           where  " + ServiceDictionaryTable + ".labelId in ( " +
                 labelIdsSumLastMonth + " )     \n" +
                 periodWhereSelection + institutionWhere + facilityWhereQuery +
                 "           group by FullDescription, sequenceNo, \n" +
                 //" ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 yearPeriodComma + "  [FiscalMonth]  \n" +
                 "   )s " +
                 "   inner join " +
                 "   (  " +
                 "           select " +
                 //"           ReportingRegionName, " +
                 taggregationGroupBy + lastMonthAggrIdComma +
                 "           FullDescription, sequenceNo, " +
                 yearPeriodComma + "   max(FiscalMonth) as FiscalMonth from \n" +
                 "           (  " +
                  "           select " +
                 //"           ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 "              FullDescription, sequenceNo, " +
                 yearPeriodComma + "   [FiscalMonth], \n" +
                 "              sum(value) as Value from v_EthEhmis_HmisValue   " +
                 "              INNER JOIN " + FacilityTable + " on    " +
                                FacilityTable + ".hmiscode = v_EthEhmis_HmisValue.LocationId \n" +
                 "              INNER JOIN " + ServiceDictionaryTable + "  on  " +
                                 ServiceDictionaryTable + ".LabelId = v_EthEhmis_HmisValue.LabelID \n" +
                 "           where  " + ServiceDictionaryTable + ".labelId in ( " +
                                labelIdsSumLastMonth + " )     \n" +
                                periodWhereSelection + institutionWhere +
                 "              group by FullDescription, sequenceNo, \n" +
                 //"              ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 yearPeriodComma + "   [FiscalMonth]  \n" +
                 "           ) t    " +
                 "              group by FullDescription, sequenceNo, " +
                 //"              ReportingRegionName, " +
                 taggregationGroupBy + lastMonthAggrIdComma +
                 yearPeriodNoComma + "\n" +
                 "   ) v    " +
                 "    on v.FullDescription = s.FullDescription and   \n" +
                 "    v.fiscalMonth = s.FiscalMonth and  \n" +
                 "    v.fiscalYear = s.FiscalYear    \n" +
                 //"    v.ReportingRegionName  = s.ReportingRegionName " + "\n" +
                 lastMonthInnerJoins +
                 groupByListing +
                 orderByListing;

            return cmdText;
        }

        private string getServiceQuery(AnalyticsParameters parameterValues, string fullDescriptions,
            string fullDescriptionsPivot, string labelIdsSumAll, string labelIdsSumLastMonth, string labelIdsSumAllTotal,
            string labelIdsSumLastMonthTotal, bool lastMonthCalc, bool totalCalcLabelId,
            bool totalCalcLabelIdLastMonth, bool totalSumAll, string facilityWhereQuery)
        {

            string aggregationOptions = string.Empty;
            string aggregationGroupBy = string.Empty;
            string institutionWhere = string.Empty;
            string aggregationIdsPivot = string.Empty;
            string aggregationIdsRows = string.Empty;
            string institutionType = string.Empty;
            string lastMonthAggrIdComma = string.Empty;
            string lastMonthAggrId = string.Empty;

            string vaggregationOptions = string.Empty;
            string vaggregationIdsRows = string.Empty;
            string lastMonthInnerJoins = string.Empty;
            string vaggregationOptionsAs = string.Empty;
            string vaggregationIdsPivot = string.Empty;
            string taggregationGroupBy = string.Empty;


            processAggregationType(parameterValues, out aggregationOptions, out aggregationGroupBy, out aggregationIdsPivot,
                out aggregationIdsRows, out institutionWhere, out institutionType,
                out lastMonthAggrIdComma, out vaggregationOptions, out vaggregationOptionsAs,
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy,
                dataElementType.sumAll);

            string cmdText = "";

            // If dataelements are greater than 15, difficult, slow, non-user friendly to use pivot with columns so disable it by default....
            //if (parameterValues.dataElements.Length > 15)
            //{
            //    parameterValues.dataElementsPivot = false;
            //}

            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;
            bool lastMonth = false;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection, lastMonth);

            string unionAll = string.Empty;

            if (lastMonthCalc)
            {
                if (totalSumAll)
                {
                    unionAll = " union  \n" + getLastMonthServiceQuery(parameterValues, labelIdsSumLastMonth, totalSumAll, totalCalcLabelId, totalCalcLabelIdLastMonth);
                }
                else
                {
                    unionAll = getLastMonthServiceQuery(parameterValues, labelIdsSumLastMonth, totalSumAll, totalCalcLabelId, totalCalcLabelIdLastMonth) + " \n";
                }
            }

            if (totalCalcLabelId)
            {
                if ((totalSumAll) || (lastMonthCalc))
                {
                    unionAll += " union  \n" + getTotalAggregateQuery(parameterValues, labelIdsSumAllTotal, totalSumAll, totalCalcLabelIdLastMonth, lastMonthCalc);
                }
                else
                {
                    unionAll += getTotalAggregateQuery(parameterValues, labelIdsSumAllTotal, totalSumAll, totalCalcLabelIdLastMonth, lastMonthCalc) + " \n";
                }
            }

            if (totalCalcLabelIdLastMonth)
            {
                if ((totalSumAll) || (totalCalcLabelId) || (lastMonthCalc))
                {
                    unionAll += " union  \n" + getLastMonthServiceQueryTotal(parameterValues, labelIdsSumLastMonthTotal, totalSumAll, totalCalcLabelId, lastMonthCalc);
                }
                else
                {
                    unionAll += getLastMonthServiceQueryTotal(parameterValues, labelIdsSumLastMonthTotal, totalSumAll, totalCalcLabelId, lastMonthCalc) + " \n";
                }
            }

            //string totalQuery = getTotalAggregateQuery(parameterValues, labelIdsSumAllTotal, totalCalcLabelIdLastMonth);

            //string totalQueryLastMonth = getLastMonthServiceQueryTotal(parameterValues, labelIdsSumLastMonthTotal, "");

            string sumAllInsideQueryDataElements = string.Empty;
            string sumAllInsideQueryInstitutions = string.Empty;
            string sumAllInsideQueryRow = string.Empty;

            if (totalSumAll)
            {
                if (parameterValues.dataElementsPivot == "pivotdataelements")
                {
                    sumAllInsideQueryDataElements =
                               " select " + aggregationGroupBy + aggregationIdsRows + "\n" +
                    " FullDescription, " + "\n" +
                    //" FiscalYear, " +
                    periodListing + "\n" +
                    " sum(value) as value  from " + FacilityTable + "\n" +
                    " inner join v_EthEhmis_HmisValue on " + "\n" +
                    FacilityTable + ".HMISCode = v_EthEhmis_HmisValue.LocationID " + "\n" +
                    " inner " + "\n" +
                    " join  " + ServiceDictionaryTable + "   on " + "\n" +
                    "  " + ServiceDictionaryTable + ".LabelId = v_EthEhmis_HmisValue.LabelId " + "\n" +
                    "   where v_EthEhmis_HmisValue.labelId in  " + "\n" +
                    " ( " + labelIdsSumAll + " ) " + "\n" +
                    " and (dataClass = 6 or dataClass =7 )" + institutionWhere + facilityWhereQuery + "\n" +
                    //" and " + " (FiscalYear = 2008) " +
                    periodWhereSelection + "\n" +
                    " group by " + ServiceDictionaryTable + ".FullDescription, " + "\n" +
                    aggregationGroupBy + aggregationIdsRows + "\n" +
                    //" FiscalYear " +
                    periodGroupByListing + "\n";
                }
                else if (parameterValues.dataElementsPivot == "pivotinstitutions")
                {
                    sumAllInsideQueryInstitutions =
                       " select " + aggregationIdsPivot + " sequenceNo, " + "\n" +
                       " FullDescription, " + "\n" +
                       //" FiscalYear, " +
                       periodListing + "\n" +
                       " sum(value) as value  from " + FacilityTable + "\n" +
                       " inner join v_EthEhmis_HmisValue on " + "\n" +
                       FacilityTable + ".HMISCode = v_EthEhmis_HmisValue.LocationID " + "\n" +
                       " inner " + "\n" +
                       " join " + ServiceDictionaryTable + "  on " + "\n" +
                       "  " + ServiceDictionaryTable + ".LabelId = v_EthEhmis_HmisValue.LabelId " + "\n" +
                       "   where v_EthEhmis_HmisValue.labelId in  " + "\n" +
                       " ( " + labelIdsSumAll + " ) " + "\n" +
                       " and (dataClass = 6 or dataClass =7 )" + institutionWhere + facilityWhereQuery + "\n" +
                       //" and " + " (FiscalYear = 2008) " +
                       periodWhereSelection + "\n" +
                       " group by " + ServiceDictionaryTable + ".FullDescription, " + "\n" +
                       aggregationGroupBy + " sequenceNo, " + "\n" +
                       //" FiscalYear " +
                       periodGroupByListing + "\n";
                }
                else
                {
                    string orderByListing = string.Empty;

                    if ((lastMonthCalc == false) && (totalCalcLabelId == false) && (totalCalcLabelIdLastMonth == false))
                    {
                        orderByListing =
                            " order by " + aggregationGroupBy + " sequenceNo, \n" +
                            // " FiscalYear ";
                            periodGroupByListing + "\n";
                    }

                    sumAllInsideQueryRow =
                         " select  " + aggregationOptions + aggregationIdsRows + "sequenceNo, " + "\n" +
                    " FullDescription, " +
                    //" FiscalYear, " +
                    periodListing +
                    " sum(value) as value  from " + FacilityTable + "\n" +
                    " inner join v_EthEhmis_HmisValue on " +
                    FacilityTable + ".HMISCode = v_EthEhmis_HmisValue.LocationID \n" +
                    " inner " +
                    " join " + ServiceDictionaryTable + "  on " +
                    "  " + ServiceDictionaryTable + ".LabelId = v_EthEhmis_HmisValue.LabelId \n" +
                    "   where v_EthEhmis_HmisValue.labelId in  " +
                    " ( " + labelIdsSumAll + " ) " +
                    " and (dataClass = 6 or dataClass = 7 )" + institutionWhere + facilityWhereQuery + "\n" +
                    //" and " + " (FiscalYear = 2008) " +
                    periodWhereSelection +
                    " group by " + ServiceDictionaryTable + ".FullDescription, \n" +
                    aggregationGroupBy + " sequenceNo, " + aggregationIdsRows + " \n " +
                    //" FiscalYear " +
                    periodGroupByListing +
                    orderByListing;
                }
            }

            //if (parameterValues.dataElementsPivot == true)
            if (parameterValues.dataElementsPivot == "pivotdataelements")
            {
                // For now just choose this as default
                cmdText =
                    " select " + aggregationOptions + aggregationIdsPivot + "\n" +
                    //" FiscalYear, " + 
                    periodListing + "\n" +
                    fullDescriptions + "\n" +
                    " from " + "\n" +
                    " ( " + sumAllInsideQueryDataElements +

                    unionAll + "\n" +
                    "    ) as t  " + "\n" +
                    "    pivot " + "\n" +
                    "    (  " + "\n" +
                    "        max(value) for FullDescription in  " + "\n" +
                    "    (  " + fullDescriptionsPivot + " ) " + "\n" +
                    "    )p " + "\n" +
                    "    order by " + aggregationGroupBy + "\n" +
                    //" FiscalYear ";
                    periodGroupByListing;
            }
            else if (parameterValues.dataElementsPivot == "pivotinstitutions")
            {
                // For now just choose this as default
                cmdText =
                    " select " + periodListing + " FullDescription, " + aggregationOptions + "\n" +
                    //" FiscalYear, " + 

                    " from " + "\n" +
                    " ( " +
                    sumAllInsideQueryInstitutions +
                    unionAll + "\n" +
                    "    ) as t  " + "\n" +
                    "    pivot " + "\n" +
                    "    (  " + "\n" +
                    "        max(value) for " + institutionType + " in  " + "\n" +
                    "    (  " + aggregationOptions + " ) " + "\n" +
                    "    )p " + "\n" +
                    "    order by sequenceNo, " + periodGroupByListing + "\n";
                //" FiscalYear ";

            }
            else
            {
                // For now just choose this as default
                cmdText =
                   sumAllInsideQueryRow +
                   unionAll;
            }
            return cmdText;
        }

        private void processAggregationType(AnalyticsParameters parameterValues, out string aggregationOptions, out string aggregationGroupBy,
            out string aggregationIdsPivot, out string aggregationIdsRows,
            out string institutionWhere, out string institutionType,
            out string lastMonthAggrIdComma, out string vaggregationOptions, out string vaggregationOptionsAs,
            out string vaggregationIdsRows, out string lastMonthInnerJoins, out string vaggregationIdsPivot, out string taggregationGroupBy,
            dataElementType elementType)
        {
            aggregationOptions = string.Empty;
            aggregationGroupBy = string.Empty;
            institutionWhere = string.Empty;
            aggregationIdsPivot = string.Empty;
            aggregationIdsRows = string.Empty;
            institutionType = string.Empty;
            lastMonthAggrIdComma = string.Empty;
            vaggregationOptions = string.Empty;
            vaggregationIdsRows = string.Empty;
            lastMonthInnerJoins = string.Empty;
            vaggregationOptionsAs = string.Empty;
            vaggregationIdsPivot = string.Empty;
            taggregationGroupBy = string.Empty;

            //string facilityTable = "  v_EthEhmis_AllFacilityWithID";

            if (parameterValues.institutionAggrOptions.aggrType == "federal")
            {
                // No Institutional aggregation type....
                //"   ReportingRegionName as RegionName, ZoneName, WoredaName, " +
                aggregationOptions = string.Empty;
                aggregationGroupBy = string.Empty;
                institutionWhere = string.Empty;
                lastMonthAggrIdComma = string.Empty;
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "region")
            {
                lastMonthAggrIdComma = " RegionId, ";
                vaggregationOptions = " v.ReportingRegionName, ";
                vaggregationOptionsAs = " v.ReportingRegionName as RegionName, ";
                vaggregationIdsRows = " v.RegionId, ";

                if (parameterValues.dataElementsPivot == "pivotinstitutions")
                {
                    institutionType = " t.ReportingRegionName ";
                    if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                    {
                        if (parameterValues.institutionAggrOptions.hmiscode != null)
                        {
                            institutionWhere = "  and ( " + FacilityTable + ".regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                        }
                    }

                    string cmdText = " select distinct ReportingRegionName as Name " +
                                     " from " + FacilityTable + " where " +
                                     " (hmiscode != '88') " + institutionWhere;

                    DBConnHelper _helper = new DBConnHelper();
                    SqlCommand parameterCmd = new SqlCommand(cmdText);
                    DataTable dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    int count = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        count++;
                        if (dt.Rows.Count == count) // Last item, do something else
                        {
                            aggregationOptions += "[" + row["Name"].ToString() + "]\n";
                        }
                        else
                        {
                            aggregationOptions += "[" + row["Name"].ToString() + "],\n";
                        }
                    }

                    aggregationGroupBy = "   ReportingRegionName,  ";
                    aggregationIdsPivot = " ReportingRegionName, ";
                    aggregationIdsRows = FacilityTable + ".RegionId, ";

                    lastMonthInnerJoins = " and \n  " +
                                    " v.ReportingRegionName = s.ReportingRegionName \n";
                }
                else
                {
                    lastMonthInnerJoins = " and \n  " +
                                    " v.ReportingRegionName = s.ReportingRegionName \n ";
                    //and \n" +
                    //" v.RegionId = s.RegionId \n";

                    aggregationOptions = "   ReportingRegionName as RegionName,  ";
                    aggregationGroupBy = "   ReportingRegionName,  ";
                    aggregationIdsPivot = " RegionId, ";
                    aggregationIdsRows = FacilityTable + ".RegionId, ";
                }

                taggregationGroupBy = aggregationGroupBy;
                vaggregationIdsPivot = vaggregationOptions;
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "zone")
            {
                lastMonthAggrIdComma = "RegionId, ZoneId, ";

                lastMonthAggrIdComma = " RegionId, ZoneId, ";
                vaggregationOptions = " v.ReportingRegionName, v.ZoneName, ";
                vaggregationOptionsAs = " v.ReportingRegionName as RegionName,  v.ZoneName, ";
                vaggregationIdsRows = " v.RegionId, v.ZoneId,  ";

                if (parameterValues.institutionAggrOptions.facilityTypeId == 9) // It is a zone
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".zoneId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }

                if (parameterValues.dataElementsPivot == "pivotinstitutions")
                {
                    institutionType = " t.ZoneName ";
                    //aggregationOptions = "   ReportingRegionName as RegionName,  ";

                    string cmdText = " select ReportingRegionName + '_' + Name as Name from " +
                                     "   ( " +
                                     "   select distinct(rtrim(replace(zoneName, 'Zonal Health Department', ''))) as " +
                                     "   Name, ReportingRegionName, RegionId, ZoneId from " + FacilityTable + 
                                     "   where zoneName not like('%NoZone_National%') " + institutionWhere +
                                     "   ) as t  order by ReportingRegionName ";


                    DBConnHelper _helper = new DBConnHelper();
                    SqlCommand parameterCmd = new SqlCommand(cmdText);
                    DataTable dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    int count = 0;
                    aggregationIdsPivot = "   ReportingRegionName + '_' + rtrim(replace(zoneName, 'Zonal Health Department', '')) " +
                                          "   as ZoneName, ";
                    vaggregationIdsPivot = "   v.ReportingRegionName + '_' + rtrim(replace(v.zoneName, 'Zonal Health Department', '')) " +
                                          "   as ZoneName, ";
                    foreach (DataRow row in dt.Rows)
                    {
                        count++;
                        if (dt.Rows.Count == count) // Last item, do something else
                        {
                            aggregationOptions += "[" + row["Name"].ToString().Trim() + "]\n";
                        }
                        else
                        {
                            aggregationOptions += "[" + row["Name"].ToString().Trim() + "],\n";
                        }
                    }


                    aggregationGroupBy = " ReportingRegionName, ZoneName, ";
                    lastMonthInnerJoins = " and \n  " +
                                   " v.ReportingRegionName = s.ReportingRegionName and \n" +
                                   " v.ZoneName = s.ZoneName \n";
                }
                else
                {
                    aggregationOptions = "   ReportingRegionName as RegionName, ZoneName,  ";
                    aggregationGroupBy = "   ReportingRegionName, ZoneName, ";
                    aggregationIdsPivot = " RegionId, ZoneId, ";
                    aggregationIdsRows = FacilityTable + ".RegionId, " + FacilityTable + ".ZoneId, ";

                    lastMonthInnerJoins = " and \n  " +
                                   " v.RegionId = s.RegionId and \n" +
                                   " v.ZoneName = s.ZoneName \n";
                }
                taggregationGroupBy = aggregationGroupBy;
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "district")
            {
                lastMonthAggrIdComma = "RegionId, ZoneId, WoredaId, ";

                vaggregationOptionsAs = " v.ReportingRegionName as RegionName,  v.ZoneName, v.WoredaName, ";
                vaggregationIdsRows = " v.RegionId, v.ZoneId, v.WoredaId,  ";

                if (parameterValues.institutionAggrOptions.facilityTypeId == 8) // It is a Woreda
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".woredaId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 9) // It is a Zone
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".zoneId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }

                if (parameterValues.dataElementsPivot == "pivotinstitutions")
                {
                    institutionType = " t.WoredaName ";
                    //aggregationOptions = "   ReportingRegionName as RegionName,  ";

                    string woredaId1 = "cast(WoredaID as varchar(100))";
                    string woredaId2 = "cast(" + FacilityTable + ".WoredaID as varchar(100))";

                    string cmdText = " select ReportingRegionName + '_' + Name + '_' + " + woredaId1 + " as Name from " +
                                     "   ( " +
                                     "   select distinct(rtrim(WoredaName)) as " +
                                     "   Name, ReportingRegionName, RegionId, WoredaId from " + FacilityTable +
                                     "   where WoredaName not like('NoWoreda%National%') " + institutionWhere +
                                     "   ) as t  order by ReportingRegionName ";


                    DBConnHelper _helper = new DBConnHelper();
                    SqlCommand parameterCmd = new SqlCommand(cmdText);
                    DataTable dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    int count = 0;

                    if (elementType == dataElementType.sumAllTotal)
                    {
                        aggregationIdsPivot = "   ReportingRegionName + '_' + rtrim(WoredaName) + '_' + " + woredaId1 + " as WoredaName, ";
                    }
                    else
                    {
                        aggregationIdsPivot = "   ReportingRegionName + '_' + rtrim(WoredaName) + '_' + " + woredaId2 + " as WoredaName, ";
                    }

                    string vworedaId2 = "cast(v.WoredaID as varchar(100))";
                    vaggregationIdsPivot = "   v.ReportingRegionName + '_' + rtrim(v.WoredaName) + '_' + " + vworedaId2 + " as WoredaName, ";

                    foreach (DataRow row in dt.Rows)
                    {
                        count++;
                        if (dt.Rows.Count == count) // Last item, do something else
                        {
                            aggregationOptions += "[" + row["Name"].ToString().Trim() + "]\n";
                        }
                        else
                        {
                            aggregationOptions += "[" + row["Name"].ToString().Trim() + "],\n";
                        }
                    }


                    aggregationGroupBy = " ReportingRegionName, WoredaName, " + FacilityTable + ".WoredaId, ";
                    taggregationGroupBy = " ReportingRegionName, WoredaName, " + " t.WoredaId, ";

                    lastMonthInnerJoins = " and \n  " +
                                  " v.ReportingRegionName = s.ReportingRegionName and \n" +
                                  " v.WoredaName = s.WoredaName \n";
                    vaggregationOptions = " v.ReportingRegionName, v.WoredaId, v.WoredaName,  ";
                }
                else
                {
                    aggregationOptions = "   ReportingRegionName as RegionName, ZoneName, WoredaName, ";
                    aggregationGroupBy = "   ReportingRegionName, ZoneName, WoredaName, ";
                    aggregationIdsPivot = " RegionId, ZoneId, WoredaId, ";
                    aggregationIdsRows = FacilityTable + ".RegionId, " + FacilityTable + ".ZoneId, " + FacilityTable + ".WoredaId, ";
                    vaggregationOptions = " v.ReportingRegionName, v.ZoneName, v.WoredaName,  ";
                    taggregationGroupBy = aggregationGroupBy;

                    lastMonthInnerJoins = " and \n  " +
                   " v.ReportingRegionName = s.ReportingRegionName and \n" +
                   " v.WoredaId = s.WoredaId \n";
                }
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "facility")
            {
                //lastMonthAggrIdComma = "RegionId, ZoneId, WoredaId, LocationId, ";

                vaggregationOptions = " v.ReportingRegionName, v.ZoneName, v.WoredaName, v.FacilityName, v.FacilityTypeName, v.LocationId, ";
                vaggregationOptionsAs = " v.ReportingRegionName as RegionName,  v.ZoneName, v.WoredaName, v.FacilityName, v.FacilityTypeName, v.LocationId, ";
                // vaggregationIdsRows = " v.RegionId, v.ZoneId, v.WoredaId,  v.LocationId, ";
                lastMonthInnerJoins = " and \n  " +
                                      " v.LocationId = s.LocationId \n";
                //  // " v.RegionId = s.RegionId and \n" +
                //  " v.ZoneName = s.ZoneName and \n" +
                //  // " v.ZoneId = s.ZoneId and \n" +
                //  " v.WoredaName = s.WoredaName and \n" +
                //  // " v.WoredaId = s.WoredaId and \n" +
                //  " v.FacilityName = s.FacilityName ";
                ////  " v.LocationId = s.LocationId \n";

                aggregationOptions = "   ReportingRegionName as RegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName, LocationId, ";
                aggregationGroupBy = "   ReportingRegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName, LocationId, ";
                taggregationGroupBy = aggregationGroupBy;

                if (parameterValues.institutionAggrOptions.facilityTypeId == 8) // It is a Woreda
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".woredaId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 9) // It is a Zone
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".zoneId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    if (parameterValues.institutionAggrOptions.hmiscode != null)
                    {
                        institutionWhere = "  and ( " + FacilityTable + ".regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                    }
                }
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "none") // Lowest facility
            {
                aggregationOptions = "   ReportingRegionName as RegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName, LocationId,  ";
                aggregationGroupBy = "   ReportingRegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName, LocationId,  ";
                taggregationGroupBy = aggregationGroupBy;

                if (parameterValues.institutionAggrOptions.hmiscode != null)
                {
                    institutionWhere = "  and ( " + FacilityTable + ".hmiscode = '" + parameterValues.institutionAggrOptions.hmiscode + "') ";
                }

            }

            if ((parameterValues.institutionAggrOptions.reportingFacilityTypeId == 10) || (parameterValues.institutionAggrOptions.reportingFacilityTypeId == 9) ||
                (parameterValues.institutionAggrOptions.reportingFacilityTypeId == 8))
            {
                // Reporting Site specified
                if (parameterValues.institutionAggrOptions.hmiscode != null)
                {
                    institutionWhere += " and reportingFacilityTypeId = " + parameterValues.institutionAggrOptions.reportingFacilityTypeId + "    ";
                }
            }

            // okay, after all this, further consider what was passed via the locations whitelist and their facility types...
            if (parameterValues.FilterFacilityType != 0)
            {
                if (parameterValues.FilterFacilityType == 11)
                {
                    //MOH, do nothing more...
                }
                else
                {
                    if (parameterValues.FilterFacilityType == 10)// regions
                    {
                        institutionWhere += " and (" + FacilityTable + ".regionId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                    }
                    else
                        if (parameterValues.FilterFacilityType == 9)// zone
                    {
                        institutionWhere += " and (" + FacilityTable + ".zoneId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                    }
                    if (parameterValues.FilterFacilityType == 8)// woreda
                    {
                        institutionWhere += " and (" + FacilityTable + ".woredaId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                    }

                }
            }
        }


        private DataTable DiseaseProcessing(AnalyticsParameters parameterValues, string facilityWhereQuery)
        {
            List<object> stringAndDataTable = new List<object>();
            DBConnHelper _helper = new DBConnHelper();           

            // Configure selected diseases
            int count = 0;
            string diseaseList = string.Empty;
            string aggregationOptions = string.Empty;
            string aggregationGroupBy = string.Empty;
            string institutionWhere = string.Empty;
            string aggregationIdsPivot = string.Empty;
            string aggregationIdsRows = string.Empty;
            string initialListing = string.Empty;
            string innerJoinPopulation = string.Empty;
            string institutionType = string.Empty;
            string lastMonthAggrId = string.Empty;
            string lastMonthAggrIdComma = string.Empty;
            string vaggregationOptions = string.Empty;
            string vaggregationIdsRows = string.Empty;
            string lastMonthInnerJoins = string.Empty;
            string vaggregationOptionsAs = string.Empty;
            string vaggregationIdsPivot = string.Empty;
            string taggregationGroupBy = string.Empty;


            processCasePerPopulation(parameterValues, out initialListing, out innerJoinPopulation);

            processAggregationType(parameterValues, out aggregationOptions, out aggregationGroupBy, out aggregationIdsPivot,
                out aggregationIdsRows, out institutionWhere, out institutionType,
                out lastMonthAggrIdComma, out vaggregationOptions, out vaggregationOptionsAs,
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy,
                dataElementType.disease);

            //if ((parameterValues.dataElements.Length < 20) && (parameterValues.dataElements.Length != 0))
            string updateGroupName = "";
            string diseaseTableName = getDiseaseTableName();
            if (parameterValues.dataElements.Length != 0)
            {
                diseaseList = " ( disease in (";
                foreach (SelectedDataElements elements in parameterValues.dataElements)
                {
                    count++;
                    if (parameterValues.dataElements.Length == count) // Last item, do something else
                    {
                        diseaseList += "N'" + elements.Disease + "'))\n";
                    }
                    else
                    {
                        diseaseList += "N'" + elements.Disease + "',\n";
                    }

                    if ((elements.Disease != elements.groupName) && (elements.groupName != ""))
                    {
                        // should be updated
                        updateGroupName += " update " + diseaseTableName + " set groupName = '" + elements.groupName + "'" + "\n" +
                                           " where disease = N'" + elements.Disease + "'" + "\n\n";
                    }
                }
            }

            prepareDiseaseTable(parameterValues, diseaseList, diseaseTableName, updateGroupName);

            string dataClass = "";
            bool showIPD_OPD_Column = false;
            bool showAge_Column = false;
            bool showGender_column = false;

            string valueType = string.Empty;
            string ipdOpdWhereQuery = string.Empty;
            string ageWhereQuery = string.Empty;
            string genderWhereQuery = string.Empty;
            string genderColumn = string.Empty;
            string ageColumn = string.Empty;
            string ipdOpdColumn = string.Empty;
            string ipdGroupBy = string.Empty;

            processAgeGenderDepartment(parameterValues, out showAge_Column, out ageWhereQuery,
                out showGender_column, out genderWhereQuery, out showIPD_OPD_Column,
                out ipdOpdWhereQuery, out dataClass, out genderColumn, out ageColumn);

            if (parameterValues.diseaseOptions.caseDeath == "mortality")
            {
                valueType = " as Deaths ";
                if (parameterValues.diseaseOptions.ipd == true)
                {
                    dataClass = " ( v_EthEhmis_HmisValue.dataclass = 3 )";
                    showIPD_OPD_Column = true;
                }
                else
                {
                    dataClass = " ( v_EthEhmis_HmisValue.dataclass = 3 )";
                    showIPD_OPD_Column = false;
                }
            }
            else if (parameterValues.diseaseOptions.caseDeath == "morbidity")
            {
                valueType = " as Cases ";
                if ((parameterValues.diseaseOptions.ipd == true) && (parameterValues.diseaseOptions.opd == true))
                {
                    dataClass = " (v_EthEhmis_HmisValue.dataclass = 2 or v_EthEhmis_HmisValue.dataclass = 8 )";
                    showIPD_OPD_Column = true;
                }
                else if ((parameterValues.diseaseOptions.ipd == true) && (parameterValues.diseaseOptions.opd == false))
                {
                    dataClass = " ( v_EthEhmis_HmisValue.dataclass = 2 ) ";
                    showIPD_OPD_Column = true;
                }
                if ((parameterValues.diseaseOptions.ipd == false) && (parameterValues.diseaseOptions.opd == true))
                {
                    dataClass = " ( v_EthEhmis_HmisValue.dataclass = 8 ) ";
                    showIPD_OPD_Column = true;
                }
                if ((parameterValues.diseaseOptions.ipd == false) && (parameterValues.diseaseOptions.opd == false))
                {
                    dataClass = " (v_EthEhmis_HmisValue.dataclass = 2 or v_EthEhmis_HmisValue.dataclass = 8 )";
                    showIPD_OPD_Column = false;
                }
            }

            if (showIPD_OPD_Column)
            {
                // Morbidity
                if (parameterValues.diseaseOptions.caseDeath == "morbidity")
                {
                    ipdOpdColumn = " case when v_EthEhmis_HmisValue.DataClass = 2 then 'IPD_Cases'  " +
                        "   when v_EthEhmis_HmisValue.dataclass = 8 then 'OPD_Cases' end as CaseType, ";
                }
                else if (parameterValues.diseaseOptions.caseDeath == "mortality")
                {
                    ipdOpdColumn = " case when v_EthEhmis_HmisValue.DataClass = 3 then 'IPD_Deaths'  " +
                                   " end as CaseType, ";
                }
                ipdGroupBy = " v_EthEhmis_HmisValue.dataclass, ";
            }

            string diseaseQuery = string.Empty;
            if (diseaseList != string.Empty)
            {
                diseaseQuery = " and " + diseaseList;
            }

            string aggregateWithDisease = string.Empty; // do not aggregate by disease by default
            string diseaseSelect = string.Empty;
            if (parameterValues.dataElements.Length > 0)
            {
                //aggregateWithDisease = " Disease, ";
                aggregateWithDisease = " groupName, ";
                diseaseSelect = " groupName as Disease, ";
            }
            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;
            bool lastMonth = false;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection, lastMonth);

            string cmdText =
            "   select " + ipdOpdColumn + diseaseSelect + genderColumn + ageColumn +
            aggregationOptions + aggregationIdsRows +
            //"   FiscalYear, " + 
            periodListing +
            "   sum(value) " + valueType + "   " + initialListing + "  " +
            " from v_EthEhmis_HmisValue \n" +
            //"   inner join DiseaseDictionary \n" +
            "   inner join " + diseaseTableName + " \n" +
            //"   on DiseaseDictionary.dataEleClass = v_EthEhmis_HmisValue.dataclass \n" +
             "   on " + diseaseTableName + ".dataEleClass = v_EthEhmis_HmisValue.dataclass \n" +
            "   inner join " + FacilityTable + " \n" +
            //"   on v_EthEhmis_HmisValue.LocationId = v_EthEhmis_AllFacilityWithId.hmisCode \n" +
            "   on v_EthEhmis_HmisValue.LocationId = " + FacilityTable + ".hmisCode \n" +
            "   and \n" +
            //"   DiseaseDictionary.LabelId = v_EthEhmis_HmisValue.LabelId \n" +
            diseaseTableName + ".LabelId = v_EthEhmis_HmisValue.LabelId \n" +
            innerJoinPopulation + // Case per 1000 population inner join   
            "   where " + dataClass + " \n " +
            //"   and fiscalYear = 2008 " 
            periodWhereSelection + ageWhereQuery + genderWhereQuery + facilityWhereQuery
            + diseaseQuery + institutionWhere + " \n " +
            "   group by " + ipdGroupBy + " \n " +
                aggregationGroupBy + aggregationIdsRows + " \n " +
                aggregateWithDisease + genderColumn + ageColumn +
            //" FiscalYear " + " \n " +
            periodGroupByListing + " \n " +
            "   order by " + aggregationGroupBy +
            //" FiscalYear ";
            periodGroupByListing;

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdText);

            toExecute.CommandTimeout = 0; //300 // = 1000000;

            DataTable reportDataTable = _helper.GetDataSet(toExecute).Tables[0];

            //string[] columnNames = reportDataTable.Columns.Cast<DataColumn>()
            //                .Select(x => x.ColumnName)
            //                .ToArray();

            //stringAndDataTable.Add(columnNames);
            //stringAndDataTable.Add(reportDataTable);
            //return stringAndDataTable;
            return reportDataTable;
        }

        private string getDiseaseTableName()
        {
            string diseaseTable = "[aaTempDiseaseDictionaryRoot]";
            // First delete and insert or re-create the table if it doesn't exist...

            return diseaseTable;
        }

        private void prepareDiseaseTable(AnalyticsParameters parameterValues, string diseaseList, string diseaseTable, string updateGroupName)
        {
            // First delete and insert or re-create the table if it doesn't exist...
            DBConnHelper _helper = new DBConnHelper();
            string cmdDelete = " delete from [aaTempDiseaseDictionaryRoot]";
            string cmdInsert = string.Empty;

            if (diseaseList == "")
            {
                cmdInsert = " insert into [aaTempDiseaseDictionaryRoot] " +
                              " select id, sno, labelId, dataEleClass, descrip, classAndLabel, " +
                              " gender, age, disease, disease from " + DiseaseDictionaryTable;
            }
            else
            {

                cmdInsert = " insert into [aaTempDiseaseDictionaryRoot] " +
                               " select id, sno, labelId, dataEleClass, descrip, classAndLabel, " +
                               " gender, age, disease, disease from " + DiseaseDictionaryTable +
                               " where " + diseaseList;
            }

            string cmdUpdate = updateGroupName;

            SqlCommand toExecute;
            toExecute = new SqlCommand(cmdDelete);
            toExecute.CommandTimeout = 0; //300 // = 1000000;

            _helper.Execute(cmdDelete);
            _helper.Execute(cmdInsert);
            if (cmdUpdate != "")
            {
                _helper.Execute(cmdUpdate);
            }

        }

        private void processCasePerPopulation(AnalyticsParameters parameterValues, out string initialListing, out string innerJoinPopulation)
        {
            initialListing = string.Empty;
            innerJoinPopulation = string.Empty;

            if ((parameterValues.diseaseOptions.per1000Population == true) && (parameterValues.institutionAggrOptions.aggrType != "facility"))
            {
                string perPopulationText = "CasesPer";
                if (parameterValues.diseaseOptions.caseDeath == "mortality")
                {
                    perPopulationText = "DeathsPer";
                }

                initialListing = " , max([population]) as Population, \n" +
                                    " cast((((cast(sum(value) as decimal(18, 4))) * " + populationMultiplier + " ) " +
                                    "  / (cast(max([population]) as decimal(18, 4)))) as decimal(18,4)) \n" +
                                    " as " + perPopulationText + populationMultiplier + "Population \n ";

                innerJoinPopulation = " inner join aaPopulationDenominator on  \n ";

                if (parameterValues.institutionAggrOptions.aggrType == "federal")
                {
                    innerJoinPopulation += " aaPopulationDenominator.LocationId = 88 and  \n ";
                }
                else if (parameterValues.institutionAggrOptions.aggrType == "region")
                {
                    innerJoinPopulation += " aaPopulationDenominator.LocationId = " + FacilityTable + ".RegionId and  \n ";
                }
                else if (parameterValues.institutionAggrOptions.aggrType == "zone")
                {
                    string cmdText = "select distinct regionId from aaPopulationDenominator where facilityTypeId = 9";
                    SqlCommand toExecute = new SqlCommand(cmdText);
                    DBConnHelper _helper = new DBConnHelper();
                    DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
                    ArrayList regionIdsWithZone = new ArrayList();

                    foreach (DataRow row in dt.Rows)
                    {
                        regionIdsWithZone.Add(Convert.ToInt16(row["regionId"]));
                    }

                    // Check if any region is selected
                    if (parameterValues.institutionAggrOptions.hmiscode != "")
                    {
                        // check if the region
                        if (!regionIdsWithZone.Contains(parameterValues.institutionAggrOptions.hmiscode))
                        {
                            parameterValues.institutionAggrOptions.aggrType = "woreda";
                            innerJoinPopulation += " aaPopulationDenominator.LocationId = " + FacilityTable + ".WoredaId and  \n ";
                        }
                        else
                        {
                            innerJoinPopulation += " aaPopulationDenominator.LocationId = " + FacilityTable + ".ZoneId and  \n ";
                        }
                    }
                    else

                    {
                        innerJoinPopulation += " aaPopulationDenominator.LocationId = " + FacilityTable + ".ZoneId and  \n ";
                    }
                }
                else if (parameterValues.institutionAggrOptions.aggrType == "district")
                {
                    innerJoinPopulation += " aaPopulationDenominator.LocationId = " + FacilityTable + ".WoredaId and  \n ";
                }

                innerJoinPopulation += " v_EthEhmis_HmisValue.fiscalYear = aaPopulationDenominator.Year \n";
            }
        }

        private void processAgeGenderDepartment(AnalyticsParameters parameterValues, out bool showAge_Column, out string ageQuery,
                out bool showGender_column, out string genderQuery, out bool showIPD_OPD_Column,
                out string departmentQuery, out string dataClass, out string genderColumn, out string ageColumn)
        {
            showIPD_OPD_Column = false;
            showAge_Column = false;
            showGender_column = false;
            genderColumn = string.Empty;
            ageColumn = string.Empty;
            dataClass = string.Empty;

            ageQuery = string.Empty;
            genderQuery = string.Empty;
            departmentQuery = string.Empty;

            string genderMale = languageHash["male"].ToString();
            string genderFemale = languageHash["female"].ToString();
            string underFive = languageHash["underfive"].ToString();
            string fivetofourteen = languageHash["fivetofourteen"].ToString();
            string abovefourteen = languageHash["abovefourteen"].ToString();
            
            if ((parameterValues.diseaseOptions.male == false) &&
                 (parameterValues.diseaseOptions.female == false))
            {
                showGender_column = false;
            }
            else if ((parameterValues.diseaseOptions.male == true) &&
                 (parameterValues.diseaseOptions.female == true))
            {
                showGender_column = true;
                genderQuery = " and ((gender = N'" + genderMale + "') or (gender = N'" + genderFemale + "'))";
            }
            else if ((parameterValues.diseaseOptions.male == false) &&
               (parameterValues.diseaseOptions.female == true))
            {
                showGender_column = true;
                genderQuery = " and (gender = N'" + genderFemale + "') ";
            }
            else if ((parameterValues.diseaseOptions.male == true) &&
               (parameterValues.diseaseOptions.female == false))
            {
                showGender_column = true;
                genderQuery = " and (gender = N'" + genderMale + "') ";
            }

            if (showGender_column)
            {
                genderColumn = " Gender, ";
            }

            if ((parameterValues.diseaseOptions.zeroToFour == false) &&
                 (parameterValues.diseaseOptions.fiveToFourteen == false) &&
                 (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                showAge_Column = false;
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
               (parameterValues.diseaseOptions.fiveToFourteen == true) &&
               (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and ((age = N'" + underFive + " ') OR " +
                           " (age = N'" + fivetofourteen + "') OR " +
                           " (age = N'" + abovefourteen + "')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
               (parameterValues.diseaseOptions.fiveToFourteen == true) &&
               (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                //Under_5
                //5_14
                //Above_14
                showAge_Column = true;
                ageQuery = " and ((age = N'" + underFive + " ') OR " +
                           " (age = N'" + fivetofourteen + "'))";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
               (parameterValues.diseaseOptions.fiveToFourteen == false) &&
               (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and ((age = N'" + underFive + "') OR " +
                           " (age = N'" + abovefourteen + "')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == false) &&
                (parameterValues.diseaseOptions.fiveToFourteen == true) &&
                (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and ((age = N'" + fivetofourteen + "') OR (age = N'" + abovefourteen + "')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == false) &&
              (parameterValues.diseaseOptions.fiveToFourteen == false) &&
              (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and (age = N'" + abovefourteen + "') ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
            (parameterValues.diseaseOptions.fiveToFourteen == false) &&
            (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                //Under_5
                //5_14
                //Above_14
                showAge_Column = true;
                ageQuery = " and (age = N'" + underFive + "') ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == false) &&
           (parameterValues.diseaseOptions.fiveToFourteen == true) &&
           (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                //Under_5
                //5_14
                //Above_14
                showAge_Column = true;
                ageQuery = " and (age = N'" + fivetofourteen + "') ";
            }

            if (showAge_Column)
            {
                ageColumn = " Age, ";
            }
        }

        private void processingPeriod(AnalyticsParameters parameterValues, out string periodListing,
            out string periodGroupByListing, out string periodWhereSelection, bool lastMonth)
        {
            periodListing = string.Empty;
            periodGroupByListing = string.Empty;
            periodWhereSelection = string.Empty;

            if (parameterValues.PeriodRange == "PeriodRangeMonthly")
            {
                periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                              " and (FiscalMonth >= " + parameterValues.periodSelect.StartMonth +
                              " and FiscalMonth <= " + parameterValues.periodSelect.EndMonth + "))";

                if ((parameterValues.periodSelect.aggregate == true) &&
                    (parameterValues.periodSelect.StartMonth != parameterValues.periodSelect.EndMonth))
                {
                    if (lastMonth)
                    {
                        periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                             " and (FiscalMonth = " + parameterValues.periodSelect.EndMonth + "))";
                    }
                    string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                    periodListing = " FiscalYear, " + "'" + monthRange + "'" + " as FiscalMonth, ";
                    periodGroupByListing = " FiscalYear ";
                }
                else if ((parameterValues.periodSelect.aggregateAll == true) &&
                   (parameterValues.periodSelect.StartMonth != parameterValues.periodSelect.EndMonth))
                {
                    if (lastMonth)
                    {
                        periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                             " and (FiscalMonth = " + parameterValues.periodSelect.EndMonth + "))";
                    }
                    string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                    periodListing = " FiscalYear, " + "'" + monthRange + "'" + " as FiscalMonth, ";
                    periodGroupByListing = " FiscalYear ";
                }
                else
                {
                    periodListing = " FiscalYear, FiscalMonth, ";
                    periodGroupByListing = " FiscalYear, FiscalMonth ";
                }
            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearly")
            {
                periodListing = " FiscalYear,  ";
                periodGroupByListing = " FiscalYear ";
                periodWhereSelection = " and (FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                       " and FiscalYear <= " + parameterValues.periodSelect.EndFiscalYear + ")";
            }
            else if (parameterValues.PeriodRange == "PeriodRangeQuarterly")
            {
                periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                          " and (Quarter >= " + parameterValues.periodSelect.StartQuarter +
                          " and Quarter <= " + parameterValues.periodSelect.EndQuarter + "))";

                if ((parameterValues.periodSelect.aggregate == true)
                     && (parameterValues.periodSelect.StartQuarter != parameterValues.periodSelect.EndQuarter))
                {
                    if (lastMonth)
                    {
                        periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                             " and (Quarter = " + parameterValues.periodSelect.EndQuarter + "))";
                    }

                    string quarterRange = "Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter;
                    periodListing = " FiscalYear, " + "'" + quarterRange + "'" + " as Quarter, ";
                    periodGroupByListing = " FiscalYear ";
                }
                else if ((parameterValues.periodSelect.aggregateAll == true)
                    && (parameterValues.periodSelect.StartQuarter != parameterValues.periodSelect.EndQuarter))
                {
                    if (lastMonth)
                    {
                        periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                             " and (Quarter = " + parameterValues.periodSelect.EndQuarter + "))";
                    }

                    string quarterRange = "Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter;
                    periodListing = " FiscalYear, " + "'" + quarterRange + "'" + " as Quarter, ";
                    periodGroupByListing = " FiscalYear ";
                }
                else
                {
                    periodListing = " FiscalYear, Quarter, ";
                    periodGroupByListing = " FiscalYear, Quarter ";
                }
            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearMonthly")
            {
                periodWhereSelection = " and ( " +
                    "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                        " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                        " and FiscalMonth >= " + parameterValues.periodSelect.StartYearStartMonth +
                                       " and FiscalMonth <= " + parameterValues.periodSelect.StartYearEndMonth + ")) " +

                     " OR " +

                     "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " and FiscalMonth >= " + parameterValues.periodSelect.EndYearStartMonth +
                                       " and FiscalMonth <= " + parameterValues.periodSelect.EndYearEndMonth +
                      "))";

                if (parameterValues.periodSelect.aggregate == true)
                {
                    if (lastMonth == true)
                    {
                        periodWhereSelection = " and ( " +
                                           "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                               " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                                               " and FiscalMonth >= " + parameterValues.periodSelect.StartYearEndMonth +
                                                              " and FiscalMonth <= " + parameterValues.periodSelect.StartYearEndMonth + ")) " +

                                            " OR " +

                                            "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                               " and FiscalMonth >= " + parameterValues.periodSelect.EndYearEndMonth +
                                                              " and FiscalMonth <= " + parameterValues.periodSelect.EndYearEndMonth +
                                             "))";
                    }

                    int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                    string monthRange =
                        " case   \n" +
                        " when FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                          endFiscalYearMinusOne + " then " + "'Month:" +
                          parameterValues.periodSelect.StartYearStartMonth + "-" +
                          parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                        " when FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " then " + "'Month:" +
                          parameterValues.periodSelect.EndYearStartMonth + "-" +
                          parameterValues.periodSelect.EndYearEndMonth + "'" + " end as FiscalMonth, ";

                    periodListing = " FiscalYear, " + monthRange;
                    periodGroupByListing = " FiscalYear ";
                }
                else if (parameterValues.periodSelect.aggregateAll == true)
                {
                    if (lastMonth == true)
                    {
                        periodWhereSelection = " and ( " +
                                           "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                               " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                                               " and FiscalMonth >= " + parameterValues.periodSelect.StartYearEndMonth +
                                                              " and FiscalMonth <= " + parameterValues.periodSelect.StartYearEndMonth + ")) " +

                                            " OR " +

                                            "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                               " and FiscalMonth >= " + parameterValues.periodSelect.EndYearEndMonth +
                                                              " and FiscalMonth <= " + parameterValues.periodSelect.EndYearEndMonth +
                                             "))";
                    }

                    int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                    string monthRange =
                        " case   \n" +
                        " when FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                          endFiscalYearMinusOne + " then " + "'Month:" +
                          parameterValues.periodSelect.StartYearStartMonth + "-" +
                          parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                        " when FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " then " + "'Month:" +
                          parameterValues.periodSelect.EndYearStartMonth + "-" +
                          parameterValues.periodSelect.EndYearEndMonth + "'" + " end as FiscalMonth, ";

                    periodListing = " FiscalYear, " + monthRange;
                    periodGroupByListing = " FiscalYear ";
                }
                else
                {
                    periodListing = " FiscalYear, FiscalMonth,  ";
                    periodGroupByListing = " FiscalYear, FiscalMonth ";
                }
            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
            {
                periodWhereSelection = " and ( " +
                    "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                        " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                        " and Quarter >= " + parameterValues.periodSelect.StartYearStartQuarter +
                                       " and Quarter <= " + parameterValues.periodSelect.StartYearEndQuarter + " )) " +
                     " OR " +
                     "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " and Quarter >= " + parameterValues.periodSelect.EndYearStartQuarter +
                                       " and Quarter <= " + parameterValues.periodSelect.EndYearEndQuarter +
                      "))";

                if (parameterValues.periodSelect.aggregate == true)
                {
                    if (lastMonth == true)
                    {
                        periodWhereSelection = " and ( " +
                                           "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                               " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                                               " and Quarter >= " + parameterValues.periodSelect.StartYearEndQuarter +
                                                              " and Quarter <= " + parameterValues.periodSelect.StartYearEndQuarter + " )) " +
                                            " OR " +
                                            "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                                               " and Quarter >= " + parameterValues.periodSelect.EndYearEndQuarter +
                                                              " and Quarter <= " + parameterValues.periodSelect.EndYearEndQuarter +
                                             "))";
                    }

                    int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                    string quarterRange =
                        " case   \n" +
                        " when FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                          endFiscalYearMinusOne + " then " + "'Quarter:" +
                          parameterValues.periodSelect.StartYearStartQuarter + "-" +
                          parameterValues.periodSelect.StartYearEndQuarter + "'" + "\n" +
                        " when FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " then " + "'Quarter:" +
                          parameterValues.periodSelect.EndYearStartQuarter + "-" +
                          parameterValues.periodSelect.EndYearEndQuarter + "'" + " end as Quarter, ";

                    periodListing = " FiscalYear, " + quarterRange;
                    periodGroupByListing = " FiscalYear ";
                }
                else
                {
                    periodListing = " FiscalYear, Quarter,  ";
                    periodGroupByListing = " FiscalYear, Quarter ";
                }
            }
        }

        // POST: api/Analytics
        public DataTable Post([FromBody]CategoryParameters value)
        {            

            JObject obj = (Newtonsoft.Json.Linq.JObject)value.categoryList;
            List<string> categories = new List<string>();
            List<string> mainCategory = new List<string>();
            List<string> category1 = new List<string>();
            List<string> category2 = new List<string>();
            List<string> category3 = new List<string>();
            List<string> category4 = new List<string>();
            List<string> category5 = new List<string>();

            DataTable dt = new DataTable();

            // If main categories are clicked...don't worry about the lower...for example if service is clicked...

            foreach (var jObj in obj)
            {
                string category = jObj.Key;
                if (Convert.ToBoolean(jObj.Value) == true)
                {
                    categories.Add(category);
                }
                //string categoryQuery = "";
            }

            string cat1 = string.Empty;
            string cat2 = string.Empty;
            string cat3 = string.Empty;
            string cat4 = string.Empty;
            string cat5 = string.Empty;
            string catList = string.Empty;

            if (categories.Count != 0)
            {
                if (value.reportType == "service")
                {
                    //if ((categories.Count == 1) && (categories[0].ToString() == "service"))
                    if ((categories[0].ToString() == "service"))
                    {
                        string parameterSql = "select LabelId, FullDescription, 'False' as Checked, '' as groupName, Category1 " +
                       " from  " + ServiceDictionaryTable + "  where (labelId is not null or labelId != '') " +
                       " order by sequenceNo ";

                        DBConnHelper _helper = new DBConnHelper();
                        SqlCommand parameterCmd = new SqlCommand(parameterSql);
                        dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    }
                    else
                    {

                        cat1 = " (category1 in (";
                        cat2 = " (category2 in (";
                        cat3 = " (category3 in (";
                        cat4 = " (category4 in (";
                        cat5 = " (category5 in (";
                        int count = 0;
                        foreach (string cat in categories)
                        {
                            count++;
                            if (count == categories.Count)
                            {
                                // last one so no need for ,
                                catList += "'" + cat + "'";
                            }
                            else
                            {
                                catList += "'" + cat + "',";
                            }
                        }
                        cat1 += catList;
                        cat1 += "))";
                        cat2 += catList;
                        cat2 += "))";
                        cat3 += catList;
                        cat3 += "))";
                        cat4 += catList;
                        cat4 += "))";
                        cat5 += catList;
                        cat5 += "))";

                        string parameterSql = "select LabelId, FullDescription, 'False' as Checked, '' as groupName, Category1 " +
                       " from " + ServiceDictionaryTable + "  where (labelId is not null or labelId != '') and (" + cat1 + " or " + cat2 +
                       " or " + cat3 + ") order by sequenceNo ";

                        DBConnHelper _helper = new DBConnHelper();
                        SqlCommand parameterCmd = new SqlCommand(parameterSql);
                        dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    }
                }
                else if (value.reportType == "indicator")
                {
                    //if ((categories.Count == 1) && (categories[0].ToString() == "indicator"))
                    if ((categories[0].ToString() == "indicator"))
                    {
                        string parameterSql = "  select SequenceNo, 'False' as Checked, '' as groupName, IndicatorName, " +
                                              "  NumeratorName, DenominatorName, " +
                                              "  NumeratorLabelId, DenominatorLabelId, NumeratorDataEleClass, DenominatorDataEleClass, " +
                                             "  Category1 from EthEhmis_IndicatorsNewDisplay where ReadOnly = 0 ";

                        DBConnHelper _helper = new DBConnHelper();
                        SqlCommand parameterCmd = new SqlCommand(parameterSql);
                        dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    }
                    else
                    {

                        cat1 = " (category1 in (";
                        cat2 = " (category2 in (";
                        cat3 = " (category3 in (";
                        cat4 = " (category4 in (";
                        cat5 = " (category5 in (";
                        int count = 0;
                        foreach (string cat in categories)
                        {
                            count++;
                            if (count == categories.Count)
                            {
                                // last one so no need for ,
                                catList += "'" + cat + "'";
                            }
                            else
                            {
                                catList += "'" + cat + "',";
                            }
                        }
                        cat1 += catList;
                        cat1 += "))";
                        cat2 += catList;
                        cat2 += "))";
                        cat3 += catList;
                        cat3 += "))";
                        cat4 += catList;
                        cat4 += "))";
                        cat5 += catList;
                        cat5 += "))";

                        string parameterSql = "  select SequenceNo, 'False' as Checked, '' as groupName, IndicatorName, " +
                                              "  NumeratorName, DenominatorName, " +
                                              "  NumeratorLabelId, DenominatorLabelId, NumeratorDataEleClass, DenominatorDataEleClass, " +
                                              "  Category1 from EthEhmis_IndicatorsNewDisplay where ReadOnly = 0 " +
                                              "  and (" + cat1 + ") order by sequenceNo ";

                        DBConnHelper _helper = new DBConnHelper();
                        SqlCommand parameterCmd = new SqlCommand(parameterSql);
                        dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    }
                }
            }

            return dt;
        }

        [EnableCors("*", "*", "*")]
        private DataTable indicatorCalc(AnalyticsParameters parametersValue)
        {

            DBConnHelper _helper = new DBConnHelper();
            string facilityQuery = "";

            if (parametersValue.institutionAggrOptions.aggrType == "region")
            {
                facilityQuery =
                " select * from  " +
                                   FacilityTable + " where facilityTypeid in (10) " +
                                   " order by RegionId, ZoneId, WoredaId, FacilityTypeId ";
            }
            else if (parametersValue.institutionAggrOptions.aggrType == "zone")
            {
                facilityQuery =
                " select * from  " +
                                   FacilityTable + " where facilityTypeid in (9) " +
                                   " order by RegionId, ZoneId, WoredaId, FacilityTypeId ";
            }
            else if (parametersValue.institutionAggrOptions.aggrType == "district")
            {
                facilityQuery =
                " select * from  " +
                                   FacilityTable + " where facilityTypeid in (8) " +
                                   " order by RegionId, ZoneId, WoredaId, FacilityTypeId ";
            }

            // Get Last month values correctly
            ServiceDictionaryTable = "EthioHIMS_ServiceDataElementsNew";
            Hashtable lastMonthLabelIds = new Hashtable();

            string lastMonthCmdTxt = " select labelId from " + ServiceDictionaryTable +
                                     " where aggregationType = 1";
            SqlCommand lastMonthCmd = new SqlCommand(lastMonthCmdTxt);
            DataTable dtLastMonth = _helper.GetDataSet(lastMonthCmd).Tables[0];
            foreach (DataRow row in dtLastMonth.Rows)
            {
                string labelId = row["labelId"].ToString();
                lastMonthLabelIds[labelId] = 1;
            }

            string loc = string.Empty;
            Hashtable locWoredaName = new Hashtable();
            Hashtable locZoneName = new Hashtable();
            Hashtable locRegionName = new Hashtable();

            Hashtable locWoredaId = new Hashtable();
            Hashtable locZoneId = new Hashtable();
            Hashtable locRegionId = new Hashtable();

            Hashtable locFiscalMonth = new Hashtable();
            Hashtable locQuarter = new Hashtable();
            Hashtable locFiscalYear = new Hashtable();
            Hashtable locRange = new Hashtable();
            Hashtable yearDenominator = new Hashtable();
            Hashtable locYearDenominator = new Hashtable();

            StringBuilder locIndex = new StringBuilder();

            List<object> listObj = new List<object>();
            DataTable dt = null;

            bool yearly = false;

            if (parametersValue.institutionAggrOptions.aggrType != "federal")
            {
                SqlCommand facilityCmd = new SqlCommand(facilityQuery);

                dt = _helper.GetDataSet(facilityCmd).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    string regionName = row["ReportingRegionName"].ToString();
                    string zoneName = row["ZoneName"].ToString();
                    string woredaName = row["WoredaName"].ToString();
                    string regionId = row["RegionId"].ToString();
                    string zoneId = row["ZoneId"].ToString();
                    string woredaId = row["WoredaId"].ToString();
                    locIndex = new StringBuilder();


                    if (parametersValue.institutionAggrOptions.aggrType == "region")
                    {
                        locIndex.Append(regionId);
                    }
                    else if (parametersValue.institutionAggrOptions.aggrType == "zone")
                    {
                        locIndex.Append(regionId); locIndex.Append("_"); locIndex.Append(zoneId);
                    }
                    else if (parametersValue.institutionAggrOptions.aggrType == "district")
                    {
                        locIndex.Append(regionId); locIndex.Append("_"); locIndex.Append(zoneId);
                        locIndex.Append("_"); locIndex.Append(woredaId);
                    }


                    string ind = locIndex.ToString();
                    locWoredaName[ind] = woredaName;
                    locZoneName[ind] = zoneName;
                    locRegionName[ind] = regionName;

                    locWoredaId[ind] = woredaId;
                    locZoneId[ind] = zoneId;
                    locRegionId[ind] = regionId;

                }
            }

            // Denominator Multiplication...
            decimal denomMultiply = 1;
            decimal numMain = Convert.ToDecimal(1.00);
            decimal denomMain = Convert.ToDecimal(12.00);
            if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
            {
                if ((parametersValue.periodSelect.aggregate == true) || (parametersValue.periodSelect.aggregateAll == true))
                {
                    if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                    {
                        int numMonths = (parametersValue.periodSelect.EndMonth - parametersValue.periodSelect.StartMonth) + 1;
                        numMain = Convert.ToDecimal(numMonths);
                    }
                    else if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                    {
                        int numMonthsStart = (parametersValue.periodSelect.StartYearEndMonth - parametersValue.periodSelect.StartYearStartMonth) + 1;
                        decimal numMainStart = Convert.ToDecimal(numMonthsStart);
                        decimal denomMultiplyStart = numMainStart / denomMain;

                        for (int i = parametersValue.periodSelect.StartFiscalYear;
                            i < parametersValue.periodSelect.EndFiscalYear; i++)
                        {
                            yearDenominator[i] = denomMultiplyStart;
                        }

                        int numMonthsEnd = (parametersValue.periodSelect.EndYearEndMonth - parametersValue.periodSelect.EndYearStartMonth) + 1;
                        decimal numMainEnd = Convert.ToDecimal(numMonthsEnd);
                        decimal denomMultiplyEnd = numMainEnd / denomMain;

                        yearDenominator[parametersValue.periodSelect.EndFiscalYear] = denomMultiplyEnd;
                    }
                }

                if (parametersValue.periodSelect.aggregateAll == true)
                {
                    //decimal count = (parametersValue.periodSelect.EndFiscalYear - parametersValue.periodSelect.StartFiscalYear) + 1;

                    //decimal denomDivider = 1 / count;
                    denomMultiply = 1;
                }
                else
                {
                    denomMultiply = numMain / denomMain;
                }
            }

            if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
            {
                numMain = Convert.ToDecimal(3.00);

                if (parametersValue.periodSelect.aggregate == true)
                {
                    if (parametersValue.PeriodRange == "PeriodRangeQuarterly")
                    {
                        int numQuarters = (parametersValue.periodSelect.EndQuarter - parametersValue.periodSelect.StartQuarter) + 1;
                        numMain = Convert.ToDecimal(numQuarters * 3);
                    }
                    else if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                    {
                        int numQuarterStart = (parametersValue.periodSelect.StartYearEndQuarter -
                            parametersValue.periodSelect.StartYearStartQuarter) + 1;
                        decimal numMainStart = Convert.ToDecimal(numQuarterStart * 3);
                        decimal denomMultiplyStart = numMainStart / denomMain;

                        for (int i = parametersValue.periodSelect.StartFiscalYear;
                            i < parametersValue.periodSelect.EndFiscalYear; i++)
                        {
                            yearDenominator[i] = denomMultiplyStart;
                        }

                        int numQuarterEnd = (parametersValue.periodSelect.EndYearEndQuarter -
                            parametersValue.periodSelect.EndYearStartQuarter) + 1;
                        decimal numMainEnd = Convert.ToDecimal(numQuarterEnd * 3);
                        decimal denomMultiplyEnd = numMainEnd / denomMain;

                        yearDenominator[parametersValue.periodSelect.EndFiscalYear] = denomMultiplyEnd;
                    }
                }

                denomMultiply = numMain / denomMain;
            }

            if (parametersValue.PeriodRange == "PeriodRangeYearly")
            {
                yearly = true;
            }


            string institutionWhere = string.Empty;

            string parameterSql = processFacilityIndicator(parametersValue, out institutionWhere);

            _helper = new DBConnHelper();
            SqlCommand parameterCmd = new SqlCommand(parameterSql);
            dt = _helper.GetDataSet(parameterCmd).Tables[0];
            indicatorLocationIdValueHash = new Hashtable();
            Hashtable selectedLocations = new Hashtable();

            StringBuilder index = new StringBuilder();
            loc = string.Empty;
            int FiscalYear = 0;
            foreach (DataRow row in dt.Rows)
            {
                //RegionId, ZoneId, WoredaId, DataClass, LabelId, sum(value) as Value

                index = new StringBuilder();

                if (parametersValue.institutionAggrOptions.aggrType != "federal")
                {
                    if (parametersValue.institutionAggrOptions.aggrType == "region")
                    {
                        string regionId = row["RegionId"].ToString();
                        //int FiscalMonth = 0, Quarter = 0;
                        string FiscalMonth = string.Empty;
                        string Quarter = string.Empty;
                        string range = string.Empty;

                        index.Append(regionId);
                        string regionName = locRegionName[index.ToString()].ToString();
                        index.Append("_");

                        if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                        {
                            if (parametersValue.periodSelect.aggregateAll == true)
                            {
                                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                                //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                                range = row["Range"].ToString();
                                index.Append(range);
                                loc = index.ToString();
                                locRange[loc] = range;
                            }
                            else
                            {
                                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                                //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                                FiscalMonth = row["FiscalMonth"].ToString();
                                index.Append(FiscalYear);
                                index.Append("_");
                                index.Append(FiscalMonth);
                                loc = index.ToString();

                                if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                {
                                    locYearDenominator[loc] = yearDenominator[FiscalYear];
                                }
                            }
                        }
                        else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            //Quarter = Convert.ToInt32(row["Quarter"].ToString());
                            Quarter = row["Quarter"].ToString();
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(Quarter);
                            loc = index.ToString();

                            if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                            {
                                locYearDenominator[loc] = yearDenominator[FiscalYear];
                            }
                        }
                        else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            index.Append(FiscalYear);
                            loc = index.ToString();
                        }


                        locRegionName[loc] = regionName;
                        locRegionId[loc] = regionId;
                        locFiscalMonth[loc] = FiscalMonth;
                        locFiscalYear[loc] = FiscalYear;
                        locQuarter[loc] = Quarter;

                        selectedLocations[loc] = 1;

                        index.Append("_");
                    }
                    else if (parametersValue.institutionAggrOptions.aggrType == "zone")
                    {
                        string regionId = row["RegionId"].ToString();
                        string zoneId = row["ZoneId"].ToString();

                        index.Append(regionId); index.Append("_"); index.Append(zoneId);
                        string ind = index.ToString();
                        string regionName = locRegionName[ind].ToString();
                        string zoneName = locZoneName[ind].ToString();

                        index.Append("_");

                        //int FiscalMonth = 0, Quarter = 0;
                        string FiscalMonth = string.Empty;
                        string Quarter = string.Empty;
                        string range = string.Empty;

                        if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                        {
                            if (parametersValue.periodSelect.aggregateAll == true)
                            {
                                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                                //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                                range = row["Range"].ToString();
                                index.Append(range);
                                loc = index.ToString();
                                locRange[loc] = range;
                            }
                            else
                            {
                                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                                //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                                FiscalMonth = row["FiscalMonth"].ToString();
                                index.Append(FiscalYear);
                                index.Append("_");
                                index.Append(FiscalMonth);
                                loc = index.ToString();

                                if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                {
                                    locYearDenominator[loc] = yearDenominator[FiscalYear];
                                }
                            }
                        }
                        else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            //Quarter = Convert.ToInt32(row["Quarter"].ToString());
                            Quarter = row["Quarter"].ToString();
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(Quarter);
                            loc = index.ToString();

                            if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                            {
                                locYearDenominator[loc] = yearDenominator[FiscalYear];
                            }
                        }
                        else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            index.Append(FiscalYear);
                            loc = index.ToString();
                        }


                        locRegionName[loc] = regionName;
                        locZoneName[loc] = zoneName;
                        locRegionId[loc] = regionId;
                        locZoneId[loc] = zoneId;
                        locFiscalMonth[loc] = FiscalMonth;
                        locFiscalYear[loc] = FiscalYear;
                        locQuarter[loc] = Quarter;

                        selectedLocations[loc] = 1;
                        index.Append("_");
                    }
                    else if (parametersValue.institutionAggrOptions.aggrType == "district")
                    {
                        string regionId = row["RegionId"].ToString();
                        string zoneId = row["ZoneId"].ToString();
                        string woredaId = row["WoredaId"].ToString();

                        index.Append(regionId); index.Append("_"); index.Append(zoneId); index.Append("_");
                        index.Append(woredaId);
                        string ind = index.ToString();

                        string regionName = locRegionName[ind].ToString();
                        string zoneName = locZoneName[ind].ToString();
                        string woredaName = locWoredaName[ind].ToString();

                        index.Append("_");

                        //int FiscalMonth = 0, Quarter = 0;
                        string FiscalMonth = string.Empty;
                        string Quarter = string.Empty;
                        string range = string.Empty;

                        if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                        {
                            if (parametersValue.periodSelect.aggregateAll == true)
                            {
                                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                                //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                                range = row["Range"].ToString();
                                index.Append(range);
                                loc = index.ToString();
                                locRange[loc] = range;
                            }
                            else
                            {
                                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                                //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                                FiscalMonth = row["FiscalMonth"].ToString();
                                index.Append(FiscalYear);
                                index.Append("_");
                                index.Append(FiscalMonth);
                                loc = index.ToString();

                                if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                {
                                    locYearDenominator[loc] = yearDenominator[FiscalYear];
                                }
                            }

                        }
                        else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            // FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            //Quarter = Convert.ToInt32(row["Quarter"].ToString());
                            Quarter = row["Quarter"].ToString();
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(Quarter);
                            loc = index.ToString();

                            if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                            {
                                locYearDenominator[loc] = yearDenominator[FiscalYear];
                            }
                        }
                        else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            index.Append(FiscalYear);
                            loc = index.ToString();
                        }

                        locRegionName[loc] = regionName;
                        locZoneName[loc] = zoneName;
                        locWoredaName[loc] = woredaName;
                        locRegionId[loc] = regionId;
                        locZoneId[loc] = zoneId;
                        locWoredaId[loc] = woredaId;

                        locFiscalMonth[loc] = FiscalMonth;
                        locFiscalYear[loc] = FiscalYear;
                        locQuarter[loc] = Quarter;

                        selectedLocations[loc] = 1;
                        index.Append("_");
                    }
                }
                else
                {
                    //int FiscalMonth = 0, Quarter = 0;
                    string FiscalMonth = string.Empty;
                    string Quarter = string.Empty;
                    string range = string.Empty;

                    if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                    {
                        if (parametersValue.periodSelect.aggregateAll == true)
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            range = row["Range"].ToString();
                            index.Append(range);
                            loc = index.ToString();
                            locRange[loc] = range;
                        }
                        else
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            FiscalMonth = row["FiscalMonth"].ToString();
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(FiscalMonth);
                            loc = index.ToString();

                            if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                            {
                                locYearDenominator[loc] = yearDenominator[FiscalYear];
                            }
                        }
                    }
                    else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                    {
                        FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                        //FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                        // Quarter = Convert.ToInt32(row["Quarter"].ToString());
                        Quarter = row["Quarter"].ToString();
                        index.Append(FiscalYear);
                        index.Append("_");
                        index.Append(Quarter);
                        loc = index.ToString();

                        if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                        {
                            locYearDenominator[loc] = yearDenominator[FiscalYear];
                        }
                    }
                    else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                    {
                        FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                        index.Append(FiscalYear);
                        loc = index.ToString();
                    }

                    locFiscalMonth[loc] = FiscalMonth;
                    locFiscalYear[loc] = FiscalYear;
                    locQuarter[loc] = Quarter;

                    selectedLocations[loc] = 1;
                    index.Append("_");
                }

                string dataClass = row["DataClass"].ToString();
                string labelId = row["LabelId"].ToString();
                string value = row["Value"].ToString();
                FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());

                index.Append(dataClass); index.Append("_"); index.Append(labelId);

                loc = index.ToString();

                if (indicatorLocationIdValueHash[loc] == null)
                {
                    if ((dataClass == "4") && (parametersValue.periodSelect.aggregateAll))
                    {
                        decimal newValue = Convert.ToDecimal(value);
                        newValue = newValue * Convert.ToDecimal(yearDenominator[FiscalYear]);
                        newValue = decimal.Round(newValue, 0);
                        indicatorLocationIdValueHash[loc] = Convert.ToInt64(newValue);
                    }
                    else
                    {
                        indicatorLocationIdValueHash[loc] = value;
                    }
                }
                else
                {
                    if (lastMonthLabelIds[labelId] == null)
                    {
                        if ((dataClass == "4") && (parametersValue.periodSelect.aggregateAll))
                        {
                            decimal newValue = Convert.ToDecimal(value);
                            newValue = newValue * Convert.ToDecimal(yearDenominator[FiscalYear]);
                            newValue = decimal.Round(newValue, 0);
                            long updatedValue = Convert.ToInt64(newValue);
                            long prevValue = Convert.ToInt64(indicatorLocationIdValueHash[loc]);
                            updatedValue = prevValue + updatedValue;
                            indicatorLocationIdValueHash[loc] = updatedValue.ToString();
                        }
                        else
                        {
                            long prevValue = Convert.ToInt64(indicatorLocationIdValueHash[loc]);
                            long newValue = Convert.ToInt64(value);
                            long updatedValue = prevValue + newValue;
                            indicatorLocationIdValueHash[loc] = updatedValue.ToString();
                        }
                    }
                    else
                    {
                        indicatorLocationIdValueHash[loc] = value;
                    }
                }
            }

            string selectedSequenceNo = string.Empty;

            //if (parametersValue.dataElements.Length > 20)
            //{
            //    parametersValue.dataElementsPivot = false;
            //}

            if ((parametersValue.dataElements.Length < 20) && (parametersValue.dataElements.Length > 0))
            {
                selectedSequenceNo = " where sequenceNo in (";
                int count = 0;
                foreach (SelectedDataElements elements in parametersValue.dataElements)
                {
                    count++;
                    if (parametersValue.dataElements.Length == count)
                    {
                        selectedSequenceNo += elements.SequenceNo + ")";
                    }
                    else
                    {
                        selectedSequenceNo += elements.SequenceNo + ",";
                    }

                }
            }

            string cmdIndicator = " select * from EthEhmis_IndicatorsNewDisplay " + selectedSequenceNo +
                                  " order by SequenceNo ";
            _helper = new DBConnHelper();
            SqlCommand paramIndicator = new SqlCommand(cmdIndicator);
            DataTable dtIndicator = _helper.GetDataSet(paramIndicator).Tables[0];
            int sequenceNo; string sno = string.Empty; string indicatorName = string.Empty; string actions = string.Empty;
            string numeratorName = string.Empty; string numeratorLabelid = string.Empty; bool readOnly = false;
            string numDataEleClass = string.Empty; string denomDataEleClass = string.Empty; string reportType = string.Empty;
            string denominatorName = string.Empty; string denominatorLabelid = string.Empty;

            DataTable reportIndicatorDataTable = new DataTable();
            reportIndicatorDataTable.TableName = "IndicatorTable";
            // RegionId, ZoneId, WoredaId, IndicatorName, 
            // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue

            if (parametersValue.institutionAggrOptions.aggrType != "federal")
            {
                if (parametersValue.institutionAggrOptions.aggrType == "region")
                {
                    reportIndicatorDataTable.Columns.Add("RegionName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("RegionId", typeof(string));
                }
                else if (parametersValue.institutionAggrOptions.aggrType == "zone")
                {
                    reportIndicatorDataTable.Columns.Add("RegionName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("ZoneName", typeof(string));

                    reportIndicatorDataTable.Columns.Add("RegionId", typeof(string));
                    reportIndicatorDataTable.Columns.Add("ZoneId", typeof(string));
                }
                else if (parametersValue.institutionAggrOptions.aggrType == "district")
                {
                    reportIndicatorDataTable.Columns.Add("RegionName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("ZoneName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("WoredaName", typeof(string));

                    reportIndicatorDataTable.Columns.Add("RegionId", typeof(string));
                    reportIndicatorDataTable.Columns.Add("ZoneId", typeof(string));
                    reportIndicatorDataTable.Columns.Add("WoredaId", typeof(string));
                }
            }
            string periodWhereSelection = string.Empty;
            string periodListing = string.Empty;
            string periodGroupByListing = string.Empty;

            if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
            {
                if (parametersValue.periodSelect.aggregateAll == true)
                {
                    reportIndicatorDataTable.Columns.Add("Range", typeof(string));
                }
                else
                {
                    reportIndicatorDataTable.Columns.Add("FiscalYear", typeof(int));
                    // reportIndicatorDataTable.Columns.Add("FiscalMonth", typeof(int));
                    reportIndicatorDataTable.Columns.Add("FiscalMonth", typeof(string));
                }
            }
            if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
            {
                reportIndicatorDataTable.Columns.Add("FiscalYear", typeof(int));
                //reportIndicatorDataTable.Columns.Add("Quarter", typeof(int));
                reportIndicatorDataTable.Columns.Add("Quarter", typeof(string));
            }
            else if (parametersValue.PeriodRange == "PeriodRangeYearly")
            {
                reportIndicatorDataTable.Columns.Add("FiscalYear", typeof(int));
            }

            try
            {

                if (parametersValue.dataElementsPivot == "row")
                {
                    reportIndicatorDataTable.Columns.Add("IndicatorName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("IndicatorValue", typeof(decimal));
                    reportIndicatorDataTable.Columns.Add("NumeratorName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("NumeratorValue", typeof(decimal));
                    reportIndicatorDataTable.Columns.Add("DenominatorName", typeof(string));
                    reportIndicatorDataTable.Columns.Add("DenominatorValue", typeof(decimal));

                    foreach (DataRow row in dtIndicator.Rows)
                    {
                        sequenceNo = Convert.ToInt16(row["sequenceNo"]);
                        sno = row["SNO"].ToString();
                        indicatorName = row["indicatorName"].ToString();
                        actions = row["actions"].ToString();
                        numeratorName = row["numeratorName"].ToString();
                        numeratorLabelid = row["numeratorLabelid"].ToString();
                        readOnly = Convert.ToBoolean(row["readOnly"].ToString());
                        numDataEleClass = row["NumeratorDataEleClass"].ToString();
                        denomDataEleClass = row["DenominatorDataEleClass"].ToString();
                        reportType = row["reportType"].ToString();
                        denominatorName = row["denominatorName"].ToString();
                        denominatorLabelid = row["denominatorLabelid"].ToString();

                        // RegionId, ZoneId, WoredaId, IndicatorName, 
                        // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue
                        // 9 columns 

                        if (readOnly == false)
                        {
                            if (parametersValue.institutionAggrOptions.aggrType != "federal")
                            {
                                foreach (string locationIndex in selectedLocations.Keys)
                                {
                                    //splitLocationId = numeratorLabelid.Split(splitChar);
                                    decimal numeratorValue;
                                    decimal denominatorValue;
                                    //41929	15787	265.5919427376955723	3	304	30402

                                    DataRow rowsToAdd = reportIndicatorDataTable.NewRow();

                                    if (parametersValue.institutionAggrOptions.aggrType == "region")
                                    {
                                        rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                        rowsToAdd["RegionId"] = locRegionId[locationIndex];
                                    }
                                    else if (parametersValue.institutionAggrOptions.aggrType == "zone")
                                    {
                                        rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                        rowsToAdd["ZoneName"] = locZoneName[locationIndex];

                                        rowsToAdd["RegionId"] = locRegionId[locationIndex];
                                        rowsToAdd["ZoneId"] = locZoneId[locationIndex];
                                    }
                                    else if (parametersValue.institutionAggrOptions.aggrType == "district")
                                    {
                                        rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                        rowsToAdd["ZoneName"] = locZoneName[locationIndex];
                                        rowsToAdd["WoredaName"] = locWoredaName[locationIndex];

                                        rowsToAdd["RegionId"] = locRegionId[locationIndex];
                                        rowsToAdd["ZoneId"] = locZoneId[locationIndex];
                                        rowsToAdd["WoredaId"] = locWoredaId[locationIndex];
                                    }

                                    if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                                    {

                                        if (parametersValue.periodSelect.aggregateAll == true)
                                        {
                                            rowsToAdd["Range"] = locRange[locationIndex];
                                        }
                                        else
                                        {
                                            rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                            rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];

                                            if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                            {
                                                denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                            }
                                        }
                                    }
                                    else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                                    {
                                        rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                        rowsToAdd["Quarter"] = locQuarter[locationIndex];

                                        if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                                        {
                                            denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                        }
                                    }
                                    else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                                    {
                                        rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                    }

                                    decimal indicatorValue = sumLabelIds(yearly, denomMultiply, locationIndex, numeratorLabelid, denominatorLabelid,
                                        actions, numDataEleClass,
                                        denomDataEleClass, out numeratorValue, out denominatorValue, parametersValue);
                                    // RegionId, ZoneId, WoredaId, IndicatorName, 
                                    // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue
                                    if (indicatorValue == -1)
                                    {
                                        indicatorValue = 0;
                                    }

                                    if (numeratorValue == -1)
                                    {
                                        numeratorValue = 0;
                                    }

                                    if (denominatorValue == -1)
                                    {
                                        denominatorValue = 0;
                                    }

                                    rowsToAdd["IndicatorName"] = indicatorName;
                                    rowsToAdd["IndicatorValue"] = indicatorValue;
                                    rowsToAdd["NumeratorName"] = numeratorName;
                                    rowsToAdd["NumeratorValue"] = numeratorValue;
                                    rowsToAdd["DenominatorName"] = denominatorName;
                                    rowsToAdd["DenominatorValue"] = denominatorValue;

                                    if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                                    {
                                        if (locFiscalMonth[locationIndex].ToString() != "0")
                                        {
                                            reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                        }
                                    }
                                    else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                                    {
                                        if (locQuarter[locationIndex].ToString() != "0")
                                        {
                                            reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                        }
                                    }
                                    else
                                    {
                                        reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                    }
                                }
                            }
                            else
                            {
                                foreach (string locationIndex in selectedLocations.Keys)
                                {
                                    //splitLocationId = numeratorLabelid.Split(splitChar);
                                    decimal numeratorValue;
                                    decimal denominatorValue;

                                    DataRow rowsToAdd = reportIndicatorDataTable.NewRow();

                                    //rowsToAdd["FiscalYear"] = "2008";
                                    if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                                    {

                                        if (parametersValue.periodSelect.aggregateAll == true)
                                        {
                                            rowsToAdd["Range"] = locRange[locationIndex];
                                        }
                                        else
                                        {
                                            rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                            rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];

                                            if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                            {
                                                denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                            }
                                        }
                                    }
                                    else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                                    {
                                        rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                        rowsToAdd["Quarter"] = locQuarter[locationIndex];

                                        if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                                        {
                                            denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                        }
                                    }
                                    else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                                    {
                                        rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                    }

                                    decimal indicatorValue = sumLabelIds(yearly, denomMultiply, locationIndex, numeratorLabelid, denominatorLabelid, actions, numDataEleClass,
                                        denomDataEleClass, out numeratorValue, out denominatorValue, parametersValue);
                                    // RegionId, ZoneId, WoredaId, IndicatorName, 
                                    // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue
                                    if (indicatorValue == -1)
                                    {
                                        indicatorValue = 0;
                                    }

                                    if (numeratorValue == -1)
                                    {
                                        numeratorValue = 0;
                                    }

                                    if (denominatorValue == -1)
                                    {
                                        denominatorValue = 0;
                                    }

                                    rowsToAdd["IndicatorName"] = indicatorName;
                                    rowsToAdd["IndicatorValue"] = indicatorValue;
                                    rowsToAdd["NumeratorName"] = numeratorName;
                                    rowsToAdd["NumeratorValue"] = numeratorValue;
                                    rowsToAdd["DenominatorName"] = denominatorName;
                                    rowsToAdd["DenominatorValue"] = denominatorValue;

                                    if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                                    {
                                        if (locFiscalMonth[locationIndex].ToString() != "0")
                                        {
                                            reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                        }
                                    }
                                    else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                                    {
                                        if (locQuarter[locationIndex].ToString() != "0")
                                        {
                                            reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                        }
                                    }
                                    else
                                    {
                                        reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                    }
                                }

                            }
                        }
                    }
                }
                else if (parametersValue.dataElementsPivot != "row")
                {
                    if (parametersValue.institutionAggrOptions.aggrType != "federal")
                    {
                        foreach (string locationIndex in selectedLocations.Keys)
                        {
                            DataRow rowsToAdd = reportIndicatorDataTable.NewRow();

                            if (parametersValue.institutionAggrOptions.aggrType == "region")
                            {
                                rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                rowsToAdd["RegionId"] = locRegionId[locationIndex];
                            }
                            else if (parametersValue.institutionAggrOptions.aggrType == "zone")
                            {
                                rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                rowsToAdd["ZoneName"] = locZoneName[locationIndex];

                                rowsToAdd["RegionId"] = locRegionId[locationIndex];
                                rowsToAdd["ZoneId"] = locZoneId[locationIndex];
                            }
                            else if (parametersValue.institutionAggrOptions.aggrType == "district")
                            {
                                rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                rowsToAdd["ZoneName"] = locZoneName[locationIndex];
                                rowsToAdd["WoredaName"] = locWoredaName[locationIndex];

                                rowsToAdd["RegionId"] = locRegionId[locationIndex];
                                rowsToAdd["ZoneId"] = locZoneId[locationIndex];
                                rowsToAdd["WoredaId"] = locWoredaId[locationIndex];
                            }

                            if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                            {
                                if (parametersValue.periodSelect.aggregateAll == true)
                                {
                                    rowsToAdd["Range"] = locRange[locationIndex];
                                }
                                else
                                {
                                    rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                    rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];

                                    if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                    {
                                        denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                    }
                                }
                            }
                            else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                            {
                                rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                rowsToAdd["Quarter"] = locQuarter[locationIndex];

                                if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                                {
                                    denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                }
                            }
                            else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                            {
                                rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                            }

                            foreach (DataRow row in dtIndicator.Rows)
                            {
                                sequenceNo = Convert.ToInt16(row["sequenceNo"]);
                                sno = row["SNO"].ToString();
                                indicatorName = row["indicatorName"].ToString();
                                actions = row["actions"].ToString();
                                numeratorName = row["numeratorName"].ToString();
                                numeratorLabelid = row["numeratorLabelid"].ToString();
                                readOnly = Convert.ToBoolean(row["readOnly"].ToString());
                                numDataEleClass = row["NumeratorDataEleClass"].ToString();
                                denomDataEleClass = row["DenominatorDataEleClass"].ToString();
                                reportType = row["reportType"].ToString();
                                denominatorName = row["denominatorName"].ToString();
                                denominatorLabelid = row["denominatorLabelid"].ToString();

                                // RegionId, ZoneId, WoredaId, IndicatorName, 
                                // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue
                                // 9 columns 

                                if (readOnly == false)
                                {
                                    //splitLocationId = numeratorLabelid.Split(splitChar);
                                    decimal numeratorValue;
                                    decimal denominatorValue;
                                    //41929	15787	265.5919427376955723	3	304	30402
                                    decimal indicatorValue = sumLabelIds(yearly, denomMultiply, locationIndex, numeratorLabelid, denominatorLabelid, actions, numDataEleClass,
                                        denomDataEleClass, out numeratorValue, out denominatorValue, parametersValue);
                                    // RegionId, ZoneId, WoredaId, IndicatorName, 
                                    // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue                    

                                    if (!reportIndicatorDataTable.Columns.Contains(indicatorName))
                                    {
                                        reportIndicatorDataTable.Columns.Add(indicatorName, typeof(decimal));
                                    }
                                    if (indicatorValue == -1)
                                    {
                                        indicatorValue = 0;
                                    }
                                    rowsToAdd[indicatorName] = indicatorValue;

                                    if (!reportIndicatorDataTable.Columns.Contains(numeratorName))
                                    {
                                        reportIndicatorDataTable.Columns.Add(numeratorName, typeof(decimal));
                                    }
                                    if (numeratorValue == -1)
                                    {
                                        numeratorValue = 0;
                                    }
                                    rowsToAdd[numeratorName] = numeratorValue;

                                    if (!reportIndicatorDataTable.Columns.Contains(denominatorName))
                                    {
                                        reportIndicatorDataTable.Columns.Add(denominatorName, typeof(decimal));
                                    }
                                    if (denominatorValue == -1)
                                    {
                                        denominatorValue = 0;
                                    }
                                    rowsToAdd[denominatorName] = denominatorValue;

                                }
                            }

                            if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                            {
                                if (locFiscalMonth[locationIndex].ToString() != "0")
                                {
                                    reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                }
                            }
                            else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                            {
                                if (locQuarter[locationIndex].ToString() != "0")
                                {
                                    reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                }
                            }
                            else
                            {
                                reportIndicatorDataTable.Rows.Add(rowsToAdd);
                            }
                        }
                    }
                    else
                    {
                        foreach (string locationIndex in selectedLocations.Keys)
                        {
                            DataRow rowsToAdd = reportIndicatorDataTable.NewRow();
                            //rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];

                            if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                            {
                                if (parametersValue.periodSelect.aggregateAll == true)
                                {
                                    rowsToAdd["Range"] = locRange[locationIndex];
                                }
                                else
                                {
                                    rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                    rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];

                                    if (parametersValue.PeriodRange == "PeriodRangeYearMonthly")
                                    {
                                        denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                    }
                                }
                            }
                            else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                            {
                                rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                rowsToAdd["Quarter"] = locQuarter[locationIndex];

                                if (parametersValue.PeriodRange == "PeriodRangeYearQuarterly")
                                {
                                    denomMultiply = Convert.ToDecimal(locYearDenominator[locationIndex]);
                                }
                            }
                            else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                            {
                                rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                            }

                            foreach (DataRow row in dtIndicator.Rows)
                            {
                                sequenceNo = Convert.ToInt16(row["sequenceNo"]);
                                sno = row["SNO"].ToString();
                                indicatorName = row["indicatorName"].ToString();
                                actions = row["actions"].ToString();
                                numeratorName = row["numeratorName"].ToString();
                                numeratorLabelid = row["numeratorLabelid"].ToString();
                                readOnly = Convert.ToBoolean(row["readOnly"].ToString());
                                numDataEleClass = row["NumeratorDataEleClass"].ToString();
                                denomDataEleClass = row["DenominatorDataEleClass"].ToString();
                                reportType = row["reportType"].ToString();
                                denominatorName = row["denominatorName"].ToString();
                                denominatorLabelid = row["denominatorLabelid"].ToString();

                                // RegionId, ZoneId, WoredaId, IndicatorName, 
                                // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue
                                // 9 columns 

                                if (readOnly == false)
                                {
                                    //splitLocationId = numeratorLabelid.Split(splitChar);
                                    decimal numeratorValue;
                                    decimal denominatorValue;
                                    //41929	15787	265.5919427376955723	3	304	30402
                                    decimal indicatorValue = sumLabelIds(yearly, denomMultiply, locationIndex, numeratorLabelid,
                                        denominatorLabelid, actions, numDataEleClass,
                                        denomDataEleClass, out numeratorValue, out denominatorValue, parametersValue);
                                    // RegionId, ZoneId, WoredaId, IndicatorName, 
                                    // IndicatorValue, NumeratorName, NumeratorValue, DenominatorName, DenominatorValue                           

                                    if (!reportIndicatorDataTable.Columns.Contains(indicatorName))
                                    {
                                        reportIndicatorDataTable.Columns.Add(indicatorName, typeof(decimal));
                                    }
                                    if (indicatorValue == -1)
                                    {
                                        indicatorValue = 0;
                                    }
                                    rowsToAdd[indicatorName] = indicatorValue;

                                    if (!reportIndicatorDataTable.Columns.Contains(numeratorName))
                                    {
                                        reportIndicatorDataTable.Columns.Add(numeratorName, typeof(decimal));
                                    }
                                    if (numeratorValue == -1)
                                    {
                                        numeratorValue = 0;
                                    }
                                    rowsToAdd[numeratorName] = numeratorValue;

                                    if (!reportIndicatorDataTable.Columns.Contains(denominatorName))
                                    {
                                        reportIndicatorDataTable.Columns.Add(denominatorName, typeof(decimal));
                                    }
                                    if (denominatorValue == -1)
                                    {
                                        denominatorValue = 0;
                                    }
                                    rowsToAdd[denominatorName] = denominatorValue;
                                }
                            }

                            if ((parametersValue.PeriodRange == "PeriodRangeMonthly") || (parametersValue.PeriodRange == "PeriodRangeYearMonthly"))
                            {
                                if (locFiscalMonth[locationIndex].ToString() != "0")
                                {
                                    reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                }
                            }
                            else if ((parametersValue.PeriodRange == "PeriodRangeQuarterly") || (parametersValue.PeriodRange == "PeriodRangeYearQuarterly"))
                            {
                                if (locQuarter[locationIndex].ToString() != "0")
                                {
                                    reportIndicatorDataTable.Rows.Add(rowsToAdd);
                                }
                            }
                            else
                            {
                                reportIndicatorDataTable.Rows.Add(rowsToAdd);
                            }
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                string expText = exp.Message;
            }



            //DataTable reportDataTable = _helper.GetDataSet(toExecute).Tables[0];

            //string[] columnNames = reportIndicatorDataTable.Columns.Cast<DataColumn>()
            //                .Select(x => x.ColumnName)
            //                .ToArray();

            //listObj.Add(columnNames);
            //listObj.Add(reportIndicatorDataTable);            

            return reportIndicatorDataTable;
        }

        [EnableCors("*", "*", "*")]
        private decimal sumLabelIds(bool yearly, decimal denomMultiply, string locationId, string numeratorLabelid, string denominatorLabelid, string actions,
            string numDataEleClass, string denomDataEleClass, out decimal num,
            out decimal denom, AnalyticsParameters parametersValues)
        {
            //int denomMultiply = 1;

            Hashtable labelIdValue = indicatorLocationIdValueHash;

            char splitChar = ',';
            numeratorLabelid = numeratorLabelid.Trim();
            numDataEleClass = numDataEleClass.Trim();
            denominatorLabelid = denominatorLabelid.Trim();
            denomDataEleClass = denomDataEleClass.Trim();

            bool populationDenom = false;
            bool includeLoc = false;

            if (locationId != "")
            {
                includeLoc = true;
            }
            else
            {
                includeLoc = false;
            }


            decimal indicatorValue = 0;

            string[] splitNumLabelIds = numeratorLabelid.Split(splitChar);
            string[] splitNumDataEleClass = numDataEleClass.Split(splitChar);

            string[] splitDenomLabelIds = denominatorLabelid.Split(splitChar);
            string[] splitDenomDataEleClass = denomDataEleClass.Split(splitChar);

            num = 0;
            denom = 0;

            bool numData = false;
            bool denomData = false;

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
                string trimmedLabelId = string.Empty;
                StringBuilder indexBuilder = new StringBuilder();

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
                        if (includeLoc)
                        {
                            indexBuilder.Append(locationId); indexBuilder.Append("_");
                        }
                        indexBuilder.Append(trimmedDataEleClass);
                        indexBuilder.Append("_"); indexBuilder.Append(labelid.Trim());
                        trimmedLabelId = indexBuilder.ToString();

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
                    }
                }
            }
            else
            {
                string trimmedLabelId = string.Empty;
                StringBuilder indexBuilder = new StringBuilder();
                foreach (string labelid in splitNumLabelIds)
                {
                    foreach (string dataEleClass in splitNumDataEleClass)
                    {
                        string trimmedDataEleClass = dataEleClass.Trim();
                        indexBuilder = new StringBuilder();
                        if (includeLoc)
                        {
                            indexBuilder.Append(locationId); indexBuilder.Append("_");
                        }
                        indexBuilder.Append(trimmedDataEleClass);
                        indexBuilder.Append("_"); indexBuilder.Append(labelid.Trim());
                        trimmedLabelId = indexBuilder.ToString();

                        if (labelIdValue[trimmedLabelId] != null)
                        {
                            num += Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                            numData = true;
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
                        StringBuilder indexBuilder = new StringBuilder();
                        string trimmedLabelId = string.Empty;
                        foreach (string labelid in splitDenomLabelIds)
                        {
                            foreach (string dataEleClass in splitDenomDataEleClass)
                            {
                                string trimmedDataEleClass = dataEleClass.Trim();
                                if (includeLoc)
                                {
                                    if ((dataEleClass == "4") && (!yearly)
                                        && (parametersValues.periodSelect.aggregateAll == false))
                                    {
                                        char locSplitChar = '_';
                                        string[] newLocIdSplit = locationId.Split(locSplitChar);
                                        string newLoc = string.Empty;
                                        for (int i = 0; i < newLocIdSplit.Length - 1; i++)
                                        {
                                            newLoc += newLocIdSplit[i] + "_";
                                        }
                                        newLoc += "0";
                                        locationId = newLoc;
                                        //newLocId[0]
                                    }

                                    indexBuilder.Append(locationId); indexBuilder.Append("_");
                                }
                                indexBuilder.Append(trimmedDataEleClass);
                                indexBuilder.Append("_"); indexBuilder.Append(labelid.Trim());
                                trimmedLabelId = indexBuilder.ToString();

                                if (labelIdValue[trimmedLabelId] != null)
                                {
                                    decimal multiDenom = Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                                    denom *= Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                                    denomData = true;

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
                            }
                        }

                        if (denom != 0)
                        {
                            if (populationDenom == true)
                            {
                                denom = denom * denomMultiply;
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
                        StringBuilder indexBuilder = new StringBuilder();
                        string trimmedLabelId = string.Empty;
                        foreach (string labelid in splitDenomLabelIds)
                        {
                            foreach (string dataEleClass in splitDenomDataEleClass)
                            {
                                string trimmedDataEleClass = dataEleClass.Trim();
                                indexBuilder = new StringBuilder();

                                if (includeLoc)
                                {
                                    if ((dataEleClass == "4") && (!yearly)
                                         && (parametersValues.periodSelect.aggregateAll == false))
                                    {
                                        char locSplitChar = '_';
                                        string[] newLocIdSplit = locationId.Split(locSplitChar);
                                        string newLoc = string.Empty;
                                        for (int i = 0; i < newLocIdSplit.Length - 1; i++)
                                        {
                                            newLoc += newLocIdSplit[i] + "_";
                                        }
                                        newLoc += "0";
                                        locationId = newLoc;
                                        //newLocId[0]
                                    }

                                    indexBuilder.Append(locationId); indexBuilder.Append("_");
                                }
                                indexBuilder.Append(trimmedDataEleClass);
                                indexBuilder.Append("_"); indexBuilder.Append(labelid.Trim());
                                trimmedLabelId = indexBuilder.ToString();

                                if (labelIdValue[trimmedLabelId] != null)
                                {
                                    denom += Convert.ToDecimal(labelIdValue[trimmedLabelId]);
                                    denomData = true;
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
                            }
                        }
                    }
                    if (denom != 0)
                    {
                        if (populationDenom == true)
                        {
                            denom = denom * denomMultiply;
                            // we need the monthly
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
            }
            else
            {
                denom = -1;
                indicatorValue = decimal.Round(num, 2);
            }

            if (denomData == true)
            {
                denom = decimal.Round(denom, 2);
            }
            else
            {
                denom = -1;
            }

            if (numData == true)
            {
                if (actions.ToUpper() == "SUMHUNDREDMINUS")
                {
                    if (indicatorValue != 0)
                    {
                        indicatorValue = 100 - indicatorValue;
                    }
                }

                num = decimal.Round(num, 2);
                indicatorValue = decimal.Round(indicatorValue, 2);
            }
            else
            {
                num = -1;
                indicatorValue = -1;
            }

            return indicatorValue;
        }

        private string processFacilityIndicator(AnalyticsParameters parameterValues, out string institutionWhere)
        {
            string parameterSql = string.Empty;
            institutionWhere = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodListing = string.Empty;
            string periodGroupByListing = string.Empty;
            string periodOrderByListing = string.Empty;

            if (parameterValues.PeriodRange == "PeriodRangeMonthly")
            {

                periodWhereSelection = " (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                              " and ((FiscalMonth = 0) or (FiscalMonth >= " + parameterValues.periodSelect.StartMonth +
                              " and FiscalMonth <= " + parameterValues.periodSelect.EndMonth + ")))";

                if ((parameterValues.periodSelect.aggregate == true) &&
                  (parameterValues.periodSelect.StartMonth != parameterValues.periodSelect.EndMonth))
                {
                    string monthRange =
                               " case   \n" +
                               " when FiscalMonth = 0 " + " then '0' " +
                               " else " +
                               " 'Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth + "'" +
                               " end as FiscalMonth, ";

                    // string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                    periodListing = " FiscalYear, " + monthRange;
                }
                else if ((parameterValues.periodSelect.aggregateAll == true) &&
                 (parameterValues.periodSelect.StartMonth != parameterValues.periodSelect.EndMonth))
                {
                    string monthRange =
                               " case   \n" +
                               " when FiscalMonth = 0 " + " then '0' " +
                               " else " +
                               " 'Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth + "'" +
                               " end as FiscalMonth, ";

                    // string monthRange = "Fiscal Month:" + parameterValues.periodSelect.StartMonth + "-" + parameterValues.periodSelect.EndMonth;
                    periodListing = " FiscalYear, " + monthRange;
                }
                else
                {
                    periodListing = " FiscalYear, FiscalMonth, ";
                }

                periodGroupByListing = " FiscalYear, FiscalMonth, ";
                periodOrderByListing = " FiscalYear, FiscalMonth ";
            }
            else if (parameterValues.PeriodRange == "PeriodRangeQuarterly")
            {

                periodWhereSelection = " (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                              " and ((Quarter = 0) or (Quarter >= " + parameterValues.periodSelect.StartQuarter +
                              " and Quarter <= " + parameterValues.periodSelect.EndQuarter + ")))";

                if ((parameterValues.periodSelect.aggregate == true) &&
                (parameterValues.periodSelect.StartQuarter != parameterValues.periodSelect.EndQuarter))
                {
                    string quarterRange =
                               " case   \n" +
                               " when Quarter = 0 " + " then '0' " +
                               " else " +
                               " 'Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter + "'" +
                               " end as Quarter, ";

                    periodListing = " FiscalYear, " + quarterRange;
                }
                else if ((parameterValues.periodSelect.aggregateAll == true) &&
                (parameterValues.periodSelect.StartQuarter != parameterValues.periodSelect.EndQuarter))
                {
                    string quarterRange =
                               " case   \n" +
                               " when Quarter = 0 " + " then '0' " +
                               " else " +
                               " 'Quarter:" + parameterValues.periodSelect.StartQuarter + "-" + parameterValues.periodSelect.EndQuarter + "'" +
                               " end as Quarter, ";

                    periodListing = " FiscalYear, " + quarterRange;
                }
                else
                {
                    periodListing = " FiscalYear, Quarter, ";
                }

                periodGroupByListing = " FiscalYear, Quarter, FiscalMonth, ";
                periodOrderByListing = " FiscalYear, Quarter, FiscalMonth ";

            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearly")
            {
                if (parameterValues.periodSelect.EndFiscalYear != 0)
                {
                    periodListing = " FiscalYear,  ";
                    periodGroupByListing = " FiscalYear, FiscalMonth,  ";
                    periodOrderByListing = " FiscalYear, FiscalMonth ";
                    periodWhereSelection = " (FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                           " and FiscalYear <= " + parameterValues.periodSelect.EndFiscalYear + ")";
                }
                else
                {
                    periodListing = " FiscalYear,  ";
                    periodGroupByListing = " FiscalYear, FiscalMonth,  ";
                    periodOrderByListing = " FiscalYear, FiscalMonth ";
                    periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear + ")";
                }
            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearMonthly")
            {
                //(((FiscalYear >= 2008 and FiscalYear < 2009 )  and((Quarter >= 1 and Quarter <= 2)  or Quarter = 0))
                //OR
                //(FiscalYear = 2009 and((Quarter >= 1 and Quarter <= 2) or Quarter = 0)))                               
                periodWhereSelection =
                    "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                        " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                        " and ((FiscalMonth >= " + parameterValues.periodSelect.StartYearStartMonth +
                                       " and FiscalMonth <= " + parameterValues.periodSelect.StartYearEndMonth + ") " +
                                       " or FiscalMonth = 0 ))" +
                     " OR " +
                     "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " and ((FiscalMonth >= " + parameterValues.periodSelect.EndYearStartMonth +
                                       " and FiscalMonth <= " + parameterValues.periodSelect.EndYearEndMonth + ")" +
                                       " or FiscalMonth = 0 )))";

                if (parameterValues.periodSelect.aggregate == true)
                {
                    int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                    string monthRange =
                        " case   \n" +
                        " when (FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                          endFiscalYearMinusOne + " and FiscalMonth != 0) " +
                          " then " + "'Month:" +
                          parameterValues.periodSelect.StartYearStartMonth + "-" +
                          parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                        " when (FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " and FiscalMonth != 0 ) " +
                        " then " + "'Month:" +
                          parameterValues.periodSelect.EndYearStartMonth + "-" +
                          parameterValues.periodSelect.EndYearEndMonth + "'" +
                        " when FiscalMonth = 0 " + " then '0' " +
                        " end as FiscalMonth, ";

                    periodListing = " FiscalYear, " + monthRange;
                    periodOrderByListing = " FiscalYear, FiscalMonth ";
                }
                else if (parameterValues.periodSelect.aggregateAll == true)
                {
                    int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                    string range = "'Range:" + parameterValues.periodSelect.StartFiscalYear + "_Month_" +
                        parameterValues.periodSelect.StartYearStartMonth + "_" + parameterValues.periodSelect.StartYearEndMonth +
                        "_TO_" + parameterValues.periodSelect.EndFiscalYear + "_Month_" +
                        parameterValues.periodSelect.EndYearStartMonth + "_" + parameterValues.periodSelect.EndYearEndMonth + "'" +
                        " as Range, ";
                    //" case   \n" +
                    //" when (FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                    //  endFiscalYearMinusOne + " and FiscalMonth != 0) " +
                    //  " then " + "'Month:" +
                    //  parameterValues.periodSelect.StartYearStartMonth + "-" +
                    //  parameterValues.periodSelect.StartYearEndMonth + "'" + "\n" +
                    //" when (FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                    //" and FiscalMonth != 0 ) " +
                    //" then " + "'Month:" +
                    //  parameterValues.periodSelect.EndYearStartMonth + "-" +
                    //  parameterValues.periodSelect.EndYearEndMonth + "'" +
                    //" when FiscalMonth = 0 " + " then '0' " +
                    //" end as FiscalMonth, ";

                    periodListing = "FiscalYear, " + range;
                    periodOrderByListing = " FiscalYear, FiscalMonth ";
                }
                else
                {
                    periodListing = " FiscalYear, FiscalMonth,  ";
                    periodOrderByListing = " FiscalYear, FiscalMonth ";
                }

                periodGroupByListing = " FiscalYear, FiscalMonth, ";

            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearQuarterly")
            {
                //(((FiscalYear >= 2008 and FiscalYear < 2009 )  and((Quarter >= 1 and Quarter <= 2)  or Quarter = 0))
                //OR
                //(FiscalYear = 2009 and((Quarter >= 1 and Quarter <= 2) or Quarter = 0)))                

                periodWhereSelection =
                   "(((FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                       " and FiscalYear < " + parameterValues.periodSelect.EndFiscalYear + " ) " +
                       " and ((Quarter >= " + parameterValues.periodSelect.StartYearStartQuarter +
                                      " and Quarter <= " + parameterValues.periodSelect.StartYearEndQuarter + " ) " +
                                      " or Quarter = 0 ))" +
                    " OR " +
                    "(FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                       " and ((Quarter >= " + parameterValues.periodSelect.EndYearStartQuarter +
                                      " and Quarter <= " + parameterValues.periodSelect.EndYearEndQuarter + ")" +
                                      " or Quarter = 0)))";

                if (parameterValues.periodSelect.aggregate == true)
                {
                    int endFiscalYearMinusOne = parameterValues.periodSelect.EndFiscalYear - 1;
                    string quarterRange =
                        " case   \n" +
                        " when (FiscalYear between " + parameterValues.periodSelect.StartFiscalYear + " and \n" +
                          endFiscalYearMinusOne + " and Quarter != 0) " +
                          " then " + "'Quarter:" +
                          parameterValues.periodSelect.StartYearStartQuarter + "-" +
                          parameterValues.periodSelect.StartYearEndQuarter + "'" + "\n" +
                        " when (FiscalYear = " + parameterValues.periodSelect.EndFiscalYear +
                        " and Quarter != 0 ) " +
                        " then " + "'Quarter:" +
                          parameterValues.periodSelect.EndYearStartQuarter + "-" +
                          parameterValues.periodSelect.EndYearEndQuarter + "'" +
                        " when Quarter = 0 " + " then '0' " +
                        " end as Quarter, ";

                    periodListing = " FiscalYear, " + quarterRange;
                }
                else
                {
                    periodListing = " FiscalYear, Quarter,  ";
                }
                periodGroupByListing = " FiscalYear, Quarter, FiscalMonth, ";
                periodOrderByListing = " FiscalYear, Quarter, FiscalMonth ";
            }


            //-------------------------------//---------------------------------------------------

            if (parameterValues.institutionAggrOptions.aggrType == "federal")
            {
                parameterSql = " select " + periodListing +
                                 //FiscalYear, 
                                 " DataClass, LabelId, sum(value) as Value \n" +
                                 " from v_EthEhmis_HmisValue \n" +
                                 " where  \n" +
                                 //"  fiscalYear = 2008 " +
                                 periodWhereSelection +
                                 " and (((DataClass in (2,3,6,7,8))) \n" +
                                 "  or  \n" +
                                 "   (DataClass = 4  and ((locationId = '88')))) \n" +
                                 "  group by  " +
                                // "  FiscalYear, " +
                                periodGroupByListing +
                                 "  DataClass,LabelId  \n" +
                                 " order by " +
                                 " DataClass,LabelId," + periodOrderByListing + "\n";

                // okay, after all this, further consider what was passed via the locations whitelist and their facility types...
                if (parameterValues.FilterFacilityType != 0)
                {
                    if (parameterValues.FilterFacilityType == 11)
                    {
                        //MOH, do nothing more...
                    }
                    else
                    {
                        if (parameterValues.FilterFacilityType == 10)// regions
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.regionId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        else
                            if (parameterValues.FilterFacilityType == 9)// zone
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.zoneId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        if (parameterValues.FilterFacilityType == 8)// woreda
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.woredaId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }

                    }
                }

            }
            else if (parameterValues.institutionAggrOptions.aggrType == "region")
            {
                if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }

                // okay, after all this, further consider what was passed via the locations whitelist and their facility types...
                if (parameterValues.FilterFacilityType != 0)
                {
                    if (parameterValues.FilterFacilityType == 11)
                    {
                        //MOH, do nothing more...
                    }
                    else
                    {
                        if (parameterValues.FilterFacilityType == 10)// regions
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.regionId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        else
                            if (parameterValues.FilterFacilityType == 9)// zone
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.zoneId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        if (parameterValues.FilterFacilityType == 8)// woreda
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.woredaId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }

                    }
                }

                parameterSql = " select " + periodListing +
                                 //"  FiscalYear, "
                                 " RegionID, DataClass, LabelId, sum(value) as Value \n" +
                                 " from v_EthEhmis_HmisValue \n" +
                                 " where  \n" +
                                 //"  fiscalYear = 2008 " +
                                 periodWhereSelection +
                                 " and (regionId != 88) " + institutionWhere + " and " +
                                 "  (((DataClass in (2,3,6,7,8))) \n" +
                                 "  or  \n" +
                                 "  ( DataClass = 4  and ((locationId = cast(regionId as varchar(100)))))) \n" +
                                 "  group by " +
                                // " FiscalYear, " +
                                periodGroupByListing +
                                 " RegionId, DataClass,LabelId  \n" +
                                 " order by " +
                                 " RegionId, DataClass,LabelId," + periodOrderByListing + "\n";

            }
            else if (parameterValues.institutionAggrOptions.aggrType == "zone")
            {
                if (parameterValues.institutionAggrOptions.facilityTypeId == 9) // It is a zone
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.zoneId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }

                // okay, after all this, further consider what was passed via the locations whitelist and their facility types...
                if (parameterValues.FilterFacilityType != 0)
                {
                    if (parameterValues.FilterFacilityType == 11)
                    {
                        //MOH, do nothing more...
                    }
                    else
                    {
                        if (parameterValues.FilterFacilityType == 10)// regions
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.regionId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        else
                            if (parameterValues.FilterFacilityType == 9)// zone
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.zoneId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        if (parameterValues.FilterFacilityType == 8)// woreda
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.woredaId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }

                    }
                }

                parameterSql = " select " + periodListing +
                                 //"  FiscalYear, 
                                 " RegionID, ZoneId, DataClass, LabelId, sum(value) as Value \n" +
                                 " from v_EthEhmis_HmisValue \n" +
                                  " where  \n" +
                                 //"  fiscalYear = 2008 " +
                                 periodWhereSelection +
                                 "  and zoneId != 0 " + institutionWhere + " and " +
                                 "  (((DataClass in (2,3,6,7,8))) \n" +
                                 "  or  \n" +
                                 "  ( DataClass = 4  and ((locationId = cast(zoneId as varchar(100)))))) \n" +
                                  "  group by " +
                                // " FiscalYear, " +
                                periodGroupByListing +
                                " RegionId, ZoneId, DataClass,LabelId  \n" +
                                " order by " +
                                " RegionId, ZoneId, DataClass,LabelId," + periodOrderByListing + "\n";
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "district")
            {
                if (parameterValues.institutionAggrOptions.facilityTypeId == 8) // It is a Woreda
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.woredaId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 9) // It is a Zone
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.zoneId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }
                else if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }

                // okay, after all this, further consider what was passed via the locations whitelist and their facility types...
                if (parameterValues.FilterFacilityType != 0)
                {
                    if (parameterValues.FilterFacilityType == 11)
                    {
                        //MOH, do nothing more...
                    }
                    else
                    {
                        if (parameterValues.FilterFacilityType == 10)// regions
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.regionId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        else
                            if (parameterValues.FilterFacilityType == 9)// zone
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.zoneId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }
                        if (parameterValues.FilterFacilityType == 8)// woreda
                        {
                            institutionWhere += " and (v_EthEhmis_HmisValue.woredaId IN (" + String.Join(",", parameterValues.LocationIds) + ")) ";
                        }

                    }
                }

                parameterSql = " select " + periodListing +
                              //"  FiscalYear, 
                              " RegionID, ZoneId, WoredaId, DataClass, LabelId, sum(value) as Value \n" +
                                " from v_EthEhmis_HmisValue \n" +
                                  " where  \n" +
                                 //"  fiscalYear = 2008 " +
                                 periodWhereSelection +
                                "  and WoredaId != 0 " + institutionWhere + " and " +
                                "  (((DataClass in (2,3,6,7,8))) \n" +
                                "  or  \n" +
                                "  ( DataClass = 4  and ((locationId = cast(woredaId as varchar(100)))))) \n" +
                               "  group by " +
                                // " FiscalYear, " +
                                periodGroupByListing +
                                "  RegionId, ZoneId, WoredaId, DataClass,LabelId  \n" +
                                 " order by " +
                                 " RegionId, ZoneId, WoredaId, DataClass,LabelId," + periodOrderByListing + "\n";

            }

            return parameterSql;
        }
    }
}
