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

namespace UtilitiesNew.GeneralUtilities
{
    public class Constants
    {
        //2007-01-24 @JJ: pulled this out so I could move eHMIS.Entity out of the CCPFramework project.
        public static DateTime NULLDATE = new DateTime(1900, 1, 1);

        #region Setting Properties

        public static readonly string DONT_USE_FLAT_TABLES = "TurnOffFlatTables";
        public static readonly string DONT_USE_CUSTOM_TABLES = "TurnOffCustomTables";

        #endregion

        
        #region Property types

        public static readonly string AUXILIARY_ID = "AuxiliaryId";
        public static readonly string PROBLEM = "Problem";
        public static readonly string IGNORE_ON_INTEGRITY_CHECK = "IgnoreOnIntegrityCheck";
        public static readonly string PERIOD_DRIVEN_INDICATOR = "PeriodDrivenIndicator";
        public static readonly string REFERRAL = "Referral";

        #endregion

        #region Patient summary concepts

        public static readonly string WHO_STAGE_TODAY = "who_stage_today";
        public static readonly string CD4_COUNT = "cd4_count";
        public static readonly string HISTORICAL_CD4_COUNT = "historical_cd4_count";
        public static readonly string HISTORICAL_CD4_COUNT_DATE = "historical_cd4_count_date";
        public static readonly string HISTORICAL_CD4_COUNT_LAB = "historical_cd4_count_lab";
        public static readonly string LAB_RESULTS_DATE = "lab_results_date";
        public static readonly string TT_PROTECTED = "pregnancy_tt_protected";
        public static readonly string MED_ALLERGIES = "med_allergies";        

        
        #endregion
        
        #region AuxId types

        public static readonly string DONOR_ID = "donor_id";

        #endregion

        #region Config Keys

        public static string MOBILE_SITE_CONFIG_KEY = "IsMobileSite";

        #endregion
    }
}
