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
using System.Web;

namespace eHMISWebApi.Models
{
    public class CustomReportParameters
    {

        public string Period { get; set; }
        public string ReportType { get; set; }
        public int FiscalYear { get; set; }

        public int StartPeriod { get; set; }
        public int EndPeriod { get; set; }
        public object Location { get; set; }
        public bool showOnlyQDE { get; set; }
        public bool isCacheEnabled { get; set; }
        public bool showNumDenom { get; set; }
        public bool showTarget { get; set; }
       

        //var reportParameters = {"Period": $scope.Period, "ReportType": $scope.ReportType,
        //        "FiscalYear": $scope.FiscalYear, "StartMonth": $scope.StartMonth, "EndMonth": $scope.EndMonth,
        //        "Location": $scope.checkedFacilities

    }
}