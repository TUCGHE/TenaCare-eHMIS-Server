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
using System.Data.SqlClient;
using System.Collections;
using eHMIS.HMIS.ReportViewing;
using eHMIS.HMIS.ReportHelper;
using eHMIS.HMIS.ReportAggregation.NewReporting;
using SqlManagement.Database;

namespace eHMIS.HMIS.ReportAggregation
{
    public class HMISReportGeneratorQRODR
    {

        //SqlConnection mycon
        public static SqlConnection myconn = new SqlConnection();

        private string _locationHMISCode;
        private decimal _selectedYear;
        private int _selectedMonth;
        private int _selectedQuarter;
        private int _selectedWeek;
        private ReportPeriod _reportPeriod;


        private int _selectedlocationLevel;  // 0-facility , 1-woreda, 2-zone, 3-region, 4-FMoH
        private int _selectedlocationType;   // 0-health center, 1-hospital , 2-health post, 3-Woreda HO
        // 4-zone HD, 5-region HB, 6-FMoH
        private int _selectedRegionID;    //
        private int _selectedZoneID;      // 
        private int _selectedWoredaID;   //

        List<string> _includedList = new List<string>();



        //private string connectionString = eHMIS.Framework.Sql.SqlManager.Instance.SqlConnection._connectionString;
        private string connectionString = "";
        private string _locationSettStat;
       
        private  int _startingMonth=0;
        private string _submittedLocations;

        private DBConnHelper DBConnHelper = new DBConnHelper();
        public HMISReportGeneratorQRODR(string locID, int locType, int locLevel, int regID, 
                    decimal year, int quarter)
        {
            _locationHMISCode = locID;
            _selectedlocationType = locType;
            _selectedlocationLevel = locLevel;

            _selectedRegionID = regID;
            

            _selectedYear = year;
            _selectedQuarter = quarter;

            this.setLocationSelectString();
            this.setStartingMonth();
              
        }


         
        
        public List<string> IncludedList
        {
            get { return _includedList; }
            set { _includedList = value; }
        }


        public HMISReportGeneratorQRODR(string locID, int locType, int locLevel, int regID,
           ReportPeriod repPeriod)
        {
            _locationHMISCode = locID;
            _selectedlocationType = locType;
            _selectedlocationLevel = locLevel;

            _selectedRegionID = regID;

            _reportPeriod = repPeriod;
        
            this.setLocationSelectString();
        
       }


        private void setStartingMonth()
        {

            switch (_selectedQuarter){

                case 1: _startingMonth = 11;break;
                case 2: _startingMonth = 2;break;
                case 3: _startingMonth = 5;break;
                case 4: _startingMonth = 8;break;
            }
        }


        public void newGenerateReport(reportObject reportObj)
        {           

            // New Report Generator

            if (reportObj.RepKind == reportObject.ReportKind.Weekly_Disease)
            {
                NewWeeklyCaseListReportAggr newReport = new NewWeeklyCaseListReportAggr(reportObj);
                newReport.startReportTableGeneration(true);
                //reportObj.SubmittingInstId = newReport.GetIncludedLocations();

                newReport.UpdateHashTable();
            }
            else
            {
                NewDiseaseReportAggregation newReport = new NewDiseaseReportAggregation(reportObj);
                newReport.startReportTableGeneration(true);
                reportObj.SubmittingInstId = newReport.GetIncludedLocations();

                newReport.UpdateHashTable();
            }

            // newReport.startReportTableGeneration(true);

            // Populate the included List of the 
           


        }

        private void setLocationSelectString()
        {
            switch (_selectedlocationType)
            {
                case 0: _locationSettStat = " and (Healthcenter=1)"; break;
                case 1: _locationSettStat = " and (Hospital=1)"; break;
                case 2: _locationSettStat = " and (Healthpost=1)"; break;
                case 3: _locationSettStat = " and (WoredaHO=1)"; break;
                case 4: _locationSettStat = " and (ZonalHD=1)"; break;
                case 5: _locationSettStat = " and (RegionalHB=1)"; break;
                case 6: _locationSettStat = " and (FMOH=1)"; break;
            }

        }



        public void Con()
        {

            myconn.Close();
            myconn.ConnectionString = connectionString;
            try
            {
                myconn.Open();

            }
            catch
            {                
            }
        }      

        private bool hasIndicators(int gID)
        {
           Con();

           string selectStatment = "SELECT LabelID, LabelOrder, LabelName as [S.NO], LabelDescription FROM vw_EthEhmis_HMISLabelPeriod  WHERE GroupID=" + gID + _locationSettStat + "ORDER BY LabelOrder, LabelID";
           SqlDataAdapter KeyAcAd = new SqlDataAdapter(selectStatment, myconn);
           DataSet KeyAcDs = new DataSet();
           KeyAcAd.Fill(KeyAcDs, "vw_EthEhmis_HMISLabelPeriod");
           myconn.Close();
           DataTable KeyAcTa = new DataTable();
           KeyAcTa = KeyAcDs.Tables["vw_EthEhmis_HMISLabelPeriod"];

           if (KeyAcTa.Rows.Count == 0)
           {
               return false;
           }
           return true;
        }

      


      
        private int getStartingMonth(int quarter)
        {

            switch (quarter)
            {

                case 1: return 11;
                case 2: return 2;
                case 3: return 5;
                case 4: return 8;
            }
            return 11;
        }
     
     
        int getFacilityType(string hmisCode)
        {

            Con();

            string selectStatment = "SELECT [AggregationLevelId],[FacilityOwnerId],[HealthCentreTypeId]," +
               "[FacilityTypeId]  FROM  [Facility] where [HMISCode] = '" + hmisCode + "'";

            SqlDataAdapter KeyAcAd = new SqlDataAdapter(selectStatment, myconn);
            DataSet KeyAcDs = new DataSet();
            KeyAcAd.Fill(KeyAcDs, "Facility");
            myconn.Close();
            DataTable KeyAcTa = new DataTable();
            KeyAcTa = KeyAcDs.Tables["Facility"];

            if (KeyAcTa.Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(KeyAcTa.Rows[0]["FacilityTypeId"]);
            }


        }

    }
}
