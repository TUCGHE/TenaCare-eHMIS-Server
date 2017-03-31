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
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using SqlManagement.Database;


namespace Admin.SiteSetting.DataAccess
{
    public class SiteSettingDataAccess
    {

        //public Hashtable higerLevelInstitutions = new Hashtable();
        //public Hashtable facilities = new Hashtable();

        public static DBConnHelper dbConnHelper = new DBConnHelper();

        public static Boolean facilityAdded = false;
        public static Boolean siteSettingTreeFinished = false;

        public static Hashtable globalLowerLocations = new Hashtable();
        private static ArrayList lowerHmisCodeList = new ArrayList();

        public SiteSettingDataAccess()
        {
        }

        public static Hashtable getReportingHierarchy(string hmisCode, out ArrayList hmisCodeList)
        {
            Hashtable hierarchy = new Hashtable();
            hmisCodeList = new ArrayList();                 

            string cmd = " select * from facility " +
                         " where hmiscode = @hmiscode"; //'" + hmisCode + "'";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmiscode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];
            string name = "", districtid = "", hmiscode = "",
                facilitytype = "";
            if (dt.Rows.Count > 0) // There is Zone
            {

                name = dt.Rows[0]["facilityname"].ToString();
                districtid = dt.Rows[0]["districtId"].ToString();
                facilitytype = dt.Rows[0]["facilityTypeId"].ToString();
                hmiscode = dt.Rows[0]["hmiscode"].ToString();

                if (facilitytype == "11") // Federal
                {
                    hmisCodeList.Add(hmiscode);
                    hierarchy.Add(hmiscode,name);
                }
                else if (facilitytype == "10") // Regions
                {
                    hierarchy.Add("20", "Federal Ministry of Health");
                    hmisCodeList.Add("20");
                    hmisCodeList.Add(hmiscode);
                    hierarchy.Add(hmiscode, name);
                }
                else if (facilitytype == "9") // Zones
                {
                    hierarchy.Add("20", "Federal Ministry of Health");
                    hmisCodeList.Add("20");

                    cmd = " select name, code from province " +
                          " inner join EthEhmis_HmisZone on RegionId = Code " +
                          " where zoneId = @hmisCode";

                    toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                    ds = dbConnHelper.GetDataSet(toExecute);

                    dt = ds.Tables[0];

                    string regionName = "", provinceCode = "";
                    if (dt.Rows.Count > 0) // There is Zone
                    {
                        regionName = dt.Rows[0]["name"].ToString() + " Regional Health Bureau";
                        provinceCode = dt.Rows[0]["code"].ToString();
                    }

                    hierarchy.Add(provinceCode,regionName);
                    hmisCodeList.Add(provinceCode);

                    hierarchy.Add(hmiscode,name);
                    hmisCodeList.Add(hmiscode);
                    
                }

                else if (facilitytype == "8") // Woredas
                {
                    hierarchy.Add("20", "Federal Ministry of Health");
                    hmisCodeList.Add("20");

                    cmd = " select ZoneName, province.Name as RegionName, EthEhmis_HmisZoneDistrict.ZoneId, province.Code  from EthEhmis_HmisZoneDistrict  " +
                          " left join EthEhmis_HmisZone on " +
                          " EthEhmis_HmisZoneDistrict.ZoneId = EthEhmis_HmisZone.ZoneId " +
                          " left join province on " +
                          " EthEhmis_hmisZone.RegionId = province.code " +
                          " where EthEhmis_HmisZoneDistrict.districtSeq = @hmisCode"; //'" + hmisCode + "'";                          

                    toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                    ds = dbConnHelper.GetDataSet(toExecute);

                    dt = ds.Tables[0];

                    string regionName = "", zoneName = "", zoneId = "", provinceCode = "";
                    if (dt.Rows.Count > 0) // there is zone
                    {
                        regionName = dt.Rows[0]["RegionName"].ToString() + " Regional Health Bureau";
                        zoneName = dt.Rows[0]["zoneName"].ToString();
                        zoneId = dt.Rows[0]["zoneId"].ToString();
                        provinceCode = dt.Rows[0]["code"].ToString();

                        hierarchy.Add(provinceCode, regionName);
                        hmisCodeList.Add(provinceCode);

                        hierarchy.Add(zoneId,zoneName);
                        hmisCodeList.Add(zoneId);

                        hierarchy.Add(hmiscode,name);
                        hmisCodeList.Add(hmiscode);
                    }
                    else // No Zone for this woreda
                    {
                        cmd = " select province.Name as RegionName, province.code  from province  " +
                         " inner join district on " +                          
                         " district.provinceCode = province.code " +
                         " where district.districtSeq = @hmisCode"; //'" + hmisCode + "'";

                        toExecute = new SqlCommand(cmd);
                        toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                        ds = dbConnHelper.GetDataSet(toExecute);

                        dt = ds.Tables[0];

                        regionName = dt.Rows[0]["RegionName"].ToString() + " Regional Health Bureau";
                        provinceCode = dt.Rows[0]["code"].ToString();

                        hierarchy.Add(provinceCode, regionName);
                        hmisCodeList.Add(provinceCode);
                      
                        hierarchy.Add(hmiscode,name);
                        hmisCodeList.Add(hmiscode);                        
                    }                                    
                }
                else 
                {                                        
                    hierarchy = getReportingHierarchy(districtid, out hmisCodeList);
                    hmisCodeList.Add(hmiscode);
                    hierarchy.Add(hmiscode,name);
                }
            }
            return hierarchy;
        }

