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
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Web.Http.Cors;
using Admin.SiteSetting.DataAccess;

namespace eHMISWebApi.Controllers
{
    //[Authorize]
    
    public class EhmisValuesController : ApiController
    {
        // GET api/values

        string ipdTable = string.Empty;
        string serviceTable = string.Empty;
        string languageSet = string.Empty;

        [EnableCors("*", "*", "*")]
        public string Get()
        //public IEnumerable<DataTable> Get(int regionId, int facilityType)
        {
            return "value";
        }

        private void setCorrectLanguageTable()
        {           
            ipdTable = "EthioHIMS_QuarterIPDDiseaseView";
            serviceTable = "EthioHIMS_ServiceDataElementsNew";

            DBConnHelper _helper = new DBConnHelper();
            string cmdText = " select language from languageSetting where languageSet = 1";
            SqlCommand languageCmd = new SqlCommand(cmdText);

            DataTable dt = _helper.GetDataSet(languageCmd).Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                languageSet = row["language"].ToString();
            }

            if (languageSet != "english")
            {
                ipdTable = ipdTable + languageSet;
                serviceTable = serviceTable + languageSet;
            }
        }

        // GET api/values/5
        [EnableCors("*", "*", "*")]
        public DataTable Get(int id)
        {          
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = string.Empty;

            setCorrectLanguageTable();

            DataTable dt = null;

            if (id == 1)
            {
                cmdText = " select sno, activityhc, readonly, labelid from " + serviceTable +
                          " where periodType = 0 order by sequenceno";
                SqlCommand getIPDDictionary = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(getIPDDictionary).Tables[0];
            }
            else if (id == 2)
            {
                cmdText = "select * from  " + ipdTable;
                SqlCommand getIPDDictionary = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(getIPDDictionary).Tables[0];
            }           
                      
            return dt;
        }
        [EnableCors("*", "*", "*")]
        public DataTable GetByFacilityType([FromUri] int id, string type, int periodType)
        {

            DBConnHelper _helper = new DBConnHelper();
            string cmdText = string.Empty;

            DataTable dt = null;

            if (id == 1)
            {
                cmdText = "select sno, activityhc, readonly, labelid from EthioHIMS_ServiceDataElementsNew where " + type + " =1 AND periodType = " + periodType + " order by sequenceno";
                SqlCommand getIPDDictionary = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(getIPDDictionary).Tables[0];
            }
            else if (id == 2)
            {
                cmdText = "select * from " + ipdTable;
                SqlCommand getIPDDictionary = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(getIPDDictionary).Tables[0];
            }

            return dt;
        }


        // GET api/values/5
        [EnableCors("*", "*", "*")]      
        public DataTable Get(int id, string locationId, int year, int month, string reportType)
        {
            //string reportType = "IPD";
            //int year = 2008;
            //int month = 1;
            string dataEleClassQuery = "";
            string idQuery = "   (locationId = '" + locationId + "') ";
            string periodQuery = "   (year = " + year + " and month = " + month + ") ";
            DataTable dt = null;
            DBConnHelper _helper = new DBConnHelper();

            if (reportType.ToUpper() == "IPD")
            {
                dataEleClassQuery = "   (dataEleClass = 2 or dataEleClass = 3) ";
            }
            else if (reportType.ToUpper() == "OPD")
            {

            }
            else if (reportType.ToUpper() == "MONTHLYSERVICE") // more divisions like quarterly, yearly etc
            {
                dataEleClassQuery = "   ( dataEleClass = 6 ) "; //Only Monthly for now
            }

            string valuesQuery = " select labelId, dataEleClass, value from EthEhmis_HmisValue where " + idQuery + " and " + periodQuery + " and " +
            dataEleClassQuery;
            SqlCommand getValuesCmd = new SqlCommand(valuesQuery);
            dt = _helper.GetDataSet(getValuesCmd).Tables[0];


            return dt;
        }


        // http://localhost:49407/api/values/hierarchy?id1=1&id2=2&id3=3
        [Route("hierarchy")]
        [HttpGet]
        [EnableCors("*", "*", "*")]
        public string Gethierarchy(int id1,int id2)
        {
            return id1.ToString();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {            
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
