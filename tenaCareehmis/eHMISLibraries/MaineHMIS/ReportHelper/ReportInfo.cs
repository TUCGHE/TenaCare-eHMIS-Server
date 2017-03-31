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
using System.Data.SqlClient;
using System.Data;
using SqlManagement.Database;

namespace eHMIS.HMIS.ReportHelper
{
    public class ReportInfo
    {
        
        private string _institutionName;
        private string _woreda;
        private string _zone;
        private string _region;

        private string _reportingTo;

        private string _month;

        private int _week;
        private int _monthint;

        private int _quarter;

        private int _year;

        private string _recivedMethod;


        private string _reportName;
         


        DateTime _receivedDate;
        string _receivedDateEC;
        string _expectedDate;
        bool _timeliness;
        private string _completness;

        public int Week
        {
            get { return _week; }
            set { _week = value; }
        }

        public string InstitutionName
        {
            get { return _institutionName; }
            set { _institutionName = value; }
        }



        public string ReportingTo
        {
            get { return _reportingTo; }
            set { _reportingTo = value; }
        }

        public string ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }



        public DateTime ReceivedDateGC
        {
            get { return _receivedDate; }
            set { _receivedDate = value; }
        }

        public string ReceivedDateEC
        {
            get { return _receivedDateEC; }
            set { _receivedDateEC = value; }
        }

        public string ExpectedDateEC
        {
            get { return _expectedDate; }
            set { _expectedDate = value; }
        }


        //public string Woreda
        //{
        //    get { return _woreda; }
        //    set { _woreda = value; }
        //}


        //public string Zone
        //{
        //    get { return _zone; }
        //    set { _zone = value; }
        //}


        //public string Region
        //{
        //    get { return _region; }
        //    set { _region = value; }
        //}


