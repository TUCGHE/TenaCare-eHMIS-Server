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
using System.Data;
using General.Util.Ethiopia;
using SqlManagement.Database;
namespace eHMIS.HMIS.ReportViewing
{
   
     

    class ReceivedReport
    {
        int _sourceLocationID;
        string _sourceLocationName = "";
        DateTime _receivedDate;
        DateTime _receivedDateEC;
        DateTime _expectedDate;
        decimal _completness;
        bool _timeliness;
        int _reportType;
        string _reportName;
        int _quarter;
        int _month;
        int _year;

        private DBConnHelper DBConnHelper = new DBConnHelper();

         

        public string SourceLocationName
        {
            get { return _sourceLocationName; }
            set { _sourceLocationName = value; }
        }


      
        public DateTime ReceivedDateGC
        {
            get { return _receivedDate; }
            set { _receivedDate = value; }
        }

        public DateTime ReceivedDateEC
        {
            get { return _receivedDateEC; }
            set { _receivedDateEC = value; }
        }

        public DateTime ExpectedDateEC
        {
            get { return _expectedDate; }
            set { _expectedDate = value; }
        }

        public string ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }


        public int Quarter
        {
            get { return _quarter; }
            set { _quarter = value; }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public bool Timeliness
        {
            get { return _timeliness; }
            set { _timeliness = value; }
        }

        public Decimal Completness
        {
            get { return _completness; }
            set { _completness = value; }
        }

        public ReceivedReport(int locID, DateTime recDate, decimal completness, int repType, int mth, int qua, int yr)
        {
            _sourceLocationID = locID;
            _month = mth;
            _quarter = qua;
            _year = yr;
            _reportType = repType;
            _receivedDate = recDate;
            _completness = completness;
            setEthiopanRecivedDate(recDate);
            setLocationName();
            setExpectedDateReportName();
            setTimeliness();
           

        }

        private void setEthiopanRecivedDate(DateTime recDate)
        {


            General.Util.Ethiopia.Day _selDay = Calendar.GregorianToEthiopian( recDate.Month, recDate.Day, recDate.Year);


            _receivedDateEC = new DateTime(_selDay.getYear(), _selDay.getMonth(), _selDay.getDate()); 

        }




        private void setLocationName()
        {

            string seleFacilitName = " Select FacilityName from Facility  " +
                                      "where facility.hmiscode = '"+ _sourceLocationID+"'";

            DataSet ds = DBConnHelper.GetDataSet(seleFacilitName);

            if (ds.Tables[0].Rows.Count > 0)
            {
                _sourceLocationName = Convert.ToString(ds.Tables[0].Rows[0]["FacilityName"]);
            }
        }


        private void setExpectedDateReportName()
        {
            string seleRepDate = "SELECT [ReportName],[ReportPeriod] ,[ReportDate]"+
                                  " FROM  [dbo].[EthEhmis_HMISReportType] where [ReportID] = "+_reportType.ToString();
         
            DataSet ds = DBConnHelper.GetDataSet(seleRepDate);

            if (ds.Tables[0].Rows.Count > 0)
            {
                _reportName = Convert.ToString(ds.Tables[0].Rows[0]["ReportName"]);
              

                int repPeriod = Convert.ToInt32(ds.Tables[0].Rows[0]["ReportPeriod"]);
                int expDate = Convert.ToInt32(ds.Tables[0].Rows[0]["ReportDate"]);
                if (repPeriod == 2)
                {
                    switch (_quarter)
                    {

                        case 1: _expectedDate = new DateTime(_year, 1, expDate); break;
                        case 2: _expectedDate = new DateTime(_year, 4, expDate); break;
                        case 3: _expectedDate = new DateTime(_year, 7, expDate); break;
                        case 4: _expectedDate = new DateTime(_year, 11, expDate); break;
                    }

                } 
                else if (repPeriod == 3)
                {

                    _expectedDate = new DateTime(_year, 1, expDate);
                }
            }

        }

        private void setTimeliness()
        {
            if (_receivedDateEC <= _expectedDate)
            {
                _timeliness = true;
            }
            else
            {
                _timeliness = false;
            }
        }



    }
}
