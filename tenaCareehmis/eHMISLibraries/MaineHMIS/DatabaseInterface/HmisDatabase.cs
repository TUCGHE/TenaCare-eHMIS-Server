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
using System.Data;
using System.IO;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SqlManagement.Database;

namespace eHMIS.HMIS.DatabaseInterace
{
    public class HmisDatabase
    {
        public static string _hmisValueTable = "EthEhmis_HMISValue";
        public DBConnHelper _helper = new DBConnHelper();
        private int _currentDistrictId;

        public int CurrentDistrictId
        {
            get { return _currentDistrictId; }
            set { _currentDistrictId = value; }
        }
        private string _currentDistrictFacilityId = "";
        private string _HMISCode = "";
        private string _HMISDistrict = "";
        private string _facilityName = "";
        private string _altFacilityName;
        private int _aggregationLevelId;
        private int _facilityOwnerId;
        private int _healthCentreTypeId;
        private int _healthPostTypeId;
        private int _hospitalTypeId;
        private int _facilityTypeId;
        private int _laboratoryLevelId;
        private int _locationDistrictId;

        public static int _federalId = 20;
        public static int _regionId;
        public static int _woredaId;
        public static int _zoneId;
        //private string _facilityId;

        public static Hashtable facilityIdNamesHash = new Hashtable();
        public static Hashtable facilityOwnerIdNamesHash = new Hashtable();
        public static Hashtable HealthPostIdNamesHash = new Hashtable();
        public static Hashtable HealthCentreIdNamesHash = new Hashtable();
        public static Hashtable HospitalIdNamesHash = new Hashtable();
        public static Hashtable ReportingLevelNameHash = new Hashtable();

        public static ArrayList RealWoredaNames = new ArrayList();
        public static ArrayList RealWoredaIDs = new ArrayList();


        private int level = HMISMainPage.SelectedLocationLevel;

        public static int setWoredaId = 0;
        public static int setRegionId = 0;
        public static int setZoneId = 0;

        public static string hmisValueTable
        {
            get { return _hmisValueTable; }
            set { _hmisValueTable = value; }
        }


        public static void reset_hmisValueTable()
        {
            hmisValueTable = "EthEhmis_HMISValue";
        }
        public HmisDatabase()
        {
            /*if (eHMIS.UI.HEWFamilyFolder.First.familyFolder == true)
            {
                HMISMainPage.SelectedLocationID = eHMIS.UI.HEWFamilyFolder.First._hmisCode;
                level = eHMIS.UI.HEWFamilyFolder.First.currentLocationLevel;
                _regionId = eHMIS.UI.HEWFamilyFolder.First._regionId;
                _zoneId = eHMIS.UI.HEWFamilyFolder.First._zoneId;
                _woredaId = eHMIS.UI.HEWFamilyFolder.First._woredaId;
                _locationDistrictId = eHMIS.UI.HEWFamilyFolder.First._locationDistrictId;
            }
            */
            setHmisDatabase();
        }

        public void setHmisDatabase()
        {

            //if (level == 0) // Facility
            //{
            //}
            //else if (level == 1) // Woreda
            //{
            //}
            //else if (level == 2) // Zone
            //{
            //}
            //else if (level == 3) // Region
            //{
            //}
            //else if (level == 4) // Federal
            //{
            //}

            /* if (eHMIS.UI.HEWFamilyFolder.First.familyFolder != true)
             {*/
            string mainHmisCode = "";
            
                mainHmisCode = _helper.GetSetting("HmisCode", "string").ToString();
                _regionId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(mainHmisCode);
                _woredaId = Convert.ToInt32(Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getDistrictId(mainHmisCode));
                _zoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(mainHmisCode);
            


            //}


            //_selectedZoneID = _selectedRegionID;

            //string seleLocaInfor = " SELECT [ZONEID] " +
            //          " FROM [dbo].[EthEhmis_HMISZoneDistrict] where DistrictSeq = '" + _woredaId + "'";


            //DataSet ds = _helper.GetDataSet(seleLocaInfor);

            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    _zoneId = Convert.ToInt32(ds.Tables[0].Rows[0]["ZONEID"]);
            //}
            //else
            //{
            //    _zoneId = HMISMainPage.SelectedZoneID;
            //}

            // _regionId = HMISMainPage.SelectedRegionID;

            //  _woredaId = HMISMainPage.SelectedWoredaID;


        }

        public string getRegionName()
        {
            // _regionId = HMISMainPage.SelectedRegionID;

            string regionName = "";
            string cmdText = "";

            cmdText = " SELECT name " +
                     " FROM province where code = " + _regionId + " order by [name]";

            DataSet ds = _helper.GetDataSet(cmdText);

            if (ds.Tables[0].Rows.Count > 0)
            {
                regionName = ds.Tables[0].Rows[0]["Name"].ToString();
            }
            return regionName;
        }

        public ArrayList getRegionNames()
        {
            ArrayList regionList = new ArrayList();
            string regionName = "";
            string cmdText = "";

            if (level == 4)
            {
                cmdText = " SELECT name " +
                         " FROM province where name != 'Federal' order by [name]";
            }
            else
            {
                cmdText = " SELECT name " +
                         " FROM province where code = " + _regionId + " order by [name]";
            }


            DataSet ds = _helper.GetDataSet(cmdText);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                regionName = row["Name"].ToString();
                regionList.Add(regionName);
            }
            return regionList;
        }


        public int getRegionId(string name)
        {
            //int regionId = 0;


            string cmdText = " SELECT code " +
                         " FROM province where name = @name";

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.Parameters.AddWithValue("name", name);

            DataSet ds = _helper.GetDataSet(toExecute);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                _regionId = Convert.ToInt32(row["Code"]);
            }

