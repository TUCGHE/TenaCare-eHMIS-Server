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

namespace eHMISWebApi.Controllers
{
    [RoutePrefix("api/Location")]
    [EnableCors("*", "*", "*")]
    public class LocationController : ApiController
    {
        int federalHmiscode = 88;
        private string FacilityTable = "EthEhmis_AllFacilityWithID";
        private string languageSet = LanguageController.languageSet;

        public LocationController()
        {
            // Get the language settings
            //DBConnHelper _helper = new DBConnHelper();
            //string cmdText = " select language from languageSetting where languageSet = 1";
            //SqlCommand languageCmd = new SqlCommand(cmdText);

            //DataTable dt = _helper.GetDataSet(languageCmd).Tables[0];

            //foreach (DataRow row in dt.Rows)
            //{
            //    languageSet = row["language"].ToString();
            //}

            languageSet = LanguageController.languageSet;

            if (languageSet == string.Empty)
            {
                languageSet = "english";
            }

            if (languageSet != "english")
            {
                FacilityTable = FacilityTable + languageSet;
            }
        }
        // GET: api/Location
        [EnableCors("*", "*", "*")]
               public DataTable Get()
        //public IEnumerable<DataTable> Get(int regionId, int facilityType)
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "select ReportingDistrictId as ParentId, ReportingAdminSite as ParentName, " +
                             " HMISCode as ChildId, FacilityName as ChildName,  " +
                             " facilityTypeId as FacilityType " +
                             " from " + FacilityTable +
                             " where ReportingDistrictId = " + federalHmiscode +
                             "  and hmiscode != " + federalHmiscode +
                             " order by FacilityTypeId, FacilityName ";

            SqlCommand getFacilityCmd = new SqlCommand(cmdText);
            List<DataTable> dtTableCollection = new List<DataTable>();
            DataTable dt = _helper.GetDataSet(getFacilityCmd).Tables[0];
            dtTableCollection.Add(dt);

            return dt;
            //return dtTableCollection;

            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [EnableCors("*", "*", "*")]
        public DataTable Get(int id)
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = " select hmiscode, '1' as Value from facility " +
                             " where hmiscode in " +
                             " (select cast(ReportingDistrictId as varchar(1000)) from " + FacilityTable + ") ";

            SqlCommand getFacilityCmd = new SqlCommand(cmdText);
            List<DataTable> dtTableCollection = new List<DataTable>();
            DataTable dt = _helper.GetDataSet(getFacilityCmd).Tables[0];
            dtTableCollection.Add(dt);

            return dt;                       
        }

        // GET: api/Location/5
        [EnableCors("*", "*", "*")]
        public DataTable Get(int id, string locationId)
        {
            DataTable dt = null;
            // id = 1 get the current set locationId
            // id = 2 get you the hiearchy...
            // locationId is the id of the location you are getting
            DBConnHelper _helper = new DBConnHelper();
            SqlCommand facilitySqlCmd = null;
            List<object> stringAndDataTable = new List<object>();

            string cmdText = "";
            if (id == 1)
            {
                cmdText = "select value as LocationId from setting where name = 'hmiscode' ";

                facilitySqlCmd = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(facilitySqlCmd).Tables[0];

                //stringAndDataTable.Add(dt);
            }
            else if (id == 2) // get the hierarchy
            {
                // Joe: prevent exception?
                if(locationId == "undefined")
                {
                    return new DataTable();
                }

                cmdText = "select * from " + FacilityTable +
                                 " where hmiscode = '" + locationId + "'";                            


                facilitySqlCmd = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(facilitySqlCmd).Tables[0];

                //stringAndDataTable.Add(dt);
            }  
            else if (id == 3)  // save selected location
            {
                // Joe: prevent exception?
                if (locationId == "undefined")
                {
                    return new DataTable();
                }

                cmdText = "select top 1 * from " + FacilityTable +
                                 " where hmiscode = '" + locationId + "'";

                facilitySqlCmd = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(facilitySqlCmd).Tables[0];

                //stringAndDataTable.Add(dt);
                Admin.SiteSetting.DataAccess.SiteSettingDataAccess.SaveSiteSetting(locationId);
                
            }
            else if (id == 4) // Get all children
            {
                
                if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityTypeId(locationId) == 3)
                {
                    dt = new DataTable();
                }
                else
                {
                    // Joe: prevent exception?
                    if (locationId == "undefined")
                    {
                        return new DataTable();
                    }

                    cmdText = "select ReportingDistrictId as ParentId, ReportingAdminSite as ParentName, " +
                               " HMISCode as ChildId, FacilityName as ChildName, " +
                               " facilityTypeId as FacilityType " +
                               " from " + FacilityTable +
                               " where reportingDistrictId = '" + locationId + "'" + " and hmiscode != " +
                               federalHmiscode +
                               " order by FacilityTypeId, FacilityName ";

                    SqlCommand getFacilityCmd = new SqlCommand(cmdText);
                    dt = _helper.GetDataSet(getFacilityCmd).Tables[0];                    
                }
                //stringAndDataTable.Add(dt);
            }
           

            return dt;
        }

        // GET: api/Location/5
        [EnableCors("*", "*", "*")]       
        public List<object> Get(int id, int id2, string facilityName)
        {
            DataTable dt = new DataTable();
            List<object> stringAndDataTable = new List<object>();
            // id = 1 get the current set locationId
            // id = 2 get you the hiearchy...
            // locationId is the id of the location you are getting
            if ((facilityName != "") && (facilityName != null))
            {
                DBConnHelper _helper = new DBConnHelper();
                SqlCommand facilitySqlCmd = null;               

                string cmdText = " select FacilityName, FacilityTypeName, HMISCode as FacilityTypeId, " +
                              " reportingadminsite as ParentInstitution, " +
                              " WoredaName, ZoneName, ReportingRegionName as RegionName " +
                              " from " + FacilityTable +
                              " where facilityname like '%" + facilityName + "%'";

                facilitySqlCmd = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(facilitySqlCmd).Tables[0];

                string[] columnNames = dt.Columns.Cast<DataColumn>()
                                .Select(x => x.ColumnName)
                                .ToArray();

                stringAndDataTable.Add(columnNames);
                stringAndDataTable.Add(dt);
            }
            return stringAndDataTable;
        }

        // POST: api/Location
        public void Post([FromBody]string value)
        {
            Admin.SiteSetting.DataAccess.SiteSettingDataAccess.SaveSiteSetting(value);
        }

        // PUT: api/Location/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Location/5
        public void Delete(int id)
        {
        }
    }
}
