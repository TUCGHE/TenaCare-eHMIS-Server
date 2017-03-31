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
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using SqlManagement.Database;
using System.Web.Http.Cors;
using eHMIS.HMIS.DatabaseInterace;
using Admin.SiteSetting.DataAccess;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AddInstitutionController : ApiController
    {
        private Hashtable indicatorLocationIdValueHash = new Hashtable();
        private string FacilityTable = "EthEhmis_AllFacilityWithID";
        private string languageSet = LanguageController.languageSet;
   
        private void setCorrectLanguageTable()
        {
            LanguageController.setCorrectLanguageTable();
            
            FacilityTable = LanguageController.facilityTable;
            languageSet = LanguageController.languageSet;
           
        }

        public AddInstitutionController()
        {            
            // Get the language settings
            setCorrectLanguageTable();
        }

        [EnableCors("*", "*", "*")]
        public string Get()
        {
            string misgana = "Misgana";
            return misgana;
        }
        HmisDatabase _dbhelper = new HmisDatabase();

        [HttpGet]
        [EnableCors("*", "*", "*")]
        public IEnumerable<object> Get(int id, string hmisCode, string nextStep)
        {

            DBConnHelper _helper = new DBConnHelper();
            DataTable dt = new DataTable();
            DataTable institutionType = new DataTable();
            DataTable facilityOwner = new DataTable();
            DataTable woredas = new DataTable();
            ArrayList regionNames = new ArrayList();
            ArrayList zoneNames = new ArrayList();
            List<object> stringAndDataTable = new List<object>();
            string cmdText;
            SqlCommand toExecute;
            if (id == 1)
            {
                bool _noZone;
                int level = SiteSettingDataAccess.getAggregationLevelId(hmisCode);
                string locationId = hmisCode;
                if (level == 1) //if currently setted site is Federal
                {
                    stringAndDataTable.Clear();
                    stringAndDataTable.Add("Federal");
                    if (nextStep == "undefined")
                    {
                        cmdText = "Select FacilityName, HMISCode from " + FacilityTable + " where FacilityTypeId = 10 order by FacilityName";
                        toExecute = new SqlCommand(cmdText);
                        dt = _helper.GetDataSet(toExecute).Tables[0];

                        zoneNames = GetAllZonesInRegion(dt.Rows[0]["HMISCode"].ToString());
                        stringAndDataTable.Add(dt);

                    }
                    else
                    {
                        zoneNames = GetAllZonesInRegion(nextStep);
                    }


                    if (zoneNames.Count == 2)
                    {
                        stringAndDataTable.Add(zoneNames[0]);
                        stringAndDataTable.Add(zoneNames[1]);
                        _noZone = true;
                        stringAndDataTable.Add(setReportingLevels(hmisCode, _noZone));
                    }
                    else
                    {
                        stringAndDataTable.Add(zoneNames[0]);
                        _noZone = false;
                        stringAndDataTable.Add(setReportingLevels(hmisCode, _noZone));
                    }
                    stringAndDataTable.Add(FacilityOwner());
                    stringAndDataTable.Add(InstitutionType());


                }
                else if (level == 2)// if currently setted site is Region
                {
                    stringAndDataTable.Clear();
                    stringAndDataTable.Add("Region");
                    zoneNames = GetAllZonesInRegion(hmisCode);
                    if (zoneNames.Count == 2)
                    {
                        stringAndDataTable.Add(zoneNames[1]);
                        _noZone = true;
                        stringAndDataTable.Add(setReportingLevels(hmisCode, _noZone));
                    }
                    else
                    {
                        _noZone = false;
                        stringAndDataTable.Add(setReportingLevels(hmisCode, _noZone));
                    }

                    string reName = SiteSettingDataAccess.getFacilityName(hmisCode);
                    string[] region = new string[2];
                    region[0] = reName;
                    region[1] = hmisCode;

                    stringAndDataTable.Add(region);
                    stringAndDataTable.Add(zoneNames[0]);

                    stringAndDataTable.Add(FacilityOwner());
                    stringAndDataTable.Add(InstitutionType());

                }
                else if (level == 3)// if currently setted site  is woreda
                {

                    cmdText = "select RegionId, ReportingRegionName, ZoneId, ZoneName, HMISCode, WoredaName from " + FacilityTable + " where HMISCode = '" + hmisCode + "'";
                    toExecute = new SqlCommand(cmdText);
                    dt = _helper.GetDataSet(toExecute).Tables[0];
                    stringAndDataTable.Clear();
                    stringAndDataTable.Add("Woreda");
                    stringAndDataTable.Add(dt);
                    stringAndDataTable.Add(InstitutionType());
                    stringAndDataTable.Add(FacilityOwner());
                    stringAndDataTable.Add(setReportingLevels(hmisCode, false));


                }
                else if (level == 4) // if currently setted institution is Health Center
                {
                    cmdText = "select RegionId, ReportingRegionName, ZoneId, ZoneName, WoredaName, WoredaId, " +
                              " FacilityName, HMISCode from " + FacilityTable +
                              " where HMISCode = '" + hmisCode + "'";
                    toExecute = new SqlCommand(cmdText);
                    dt = _helper.GetDataSet(toExecute).Tables[0];
                    stringAndDataTable.Clear();
                    stringAndDataTable.Add("HealthCenter");
                    stringAndDataTable.Add(dt);
                    stringAndDataTable.Add("Health Post");//instution type 
                    stringAndDataTable.Add(HealthPostType());
                    stringAndDataTable.Add(FacilityOwner());
                    stringAndDataTable.Add("Reporting to Health Center");

                }
                else if (level == 5)
                {
                    cmdText = "select RegionId, ReportingRegionName, ZoneId, ZoneName from " +
                              FacilityTable + " where HMISCode = '" + hmisCode + "'";
                    toExecute = new SqlCommand(cmdText);
                    dt = _helper.GetDataSet(toExecute).Tables[0];

                    cmdText = "select FacilityName, HMISCode from " + FacilityTable +
                              " where ReportingDistrictId = '" + hmisCode + "' and FacilityTypeId = 8 ";
                    toExecute = new SqlCommand(cmdText);
                    woredas = _helper.GetDataSet(toExecute).Tables[0];

                    stringAndDataTable.Clear();
                    stringAndDataTable.Add("Zone");
                    stringAndDataTable.Add(dt);
                    stringAndDataTable.Add(woredas);
                    stringAndDataTable.Add(FacilityOwner());
                    stringAndDataTable.Add(InstitutionType());
                    stringAndDataTable.Add(setReportingLevels(hmisCode, false));

                }



            }
            else if (id == 4)//if Institution Type is selected
            {
                int facilityType = int.Parse(hmisCode);
                switch (facilityType)
                {
                    case 1:
                    case 5:
                    case 7:
                        dt = HospitalType();
                        stringAndDataTable.Clear();
                        stringAndDataTable.Add("Hospital");
                        stringAndDataTable.Add(dt);
                        break;
                    case 2:
                        dt = HealthCenterType();
                        stringAndDataTable.Clear();
                        stringAndDataTable.Add("Health Center Type");
                        stringAndDataTable.Add(dt);
                        break;
                    case 3:
                        dt = HealthPostType();
                        stringAndDataTable.Clear();
                        stringAndDataTable.Add("Health Post Type");
                        stringAndDataTable.Add(dt);
                        stringAndDataTable.Add("Reporting to Health Center");
                        DataTable dtHealthCenter = GetAllHCInWoreda(nextStep);
                        stringAndDataTable.Add(dtHealthCenter);
                        break;





                }
            }





            return stringAndDataTable;

        }
        [HttpPost]
        [EnableCors("*", "*", "*")]
        public bool Post(int id, [FromBody]NewInstitution values)
        {

            HmisDatabase._regionId = values.Region;
            HmisDatabase._woredaId = Convert.ToInt32(values.Woreda);
            HmisDatabase._zoneId = Convert.ToInt32(values.Zone);

            HmisDatabase.setRegionId = values.Region;
            HmisDatabase.setZoneId = Convert.ToInt32(values.Zone);
            HmisDatabase.setWoredaId = Convert.ToInt32(values.Woreda);

            if (values.HPChosen)
            {
                HmisDatabase.setWoredaId = int.Parse(Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getDistrictId(values.ReportingLevel.ToString()));
            }
            if (values.ReportingLevel == 0)
            {
                HmisDatabase.setWoredaId = 20;
            }
            if (values.ReportingLevel == 1)
            {
                HmisDatabase.setWoredaId = HmisDatabase.setRegionId;
            }
            if (values.ReportingLevel == 2)
            {
                HmisDatabase.setRegionId = HmisDatabase.setZoneId;
            }

            int healthCentreId = 0;
            int hospitalId = 0;
            int reportingLevel = -1;
            int facilityId = 0;
            int facilityOwnerId = 0;

            facilityId = values.InstitutionType;
            facilityOwnerId = values.FacilityOwner;
            if (values.HCChosen)
            {
                healthCentreId = values.HealthType;
            }
            if (values.HospitalChosen)
            {
                hospitalId = values.HealthType;
            }
            if (values.HPChosen)
            {
                healthCentreId = values.HealthType;
            }
            string FName = UppercaseFirst(values.InstitutionName.Trim());
            bool flag;
            if (values.HPChosen)
            {
                if (_dbhelper.insertIntoFacility(FName, facilityId, facilityOwnerId, healthCentreId, hospitalId, reportingLevel,
                    values.ReportingLevel))
                {
                    //string msg = "You have successfully added facility:  " + FName;  
                    flag = true;
                    Refresh_EthEhmis_AllFacilityWithID();
                }
                else
                {
                    //string msg = " Facility adding can not happen in this Woreda, or this facility already exists. Please talk to " +
                    //  " system administrators for software upgrade ";
                    flag = false;
                }
            }
            else
            {
                reportingLevel = values.ReportingLevel;
                if (_dbhelper.insertIntoFacility(FName, facilityId, facilityOwnerId, healthCentreId, hospitalId, reportingLevel))
                {
                    //string msg = "You have successfully added facility:  " + FName; 
                    flag = true;
                    Refresh_EthEhmis_AllFacilityWithID();
                }
                else
                {
                    //string msg = " Facility adding can not happen in this Woreda, or this facility already exists. Please talk to " +
                    // " system administrators for software upgrade ";   
                    flag = false;
                }
            }
            return flag;
        }
        private string UppercaseFirst(string s)
        {
            char[] arr = s.ToCharArray();
            arr[0] = char.ToUpper(arr[0]);
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] == ' ' || arr[i] == '_' || arr[i] == '-')
                {
                    arr[i + 1] = char.ToUpper(arr[i + 1]);
                }
            }
            return new string(arr);
        }
        public ArrayList GetAllZonesInRegion(string hmisCode)
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = " select FacilityName, HMISCode from " + FacilityTable +
                             " where ReportingDistrictId = " + hmisCode + " and FacilityTypeId = 9";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            ArrayList listOfZones = new ArrayList();

            if (dt.Rows.Count <= 0)// No zones under the region
            {
                cmdText = " select FacilityName, HMISCode from " + FacilityTable + 
                          " where RegionId = " + hmisCode + " and FacilityTypeId = 8";
                toExecute = new SqlCommand(cmdText);
                dt = _helper.GetDataSet(toExecute).Tables[0];
                listOfZones.Add(dt);
                listOfZones.Add("No Zones for this Region");

            }
            else
            {
                listOfZones.Add(dt);
            }
            return listOfZones;

        }
        public DataTable GetAllHCInWoreda(string woredaID)
        {
            DBConnHelper _helper = new DBConnHelper();
            //string hmiscode = GetFacilityIDFromName(woredaname);
            string cmdText = " select FacilityName, HMISCode from  " + FacilityTable +
                             " where DistrictID ='" + woredaID + "' and FacilityTypeId = 2 order by FacilityName";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            return dt;

        }
        public DataTable FacilityOwner()
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "select * from FacilityOwner";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            return dt;
        }
        public DataTable InstitutionType()
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "select * from FacilityType where FacilityTypeId in (1, 2, 3, 4, 5, 6, 7, 50, 51, 52, 53, 54)";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            return dt;
        }
        public DataTable HospitalType()
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "select * from HospitalType";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            return dt;
        }
        public DataTable HealthCenterType()
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "select * from HealthCentreType";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            return dt;
        }
        public DataTable HealthPostType()
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "select * from HealthPostType";
            SqlCommand toExecute = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            return dt;
        }
        public void Refresh_EthEhmis_AllFacilityWithID()
        {
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "truncate table EthEhmis_AllFacilityWithID insert into EthEhmis_AllFacilityWithID select * from v_EthEhmis_AllFacilityWithID";
            SqlCommand toExecute = new SqlCommand(cmdText);
            _helper.Execute(toExecute);

        }
        private ArrayList setReportingLevels(string hmisCode, bool _noZone)
        {
            ArrayList reportingLevel = new ArrayList();
            int level = SiteSettingDataAccess.getAggregationLevelId(hmisCode);
            HmisDatabase.ReportingLevelNameHash.Clear();
            string[] values = new string[2];
            if (level == 1) // Federal
            {
                reportingLevel.Add("Federal");
                reportingLevel.Add("Regional");

                if (_noZone == false)
                {
                    reportingLevel.Add("Zonal");
                    HmisDatabase.ReportingLevelNameHash.Add("Zonal", 2);
                }

                //if (_noWoreda == false)
                //{
                reportingLevel.Add("Woreda");
                // cmbReportingLevel.Items.Add("Woreda");
                HmisDatabase.ReportingLevelNameHash.Add("Woreda", 3);
                //}

                HmisDatabase.ReportingLevelNameHash.Add("Federal", 0);
                HmisDatabase.ReportingLevelNameHash.Add("Regional", 1);
            }
            else if (level == 2) //Regional
            {
                reportingLevel.Add("Regional");

                if (_noZone == false)
                {
                    reportingLevel.Add("Zonal");
                    HmisDatabase.ReportingLevelNameHash.Add("Zonal", 2);
                }

                //if (_noWoreda == false)
                //{
                reportingLevel.Add("Woreda");
                HmisDatabase.ReportingLevelNameHash.Add("Woreda", 3);
                //}

                HmisDatabase.ReportingLevelNameHash.Add("Regional", 1);
            }
            else if (level == 5) // Zonal
            {
                reportingLevel.Add("Zonal");

                //if (_noWoreda == false)
                //{
                reportingLevel.Add("Woreda");
                HmisDatabase.ReportingLevelNameHash.Add("Woreda", 3);
                //}

                HmisDatabase.ReportingLevelNameHash.Add("Zonal", 2);

            }
            else if (level == 3)// Woreda
            {
                reportingLevel.Add("Woreda");
                HmisDatabase.ReportingLevelNameHash.Add("Woreda", 3);
            }

            return reportingLevel;
        }

    }
    public class NewInstitution
    {
        public string InstitutionName { get; set; }
        public int Region { get; set; }
        public string Zone { get; set; }
        public string Woreda { get; set; }
        public int InstitutionType { get; set; }
        public int FacilityOwner { get; set; }
        public int ReportingLevel { get; set; }
        public int HealthType { get; set; }
        public bool HospitalChosen { get; set; }
        public bool HCChosen { get; set; }
        public bool HPChosen { get; set; }
    }
}