            //switch (_regionId)
            //{
            //    case 12:
            //        _regionId = 8;
            //        break;
            //    case 13:
            //        _regionId = 9;
            //        break;
            //    case 14:
            //        _regionId = 10;
            //        break;
            //    case 15:
            //        _regionId = 11;
            //        break;
            //    default:
            //        break;
            //}

            return _regionId;
        }

        public string getZoneName()
        {

            string zoneName = "";

            if (_zoneId == 0)
            {
                if (level == 2)
                {
                    _zoneId = Convert.ToInt32(HMISMainPage.SelectedLocationID);
                }
                else //if (level == 0)
                {
                    _zoneId = HMISMainPage.SelectedZoneID;
                }
            }

            string cmdText = " SELECT ZoneName " +
                     " FROM EthEhmis_HmisZone where zoneid = " + _zoneId + " order by [ZoneName]";

            DataSet ds = _helper.GetDataSet(cmdText);

            if (ds.Tables[0].Rows.Count > 0)
            {
                zoneName = ds.Tables[0].Rows[0]["ZoneName"].ToString();
            }


            return zoneName;
        }

        public ArrayList getZoneNames(int regionId)
        {
            ArrayList zoneList = new ArrayList();
            string zoneName = "";

            string cmdText = " SELECT ZoneName " +
                     " FROM EthEhmis_HmisZone where regionId = " + regionId + " order by [ZoneName]";

            DataSet ds = _helper.GetDataSet(cmdText);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                zoneName = row["ZoneName"].ToString();
                zoneList.Add(zoneName);
            }

            return zoneList;
        }

        public int getZoneId(string name)
        {
            //int zoneId = 0;

            string cmdText = " SELECT ZoneId " +
                     " FROM EthEhmis_HmisZone where ZoneName = @name";

            SqlCommand toExecute = new SqlCommand(cmdText);
            toExecute.Parameters.AddWithValue("name", name);

            DataSet ds = _helper.GetDataSet(toExecute);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                _zoneId = Convert.ToInt32(row["ZoneId"]);
            }

            return _zoneId;
        }


        public ArrayList getWoredaNames(int zoneId)
        {
            ArrayList woredaList = new ArrayList();
            string woredaName = "";


            string cmdText = " select district.name from district " +
                             " inner join " +
                             " (select EthEhmis_HmisZoneDistrict.districtseq from EthEhmis_HmisZoneDistrict " +
                             " where EthEhmis_HmisZoneDistrict.zoneid = @zoneId) as test " +
                             " on district.districtseq = test.districtSeq  ORDER BY [name]";

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("zoneId", zoneId);

            DataSet ds = _helper.GetDataSet(toExecute);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                woredaName = row["Name"].ToString();
                woredaList.Add(woredaName);
            }

            return woredaList;
        }

        public ArrayList getWoredaNamesFromRegion(int regionID)
        {
            ArrayList woredaList = new ArrayList();
            string woredaName = "";
           
            string cmdText = " select district.name from district " +
                             " where provinceCode = @regionID   ORDER BY [name]";

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("regionID", regionID);

            DataSet ds = _helper.GetDataSet(toExecute);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                woredaName = row["Name"].ToString();
                woredaList.Add(woredaName);
            }

            return woredaList;
        }


        public void setRealWoredaNames()
        {
            string woredaName = "";
            int woredaID;

            if (RealWoredaIDs.Count == 0)
            {           

                string cmdText = "select * from district where notworeda = 0 ";


                DataSet ds = _helper.GetDataSet(cmdText);


                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    woredaName = row["Name"].ToString();
                    woredaID = int.Parse(row["DistrictSeq"].ToString());
                    RealWoredaNames.Add(woredaName);
                    RealWoredaIDs.Add(woredaID);
                }
            }

            //return RealWoredaID;
        }


        public string getWoredaName()
        {
            string woredaName = "";

            _woredaId = HMISMainPage.SelectedWoredaID;

            if (_regionId == _woredaId)
            {
                _woredaId = HMISMainPage.SelectedWoredaID;
            }

            string cmdText = " SELECT Name " +
                     " FROM District where districtSeq = " + _woredaId + "  ORDER BY [Name]";

            DataSet ds = _helper.GetDataSet(cmdText);

            if (ds.Tables[0].Rows.Count > 0)
            {
                woredaName = ds.Tables[0].Rows[0]["Name"].ToString();
            }

            return woredaName;
        }

        public int getWoredaId(string name)
        {           
            string cmdText = "";
            int woredaId = 0;

            if (_regionId == 20) // Federal thus search for all woredas regardless of regionid
            {

                cmdText = " SELECT * " +
                         " FROM District where name = @name";
            }
            else
            {
                cmdText = " SELECT * " +
                         " FROM District where name = @name and provinceCode = @regionID";
            }

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("name", name);
            toExecute.Parameters.AddWithValue("regionID", _regionId);

            DataSet ds = _helper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 1)
            {
                // Same Woreda in that Region, sort by using Zones
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    woredaId = Convert.ToInt32(row["districtSeq"]);
                    int myZoneId = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getZoneId(woredaId.ToString());
                    if (myZoneId == _zoneId)
                    {
                        _woredaId = woredaId;
                    }
                }
            }
            else if (ds.Tables[0].Rows.Count > 0)
            {
                woredaId = Convert.ToInt32(ds.Tables[0].Rows[0]["districtSeq"]);
                _woredaId = woredaId;
            }

            return woredaId;
        }


        public ArrayList getFacilityTypes()
        {
            ArrayList facilityTypes = new ArrayList();
            int facilityTypeId;
            string facilityTypeName = "";

            // string cmdText = "select * from facilitytype where facilitytypeid <=6 ";
            string cmdText = "select distinct * from facilitytype";

            DataSet ds = _helper.GetDataSet(cmdText);

            facilityTypes.Clear();
            facilityIdNamesHash.Clear();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                facilityTypeName = row["FacilityTypeName"].ToString();
                facilityTypeId = Convert.ToInt32(row["FacilityTypeId"]);
                facilityTypes.Add(facilityTypeName);
                facilityIdNamesHash.Add(facilityTypeName, facilityTypeId);
            }

            return facilityTypes;

        }

        public ArrayList getFacilityOwners()
        {
            ArrayList facilityOwners = new ArrayList();
            int facilityOwnerId;
            string facilityOwnerName = "";

            // string cmdText = "select * from facilitytype where facilitytypeid <=6 ";
            string cmdText = "select * from facilityOwner";

            DataSet ds = _helper.GetDataSet(cmdText);

            facilityOwners.Clear();
            facilityOwnerIdNamesHash.Clear();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                facilityOwnerName = row["FacilityOwnerName"].ToString();
                facilityOwnerId = Convert.ToInt32(row["FacilityOwnerId"]);
                facilityOwners.Add(facilityOwnerName);
                facilityOwnerIdNamesHash.Add(facilityOwnerName, facilityOwnerId);
            }

            return facilityOwners;

        }

        public ArrayList getHealthCentreTypes()
        {
            ArrayList healthCenterTypes = new ArrayList();
            int HealthCentreTypeId;
            string HealthCentreTypeName = "";

            string cmdText = " select * from healthcentretype ";

            DataSet ds = _helper.GetDataSet(cmdText);

            healthCenterTypes.Clear();
            HealthCentreIdNamesHash.Clear();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                HealthCentreTypeName = row["HealthCentreTypeName"].ToString();
                HealthCentreTypeId = Convert.ToInt32(row["HealthCentreTypeId"]);
                healthCenterTypes.Add(HealthCentreTypeName);
                HealthCentreIdNamesHash.Add(HealthCentreTypeName, HealthCentreTypeId);
            }

            return healthCenterTypes;
        }

        public ArrayList getHealthPostTypes()
        {
            ArrayList healthPostTypes = new ArrayList();
            int HealthPostTypeId;
            string HealthPostTypeName = "";

            string cmdText = " select * from healthposttype ";

            DataSet ds = _helper.GetDataSet(cmdText);

            healthPostTypes.Clear();
            HealthPostIdNamesHash.Clear();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                HealthPostTypeName = row["HealthPostTypeName"].ToString();
                HealthPostTypeId = Convert.ToInt32(row["HealthPostTypeId"]);
                healthPostTypes.Add(HealthPostTypeName);
                HealthPostIdNamesHash.Add(HealthPostTypeName, HealthPostTypeId);
            }

            return healthPostTypes;
        }

        public ArrayList getHospitalTypes()
        {
            ArrayList hospitalTypes = new ArrayList();
            int HospitalTypeId;
            string HospitalTypeName = "";

            string cmdText = " select * from HospitalType ";

            DataSet ds = _helper.GetDataSet(cmdText);

            hospitalTypes.Clear();
            HospitalIdNamesHash.Clear();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                HospitalTypeName = row["HospitalTypeName"].ToString();
                HospitalTypeId = Convert.ToInt32(row["HospitalTypeId"]);
                hospitalTypes.Add(HospitalTypeName);
                HospitalIdNamesHash.Add(HospitalTypeName, HospitalTypeId);
            }

            return hospitalTypes;
        }


        private void setfacilityId(int hospitalLevel)
        {
            ArrayList facilityIdLists = new ArrayList();
            string facilityId = "";
            string districtFacilityId = "";
           
            string cmdText = "select max(cast(hmiscode as integer)) as HMISCode from facility where districtId = @districtId";

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("districtId", setWoredaId);

            DataSet ds = _helper.GetDataSet(toExecute);
            int newFacilityId = -1;
            string asciiFacilityID = "";

            facilityId = ds.Tables[0].Rows[0]["HMISCode"].ToString();

            //if (ds.Tables[0].Rows.Count > 0)
            if (facilityId != "")
            {               

                if (int.TryParse(facilityId, out newFacilityId))
                {
                    newFacilityId = newFacilityId + 1;
                    _HMISCode = newFacilityId.ToString();
                }
                else
                {
                    System.Text.Encoding ascii = System.Text.Encoding.ASCII;
                    Byte[] encodedBytes = ascii.GetBytes(facilityId);

                    int length = encodedBytes.Length;

                    int lastValue = (int)encodedBytes[length - 1] + 1;

                    encodedBytes[length - 1] = (byte)lastValue;

                    // string newFacilityId = "";

                    foreach (char b in encodedBytes)
                    {
                        asciiFacilityID += b;
                    }

                    _HMISCode = asciiFacilityID.ToString();

                }

                //facilityIdLists.Add(facilityId);
            }
            else
            {
                // We would assume that this would be a new Facility, in that District
                // So HMISCode would be
                newFacilityId = 1;
                _HMISCode = newFacilityId.ToString();
            }


            // If HmisCode exists then add one number to it and re-check

            checkIfHmisCodeExists();

            cmdText = "select DistrictFacilityId as DistrictFacilityId from facility where districtid = @districtId and AggregationLevelId = 4 and DistrictFacilityId != '10M'";
          
            string maxDistrictFacilityID = "select max(cast (DistrictFacilityId as int)) as DistrictFacilityId from facility where districtid = @districtId and AggregationLevelId = 4 and DistrictFacilityId != '10M'";

            // To resolve issue when districtFacilityID
            toExecute = new SqlCommand(cmdText);

            SqlCommand toExecute2 = new SqlCommand(maxDistrictFacilityID);

            switch (hospitalLevel)
            {
                case 0: //Federal
                    toExecute.Parameters.AddWithValue("districtId", _federalId);
                    toExecute2.Parameters.AddWithValue("districtId", _federalId);
                    _currentDistrictId = _federalId;
                    break;
                case 1:
                    toExecute.Parameters.AddWithValue("districtId", _regionId);
                    toExecute2.Parameters.AddWithValue("districtId", _regionId);
                    _currentDistrictId = _regionId;
                    break;
                case 2:
                    toExecute.Parameters.AddWithValue("districtId", _zoneId);
                    toExecute2.Parameters.AddWithValue("districtId", _zoneId);
                    _currentDistrictId = _zoneId;
                    break;
                case 3:
                    toExecute.Parameters.AddWithValue("districtId", _woredaId);
                    toExecute2.Parameters.AddWithValue("districtId", _woredaId);
                    //_currentDistrictId = _woredaId;
                    break;
                default:
                    toExecute.Parameters.AddWithValue("districtId", _woredaId);
                    toExecute2.Parameters.AddWithValue("districtId", _woredaId);
                    //_currentDistrictId = _woredaId;
                    break;
            }

            ds = _helper.GetDataSet(toExecute);

            //


            int newDistrictFacilityId = -1;

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataSet ds1 = _helper.GetDataSet(toExecute2);

                districtFacilityId = ds1.Tables[0].Rows[0]["DistrictFacilityId"].ToString();

                if (int.TryParse(districtFacilityId, out newDistrictFacilityId))
                {
                    newDistrictFacilityId = newDistrictFacilityId + 1;
                }
                else
                {
                    newDistrictFacilityId = -1;
                }

                //facilityIdLists.Add(facilityId);
            }
            else // Could be a new Facility
            {
                //newDistrictFacilityId = -1;
                newDistrictFacilityId = 1;
            }


            _currentDistrictFacilityId = newDistrictFacilityId.ToString();
        }

        private void checkIfHmisCodeExists()
        {
            string cmdText = "select HMISCode from facility where HMISCode = '" + _HMISCode + "'";

            SqlCommand toExecute = new SqlCommand(cmdText);            

            DataSet ds = _helper.GetDataSet(toExecute);            

            if (ds.Tables[0].Rows.Count > 0)
            {
                int code;
                code = Convert.ToInt32(_HMISCode);
                code = code + 1;
                _HMISCode = code.ToString();
                checkIfHmisCodeExists();
            }

        }

        private Boolean setFacilityValues(int hospitalLevel)
        {
            Boolean correctValue = true;
            _currentDistrictId = _woredaId;
            _locationDistrictId = _currentDistrictId;
            //_currentDistrictFacilityId = "";
            // _HMISCode = "ABC47655";
            setfacilityId(hospitalLevel);

            if (_HMISCode == "-1")
            {
                correctValue = false;
            }
            else if (_currentDistrictFacilityId == "-1")
            {
                correctValue = false;
            }

            _HMISDistrict = "9999";
            _facilityName = "";// need to set it after the user enter the Name
            _altFacilityName = ""; //  need to set it after the user enter the Name
            _aggregationLevelId = 4; // We are adding a facility
            _facilityOwnerId = 1;
            _healthCentreTypeId = 0;
            _facilityTypeId = 1;
            _laboratoryLevelId = 6;

            return correctValue;
        }

        private int GetNextHealthPostId(int currentDistrictID)
        {
            string sql = "select max(DistrictFacilityId)+1 from  dbo.Facility  where  DistrictId=" + currentDistrictID +
                "group by DistrictId";
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            object nid = helper.GetScalar(sql);
            int nextid = nid != null ? Convert.ToInt32(nid) : 1;
            return nextid;
        }
       
        public Boolean insertIntoFacility(string facilityName, int facilityTypeId, int facilityOwnerId, int healthCentreTypeId,
            int hospitalId, int HospitalLevel, int currentDId)
        {
            Boolean success = true;
            if (setFacilityValues(HospitalLevel))
            {

                if (hospitalId != 0)
                {
                    switch (HospitalLevel) // To indicate for the 3 Administrative levels
                    {
                        case 0: // Federal Hospitals
                            _currentDistrictId = _federalId;
                            break;
                        case 1: // Regional Hospitals
                            _currentDistrictId = _regionId;
                            break;
                        case 2: // Zonal Hospitals
                            _currentDistrictId = _zoneId;
                            break;
                        default:
                            break;
                    }
                }
                int districtFacilityId = GetNextHealthPostId(currentDId);
                _currentDistrictId = currentDId;
                _facilityName = facilityName;
                _altFacilityName = facilityName;
                _facilityTypeId = facilityTypeId;
                _facilityOwnerId = facilityOwnerId;
                _healthCentreTypeId = healthCentreTypeId;
                _hospitalTypeId = hospitalId;

                if (!isThisFacilityExists(_currentDistrictId, _facilityName, _facilityTypeId))
                {
                    string cmdText = " INSERT INTO Facility " +
                   " (DistrictId, DistrictFacilityId, HMISCode, HMISDistrict, FacilityName " +
                   " ,AltFacilityName, AggregationLevelId, FacilityOwnerId, HealthCentreTypeId " +
                   " ,FacilityTypeId, LaboratoryLevelId, HospitalTypeId, HealthPostTypeId, LocationDistrictId) " +
                   " VALUES " +
                   " (@DistrictId, @DistrictFacilityId, @HMISCode, @HMISDistrict, " +
                   " @FacilityName, @AltFacilityName, @AggregationLevelId, @FacilityOwnerId, " +
                   " @HealthCentreTypeId, @FacilityTypeId, @LaboratoryLevelId, " +
                   " @HospitalTypeId, @HealthCentreTypeId, @LocationDistrictId )";

                    SqlCommand toExecute;

                    //if (_locationDistrictId == 0)
                    //{
                    _locationDistrictId = int.Parse(Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getLocationDistrictId(_currentDistrictId.ToString()));
                    // }

                    toExecute = new SqlCommand(cmdText);
                    toExecute.Parameters.AddWithValue("DistrictId", _currentDistrictId);
                    toExecute.Parameters.AddWithValue("LocationDistrictId", _locationDistrictId);
                    toExecute.Parameters.AddWithValue("DistrictFacilityId", districtFacilityId);
                    toExecute.Parameters.AddWithValue("HMISCode", _HMISCode);
                    toExecute.Parameters.AddWithValue("HMISDistrict", _HMISDistrict);
                    toExecute.Parameters.AddWithValue("FacilityName", _facilityName);
                    toExecute.Parameters.AddWithValue("AltFacilityName", _altFacilityName);
                    toExecute.Parameters.AddWithValue("AggregationLevelId", _aggregationLevelId);
                    toExecute.Parameters.AddWithValue("FacilityOwnerId", _facilityOwnerId);
                    toExecute.Parameters.AddWithValue("HealthCentreTypeId", _healthCentreTypeId);
                    toExecute.Parameters.AddWithValue("FacilityTypeId", _facilityTypeId);
                    toExecute.Parameters.AddWithValue("LaboratoryLevelId", _laboratoryLevelId);
                    toExecute.Parameters.AddWithValue("HospitalTypeId", _hospitalTypeId);

                    try
                    {
                        _helper.Execute(toExecute);
                    }
                    catch
                    {
                        success = false;
                        return success;
                    }
                 
                    string xmlFile = AppDomain.CurrentDomain.BaseDirectory + @"ConfigurationData\facility.1.xml";
                    string xmlFile2 = AppDomain.CurrentDomain.BaseDirectory + @"ConfigurationData\OtherXmlFiles\facilityNew.1.xml";
                   
                    FileStream fs = new FileStream(xmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    FileStream fs1 = new FileStream(xmlFile2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    // Create the XML Document
                    XmlDocument xDoc = new XmlDocument();

                    XmlDocument xDoc2 = new XmlDocument();


                    // Load the xml document
                    xDoc.Load(fs);

                    xDoc2.Load(fs1);

                    // Close the File Stream Connection
                    fs.Close();
                    fs1.Close();

                    // Create the new Element (node)
                    XmlElement newitem = xDoc.CreateElement("Facility");

                    XmlAttribute newAttribute = xDoc.CreateAttribute("DistrictId");
                    newAttribute.Value = _currentDistrictId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("DistrictFacilityId");
                    newAttribute.Value = districtFacilityId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HMISCode");
                    newAttribute.Value = _HMISCode.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HMISDistrict");
                    newAttribute.Value = _HMISDistrict.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("FacilityName");
                    newAttribute.Value = _facilityName.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("AltFacilityName");
                    newAttribute.Value = _altFacilityName.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("AggregationLevelId");
                    newAttribute.Value = _aggregationLevelId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("FacilityOwnerId");
                    newAttribute.Value = _facilityOwnerId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("FacilityTypeId");
                    newAttribute.Value = _facilityTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HealthCentreTypeId");
                    newAttribute.Value = _healthCentreTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("LaboratoryLevelId");
                    newAttribute.Value = _laboratoryLevelId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HospitalTypeId");
                    newAttribute.Value = _hospitalTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HealthPostTypeId");
                    newAttribute.Value = _healthCentreTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("LocationDistrictID");
                    newAttribute.Value = _locationDistrictId.ToString();
                    newitem.Attributes.Append(newAttribute);
                                      
                    xDoc.DocumentElement.InsertAfter(newitem, xDoc.DocumentElement.LastChild);

                    // Write to a New File
                    XmlElement newitem2 = xDoc2.CreateElement("Facility");

                    XmlAttribute newAttribute2 = xDoc2.CreateAttribute("DistrictId");
                    newAttribute2.Value = _currentDistrictId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("DistrictFacilityId");
                    newAttribute2.Value = districtFacilityId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HMISCode");
                    newAttribute2.Value = _HMISCode.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HMISDistrict");
                    newAttribute2.Value = _HMISDistrict.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("FacilityName");
                    newAttribute2.Value = _facilityName.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("AltFacilityName");
                    newAttribute2.Value = _altFacilityName.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("AggregationLevelId");
                    newAttribute2.Value = _aggregationLevelId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("FacilityOwnerId");
                    newAttribute2.Value = _facilityOwnerId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("FacilityTypeId");
                    newAttribute2.Value = _facilityTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HealthCentreTypeId");
                    newAttribute2.Value = _healthCentreTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("LaboratoryLevelId");
                    newAttribute2.Value = _laboratoryLevelId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HospitalTypeId");
                    newAttribute2.Value = _hospitalTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HealthPostTypeId");
                    newAttribute2.Value = _healthCentreTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("LocationDistrictID");
                    newAttribute2.Value = _locationDistrictId.ToString();
                    newitem2.Attributes.Append(newAttribute2);             

                    xDoc2.DocumentElement.InsertAfter(newitem2, xDoc2.DocumentElement.LastChild);
              
                    FileStream writer = new FileStream(xmlFile, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                    FileStream writer2 = new FileStream(xmlFile2, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                    xDoc.Save(writer);
                    xDoc2.Save(writer2);

                    // Close the writer filestream
                    writer.Close();
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }
        public Boolean insertIntoFacility(string facilityName, int facilityTypeId, int facilityOwnerId, int healthCentreTypeId, int hospitalId, int HospitalLevel)
        {
            Boolean success = true;
            if (setFacilityValues(HospitalLevel))
            {

                if ((hospitalId != 0) || (HospitalLevel == 4))
                {
                    switch (HospitalLevel) // To indicate for the 3 Administrative levels
                    {
                        case 0: // Federal Hospitals
                            _currentDistrictId = _federalId;
                            break;
                        case 1: // Regional Hospitals
                            _currentDistrictId = _regionId;
                            break;
                        case 2: // Zonal Hospitals
                            _currentDistrictId = _zoneId;
                            break;
                        case 4: // Health Centers (for health posts under them)
                            _currentDistrictId = Convert.ToInt32(HMISMainPage.SelectedLocationID);
                            break;
                        default:
                            break;
                    }
                }


                _facilityName = facilityName;
                _altFacilityName = facilityName;
                _facilityTypeId = facilityTypeId;
                _facilityOwnerId = facilityOwnerId;
                _healthCentreTypeId = healthCentreTypeId;
                _hospitalTypeId = hospitalId;

                if (!isThisFacilityExists(_currentDistrictId, _facilityName, _facilityTypeId))
                {
                    string cmdText = " INSERT INTO Facility " +
                   " (DistrictId, DistrictFacilityId, HMISCode, HMISDistrict, FacilityName " +
                   " ,AltFacilityName, AggregationLevelId, FacilityOwnerId, HealthCentreTypeId " +
                   " ,FacilityTypeId, LaboratoryLevelId, HospitalTypeId, HealthPostTypeId, LocationDistrictId) " +
                   " VALUES " +
                   " (@DistrictId, @DistrictFacilityId, @HMISCode, @HMISDistrict, " +
                   " @FacilityName, @AltFacilityName, @AggregationLevelId, @FacilityOwnerId, " +
                   " @HealthCentreTypeId, @FacilityTypeId, @LaboratoryLevelId, " +
                   " @HospitalTypeId, @HealthCentreTypeId, @LocationDistrictId )";

                    SqlCommand toExecute;

                    if (_locationDistrictId == 0)
                    {
                        _locationDistrictId = _currentDistrictId;
                    }

                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.notWoreda(_locationDistrictId.ToString()))
                    {
                        _locationDistrictId = int.Parse(Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getLocationDistrictId(_currentDistrictId.ToString()));
                    }

                    toExecute = new SqlCommand(cmdText);
                    toExecute.Parameters.AddWithValue("DistrictId", _currentDistrictId);
                    toExecute.Parameters.AddWithValue("LocationDistrictId", _locationDistrictId);
                    toExecute.Parameters.AddWithValue("DistrictFacilityId", _currentDistrictFacilityId);
                    toExecute.Parameters.AddWithValue("HMISCode", _HMISCode);
                    toExecute.Parameters.AddWithValue("HMISDistrict", _HMISDistrict);
                    toExecute.Parameters.AddWithValue("FacilityName", _facilityName);
                    toExecute.Parameters.AddWithValue("AltFacilityName", _altFacilityName);
                    toExecute.Parameters.AddWithValue("AggregationLevelId", _aggregationLevelId);
                    toExecute.Parameters.AddWithValue("FacilityOwnerId", _facilityOwnerId);
                    toExecute.Parameters.AddWithValue("HealthCentreTypeId", _healthCentreTypeId);
                    toExecute.Parameters.AddWithValue("FacilityTypeId", _facilityTypeId);
                    toExecute.Parameters.AddWithValue("LaboratoryLevelId", _laboratoryLevelId);
                    toExecute.Parameters.AddWithValue("HospitalTypeId", _hospitalTypeId);

                    try
                    {
                        _helper.Execute(toExecute);
                    }
                    catch
                    {
                        success = false;
                        return success;
                    }
                   
                    string xmlFile = AppDomain.CurrentDomain.BaseDirectory + @"ConfigurationData\facility.1.xml";
                    string xmlFile2 = AppDomain.CurrentDomain.BaseDirectory + @"ConfigurationData\OtherXmlFiles\facilityNew.1.xml";

                    FileStream fs = new FileStream(xmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    FileStream fs1 = new FileStream(xmlFile2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    // Create the XML Document
                    XmlDocument xDoc = new XmlDocument();

                    XmlDocument xDoc2 = new XmlDocument();


                    // Load the xml document
                    xDoc.Load(fs);

                    xDoc2.Load(fs1);

                    // Close the File Stream Connection
                    fs.Close();
                    fs1.Close();

                    // Create the new Element (node)
                    XmlElement newitem = xDoc.CreateElement("Facility");

                    XmlAttribute newAttribute = xDoc.CreateAttribute("DistrictId");
                    newAttribute.Value = _currentDistrictId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("DistrictFacilityId");
                    newAttribute.Value = _currentDistrictFacilityId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HMISCode");
                    newAttribute.Value = _HMISCode.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HMISDistrict");
                    newAttribute.Value = _HMISDistrict.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("FacilityName");
                    newAttribute.Value = _facilityName.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("AltFacilityName");
                    newAttribute.Value = _altFacilityName.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("AggregationLevelId");
                    newAttribute.Value = _aggregationLevelId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("FacilityOwnerId");
                    newAttribute.Value = _facilityOwnerId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("FacilityTypeId");
                    newAttribute.Value = _facilityTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HealthCentreTypeId");
                    newAttribute.Value = _healthCentreTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("LaboratoryLevelId");
                    newAttribute.Value = _laboratoryLevelId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HospitalTypeId");
                    newAttribute.Value = _hospitalTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("HealthPostTypeId");
                    newAttribute.Value = _healthCentreTypeId.ToString();
                    newitem.Attributes.Append(newAttribute);

                    newAttribute = xDoc.CreateAttribute("LocationDistrictID");
                    newAttribute.Value = _locationDistrictId.ToString();
                    newitem.Attributes.Append(newAttribute);                 

                    xDoc.DocumentElement.InsertAfter(newitem, xDoc.DocumentElement.LastChild);

                    // Write to a New File
                    XmlElement newitem2 = xDoc2.CreateElement("Facility");

                    XmlAttribute newAttribute2 = xDoc2.CreateAttribute("DistrictId");
                    newAttribute2.Value = _currentDistrictId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("DistrictFacilityId");
                    newAttribute2.Value = _currentDistrictFacilityId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HMISCode");
                    newAttribute2.Value = _HMISCode.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HMISDistrict");
                    newAttribute2.Value = _HMISDistrict.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("FacilityName");
                    newAttribute2.Value = _facilityName.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("AltFacilityName");
                    newAttribute2.Value = _altFacilityName.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("AggregationLevelId");
                    newAttribute2.Value = _aggregationLevelId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("FacilityOwnerId");
                    newAttribute2.Value = _facilityOwnerId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("FacilityTypeId");
                    newAttribute2.Value = _facilityTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HealthCentreTypeId");
                    newAttribute2.Value = _healthCentreTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("LaboratoryLevelId");
                    newAttribute2.Value = _laboratoryLevelId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HospitalTypeId");
                    newAttribute2.Value = _hospitalTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("HealthPostTypeId");
                    newAttribute2.Value = _healthCentreTypeId.ToString();
                    newitem2.Attributes.Append(newAttribute2);

                    newAttribute2 = xDoc2.CreateAttribute("LocationDistrictID");
                    newAttribute2.Value = _locationDistrictId.ToString();
                    newitem2.Attributes.Append(newAttribute2);
                   
                    xDoc2.DocumentElement.InsertAfter(newitem2, xDoc2.DocumentElement.LastChild);
                    // Save the XML file
                    // xDoc2.DocumentElement.RemoveChild(xDoc2.DocumentElement.FirstChild);

                    FileStream writer = new FileStream(xmlFile, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                    FileStream writer2 = new FileStream(xmlFile2, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                    xDoc.Save(writer);
                    xDoc2.Save(writer2);

                    // Close the writer filestream
                    writer.Close();                   

                    if (Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getFacilityTypeId(_HMISCode) == 2)
                    {
                        if (!insertIntoDistrict())
                        {
                            //General.Util.UI.MyMessageDialogSmall.Show(" Problem Adding to Districts table or xml ");
                            success = false;
                        }
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        public Boolean insertIntoDistrict()
        {
            Boolean success = true;

            if (!Admin.SiteSetting.DataAccess.SiteSettingDataAccess.districtExists(_HMISCode))
            {
                int provinceSeq = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getProvinceSeq(_HMISCode);
                int provinceCode = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getRegionId(_HMISCode);

                int notWoreda = 1; // For health Centers

                string cmdText = " INSERT INTO District " +
               " (DistrictSeq, ProvinceSeq, name, code, HmisDistrict " +
               " ,ProvinceCode, notWoreda) " +
               " VALUES " +
               " (@DistrictSeq, @ProvinceSeq, @name, @code, " +
               " @HmisDistrict, @ProvinceCode, @notWoreda)";

                SqlCommand toExecute;

                toExecute = new SqlCommand(cmdText);
                toExecute.Parameters.AddWithValue("DistrictSeq", Convert.ToInt32(_HMISCode));
                toExecute.Parameters.AddWithValue("ProvinceSeq", provinceSeq);
                toExecute.Parameters.AddWithValue("name", _facilityName);
                toExecute.Parameters.AddWithValue("code", Convert.ToInt32(_HMISCode));
                toExecute.Parameters.AddWithValue("HMISDistrict", _HMISDistrict);
                toExecute.Parameters.AddWithValue("ProvinceCode", provinceCode);
                toExecute.Parameters.AddWithValue("notWoreda", notWoreda);

                try
                {
                    _helper.Execute(toExecute);
                }
                catch
                {
                    success = false;
                    return success;
                }


                string districtXmlFile = AppDomain.CurrentDomain.BaseDirectory + @"ConfigurationData\district.1.xml";
                string districtxmlFile2 = AppDomain.CurrentDomain.BaseDirectory + @"ConfigurationData\OtherXmlFiles\districtNew.1.xml";


                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(xmlWrite);

                //doc.Save(xmlFile);

                // Create the reader filestream (fs)
                FileStream fs = new FileStream(districtXmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                FileStream fs1 = new FileStream(districtxmlFile2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Create the XML Document
                XmlDocument xDoc = new XmlDocument();

                XmlDocument xDoc2 = new XmlDocument();


                // Load the xml document
                xDoc.Load(fs);

                xDoc2.Load(fs1);

                // Close the File Stream Connection
                fs.Close();
                fs1.Close();

                // Create the new Element (node)
                XmlElement newitem = xDoc.CreateElement("District");               

                XmlAttribute newAttribute = xDoc.CreateAttribute("DistrictSeq");
                newAttribute.Value = _HMISCode.ToString();
                newitem.Attributes.Append(newAttribute);

                newAttribute = xDoc.CreateAttribute("ProvinceSeq");
                newAttribute.Value = provinceSeq.ToString();
                newitem.Attributes.Append(newAttribute);

                newAttribute = xDoc.CreateAttribute("Name");
                newAttribute.Value = _facilityName.ToString();
                newitem.Attributes.Append(newAttribute);

                newAttribute = xDoc.CreateAttribute("Code");
                newAttribute.Value = _HMISCode.ToString();
                newitem.Attributes.Append(newAttribute);

                newAttribute = xDoc.CreateAttribute("HMISDistrict");
                newAttribute.Value = _HMISDistrict.ToString();
                newitem.Attributes.Append(newAttribute);

                newAttribute = xDoc.CreateAttribute("ProvinceCode");
                newAttribute.Value = provinceCode.ToString();
                newitem.Attributes.Append(newAttribute);

                newAttribute = xDoc.CreateAttribute("notWoreda");
                newAttribute.Value = notWoreda.ToString();
                newitem.Attributes.Append(newAttribute);


                xDoc.DocumentElement.InsertAfter(newitem, xDoc.DocumentElement.LastChild);

                // Write to a New File
                XmlElement newitem2 = xDoc2.CreateElement("District");

                XmlAttribute newAttribute2 = xDoc2.CreateAttribute("DistrictSeq");
                newAttribute2.Value = _HMISCode.ToString();
                newitem2.Attributes.Append(newAttribute2);

                newAttribute2 = xDoc2.CreateAttribute("ProvinceSeq");
                newAttribute2.Value = provinceSeq.ToString();
                newitem2.Attributes.Append(newAttribute2);

                newAttribute2 = xDoc2.CreateAttribute("Name");
                newAttribute2.Value = _facilityName.ToString();
                newitem2.Attributes.Append(newAttribute2);

                newAttribute2 = xDoc2.CreateAttribute("Code");
                newAttribute2.Value = _HMISCode.ToString();
                newitem2.Attributes.Append(newAttribute2);

                newAttribute2 = xDoc2.CreateAttribute("HMISDistrict");
                newAttribute2.Value = _HMISDistrict.ToString();
                newitem2.Attributes.Append(newAttribute2);

                newAttribute2 = xDoc2.CreateAttribute("ProvinceCode");
                newAttribute2.Value = provinceCode.ToString();
                newitem2.Attributes.Append(newAttribute2);

                newAttribute2 = xDoc2.CreateAttribute("notWoreda");
                newAttribute2.Value = notWoreda.ToString();
                newitem2.Attributes.Append(newAttribute2);


                xDoc2.DocumentElement.InsertAfter(newitem2, xDoc2.DocumentElement.LastChild);
                // Save the XML file
                // xDoc2.DocumentElement.RemoveChild(xDoc2.DocumentElement.FirstChild);

                FileStream writer = new FileStream(districtXmlFile, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                FileStream writer2 = new FileStream(districtxmlFile2, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                xDoc.Save(writer);
                xDoc2.Save(writer2);

                // Close the writer filestream
                writer.Close();
            }
            else
            {
                success = false;
            }

            return success;
        }

        private bool isThisFacilityExists(int DistrictId, string FacilityName, int FacilityTypeId)
        {
            string sqlStr = "SELECT * FROM Facility Where DistrictId=@DistrictId AND FacilityName=@FacilityName AND FacilityTypeId=@FacilityTypeId";
            DBConnHelper DBConnHelper = new DBConnHelper();

            SqlCommand toExecute;

            toExecute = new SqlCommand(sqlStr);
            toExecute.Parameters.AddWithValue("DistrictId", DistrictId);
            toExecute.Parameters.AddWithValue("FacilityName", FacilityName);
            toExecute.Parameters.AddWithValue("FacilityTypeId", FacilityTypeId);

            DataSet ds = DBConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            return false;
        }


        public static string GetFacilityTypeFromID(string id)
        {
            string sql = "SELECT FacilityTypeId FROM dbo.facility where HMISCODE ='" + id + "'";
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            return helper.GetScalar(sql).ToString();
        }
        public static string GetFacilityNameFromID(string id)
        {
            string sql = "SELECT FacilityName FROM dbo.facility where HMISCODE ='" + id + "'";
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            return helper.GetScalar(sql).ToString();
        }
        public static string GetFacilityIDFromName(string name)
        {
            string sql = "SELECT Code FROM dbo.District where [Name] ='" + name + "'";
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            return helper.GetScalar(sql).ToString();
        }
        public static DataTable GetAllHCInWoreda(string woredaname)
        {

            string hmiscode = (new HmisDatabase().getWoredaId(woredaname)).ToString();
            //string hmiscode = GetFacilityIDFromName(woredaname);
            string sql = " select HMISCODE, FacilityName from facility where FacilityTypeId=2 " +
              " and districtid in(select districtid from facility  where HMISCode = (select Convert(nvarchar(10),DistrictSeq) from " +
                " dbo.District  where DistrictSeq= '" + hmiscode + "')) order by FacilityName";
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            DataSet ds = helper.GetDataSet(sql);
            return ds.Tables[0];

        }
        public static DataTable GetAllHCInZone(string zoneName)
        {         
            HmisDatabase dbHelper = new HmisDatabase();
            int zoneId = dbHelper.getZoneId(zoneName);

            string sql = " select HMISCODE, FacilityName from facility where FacilityTypeId=2 " +
              " and DistrictId = " + zoneId;
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            DataSet ds = helper.GetDataSet(sql);
            return ds.Tables[0];

        }
        public static DataTable GetAllHCInRegion(string regionName)
        {
            HmisDatabase dbHelper = new HmisDatabase();
            int regionId = dbHelper.getRegionId(regionName);

            string sql = " select HMISCODE, FacilityName from facility where FacilityTypeId=2 " +
              " and DistrictId = " + regionId;
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            DataSet ds = helper.GetDataSet(sql);
            return ds.Tables[0];

        }
        public static DataTable GetHealthPostUnderHealthCenter(string hcid)
        {
            string sql = "select HMISCODE, FacilityName from facility where FacilityTypeId=3 " +
                " and districtid in(select HMISCODE from facility where HMISCODE = '" + hcid + "' and FacilityTypeId=2)";
            SqlManagement.Database.DBConnHelper helper = new SqlManagement.Database.DBConnHelper();
            DataSet ds = helper.GetDataSet(sql);
            return ds.Tables[0];
        }
    }
}
