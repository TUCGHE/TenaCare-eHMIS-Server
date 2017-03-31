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
using General.Util.Ethiopia;
using eHMIS.HMIS.ReportViewing;
using SqlManagement.Database;

namespace eHMIS.HMIS.ReportHelper
{
    public enum RecivedMethod
    {
        ON_PAPER = 0,
        BY_EMAIL,
        BY_PORTABLEMEDIA,
        BY_LOCALACCESS,
        FROM_EMR,
    }

    public enum ReportStatus
    {
        RECEIVED = 0,
        SUBMITTED,
    }

    public enum ReportName
    {
        UNKNOWN = 0,
        HealthPostMONTHLYOPDReport = 662,
        HealthPostMONTHLYIPDReport = 663,
        HealthPostMONTHLYServiceReport = 664,
        HealthPostQUARTERLYOPDReport = 62,
        HealthPostQUARTERLYIPDReport = 63,
        HealthPostQUARTERLYServiceReport = 64,
        HealthPostANNUALServiceReport = 65,
        FacilityMONTHLYOPDReport = 82,
        FacilityMONTHLYIPDReport = 83,
        FacilityMONTHLYServiceReport = 84,
        FacilityMONTHLYServiceAgrigatedReport = 85,
        FacilityQUARTERLYServiceAgrigatedReport = 86,
        FacilityHCMONTHLYOPDAggregateReport = 87,
        FacilityHCQUARTERLYOPDAggregateReport = 88,       
        FacilityQUARTERLYOPDReport = 2,
        FacilityQUARTERLYIPDReport = 3,
        FacilityQUARTERLYServiceReport = 4,
        FacilityANNUALServiceReport = 5,
        WHOQUARTERLYOPDReport = 22,
        WHOQUARTERLYIPDReport = 23,
        WHOQUARTERLYServiceReport = 24,
        WHOANNUALServiceReport = 25,
        ZHDQUARTERLYOPDReport = 32,
        ZHDQUARTERLYIPDReport = 33,
        ZHDQUARTERLYServiceReport = 34,
        ZHDANNUALServiceReport = 35,
        RHBQUARTERLYOPDReport = 42,
        RHBQUARTERLYIPDReport = 43,
        RHBQUARTERLYServiceReport = 44,
        RHBANNUALServiceReport = 45,
        FMOHQUARTERLYOPDReport = 52,
        FMOHQUARTERLYIPDReport = 53,
        FMOHQUARTERLYServiceReport = 54,
        FMOHANNUALServiceReport = 55,
        WHOMONTHLYOPDReport = 622,
        WHOMONTHLYIPDReport = 623,
        WHOMONTHLYServiceReport = 624,
        ZHDMONTHLYOPDReport = 632,
        ZHDMONTHLYIPDReport = 633,
        ZHDMONTHLYServiceReport = 634,
        RHBMONTHLYOPDReport = 642,
        RHBMONTHLYIPDReport = 643,
        RHBMONTHLYServiceReport = 644,
        FMOHMONTHLYOPDReport = 652,
        FMOHMONTHLYIPDReport = 653,
        FMOHMONTHLYServiceReport = 654,
        WHOOOwnMONTHLYServiceReport = 124,
        WHOOwnANNUALServiceReport = 125,
        ZHDOwnMONTHLYServiceReport = 134,
        ZHDOwnANNUALServiceReport = 135,
        RHBOwnMONTHLYServiceReport = 144,
        RHBOwnANNUALServiceReport = 145,
        FMOHOwnMONTHLYServiceReport = 154,
        FMOHOwnANNUALServiceReport = 155,
        WeeklyIDSReport = 1,
        PHEMCasebasedAFP=888,
        PHEMCasebasedLAB=555,
        PHEMCasebasedNNT=777,
        PHEMCasebasedCBR=999,
    }

    [Serializable]
    public class SavedReports
    {

        string _sourceLocationID;
        string _sourceLocationName = "";
        int _reportType;

        //ReportType _reportType;
        int _week;
        int _quarter;
        int _month;
        int _year;
        private int _locationLevel;

        private ReportStatus _reportStatus;

