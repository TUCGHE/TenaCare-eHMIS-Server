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
    public class DiseaseOptions
    {
        public string caseDeath { get; set; }        
        public bool female { get; set; }
        public bool male { get; set; }
        public bool zeroToFour { get; set; }
        public bool fiveToFourteen { get; set; }
        public bool fifteenAndAbove { get; set; }
        public bool opd { get; set; }
        public bool ipd { get; set; }
        public bool per1000Population { get; set; }
        public string populationMultiplier { get; set; }
    }
}