       // public static void getInstitutions(string higherLevelId, out Hashtable facilities, out Hashtable higerLevelInstitutions, out ArrayList facilityCodes, out ArrayList institutionCodes)
        public static void getInstitutions(string higherLevelId, out Hashtable facilities, out ArrayList facilityCodes, string searchQuery)
        {
            ArrayList test = new ArrayList();
            facilities = new Hashtable();
            //higerLevelInstitutions = new Hashtable();
            facilityCodes = new ArrayList();
            //institutionCodes = new ArrayList();
            string orderByClause = "  order by facilityName ";
            string cmd = "";

            //string cmd = "select facilityname, hmiscode from facility where aggregationlevelId = 4 and districtId = " + higherLevelId + searchQuery + orderByClause;

            //using (DBConnHelper dbConnHelper = new DBConnHelper())
            //{
            int result=0;
            //if (!(int.TryParse(higherLevelId, out result)))
            //{
            //    higherLevelId = "-99";
            //}
            cmd = "select facilityname, hmiscode from facility where aggregationlevelId = 4 and districtId = @higherLevelId " + searchQuery + orderByClause;

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("higherLevelId", higherLevelId);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                string hmisCode = row["hmiscode"].ToString();
                string facilityName = row["facilityname"].ToString();
                facilities.Add(hmisCode, facilityName);
                facilityCodes.Add(hmisCode);
            }     
        }

        public static void getLocations(string higherLevelId, out Hashtable locations, out ArrayList institutionCodes, out string institutionType, string searchQuery)
        {
            // Determine where the higher level starts if higherLevelId is -1 then stop the recursive call
            string cmd = "";

            //string cmd = "select facilityTypeId from facility where  hmiscode = '" + higherLevelId + "'";
            locations = new Hashtable();
            institutionCodes = new ArrayList();
            institutionType = "";
            string orderByClause = "  order by facilityName ";
            //using (DBConnHelper dbConnHelper = new DBConnHelper())
            //{
            cmd = "select facilityTypeId from facility where hmiscode = @hmisCode"; //'" + higherLevelId + "'";

            SqlCommand toExecute = new SqlCommand(cmd);
            toExecute.Parameters.AddWithValue("hmisCode", higherLevelId);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];
            int facilityTypeId = 0;

            foreach (DataRow row in dt.Rows)
            {
                facilityTypeId = Convert.ToInt32(row["facilityTypeId"].ToString());
            }

            if (facilityTypeId == 11) // Federal
            {
                // Display the Region Locations
                cmd = " select hmiscode,facilityname from facility where hmiscode in " +
                      " (select cast(Code as varchar(max)) from province where code != '20') " + searchQuery + orderByClause;

                toExecute = new SqlCommand(cmd);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    string hmiscode = row["hmiscode"].ToString();
                    string facilityname = row["facilityname"].ToString();
                    locations.Add(hmiscode, facilityname);
                    institutionCodes.Add(hmiscode);
                }