        private RecivedMethod _recivedMethod;

        private ReportName _reportName;


        private int _expectedDataElements;

        private int _filledDataEle;

        private decimal _completness;
        private string _dbID;

        private DBConnHelper DBConnHelper = new DBConnHelper();


        private HMISMainPage.locationLevel _currentLocationLevel;


        public HMISMainPage.locationLevel LocationLevel
        {
            get { return _currentLocationLevel; }
            set { _currentLocationLevel = value; }
        }

        public string SourceLocationName
        {
            get { return _sourceLocationName; }
            set { _sourceLocationName = value; }
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

        public ReportName ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }

        public SavedReports()
        {
        }

        public SavedReports(string locID, int locaLev, int repType, ReportName repName,int week, int mth, int qua, decimal yr, int filled, RecivedMethod recMethod, ReportStatus status)
        {
            _sourceLocationID = locID;
            _week = week;
            _month = mth;
            _quarter = qua;
            _year = Convert.ToInt32(yr);
            _reportType = repType;
            _reportName = repName;
            _reportStatus = status;
            _recivedMethod = recMethod;
            _locationLevel = locaLev;

            _currentLocationLevel = getLocationLevel(_locationLevel);
            _filledDataEle = filled;
            setExpectedDataElements();
            calculateCompletness();


        }

        private HMISMainPage.locationLevel getLocationLevel(int _locationLevel)
        {
            throw new NotImplementedException();
        }

        public SavedReports(string locID, int locaLev, int repType, ReportName repName, int mth, int qua, decimal yr, int filled, RecivedMethod recMethod, ReportStatus status)
        {
            _sourceLocationID = locID;
            _month = mth;
            _quarter = qua;
            _year = Convert.ToInt32(yr);
            _reportType = repType;
            _reportName = repName;
            _reportStatus = status;
            _recivedMethod = recMethod;
            _locationLevel = locaLev;

            _currentLocationLevel = getLocationLevel(_locationLevel);
            _filledDataEle = filled;
            setExpectedDataElements();
            calculateCompletness();


        }


        public SavedReports(string locID, int locaLev, int repType, ReportName repName, int mth, int qua, decimal yr, int filled,
            RecivedMethod recMethod, ReportStatus status, int expectData)
        {
            _sourceLocationID = locID;
            _month = mth;
            _quarter = qua;
            _year = Convert.ToInt32(yr);
            _reportType = repType;
            _reportName = repName;
            _reportStatus = status;
            _recivedMethod = recMethod;
            _locationLevel = locaLev;

            _currentLocationLevel = getLocationLevel(_locationLevel);
            _filledDataEle = filled;

            if (_locationLevel == 0)
            {
                _expectedDataElements = expectData;
            }
            else
            {
                setExpectedDataElements();
            }
            calculateCompletness();


        }



        public SavedReports(string locID, int locaLev, int repType, int mth, int qua, decimal yr, int filled, RecivedMethod recMethod, ReportStatus status)
        {
            _sourceLocationID = locID;
            _month = mth;
            _quarter = qua;
            _year = Convert.ToInt32(yr);
            _reportType = repType;
            _reportName = this.getReportName(repType);
            _reportStatus = status;
            _recivedMethod = recMethod;
            _locationLevel = locaLev;
            _currentLocationLevel = getLocationLevel(_locationLevel);
            _filledDataEle = filled;
            setExpectedDataElements();
            calculateCompletness();



        }





        public SavedReports(string locID, int locaLev, int repType, int mth, int qua, decimal yr, int expected, int recived, decimal completness, RecivedMethod recMethod, ReportStatus status)
        {
            _sourceLocationID = locID;
            _month = mth;
            _quarter = qua;
            _year = Convert.ToInt32(yr);
            _reportType = repType;
            _reportName = this.getReportName(repType);
            _reportStatus = status;
            _recivedMethod = recMethod;
            _locationLevel = locaLev;
            _currentLocationLevel = getLocationLevel(_locationLevel);
            _filledDataEle = recived;
            _expectedDataElements = expected;
            _filledDataEle = recived;
            _completness = completness;


        }




