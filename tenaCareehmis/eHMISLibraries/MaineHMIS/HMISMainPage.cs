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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using eHMIS.HMIS.ReportViewing;
using SqlManagement.Database;
using Admin.Security.Utility;
using System.Configuration.Assemblies;
using System.Configuration;
using System.Configuration.Internal;
using eHMIS;

namespace eHMIS.HMIS
{
    public partial class HMISMainPage 
    {
       

        public static Boolean mainLocationChanged = false;
        public static Boolean showFacilitySetup = false;
        public static Boolean showFacilityAdd = false;
        //private static MainForm _parent;
       
        private static int _reportMode = 1; // By default should be HMIS Report

        private static bool _useNewVersion=false;

        private static bool _useNewServiceDataElement2014 = false;

        private static bool _useNewServiceDataElementValidation = false;

        public static int ReportMode = 1;
        public static string SelectedLocationID = "-1";
        public static int SelectedRegionID = -1;
        public static int SelectedZoneID = -1;
        public static int SelectedWoredaID = -1;
        public static int SelectedLocationLevel = -1;

        //HMISMainPage.SelectedLocationType, HMISMainPage.SelectedLocationLevel, HMISMainPage.SelectedRegionID

        public static bool UseNewVersion
        {

            get { return _useNewVersion; }
            set { _useNewVersion = value; }

        }

        public static bool UseNewServiceDataElement2014
        {

            get { return _useNewServiceDataElement2014; }
            set { _useNewServiceDataElement2014 = value; }

        }

        public static bool UseNewServiceDataElementValidation
        {

            get { return _useNewServiceDataElementValidation; }
            set { _useNewServiceDataElementValidation = value; }

        }

        public enum locationLevel
        {
            facility = 0,
            woreda = 1,
            zone = 2,
            region = 3,
            FMoH = 4,
            undefined = 5,
        }

        public HMISMainPage()
        {            
            UseNewServiceDataElement2014 = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewServiceDataElement2014"]);
            UseNewServiceDataElementValidation = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewServiceDataElementValidation"]);                     
        }
        private void InitializeStaticComponets()
        {
            
        }
   
         
        public HMISMainPage(int inReportMode)
        {         
            UseNewServiceDataElement2014 = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewServiceDataElement2014"]);
            UseNewServiceDataElementValidation = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewServiceDataElementValidation"]);               
        }
      
        public HMISMainPage(int inReportMode, bool newVersion)
        {
          
            _useNewVersion = newVersion;

            
            UseNewServiceDataElement2014 = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewServiceDataElement2014"]);
            UseNewServiceDataElementValidation = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewServiceDataElementValidation"]);            

        }  

        private int getAggregationLevel(string locationID)
        {
            // Given the location ID returns the Facility Name
            string cmdText = "select AggregationLevelID from facility where hmiscode = @locationID";
            int aggregationLevel = 0;

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("locationID", locationID);
            DBConnHelper dbConnHelper = new DBConnHelper();
            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
            {
                aggregationLevel = Convert.ToInt16(ds.Tables[0].Rows[0]["AggregationLevelID"]);
            }

            return aggregationLevel;
        }

        private int getFacilityType(string locationID)
        {
            // Given the location ID returns the Facility Name
            string cmdText = "select FacilityTypeID from facility where hmiscode = @locationID";
            int facilityType = 0;

            SqlCommand toExecute = new SqlCommand(cmdText);

            toExecute.Parameters.AddWithValue("locationID", locationID);
            DBConnHelper dbConnHelper = new DBConnHelper();
            DataSet ds = dbConnHelper.GetDataSet(toExecute);

            if (ds.Tables[0].Rows.Count > 0)
            {
                facilityType = Convert.ToInt16(ds.Tables[0].Rows[0]["FacilityTypeID"]);
            }

            return facilityType;
        }  


    }
}