                institutionType = "Regions";
            }
            else if (facilityTypeId == 10) // Region
            {
                // Display the Zone Locations If they exist, else show Woredas directly
                cmd = " select hmiscode,facilityname from facility where hmiscode in " +
                      " (select cast(zoneId as varchar(max)) from EthEhmis_HmisZone where RegionId = @higherLevelId " + ")" + searchQuery + orderByClause;

                toExecute = new SqlCommand(cmd);

                toExecute.Parameters.AddWithValue("higherLevelId", higherLevelId);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];

                if (dt.Rows.Count > 0) // There is Zone
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string hmiscode = row["hmiscode"].ToString();
                        string facilityname = row["facilityname"].ToString();
                        locations.Add(hmiscode, facilityname);
                        institutionCodes.Add(hmiscode);
                    }
                    institutionType = "Zones";
                }
                else
                {
                    cmd = " select hmiscode,facilityname from facility where hmiscode in " +
                          " (select cast(districtSeq as varchar(max)) from District where ProvinceCode = @higherLevelId "  + ")" + 
                          " and facilityTypeId = 8 " + searchQuery + orderByClause; 

                    toExecute = new SqlCommand(cmd);
                    toExecute.Parameters.AddWithValue("higherLevelId", higherLevelId);

                    ds = dbConnHelper.GetDataSet(toExecute);

                    dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string hmiscode = row["hmiscode"].ToString();
                            string facilityname = row["facilityname"].ToString();
                            locations.Add(hmiscode, facilityname);
                            institutionCodes.Add(hmiscode);
                        }
                        institutionType = "Woredas";
                    }
                }
            }
            else if (facilityTypeId == 9)// Zone
            {
                // Display Woredas
                cmd = " select hmiscode,facilityname from facility where hmiscode in " +
                          " (select cast(districtSeq as varchar(max)) from EthEhmis_HmisZoneDistrict where ZoneId = @higherLevelId " + ")" +
                          " and facilityTypeId = 8 " + searchQuery + orderByClause;

                toExecute = new SqlCommand(cmd);

                toExecute.Parameters.AddWithValue("higherLevelId", higherLevelId);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];

                if (dt.Rows.Count > 0) // There is Zone
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string hmiscode = row["hmiscode"].ToString();
                        string facilityname = row["facilityname"].ToString();
                        locations.Add(hmiscode, facilityname);
                        institutionCodes.Add(hmiscode);
                    }
                    institutionType = "Woredas";
                }
                //}               
            }
        }

        public static void SaveSiteSetting(string hmisCode)
        {            
            string cmd = " select * from facility inner join district on  " +
                         " facility.districtid = district.districtseq " +                         
                         " where hmiscode = @hmisCode"; //'" + hmisCode + "'";            

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];
            string hmiscode = "", districtid = "", locationdistrictid = "", 
                aggregationlevel = "", facilitytype = "", facilityowner = "", provinceID="";
            if (dt.Rows.Count > 0) 
            {

                hmiscode = dt.Rows[0]["hmiscode"].ToString();
                districtid = dt.Rows[0]["districtId"].ToString();
                locationdistrictid = dt.Rows[0]["locationdistrictId"].ToString();
                aggregationlevel = dt.Rows[0]["aggregationlevelId"].ToString();
                facilitytype = dt.Rows[0]["facilityTypeId"].ToString();
                facilityowner = dt.Rows[0]["facilityOwnerId"].ToString();
                provinceID = dt.Rows[0]["ProvinceCode"].ToString();


                // drop the table and re-create it, not right now , something to think about


                dbConnHelper.InsertOrUpdateSetting("HmisCode", hmiscode, "string", "HMIS Code");
                dbConnHelper.InsertOrUpdateSetting("DistrictId", districtid, "int", "District ID");
                dbConnHelper.InsertOrUpdateSetting("LocationDistrictID", locationdistrictid, "int", "Location District ID");

                dbConnHelper.InsertOrUpdateSetting("AggregationLevel", getAggregationString(aggregationlevel), "string", "Aggregation Level");
                dbConnHelper.InsertOrUpdateSetting("ProvinceId", provinceID, "string", "Province ID");
                dbConnHelper.InsertOrUpdateSetting("FacilityOwner", facilityowner, "string", "Facility Owner");
                dbConnHelper.InsertOrUpdateSetting("FacilityType", facilitytype, "string", "Facility Type");

            }
        }

        private static string getAggregationString(string aggregationlevel)
        {
            string aggregationString = "";

            if (aggregationlevel == "1")
            {
                aggregationString = "National";
            }
            else if (aggregationlevel == "2")
            {
                aggregationString = "Regional";
            }
            else if (aggregationlevel == "3")
            {
                aggregationString = "Woreda";
            }
            else if (aggregationlevel == "5")
            {
                aggregationString = "Zonal";
            }

            return aggregationString;
        }

        public static int getAggregationLevelId(string hmisCode)
        {
            string cmd = " select aggregationLevelId from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";
            int aggregationlevel = 0;

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];
            
            if (dt.Rows.Count > 0) // There is Zone
            {
                aggregationlevel = Convert.ToInt16(dt.Rows[0]["aggregationlevelId"].ToString());
            }

            return aggregationlevel;
        }

        public static int getFacilityTypeId(string hmisCode)
        {
            string cmd = " select facilityTypeId from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";
            int facilityTypeId = 0;

            SqlCommand toExecute = new SqlCommand(cmd);
            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                facilityTypeId = Convert.ToInt16(dt.Rows[0]["facilityTypeId"].ToString());
            }

            return facilityTypeId;
        }

        public static string getFacilityTypeName(string hmisCode)
        {
            string cmd = " select FacilityTypeName from facility  " +
                         " inner join FacilityType on  " +
                         " Facility.FacilityTypeId = FacilityType.FacilityTypeId " +
                         " where hmiscode = @hmisCode "; //'" + hmisCode + "'";

            string facilityTypeName = "";

            SqlCommand toExecute = new SqlCommand(cmd);
            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                facilityTypeName = dt.Rows[0]["FacilityTypeName"].ToString();
            }

            return facilityTypeName;
        }

        public static string getFacilityOwnerName(string hmisCode)
        {
            string cmd = " select FacilityOwnerName from facility inner join FacilityOwner " +
                         " on facility.FacilityOwnerId = FacilityOwner.FacilityOwnerId " +
                         " where hmiscode = @hmisCode "; //'" + hmisCode + "'";

            string facilityOwnerName = "";

            SqlCommand toExecute = new SqlCommand(cmd);
            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                facilityOwnerName = dt.Rows[0]["FacilityOwnerName"].ToString();
            }

            return facilityOwnerName;
        }

        public static Hashtable getSiteSettingData(string hmisCode)
        {
            Hashtable siteSettingData = new Hashtable();

            if (hmisCode != "")
            {
                string cmd = "select * from facility where hmisCode = @hmisCode "; //'" + hmisCode + "'";

               

                SqlCommand toExecute = new SqlCommand(cmd);
                toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                DataSet ds = dbConnHelper.GetDataSet(toExecute);

                DataTable dt = ds.Tables[0];

                string locationDistrictId = "";
                string facilityName = "";
                if (dt.Rows.Count > 0)
                {
                    locationDistrictId = dt.Rows[0]["locationDistrictID"].ToString();
                    facilityName = dt.Rows[0]["facilityName"].ToString();
                }

                siteSettingData.Add("FacilityName", facilityName);               

                cmd = " select province.Name as Region, ZoneName as Zone, District.Name as Woreda from District " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " district.districtSeq = EthEhmis_HmisZoneDistrict.districtseq " +
                      " left join EthEhmis_HmisZone on " +
                      " EthEhmis_HmisZoneDistrict.ZoneId = EthEhmis_HmisZone.ZoneId " +
                      " left join province on " +
                      " District.ProvinceCode = Province.Code " +
                      " where District.districtSeq = @locationDistrictID "; // +locationDistrictId;

                toExecute = new SqlCommand(cmd);

                toExecute.Parameters.AddWithValue("locationDistrictID", locationDistrictId);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];

                string Region = "", Zone = "", Woreda = "";

                if (dt.Rows.Count > 0)
                {
                    Region = dt.Rows[0]["Region"].ToString();
                    Zone = dt.Rows[0]["Zone"].ToString();
                    Woreda = dt.Rows[0]["Woreda"].ToString();

                    if (Region != "")
                    {
                        siteSettingData.Add("Region", Region);
                    }

                    if (Zone != "")
                    {
                        Zone = Zone.Replace("Zonal Health Department", "");

                        if (Zone.ToUpper().Contains("SPECIAL"))
                        {
                            Zone = Zone + " zone";
                        }
                        siteSettingData.Add("Zone", Zone);
                    }

                    if (!specialWoreda(hmisCode))
                    {
                        if (Woreda != "")
                        {
                            siteSettingData.Add("Woreda", Woreda);
                        }
                    }
                }
            }

            return siteSettingData;

        }

        public static Boolean specialWoreda(string hmisCode)
        {           
            string cmd = " select hmiscode from facility where hmiscode = @hmisCode " + //'" + hmisCode + "'" +
                         " and hmiscode in (select hmiscode from facility where cast(districtid as varchar(max)) = hmiscode " +
                         " and (facilityTypeId > 8 or facilityTypeId = 2) and districtid = locationdistrictid ) ";

            Hashtable siteSettingData = new Hashtable();

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            Boolean specialWoreda = false;

            if (dt.Rows.Count > 0)
            {
                specialWoreda = true;
            }

            return specialWoreda;
        }

        public static Boolean checkLowerHealthFacility(string hmisCode)
        {

            int result = 0;
            //if (!(int.TryParse(hmisCode, out result)))
            //{
            //    hmisCode = "-99";
            //}

            string cmd = " select * from facility where districtid = @hmisCode "; //'" + hmisCode + "'";
                        

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);


            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            Boolean lowerHealthFacility = false;

            if (dt.Rows.Count > 0)
            {
                lowerHealthFacility = true;
            }

            return lowerHealthFacility;
        }

        public static int getRegionId(string hmisCode)
        {
            int regionId = 0;

            int result = 0;
            //if (!(int.TryParse(hmisCode, out result)))
            //{
            //    hmisCode = "-99";
            //}

            if (hmisCode == "")
            {
                return 0;
            }

            string cmd = " select * from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            string districtId = "";
            int facilityTypeId = 0;

            if (dt.Rows.Count > 0)
            {
                districtId = dt.Rows[0]["districtId"].ToString();
                facilityTypeId = Convert.ToInt32(dt.Rows[0]["facilityTypeId"].ToString());
            }

            if (facilityTypeId == 11)
            {
                regionId = 20;
            }
            else if (facilityTypeId == 10)
            {
                regionId = Convert.ToInt32(districtId);
            }
            else if (facilityTypeId == 9) // Zone
            {
                cmd = " select regionId from EthEhmis_HmisZone where ZoneId = @hmisCode "; //'" + hmiscode + "'";

                toExecute = new SqlCommand(cmd);
                toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];
                
                if (dt.Rows.Count > 0)
                {
                    regionId = Convert.ToInt32(dt.Rows[0]["regionId"].ToString());
                }
            }
            else if (facilityTypeId == 8) //
            {
                cmd = " select ProvinceCode from District where districtSeq = @hmisCode "; //'" + hmiscode + "'";

                toExecute = new SqlCommand(cmd);
                toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    regionId = Convert.ToInt32(dt.Rows[0]["ProvinceCode"].ToString());
                }
            }
            else 
            {                
                regionId = getRegionId(districtId);
            }

            return regionId;
        }

        public static int getProvinceSeq(string hmisCode)
        {
            int regionId = getRegionId(hmisCode);
            int provinceSeq = 0;

            string cmd = " select provinceSeq from province where code = @regionId "; // +regionId;

            SqlCommand toExecute = new SqlCommand(cmd);
            toExecute.Parameters.AddWithValue("regionId", regionId);


            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                provinceSeq = Convert.ToInt32(dt.Rows[0]["provinceSeq"].ToString());
            }

            return provinceSeq;
        }

        public static int getZoneId(string hmisCode)
        {
            int zoneId = 0;

            int result = 0;
            //if (!(int.TryParse(hmisCode, out result)))
            //{
            //    hmisCode = "-99";
            //}
            if (hmisCode == "")
            {
                return 0;
            }

            string cmd = " select * from facility where hmiscode = @hmisCode "; //'" + hmiscode + "'";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            string districtId = "";
            int facilityTypeId = 0;

            if (dt.Rows.Count > 0)
            {
                districtId = dt.Rows[0]["districtId"].ToString();
                facilityTypeId = Convert.ToInt32(dt.Rows[0]["facilityTypeId"].ToString());
            }

            if (facilityTypeId == 11)
            {
                zoneId = 0;
            }
            else if (facilityTypeId == 10)
            {
                zoneId = 0;
            }
            else if (facilityTypeId == 9) // Zone
            {
                zoneId = Convert.ToInt32(districtId);
            }
            else if (facilityTypeId == 8) //
            {
                cmd = " select zoneId from EthEhmis_HmisZoneDistrict where DistrictSeq = @hmisCode "; //'" + hmiscode + "'"; 

                toExecute = new SqlCommand(cmd);

                toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                ds = dbConnHelper.GetDataSet(toExecute);

                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    zoneId = Convert.ToInt32(dt.Rows[0]["zoneId"].ToString());
                }
            }
            else
            {
                zoneId = getZoneId(districtId);
            }

            return zoneId;
        }

        public static int getWoredaId(string hmisCode)
        {
            int woredaId = 0;

            int result = 0;
            //if (!(int.TryParse(hmisCode, out result)))
            //{
            //    hmisCode = "-99";
            //}

            if (hmisCode == "")
            {
                return 0;
            }
            string cmd = " select * from facility where hmiscode = @hmisCode "; //'" + hmiscode + "'";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);


            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            string districtId = "";
            int facilityTypeId = 0;

            if (dt.Rows.Count > 0)
            {
                districtId = dt.Rows[0]["districtId"].ToString();
                facilityTypeId = Convert.ToInt32(dt.Rows[0]["facilityTypeId"].ToString());
            }

            if (facilityTypeId == 11)
            {
                woredaId = 0;
            }
            else if (facilityTypeId == 10)
            {
                woredaId = 0;
            }
            else if (facilityTypeId == 9) // Zone
            {
                woredaId = 0;
            }
            else if (facilityTypeId == 8) //
            {
                woredaId = Convert.ToInt32(districtId);
            }
            else
            {
                woredaId = getWoredaId(districtId);
            }

            return woredaId;
        }
        public static int getHCId(string hmisCode)
        {
            int hcId = 0;

            int result = 0;
            //if (!(int.TryParse(hmisCode, out result)))
            //{
            //    hmisCode = "-99";
            //}

            if (hmisCode == "")
            {
                return 0;
            }
            string cmd = " select * from facility where hmiscode = @hmisCode "; //'" + hmiscode + "'";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);


            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            string districtId = "";
            int facilityTypeId = 0;

            if (dt.Rows.Count > 0)
            {
                districtId = dt.Rows[0]["districtId"].ToString();
                facilityTypeId = Convert.ToInt32(dt.Rows[0]["facilityTypeId"].ToString());
            }

            if (facilityTypeId == 11)
            {
                hcId = 0;
            }
            else if (facilityTypeId == 10)
            {
                hcId = 0;
            }
            else if (facilityTypeId == 9) // Zone
            {
                hcId = 0;
            }
            else if (facilityTypeId == 8) //
            {
                hcId = 0;
            }
            else if (facilityTypeId == 2)//hc
            {
                hcId = Convert.ToInt32(hmisCode);
            }
            else if (facilityTypeId == 3) //hp
            {
                hcId = Convert.ToInt32(districtId);
            }
           
            else
            {
                hcId = getWoredaId(districtId);
            }

            return hcId;
        }

        public Hashtable getLowerLocations(string hmisCode)
        {
            int result = 0;
            Hashtable nameWithHmisCode = new Hashtable();

            //if (!(int.TryParse(hmisCode, out result)))
            //{
            //    hmisCode = "-99";
            //}

            string cmd = " select * from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);


            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];
            
            foreach (DataRow row in dt.Rows)
            {                
                string facilityname = row["facilityname"].ToString();
                if (!nameWithHmisCode.ContainsKey(hmisCode))
                nameWithHmisCode.Add(hmisCode, facilityname);
                //institutionCodes.Add(facilityname);
            }

            return nameWithHmisCode;
        }

        public static Hashtable getAllSearchedNames(string searchName, out ArrayList hmisCodes)
        {
            Hashtable allSearchedNames = new Hashtable();
            hmisCodes = new ArrayList();
            
            string mainHmisCode = dbConnHelper.GetSetting("HmisCode", "string").ToString();
            ArrayList lowerLocations = new ArrayList();

            Hashtable allLowerLocations = new Hashtable();
            //ArrayList lowerHmisCodeList = new ArrayList();
            string types = "";

            globalLowerLocations.Clear();
            lowerHmisCodeList.Clear();

            //string searchQuery = "  and facilityname like '%" + searchName + "%'";
            string searchQuery = "  and facilityname like  @searchName ";

           // searchQuery = "";

            //allLowerLocations = getRecursiveAllLowerLocations(mainHmisCode, searchQuery, searchName);
            //hmisCodes = lowerHmisCodeList;
            
            // For Federal
            //string cmd = " select * from facility where facilityname like '%" + searchName +  "%'";

            string cmd = " select * from facility where facilityname like @searchName ";

            if (getFacilityTypeId(mainHmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode " + searchQuery;
            }
            else if (getFacilityTypeId(mainHmisCode) == 9) // Zone
            {
                cmd = " select * from facility left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.Districtseq = facility.districtId " +
                      " where ((facility.districtId = @mainHmisCode) or (ZoneId = @mainHmisCode ))" + searchQuery;
            }
            else if (getFacilityTypeId(mainHmisCode) == 8) // District
            {
                cmd = " select * from facility where districtId = @mainHmisCode "  + searchQuery;
            }
            else if (getFacilityTypeId(mainHmisCode) == 2) // Health Center
            {
                cmd = " select * from facility where districtId = @mainHmisCode " + searchQuery;
            }


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", mainHmisCode);
            toExecute.Parameters.AddWithValue("searchName", "%" + searchName + "%");

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                string facilityname = row["facilityname"].ToString();
                string code = row["hmiscode"].ToString();
                hmisCodes.Add(code);
                allSearchedNames.Add(code, facilityname);
            }

            return allSearchedNames;

        }

        public static DataTable getAllLowerLocations(string hmisCode)
        {
            //Hashtable allSearchedNames = new Hashtable();
            //hmisCodes = new ArrayList();

            //string mainHmisCode = dbConnHelper.GetSetting("HmisCode", "string").ToString();
            //ArrayList lowerLocations = new ArrayList();

            //Hashtable allLowerLocations = new Hashtable();
            ////ArrayList lowerHmisCodeList = new ArrayList();
            //string types = "";

            //globalLowerLocations.Clear();
            //lowerHmisCodeList.Clear();

            ////string searchQuery = "  and facilityname like '%" + searchName + "%'";
            //string searchQuery = "  and facilityname like  @searchName ";

            // searchQuery = "";

            //allLowerLocations = getRecursiveAllLowerLocations(mainHmisCode, searchQuery, searchName);
            //hmisCodes = lowerHmisCodeList;

            // For Federal
            //string cmd = " select * from facility where facilityname like '%" + searchName +  "%'";

            string cmd = " select * from facility where facilityname  ";

            if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode order by facilityTypeId desc, " +
                      " facilityName asc ";
            }
            else if (getFacilityTypeId(hmisCode) == 9) // Zone
            {
                cmd = " select * from facility left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.Districtseq = facility.districtId " +
                      " where ((facility.districtId = @mainHmisCode) or (ZoneId = @mainHmisCode ))";
            }
            else if (getFacilityTypeId(hmisCode) == 8) // District
            {
                cmd = " select * from facility where districtId = @mainHmisCode ";
            }
            else if (getFacilityTypeId(hmisCode) == 2) // Health Center
            {
                cmd = " select * from facility where districtId = @mainHmisCode ";
            }


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);
            
            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            //foreach (DataRow row in dt.Rows)
            //{
            //    string facilityname = row["facilityname"].ToString();
            //    string code = row["hmiscode"].ToString();
            //    hmisCodes.Add(code);
            //    allSearchedNames.Add(code, facilityname);
            //}

            return dt;

        }

        public static Hashtable getRecursiveAllLowerLocations(string higherLevelId, string searchQuery, string searchName)
        {
            
            



            return globalLowerLocations;
        }
     
        public static string getFacilityName(string hmisCode)
        {
            string facilityName = "";
            using (DBConnHelper dbConnHelper = new DBConnHelper())
            {
                string cmd = " select facilityName from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";
                

                SqlCommand toExecute = new SqlCommand(cmd);

                toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

                DataSet ds = dbConnHelper.GetDataSet(toExecute);

                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0) // There is Zone
                {
                    facilityName = dt.Rows[0]["facilityName"].ToString();
                }                
            }
            return facilityName;
        }

        public static string getDistrictId(string hmisCode)
        {
            string cmd = " select districtId from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";
            string districtId = "";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                districtId =  dt.Rows[0]["districtId"].ToString();
            }

            return districtId;
        }

        public static string getDistrictFacilityId(string hmisCode)
        {
            string cmd = " select districtFacilityId from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";
            string districtFacilityId = "";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                districtFacilityId = dt.Rows[0]["districtFacilityId"].ToString();
            }

            return districtFacilityId;
        }


        public static string getLocationDistrictId(string hmisCode)
        {
            string cmd = " select LocationDistrictId from facility where hmiscode = @hmisCode "; //'" + hmisCode + "'";
            string locationDistrictId = "";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                locationDistrictId = dt.Rows[0]["LocationDistrictId"].ToString();
            }
            
            return locationDistrictId;
        }


        public static bool districtExists(string code)
        {
            string sqlStr = "SELECT * FROM District where DistrictSeq = @code "; //'" + code + "'";
            DBConnHelper DBConnHelper = new DBConnHelper();

            SqlCommand toExecute;

            toExecute = new SqlCommand(sqlStr);

            toExecute.Parameters.AddWithValue("code", code);

            DataSet ds = DBConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            return false;
        }

        public static bool specialZone(string hmisCode)
        {
            string cmd = " select * from facility where districtid = @hmisCode and aggregationlevelId = 4  " +
                          " and districtid in (select districtid from facility where facilitytypeid = 9) ";

            DBConnHelper DBConnHelper = new DBConnHelper();

            SqlCommand toExecute;

            toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = DBConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            return false;
        
        }

        public static bool specialZoneNoWoreda(string hmisCode)
        {
            string cmd = " select * from facility where districtid = @hmisCode and aggregationlevelId = 4  " +
                          " and districtid in (select districtid from facility where facilitytypeid = 9) " +
                          " and districtId not in (select zoneId from EthEhmis_HmisZoneDistrict) ";

            DBConnHelper DBConnHelper = new DBConnHelper();

            SqlCommand toExecute;

            toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = DBConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            return false;

        }


        public static bool specialRegion(string hmisCode)
        {            
            bool specialRegion = false;           

            return specialRegion;

        }

        public static bool notWoreda(string hmisCode)
        {
            string cmd = " select * from district where districtSeq = @hmisCode and notWoreda = 1 ";

            DBConnHelper DBConnHelper = new DBConnHelper();

            SqlCommand toExecute;

            toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = DBConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            return false;
        }

        public static string getOneUp(string hmisCode)
        {          
            string cmd = " select * from facility where facilityname  ";
            string oneUpHmisCode = "";

            if (getFacilityTypeId(hmisCode) == 11) // Region
            {
                oneUpHmisCode = "20";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                oneUpHmisCode = "20";              
            }
            else if (getFacilityTypeId(hmisCode) == 9) // Zone
            {
                oneUpHmisCode = getRegionId(hmisCode).ToString();
            }
            else if (getFacilityTypeId(hmisCode) == 8) // District
            {
                oneUpHmisCode = getZoneId(hmisCode).ToString();

                if (oneUpHmisCode == "0") // No zone
                {
                    oneUpHmisCode = getRegionId(hmisCode).ToString();
                }
            }
            else if (getFacilityTypeId(hmisCode) < 7) // Facility
            {
                oneUpHmisCode = getDistrictId(hmisCode);
            }
          
            return oneUpHmisCode;
        }

        public static string getTwoUp(string hmisCode)
        {
            string oneUpHmisCode = getOneUp(hmisCode);
            string twoUpHmisCode = getOneUp(oneUpHmisCode);

            return twoUpHmisCode;
        }

        public static string getEmailFromHmisCode(string hmisCode, out string passWord)
        {
            string cmd = " select email, password from EthHmis_EmailList where hmiscode = @hmisCode "; //'" + hmisCode + "'";
            string email = "";
            passWord = "";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("hmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0) // There is Zone
            {
                email = dt.Rows[0]["email"].ToString();
                passWord = dt.Rows[0]["password"].ToString();
            }

            return email;
        }

        public static Hashtable getRegions(out ArrayList listRegionCodes)
        {
            string cmd = " select * from province where name not like '%Federal%' order by name ";

             Hashtable regionCodeName = new Hashtable();
            listRegionCodes = new ArrayList();
            string regionName = "";
            string regionCode = "";

            SqlCommand toExecute = new SqlCommand(cmd);            

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            Hashtable allZoneHash = new Hashtable();
            ArrayList allZoneCodeList = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {                
                regionName = row["Name"].ToString();
                regionCode = row["Code"].ToString();

                listRegionCodes.Add(regionCode);
                regionCodeName.Add(regionCode, regionName);              
            }

            return regionCodeName;
        }

        public static bool regionHasZone(string regionCode)
        {
            string cmd = "select * from EthEhmis_HmisZone where regionId = " + regionCode;

            bool hasZone = false;

            SqlCommand toExecute = new SqlCommand(cmd);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                hasZone = true;
            }

            return hasZone;
        }

        public static DataTable getAllLowerZonesWoredas(string hmisCode)
        {
            string cmd = "";        
            if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and " +
                      " (facilityTypeId = 9 or facilityTypeId = 8)  " +
                      " order by facilityTypeId desc, " +
                      " facilityName asc ";
            }            


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];          

            return dt;

        }

        public static DataTable getAllLowerZones(string hmisCode)
        {
            string cmd = "";
            if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and facilityTypeId = 9 " +
                      " order by facilityName asc ";
            }


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            return dt;

        }

        public static DataTable getAllLowerWoredas(string hmisCode)
        {
            string cmd = "";
            if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and facilityTypeId = 8 " +
                      " order by facilityName asc ";
            }
            else if (getFacilityTypeId(hmisCode) == 9) // Zone
            {
                cmd = " select * from facility  " +
                      " inner join EthEhmis_HmisZoneDistrict  " +
                      " on facility.hmiscode = cast(EthEhmis_HmisZoneDistrict.districtSeq as varchar(max)) " +
                      " where zoneId = @mainHmisCode " +
                      " order by facilityName asc ";
            }


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            return dt;

        }

        public static DataTable getAllLowerHCs(string hmisCode)
        {
            string cmd = "";
            if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and facilityTypeId = 2 " +
                      " order by facilityName asc ";
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where zoneId = @mainHmisCode and facilityTypeId = 2 " +
                      " order by facilityName asc ";
            }
            else if ((getFacilityTypeId(hmisCode) == 8) // Woreda
                     || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select * from facility where districtID= @mainHmisCode " +
                      " and facilityTypeId = 2 " +
                      " order by facilityName asc ";
            }


            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0]; 

            return dt;

        }

        public static DataTable getAllLowerHospHPs(string hmisCode)
        {
            string cmd = "";
            string facilityTypeString =
                 " ( facilityTypeId = 1 or facilityTypeId = 3 or facilityTypeId = 4 " +
                      "  or facilityTypeId = 5 or facilityTypeId = 6 or facilityTypeId = 7 " +
                      "   or facilityTypeId >= 50 ) ";

            if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictID " +
                      " where district.provincecode = @mainHmisCode and " +
                      facilityTypeString +
                      //" (facilityTypeId = 1 or facilityTypeId = 3 or facilityTypeId = 4 " +
                      //"  or facilityTypeId = 5 or facilityTypeId = 6 or facilityTypeId = 7)  " +
                      " order by facilityName asc ";
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select * from facility inner join District on  " +
                      " District.DistrictSeq = Facility.LocationDistrictID " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.LocationDistrictID " +
                      " where zoneID = @mainHmisCode and " +
                      facilityTypeString +
                      //" (facilityTypeId = 1 or facilityTypeId = 3 or facilityTypeId = 4 " +
                      //"  or facilityTypeId = 5 or facilityTypeId = 6 or facilityTypeId = 7)  " +
                      " order by facilityName asc ";
            }
            else if ((getFacilityTypeId(hmisCode) == 8) // Woreda
           || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select * from facility where districtID= @mainHmisCode " +
                    facilityTypeString +
                    //" and  (facilityTypeId = 1 or facilityTypeId = 3 or facilityTypeId = 4 " +
                    //"  or facilityTypeId = 5 or facilityTypeId = 6 or facilityTypeId = 7)  " +
                      " order by facilityName asc ";
            }
            else
            {
                cmd = " select * from facility where districtID= @mainHmisCode order by facilityName asc ";
            }



            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];         

            return dt;

        }

        public static DataTable getLowerFacilities(string hmiscode)
        {
            string cmd = " select * from facility where districtId = @mainHmisCode and facilitytypeid < 8  " +
                         " order by facilityName";

            SqlCommand toExecute = new SqlCommand(cmd);

            toExecute.Parameters.AddWithValue("mainHmisCode", hmiscode);

            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            DataTable dt = ds.Tables[0];

            return dt;
        }

        public static string getcountAllLowerZones(string hmisCode)
        {
            string cmd = "";
            string getZoncount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getZoncount from facility " +
                          " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                          " where (facilityTypeId = 9) and FacilityOwner.FacilityOwnerId = 1";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getZoncount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and facilityTypeId = 9 ";
            }

            if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 10))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {
                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        getZoncount = row["getZoncount"].ToString();
                    }
                }

            }
            return getZoncount;

        }
        public static string getcountAllLowerHPs(string hmisCode)
        {
            string cmd = "";
            string getHPcount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getHPcount from facility " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " where (facilityTypeId = 3) and FacilityOwner.FacilityOwnerId = 1 ";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getHPcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictID " +
                      " where district.provincecode = @mainHmisCode and " +
                      " (facilityTypeId = 3) ";
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select  count(*) as getHPcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.LocationDistrictID " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.LocationDistrictID " +
                      " where zoneID = @mainHmisCode and " +
                      " (facilityTypeId = 3)";
            }
            else if ((getFacilityTypeId(hmisCode) == 8) // Woreda
           || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select  count(*) as getHPcount from facility where districtID= @mainHmisCode " +
                      " and  (facilityTypeId = 3)  ";
            }

            if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 10) || ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))
                || ((getFacilityTypeId(hmisCode) == 8)
           || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode)))))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {
                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        getHPcount = row["getHPcount"].ToString();
                    }
                }
            }
            return getHPcount;


        }
        public static string getcountAllLowerWoredas(string hmisCode)
        {
            string cmd = "";
            string getWorcount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getWorcount from facility " +
                          " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                          " where (facilityTypeId = 8) and FacilityOwner.FacilityOwnerId = 1 ";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getWorcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and facilityTypeId = 8 ";
            }
            else if (getFacilityTypeId(hmisCode) == 9) // Zone
            {
                cmd = " select count(*) as getWorcount from facility  " +
                      " inner join EthEhmis_HmisZoneDistrict  " +
                      " on facility.hmiscode = cast(EthEhmis_HmisZoneDistrict.districtSeq as varchar(max)) " +
                      " where zoneId = @mainHmisCode ";
            }
            if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 9) || (getFacilityTypeId(hmisCode) == 10))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {

                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        getWorcount = row["getWorcount"].ToString();
                    }

                }

            }

            return getWorcount;
        }
        public static string getcountAllLowerHCs(string hmisCode)
        {
            string cmd = "";
            string getHCcount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getHCcount from facility " +
                          " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                          " where (facilityTypeId = 2) and FacilityOwner.FacilityOwnerId = 1 ";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getHCcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where district.provincecode = @mainHmisCode and facilityTypeId = 2 ";
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select count(*) as getHCcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictId " +
                      " where zoneId = @mainHmisCode and facilityTypeId = 2 ";
            }
            else if ((getFacilityTypeId(hmisCode) == 8) // Woreda
                     || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select count(*) as getHCcount from facility where districtID= @mainHmisCode " +
                      " and facilityTypeId = 2 ";
            } if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 10) ||
              ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode))) ||
            ((getFacilityTypeId(hmisCode) == 8) // Woreda
                   || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode)))))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {

                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];


                    foreach (DataRow row in dt.Rows)
                    {
                        getHCcount = row["getHCcount"].ToString();
                    }
                }
            }
            return getHCcount;


        }

        public static string getcountAllLowerGovHosp(string hmisCode)
        {
            string cmd = "";
            string getHopcount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getHopcount from facility " +
                          " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                          " where(facilityTypeId = 1) and FacilityOwner.FacilityOwnerId = 1 ";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getHopcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictID " +
                      " where district.provincecode = @mainHmisCode and " +
                      " (facilityTypeId = 1) " +
                      " and FacilityOwner.FacilityOwnerId = 1 "; ;
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select count(*) as getHopcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.LocationDistrictID " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.LocationDistrictID " +
                      " where zoneID = @mainHmisCode and " +
                      " (facilityTypeId = 1 )  " +
                      " and FacilityOwner.FacilityOwnerId = 1 "; ;
            }
            else if ((getFacilityTypeId(hmisCode) == 8) // Woreda
           || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select count(*) as getHopcount from facility " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " where districtID= @mainHmisCode " +
                      " and  (facilityTypeId = 1 )" +
                      " and FacilityOwner.FacilityOwnerId = 1 ";
            }

            if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 10) || ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))
                || ((getFacilityTypeId(hmisCode) == 8) || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode)))))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {
                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];


                    foreach (DataRow row in dt.Rows)
                    {
                        getHopcount = row["getHopcount"].ToString();
                    }
                }
            }
            return getHopcount;

        }
        public static string getcountAllLowerPriHosp(string hmisCode)
        {
            string cmd = "";
            string getHopcount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getHopcount from facility " +
                          " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                          " where (facilityTypeId = 1) and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getHopcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictID " +
                      " where district.provincecode = @mainHmisCode and " +
                      " (facilityTypeId = 1) " +
                      " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select count(*) as getHopcount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.LocationDistrictID " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.LocationDistrictID " +
                      " where zoneID = @mainHmisCode and " +
                      " (facilityTypeId = 1 )  " +
                      " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }
            else if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 8) // Woreda
           || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select count(*) as getHopcount from facility " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " where districtID= @mainHmisCode " +
                      " and  (facilityTypeId = 1)" +
                      " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }

            if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 10) || ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))
                || ((getFacilityTypeId(hmisCode) == 8) || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode)))))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {
                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];


                    foreach (DataRow row in dt.Rows)
                    {
                        getHopcount = row["getHopcount"].ToString();
                    }
                }
            }
            return getHopcount;


        }
        public static string getcountAllLowerPriClinic(string hmisCode)
        {
            string cmd = "";
            string getClincount = "";

            if (getFacilityTypeId(hmisCode) == 11) //Federal or EHNRI
            {
                cmd = " select count(*) as getClincount from facility " +
                          " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                          " where  ((facilityTypeId = 4) or (facilityTypeId = 6))" +
                       " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }
            else if (getFacilityTypeId(hmisCode) == 10) // Region
            {
                cmd = " select count(*) as getClincount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.DistrictId " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.DistrictID " +
                      " where district.provincecode = @mainHmisCode " +
                       " and  ((facilityTypeId = 4) or (facilityTypeId = 6))" +
                      " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }
            else if ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))// Zone
            {
                cmd = " select count(*) as getClincount from facility inner join District on  " +
                      " District.DistrictSeq = Facility.LocationDistrictID " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " left join EthEhmis_HmisZoneDistrict on " +
                      " EthEhmis_HmisZoneDistrict.DistrictSeq = Facility.LocationDistrictID " +
                      " where zoneID = @mainHmisCode  " +
                      " and  ((facilityTypeId = 4) or (facilityTypeId = 6))" +
                      " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }
            else if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 8) // Woreda
           || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode))))
            {
                cmd = " select count(*) as getClincount from facility " +
                      " inner join FacilityOwner on FacilityOwner.FacilityOwnerId =  Facility.FacilityOwnerId   " +
                      " where districtID= @mainHmisCode " +
                      " and  ((facilityTypeId = 4) or (facilityTypeId = 6))" +
                      " and (FacilityOwner.FacilityOwnerId = 2 or FacilityOwner.FacilityOwnerId = 3 or FacilityOwner.FacilityOwnerId = 4" +
                      " or FacilityOwner.FacilityOwnerId = 5 or FacilityOwner.FacilityOwnerId = 6 or FacilityOwner.FacilityOwnerId = 7" +
                      " or FacilityOwner.FacilityOwnerId = 8 or FacilityOwner.FacilityOwnerId = 9) ";
            }

            if ((getFacilityTypeId(hmisCode) == 11) || (getFacilityTypeId(hmisCode) == 10) || ((getFacilityTypeId(hmisCode) == 9) && (!specialZoneNoWoreda(hmisCode)))
                || ((getFacilityTypeId(hmisCode) == 8) || ((getFacilityTypeId(hmisCode) == 9) && (specialZoneNoWoreda(hmisCode)))))
            {
                using (DBConnHelper dbConnHelper = new DBConnHelper())
                {
                    SqlCommand toExecute = new SqlCommand(cmd);

                    toExecute.Parameters.AddWithValue("mainHmisCode", hmisCode);

                    DataSet ds = dbConnHelper.GetDataSet(toExecute);

                    DataTable dt = ds.Tables[0];


                    foreach (DataRow row in dt.Rows)
                    {
                        getClincount = row["getClincount"].ToString();
                    }
                }
            }
            return getClincount;


        }

        public static string getToEmail(string hmisCode)
        {
            string password = "";
            string oneUpHmisCode = getOneUp(hmisCode);
            string emailAddress = getEmailFromHmisCode(oneUpHmisCode, out password);

            return emailAddress;
        }

        public static string getCcEmail(string hmisCode)
        {
            string password = "";
            string twoUpHmisCode = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getTwoUp(hmisCode);
            string emailAddress = Admin.SiteSetting.DataAccess.SiteSettingDataAccess.getEmailFromHmisCode(twoUpHmisCode, out password);

            return emailAddress;
        }


    }
}