        private void calculateCompletness()
        {

            if (_expectedDataElements > 0)
            {
                _completness = Math.Round(((decimal)_filledDataEle / _expectedDataElements) * 100, 1);
            }
            else
            {
                _completness = 100;
            }

        }
       

        private bool recivedReportType(string reportType)
        {

            string myReportType = _reportType.ToString();
            int length1 = myReportType.Length;
            int length2 = reportType.Length;
            return (reportType[length2 - 1] == myReportType[length1 - 1]);

        }




        private void setExpectedDataElements()
        {

            string seleSt = "SELECT [ExpectedDataElements] FROM [dbo].[EthEhmis_HMISReportType] " +
                            " where [ReportID] = " + _reportType.ToString();

            DataTable dt = DBConnHelper.GetDataSet(seleSt).Tables[0];
            _expectedDataElements = 0;
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["ExpectedDataElements"] != DBNull.Value)
                {
                    _expectedDataElements = Convert.ToInt32(dt.Rows[0]["ExpectedDataElements"]);
                }

            }


            //if (recivedReportType("4"))
            //{
            //    if ((_month != 1) & (_month != 4) & (_month != 7) & (_month != 10))
            //    {
            //        _expectedDataElements = _expectedDataElements;
            //    }


            //}
            //switch (_reportType)
            //{

            //    case 44:
            //    case 34:
            //    case 24:
            //    case 14: _expectedDataElements = 1351; break;
            //    case 45:
            //    case 35:
            //    case 25:
            //    case 15: _expectedDataElements = 420; break;
            //    default:
            //        {
            //            if (recivedReportType("4"))
            //            {
            //                // annaul datat entry form

            //                switch (_currentLocationLevel)
            //                {
            //                    case HMISMainPage.locationLevel.facility: switch (HMISMainPage.SelectedLocationType)
            //                        {
            //                            case 1:
            //                            case 2: _expectedDataElements = 193; break;
            //                            case 3: _expectedDataElements = 80; break;
            //                        } break;
            //                    default: _expectedDataElements = 15; break;
            //                }


            //            }
            //            else if (recivedReportType("2"))
            //            {

            //                switch (HMISMainPage.SelectedLocationType)
            //                {
            //                    case 1:
            //                    case 2: _expectedDataElements = 621; break;
            //                    case 3: _expectedDataElements = 147; break;
            //                    default: _expectedDataElements = 1599; break;
            //                }


            //            }
            //            else if (recivedReportType("3"))
            //            {


            //                switch (HMISMainPage.SelectedLocationType)
            //                {
            //                    case 1:
            //                    case 2: _expectedDataElements = 1256; break;
            //                    case 3: _expectedDataElements = 147; break;
            //                    default: _expectedDataElements = 1887; break;
            //                }
            //            }
            //            else if (recivedReportType("5"))
            //            {

            //                switch (_currentLocationLevel)
            //                {
            //                    case HMISMainPage.locationLevel.facility: switch (HMISMainPage.SelectedLocationType)
            //                        {
            //                            case 1:
            //                            case 2: _expectedDataElements = 60; break;
            //                            case 3: _expectedDataElements = 36; break;
            //                        } break;
            //                    default: _expectedDataElements = 65; break;
            //                }



            //            }