        public string Month
        {
            get { return _month; }
            set { _month = value; }
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

        public string RecivedMethod
        {
            get { return _recivedMethod; }
            set { _recivedMethod = value; }
        }

        public string Completness
        {
            get { return _completness; }
            set { _completness = value; }
        }

        public bool Timeliness
        {
            get { return _timeliness; }
            set { _timeliness = value; }
        }


        private string _lastUpdate;

        public string LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        public ReportInfo(string name, string reporTo,
                          int month, int quar, int year, string repName, 
                          string recMethod, string complet, string lastUpdate, int week)
        {
            _week = week;
            _institutionName = name;
            _reportingTo = reporTo;
            _monthint = month;
            _month = getMonthName(_monthint);
            _quarter = quar;
            _year = year;
            _reportName = repName;
            _recivedMethod = recMethod;
            _completness = complet;

            _lastUpdate = lastUpdate;
            DateTime updateDate;
            if (DateTime.TryParse(_lastUpdate, out updateDate))
            {
                _receivedDate = updateDate; // Convert.ToDateTime(_lastUpdate);
                setEthiopanRecivedDate(_receivedDate);

                if (HMISMainPage.UseNewServiceDataElement2014 == true)
                {
                    setExpectedDateReportNameNew2014();
                    setTimelinessNew2014();
                }
                else
                {
                    setExpectedDateReportName();
                    setTimeliness();
                }
            }




        }

        public ReportInfo(string name, string woreda, string zone, string region,
                   string month, int quar, int year, string repName, string complet)
        {

            _institutionName = name;
            _woreda = woreda;
            _zone = zone;
            _region = region;
            _month = month;
            _quarter = quar;
            _year = year;
            _recivedMethod = repName;

            _completness = complet;




        }
      
        public ReportInfo()
        {
        }

        public ReportInfo(string name, string reporTo,
                   int month, int quar, int year, string repName, string recMethod, string complet, string lastUpdate)
        {

            _institutionName = name;
            _reportingTo = reporTo;
            _monthint = month;
            _month = getMonthName(_monthint);
            _quarter = quar;
            _year = year;
            _reportName = repName;
            _recivedMethod = recMethod;
            _completness = complet;

            _lastUpdate = lastUpdate;
            DateTime updateDate;
            if (DateTime.TryParse(_lastUpdate, out updateDate))
            {
                _receivedDate = updateDate; // Convert.ToDateTime(_lastUpdate);
                setEthiopanRecivedDate(_receivedDate);

                if (HMISMainPage.UseNewServiceDataElement2014 == true)
                {
                    setExpectedDateReportNameNew2014();
                    setTimelinessNew2014();
                }
                else
                {
                    setExpectedDateReportName();
                    setTimeliness();
                }
            }


            //try
            //{
            //    DateTime.TryParse(_lastUpdate, out updateDate);
            //    _receivedDate = Convert.ToDateTime(_lastUpdate);
            //    setEthiopanRecivedDate(_receivedDate);

            //    setExpectedDateReportName();
            //    setTimeliness();
            //}
            //catch
            //{

            //}

        }




        public string getMonthName(int monthno)
        {
            string monthStr = "";

            switch (monthno)
            {

                case 11: monthStr = "Hamle"; break;
                case 12: monthStr = "Nehase"; break;
                case 1: monthStr = "Meskerem"; break;

                case 2: monthStr = "Tikimt"; break;
                case 3: monthStr = "Hidar"; break;
                case 4: monthStr = "Tahisas"; break;

                case 5: monthStr = "Tir"; break;
                case 6: monthStr = "Yekatit"; break;
                case 7: monthStr = "Megabit"; break;

                case 8: monthStr = "Miyaza"; break;
                case 9: monthStr = "Ginbot"; break;
                case 10: monthStr = "Sene"; break;
            }

            return monthStr;

        }




        string formatECDate(int day, int month, int year)
        {


            return (day.ToString() + "/" + month.ToString() + "/" +
                                    year.ToString());


        }

        private void setEthiopanRecivedDate(DateTime recDate)
        {


            General.Util.Ethiopia.Day _selDay = General.Util.Ethiopia.Calendar.GregorianToEthiopian(recDate.Month, recDate.Day, recDate.Year);

            _receivedDateEC = formatECDate(_selDay.getDate(), _selDay.getMonth(),
                                 _selDay.getYear());
   
        }


        private void setExpectedDateReportName()
        {
            string seleRepDate = "SELECT [ReportName],[ReportPeriod] ,[ReportDate]" +
                                  " FROM  [dbo].[EthEhmis_HMISReportType] where [ReportName] like '" + _reportName + "'";
            using (DBConnHelper dbConnHelper = new DBConnHelper())
            {

                DataSet ds = dbConnHelper.GetDataSet(seleRepDate);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    _reportName = Convert.ToString(ds.Tables[0].Rows[0]["ReportName"]);


                    int repPeriod = Convert.ToInt32(ds.Tables[0].Rows[0]["ReportPeriod"]);
                    int expDate = Convert.ToInt32(ds.Tables[0].Rows[0]["ReportDate"]);
                    if (repPeriod == 1)
                    {
                        if (_monthint < 11)
                        {
                            _expectedDate = formatECDate(expDate, _monthint + 1, _year);// new DateTime(_year, _monthint + 1, expDate);
                        }
                        else if (_monthint == 11)
                        {
                            _expectedDate = formatECDate(expDate, 12, _year);//new DateTime(_year, 12, expDate);
                        }
                        if (_monthint == 12)
                        {
                            _expectedDate = formatECDate(expDate, 1, _year + 1);// new DateTime(_year+1, 1, expDate);
                        }

                    }
                    else if (repPeriod == 2)
                    {
                        switch (_quarter)
                        {

                            case 1: _expectedDate = formatECDate(expDate, 2, _year); break;// new DateTime(_year, 2, expDate); break;
                            case 2: _expectedDate = formatECDate(expDate, 5, _year); break;//new DateTime(_year, 5, expDate); break;
                            case 3: _expectedDate = formatECDate(expDate, 8, _year); break;//new DateTime(_year, 8, expDate); break;
                            case 4: _expectedDate = formatECDate(expDate, 11, _year); break;//new DateTime(_year, 11, expDate); break;
                        }

                    }
                    else if (repPeriod == 3)
                    {

                        _expectedDate = formatECDate(expDate, 1, _year + 1);// new DateTime(_year+1, 1, expDate);
                    }
                }
            }
        }

        private void setTimeliness()
        {
            string[] datesRecive = _receivedDateEC.Split('/');

            string[] datesexpected = _expectedDate.Split('/');


            if ((datesRecive.Length != 3) && (datesexpected.Length != 3))
            {

                _timeliness = false;
            }
            else if (String.Compare(datesexpected[2], datesRecive[2]) > 0)
            {
                _timeliness = true;
            }
            else if ((String.Compare(datesRecive[2], datesexpected[2]) == 0) &&
           (String.Compare(datesexpected[1], datesRecive[1]) < 0))
            {
                _timeliness = true;
            }
            else if ((String.Compare(datesRecive[2], datesexpected[2]) == 0) &&
                  (String.Compare(datesRecive[1], datesexpected[1]) == 0)
                & (String.Compare(datesRecive[0], datesexpected[0]) == 0))
            {

                _timeliness = true;
            }

            else
            {
                _timeliness = false;
            }
        }


