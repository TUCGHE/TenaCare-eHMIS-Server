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
using System.Collections;

namespace eHMISWebApi.Models
{
public class PeriodSelect
    {
        public int StartFiscalYear { get; set; }
        public int EndFiscalYear { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int StartQuarter { get; set; }
        public int EndQuarter { get; set; }
        // for ranges
        public int StartYearStartMonth { get; set; }
        public int StartYearEndMonth { get; set; }
        public int EndYearStartMonth { get; set; }
        public int EndYearEndMonth { get; set; }

        // ranges: yr-quarterly
        public int StartYearStartQuarter { get; set; }
        public int StartYearEndQuarter { get; set; }
        public int EndYearStartQuarter { get; set; }
        public int EndYearEndQuarter { get; set; }
        public bool aggregate { get; set; }
        public bool aggregateAll { get; set; }
    }
}