            //        }break;
            //} 

        }


        public bool existsInDataBase()
        {
            string seleStatment = "";

            if (_reportType == 160)
            {
                seleStatment = "Select [ID] from [EthEhmis_HMISReportStatus] where " +
                       "[ReportType]=" + _reportType + " and " +
                       "[Location]='" + _sourceLocationID + "'" + " and " +
                       "[ReportYear]=" + _year + " and " +
                       "[ReportQuarter]=" + _quarter + " and " +
                       "[ReportMonth]=" + _month + " and " +
                       "[ReportWeek]=" + _week + " and " +
                       "[Status] = '" + _reportStatus.ToString() + "'";
            }
            else
            {
                seleStatment = "Select [ID] from [EthEhmis_HMISReportStatus] where " +
                      "[ReportType]=" + _reportType + " and " +
                      "[Location]='" + _sourceLocationID + "'" + " and " +
                      "[ReportYear]=" + _year + " and " +
                       "[ReportQuarter]=" + _quarter + " and " +
                      "[ReportMonth]=" + _month + " and " +
                      "[Status] = '" + _reportStatus.ToString() + "'";
            }

            SqlCommand cmd = new SqlCommand(seleStatment);

            DataSet KeyAcDs = DBConnHelper.GetDataSet(cmd);

            if (KeyAcDs.Tables[0].Rows.Count == 0)
            {

                return false;
            }
            _dbID = Convert.ToString(KeyAcDs.Tables[0].Rows[0]["ID"]);
            return true;
        }

        public bool existsInDataBaseForMonthWithInQuarter()
        {
            if (_quarter == 0)
                return false;

            string seleStatment = "Select [ID] from [EthEhmis_HMISReportStatus] where " +                   
                   "[Location]='" + _sourceLocationID + "'" + " and " +
                   getMonthsWithInQuarter(_quarter,_year) +
                   "[Status] = '" + _reportStatus.ToString() + "'";
            SqlCommand cmd = new SqlCommand();

            DataSet KeyAcDs = DBConnHelper.GetDataSet(seleStatment);

            if (KeyAcDs.Tables[0].Rows.Count == 0)
            {

                return false;
            }
            _dbID = Convert.ToString(KeyAcDs.Tables[0].Rows[0]["ID"]);
            return true;
        }

        private string getMonthsWithInQuarter(int quarter,int year)
        {
            string months = "";

            switch (quarter)
            {
                case 1: months = "11,12,1"; break;
                case 2: months = "2,3,4"; break;
                case 3: months = "5,6,7"; break;
                case 4: months = "8,9,10"; break;
                default: months = ""; break;
            }
            
            return getWhereClose(months, year, quarter);
        }

        private string getWhereClose(string months, int year,int quarter)
        {
            string strWhere = "";
            string[] m = months.Split(',');
            int preYear = year - 1;
            switch (quarter)
            {
                case 1: strWhere = " [ReportYear]=" + preYear.ToString() + " AND ([ReportMonth] = " + m[0] + " OR [ReportMonth] = " + m[1] + " OR [ReportMonth] = " + m[2] + ") AND "; break;
                default: strWhere = " [ReportYear]=" + year.ToString() + " AND ([ReportMonth] = " + m[0] + " OR [ReportMonth] = " + m[1] + " OR [ReportMonth] = " + m[2] + ") AND "; break;
            }

            return strWhere;
        }

        public bool updateReportStatus()
        {
            SqlCommand cmd = new SqlCommand();
            if (existsInDataBase())
            {
                cmd.CommandText = "delete from  [EthEhmis_HMISReportStatus]" +
                               "  WHERE  [ID] =  " + _dbID;

                cmd.CommandType = CommandType.Text;
                int rowsAffected = DBConnHelper.Execute(cmd);


            }

            cmd.CommandText = " INSERT INTO [EthEhmis_HMISReportStatus]" +
                             "([ReportType],[Status],[Location] ,[ReportMonth],[ReportQuarter]" +
                             ",[ReportYear],ExpectedDataElements,FilledDataElements,Method,ReportName,[Completness],[ReportWeek]) VALUES (" + _reportType +
                            ",'" + _reportStatus.ToString() + "','" + _sourceLocationID + "'," +
                             _month + "," + _quarter + "," + _year + "," + _expectedDataElements + ","
                              + _filledDataEle + ",'" + _recivedMethod.ToString() + "','" + _reportName.ToString() + "'," + _completness.ToString() + "," + _week.ToString() + ")";

            cmd.CommandType = CommandType.Text;

            try
            {
                int rowsAffected = DBConnHelper.Execute(cmd);
            }
            catch
            {
                return false;
            }
            return true;


        }



        public ReportName getReportName(int reportCode)
        {
            ReportName reportName = ReportName.UNKNOWN;
            switch (reportCode)
            {
                case 662: reportName = ReportName.HealthPostMONTHLYOPDReport; break;
                case 663: reportName = ReportName.HealthPostMONTHLYIPDReport; break;
                case 664: reportName = ReportName.HealthPostMONTHLYServiceReport; break;
                case 62: reportName = ReportName.HealthPostQUARTERLYOPDReport; break;
                case 63: reportName = ReportName.HealthPostQUARTERLYIPDReport; break;
                case 64: reportName = ReportName.HealthPostQUARTERLYServiceReport; break;
                case 65: reportName = ReportName.HealthPostANNUALServiceReport; break;
                case 82: reportName = ReportName.FacilityMONTHLYOPDReport; break;
                case 83: reportName = ReportName.FacilityMONTHLYIPDReport; break;
                case 84: reportName = ReportName.FacilityMONTHLYServiceReport; break;
                case 85: reportName = ReportName.FacilityMONTHLYServiceAgrigatedReport; break;
                case 86: reportName = ReportName.FacilityQUARTERLYServiceAgrigatedReport; break;
                case 87: reportName = ReportName.FacilityHCMONTHLYOPDAggregateReport; break;
                case 88: reportName = ReportName.FacilityHCQUARTERLYOPDAggregateReport; break;
                case 2: reportName = ReportName.FacilityQUARTERLYOPDReport; break;
                case 3: reportName = ReportName.FacilityQUARTERLYIPDReport; break;
                case 4: reportName = ReportName.FacilityQUARTERLYServiceReport; break;
                case 5: reportName = ReportName.FacilityANNUALServiceReport; break;
                case 22: reportName = ReportName.WHOQUARTERLYOPDReport; break;
                case 23: reportName = ReportName.WHOQUARTERLYIPDReport; break;
                case 24: reportName = ReportName.WHOQUARTERLYServiceReport; break;
                case 25: reportName = ReportName.WHOANNUALServiceReport; break;
                case 32: reportName = ReportName.ZHDQUARTERLYOPDReport; break;
                case 33: reportName = ReportName.ZHDQUARTERLYIPDReport; break;
                case 34: reportName = ReportName.ZHDQUARTERLYServiceReport; break;
                case 35: reportName = ReportName.ZHDANNUALServiceReport; break;
                case 42: reportName = ReportName.RHBQUARTERLYOPDReport; break;
                case 43: reportName = ReportName.RHBQUARTERLYIPDReport; break;
                case 44: reportName = ReportName.RHBQUARTERLYServiceReport; break;
                case 45: reportName = ReportName.RHBANNUALServiceReport; break;
                case 52: reportName = ReportName.FMOHQUARTERLYOPDReport; break;
                case 53: reportName = ReportName.FMOHQUARTERLYIPDReport; break;
                case 54: reportName = ReportName.FMOHQUARTERLYServiceReport; break;
                case 55: reportName = ReportName.FMOHANNUALServiceReport; break;
                case 622: reportName = ReportName.WHOMONTHLYOPDReport; break;
                case 623: reportName = ReportName.WHOMONTHLYIPDReport; break;
                case 624: reportName = ReportName.WHOMONTHLYServiceReport; break;
                case 632: reportName = ReportName.ZHDMONTHLYOPDReport; break;
                case 633: reportName = ReportName.ZHDMONTHLYIPDReport; break;
                case 634: reportName = ReportName.ZHDMONTHLYServiceReport; break;
                case 642: reportName = ReportName.RHBMONTHLYOPDReport; break;
                case 643: reportName = ReportName.RHBMONTHLYIPDReport; break;
                case 644: reportName = ReportName.RHBMONTHLYServiceReport; break;
                case 652: reportName = ReportName.FMOHMONTHLYOPDReport; break;
                case 653: reportName = ReportName.FMOHMONTHLYIPDReport; break;
                case 654: reportName = ReportName.FMOHMONTHLYServiceReport; break;
                case 888: reportName = ReportName.PHEMCasebasedAFP; break;
                case 555: reportName = ReportName.PHEMCasebasedLAB; break;
                case 777: reportName = ReportName.PHEMCasebasedNNT; break;
                case 999 : reportName = ReportName.PHEMCasebasedCBR; break;
                case 1: reportName = ReportName.WeeklyIDSReport; break;
            }
            return reportName;
        }


    }


}