        private void setExpectedDateReportNameNew2014()
        {
            string seleRepDate = "SELECT [ReportName],[ReportPeriod] ,[ReportDate]" +
                                  " FROM  [dbo].[EthEhmis_HMISReportTypeNew2014] where [ReportName] like '" + _reportName + "'";
            DBConnHelper DBConnHelper = new DBConnHelper();

            DataSet ds = DBConnHelper.GetDataSet(seleRepDate);

            if (ds.Tables[0].Rows.Count > 0)
            {
                _reportName = Convert.ToString(ds.Tables[0].Rows[0]["ReportName"]);


                int repPeriod = Convert.ToInt32(ds.Tables[0].Rows[0]["ReportPeriod"]);
                int expDate = Convert.ToInt32(ds.Tables[0].Rows[0]["ReportDate"]);
                if (repPeriod == 1)
                {
                    if (_monthint < 11)
                    {
                        if(expDate == 26)
                            _expectedDate = formatECDate(expDate, _monthint, _year);// new DateTime(_year, _monthint + 1, expDate);
                        else
                            _expectedDate = formatECDate(expDate, _monthint + 1, _year);// new DateTime(_year, _monthint + 1, expDate);
                   }
                    else if (_monthint == 11)
                    {
                        if (expDate == 26)
                            _expectedDate = formatECDate(expDate, 11, _year);//new DateTime(_year, 12, expDate);
                        else
                            _expectedDate = formatECDate(expDate, 12, _year);//new DateTime(_year, 12, expDate);
                    } 
                    if (_monthint == 12)
                    {
                        if (expDate == 26)
                            _expectedDate = formatECDate(expDate, 12, _year);// new DateTime(_year+1, 1, expDate);
                        else
                            _expectedDate = formatECDate(expDate, 1, _year + 1);// new DateTime(_year+1, 1, expDate);
                    }

                }
                else if (repPeriod == 2)
                {
                    if (expDate == 26)
                    {
                        switch (_quarter)
                        {

                            case 1: _expectedDate = formatECDate(expDate, 1, _year); break;// new DateTime(_year, 2, expDate); break;
                            case 2: _expectedDate = formatECDate(expDate, 4, _year); break;//new DateTime(_year, 5, expDate); break;
                            case 3: _expectedDate = formatECDate(expDate, 7, _year); break;//new DateTime(_year, 8, expDate); break;
                            case 4: _expectedDate = formatECDate(expDate, 10, _year); break;//new DateTime(_year, 11, expDate); break;
                        }
                    }
                    else
                    {
                        switch (_quarter)
                        {

                            case 1: _expectedDate = formatECDate(expDate, 2, _year); break;// new DateTime(_year, 2, expDate); break;
                            case 2: _expectedDate = formatECDate(expDate, 5, _year); break;//new DateTime(_year, 5, expDate); break;
                            case 3: _expectedDate = formatECDate(expDate, 8, _year); break;//new DateTime(_year, 8, expDate); break;
                            case 4: _expectedDate = formatECDate(expDate, 11, _year); break;//new DateTime(_year, 11, expDate); break;
                        }
                    }

                }
                else if (repPeriod == 3)
                {

                    _expectedDate = formatECDate(expDate, 1, _year + 1);// new DateTime(_year+1, 1, expDate);
                }
            }

        }

        private void setTimelinessNew2014()
        {
            string[] datesRecive = _receivedDateEC.Split('/');
            string[] datesexpected = _expectedDate.Split('/');

            int dayRecive = 0;
            int dayExpected =0;
            int monthExpected = 0;
            int yearRecive = 0;
            int yearExpected =0;
            int monthRecive = 0;

            int.TryParse(datesexpected[0], out dayExpected);
            int.TryParse(datesexpected[1], out monthExpected);
            int.TryParse(datesexpected[2], out yearExpected);
            
            
            int.TryParse(datesRecive[0], out dayRecive);
            int.TryParse(datesRecive[1], out monthRecive);
            int.TryParse(datesRecive[2], out yearRecive);
            
            

            if ((datesRecive.Length != 3) && (datesexpected.Length != 3))
            {
                _timeliness = false;
            }
            else if (yearExpected > yearRecive)
            {
                _timeliness = true;
            }
            else if (yearRecive == yearExpected && monthRecive < monthExpected)
            {
                _timeliness = true;
            }
            else if (yearRecive == yearExpected &&
                    monthRecive == monthExpected &&
               dayRecive <= dayExpected)
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
