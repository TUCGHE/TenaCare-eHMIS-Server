using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SqlManagement.Database;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Web.Http.Cors;
using System.Collections;
using eHMISWebApi.Models;
using Newtonsoft.Json.Linq;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AnalyticsController : ApiController
    {
        // GET: api/Analytics
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Analytics/5
        public DataTable Get(int id)
        {
            DataTable dt = new DataTable();           

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
                    categorySql =   "  select labelid, FullDescription from EthioHIMS_ServiceDataElementsNew  " +
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

        // POST: api/Analytics
        public DataTable Post([FromBody]AnalyticsParameters value)
        {
            JObject obj = (Newtonsoft.Json.Linq.JObject)value.categoryList;
            List<string> categories = new List<string>();
            List<string> mainCategory = new List<string>();
            List<string> category1 = new List<string>();
            List<string> category2 = new List<string>();
            List<string> category3 = new List<string>();
            List<string> category4 = new List<string>();
            List<string> category5 = new List<string>();

            // If main categories are clicked...don't worry about the lower...for example if service is clicked...

            foreach (var jObj in obj)
            {
                string category = jObj.Key;
                if (category == "category1")
                {
                    category1.Add(jObj.Value.ToString());
                }
                else if (category == "category2")
                {
                    category2.Add(jObj.Value.ToString());
                }
                else if (category == "category3")
                {
                    category3.Add(jObj.Value.ToString());
                }
                else if (category == "category4")
                {
                    category4.Add(jObj.Value.ToString());
                }
                else if (category == "category5")
                {
                    category5.Add(jObj.Value.ToString());
                }

                categories.Add(category);
                //string categoryQuery = "";
            }

            string cat1 = string.Empty;
            string cat2 = string.Empty;
            string cat3 = string.Empty;
            string cat4 = string.Empty;
            string cat5 = string.Empty;
            if (category1.Count != 0)
            {

                cat1 = " (category1 in (";
                int count = 0;
                foreach (string cat in category1)
                {
                    count++;
                    if (count == category1.Count)
                    {
                        // last one so no need for ,
                        cat1 += cat;
                    }
                    else
                    {
                        cat1 += cat + ",";
                    }
                }
                cat1 += "))";
            }

            if (category2.Count != 0)
            {

                cat2 = " (category1 in (";
                int count = 0;
                foreach (string cat in category2)
                {
                    count++;
                    if (count == category2.Count)
                    {
                        // last one so no need for ,
                        cat2 += cat;
                    }
                    else
                    {
                        cat2 += cat + ",";
                    }
                }
                cat2 += "))";
            }

            if (category3.Count != 0)
            {

                cat3 = " (category1 in (";
                int count = 0;
                foreach (string cat in category3)
                {
                    count++;
                    if (count == category3.Count)
                    {
                        // last one so no need for ,
                        cat3 += cat;
                    }
                    else
                    {
                        cat3 += cat + ",";
                    }
                }
                cat3 += "))";
            }

            if (category4.Count != 0)
            {

                cat4 = " (category1 in (";
                int count = 0;
                foreach (string cat in category4)
                {
                    count++;
                    if (count == category4.Count)
                    {
                        // last one so no need for ,
                        cat4 += cat;
                    }
                    else
                    {
                        cat4 += cat + ",";
                    }
                }
                cat4 += "))";
            }

            if (category5.Count != 0)
            {

                cat5 = " (category1 in (";
                int count = 0;
                foreach (string cat in category5)
                {
                    count++;
                    if (count == category5.Count)
                    {
                        // last one so no need for ,
                        cat5 += cat;
                    }
                    else
                    {
                        cat5 += cat + ",";
                    }
                }
                cat5 += "))";
            }     

            DataTable dt = new DataTable();

            string parameterSql = "select * from EthioHIMS_ServiceDataElementsNew where " + cat1 + " or " + cat2 +
                " or " + cat3 + " or " + cat4 + " or " + cat5 + " order by sequenceNo ";

            DBConnHelper _helper = new DBConnHelper();
            SqlCommand parameterCmd = new SqlCommand(parameterSql);
            dt = _helper.GetDataSet(parameterCmd).Tables[0];

            return dt;
        }

        // PUT: api/Analytics/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Analytics/5
        public void Delete(int id)
        {
        }
    }
}
