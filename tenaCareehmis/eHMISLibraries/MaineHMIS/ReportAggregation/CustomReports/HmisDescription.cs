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
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Collections;

namespace eHMIS.HMIS.ReportAggregation.CustomReports
{
    class HmisDescription
    {
        public string LocationID = string.Empty;
        public int dataeleclass = 0;
        public int RegionId = 0;
        public int ZoneId = 0;
        public int WoredaId = 0;
        public int Month = 0;
        public int Year = 0;
        //public ArrayList LabelIds = new ArrayList();
        //public Hashtable labelIdValue = new Hashtable();
        private Dictionary<int, decimal> _labelIdValues  = new Dictionary<int, decimal>();

        public Dictionary<int, decimal> LabelIdValues
        {
            get
            {
                return _labelIdValues;
            }

            set
            {
                _labelIdValues = value;
            }
        }
    }
}
