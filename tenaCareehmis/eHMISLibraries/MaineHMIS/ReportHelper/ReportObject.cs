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

namespace eHMIS.HMIS.ReportHelper
{

    [Serializable]
    public class reportObject
    {
        public enum ReportGlobalType
        {
            facility = 0,
            woreda = 1,
            zone = 2,
            region = 3,
            federal = 4,
            HealthPost = 5,
            HealthCenter = 6
        }

        public enum ReportKind
        {
            OPD_Disease = 0,
            IPD_Disease = 1,
            OPD_Disease_Facility_Quarterly = 2,
            IPD_Disease_Facility_Quarterly = 3,
            OPD_Disease_Facility_Monthly = 4,
            IPD_Disease_Facility_Monthly = 5,
            Annual_Service = 6,
            Quarterly_Service = 7,
            Monthly_Service = 8,
            Weekly_Disease = 9,
            Case_List = 10,
            OPD_HC_Aggregate_Monthly = 11,
            OPD_HC_Aggregate_Quarterly = 12,
            OPD_Disease_Monthly = 13,
            OPD_Disease_Quarterly = 14,
            IPD_Disease_Monthly = 15,
            IPD_Disease_Quarterly = 16
        }

        string _name;
        string _facility;
        decimal _year;
        int _startQuarter;
        int _endQuarter;
        int _startWeek;
        int _endWeek;

        int _startMonth;
        int _endMonth;
        int _day;
        string _reportPath;

        string _reportProc;

        string[] _reportPara;

        int _reportType;
        int _reportSource; // 0 - HMIS value table, 1 - Patient interaction tables 

        int _reportStatus; // 0 - ready for geneate - 1 - generated , 2 - exported, 3 - posted
        string _locationHMISCode;
        string _exportedFileName;
        string _reportFileString;
        int _selectedlocationLevel;
        int _selectedlocationType;
        string _reportingHC; // Only for Health Posts

        int _regionID;
        int _zoneID;
        int _woredaID;
        decimal _completness;
        int _filled;
        int _expected;
        string _reportTableName;
        string _viewLabeIdTableName;

        ReportGlobalType _repGlobalType;
        ReportKind _repKind;

        private List<string> _expectedInstId = new List<string>();
        private List<string> _missingInstId = new List<string>();
        private List<string> _submittingInstId = new List<string>();
       
        private bool _isShowOnlyQuartDataElement;
        public bool IsShowOnlyQuartDataElement
        {
            get { return _isShowOnlyQuartDataElement; }
            set { _isShowOnlyQuartDataElement = value; }
        }

        private bool _isCacheEnabled;
        public bool IsCacheEnabled
        {
            get { return _isCacheEnabled; }
            set { _isCacheEnabled = value; }
        }


        public List<string> ExpectedInstId
        {
            get { return _expectedInstId; }
            set { _expectedInstId = value; }
        }


        public List<string> MissingInstId
        {
            get { return _missingInstId; }
            set { _missingInstId = value; }
        }

        public decimal Completness
        {
            get { return _completness; }
            set { _completness = value; }
        }

        public int Expected
        {
            get { return _expected; }
            set { _expected = value; }
        }

        public int Filled
        {
            get { return _filled; }
            set { _filled = value; }
        }

        public List<string> SubmittingInstId
        {
            get { return _submittingInstId; }
            set { _submittingInstId = value; }
        }


        public string LocationHMISCode
        {
            get { return _locationHMISCode; }
            set { _locationHMISCode = value; }
        }


        public string ReportFileString
        {
            get { return _reportFileString; }
            set { _reportFileString = value; }
        }


        public int RegionID
        {
            get { return _regionID; }
            set { _regionID = value; }
        }

        public int ZoneID
        {
            get { return _zoneID; }
            set { _zoneID = value; }
        }

        public int WoredaID
        {
            get { return _woredaID; }
            set { _woredaID = value; }
        }

        public int ReportType
        {
            get { return _reportType; }
            set { _reportType = value; }
        }

        public int ReportSource
        {
            get { return _reportSource; }
            set { _reportSource = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public decimal Year
        {
            get { return _year; }
            set { _year = value; }
        }
        public int StartQuarter
        {
            get { return _startQuarter; }
            set { _startQuarter = value; }
        }
        public int EndQuarter
        {
            get { return _endQuarter; }
            set { _endQuarter = value; }
        }

        public int StartWeek
        {
            get { return _startWeek; }
            set { _startWeek = value; }
        }

        public int EndWeek
        {
            get { return _endWeek; }
            set { _endWeek = value; }
        }


        public int Day
        {
            get { return _day; }
            set { _day = value; }
        }
        public string Facility
        {
            get { return _facility; }
            set { _facility = value; }
        }

        public string ReportPath
        {
            get { return _reportPath; }
            set { _reportPath = value; }
        }

        public string ExportedFileName
        {
            get { return _exportedFileName; }
            set { _exportedFileName = value; }
        }

        public string ReportProc
        {
            get { return _reportProc; }
            set { _reportProc = value; }
        }


        public string[] ReportPara
        {
            get { return _reportPara; }
            set { _reportPara = value; }
        }

        public int ReportStatus
        {
            get { return _reportStatus; }
            set { _reportStatus = value; }
        }


        public decimal ReportCompletness
        {
            get { return _completness; }
            set { _completness = value; }
        }


        public int StartMonth
        {
            get { return _startMonth; }
            set { _startMonth = value; }
        }

        public int EndMonth
        {
            get { return _endMonth; }
            set { _endMonth = value; }
        }

        public int SelectedLocationLevel
        {
            get { return _selectedlocationLevel; }
            set { _selectedlocationLevel = value; }
        }


        public int SelectedLocationType
        {
            get { return _selectedlocationType; }
            set { _selectedlocationType = value; }
        }

        // Merra Kokebie, merraK@tutape.org, Jan 10, 2011 Added for the New Report Aggregation
        // 
        public string ReportTableName
        {
            get { return _reportTableName; }
            set { _reportTableName = value; }
        }

        public string ViewLabelIdTableName
        {
            get { return _viewLabeIdTableName; }
            set { _viewLabeIdTableName = value; }
        }

        public ReportGlobalType RepGlobalType
        {
            get { return _repGlobalType; }
            set { _repGlobalType = value; }
        }

        public ReportKind RepKind
        {
            get { return _repKind; }
            set { _repKind = value; }
        }

        public string ReportingHC
        {
            get { return _reportingHC; }
            set { _reportingHC = value; }
        }

    }

}
