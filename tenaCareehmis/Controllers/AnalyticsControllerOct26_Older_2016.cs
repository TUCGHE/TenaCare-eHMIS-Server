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
        private const string FacilityTable = "EthEhmis_AllFacilityWithID";

        public AnalyticsController(ICacheClient client)
        {
            _cacheClient = client;
        }

        // GET: api/Analytics/5
        public DataTable Get(int id)
        {
            DataTable dt = new DataTable();

            if (id == 0)
            {
                DBConnHelper _helper = new DBConnHelper();

                string diseaseListText = " select distinct Disease, 'False' as Checked, '' as groupName from " +
                                         " v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Age " +
                                         "  where dataEleClass = 8 or dataEleClass = 2 or dataEleClass = 3";

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

        public DataTable Get(int id1, int id2, string category)
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
                    categorySql = "select distinct category1 from EthioHIMS_ServiceDataElementsNew where category1 != ''";

                    categoryCmd = new SqlCommand(categorySql);
                    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                }
                else if (id2 == 2)
                {
                    categorySql = "  select distinct category2 from EthioHIMS_ServiceDataElementsNew where category2 != '' " +
                                  "  and category1 = '" + category + "'";

                    categoryCmd = new SqlCommand(categorySql);
                    dt = _helper.GetDataSet(categoryCmd).Tables[0];
                }
                else if (id2 == 3)
                {
                    categorySql = "select distinct category3 from EthioHIMS_ServiceDataElementsNew where category3 != '' " +
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
                    categorySql = "  select labelid, FullDescription from EthioHIMS_ServiceDataElementsNew  " +
                                  "  where labelId is not null " +
                                  "  order by SequenceNo ";
                }
                else
                {
                    categorySql = "  select labelid, FullDescription from EthioHIMS_ServiceDataElementsNew  " +
                                    "  where labelId is not null and category1 = '" + category + "'" +
                                    "  order by SequenceNo ";
                    //categorySql = "  select labelid, fullDescription from EthioHIMS_ServiceDataElementsNew where category2 != '' " +
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
            DataTable reportDataTable = new DataTable();

            string serviceParameterSql = string.Empty;
            reportDataTable = ReportHelper(parameterValues, out serviceParameterSql);
            if(Constants.UseMemcached)
                _cacheClient.AddToCache(parameterValues.GetKey(), reportDataTable);

            //sha1Hash = UtilitiesNew.GeneralUtilities.CryptorEngine.createSHA1Hash()

            string test = parameterValues.ToString();

            List<object> stringAndDataTable = new List<object>();
            string[] columnNames = reportDataTable.Columns.Cast<DataColumn>()
                                   .Select(x => x.ColumnName)
                                   .ToArray();

            stringAndDataTable.Add(columnNames);
            stringAndDataTable.Add(reportDataTable);
            // Inlucde the queries in this collection
            stringAndDataTable.Add(serviceParameterSql);
            return stringAndDataTable;
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
                ArrayList labelIdList = new ArrayList();
                int count = 0;
                bool lastMonthCalc = false;
                bool yearlyRange = false;


                if (parameterValues.PeriodRange == "PeriodRangeYearly")
                {
                    yearlyRange = true;

                    string labelIdsLastMonthText = " select labelId from EthioHIMS_ServiceDataElementsNew " +
                                                   " where aggregationType = 1 and labelId is not null";
                    SqlCommand lastMonthCmd = new SqlCommand(labelIdsLastMonthText);
                    DataTable dt = connHelper.GetDataSet(lastMonthCmd).Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        int labelId = Convert.ToInt16(row["labelId"].ToString());
                        labelIdList.Add(labelId);
                    }
                }

                foreach (SelectedDataElements elements in parameterValues.dataElements)
                {
                    count++;
                    if (parameterValues.dataElements.Length == count) // Last item, do something else
                    {
                        fullDescriptionsAll += "cast([" + elements.FullDescription + "] as bigint) as [" +

                           elements.FullDescription.Trim() + "]\n";

                        fullDescriptionsAllPivot += "[" + elements.FullDescription + "]\n";

                        if (yearlyRange)
                        {
                            if (labelIdList.Contains(elements.labelId))
                            {
                                lastMonthCalc = true;
                                labelIdsSumLastMonth += elements.labelId + ",";
                            }
                            else
                            {
                                labelIdsSumAll += elements.labelId + ",";
                            }
                        }
                        else
                        {
                            labelIdsSumAll += elements.labelId + ",";
                        }
                    }
                    else
                    {
                        fullDescriptionsAll += "cast([" + elements.FullDescription + "] as bigint) as [" +
                            elements.FullDescription.Trim() + "],\n";

                        fullDescriptionsAllPivot += "[" + elements.FullDescription + "],\n";

                        if (yearlyRange)
                        {
                            if (labelIdList.Contains(elements.labelId))
                            {
                                lastMonthCalc = true;
                                labelIdsSumLastMonth += elements.labelId + ",";
                            }
                            else
                            {
                                labelIdsSumAll += elements.labelId + ",";
                            }
                        }
                        else
                        {
                            labelIdsSumAll += elements.labelId + ",";
                        }
                    }
                }

                if (labelIdsSumAll != "")
                {
                    labelIdsSumAll = labelIdsSumAll.Remove(labelIdsSumAll.Length - 1, 1);
                }

                if (lastMonthCalc)
                {
                    if (labelIdsSumLastMonth != "")
                    {
                        labelIdsSumLastMonth = labelIdsSumLastMonth.Remove(labelIdsSumLastMonth.Length - 1, 1);
                    }
                }


                serviceParameterSql = getServiceQuery(parameterValues, fullDescriptionsAll,
                    fullDescriptionsAllPivot, labelIdsSumAll, labelIdsSumLastMonth, lastMonthCalc, facilityWhereQuery);

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
            if(Constants.UseMemcached)
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

        private string getLastMonthServiceQuery(AnalyticsParameters parameterValues, string labelIdsSumAll, string labelIdsSumLastMonth)
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

            processAggregationType(parameterValues, out aggregationOptions, out aggregationGroupBy, out aggregationIdsPivot,
                out aggregationIdsRows, out institutionWhere, out institutionType,
                out lastMonthAggrIdComma, out vaggregationOptions, out vaggregationOptionsAs, 
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy);         

            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection);

            string frontListingAggrOption = string.Empty;
            string frontListingIds = string.Empty;
            string orderByListing = string.Empty;
            string groupByListing = string.Empty;
            string facilityWhereQuery = string.Empty;

            ProcessFacilityTypes(parameterValues, out facilityWhereQuery);
            
            if (parameterValues.dataElementsPivot == "row")
            {
                frontListingAggrOption = vaggregationOptionsAs;
                frontListingIds = vaggregationIdsRows + " v.sequenceNo, \n";

                if (labelIdsSumAll == "")
                {
                    orderByListing =
                         "    order by  v.FiscalYear, \n" +
                   //"    v.ReportingRegionName, " +
                   vaggregationOptions + vaggregationIdsRows +
                   "    v.sequenceNo  \n";
                }
                else
                {
                    orderByListing =
                        "    order by FiscalYear, \n" +
                  //"    v.ReportingRegionName, " +
                  aggregationGroupBy + aggregationIdsPivot +
                  "    sequenceNo  \n";

                }

                groupByListing =
                               "    group by v.FullDescription, v.FiscalYear, " +
               //"    v.ReportingRegionName, \n" +
               vaggregationOptions + vaggregationIdsRows +
               "    v.sequenceNo \n";
            }
            else if (parameterValues.dataElementsPivot == "pivotinstitutions")
            {
                frontListingAggrOption = vaggregationIdsPivot;
                aggregationIdsRows = "";
                vaggregationIdsRows = "";
                lastMonthAggrIdComma = "";

                groupByListing =
                               "    group by " + vaggregationOptions +
                               "    v.FullDescription, v.FiscalYear " + " \n";               
            }
            else if (parameterValues.dataElementsPivot == "pivotdataelements")
            {
                frontListingAggrOption = vaggregationOptions;
                frontListingIds = vaggregationIdsRows;
       
                groupByListing =
                               "    group by v.FullDescription, v.FiscalYear, " +
               //"    v.ReportingRegionName, \n" +
               vaggregationOptions + vaggregationIdsRows +
               "    v.sequenceNo \n";
            }
            else
            {
                frontListingAggrOption = vaggregationOptions;
                frontListingIds = vaggregationIdsRows;

                orderByListing =
                     "    order by  v.FiscalYear, \n" +              
               vaggregationOptions + vaggregationIdsRows +
               "    v.sequenceNo  \n";

                groupByListing =
                               "    group by v.FullDescription, v.FiscalYear, " +
               //"    v.ReportingRegionName, \n" +
               vaggregationOptions + vaggregationIdsRows +
               "    v.sequenceNo \n";
            }

           string cmdText =                         
                 "   select " +
                // "   v.ReportingRegionName, " +
                 frontListingAggrOption + frontListingIds +
                 "   v.FullDescription, v.FiscalYear, " +
                 "   sum(value) as Value from \n" +
                 "   ( " +
                 "           select " +
                 //"           ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 "  FullDescription, sequenceNo, " +
                 "           FiscalYear, [FiscalMonth], \n" +
                 "           sum(value) as Value from v_EthEhmis_HmisValue   " +
                 "  INNER JOIN " + FacilityTable + " on    " +
                 FacilityTable + ".hmiscode = v_EthEhmis_HmisValue.LocationId \n" +
                 "  INNER JOIN EthioHIMS_ServiceDataElementsNew on  " +
                 "  EthioHIMS_ServiceDataElementsNew.LabelId = v_EthEhmis_HmisValue.LabelID \n" +
                 "           where EthioHIMS_ServiceDataElementsNew.labelId in ( " +
                 labelIdsSumLastMonth + " )     \n" +
                 periodWhereSelection + institutionWhere + facilityWhereQuery +
                 "           group by FullDescription, sequenceNo, \n" +
                 //" ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 "           fiscalYear, [FiscalMonth]  \n" +
                 "   )s " +
                 "   inner join " +
                 "   (  " +
                 "           select " +
                 //"           ReportingRegionName, " +
                 taggregationGroupBy + lastMonthAggrIdComma +
                 "           FullDescription, sequenceNo, " +
                 "           FiscalYear, max(FiscalMonth) as FiscalMonth from \n" +
                 "           (  " +
                  "           select " +
                 //"           ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 "              FullDescription, sequenceNo, " +
                 "              FiscalYear, [FiscalMonth], \n" +
                 "              sum(value) as Value from v_EthEhmis_HmisValue   " +
                 "              INNER JOIN " + FacilityTable + " on    " +
                                FacilityTable + ".hmiscode = v_EthEhmis_HmisValue.LocationId \n" +
                 "              INNER JOIN EthioHIMS_ServiceDataElementsNew on  " +
                 "              EthioHIMS_ServiceDataElementsNew.LabelId = v_EthEhmis_HmisValue.LabelID \n" +
                 "              where EthioHIMS_ServiceDataElementsNew.labelId in ( " +
                                labelIdsSumLastMonth + " )     \n" +
                                periodWhereSelection + institutionWhere +
                 "              group by FullDescription, sequenceNo, \n" +
                 //"              ReportingRegionName, " +
                 aggregationGroupBy + aggregationIdsRows +
                 "              fiscalYear, [FiscalMonth]  \n" +
                 "           ) t    " +
                 "              group by FullDescription, sequenceNo, " +
                 //"              ReportingRegionName, " +
                 taggregationGroupBy + lastMonthAggrIdComma +
                 "              FiscalYear " +
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
            string fullDescriptionsPivot, string labelIdsSumAll, string labelIdsSumLastMonth, bool lastMonthCalc,
            string facilityWhereQuery)            
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
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy);          

            string cmdText = "";

            // If dataelements are greater than 15, difficult, slow, non-user friendly to use pivot with columns so disable it by default....
            //if (parameterValues.dataElements.Length > 15)
            //{
            //    parameterValues.dataElementsPivot = false;
            //}

            string periodListing = string.Empty;
            string periodWhereSelection = string.Empty;
            string periodGroupByListing = string.Empty;

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection);

            string unionLastMonth = string.Empty;

            if (lastMonthCalc)
            {
                if (labelIdsSumAll != "")
                {
                    unionLastMonth = " union  \n" + getLastMonthServiceQuery(parameterValues, labelIdsSumAll, labelIdsSumLastMonth);
                }
                else
                {
                    unionLastMonth = getLastMonthServiceQuery(parameterValues, labelIdsSumAll, labelIdsSumLastMonth) + " \n";
                }
            }

            string sumAllInsideQueryDataElements = string.Empty;
            string sumAllInsideQueryInstitutions = string.Empty;
            string sumAllInsideQueryRow = string.Empty;

            if (labelIdsSumAll != "")
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
                    " join [EthioHIMS_ServiceDataElementsNew] on " + "\n" +
                    " [EthioHIMS_ServiceDataElementsNew].LabelId = v_EthEhmis_HmisValue.LabelId " + "\n" +
                    "   where v_EthEhmis_HmisValue.labelId in  " + "\n" +
                    " ( " + labelIdsSumAll + " ) " + "\n" +
                    " and (dataClass = 6 or dataClass =7 )" + institutionWhere + facilityWhereQuery + "\n" +
                    //" and " + " (FiscalYear = 2008) " +
                    periodWhereSelection + "\n" +
                    " group by[EthioHIMS_ServiceDataElementsNew].FullDescription, " + "\n" +
                    aggregationGroupBy + aggregationIdsRows + "\n" +
                    //" FiscalYear " +
                    periodGroupByListing + "\n";
                }
                else if (parameterValues.dataElementsPivot == "pivotinstitutions")
                {
                    sumAllInsideQueryInstitutions =
                       " select " + aggregationIdsPivot + "\n" +
                       " FullDescription, " + "\n" +
                       //" FiscalYear, " +
                       periodListing + "\n" +
                       " sum(value) as value  from " + FacilityTable + "\n" +
                       " inner join v_EthEhmis_HmisValue on " + "\n" +
                       FacilityTable + ".HMISCode = v_EthEhmis_HmisValue.LocationID " + "\n" +
                       " inner " + "\n" +
                       " join [EthioHIMS_ServiceDataElementsNew] on " + "\n" +
                       " [EthioHIMS_ServiceDataElementsNew].LabelId = v_EthEhmis_HmisValue.LabelId " + "\n" +
                       "   where v_EthEhmis_HmisValue.labelId in  " + "\n" +
                       " ( " + labelIdsSumAll + " ) " + "\n" +
                       " and (dataClass = 6 or dataClass =7 )" + institutionWhere + facilityWhereQuery + "\n" +
                       //" and " + " (FiscalYear = 2008) " +
                       periodWhereSelection + "\n" +
                       " group by[EthioHIMS_ServiceDataElementsNew].FullDescription, " + "\n" +
                       aggregationGroupBy + "\n" +
                       //" FiscalYear " +
                       periodGroupByListing + "\n";
                }
                else
                {
                    string orderByListing = string.Empty;

                    if (lastMonthCalc == false)
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
                    " join [EthioHIMS_ServiceDataElementsNew] on " +
                    " [EthioHIMS_ServiceDataElementsNew].LabelId = v_EthEhmis_HmisValue.LabelId \n" +
                    "   where v_EthEhmis_HmisValue.labelId in  " +
                    " ( " + labelIdsSumAll + " ) " +
                    " and (dataClass = 6 or dataClass = 7 )" + institutionWhere + facilityWhereQuery + "\n" +
                    //" and " + " (FiscalYear = 2008) " +
                    periodWhereSelection +
                    " group by[EthioHIMS_ServiceDataElementsNew].FullDescription, \n" +
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
             
                    unionLastMonth + "\n" +
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
                    unionLastMonth + "\n" +
                    "    ) as t  " + "\n" +
                    "    pivot " + "\n" +
                    "    (  " + "\n" +
                    "        max(value) for " + institutionType + " in  " + "\n" +
                    "    (  " + aggregationOptions + " ) " + "\n" +
                    "    )p " + "\n" +
                    "    order by " + periodGroupByListing + "\n";
                //" FiscalYear ";

            }
            else
            {
                // For now just choose this as default
                cmdText =
                   sumAllInsideQueryRow +
                   unionLastMonth;
            }
            return cmdText;
        }

        private void processAggregationType(AnalyticsParameters parameterValues, out string aggregationOptions, out string aggregationGroupBy,
         out string aggregationIdsPivot, out string aggregationIdsRows,
         out string institutionWhere, out string institutionType,
         out string lastMonthAggrIdComma, out string vaggregationOptions, out string vaggregationOptionsAs,
         out string vaggregationIdsRows, out string lastMonthInnerJoins, out string vaggregationIdsPivot, out string taggregationGroupBy)
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
                                     " from EthEhmis_AllFacilityWithID where " +
                                     " (hmiscode != '20' and hmiscode != '21') " + institutionWhere;

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
                                     "   Name, ReportingRegionName, RegionId, ZoneId from EthEhmis_AllFacilityWithId " +
                                     "   where zoneName not like('%NoZone_FMOH%') " + institutionWhere +
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
            else if (parameterValues.institutionAggrOptions.aggrType == "woreda")
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
                                     "   Name, ReportingRegionName, RegionId, WoredaId from EthEhmis_AllFacilityWithId " +
                                     "   where WoredaName not like('NoWoreda%FMOH%') " + institutionWhere +
                                     "   ) as t  order by ReportingRegionName ";


                    DBConnHelper _helper = new DBConnHelper();
                    SqlCommand parameterCmd = new SqlCommand(cmdText);
                    DataTable dt = _helper.GetDataSet(parameterCmd).Tables[0];
                    int count = 0;
                    aggregationIdsPivot = "   ReportingRegionName + '_' + rtrim(WoredaName) + '_' + " + woredaId2 + " as WoredaName, ";

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

                vaggregationOptions = " v.ReportingRegionName, v.ZoneName, v.WoredaName, v.FacilityName, v.FacilityTypeName,  ";
                vaggregationOptionsAs = " v.ReportingRegionName as RegionName,  v.ZoneName, v.WoredaName, v.FacilityName, v.FacilityTypeName,  ";
                // vaggregationIdsRows = " v.RegionId, v.ZoneId, v.WoredaId,  v.LocationId, ";
                lastMonthInnerJoins = " and \n  " +
                                      " v.ReportingRegionName = s.ReportingRegionName and \n" +
                                      // " v.RegionId = s.RegionId and \n" +
                                      " v.ZoneName = s.ZoneName and \n" +
                                      // " v.ZoneId = s.ZoneId and \n" +
                                      " v.WoredaName = s.WoredaName and \n" +
                                      // " v.WoredaId = s.WoredaId and \n" +
                                      " v.FacilityName = s.FacilityName ";
                //  " v.LocationId = s.LocationId \n";

                aggregationOptions = "   ReportingRegionName as RegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName,  ";
                aggregationGroupBy = "   ReportingRegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName, ";
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
                aggregationOptions = "   ReportingRegionName as RegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName,  ";
                aggregationGroupBy = "   ReportingRegionName, ZoneName, WoredaName, FacilityName, FacilityTypeName,  ";
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
        }



        private DataTable DiseaseProcessing(AnalyticsParameters parameterValues, string facilityWhereQuery)
        {
            List<object> stringAndDataTable = new List<object>();
            DBConnHelper _helper = new DBConnHelper();
            string facilityTable = "  EthEhmis_AllFacilityWithID";

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
                out vaggregationIdsRows, out lastMonthInnerJoins, out vaggregationIdsPivot, out taggregationGroupBy);

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
                        diseaseList += "'" + elements.Disease + "'))\n";
                    }
                    else
                    {
                        diseaseList += "'" + elements.Disease + "',\n";
                    }

                    if ((elements.Disease != elements.groupName) && (elements.groupName != ""))
                    {
                        // should be updated
                        updateGroupName += " update " + diseaseTableName + " set groupName = '" + elements.groupName + "'" + "\n" +
                                           " where disease = '" + elements.Disease + "'" + "\n\n";
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

            processingPeriod(parameterValues, out periodListing, out periodGroupByListing,
                out periodWhereSelection);

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
            "   inner join " + facilityTable + " \n" +
            //"   on v_EthEhmis_HmisValue.LocationId = v_EthEhmis_AllFacilityWithId.hmisCode \n" +
            "   on v_EthEhmis_HmisValue.LocationId = " + facilityTable + ".hmisCode \n" +
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
                              " gender, age, disease, disease from DiseaseDictionary ";
            }
            else
            {

                cmdInsert = " insert into [aaTempDiseaseDictionaryRoot] " +
                               " select id, sno, labelId, dataEleClass, descrip, classAndLabel, " +
                               " gender, age, disease, disease from DiseaseDictionary where " + diseaseList;
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
                initialListing = " , max([population]) as Population, \n" +
                                        " cast((((cast(sum(value) as decimal(18, 2))) * 1000) / (cast(max([population]) as decimal(18, 2)))) as decimal(18,0)) \n" +
                                        " as CasesPer1000Population \n ";

                innerJoinPopulation = " inner join aaPopulationDenominator on  \n ";

                if (parameterValues.institutionAggrOptions.aggrType == "federal")
                {
                    innerJoinPopulation += " aaPopulationDenominator.LocationId = 20 and  \n ";
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
                else if (parameterValues.institutionAggrOptions.aggrType == "woreda")
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

            if ((parameterValues.diseaseOptions.male == false) &&
                 (parameterValues.diseaseOptions.female == false))
            {
                showGender_column = false;
            }
            else if ((parameterValues.diseaseOptions.male == true) &&
                 (parameterValues.diseaseOptions.female == true))
            {
                showGender_column = true;
                genderQuery = " and ((gender = 'M') or (gender = 'F')) ";
            }
            else if ((parameterValues.diseaseOptions.male == false) &&
               (parameterValues.diseaseOptions.female == true))
            {
                showGender_column = true;
                genderQuery = " and (gender = 'F') ";
            }
            else if ((parameterValues.diseaseOptions.male == true) &&
               (parameterValues.diseaseOptions.female == false))
            {
                showGender_column = true;
                genderQuery = " and (gender = 'M') ";
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
                ageQuery = " and ((age ='Under_5') OR (age = '5_14') OR (age = 'Above_14')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
               (parameterValues.diseaseOptions.fiveToFourteen == true) &&
               (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                //Under_5
                //5_14
                //Above_14
                showAge_Column = true;
                ageQuery = " and ((age ='Under_5') OR (age = '5_14')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
               (parameterValues.diseaseOptions.fiveToFourteen == false) &&
               (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and ((age ='Above_14') OR (age = '5_14')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == false) &&
                (parameterValues.diseaseOptions.fiveToFourteen == true) &&
                (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and ((age = 'Above_14') OR (age = '5_14')) ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == false) &&
              (parameterValues.diseaseOptions.fiveToFourteen == false) &&
              (parameterValues.diseaseOptions.fifteenAndAbove == true))
            {
                showAge_Column = true;
                ageQuery = " and (age = 'Above_14') ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == true) &&
            (parameterValues.diseaseOptions.fiveToFourteen == false) &&
            (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                //Under_5
                //5_14
                //Above_14
                showAge_Column = true;
                ageQuery = " and (age = 'Under_5') ";
            }
            else if ((parameterValues.diseaseOptions.zeroToFour == false) &&
           (parameterValues.diseaseOptions.fiveToFourteen == true) &&
           (parameterValues.diseaseOptions.fifteenAndAbove == false))
            {
                //Under_5
                //5_14
                //Above_14
                showAge_Column = true;
                ageQuery = " and (age = '5_14') ";
            }

            if (showAge_Column)
            {
                ageColumn = " Age, ";
            }
        }

        private void processingPeriod(AnalyticsParameters parameterValues, out string periodListing,
            out string periodGroupByListing, out string periodWhereSelection)
        {
            periodListing = string.Empty;
            periodGroupByListing = string.Empty;
            periodWhereSelection = string.Empty;

            if (parameterValues.PeriodRange == "PeriodRangeMonthly")
            {
                if (parameterValues.periodSelect.EndMonth == 0)
                {
                    periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                                  " and FiscalMonth = " + parameterValues.periodSelect.StartMonth + ")";
                    periodListing = " FiscalYear, FiscalMonth, ";
                    periodGroupByListing = " FiscalYear, FiscalMonth ";
                }
                else
                {
                    periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                                  " and (FiscalMonth >= " + parameterValues.periodSelect.StartMonth +
                                  " and FiscalMonth <= " + parameterValues.periodSelect.EndMonth + "))";
                    periodListing = " FiscalYear, FiscalMonth, ";
                    periodGroupByListing = " FiscalYear, FiscalMonth ";
                }

            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearly")
            {
                if (parameterValues.periodSelect.EndFiscalYear != 0)
                {
                    periodListing = " FiscalYear,  ";
                    periodGroupByListing = " FiscalYear ";
                    periodWhereSelection = " and (FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                           " and FiscalYear <= " + parameterValues.periodSelect.EndFiscalYear + ")";
                }
                else
                {
                    periodListing = " FiscalYear,  ";
                    periodGroupByListing = " FiscalYear ";
                    periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear + ")";
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
                    if ((categories.Count == 1) && (categories[0].ToString() == "service"))
                    {
                        string parameterSql = "select LabelId, FullDescription, 'False' as Checked, '' as groupName, Category1 " +
                       " from EthioHIMS_ServiceDataElementsNew where (labelId is not null or labelId != '') " +
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
                       " from EthioHIMS_ServiceDataElementsNew where (labelId is not null or labelId != '') and (" + cat1 + " or " + cat2 +
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
                                   " EthEhmis_AllFacilityWithID where facilityTypeid in (10) " +
                                   " order by RegionId, ZoneId, WoredaId, FacilityTypeId ";
            }
            else if (parametersValue.institutionAggrOptions.aggrType == "zone")
            {
                facilityQuery =
                " select * from  " +
                                   " EthEhmis_AllFacilityWithID where facilityTypeid in (9) " +
                                   " order by RegionId, ZoneId, WoredaId, FacilityTypeId ";
            }
            else if (parametersValue.institutionAggrOptions.aggrType == "woreda")
            {
                facilityQuery =
                " select * from  " +
                                   " EthEhmis_AllFacilityWithID where facilityTypeid in (8) " +
                                   " order by RegionId, ZoneId, WoredaId, FacilityTypeId ";
            }

            string loc = string.Empty;
            Hashtable locWoredaName = new Hashtable();
            Hashtable locZoneName = new Hashtable();
            Hashtable locRegionName = new Hashtable();

            Hashtable locWoredaId = new Hashtable();
            Hashtable locZoneId = new Hashtable();
            Hashtable locRegionId = new Hashtable();

            Hashtable locFiscalMonth = new Hashtable();
            Hashtable locFiscalYear = new Hashtable();

            StringBuilder locIndex = new StringBuilder();

            List<object> listObj = new List<object>();
            DataTable dt = null;

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
                    else if (parametersValue.institutionAggrOptions.aggrType == "woreda")
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
            if (parametersValue.PeriodRange == "PeriodRangeMonthly")
            {
                denomMultiply = numMain / denomMain;
            }

            string institutionWhere = string.Empty;

            string parameterSql = processFacilityIndicator(parametersValue, out institutionWhere);
            //string parameterSql = " select RegionId, 0 as ZoneId,0 as WoredaId, DataClass, LabelId, sum(value) as Value \n" +
            //                      " from v_EthEhmis_HmisValue \n" +
            //                      " where  \n" +
            //                      " woredaId = 30504 and  \n" +
            //                      "  fiscalYear = 2008 and((DataClass = 6 or DataClass = 7  \n" +
            //                      "  or dataClass = 8 or dataClass = 2 or dataClass = 3 \n" +
            //                      "  ) \n" +
            //                      "  or  \n" +
            //                      "   (DataClass = 4  and((locationId = cast(regionId as varchar(100)))))) \n" +
            //                      "  group by DataClass,LabelId, RegionId  \n" +
            //                      "  Union  \n" +
            //                      "  select RegionId, ZoneId, 0 as WoredaId, DataClass, LabelId, sum(value) as Value \n" +
            //                      "  from v_EthEhmis_HmisValue \n" +
            //                      "  where \n" +
            //                      "  woredaId = 30504 and \n" +
            //                      "  fiscalYear = 2008 and (zoneId !=0) and ((DataClass = 6 or DataClass = 7 \n" +
            //                      "  or dataClass = 8 or dataClass = 2 or dataClass = 3 \n" +
            //                      "  ) \n" +
            //                      "  or  \n" +
            //                      "   (DataClass = 4  and((locationId = cast(regionId as varchar(100))) \n" +
            //                      "   or(locationId = cast(zoneId as varchar(100)))))) \n" +
            //                      "  group by DataClass,LabelId, RegionId, ZoneId \n" +
            //                      "  Union \n" +
            //                      "  select RegionId, ZoneId, WoredaId, DataClass, LabelId, sum(value) as Value \n" +
            //                      "  from v_EthEhmis_HmisValue \n" +
            //                      "  where \n" +
            //                      "  woredaId = 30504 and \n" +
            //                      "  fiscalYear = 2008 and(woredaId != 0) and((DataClass = 6 or DataClass = 7 \n" +
            //                      "  or dataClass = 8 or dataClass = 2 or dataClass = 3 \n" +
            //                      "  ) \n" +
            //                      "  or \n" +
            //                      "   (DataClass = 4  and((locationId = cast(woredaId as varchar(100)))))) \n" +
            //                      "  group by DataClass,LabelId, RegionId, ZoneId, WoredaId \n" +
            //                      "  --order by RegionId, ZoneId, WoredaId,  DataClass,LabelID \n";



            _helper = new DBConnHelper();
            SqlCommand parameterCmd = new SqlCommand(parameterSql);
            dt = _helper.GetDataSet(parameterCmd).Tables[0];
            indicatorLocationIdValueHash = new Hashtable();
            Hashtable selectedLocations = new Hashtable();

            StringBuilder index = new StringBuilder();
            loc = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                //RegionId, ZoneId, WoredaId, DataClass, LabelId, sum(value) as Value

                index = new StringBuilder();

                if (parametersValue.institutionAggrOptions.aggrType != "federal")
                {
                    if (parametersValue.institutionAggrOptions.aggrType == "region")
                    {
                        string regionId = row["RegionId"].ToString();
                        int FiscalYear = 0, FiscalMonth = 0;

                        index.Append(regionId);
                        string regionName = locRegionName[index.ToString()].ToString();
                        index.Append("_");

                        if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(FiscalMonth);
                        }
                        else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            index.Append(FiscalYear);
                        }
                        loc = index.ToString();

                        locRegionName[loc] = regionName;
                        locRegionId[loc] = regionId;
                        locFiscalMonth[loc] = FiscalMonth;
                        locFiscalYear[loc] = FiscalYear;

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

                        int FiscalYear = 0, FiscalMonth = 0;
                        if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(FiscalMonth);
                        }
                        else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            index.Append(FiscalYear);
                        }
                        loc = index.ToString();

                        locRegionName[loc] = regionName;
                        locZoneName[loc] = zoneName;
                        locRegionId[loc] = regionId;
                        locZoneId[loc] = zoneId;
                        locFiscalMonth[loc] = FiscalMonth;
                        locFiscalYear[loc] = FiscalYear;


                        selectedLocations[loc] = 1;
                        index.Append("_");
                    }
                    else if (parametersValue.institutionAggrOptions.aggrType == "woreda")
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

                        int FiscalYear = 0, FiscalMonth = 0;
                        if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                            index.Append(FiscalYear);
                            index.Append("_");
                            index.Append(FiscalMonth);
                        }
                        else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                        {
                            FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                            index.Append(FiscalYear);
                        }
                        loc = index.ToString();

                        locRegionName[loc] = regionName;
                        locZoneName[loc] = zoneName;
                        locWoredaName[loc] = woredaName;
                        locRegionId[loc] = regionId;
                        locZoneId[loc] = zoneId;
                        locWoredaId[loc] = woredaId;

                        locFiscalMonth[loc] = FiscalMonth;
                        locFiscalYear[loc] = FiscalYear;

                        selectedLocations[loc] = 1;
                        index.Append("_");
                    }
                }
                else
                {
                    int FiscalYear = 0, FiscalMonth = 0;

                    if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                    {
                        FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                        FiscalMonth = Convert.ToInt32(row["FiscalMonth"].ToString());
                        index.Append(FiscalYear);
                        index.Append("_");
                        index.Append(FiscalMonth);
                    }
                    else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                    {
                        FiscalYear = Convert.ToInt32(row["FiscalYear"].ToString());
                        index.Append(FiscalYear);
                    }
                    loc = index.ToString();

                    locFiscalMonth[loc] = FiscalMonth;
                    locFiscalYear[loc] = FiscalYear;
                    selectedLocations[loc] = 1;
                    index.Append("_");
                }

                string dataClass = row["DataClass"].ToString();
                string labelId = row["LabelId"].ToString();
                string value = row["Value"].ToString();

                index.Append(dataClass); index.Append("_"); index.Append(labelId);

                loc = index.ToString();

                indicatorLocationIdValueHash[loc] = value;
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
                else if (parametersValue.institutionAggrOptions.aggrType == "woreda")
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

            if (parametersValue.PeriodRange == "PeriodRangeMonthly")
            {
                reportIndicatorDataTable.Columns.Add("FiscalYear", typeof(int));
                reportIndicatorDataTable.Columns.Add("FiscalMonth", typeof(int));
            }
            else if (parametersValue.PeriodRange == "PeriodRangeYearly")
            {
                reportIndicatorDataTable.Columns.Add("FiscalYear", typeof(int));
            }


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

                                decimal indicatorValue = sumLabelIds(denomMultiply, locationIndex, numeratorLabelid, denominatorLabelid,
                                    actions, numDataEleClass,
                                    denomDataEleClass, out numeratorValue, out denominatorValue);
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
                                else if (parametersValue.institutionAggrOptions.aggrType == "woreda")
                                {
                                    rowsToAdd["RegionName"] = locRegionName[locationIndex];
                                    rowsToAdd["ZoneName"] = locZoneName[locationIndex];
                                    rowsToAdd["WoredaName"] = locWoredaName[locationIndex];

                                    rowsToAdd["RegionId"] = locRegionId[locationIndex];
                                    rowsToAdd["ZoneId"] = locZoneId[locationIndex];
                                    rowsToAdd["WoredaId"] = locWoredaId[locationIndex];
                                }

                                if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                                {
                                    rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                    rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];
                                }
                                else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                                {
                                    rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                }


                                rowsToAdd["IndicatorName"] = indicatorName;
                                rowsToAdd["IndicatorValue"] = indicatorValue;
                                rowsToAdd["NumeratorName"] = numeratorName;
                                rowsToAdd["NumeratorValue"] = numeratorValue;
                                rowsToAdd["DenominatorName"] = denominatorName;
                                rowsToAdd["DenominatorValue"] = denominatorValue;

                                if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                                {
                                    if (locFiscalMonth[locationIndex].ToString() != "0")
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
                                //41929	15787	265.5919427376955723	3	304	30402
                                decimal indicatorValue = sumLabelIds(denomMultiply, locationIndex, numeratorLabelid, denominatorLabelid, actions, numDataEleClass,
                                    denomDataEleClass, out numeratorValue, out denominatorValue);
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
                                DataRow rowsToAdd = reportIndicatorDataTable.NewRow();

                                //rowsToAdd["FiscalYear"] = "2008";
                                if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                                {
                                    rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                    rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];
                                }
                                else if (parametersValue.PeriodRange == "PeriodRangeYearly")
                                {
                                    rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                                }


                                rowsToAdd["IndicatorName"] = indicatorName;
                                rowsToAdd["IndicatorValue"] = indicatorValue;
                                rowsToAdd["NumeratorName"] = numeratorName;
                                rowsToAdd["NumeratorValue"] = numeratorValue;
                                rowsToAdd["DenominatorName"] = denominatorName;
                                rowsToAdd["DenominatorValue"] = denominatorValue;

                                if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                                {
                                    if (locFiscalMonth[locationIndex].ToString() != "0")
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
            else
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
                        else if (parametersValue.institutionAggrOptions.aggrType == "woreda")
                        {
                            rowsToAdd["RegionName"] = locRegionName[locationIndex];
                            rowsToAdd["ZoneName"] = locZoneName[locationIndex];
                            rowsToAdd["WoredaName"] = locWoredaName[locationIndex];

                            rowsToAdd["RegionId"] = locRegionId[locationIndex];
                            rowsToAdd["ZoneId"] = locZoneId[locationIndex];
                            rowsToAdd["WoredaId"] = locWoredaId[locationIndex];
                        }

                        if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                        {
                            rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];
                            rowsToAdd["FiscalMonth"] = locFiscalMonth[locationIndex];
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
                                decimal indicatorValue = sumLabelIds(denomMultiply, locationIndex, numeratorLabelid, denominatorLabelid, actions, numDataEleClass,
                                    denomDataEleClass, out numeratorValue, out denominatorValue);
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

                            }
                        }

                        if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                        {
                            if (locFiscalMonth[locationIndex].ToString() != "0")
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
                        rowsToAdd["FiscalYear"] = locFiscalYear[locationIndex];

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
                                decimal indicatorValue = sumLabelIds(denomMultiply, "", numeratorLabelid, denominatorLabelid, actions, numDataEleClass,
                                    denomDataEleClass, out numeratorValue, out denominatorValue);
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
                            }
                        }

                        if (parametersValue.PeriodRange == "PeriodRangeMonthly")
                        {
                            if (locFiscalMonth[locationIndex].ToString() != "0")
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

            //DataTable reportDataTable = _helper.GetDataSet(toExecute).Tables[0];

            //string[] columnNames = reportIndicatorDataTable.Columns.Cast<DataColumn>()
            //                .Select(x => x.ColumnName)
            //                .ToArray();

            //listObj.Add(columnNames);
            //listObj.Add(reportIndicatorDataTable);            

            return reportIndicatorDataTable;
        }

        [EnableCors("*", "*", "*")]
        private decimal sumLabelIds(decimal denomMultiply, string locationId, string numeratorLabelid, string denominatorLabelid, string actions,
            string numDataEleClass, string denomDataEleClass, out decimal num,
            out decimal denom)
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
                                    if ((dataEleClass == "4") && (denomMultiply != 1))
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
                                    if ((dataEleClass == "4") && (denomMultiply != 1))
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

            if (parameterValues.PeriodRange == "PeriodRangeMonthly")
            {
                if (parameterValues.periodSelect.EndMonth == 0)
                {
                    periodWhereSelection = " (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                                  " and ((FiscalMonth = 0) or  (FiscalMonth = " + parameterValues.periodSelect.StartMonth + ")))";
                    periodListing = " FiscalYear, FiscalMonth, ";
                    periodGroupByListing = " FiscalYear, FiscalMonth ";
                }
                else
                {
                    periodWhereSelection = " (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear.ToString() +
                                  " and ((FiscalMonth = 0) or (FiscalMonth >= " + parameterValues.periodSelect.StartMonth +
                                  " and FiscalMonth <= " + parameterValues.periodSelect.EndMonth + ")))";
                    periodListing = " FiscalYear, FiscalMonth, ";
                    periodGroupByListing = " FiscalYear, FiscalMonth ";
                }

            }
            else if (parameterValues.PeriodRange == "PeriodRangeYearly")
            {
                if (parameterValues.periodSelect.EndFiscalYear != 0)
                {
                    periodListing = " FiscalYear,  ";
                    periodGroupByListing = " FiscalYear ";
                    periodWhereSelection = " (FiscalYear >= " + parameterValues.periodSelect.StartFiscalYear +
                                           " and FiscalYear <= " + parameterValues.periodSelect.EndFiscalYear + ")";
                }
                else
                {
                    periodListing = " FiscalYear,  ";
                    periodGroupByListing = " FiscalYear ";
                    periodWhereSelection = " and (FiscalYear = " + parameterValues.periodSelect.StartFiscalYear + ")";
                }
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
                                 "   (DataClass = 4  and ((locationId = '20')))) \n" +
                                 "  group by  " +
                                // "  FiscalYear, " +
                                periodListing +
                                 "  DataClass,LabelId  \n";

            }
            else if (parameterValues.institutionAggrOptions.aggrType == "region")
            {
                if (parameterValues.institutionAggrOptions.facilityTypeId == 10) // It is a region
                {
                    institutionWhere = "  and ( v_EthEhmis_HmisValue.regionId = " + parameterValues.institutionAggrOptions.hmiscode + ") ";
                }

                parameterSql = " select " + periodListing +
                                 //"  FiscalYear, "
                                 " RegionID, DataClass, LabelId, sum(value) as Value \n" +
                                 " from v_EthEhmis_HmisValue \n" +
                                 " where  \n" +
                                 //"  fiscalYear = 2008 " +
                                 periodWhereSelection +
                                 " and (regionId != 20) " + institutionWhere + " and " +
                                 "  (((DataClass in (2,3,6,7,8))) \n" +
                                 "  or  \n" +
                                 "  ( DataClass = 4  and ((locationId = cast(regionId as varchar(100)))))) \n" +
                                 "  group by " +
                                // " FiscalYear, " +
                                periodListing +
                                 " RegionId, DataClass,LabelId  \n";

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
                                periodListing +
                                " RegionId, ZoneId, DataClass,LabelId  \n";
            }
            else if (parameterValues.institutionAggrOptions.aggrType == "woreda")
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
                                periodListing +
                                "  RegionId, ZoneId, WoredaId, DataClass,LabelId  \n";

            }

            return parameterSql;
        }
    }
}
